using Microsoft.Data.Sqlite;
using Stock.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace Stock
{
    public partial class ConferenceUnit : Form
    {
        private List<long> _manifestIds;
        private List<ItemManifest> _itemsToCount;
        private List<ItemManifest> _printQueue = new List<ItemManifest>();
        private string database = "Data Source=AGB_v2.db";
        // --- NOVAS VARIÁVEIS PARA FILA E VALIDADE ---
        private Queue<ScannedItem> expiryQueue = new Queue<ScannedItem>();
        private Dictionary<string, string> rememberedExpiries = new Dictionary<string, string>();
        private string user;
        public ConferenceUnit(List<long> _manifestIds, string username)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this._manifestIds = _manifestIds;
            _itemsToCount = new List<ItemManifest>();

            this.user = username;

            LoadData();
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



        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string codigoLido = txtProductCode.Text.Trim();
                string cleanCode = codigoLido.Contains(" ") ? codigoLido.Split(' ')[0] : codigoLido;


                var item = _itemsToCount.FirstOrDefault(x => x.Code == cleanCode);

                if (item != null)
                {

                    if (item.CountedQuantity >= item.ExpectedQuantity)
                    {
                      
                         MessageBox.Show("Atenção: Item já totalmente conferido!");
                    }
                    PlayBipe();
                    item.CountedQuantity += 1;

                    try
                    {
                        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(database))
                        {
                            connection.Open();
                            var updateCmd = connection.CreateCommand();
                            // Atualizamos o CountedQuantity somando 1 no banco para o manifesto atual
                            updateCmd.CommandText = "UPDATE ManifestItem SET CountedQuantity = CountedQuantity + 1 WHERE Code = @code AND ManifestId IN (" + string.Join(",", _manifestIds) + ")";
                            updateCmd.Parameters.AddWithValue("@code", item.Code);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao salvar no banco: " + ex.Message);
                    }

                    txtDescription.Text = item.Description;
                    txtDescription.ForeColor = System.Drawing.Color.DarkGreen; 

                    txtProductCode.Clear();
                    txtProductCode.Focus();

                    SortAndRefreshGrid(); 
                    UpdateTopLabels();

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



        private void btnFinalizar_Click(object sender, EventArgs e)
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

        private void btnRelatorio_Click(object sender, EventArgs e)
        {
            
            string pasta = @"C:\Temp";
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
            string wayPdf = Path.Combine(pasta, "Relatorio_Faltas.pdf");

            try
            {
                
                using (var writer = new iText.Kernel.Pdf.PdfWriter(wayPdf))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var doc = new iText.Layout.Document(pdf))
                {
                    
                    var fontBold = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                    
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

        private void btnInterromper_Click(object sender, EventArgs e)
        {
            //Menu inicial = new Menu();
            this.Hide();
            //inicial.ShowDialog();
            this.Close();
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

           
            txtProductCode.Visible = true;
            txtProductCode.Focus();
            this.Activated += (s, e) => txtProductCode.Focus();
            lblRemainingItems.Cursor = Cursors.Hand;
            lblRemainingItems.Font = new Font(lblRemainingItems.Font, FontStyle.Underline | FontStyle.Bold);

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
    }
}
