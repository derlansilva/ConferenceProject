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
    public partial class MenuBoxConferences : Form
    {


        public MenuBoxConferences()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private async void Conferences_Load(object sender, EventArgs e)
        {
            List<ManifestSelection> manifests = new List<ManifestSelection>();

            using (var connection = new SqliteConnection("Data Source=association.db"))
            {
                await connection.OpenAsync();
                var selectCmd = connection.CreateCommand();


                selectCmd.CommandText = "SELECT Id, ManifestNumber, CreatedAt FROM Manifest ORDER BY Id DESC";

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
                    Box tela = new Box(idsSelecionados);

                    tela.StartPosition = FormStartPosition.CenterScreen;

                    tela.TopMost = true;

                    tela.ShowDialog();

                    tela.TopMost = false;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, selecione ao menos um manifesto na lista.", "Aviso");
                }
            }
        }

        private void dgvManifests_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            if (dgvManifests.IsCurrentCellDirty)
            {
                // Isso força o disparo do CellValueChanged na hora do clique
                dgvManifests.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
