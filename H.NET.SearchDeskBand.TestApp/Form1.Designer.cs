using H.NET.SearchDeskBand;

namespace SearchDeskBand.TestApp
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
            this.deskBandControl1 = new DeskBandControl();
            this.SuspendLayout();
            // 
            // deskBandControl1
            // 
            this.deskBandControl1.BackColor = System.Drawing.Color.White;
            this.deskBandControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deskBandControl1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.deskBandControl1.Location = new System.Drawing.Point(271, 384);
            this.deskBandControl1.MaximumSize = new System.Drawing.Size(262, 40);
            this.deskBandControl1.MinimumSize = new System.Drawing.Size(262, 40);
            this.deskBandControl1.Name = "deskBandControl1";
            this.deskBandControl1.Padding = new System.Windows.Forms.Padding(6);
            this.deskBandControl1.Size = new System.Drawing.Size(262, 40);
            this.deskBandControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.deskBandControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DeskBandControl deskBandControl1;
    }
}

