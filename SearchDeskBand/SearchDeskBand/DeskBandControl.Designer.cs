namespace H.NET.SearchDeskBand
{
    partial class DeskBandControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskBandControl));
            this.Label = new System.Windows.Forms.Label();
            this.menuButton = new System.Windows.Forms.Button();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.RecordButton = new System.Windows.Forms.Button();
            this.uiButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Label
            // 
            this.Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label.ForeColor = System.Drawing.Color.Gray;
            this.Label.Location = new System.Drawing.Point(9, 9);
            this.Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(456, 40);
            this.Label.TabIndex = 0;
            this.Label.Text = "Enter Command Here";
            this.Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label.Click += new System.EventHandler(this.OnClick);
            // 
            // menuButton
            // 
            this.menuButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.menuButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.menuButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PowderBlue;
            this.menuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuButton.ImageIndex = 1;
            this.menuButton.ImageList = this.ImageList;
            this.menuButton.Location = new System.Drawing.Point(348, 5);
            this.menuButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(38, 49);
            this.menuButton.TabIndex = 1;
            this.menuButton.UseVisualStyleBackColor = true;
            this.menuButton.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "record.png");
            this.ImageList.Images.SetKeyName(1, "menu.png");
            this.ImageList.Images.SetKeyName(2, "ui.png");
            this.ImageList.Images.SetKeyName(3, "settings.png");
            // 
            // RecordButton
            // 
            this.RecordButton.BackColor = System.Drawing.Color.White;
            this.RecordButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.RecordButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.RecordButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PowderBlue;
            this.RecordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RecordButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RecordButton.ImageIndex = 0;
            this.RecordButton.ImageList = this.ImageList;
            this.RecordButton.Location = new System.Drawing.Point(306, 5);
            this.RecordButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(38, 49);
            this.RecordButton.TabIndex = 2;
            this.RecordButton.UseVisualStyleBackColor = false;
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // uiButton
            // 
            this.uiButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.uiButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.uiButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PowderBlue;
            this.uiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.uiButton.ImageIndex = 2;
            this.uiButton.ImageList = this.ImageList;
            this.uiButton.Location = new System.Drawing.Point(432, 5);
            this.uiButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiButton.Name = "uiButton";
            this.uiButton.Size = new System.Drawing.Size(38, 49);
            this.uiButton.TabIndex = 3;
            this.uiButton.UseVisualStyleBackColor = true;
            this.uiButton.Click += new System.EventHandler(this.UiButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.settingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PowderBlue;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsButton.ImageIndex = 3;
            this.settingsButton.ImageList = this.ImageList;
            this.settingsButton.Location = new System.Drawing.Point(390, 5);
            this.settingsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(38, 49);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // DeskBandControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.uiButton);
            this.Controls.Add(this.RecordButton);
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.Label);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(476, 60);
            this.MinimumSize = new System.Drawing.Size(476, 60);
            this.Name = "DeskBandControl";
            this.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.Size = new System.Drawing.Size(474, 58);
            this.Load += new System.EventHandler(this.DeskBandControl_Load);
            this.Click += new System.EventHandler(this.OnClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.Button uiButton;
        private System.Windows.Forms.Button settingsButton;
    }
}
