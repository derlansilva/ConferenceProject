using iText.IO.Font.Constants;
using iText.Kernel.Colors;    // Essencial para o DeviceRgb e cores
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Data.Sqlite;
using Stock.models;
using System.Data;
using System.Media;
using System.Drawing.Printing;

namespace Stock
{
    public partial class Box : Form
    {
        private List<long> _manifestIds;
        private List<ItemManifest> _itemsToCount;
        private List<ItemManifest> _printQueue = new List<ItemManifest>();
        private string database = "Data Source=AGB_v2.db";
        // --- NOVAS VARIÁVEIS PARA FILA E VALIDADE ---
        private Queue<ScannedItem> expiryQueue = new Queue<ScannedItem>();
        private Dictionary<string, string> rememberedExpiries = new Dictionary<string, string>();
        private string user;
        public Box(List<long> _manifestIds , string username)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this._manifestIds = _manifestIds;
            _itemsToCount = new List<ItemManifest>();

            this.user = username;
            
            LoadData();
            SetupUI();
        }

        public void LoadData()
        {

            try
            {
                _itemsToCount.Clear();

                string idsString = string.Join(",", _manifestIds);

                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(database))
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
           
            txtQuantity.Visible = false;
            txtProductCode.Visible = true;
            txtProductCode.Focus();
            this.Activated += (s, e) => txtProductCode.Focus();
            lblRemainingItems.Cursor = Cursors.Hand; 
            lblRemainingItems.Font = new Font(lblRemainingItems.Font, FontStyle.Underline | FontStyle.Bold); 

        }

