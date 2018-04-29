namespace H.NET.SearchDeskBand
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
            this.TextBox = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.historyTabPage = new System.Windows.Forms.TabPage();
            this.clearHistoryButton = new System.Windows.Forms.Button();
            this.historyListBox = new System.Windows.Forms.ListBox();
            this.Panel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.historyTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.BackColor = System.Drawing.Color.White;
            this.Panel.Controls.Add(this.TextBox);
            this.Panel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel.Location = new System.Drawing.Point(0, 285);
            this.Panel.Name = "Panel";
            this.Panel.Padding = new System.Windows.Forms.Padding(6, 7, 6, 6);
            this.Panel.Size = new System.Drawing.Size(318, 40);
            this.Panel.TabIndex = 0;
            this.Panel.Click += new System.EventHandler(this.Panel_Click);
            // 
            // TextBox
            // 
            this.TextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TextBox.ForeColor = System.Drawing.Color.Navy;
            this.TextBox.Location = new System.Drawing.Point(6, 7);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(306, 26);
            this.TextBox.TabIndex = 1;
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl.Controls.Add(this.historyTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(318, 285);
            this.tabControl.TabIndex = 1;
            // 
            // historyTabPage
            // 
            this.historyTabPage.Controls.Add(this.clearHistoryButton);
            this.historyTabPage.Controls.Add(this.historyListBox);
            this.historyTabPage.Location = new System.Drawing.Point(4, 4);
            this.historyTabPage.Name = "historyTabPage";
            this.historyTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.historyTabPage.Size = new System.Drawing.Size(291, 277);
            this.historyTabPage.TabIndex = 0;
            this.historyTabPage.Text = "History";
            this.historyTabPage.UseVisualStyleBackColor = true;
            // 
            // clearHistoryButton
            // 
            this.clearHistoryButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.clearHistoryButton.Location = new System.Drawing.Point(3, 243);
            this.clearHistoryButton.Name = "clearHistoryButton";
            this.clearHistoryButton.Size = new System.Drawing.Size(285, 31);
            this.clearHistoryButton.TabIndex = 1;
            this.clearHistoryButton.Text = "Clear History";
            this.clearHistoryButton.UseVisualStyleBackColor = true;
            this.clearHistoryButton.Click += new System.EventHandler(this.ClearHistoryButton_Click);
            // 
            // historyListBox
            // 
            this.historyListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.historyListBox.FormattingEnabled = true;
            this.historyListBox.Location = new System.Drawing.Point(3, 3);
            this.historyListBox.Name = "historyListBox";
            this.historyListBox.Size = new System.Drawing.Size(285, 238);
            this.historyListBox.TabIndex = 0;
            this.historyListBox.DoubleClick += new System.EventHandler(this.HistoryListBox_DoubleClick);
            // 
            // DeskBandWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 325);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeskBandWindow";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DeskBandWindow";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.DeskBandWindow_Activated);
            this.Deactivate += new System.EventHandler(this.DeskBandWindow_Deactivate);
            this.Panel.ResumeLayout(false);
            this.Panel.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.historyTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.TextBox TextBox;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage historyTabPage;
        private System.Windows.Forms.ListBox historyListBox;
        private System.Windows.Forms.Button clearHistoryButton;
    }
}