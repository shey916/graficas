namespace graficas
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.btnGraficaDona = new System.Windows.Forms.Button();
            this.btnGraficaBarras = new System.Windows.Forms.Button();
            this.btnGraficaLineas = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDatos.Location = new System.Drawing.Point(0, 60);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.ReadOnly = true;
            this.dgvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDatos.Size = new System.Drawing.Size(1000, 490);
            this.dgvDatos.TabIndex = 0;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnLoadFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadFile.ForeColor = System.Drawing.Color.White;
            this.btnLoadFile.Location = new System.Drawing.Point(12, 12);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(120, 35);
            this.btnLoadFile.TabIndex = 1;
            this.btnLoadFile.Text = "📁 Load File";
            this.btnLoadFile.UseVisualStyleBackColor = false;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnGraficaDona
            // 
            this.btnGraficaDona.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btnGraficaDona.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraficaDona.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGraficaDona.ForeColor = System.Drawing.Color.White;
            this.btnGraficaDona.Location = new System.Drawing.Point(150, 12);
            this.btnGraficaDona.Name = "btnGraficaDona";
            this.btnGraficaDona.Size = new System.Drawing.Size(150, 35);
            this.btnGraficaDona.TabIndex = 2;
            this.btnGraficaDona.Text = "🍩 Gráfica de Dona";
            this.btnGraficaDona.UseVisualStyleBackColor = false;
            this.btnGraficaDona.Click += new System.EventHandler(this.btnGraficaDona_Click);
            // 
            // btnGraficaBarras
            // 
            this.btnGraficaBarras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnGraficaBarras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraficaBarras.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGraficaBarras.ForeColor = System.Drawing.Color.White;
            this.btnGraficaBarras.Location = new System.Drawing.Point(315, 12);
            this.btnGraficaBarras.Name = "btnGraficaBarras";
            this.btnGraficaBarras.Size = new System.Drawing.Size(150, 35);
            this.btnGraficaBarras.TabIndex = 3;
            this.btnGraficaBarras.Text = "📊 Gráfica de Barras";
            this.btnGraficaBarras.UseVisualStyleBackColor = false;
            this.btnGraficaBarras.Click += new System.EventHandler(this.btnGraficaBarras_Click);
            // 
            // btnGraficaLineas
            // 
            this.btnGraficaLineas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnGraficaLineas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraficaLineas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGraficaLineas.ForeColor = System.Drawing.Color.White;
            this.btnGraficaLineas.Location = new System.Drawing.Point(480, 12);
            this.btnGraficaLineas.Name = "btnGraficaLineas";
            this.btnGraficaLineas.Size = new System.Drawing.Size(150, 35);
            this.btnGraficaLineas.TabIndex = 4;
            this.btnGraficaLineas.Text = "📈 Gráfica de Líneas";
            this.btnGraficaLineas.UseVisualStyleBackColor = false;
            this.btnGraficaLineas.Click += new System.EventHandler(this.btnGraficaLineas_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.btnLoadFile);
            this.panel1.Controls.Add(this.btnGraficaLineas);
            this.panel1.Controls.Add(this.btnGraficaDona);
            this.panel1.Controls.Add(this.btnGraficaBarras);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 60);
            this.panel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this.dgvDatos);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visualizador de Datos - Gráficas Interactivas";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button btnGraficaDona;
        private System.Windows.Forms.Button btnGraficaBarras;
        private System.Windows.Forms.Button btnGraficaLineas;
        private System.Windows.Forms.Panel panel1;
    }
}
