namespace Stock
{
    partial class MenuBoxConferences
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
            dgvManifests = new DataGridView();
            btnIniciarConferencia = new Button();
            btnInicarConferenciaUnidades = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvManifests).BeginInit();
            SuspendLayout();
            // 
            // dgvManifests
            // 
            dgvManifests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvManifests.Location = new Point(183, 115);
            dgvManifests.Name = "dgvManifests";
            dgvManifests.Size = new Size(624, 225);
            dgvManifests.TabIndex = 1;
            dgvManifests.CellValueChanged += dgvManifests_CellValueChanged;
            dgvManifests.CurrentCellDirtyStateChanged += dgvManifests_CurrentCellDirtyStateChanged_1;
            // 
            // btnIniciarConferencia
            // 
            btnIniciarConferencia.Location = new Point(574, 437);
            btnIniciarConferencia.Name = "btnIniciarConferencia";
            btnIniciarConferencia.Size = new Size(233, 34);
            btnIniciarConferencia.TabIndex = 2;
            btnIniciarConferencia.Text = "Conferencia Caixas";
            btnIniciarConferencia.UseVisualStyleBackColor = true;
            btnIniciarConferencia.Visible = false;
            btnIniciarConferencia.Click += btnIniciarConferencia_Click_1;
            // 
            // btnInicarConferenciaUnidades
            // 
            btnInicarConferenciaUnidades.Location = new Point(183, 437);
            btnInicarConferenciaUnidades.Name = "btnInicarConferenciaUnidades";
            btnInicarConferenciaUnidades.Size = new Size(233, 34);
            btnInicarConferenciaUnidades.TabIndex = 3;
            btnInicarConferenciaUnidades.Text = "Conferencia Unidades";
            btnInicarConferenciaUnidades.UseVisualStyleBackColor = true;
            btnInicarConferenciaUnidades.Visible = false;
            btnInicarConferenciaUnidades.Click += btnInicarConferenciaUnidades_Click;
            // 
            // MenuBoxConferences
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 596);
            Controls.Add(btnInicarConferenciaUnidades);
            Controls.Add(btnIniciarConferencia);
            Controls.Add(dgvManifests);
            Name = "MenuBoxConferences";
            Text = "MenuBoxConferences";
            Load += Conferences_Load;
            ((System.ComponentModel.ISupportInitialize)dgvManifests).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvManifests;
        private Button btnIniciarConferencia;
        private Button btnInicarConferenciaUnidades;
    }
}