namespace Stock
{
    partial class FormValidate
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
            components = new System.ComponentModel.Container();
            lbCode = new Label();
            lbDescription = new Label();
            txtExpiry = new TextBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            SuspendLayout();
            // 
            // lbCode
            // 
            lbCode.AutoSize = true;
            lbCode.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbCode.Location = new Point(279, 194);
            lbCode.Name = "lbCode";
            lbCode.Size = new Size(65, 21);
            lbCode.TabIndex = 0;
            lbCode.Text = "Codigo";
            // 
            // lbDescription
            // 
            lbDescription.AutoSize = true;
            lbDescription.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbDescription.Location = new Point(279, 224);
            lbDescription.Name = "lbDescription";
            lbDescription.Size = new Size(106, 30);
            lbDescription.TabIndex = 1;
            lbDescription.Text = "Descrição";
            // 
            // txtExpiry
            // 
            txtExpiry.Location = new Point(279, 287);
            txtExpiry.Multiline = true;
            txtExpiry.Name = "txtExpiry";
            txtExpiry.Size = new Size(328, 44);
            txtExpiry.TabIndex = 2;
            txtExpiry.KeyDown += txtExpiry_KeyDown;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // FormValidate
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(txtExpiry);
            Controls.Add(lbDescription);
            Controls.Add(lbCode);
            Name = "FormValidate";
            Text = "FormValidate";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbCode;
        private Label lbDescription;
        private TextBox txtExpiry;
        private ContextMenuStrip contextMenuStrip1;
    }
}