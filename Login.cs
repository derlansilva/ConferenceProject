using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stock
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            startDb();
        }

        private async void startDb()
        {
            using (var connection = new SqliteConnection("Data Source = AGB_v2.db"))
            {
                await connection.OpenAsync();

                var createTable = connection.CreateCommand();

                createTable.CommandText = @"
    
                        CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        Name TEXT NOT NULL
                    );

                    -- Insira um usuário padrão para teste
                    INSERT OR IGNORE INTO Users (Username, Password, Name) 
                    VALUES ('derlan', '03394579', 'Administrador');

                    -- Insira um usuário padrão para teste
                    INSERT OR IGNORE INTO Users (Username, Password, Name) 
                    VALUES ('thamy', '1234', 'Administrador');
                ";

                await createTable.ExecuteNonQueryAsync();
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text;
            string password = txtPassword.Text;

            using( var connection = new SqliteConnection("Data Source=AGB_v2.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Users WHERE Username=@username AND Password=@password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    string useName = DialogResult.ToString();
                    MessageBox.Show("Login successful!");

                    Menu menu = new Menu(username);
                    menu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
                
            }
        }
    }
}