        private void PlayBipe()
        {
            try
            {
                
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sound", "qr-code-scan-beep.wav");

                if (File.Exists(path))
                {
                    using (SoundPlayer player = new SoundPlayer(path))
                    {
                        player.Play();
                    }
                }
                else
                {
                    // Se cair aqui, o arquivo não foi copiado para a pasta bin/Debug
                    Console.WriteLine("Arquivo de som não encontrado em: " + path);
                    // Som alternativo do Windows apenas para você saber que a lógica entrou aqui
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tocar som: " + ex.Message);
            }
        }

        private void PlayErro()
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "sound", "sonic-error-sound.wav");
                using (SoundPlayer player = new SoundPlayer(path))
                {
                    player.Play();
                }
            }
            catch (Exception ex) { /* Log ou ignore */ }
        }
        private void ApplySapTheme()
        {
            // 1. ESCONDER O QUE NÃO PRECISA
            if (dgvItems.Columns["ExpectedQuantity"] != null) dgvItems.Columns["ExpectedQuantity"].Visible = false;
            if (dgvItems.Columns["Status"] != null) dgvItems.Columns["Status"].Visible = false;
            dgvItems.RowHeadersVisible = false; // Tira a seta da esquerda

            // 2. CONFIGURAÇÃO DE BORDAS E CORES GERAIS
            dgvItems.BackgroundColor = System.Drawing.Color.White;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvItems.GridColor = System.Drawing.Color.FromArgb(200, 200, 200); // Linhas da grade cinzas

            // Desativa a seleção visual (o clique não faz nada)
            dgvItems.ReadOnly = true;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 3. CABEÇALHO (O Cinza do SAP)
            dgvItems.EnableHeadersVisualStyles = false;
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = System.Drawing.Color.FromArgb(225, 225, 225); // Cinza SAP
            headerStyle.ForeColor = System.Drawing.Color.Black;
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
        private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var item = (ItemManifest)dgvItems.Rows[e.RowIndex].DataBoundItem;

            // Se o item foi bipado (Contados > 0)
            if (item.CountedQuantity > 0)
            {
                
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 125, 210);
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.White;

                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 125, 210);
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            }
            else
            {
                
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
                dgvItems.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string codigoLido = txtProductCode.Text.Trim();

                
                string cleanCode = codigoLido.Contains(" ") ? codigoLido.Split(' ')[0] : codigoLido;

             
                var row = dgvItems.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(r => r.Cells["Code"].Value.ToString() == cleanCode);

                if (row != null)
                {
                    PlayBipe();
                    txtDescription.Text = row.Cells["Description"].Value.ToString();
                    txtDescription.ForeColor = System.Drawing.Color.DarkBlue;

                    dgvItems.ClearSelection();
                    row.Selected = true;
                    dgvItems.FirstDisplayedScrollingRowIndex = row.Index;

                 
                    txtProductCode.Visible = false;
                    txtQuantity.Visible = true;
                    txtQuantity.Focus();
                }
                else
                {
                    PlayErro();
                    MessageBox.Show("Produto não encontrado: " + cleanCode);
                    txtProductCode.Clear();
                    txtProductCode.Focus();
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
                   
                    string codigoBruto = txtProductCode.Text.Trim();
                    string barcode = codigoBruto.Contains(" ") ? codigoBruto.Split(' ')[0] : codigoBruto;

                    var item = _itemsToCount.FirstOrDefault(x => x.Code == barcode);

                    if (item != null)
                    {
                        if ((item.CountedQuantity + qty) > item.Quantity)
                        {
                            MessageBox.Show("Quantidade maior que o esperado!", "Aviso");
                        }

                        PlayBipe();
                        item.CountedQuantity += qty;

                        
                        expiryQueue.Enqueue(new ScannedItem { Code = item.Code, Description = item.Description });

                        try
                        {
                            using (var connection = new SqliteConnection(database))
                            {
                                connection.Open();
                              
                                var updateCmd = connection.CreateCommand();
                                updateCmd.CommandText = "UPDATE ManifestItem SET CountedQuantity = CountedQuantity + @val WHERE Code = @code AND ManifestId = @mid";
                                updateCmd.Parameters.AddWithValue("@val", qty);
                                updateCmd.Parameters.AddWithValue("@code", item.Code);
                                updateCmd.Parameters.AddWithValue("@mid", _manifestIds.FirstOrDefault());
                                updateCmd.ExecuteNonQuery();

                                // INSERE NA FILA DE IMPRESSÃO (Sem validade ainda)
                                var cmdQueue = connection.CreateCommand();
                                cmdQueue.CommandText = @"INSERT INTO PrintQueue (ProductCode, Description, Qty, ScannedAt, CheckedBy, ManifestId, ExpiryDate) 
                                               VALUES (@code, @desc, @qty, @date, @user, @mid, '')";
                                cmdQueue.Parameters.AddWithValue("@code", item.Code);
                                cmdQueue.Parameters.AddWithValue("@desc", item.Description);
                                cmdQueue.Parameters.AddWithValue("@qty", qty);
                                cmdQueue.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmdQueue.Parameters.AddWithValue("@user", string.IsNullOrEmpty(this.user) ? "Sistema" : this.user);
                                cmdQueue.Parameters.AddWithValue("@mid", _manifestIds.FirstOrDefault());
                                cmdQueue.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex) { MessageBox.Show("Erro ao salvar: " + ex.Message); }

                        SortAndRefreshGrid();
                        UpdateTopLabels();

                        
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

            
            RefreshGrid();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Menu inicial = new Menu();
            this.Hide();
            //inicial.ShowDialog();
            this.Close();

        }

        private void button4_Click(object sender, EventArgs e)
        {

            // 1. Confirmação para o usuário não deletar sem querer
            var confirm = MessageBox.Show("Deseja realmente finalizar e EXCLUIR estes manifestos? Esta ação não pode ser desfeita.",
                                         "Confirmar Finalização",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string ids = string.Join(",", _manifestIds);

                try
                {
                    using (var connection = new SqliteConnection(database))
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction(); // Usar transação para segurança

                        try
                        {
                            var cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;

                            // A ordem de exclusão importa para não quebrar chaves estrangeiras:

                            // 1. Deleta a Fila de Impressão (PrintQueue)
                            cmd.CommandText = $"DELETE FROM PrintQueue WHERE ManifestId IN ({ids})";
                            cmd.ExecuteNonQuery();

                            // 2. Deleta os Itens do Manifesto (ManifestItem)
                            cmd.CommandText = $"DELETE FROM ManifestItem WHERE ManifestId IN ({ids})";
                            cmd.ExecuteNonQuery();

                            // 3. Por fim, deleta o Manifesto em si (Manifest)
                            cmd.CommandText = $"DELETE FROM Manifest WHERE Id IN ({ids})";
                            cmd.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("Conferência finalizada e dados limpos com sucesso!", "Sucesso");

                            this.Close(); // Fecha a tela de conferência e volta para o menu
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Se der erro em um, não deleta nada
                            MessageBox.Show("Erro ao limpar dados: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro de conexão: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (var connection = new SqliteConnection(database))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                string ids = string.Join(",", _manifestIds);

                string printName = "";

                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    // Verifica se o nome da impressora contém "Zebra" ou "ZDesigner"
                    if (printer.ToUpper().Contains("ZEBRA") || printer.ToUpper().Contains("ZDESIGNER"))
                    {
                        printName = printer;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(printName))
                {
                    MessageBox.Show("Nenhuma impressora Zebra detectada no Windows!", "Erro de Impressão");
                    return; // Para o código aqui e não tenta imprimir no "vazio"
                }



                var cmdCheck = connection.CreateCommand();
                cmdCheck.CommandText = $@"
                SELECT COUNT(*) 
                FROM PrintQueue 
                WHERE ManifestId IN ({ids}) 
                AND Qty > 0 
                AND (ExpiryDate IS NULL OR ExpiryDate = '')";


                long itensWithoutExpiry = (long)cmdCheck.ExecuteScalar();

                if(itensWithoutExpiry > 0 )
                {
                    MessageBox.Show(
                                    $"Atenção: Existem {itensWithoutExpiry} bipagens sem validade informada!\n\n" +
                                    "Por favor, clique no botão 'Validar' e preencha todas as datas antes de imprimir.",
                                    "Validação Pendente",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);    
                    return;
                }
                cmd.CommandText = $@"
                    SELECT ProductCode, Description, Qty, ScannedAt, CheckedBy, ExpiryDate 
                    FROM PrintQueue 
                    WHERE ManifestId IN ({ids}) 
                    ORDER BY Id ASC";

                using (var reader = cmd.ExecuteReader())
                {

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Nenhum item bipado para imprimir.");
                        return;
                    }
                    while (reader.Read())
                    {
                        
                        string code = reader.GetString(0);
                        string desc = reader.GetString(1);
                        double qty = reader.GetDouble(2);
                        string rawDate = reader.IsDBNull(3) ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : reader.GetString(3);

                        string scannedDate = "";

                        if (DateTime.TryParse(rawDate, out DateTime dt))
                        {
                            scannedDate = dt.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                        {
                            scannedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        }

                        string user = reader.IsDBNull(4) ? "N/A" : reader.GetString(4); // Nome do conferente
                        string expiryDate = reader.IsDBNull(5) ? "SEM DATA" : reader.GetString(5);
                        // DATAS DEFINIDAS AQUI PARA O ZPL
                        string printDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                      

                        // SEU CÓDIGO ZPL AQUI (usando as variáveis acima)
                        string zpl = $@"
                      CT~~CD,~CC^~CT~
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
                    ^PW799
                    ^LL480
                    ^LS0
                   ^BY2,3,70^FT300,440^BCN,70,Y,N,N,A^FD{code}^FS
                    ^FT27,82^A0N,25,25^FH\^CI28^FDNF:12345^FS^CI27
                    ^FT300,91^A0N,39,38^FH\^CI28^FD{code}^FS^CI27
                    ^FT566,75^A0N,14,15^FH\^CI28^FDConferido em :{scannedDate}^FS^CI27
                    ^FT52,399^A0N,14,15^FH\^CI28^FDQnt^FS^CI27
                    ^FT52,448^A0N,45,46^FH\^CI28^FD{qty}^FS^CI27
                    ^FT613,399^A0N,14,15^FH\^CI28^FDvalidade^FS^CI27
                    ^FT613,448^A0N,45,46^FH\^CI28^FD{expiryDate}^FS^CI27
                    ^FT566,97^A0N,14,15^FH\^CI28^FDPor:{user}^FS^CI27
                    ^FT25,270^A0N,60,60^FB750,2,,C^FD{desc}^FS
                    ^FO2,321^GB797,0,1^FS
                    ^FO7,116^GB792,0,1^FS
                    ^FO208,321^GB0,156,1^FS
                    ^FO215,14^GB0,103,1^FS
                    ^FO552,14^GB0,103,1^FS
                    ^FO570,321^GB0,156,1^FS
                    ^PQ1,0,1,Y
                    ^XZ


                        ";

                        string url = "http://labelary.com/viewer.html?zpl=" + Uri.EscapeDataString(zpl);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });

                        //Envia cada etiqueta individualmente
                        RawPrinterHelper.SendStringToPrinter(printName, zpl);

                    }
                }
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {
            // 1. Configura o local onde o PDF será salvo (C:\Temp é mais seguro que o Desktop)
            string pasta = @"C:\Temp";
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
            string wayPdf = Path.Combine(pasta, "Relatorio_Faltas.pdf");

            try
            {
                // 2. Inicia a criação do PDF
                using (var writer = new iText.Kernel.Pdf.PdfWriter(wayPdf))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var doc = new iText.Layout.Document(pdf))
                {
                    // Criando a fonte Negrito
                    var fontBold = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                    // Título do Relatório
                    doc.Add(new iText.Layout.Element.Paragraph("RELATÓRIO DE DIVERGÊNCIAS (FALTAS)")
                        .SetFont(fontBold)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    doc.Add(new iText.Layout.Element.Paragraph($"Data: {DateTime.Now:dd/MM/yyyy HH:mm} | Conferente: DERLAN")
                        .SetFontSize(10));

                    // Criando a Tabela com 4 colunas (Código, Descrição, Esperado, Falta)
                    var table = new iText.Layout.Element.Table(iText.Layout.Properties.UnitValue.CreatePercentArray(new float[] { 20, 50, 15, 15 }))
                        .UseAllAvailableWidth();

                    // Cabeçalho da Tabela
                    string[] headers = { "Código", "Descrição", "Total", "Falta" };
                    foreach (var h in headers)
                    {
                        table.AddHeaderCell(new iText.Layout.Element.Cell()
                            .Add(new iText.Layout.Element.Paragraph(h).SetFont(fontBold))
                            .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(230, 230, 230)));
                    }

                    // Filtrando apenas itens que têm falta (Quantidade > Contado)
                    var itensComFalta = _itemsToCount.Where(x => (x.Quantity - x.CountedQuantity) > 0).ToList();

                    foreach (var item in itensComFalta)
                    {
                        double faltaQtd = item.Quantity - item.CountedQuantity;

                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Code ?? "")));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Description ?? "")));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Quantity.ToString())
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)));

                        // A célula da Falta fica em Vermelho
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(faltaQtd.ToString())
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                            .SetFontColor(iText.Kernel.Colors.ColorConstants.RED)));
                    }

                    doc.Add(table);

                    // Resumo de Totais no final
                    double totalFaltas = itensComFalta.Sum(x => x.Quantity - x.CountedQuantity);
                    doc.Add(new iText.Layout.Element.Paragraph("\n"));
                    doc.Add(new iText.Layout.Element.Paragraph($"TOTAL DE ITENS FALTANTES: {totalFaltas}")
                        .SetFont(fontBold)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.RED));
                }

                // 3. Comando para abrir o arquivo gerado no Chrome/Navegador padrão
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = wayPdf,
                    UseShellExecute = true // Crucial para o Windows saber que deve abrir o navegador
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (IOException)
            {
                MessageBox.Show("O arquivo PDF já está aberto no Chrome. Feche-o e clique novamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro técnico: " + ex.Message);
            }

        }

        private void btnRelatorio_Click(object sender, EventArgs e)
        {
            // Salva na pasta Temp para evitar erros de permissão do Windows
            string pasta = @"C:\Temp";
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
            string wayPdf = Path.Combine(pasta, "Relatorio_Conferencia.pdf");

            try
            {
                using (var writer = new iText.Kernel.Pdf.PdfWriter(wayPdf))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var doc = new iText.Layout.Document(pdf))
                {
                    // Criando a fonte sem depender de arquivos externos (evita o Unknown PdfException)
                    var fontBold = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                    doc.Add(new iText.Layout.Element.Paragraph("RELATÓRIO GERAL DE CONFERÊNCIA")
                        .SetFont(fontBold).SetFontSize(16).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    doc.Add(new iText.Layout.Element.Paragraph($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}").SetFontSize(10));

                    // Tabela com 5 colunas: Código, Descrição, Esperado, Contado, Diferença
                    var table = new iText.Layout.Element.Table(iText.Layout.Properties.UnitValue.CreatePercentArray(new float[] { 15, 45, 13, 13, 14 })).UseAllAvailableWidth();

                    string[] headers = { "Código", "Descrição", "Esperado", "Contado", "Dif." };
                    foreach (var h in headers)
                    {
                        table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(h).SetFont(fontBold))
                            .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(230, 230, 230)));
                    }

                    foreach (var item in _itemsToCount)
                    {
                        // CÁLCULO: Se positivo = Sobra, Se negativo = Falta
                        double diferenca = item.CountedQuantity - item.Quantity;

                        table.AddCell(item.Code ?? "");
                        table.AddCell(item.Description ?? "");
                        table.AddCell(new iText.Layout.Element.Paragraph(item.Quantity.ToString()).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                        table.AddCell(new iText.Layout.Element.Paragraph(item.CountedQuantity.ToString()).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                        // Formatação da Diferença
                        var pDif = new iText.Layout.Element.Paragraph(diferenca.ToString("+#;-#;0")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);

                        if (diferenca < 0) pDif.SetFontColor(iText.Kernel.Colors.ColorConstants.RED); // FALTA
                        else if (diferenca > 0) pDif.SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 128, 0)); // SOBRA (Verde)

                        table.AddCell(new iText.Layout.Element.Cell().Add(pDif));
                    }

                    doc.Add(table);
                }

                // Abre no navegador
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(wayPdf) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar relatório: " + ex.Message + "\n\nSe o erro persistir, feche o PDF no navegador.");
            }
        }
        public void ProcessExpiryQueue()
        {
            List<ScannedItem> pendingItems = new List<ScannedItem>();

            // --- ESSA LINHA RESOLVE O ERRO 'IDS NÃO EXISTE' ---
            string ids = string.Join(",", _manifestIds);

            try
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(database))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();

                    // ADICIONAMOS 'Qty > 0' PARA ELE SÓ PEDIR VALIDADE DO QUE VOCÊ BIPOU
                    cmd.CommandText = $@"
                SELECT DISTINCT ProductCode, Description 
                FROM PrintQueue 
                WHERE ManifestId IN ({ids}) 
                AND Qty > 0 
                AND (ExpiryDate IS NULL OR ExpiryDate = '')";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingItems.Add(new ScannedItem
                            {
                                Code = reader.GetString(0),
                                Description = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar fila: " + ex.Message); return; }

            if (pendingItems.Count == 0)
            {
                MessageBox.Show("Nenhum item bipado pendente de validade.");
                return;
            }

            // Abre os modais apenas para o que você realmente contou
            foreach (var currentItem in pendingItems)
            {
                if (rememberedExpiries.ContainsKey(currentItem.Code))
                {
                    SaveToDatabase(currentItem.Code, rememberedExpiries[currentItem.Code]);
                    continue;
                }

                using (var modal = new FormValidate(currentItem.Code, currentItem.Description))
                {
                    if (modal.ShowDialog() == DialogResult.OK)
                    {
                        string input = modal.ExpiryValue;
                        rememberedExpiries[currentItem.Code] = input;
                        SaveToDatabase(currentItem.Code, input);
                    }
                    else { break; }
                }
            }
            MessageBox.Show("Validações concluídas!");
        }

        private void SaveToDatabase(string code, string expiry)
        {
            try
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(database))
                {
                    connection.Open();
                    string ids = string.Join(",", _manifestIds);

                    // Atualiza ManifestItem
                    var up1 = connection.CreateCommand();
                    up1.CommandText = $"UPDATE ManifestItem SET ExpiryDate = @val WHERE Code = @code AND ManifestId IN ({ids})";
                    up1.Parameters.AddWithValue("@val", expiry);
                    up1.Parameters.AddWithValue("@code", code);
                    up1.ExecuteNonQuery();

                    // Atualiza PrintQueue
                    var up2 = connection.CreateCommand();
                    up2.CommandText = $"UPDATE PrintQueue SET ExpiryDate = @val WHERE ProductCode = @code AND ManifestId IN ({ids})";
                    up2.Parameters.AddWithValue("@val", expiry);
                    up2.Parameters.AddWithValue("@code", code);
                    up2.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro na validade: " + ex.Message); }
        }
        private void btnValidate_Click(object sender, EventArgs e)
        {
            // Verifica se há algo na fila antes de começar
            if (expiryQueue.Count == 0)
            {
                MessageBox.Show("Nenhum item ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Inicia o loop automático que abre os modais um atrás do outro
            ProcessExpiryQueue();
        }
        private void label3_Click(object sender, EventArgs e)
        {
           
            SortAndRefreshGrid();
        }

        private void lblConferidoValue_Click(object sender, EventArgs e)
        {
          
            UpdateTopLabels();
            SortAndRefreshGrid();
        }

        private void lblFaltandoValue_Click(object sender, EventArgs e)
        {
         
            var faltantes = _itemsToCount.Where(x => x.CountedQuantity < x.ExpectedQuantity).ToList();
            dgvItems.DataSource = null;
            dgvItems.DataSource = faltantes;
            ApplySapTheme();
        }

    }

}