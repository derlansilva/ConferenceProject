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
        private List<ItemManifest> _printQueue = new List<ItemManifest>();
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

                string idsString = string.Join(",", _manifestIds);

                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=estoqueAGB.db"))
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

            ApplySapTheme();

            // Isso aqui limpa o azul do carregamento
            dgvItems.ClearSelection();

            // Garante que o clique não "pinte" nada de azul depois
            dgvItems.CurrentCell = null;
        }

        public void SetupUI()
        {
            // Coloque um em cima do outro no Design
            txtQuantity.Visible = false;
            txtProductCode.Visible = true;
            txtProductCode.Focus();
            this.Activated += (s, e) => txtProductCode.Focus();

        }

        private void ApplySapTheme()
        {
            // 1. ESCONDER O QUE NÃO PRECISA
            if (dgvItems.Columns["ExpectedQuantity"] != null) dgvItems.Columns["ExpectedQuantity"].Visible = false;
            if (dgvItems.Columns["Status"] != null) dgvItems.Columns["Status"].Visible = false;
            dgvItems.RowHeadersVisible = false; // Tira a seta da esquerda

            // 2. CONFIGURAÇÃO DE BORDAS E CORES GERAIS
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvItems.GridColor = Color.FromArgb(200, 200, 200); // Linhas da grade cinzas

            // Desativa a seleção visual (o clique não faz nada)
            dgvItems.ReadOnly = true;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 3. CABEÇALHO (O Cinza do SAP)
            dgvItems.EnableHeadersVisualStyles = false;
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = Color.FromArgb(225, 225, 225); // Cinza SAP
            headerStyle.ForeColor = Color.Black;
            headerStyle.Font = new Font("Tahoma", 8.5f, FontStyle.Regular);
            dgvItems.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvItems.ColumnHeadersHeight = 22;
            dgvItems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 4. NOMES DAS COLUNAS
            dgvItems.Columns["Code"].HeaderText = "Nº do item";
            dgvItems.Columns["Description"].HeaderText = "Descrição do item";
            dgvItems.Columns["Quantity"].HeaderText = "Qtd. Total";
            dgvItems.Columns["CountedQuantity"].HeaderText = "Contados";

            // 5. AJUSTE DE LARGURA (Ocupar a tela toda)
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.Columns["Code"].FillWeight = 20;
            dgvItems.Columns["Description"].FillWeight = 50;
            dgvItems.Columns["Quantity"].FillWeight = 15;
            dgvItems.Columns["CountedQuantity"].FillWeight = 15;
        }


        // 4. LÓGICA DAS CORES (Azul no Bipado)
        private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var item = (ItemManifest)dgvItems.Rows[e.RowIndex].DataBoundItem;

            // Se o item foi bipado (Contados > 0)
            if (item.CountedQuantity > 0)
            {
                // Azul "Bipado" (Um azul mais suave que o anterior para não sumir o texto)
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(0, 125, 210);
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;

                // Garante que mesmo clicando, continue azul
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 125, 210);
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                // Branco padrão para o que não foi bipado
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Black;
            }
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
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string codigoLido = txtProductCode.Text.Trim();

                var row = dgvItems.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => r.Cells["Code"].Value.ToString() == codigoLido);

                if (row != null)
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

                        _printQueue.Add(new ItemManifest
                        {
                            Code = item.Code,
                            Description = item.Description,
                            Quantity = qty  // AQUI: Usamos a variável 'qty' do bip atual para a etiqueta
                        });



                        item.CountedQuantity += qty;
                      

                        // 2. SALVA NO BANCO (SQLite) IMEDIATAMENTE
                        try
                        {
                            using (var connection = new SqliteConnection("Data Source=estoqueAGB.db"))
                            {
                                connection.Open();
                                double remainingToDistribute = qty;

                                string ids = string.Join(",", _manifestIds);
                                var searchCmd = connection.CreateCommand();
                                searchCmd.CommandText = @"
                                    SELECT Id, CountedQuantity, ExpectedQuantity 
                                    FROM ManifestItem 
                                    WHERE Code = @code AND ManifestId IN (" + ids + @")
                                    ORDER BY (ExpectedQuantity - CountedQuantity) DESC";

                                searchCmd.Parameters.AddWithValue("@code", barcode);

                                using (var reader = searchCmd.ExecuteReader())
                                {
                                    while (reader.Read() && remainingToDistribute > 0)
                                    {
                                        long dbId = reader.GetInt64(0);
                                        double expected = reader.GetDouble(1);
                                        double current = reader.GetDouble(2);
                                        double gap = expected - current;

                                        double amountToAdd = (remainingToDistribute > gap && gap > 0) ? gap : remainingToDistribute;

                                        var updateCmd = connection.CreateCommand();
                                        updateCmd.CommandText = "UPDATE ManifestItem SET CountedQuantity = CountedQuantity + @val WHERE Id = @id";
                                        updateCmd.Parameters.AddWithValue("@val", amountToAdd);
                                        updateCmd.Parameters.AddWithValue("@id", dbId);
                                        updateCmd.ExecuteNonQuery();

                                        remainingToDistribute -= amountToAdd;
                                    }
                                }
                            }

                            using (var connection = new SqliteConnection("Data Source=estoqueAGB.db"))
                            {
                                connection.Open();
                                var cmdQueue = connection.CreateCommand();

                                // Adicionamos o campo CheckedBy no INSERT
                                cmdQueue.CommandText = @"
                                INSERT INTO PrintQueue (ProductCode, Description, Qty, ScannedAt, CheckedBy) 
                                VALUES (@code, @desc, @qty, @date, @user)";

                                cmdQueue.Parameters.AddWithValue("@code", item.Code);
                                cmdQueue.Parameters.AddWithValue("@desc", item.Description);
                                cmdQueue.Parameters.AddWithValue("@qty", qty);
                                cmdQueue.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                // Aqui você passa o nome de quem está conferindo
                                string nomeConferente = "DERLAN"; // Ou System.Environment.UserName para pegar do Windows
                                cmdQueue.Parameters.AddWithValue("@user", nomeConferente);

                                cmdQueue.ExecuteNonQuery();
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


       

        private void button3_Click(object sender, EventArgs e)
        {
            Menu inicial = new Menu();
            this.Hide();
            inicial.ShowDialog();
            this.Close();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 1. Pergunta para confirmar, estilo SAP
            var result = MessageBox.Show("Deseja finalizar a conferência deste manifesto?",
                                         "Finalizar Documento",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var connection = new SqliteConnection("Data Source=estoqueAGB.db"))
                    {
                        connection.Open();

                        // 2. Atualiza o Status do Manifesto para 1 (Finalizado)
                        // Isso faz com que ele suma da lista de seleção inicial
                        var cmdStatus = connection.CreateCommand();
                        cmdStatus.CommandText = "UPDATE Manifest SET Status = 1 WHERE Id IN (" + string.Join(",", _manifestIds) + ")";
                        cmdStatus.ExecuteNonQuery();

                        // 3. Limpa a Fila de Impressão (PrintQueue)
                        // Já que você finalizou, a próxima carga deve começar com a fila vazia
                        var cmdClear = connection.CreateCommand();
                        cmdClear.CommandText = "DELETE FROM PrintQueue";
                        cmdClear.ExecuteNonQuery();
                    }

                    MessageBox.Show("Manifesto finalizado com sucesso!", "SAP Business One", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 4. Fecha a tela de conferência e volta para a seleção
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao finalizar manifesto: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
         

            using (var connection = new SqliteConnection("Data Source=estoqueAGB.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Id, ProductCode, Description, Qty, ScannedAt, CheckedBy FROM PrintQueue  ORDER BY Id ASC";

                using (var reader = cmd.ExecuteReader())
                {

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Nenhum item bipado para imprimir.");
                        return;
                    }
                    while (reader.Read())
                    {
                        long rowId = reader.GetInt64(0);
                        string code = reader.GetString(1);
                        string desc = reader.GetString(2);
                        double qty = reader.GetDouble(3);
                        string scannedDate = reader.IsDBNull(4) ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : reader.GetString(4);
                        string user = reader.IsDBNull(5) ? "N/A" : reader.GetString(4); // Nome do conferente

                        // DATAS DEFINIDAS AQUI PARA O ZPL
                        string printDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        string expiryDate = "10/28";

                        // SEU CÓDIGO ZPL AQUI (usando as variáveis acima)
                        string zpl = $@"

                                    CT
                                    ~~CD,
                                    ~CC^
                                    ~CT~
                                    ^XA
                                    ~TA000
                                    ~JSN
                                    ^LT0
                                    ^MNW
                                    ^MTT
                                    ^PON
                                    ^PMN
                                    ^LH0,0
                                    ^JMA
                                    ^PR6,6
                                    ~SD15
                                    ^JUS
                                    ^LRN
                                    ^CI27
                                    ^PA0,1,1,0
                                    ^XZ
                                    ^XA
                                    ^MMT
                                    ^PW480
                                    ^LL799
                                    ^LS0
                                    ^BY1,3,96^FT457,468^BCB,,Y,N
                                    ^FH\^FD>;{code}^FS
                                    ^FT67,468^A0B,28,28^FH\^CI28^FD{code}^FS^CI27
                                    ^FT53,750^A0B,14,15^FH\^CI28^FDNF: 12345566^FS^CI27
                                    ^FT53,223^A0B,14,15^FH\^CI28^FDconferido em : {scannedDate}^FS^CI27
                                    ^FT75,223^A0B,14,15^FH\^CI28^FDPor :{user}^FS^CI27
                                    ^FT194,750^A0B,39,38^FH\^CI28^FD{desc}^FS^CI27
                                    ^FT357,740^A0B,17,18^FH\^CI28^FDQnt^FS^CI27
                                    ^FT405,742^A0B,28,28^FH\^CI28^FD{qty}^FS^CI27
                                    ^FT357,164^A0B,17,18^FH\^CI28^FDValidade^FS^CI27
                                    ^FT405,159^A0B,28,28^FH\^CI28^FD{expiryDate}^FS^CI27
                                    ^FO90,2^GB0,795,1^FS
                                    ^FO328,2^GB0,795,1^FS
                                    ^FO330,596^GB150,0,1^FS
                                    ^FO330,242^GB150,0,1^FS
                                    ^FO1,596^GB89,0,1^FS
                                    ^FO1,242^GB89,0,1^FS
                                    ^PQ1,0,1,Y
                                    ^XZ";

                       string url = "http://labelary.com/viewer.html?zpl=" + Uri.EscapeDataString(zpl);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });

                        // Envia cada etiqueta individualmente
                        //RawPrinterHelper.SendStringToPrinter(user, zpl);



           
                    }
                }
            }

        }
    }

}