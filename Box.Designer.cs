namespace Stock
{
    partial class Box
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
            txtDescription = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            txtProductCode = new TextBox();
            txtQuantity = new TextBox();
            dgvItems = new DataGridView();
            lblManifestNumber = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            SuspendLayout();
            // 
            // TxtManifestNumber
            // 
            TxtManifestNumber.AutoSize = true;
            TxtManifestNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TxtManifestNumber.Location = new Point(72, 25);
            TxtManifestNumber.Name = "TxtManifestNumber";
            TxtManifestNumber.Size = new Size(115, 21);
            TxtManifestNumber.TabIndex = 0;
            TxtManifestNumber.Text = "Nº Manifesto ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 0, 192);
            label2.Location = new Point(72, 60);
            label2.Name = "label2";
            label2.Size = new Size(61, 30);
            label2.TabIndex = 1;
            label2.Text = "Total";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Green;
            label3.Location = new Point(402, 60);
            label3.Name = "label3";
            label3.Size = new Size(106, 30);
            label3.TabIndex = 2;
            label3.Text = "Contados";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Red;
            label4.Location = new Point(811, 60);
            label4.Name = "label4";
            label4.Size = new Size(59, 30);
            label4.TabIndex = 3;
            label4.Text = "Falta";
            // 
            // lblTotalItems
            // 
            lblTotalItems.AutoSize = true;
            lblTotalItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalItems.ForeColor = Color.FromArgb(0, 0, 192);
            lblTotalItems.Location = new Point(72, 93);
            lblTotalItems.Name = "lblTotalItems";
            lblTotalItems.Size = new Size(49, 30);
            lblTotalItems.TabIndex = 4;
            lblTotalItems.Text = "100";
            // 
            // lblCountedItems
            // 
            lblCountedItems.AutoSize = true;
            lblCountedItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCountedItems.ForeColor = Color.Green;
            lblCountedItems.Location = new Point(429, 93);
            lblCountedItems.Name = "lblCountedItems";
            lblCountedItems.Size = new Size(37, 30);
            lblCountedItems.TabIndex = 5;
            lblCountedItems.Text = "25";
            lblCountedItems.Click += lblConferidoValue_Click;
            // 
            // lblRemainingItems
            // 
            lblRemainingItems.AutoSize = true;
            lblRemainingItems.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRemainingItems.ForeColor = Color.Red;
            lblRemainingItems.Location = new Point(822, 90);
            lblRemainingItems.Name = "lblRemainingItems";
            lblRemainingItems.Size = new Size(37, 30);
            lblRemainingItems.TabIndex = 6;
            lblRemainingItems.Text = "75";
            lblRemainingItems.Click += lblFaltandoValue_Click;
            // 
            // txtDescription
            // 
            txtDescription.AutoSize = true;
            txtDescription.Location = new Point(113, 146);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(70, 15);
            txtDescription.TabIndex = 7;
            txtDescription.Text = "codigo aqui";
            // 
            // button1
            // 
            button1.Location = new Point(72, 552);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 8;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(153, 552);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 9;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(244, 552);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 10;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(334, 552);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 11;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // txtProductCode
            // 
            txtProductCode.Location = new Point(113, 174);
            txtProductCode.Multiline = true;
            txtProductCode.Name = "txtProductCode";
            txtProductCode.Size = new Size(733, 37);
            txtProductCode.TabIndex = 12;
            txtProductCode.KeyDown += txtProductCode_KeyDown;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(113, 217);
            txtQuantity.Multiline = true;
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(733, 37);
            txtQuantity.TabIndex = 13;
            txtQuantity.Visible = false;
            txtQuantity.KeyDown += txtQuantity_KeyDown;
            // 
            // dgvItems
            // 
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(113, 265);
            dgvItems.Name = "dgvItems";
            dgvItems.Size = new Size(733, 249);
            dgvItems.TabIndex = 14;
            // 
            // lblManifestNumber
            // 
            lblManifestNumber.AutoSize = true;
            lblManifestNumber.Location = new Point(221, 31);
            lblManifestNumber.Name = "lblManifestNumber";
            lblManifestNumber.Size = new Size(91, 15);
            lblManifestNumber.TabIndex = 15;
            lblManifestNumber.Text = "manifestos aqui";
            // 
            // Box
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(lblManifestNumber);
            Controls.Add(dgvItems);
            Controls.Add(txtQuantity);
            Controls.Add(txtProductCode);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtDescription);
            Controls.Add(lblRemainingItems);
            Controls.Add(lblCountedItems);
            Controls.Add(lblTotalItems);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TxtManifestNumber);
            Name = "Box";
            Text = "Box";
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
        private Label txtDescription;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextBox txtProductCode;
        private TextBox txtQuantity;
        private DataGridView dgvItems;
        private Label lblManifestNumber;
    }
}