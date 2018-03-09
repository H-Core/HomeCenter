namespace SearchDeskBand
{
    partial class DeskBandWindow
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
            this.Panel = new System.Windows.Forms.Panel();
            this.TextBox = new System.Windows.Forms.RichTextBox();
            this.Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.BackColor = System.Drawing.Color.White;
            this.Panel.Controls.Add(this.TextBox);
            this.Panel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Panel.Location = new System.Drawing.Point(0, 284);
            this.Panel.Name = "Panel";
            this.Panel.Padding = new System.Windows.Forms.Padding(7);
            this.Panel.Size = new System.Drawing.Size(264, 40);
            this.Panel.TabIndex = 0;
            this.Panel.Click += new System.EventHandler(this.Panel_Click);
            // 
            // TextBox
            // 
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TextBox.ForeColor = System.Drawing.Color.Navy;
            this.TextBox.Location = new System.Drawing.Point(7, 7);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(250, 26);
            this.TextBox.TabIndex = 1;
            this.TextBox.Text = "";
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            // 
            // DeskBandWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.ClientSize = new System.Drawing.Size(264, 325);
            this.ControlBox = false;
            this.Controls.Add(this.Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeskBandWindow";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DeskBandWindow";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.DeskBandWindow_Activated);
            this.Deactivate += new System.EventHandler(this.DeskBandWindow_Deactivate);
            this.Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.RichTextBox TextBox;
    }
}