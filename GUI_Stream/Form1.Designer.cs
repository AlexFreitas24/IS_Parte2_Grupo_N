namespace GUI_Stream
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
            labelTotal = new Label();
            labelOk = new Label();
            listViewPecas = new ListView();
            labelFalha = new Label();
            labelTempoMedio = new Label();
            SuspendLayout();
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Location = new Point(72, 19);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(44, 15);
            labelTotal.TabIndex = 0;
            labelTotal.Text = "Total: 0";
            // 
            // labelOk
            // 
            labelOk.AutoSize = true;
            labelOk.Location = new Point(196, 19);
            labelOk.Name = "labelOk";
            labelOk.Size = new Size(35, 15);
            labelOk.TabIndex = 1;
            labelOk.Text = "OK: 0";
            // 
            // listViewPecas
            // 
            listViewPecas.Location = new Point(38, 63);
            listViewPecas.Name = "listViewPecas";
            listViewPecas.Size = new Size(508, 351);
            listViewPecas.TabIndex = 2;
            listViewPecas.UseCompatibleStateImageBehavior = false;
            // 
            // labelFalha
            // 
            labelFalha.AutoSize = true;
            labelFalha.Location = new Point(325, 19);
            labelFalha.Name = "labelFalha";
            labelFalha.Size = new Size(52, 15);
            labelFalha.TabIndex = 3;
            labelFalha.Text = "Falhas: 0";
            // 
            // labelTempoMedio
            // 
            labelTempoMedio.AutoSize = true;
            labelTempoMedio.Location = new Point(451, 19);
            labelTempoMedio.Name = "labelTempoMedio";
            labelTempoMedio.Size = new Size(57, 15);
            labelTempoMedio.TabIndex = 4;
            labelTempoMedio.Text = "Média: 0s";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(646, 450);
            Controls.Add(labelTempoMedio);
            Controls.Add(labelFalha);
            Controls.Add(listViewPecas);
            Controls.Add(labelOk);
            Controls.Add(labelTotal);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelTotal;
        private Label labelOk;
        private ListView listViewPecas;
        private Label labelFalha;
        private Label labelTempoMedio;
    }
}
