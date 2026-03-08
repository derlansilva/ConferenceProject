using Microsoft.Data.Sqlite;
using Stock.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Stock
{
    public partial class Manifest : Form
    {
        public Manifest()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        Dictionary<string, ItemManifest> mainInventory = new Dictionary<string, ItemManifest>();
        private void button1_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "XML Files (*.xml)|*.xml";
                fileDialog.Multiselect = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in fileDialog.FileNames)
                    {
                        ProcessXmlAndMerge(filePath);
                    }

                    dgvItems.DataSource = null;
                    dgvItems.DataSource = mainInventory.Values.ToList();

                    FormatGrid();

                }
            }


        }



        private void ProcessXmlAndMerge(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            // Busca os itens ignorando namespace para não dar erro
            var xmlItems = from det in doc.Descendants().Where(x => x.Name.LocalName == "det")
                           let prod = det.Elements().FirstOrDefault(x => x.Name.LocalName == "prod")
                           where prod != null
                           select new
                           {
                               Code = prod.Elements().FirstOrDefault(x => x.Name.LocalName == "cProd")?.Value ?? "N/A",
                               Desc = prod.Elements().FirstOrDefault(x => x.Name.LocalName == "xProd")?.Value ?? "N/A",
                               Qty = double.Parse(prod.Elements().FirstOrDefault(x => x.Name.LocalName == "qCom")?.Value ?? "0", System.Globalization.CultureInfo.InvariantCulture)
                           };

            foreach (var item in xmlItems)
            {
                if (mainInventory.ContainsKey(item.Code))
                {
                    // SE JÁ EXISTE, SOMA A QUANTIDADE
                    mainInventory[item.Code].Quantity += item.Qty;
                }
                else
                {
                    // SE NÃO EXISTE, ADICIONA NOVO
                    mainInventory.Add(item.Code, new ItemManifest
                    {
                        Code = item.Code,
                        Description = item.Desc,
                        Quantity = item.Qty
                    });
                }
            }
        }

        private void FormatGrid()
        {
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            btnGenerateManifest.Enabled = mainInventory.Count > 0;
        }

        private async void btnGenerateManifest_Click(object sender, EventArgs e)
        {
            btnGenerateManifest.Enabled = false;
            pbProcessing.Visible = true;
            pbProcessing.Style = ProgressBarStyle.Marquee;

            try
            {
                // Use inventory.db as the filename
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=association.db"))
                {
                    await connection.OpenAsync();

                    var createTablesCmd = connection.CreateCommand();
                    createTablesCmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Manifest (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ManifestNumber TEXT,
                            CreatedAt TEXT
                        );
                        CREATE TABLE IF NOT EXISTS ManifestItem (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ManifestId INTEGER,
                            Code TEXT,
                            Description TEXT,
                            ExpectedQuantity REAL,
                            CountedQuantity REAL DEFAULT 0,
                            FOREIGN KEY (ManifestId) REFERENCES Manifest(Id)
                        );";
                    await createTablesCmd.ExecuteNonQueryAsync();

                    // 2. Generating the random number (Using full name to avoid Red Line) 🎲
                    System.Random randomTool = new System.Random();
                    string manifestIdStr = randomTool.Next(1000, 9999).ToString();

                    // 3. Saving the Parent Manifest 🆔
                    var insertManifestCmd = connection.CreateCommand();
                    insertManifestCmd.CommandText = @"
                INSERT INTO Manifest (ManifestNumber, CreatedAt) 
                VALUES (@num, @date);
                SELECT last_insert_rowid();";

                    insertManifestCmd.Parameters.AddWithValue("@num", manifestIdStr);
                    insertManifestCmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Get the ID generated by the database
                    long newManifestId = (long)await insertManifestCmd.ExecuteScalarAsync();

                    // 4. Saving the items in the "Items Spreadsheet" 📦
                    foreach (var item in mainInventory.Values)
                    {
                        var insertItemCmd = connection.CreateCommand();
                        insertItemCmd.CommandText = @"
                    INSERT INTO ManifestItem (ManifestId, Code, Description, ExpectedQuantity) 
                    VALUES (@mid, @c, @d, @q)";

                        insertItemCmd.Parameters.AddWithValue("@mid", newManifestId);
                        insertItemCmd.Parameters.AddWithValue("@c", item.Code);
                        insertItemCmd.Parameters.AddWithValue("@d", item.Description);
                        insertItemCmd.Parameters.AddWithValue("@q", item.Quantity);

                        await insertItemCmd.ExecuteNonQueryAsync();
                    }

                    // 5. Success feedback
                    await Task.Delay(1000);
                    pbProcessing.Visible = false;
                    MessageBox.Show($"Manifest #{manifestIdStr} saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnGenerateManifest.Visible = false;
                    btnBackToMenu.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGenerateManifest.Enabled = true;
                pbProcessing.Visible = false;
            }
        }


        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
           Menu inicial = new Stock.Menu();
            this.Hide();
            inicial.ShowDialog();
            this.Close();
        }
    }


}
