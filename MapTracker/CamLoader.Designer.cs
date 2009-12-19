namespace MapTracker
{
    partial class CamLoader
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
            this.uxProgess = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // uxProgess
            // 
            this.uxProgess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxProgess.Location = new System.Drawing.Point(0, 0);
            this.uxProgess.Name = "uxProgess";
            this.uxProgess.Size = new System.Drawing.Size(288, 28);
            this.uxProgess.TabIndex = 0;
            // 
            // CamLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 28);
            this.Controls.Add(this.uxProgess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CamLoader";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tracking CAM file...";
            this.Shown += new System.EventHandler(this.CamLoader_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar uxProgess;
    }
}