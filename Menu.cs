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
        public Menu()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
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
            MenuBoxConferences conferenceForm = new MenuBoxConferences();
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
