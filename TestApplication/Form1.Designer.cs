namespace TestApplication_
{
    partial class Form1
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
            this.cphControl1 = new CPHControl.CPHControl();
            this.SuspendLayout();
            // 
            // cphControl1
            // 
            this.cphControl1.BackColor = System.Drawing.Color.Black;
            this.cphControl1.Location = new System.Drawing.Point(12, 11);
            this.cphControl1.Name = "cphControl1";
            this.cphControl1.Size = new System.Drawing.Size(1036, 592);
            this.cphControl1.TabIndex = 0;
            this.cphControl1.VSync = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 615);
            this.Controls.Add(this.cphControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CPHControl.CPHControl cphControl1;
    }
}

