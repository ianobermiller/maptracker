namespace MapTracker.NET
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.cbClientList = new System.Windows.Forms.ComboBox();
            this.btnTracking = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbClientList
            // 
            this.cbClientList.FormattingEnabled = true;
            this.cbClientList.Location = new System.Drawing.Point(12, 12);
            this.cbClientList.Name = "cbClientList";
            this.cbClientList.Size = new System.Drawing.Size(167, 21);
            this.cbClientList.TabIndex = 1;
            this.cbClientList.SelectedIndexChanged += new System.EventHandler(this.cbClientList_SelectedIndexChanged);
            // 
            // btnTracking
            // 
            this.btnTracking.Location = new System.Drawing.Point(185, 12);
            this.btnTracking.Name = "btnTracking";
            this.btnTracking.Size = new System.Drawing.Size(167, 23);
            this.btnTracking.TabIndex = 11;
            this.btnTracking.Text = "Start Map Tracking";
            this.btnTracking.UseVisualStyleBackColor = true;
            this.btnTracking.Click += new System.EventHandler(this.btnTracking_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 41);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(340, 121);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 175);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnTracking);
            this.Controls.Add(this.cbClientList);
            this.Name = "frmMain";
            this.Text = "MapTracker.NET (under TibiaAPI)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbClientList;
        private System.Windows.Forms.Button btnTracking;
        private System.Windows.Forms.TextBox textBox1;
    }
}

