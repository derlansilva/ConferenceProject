using Microsoft.Data.Sqlite;
using Stock.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stock
{
    public partial class MenuBoxConferences : Form
    {

        string user;
        public MenuBoxConferences(string username)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.user = username;
        }




        private async void Conferences_Load(object sender, EventArgs e)
        {
            List<ManifestSelection> manifests = new List<ManifestSelection>();

            using (var connection = new SqliteConnection("Data Source=AGB_v2.db"))
            {
                await connection.OpenAsync();
                var selectCmd = connection.CreateCommand();


                selectCmd.CommandText = "SELECT Id, ManifestNumber, CreatedAt FROM Manifest WHERE Status = 0 ORDER BY Id DESC";

                using (var reader = await selectCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        manifests.Add(new ManifestSelection
                        {
                            IsSelected = false,
                            Id = reader.GetInt64(0),
                            ManifestNumber = reader.GetString(1),
                            CreatedAt = reader.GetString(2)
                        });
                    }
                }
            }

            if (manifests.Count == 0)
            {
                MessageBox.Show("Nenhum manifesto criado.", "Aviso");
                // Se não tem nada, vinculamos a lista vazia e saímos do método
                dgvManifests.DataSource = manifests;
                return;
            }
            ApplySapThemeToSelection();

            dgvManifests.DataSource = manifests;


            if (dgvManifests.Columns["Id"] != null) dgvManifests.Columns["Id"].Visible = false;
            dgvManifests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void dgvManifests_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            // Verifica se a coluna alterada foi a do Checkbox
            if (e.RowIndex >= 0 && dgvManifests.Columns[e.ColumnIndex].DataPropertyName == "IsSelected")
            {
                if (dgvManifests.DataSource is List<ManifestSelection> list)
                {
                    // Ativa o botão se pelo menos um estiver marcado
                    btnIniciarConferencia.Visible = list.Any(x => x.IsSelected);
                    btnInicarConferenciaUnidades.Visible = list.Any(x => x.IsSelected);
                }
            }
        }





        private void btnIniciarConferencia_Click_1(object sender, EventArgs e)
        {
            if (dgvManifests.DataSource is List<ManifestSelection> list)
            {
                List<long> idsSelecionados = list
                    .Where(x => x.IsSelected)
                    .Select(x => x.Id)
                    .ToList();

                if (idsSelecionados.Count > 0)
                {
                    Box tela = new Box(idsSelecionados, this.user);

                    tela.StartPosition = FormStartPosition.CenterScreen;

                    this.Hide();
                    tela.ShowDialog();

                    tela.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, selecione ao menos um manifesto na lista.", "Aviso");
                }
            }
        }

        private void ApplySapThemeToSelection()
        {
            // 1. LIMPEZA E ESTRUTURA
            if (dgvManifests.Columns["Id"] != null) dgvManifests.Columns["Id"].Visible = false;
            if (dgvManifests.Columns["Status"] != null) dgvManifests.Columns["Status"].Visible = false;

            // Configurações de Grade (Igual ao SAP)
            dgvManifests.BackgroundColor = System.Drawing.Color.White;
            dgvManifests.BorderStyle = BorderStyle.Fixed3D;
            dgvManifests.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvManifests.GridColor = System.Drawing.Color.FromArgb(200, 200, 200); // Linhas cinzas finas

            // MOSTRAR A COLUNA DE NÚMERO (#) NA ESQUERDA
            dgvManifests.RowHeadersVisible = true;
            dgvManifests.RowHeadersWidth = 35;
            dgvManifests.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvManifests.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 235, 235);

            // 2. CABEÇALHO (O Cinza Prateado do SAP)
            dgvManifests.EnableHeadersVisualStyles = false;
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
            headerStyle.ForeColor = System.Drawing.Color.Black;
            headerStyle.Font = new Font("Tahoma", 8.5f, FontStyle.Regular);
            dgvManifests.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvManifests.ColumnHeadersHeight = 25;
            dgvManifests.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 3. ESTILO DAS LINHAS (Sem seleção azul ou amarela)
            dgvManifests.DefaultCellStyle.Font = new Font("Tahoma", 8);
            dgvManifests.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvManifests.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvManifests.RowTemplate.Height = 22;

            // Mata o destaque de seleção (fica invisível como você pediu nas outras telas)
            dgvManifests.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            dgvManifests.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            // 4. NOMES DAS COLUNAS
            if (dgvManifests.Columns["IsSelected"] != null) dgvManifests.Columns["Selectecione"].HeaderText = "Selecionar";
            if (dgvManifests.Columns["ManifestNumber"] != null) dgvManifests.Columns["Nº Manifesto"].HeaderText = "Nº do documento";
            if (dgvManifests.Columns["CreatedAt"] != null) dgvManifests.Columns["Criado em "].HeaderText = "Data de lançamento";

            // 5. AJUSTE DE LARGURA
            dgvManifests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dgvManifests.Columns["IsSelected"] != null) dgvManifests.Columns["IsSelected"].FillWeight = 20;

            // 6. NUMERAÇÃO AUTOMÁTICA (1, 2, 3...) NA LATERAL
            dgvManifests.RowPostPaint += (s, e) =>
            {
                var grid = s as DataGridView;
                var rowIdx = (e.RowIndex + 1).ToString();
                var centerFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, grid.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            };

            // Limpa qualquer seleção inicial
            dgvManifests.ClearSelection();
            dgvManifests.CurrentCell = null;
        }
        private void dgvManifests_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            if (dgvManifests.IsCurrentCellDirty)
            {
                // Isso força o disparo do CellValueChanged na hora do clique
                dgvManifests.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnInicarConferenciaUnidades_Click(object sender, EventArgs e)
        {
            if(dgvManifests.DataSource is List<ManifestSelection> list)
            {
                List<long> idsSelections = list
                    .Where(x => x.IsSelected)
                    .Select(x => x.Id)
                    .ToList();

                if(idsSelections.Count > 0)
                {
                    ConferenceUnit conference = new ConferenceUnit(idsSelections, this.user);
                    conference.StartPosition = FormStartPosition.CenterScreen;

                    this.Hide();
                    conference.ShowDialog();

                    conference.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, selecione ao menos um manifesto na lista.", "Aviso");
                }
            }
        }
    }
}
