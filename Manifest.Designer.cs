namespace Stock
{
    partial class Manifest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            dgvItems = new DataGridView();
            btnGenerateManifest = new Button();
            panel1 = new Panel();
            btnBackToMenu = new Button();
            pbProcessing = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(310, 44);
            button1.Name = "button1";
            button1.Size = new Size(280, 39);
            button1.TabIndex = 0;
            button1.Text = "Buscar Arquivos";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // dgvItems
            // 
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(115, 192);
            dgvItems.Name = "dgvItems";
            dgvItems.Size = new Size(753, 240);
            dgvItems.TabIndex = 1;
            // 
            // btnGenerateManifest
            // 
            btnGenerateManifest.Enabled = false;
            btnGenerateManifest.Location = new Point(654, 534);
            btnGenerateManifest.Name = "btnGenerateManifest";
            btnGenerateManifest.Size = new Size(213, 34);
            btnGenerateManifest.TabIndex = 2;
            btnGenerateManifest.Text = "Gerar Manifesto";
            btnGenerateManifest.UseVisualStyleBackColor = true;
            btnGenerateManifest.Click += btnGenerateManifest_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnBackToMenu);
            panel1.Controls.Add(pbProcessing);
            panel1.Controls.Add(dgvItems);
            panel1.Location = new Point(-1, -3);
            panel1.Name = "panel1";
            panel1.Size = new Size(964, 600);
            panel1.TabIndex = 3;
            // 
            // btnBackToMenu
            // 
            btnBackToMenu.Location = new Point(115, 537);
            btnBackToMenu.Name = "btnBackToMenu";
            btnBackToMenu.Size = new Size(149, 34);
            btnBackToMenu.TabIndex = 3;
            btnBackToMenu.Text = "Voltar ao Menu";
            btnBackToMenu.UseVisualStyleBackColor = true;
            btnBackToMenu.Visible = false;
            btnBackToMenu.Click += btnBackToMenu_Click;
            // 
            // pbProcessing
            // 
            pbProcessing.Location = new Point(311, 490);
            pbProcessing.Name = "pbProcessing";
            pbProcessing.Size = new Size(556, 17);
            pbProcessing.TabIndex = 2;
            pbProcessing.Visible = false;
            // 
            // Manifest
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(btnGenerateManifest);
            Controls.Add(button1);
            Controls.Add(panel1);
            Name = "Manifest";
            Text = "Manifest";
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private OpenFileDialog openFileDialog1;
        private DataGridView dgvItems;
        private Button btnGenerateManifest;
        private Panel panel1;
        private Button btnBackToMenu;
        private ProgressBar pbProcessing;
    }
}