using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stock
{
    public partial class FormValidate : Form
    {
        public string ExpiryValue { get; private set; }
        public FormValidate(string productCode, string description)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Input Expiry Date";

            // Atribui os valores aos seus Labels (Certifique-se que os nomes batem com o Designer)
            // Se os nomes forem diferentes, ajuste lblCode e lblDescription abaixo:
            if (lbCode != null) lbCode.Text = productCode;
            if (lbDescription != null) lbDescription.Text = description;

            // Foca no campo de texto assim que abrir
            //this.Shown += (s, e) => txtExpiry.Focus();

        }

        private void txtExpiry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ExpiryValue = txtExpiry.Text;
                this.DialogResult = DialogResult.OK; // Isso faz o loop do 'while' continuar para o próximo
                this.Close();
            }
        }
    }
}
