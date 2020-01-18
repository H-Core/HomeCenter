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
            this.InputPanel = new System.Windows.Forms.Panel();
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.HistoryTabPage = new System.Windows.Forms.TabPage();
            this.ClearHistoryButton = new System.Windows.Forms.Button();
            this.HistoryListBox = new System.Windows.Forms.ListBox();
            this.InputPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.HistoryTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputPanel
            // 
            this.InputPanel.BackColor = System.Drawing.Color.White;
            this.InputPanel.Controls.Add(this.InputTextBox);
            this.InputPanel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.InputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputPanel.Location = new System.Drawing.Point(0, 441);
            this.InputPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.InputPanel.Name = "InputPanel";
            this.InputPanel.Padding = new System.Windows.Forms.Padding(9, 11, 9, 9);
            this.InputPanel.Size = new System.Drawing.Size(477, 59);
            this.InputPanel.TabIndex = 0;
            this.InputPanel.Click += new System.EventHandler(this.Panel_Click);
            // 
            // InputTextBox
            // 
            this.InputTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.InputTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.InputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputTextBox.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.InputTextBox.ForeColor = System.Drawing.Color.Navy;
            this.InputTextBox.Location = new System.Drawing.Point(9, 11);
            this.InputTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(459, 39);
            this.InputTextBox.TabIndex = 1;
            this.InputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            // 
            // TabControl
            // 
            this.TabControl.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.TabControl.Controls.Add(this.HistoryTabPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(477, 441);
            this.TabControl.TabIndex = 1;
            // 
            // HistoryTabPage
            // 
            this.HistoryTabPage.Controls.Add(this.ClearHistoryButton);
            this.HistoryTabPage.Controls.Add(this.HistoryListBox);
            this.HistoryTabPage.Location = new System.Drawing.Point(4, 4);
            this.HistoryTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HistoryTabPage.Name = "HistoryTabPage";
            this.HistoryTabPage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HistoryTabPage.Size = new System.Drawing.Size(445, 433);
            this.HistoryTabPage.TabIndex = 0;
            this.HistoryTabPage.Text = "History";
            this.HistoryTabPage.UseVisualStyleBackColor = true;
            // 
            // ClearHistoryButton
            // 
            this.ClearHistoryButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ClearHistoryButton.Location = new System.Drawing.Point(4, 380);
            this.ClearHistoryButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ClearHistoryButton.Name = "ClearHistoryButton";
            this.ClearHistoryButton.Size = new System.Drawing.Size(437, 48);
            this.ClearHistoryButton.TabIndex = 1;
            this.ClearHistoryButton.Text = "Clear History";
            this.ClearHistoryButton.UseVisualStyleBackColor = true;
            this.ClearHistoryButton.Click += new System.EventHandler(this.ClearHistoryButton_Click);
            // 
            // HistoryListBox
            // 
            this.HistoryListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HistoryListBox.FormattingEnabled = true;
            this.HistoryListBox.ItemHeight = 20;
            this.HistoryListBox.Location = new System.Drawing.Point(4, 5);
            this.HistoryListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HistoryListBox.Name = "HistoryListBox";
            this.HistoryListBox.Size = new System.Drawing.Size(437, 364);
            this.HistoryListBox.TabIndex = 0;
            this.HistoryListBox.DoubleClick += new System.EventHandler(this.HistoryListBox_DoubleClick);
            // 
            // DeskBandWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 500);
            this.ControlBox = false;
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.InputPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.InputPanel.ResumeLayout(false);
            this.InputPanel.PerformLayout();
            this.TabControl.ResumeLayout(false);
            this.HistoryTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel InputPanel;
        private System.Windows.Forms.TextBox InputTextBox;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage HistoryTabPage;
        private System.Windows.Forms.ListBox HistoryListBox;
        private System.Windows.Forms.Button ClearHistoryButton;
    }
}