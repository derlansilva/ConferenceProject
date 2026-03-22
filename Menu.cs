using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stock
{
    public partial class Menu : Form
    {
        string user;
        public Menu(string username)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            UserName.Text = $"Bem Vindo, {username}!";
            this.user = username;
            _ = EnsureDatabaseCreated();
        }


        private async Task EnsureDatabaseCreated()
        {
            string dbPath = Path.Combine(Application.StartupPath, "AGB_v2.db");

            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateCommand();

                // Use o seu código da image_bfd7d2.png aqui
                cmd.CommandText = @"
                       
                        CREATE TABLE IF NOT EXISTS Manifest (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ManifestNumber TEXT,
                            Status INTEGER DEFAULT 0,
                            CreatedAt TEXT
                        );
                        CREATE TABLE IF NOT EXISTS ManifestItem (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ManifestId INTEGER,
                            Code TEXT,
                            Description TEXT,
                            ExpectedQuantity REAL,
                            ExpiryDate,
                            CountedQuantity REAL DEFAULT 0, -- VÍRGULA AQUI É O SEGREDO
                            FOREIGN KEY (ManifestId) REFERENCES Manifest(Id)
                        );
                        CREATE TABLE IF NOT EXISTS PrintQueue (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ManifestId INTEGER,
                            ProductCode TEXT,
                            Description TEXT,
                            Qty REAL,
                            ScannedAt TEXT,
                            CheckedBy TEXT,
                            ExpiryDate
                        );
                                            ";

                await cmd.ExecuteNonQueryAsync();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Manifest manifest = new Manifest();
            manifest.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            MenuBoxConferences conferenceForm = new MenuBoxConferences(this.user);
            conferenceForm.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void UserName_Click(object sender, EventArgs e)
        {

        }
    }
}
