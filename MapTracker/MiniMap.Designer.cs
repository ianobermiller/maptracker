namespace MapTracker
{
    partial class MiniMap
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
            this.uxMap = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.uxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // uxMap
            // 
            this.uxMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxMap.Location = new System.Drawing.Point(0, 0);
            this.uxMap.Name = "uxMap";
            this.uxMap.Size = new System.Drawing.Size(290, 268);
            this.uxMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.uxMap.TabIndex = 0;
            this.uxMap.TabStop = false;
            this.uxMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.uxMap_MouseMove);
            // 
            // MiniMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 268);
            this.Controls.Add(this.uxMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MiniMap";
            this.Text = "MiniMap";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MiniMap_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.uxMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox uxMap;
    }
}