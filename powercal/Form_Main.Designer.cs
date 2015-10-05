namespace powercal
{
    partial class Form_Main
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
            this.textBoxOutputStatus = new System.Windows.Forms.TextBox();
            this.contextMenuStatusTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRun = new System.Windows.Forms.Button();
            this.comboBoxBoardTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fT232HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPowerMeter = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxRunStatus = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer_Update_Idle = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStatusTextBox.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxOutputStatus
            // 
            this.textBoxOutputStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutputStatus.ContextMenuStrip = this.contextMenuStatusTextBox;
            this.textBoxOutputStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutputStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxOutputStatus.Location = new System.Drawing.Point(12, 126);
            this.textBoxOutputStatus.Multiline = true;
            this.textBoxOutputStatus.Name = "textBoxOutputStatus";
            this.textBoxOutputStatus.ReadOnly = true;
            this.textBoxOutputStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutputStatus.Size = new System.Drawing.Size(808, 407);
            this.textBoxOutputStatus.TabIndex = 2;
            this.textBoxOutputStatus.WordWrap = false;
            // 
            // contextMenuStatusTextBox
            // 
            this.contextMenuStatusTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuStatusTextBox.Name = "contextMenuReceivedTextBox";
            this.contextMenuStatusTextBox.Size = new System.Drawing.Size(102, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_Clear);
            // 
            // buttonRun
            // 
            this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRun.Location = new System.Drawing.Point(745, 35);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 6;
            this.buttonRun.Text = "&Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonClick_Run);
            // 
            // comboBoxBoardTypes
            // 
            this.comboBoxBoardTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBoardTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxBoardTypes.FormattingEnabled = true;
            this.comboBoxBoardTypes.Location = new System.Drawing.Point(57, 27);
            this.comboBoxBoardTypes.Name = "comboBoxBoardTypes";
            this.comboBoxBoardTypes.Size = new System.Drawing.Size(170, 28);
            this.comboBoxBoardTypes.TabIndex = 7;
            this.comboBoxBoardTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxBoardTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Board:";
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.testToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(832, 24);
            this.menuStripMain.TabIndex = 9;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_Settings);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serialToolStripMenuItem,
            this.digitalOutputToolStripMenuItem,
            this.calculatorToolStripMenuItem,
            this.toolStripMenuItemPowerMeter});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.testToolStripMenuItem.Text = "Tools";
            // 
            // serialToolStripMenuItem
            // 
            this.serialToolStripMenuItem.Name = "serialToolStripMenuItem";
            this.serialToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.serialToolStripMenuItem.Text = "Serial Port";
            this.serialToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_Serial);
            // 
            // digitalOutputToolStripMenuItem
            // 
            this.digitalOutputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nIToolStripMenuItem,
            this.fT232HToolStripMenuItem});
            this.digitalOutputToolStripMenuItem.Name = "digitalOutputToolStripMenuItem";
            this.digitalOutputToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.digitalOutputToolStripMenuItem.Text = "Digital Output";
            // 
            // nIToolStripMenuItem
            // 
            this.nIToolStripMenuItem.Name = "nIToolStripMenuItem";
            this.nIToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.nIToolStripMenuItem.Text = "NI";
            this.nIToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_NI);
            // 
            // fT232HToolStripMenuItem
            // 
            this.fT232HToolStripMenuItem.Name = "fT232HToolStripMenuItem";
            this.fT232HToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.fT232HToolStripMenuItem.Text = "FT232H";
            this.fT232HToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_FT232H);
            // 
            // calculatorToolStripMenuItem
            // 
            this.calculatorToolStripMenuItem.Name = "calculatorToolStripMenuItem";
            this.calculatorToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.calculatorToolStripMenuItem.Text = "&Calculator";
            this.calculatorToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_Calculator);
            // 
            // toolStripMenuItemPowerMeter
            // 
            this.toolStripMenuItemPowerMeter.Name = "toolStripMenuItemPowerMeter";
            this.toolStripMenuItemPowerMeter.Size = new System.Drawing.Size(149, 22);
            this.toolStripMenuItemPowerMeter.Text = "Power Meter";
            this.toolStripMenuItemPowerMeter.Click += new System.EventHandler(this.toolStripMenuItem_Click_PowerMeter);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click_About);
            // 
            // textBoxRunStatus
            // 
            this.textBoxRunStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRunStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRunStatus.Location = new System.Drawing.Point(12, 71);
            this.textBoxRunStatus.Name = "textBoxRunStatus";
            this.textBoxRunStatus.ReadOnly = true;
            this.textBoxRunStatus.Size = new System.Drawing.Size(808, 49);
            this.textBoxRunStatus.TabIndex = 10;
            this.textBoxRunStatus.Text = "test";
            this.textBoxRunStatus.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(832, 22);
            this.statusStrip1.TabIndex = 11;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(817, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer_Update_Idle
            // 
            this.timer_Update_Idle.Enabled = true;
            this.timer_Update_Idle.Interval = 1000;
            this.timer_Update_Idle.Tick += new System.EventHandler(this.timer_Update_Idle_Tick);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(832, 557);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxRunStatus);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxBoardTypes);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.textBoxOutputStatus);
            this.MainMenuStrip = this.menuStripMain;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "Form_Main";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.Form_Main_Shown);
            this.contextMenuStatusTextBox.ResumeLayout(false);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOutputStatus;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.ComboBox comboBoxBoardTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStatusTextBox;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serialToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxRunStatus;
        private System.Windows.Forms.ToolStripMenuItem digitalOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPowerMeter;
        private System.Windows.Forms.ToolStripMenuItem nIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fT232HToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Timer timer_Update_Idle;
    }
}

