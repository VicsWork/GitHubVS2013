namespace PowerCalibration
{
    partial class Form_SerialTest
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
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.textBoxSend = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxReceive = new System.Windows.Forms.TextBox();
            this.contextMenuReceivedTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxHex = new System.Windows.Forms.CheckBox();
            this.buttonRefreshPorts = new System.Windows.Forms.Button();
            this.textBoxBaud = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxdataBits = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxHardwareFlow = new System.Windows.Forms.CheckBox();
            this.contextMenuReceivedTextBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(10, 24);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(77, 21);
            this.comboBoxPorts.Sorted = true;
            this.comboBoxPorts.TabIndex = 0;
            this.comboBoxPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxPorts_SelectedIndexChanged);
            // 
            // textBoxSend
            // 
            this.textBoxSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSend.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxSend.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxSend.Location = new System.Drawing.Point(14, 66);
            this.textBoxSend.Name = "textBoxSend";
            this.textBoxSend.Size = new System.Drawing.Size(430, 20);
            this.textBoxSend.TabIndex = 1;
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Location = new System.Drawing.Point(501, 66);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(74, 23);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "&Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Send:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Received:";
            // 
            // textBoxReceive
            // 
            this.textBoxReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReceive.ContextMenuStrip = this.contextMenuReceivedTextBox;
            this.textBoxReceive.Location = new System.Drawing.Point(12, 116);
            this.textBoxReceive.Multiline = true;
            this.textBoxReceive.Name = "textBoxReceive";
            this.textBoxReceive.Size = new System.Drawing.Size(561, 367);
            this.textBoxReceive.TabIndex = 5;
            // 
            // contextMenuReceivedTextBox
            // 
            this.contextMenuReceivedTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuReceivedTextBox.Name = "contextMenuReceivedTextBox";
            this.contextMenuReceivedTextBox.Size = new System.Drawing.Size(102, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(253, 500);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxHex
            // 
            this.checkBoxHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxHex.AutoSize = true;
            this.checkBoxHex.Location = new System.Drawing.Point(450, 70);
            this.checkBoxHex.Name = "checkBoxHex";
            this.checkBoxHex.Size = new System.Drawing.Size(45, 17);
            this.checkBoxHex.TabIndex = 7;
            this.checkBoxHex.Text = "Hex";
            this.checkBoxHex.UseVisualStyleBackColor = true;
            // 
            // buttonRefreshPorts
            // 
            this.buttonRefreshPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefreshPorts.Location = new System.Drawing.Point(501, 22);
            this.buttonRefreshPorts.Name = "buttonRefreshPorts";
            this.buttonRefreshPorts.Size = new System.Drawing.Size(72, 23);
            this.buttonRefreshPorts.TabIndex = 9;
            this.buttonRefreshPorts.Text = "&Refresh";
            this.buttonRefreshPorts.UseVisualStyleBackColor = true;
            this.buttonRefreshPorts.Click += new System.EventHandler(this.buttonRefreshPorts_Click);
            // 
            // textBoxBaud
            // 
            this.textBoxBaud.Location = new System.Drawing.Point(111, 24);
            this.textBoxBaud.Name = "textBoxBaud";
            this.textBoxBaud.Size = new System.Drawing.Size(61, 20);
            this.textBoxBaud.TabIndex = 10;
            this.textBoxBaud.Text = "600";
            this.textBoxBaud.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Baud Rate:";
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Location = new System.Drawing.Point(181, 23);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(65, 21);
            this.comboBoxParity.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(178, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Parity:";
            // 
            // textBoxdataBits
            // 
            this.textBoxdataBits.Location = new System.Drawing.Point(252, 25);
            this.textBoxdataBits.Name = "textBoxdataBits";
            this.textBoxdataBits.Size = new System.Drawing.Size(33, 20);
            this.textBoxdataBits.TabIndex = 15;
            this.textBoxdataBits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(249, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "DBits:";
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Location = new System.Drawing.Point(291, 23);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(100, 21);
            this.comboBoxStopBits.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(288, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "SBits:";
            // 
            // checkBoxHardwareFlow
            // 
            this.checkBoxHardwareFlow.AutoSize = true;
            this.checkBoxHardwareFlow.Checked = true;
            this.checkBoxHardwareFlow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHardwareFlow.Location = new System.Drawing.Point(398, 26);
            this.checkBoxHardwareFlow.Name = "checkBoxHardwareFlow";
            this.checkBoxHardwareFlow.Size = new System.Drawing.Size(97, 17);
            this.checkBoxHardwareFlow.TabIndex = 19;
            this.checkBoxHardwareFlow.Text = "Hardware Flow";
            this.checkBoxHardwareFlow.UseVisualStyleBackColor = true;
            // 
            // Form_SerialTest
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 535);
            this.Controls.Add(this.checkBoxHardwareFlow);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBoxStopBits);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxdataBits);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxParity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxBaud);
            this.Controls.Add(this.buttonRefreshPorts);
            this.Controls.Add(this.checkBoxHex);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxReceive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxSend);
            this.Controls.Add(this.comboBoxPorts);
            this.MinimumSize = new System.Drawing.Size(605, 300);
            this.Name = "Form_SerialTest";
            this.Text = "Serial Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSerialTest_FormClosing);
            this.contextMenuReceivedTextBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.TextBox textBoxSend;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxReceive;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxHex;
        private System.Windows.Forms.ContextMenuStrip contextMenuReceivedTextBox;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Button buttonRefreshPorts;
        private System.Windows.Forms.TextBox textBoxBaud;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxdataBits;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxHardwareFlow;
    }
}