using Microsoft.Data.Sqlite;
using Stock.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stock
{
    public partial class Box : Form
    {
        private List<long> _manifestIds;
        private List<ItemManifest> _itemsToCount;
        public Box(List<long> _manifestIds)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this._manifestIds = _manifestIds;
            _itemsToCount = new List<ItemManifest>();
            LoadData();
            SetupUI();
        }

        public void LoadData()
        {

            try
            {
                _itemsToCount.Clear();

                string idsString = string.Join(", ", _manifestIds);

                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=association.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT Code, Description ,  ExpectedQuantity , CountedQuantity  FROM ManifestItem WHERE ManifestId IN ({idsString})";
                    
                    List<ItemManifest> rawList = new List<ItemManifest>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            var qty = reader.GetDouble(2);
                            double counted = reader.GetDouble(3);

                            rawList.Add(new ItemManifest
                            {
                                Code = reader.GetString(0),
                                Description = reader.GetString(1),
                                Quantity = qty,
                                ExpectedQuantity = reader.GetDouble(2),
                                CountedQuantity = counted
                            });
                        }
                    }


                    _itemsToCount = rawList.GroupBy(i => i.Code)
                        .Select(group => new ItemManifest
                        {
                            Code = group.Key,
                            Description = group.First().Description,
                            Quantity = group.Sum(i => i.ExpectedQuantity),
                            ExpectedQuantity = group.Sum(i => i.ExpectedQuantity),
                            CountedQuantity = group.Sum(i => i.CountedQuantity)
                        }).ToList();
                }

                // Força a atualização da grade
                dgvItems.DataSource = null;
                dgvItems.DataSource = _itemsToCount;


                SortAndRefreshGrid();
                UpdateTopLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message);

            }
        }

        private void UpdateTopLabels()
        {
            double total = _itemsToCount.Sum(x => x.ExpectedQuantity);
            double counted = _itemsToCount.Sum(x => x.CountedQuantity);
            double remaining = total - counted;

            lblTotalItems.Text = total.ToString();       // Blue Label
            lblCountedItems.Text = counted.ToString();   // Green Label
            lblRemainingItems.Text = remaining.ToString(); // Red Label
        }



        private void RefreshGrid()
        {
            dgvItems.DataSource = null;
            dgvItems.DataSource = _itemsToCount;

            if (dgvItems.Columns["Code"] != null)
            {
                // Define quais colunas aparecem e seus nomes
                dgvItems.Columns["Code"].HeaderText = "Codigo";
                dgvItems.Columns["Description"].HeaderText = "Description";

                // COLUNA 1: QUANTIDADE TOTAL (DO MANIFESTO)
                dgvItems.Columns["Quantity"].Visible = true;
                dgvItems.Columns["Quantity"].HeaderText = "Total Qty";

                // COLUNA 2: QUANTIDADE JÁ CONTADA (BIPADA)
                dgvItems.Columns["CountedQuantity"].Visible = true;
                dgvItems.Columns["CountedQuantity"].HeaderText = "Contados";

                // ESCONDE O STATUS ANTIGO E A COLUNA REPETIDA
                if (dgvItems.Columns["Status"] != null) dgvItems.Columns["Status"].Visible = false;
                if (dgvItems.Columns["ExpectedQuantity"] != null) dgvItems.Columns["ExpectedQuantity"].Visible = false;
            }
        }

        public void SetupUI()
        {
            // Coloque um em cima do outro no Design
            txtQuantity.Visible = false;
            txtProductCode.Visible = true;
            txtProductCode.Focus();
            this.Activated += (s, e) => txtProductCode.Focus();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblFaltandoValue_Click(object sender, EventArgs e)
        {

        }

        private void lblConferidoValue_Click(object sender, EventArgs e)
        {

        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string codigoLido = txtProductCode.Text.Trim();
                
                var row = dgvItems.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => r.Cells["Code"].Value.ToString() == codigoLido);

                if(row != null)
                {
                    string description = row.Cells["Description"].Value.ToString();
                    txtDescription.Text = description;
                    txtDescription.ForeColor = Color.DarkBlue;

                    dgvItems.ClearSelection();
                    row.Selected = true;
                    dgvItems.FirstDisplayedScrollingRowIndex = row.Index;

                    txtProductCode.Visible = false;
                    txtQuantity.Visible = true;
                    txtQuantity.Focus();
                }
                else
                {
                    MessageBox.Show("Produto não encontrado no manifesto.");
                    txtProductCode.Clear();
                }
            }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (double.TryParse(txtQuantity.Text, out double qty))
                {
                    string barcode = txtProductCode.Text.Trim();
                    var item = _itemsToCount.FirstOrDefault(x => x.Code == barcode);

                    if (item != null)
                    {
                        // 1. Atualiza na memória (Lista)
                        item.CountedQuantity += qty;

                        // 2. SALVA NO BANCO (SQLite) IMEDIATAMENTE
                        try
                        {
                            using (var connection = new SqliteConnection("Data Source=association.db"))
                            {
                                connection.Open();
                                var command = connection.CreateCommand();

                                // Atualizamos apenas o CountedQuantity para este item e este manifesto
                                command.CommandText = @"
                                    UPDATE ManifestItem 
                                    SET CountedQuantity = @counted 
                                    WHERE Code = @code AND ManifestId IN (" + string.Join(",", _manifestIds) + ")";

                                command.Parameters.AddWithValue("@counted", item.CountedQuantity);
                                command.Parameters.AddWithValue("@code", item.Code);

                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao salvar no banco: " + ex.Message);
                        }

                        // 3. Organiza a grade (Manda para o fim se terminou e pinta de vermelho)
                        SortAndRefreshGrid();
                        UpdateTopLabels();

                        // 4. Reseta os campos para o próximo bip
                        txtQuantity.Clear();
                        txtQuantity.Visible = false;
                        txtProductCode.Clear();
                        txtProductCode.Visible = true;
                        txtProductCode.Focus();
                    }
                }
            }
        }


        private void SortAndRefreshGrid()
        {
            // Ordena: Primeiro os que faltam (Counted < Expected), depois os concluídos/sobras
            _itemsToCount = _itemsToCount
                .OrderBy(x => x.CountedQuantity >= x.ExpectedQuantity ? 1 : 0)
                .ThenBy(x => x.Description)
                .ToList();

            dgvItems.DataSource = null;
            dgvItems.DataSource = _itemsToCount;

            // Configura as colunas novamente (necessário ao dar DataSource = null)
            RefreshGrid();
        }

    
        private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var item = (ItemManifest)dgvItems.Rows[e.RowIndex].DataBoundItem;

            // Se o contado for igual ou maior que o esperado, a linha fica vermelha
            if (item.CountedQuantity >= item.ExpectedQuantity)
            {
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.DarkRed; // Destaque quando selecionado
            }
            else
            {
                // Volta ao padrão se não estiver completo
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }


    }

}