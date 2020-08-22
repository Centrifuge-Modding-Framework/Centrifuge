namespace Centrifuge.Installer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this._pathTextBox = new System.Windows.Forms.TextBox();
            this.browseGameButton = new System.Windows.Forms.Button();
            this.dotnetStandardCheckBox = new System.Windows.Forms.CheckBox();
            this._installButton = new System.Windows.Forms.Button();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._mainPanel = new System.Windows.Forms.Panel();
            this._checkBoxEnableFreeTextInput = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this._toolStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.checkBoxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._mainPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game location";
            // 
            // _pathTextBox
            // 
            this._pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._pathTextBox.Location = new System.Drawing.Point(7, 24);
            this._pathTextBox.Name = "_pathTextBox";
            this._pathTextBox.ReadOnly = true;
            this._pathTextBox.Size = new System.Drawing.Size(242, 20);
            this._pathTextBox.TabIndex = 2;
            this._pathTextBox.TextChanged += new System.EventHandler(this.PathTextBox_TextChanged);
            // 
            // browseGameButton
            // 
            this.browseGameButton.Location = new System.Drawing.Point(255, 24);
            this.browseGameButton.Name = "browseGameButton";
            this.browseGameButton.Size = new System.Drawing.Size(28, 20);
            this.browseGameButton.TabIndex = 3;
            this.browseGameButton.Text = "...";
            this.browseGameButton.UseVisualStyleBackColor = true;
            this.browseGameButton.Click += new System.EventHandler(this.BrowseGameButton_Click);
            // 
            // dotnetStandardCheckBox
            // 
            this.dotnetStandardCheckBox.AutoSize = true;
            this.dotnetStandardCheckBox.Location = new System.Drawing.Point(7, 53);
            this.dotnetStandardCheckBox.Name = "dotnetStandardCheckBox";
            this.dotnetStandardCheckBox.Size = new System.Drawing.Size(152, 17);
            this.dotnetStandardCheckBox.TabIndex = 4;
            this.dotnetStandardCheckBox.Text = "Install .NET Standard build";
            this.checkBoxToolTip.SetToolTip(this.dotnetStandardCheckBox, "Check only if your game uses Unity Engine 2017+");
            this.dotnetStandardCheckBox.UseVisualStyleBackColor = true;
            // 
            // _installButton
            // 
            this._installButton.Enabled = false;
            this._installButton.Location = new System.Drawing.Point(208, 69);
            this._installButton.Name = "_installButton";
            this._installButton.Size = new System.Drawing.Size(75, 23);
            this._installButton.TabIndex = 5;
            this._installButton.Text = "Install";
            this._installButton.UseVisualStyleBackColor = true;
            this._installButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // _mainPanel
            // 
            this._mainPanel.Controls.Add(this._checkBoxEnableFreeTextInput);
            this._mainPanel.Controls.Add(this.statusStrip);
            this._mainPanel.Controls.Add(this._installButton);
            this._mainPanel.Controls.Add(this.label1);
            this._mainPanel.Controls.Add(this.dotnetStandardCheckBox);
            this._mainPanel.Controls.Add(this._pathTextBox);
            this._mainPanel.Controls.Add(this.browseGameButton);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(289, 118);
            this._mainPanel.TabIndex = 6;
            // 
            // _checkBoxEnableFreeTextInput
            // 
            this._checkBoxEnableFreeTextInput.AutoSize = true;
            this._checkBoxEnableFreeTextInput.Location = new System.Drawing.Point(7, 75);
            this._checkBoxEnableFreeTextInput.Name = "_checkBoxEnableFreeTextInput";
            this._checkBoxEnableFreeTextInput.Size = new System.Drawing.Size(129, 17);
            this._checkBoxEnableFreeTextInput.TabIndex = 7;
            this._checkBoxEnableFreeTextInput.Text = "I know what I\'m doing";
            this.checkBoxToolTip.SetToolTip(this._checkBoxEnableFreeTextInput, "Check to enable free input in the path textbox");
            this._checkBoxEnableFreeTextInput.UseVisualStyleBackColor = true;
            this._checkBoxEnableFreeTextInput.CheckedChanged += new System.EventHandler(this.CheckBoxIknowWhatImDoing_CheckedChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this._toolStripLabel, this._toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 96);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(289, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 6;
            // 
            // _toolStripLabel
            // 
            this._toolStripLabel.Name = "_toolStripLabel";
            this._toolStripLabel.Size = new System.Drawing.Size(133, 17);
            this._toolStripLabel.Text = "Latest release: unknown";
            // 
            // _toolStripProgressBar
            // 
            this._toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._toolStripProgressBar.AutoSize = false;
            this._toolStripProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 1, 3);
            this._toolStripProgressBar.MarqueeAnimationSpeed = 50;
            this._toolStripProgressBar.Name = "_toolStripProgressBar";
            this._toolStripProgressBar.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this._toolStripProgressBar.Size = new System.Drawing.Size(100, 15);
            this._toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this._toolStripProgressBar.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 118);
            this.Controls.Add(this._mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Centrifuge Installer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._mainPanel.ResumeLayout(false);
            this._mainPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _pathTextBox;
        private System.Windows.Forms.Button browseGameButton;
        private System.Windows.Forms.CheckBox dotnetStandardCheckBox;
        private System.Windows.Forms.Button _installButton;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _toolStripLabel;
        private System.Windows.Forms.ToolStripProgressBar _toolStripProgressBar;
        private System.Windows.Forms.ToolTip checkBoxToolTip;
        private System.Windows.Forms.CheckBox _checkBoxEnableFreeTextInput;
    }
}