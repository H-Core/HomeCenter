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
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.RecordButton = new System.Windows.Forms.Button();
            this.uiButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Label
            // 
            this.Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Label.Location = new System.Drawing.Point(6, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(304, 26);
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
            this.menuButton.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuButton.ImageIndex = 1;
            this.menuButton.ImageList = this.imageList;
            this.menuButton.Location = new System.Drawing.Point(232, 3);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(25, 32);
            this.menuButton.TabIndex = 1;
            this.menuButton.UseVisualStyleBackColor = true;
            this.menuButton.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "record.png");
            this.imageList.Images.SetKeyName(1, "menu.png");
            this.imageList.Images.SetKeyName(2, "ui.png");
            this.imageList.Images.SetKeyName(3, "settings.png");
            // 
            // RecordButton
            // 
            this.RecordButton.BackColor = System.Drawing.Color.White;
            this.RecordButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.RecordButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.RecordButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PowderBlue;
            this.RecordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RecordButton.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RecordButton.ImageIndex = 0;
            this.RecordButton.ImageList = this.imageList;
            this.RecordButton.Location = new System.Drawing.Point(204, 3);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(25, 32);
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
            this.uiButton.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.uiButton.ImageIndex = 2;
            this.uiButton.ImageList = this.imageList;
            this.uiButton.Location = new System.Drawing.Point(288, 3);
            this.uiButton.Name = "uiButton";
            this.uiButton.Size = new System.Drawing.Size(25, 32);
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
            this.settingsButton.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.settingsButton.ImageIndex = 3;
            this.settingsButton.ImageList = this.imageList;
            this.settingsButton.Location = new System.Drawing.Point(260, 3);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(25, 32);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // DeskBandControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.uiButton);
            this.Controls.Add(this.RecordButton);
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.Label);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MaximumSize = new System.Drawing.Size(318, 40);
            this.MinimumSize = new System.Drawing.Size(318, 40);
            this.Name = "DeskBandControl";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Size = new System.Drawing.Size(316, 38);
            this.Click += new System.EventHandler(this.OnClick);
            this.Load += new System.EventHandler(this.DeskBandControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button uiButton;
        private System.Windows.Forms.Button settingsButton;
    }
}
