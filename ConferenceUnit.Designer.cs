namespace Stock
{
    partial class ConferenceUnit
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
            TxtManifestNumber = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            lblTotalItems = new Label();
            lblCountedItems = new Label();
            lblRemainingItems = new Label();
            dgvItems = new DataGridView();
            btnFinalizar = new Button();
            btnInterromper = new Button();
            btnRelatorio = new Button();
            txtProductCode = new TextBox();
            txtDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            SuspendLayout();
            // 
            // TxtManifestNumber
            // 
            TxtManifestNumber.AutoSize = true;
            TxtManifestNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TxtManifestNumber.Location = new Point(86, 32);
            TxtManifestNumber.Name = "TxtManifestNumber";
            TxtManifestNumber.Size = new Size(115, 21);
            TxtManifestNumber.TabIndex = 1;
            TxtManifestNumber.Text = "Nº Manifesto ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 0, 192);
            label2.Location = new Point(86, 81);
            label2.Name = "label2";
            label2.Size = new Size(61, 30);
            label2.TabIndex = 2;
            label2.Text = "Total";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Green;
            label3.Location = new Point(413, 72);
            label3.Name = "label3";
            label3.Size = new Size(106, 30);
            label3.TabIndex = 3;
            label3.Text = "Contados";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Red;
            label4.Location = new Point(751, 81);
            label4.Name = "label4";
            label4.Size = new Size(68, 30);
            label4.TabIndex = 4;
            label4.Text = "Faltas";
            // 
            // lblTotalItems
            // 
            lblTotalItems.AutoSize = true;
            lblTotalItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalItems.ForeColor = Color.FromArgb(0, 0, 192);
            lblTotalItems.Location = new Point(86, 111);
            lblTotalItems.Name = "lblTotalItems";
            lblTotalItems.Size = new Size(49, 30);
            lblTotalItems.TabIndex = 5;
            lblTotalItems.Text = "100";
            // 
            // lblCountedItems
            // 
            lblCountedItems.AutoSize = true;
            lblCountedItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCountedItems.ForeColor = Color.Green;
            lblCountedItems.Location = new Point(444, 111);
            lblCountedItems.Name = "lblCountedItems";
            lblCountedItems.Size = new Size(37, 30);
            lblCountedItems.TabIndex = 6;
            lblCountedItems.Text = "25";
            // 
            // lblRemainingItems
            // 
            lblRemainingItems.AutoSize = true;
            lblRemainingItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRemainingItems.ForeColor = Color.Red;
            lblRemainingItems.Location = new Point(782, 123);
            lblRemainingItems.Name = "lblRemainingItems";
            lblRemainingItems.Size = new Size(37, 30);
            lblRemainingItems.TabIndex = 7;
            lblRemainingItems.Text = "75";
            // 
            // dgvItems
            // 
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(86, 258);
            dgvItems.Name = "dgvItems";
            dgvItems.Size = new Size(733, 249);
            dgvItems.TabIndex = 15;
            // 
            // btnFinalizar
            // 
            btnFinalizar.BackColor = Color.Blue;
            btnFinalizar.ForeColor = SystemColors.ControlLightLight;
            btnFinalizar.Location = new Point(302, 539);
            btnFinalizar.Name = "btnFinalizar";
            btnFinalizar.Size = new Size(75, 32);
            btnFinalizar.TabIndex = 16;
            btnFinalizar.Text = "Finalizar";
            btnFinalizar.UseVisualStyleBackColor = false;
            btnFinalizar.Click += btnFinalizar_Click;
            // 
            // btnInterromper
            // 
            btnInterromper.BackColor = Color.Red;
            btnInterromper.ForeColor = SystemColors.ControlLightLight;
            btnInterromper.Location = new Point(86, 539);
            btnInterromper.Name = "btnInterromper";
            btnInterromper.Size = new Size(91, 32);
            btnInterromper.TabIndex = 17;
            btnInterromper.Text = "Interromper";
            btnInterromper.UseVisualStyleBackColor = false;
            btnInterromper.Click += btnInterromper_Click;
            // 
            // btnRelatorio
            // 
            btnRelatorio.Location = new Point(203, 539);
            btnRelatorio.Name = "btnRelatorio";
            btnRelatorio.Size = new Size(75, 32);
            btnRelatorio.TabIndex = 18;
            btnRelatorio.Text = "Relatorio";
            btnRelatorio.UseVisualStyleBackColor = true;
            btnRelatorio.Click += btnRelatorio_Click;
            // 
            // txtProductCode
            // 
            txtProductCode.Location = new Point(86, 191);
            txtProductCode.Multiline = true;
            txtProductCode.Name = "txtProductCode";
            txtProductCode.Size = new Size(733, 37);
            txtProductCode.TabIndex = 19;
            txtProductCode.KeyDown += txtProductCode_KeyDown;
            // 
            // txtDescription
            // 
            txtDescription.AutoSize = true;
            txtDescription.Location = new Point(86, 162);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(57, 15);
            txtDescription.TabIndex = 20;
            txtDescription.Text = "descrição";
            // 
            // ConferenceUnit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(txtDescription);
            Controls.Add(txtProductCode);
            Controls.Add(btnRelatorio);
            Controls.Add(btnInterromper);
            Controls.Add(btnFinalizar);
            Controls.Add(dgvItems);
            Controls.Add(lblRemainingItems);
            Controls.Add(lblCountedItems);
            Controls.Add(lblTotalItems);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TxtManifestNumber);
            Name = "ConferenceUnit";
            Text = "ConferenceUnit";
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TxtManifestNumber;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label lblTotalItems;
        private Label lblCountedItems;
        private Label lblRemainingItems;
        private DataGridView dgvItems;
        private Button btnFinalizar;
        private Button btnInterromper;
        private Button btnRelatorio;
        private TextBox txtProductCode;
        private Label txtDescription;
    }
}