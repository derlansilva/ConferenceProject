namespace Stock
{
    partial class Menu
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
            button2 = new Button();
            UserName = new Label();
            btnUser = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(388, 146);
            button1.Name = "button1";
            button1.Size = new Size(221, 38);
            button1.TabIndex = 0;
            button1.Text = "Gerar Manifesto";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(388, 190);
            button2.Name = "button2";
            button2.Size = new Size(221, 38);
            button2.TabIndex = 1;
            button2.Text = "Conferencia";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // UserName
            // 
            UserName.AutoSize = true;
            UserName.BackColor = Color.SeaShell;
            UserName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            UserName.Location = new Point(821, 20);
            UserName.Name = "UserName";
            UserName.Size = new Size(70, 17);
            UserName.TabIndex = 3;
            UserName.Text = "userName";
            UserName.Click += UserName_Click;
            // 
            // btnUser
            // 
            btnUser.Location = new Point(388, 234);
            btnUser.Name = "btnUser";
            btnUser.Size = new Size(221, 38);
            btnUser.TabIndex = 4;
            btnUser.Text = "Criar Usuario";
            btnUser.UseVisualStyleBackColor = true;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(btnUser);
            Controls.Add(UserName);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Menu";
            Text = "Menu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Label UserName;
        private Button btnUser;
    }
}