﻿namespace powercal
{
    partial class FormDigitalPortTest
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
            this.channelParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.physicalChannelComboBox = new System.Windows.Forms.ComboBox();
            this.physicalChannelLabel = new System.Windows.Forms.Label();
            this.writeButton = new System.Windows.Forms.Button();
            this.dataLabel = new System.Windows.Forms.Label();
            this.NumericUpDowndataToWrite = new System.Windows.Forms.NumericUpDown();
            this.groupBoxDIOLines = new System.Windows.Forms.GroupBox();
            this.labelOutput = new System.Windows.Forms.Label();
            this.NumericUpDownOutput = new System.Windows.Forms.NumericUpDown();
            this.labelReset = new System.Windows.Forms.Label();
            this.NumericUpDownReset = new System.Windows.Forms.NumericUpDown();
            this.labelLoad = new System.Windows.Forms.Label();
            this.NumericUpDownLoad = new System.Windows.Forms.NumericUpDown();
            this.labelACPower = new System.Windows.Forms.Label();
            this.NumericUpDownACPower = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.channelParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDowndataToWrite)).BeginInit();
            this.groupBoxDIOLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownLoad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownACPower)).BeginInit();
            this.SuspendLayout();
            // 
            // channelParametersGroupBox
            // 
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelComboBox);
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelLabel);
            this.channelParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.channelParametersGroupBox.Location = new System.Drawing.Point(10, 14);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(184, 72);
            this.channelParametersGroupBox.TabIndex = 5;
            this.channelParametersGroupBox.TabStop = false;
            this.channelParametersGroupBox.Text = "Channel Parameters";
            // 
            // physicalChannelComboBox
            // 
            this.physicalChannelComboBox.Location = new System.Drawing.Point(16, 40);
            this.physicalChannelComboBox.Name = "physicalChannelComboBox";
            this.physicalChannelComboBox.Size = new System.Drawing.Size(156, 21);
            this.physicalChannelComboBox.TabIndex = 1;
            this.physicalChannelComboBox.Text = "Dev1/port0";
            // 
            // physicalChannelLabel
            // 
            this.physicalChannelLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.physicalChannelLabel.Location = new System.Drawing.Point(16, 20);
            this.physicalChannelLabel.Name = "physicalChannelLabel";
            this.physicalChannelLabel.Size = new System.Drawing.Size(96, 14);
            this.physicalChannelLabel.TabIndex = 0;
            this.physicalChannelLabel.Text = "Port";
            // 
            // writeButton
            // 
            this.writeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.writeButton.Location = new System.Drawing.Point(93, 118);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(75, 23);
            this.writeButton.TabIndex = 4;
            this.writeButton.Text = "&Write";
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // dataLabel
            // 
            this.dataLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dataLabel.Location = new System.Drawing.Point(26, 102);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(80, 16);
            this.dataLabel.TabIndex = 6;
            this.dataLabel.Text = "Data to Write:";
            // 
            // NumericUpDowndataToWrite
            // 
            this.NumericUpDowndataToWrite.Hexadecimal = true;
            this.NumericUpDowndataToWrite.Location = new System.Drawing.Point(47, 120);
            this.NumericUpDowndataToWrite.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDowndataToWrite.Name = "NumericUpDowndataToWrite";
            this.NumericUpDowndataToWrite.Size = new System.Drawing.Size(40, 20);
            this.NumericUpDowndataToWrite.TabIndex = 7;
            this.NumericUpDowndataToWrite.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // groupBoxDIOLines
            // 
            this.groupBoxDIOLines.Controls.Add(this.labelOutput);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownOutput);
            this.groupBoxDIOLines.Controls.Add(this.labelReset);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownReset);
            this.groupBoxDIOLines.Controls.Add(this.labelLoad);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownLoad);
            this.groupBoxDIOLines.Controls.Add(this.labelACPower);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownACPower);
            this.groupBoxDIOLines.Location = new System.Drawing.Point(26, 171);
            this.groupBoxDIOLines.Name = "groupBoxDIOLines";
            this.groupBoxDIOLines.Size = new System.Drawing.Size(149, 120);
            this.groupBoxDIOLines.TabIndex = 10;
            this.groupBoxDIOLines.TabStop = false;
            this.groupBoxDIOLines.Text = "DIO Line Values";
            // 
            // labelOutput
            // 
            this.labelOutput.AutoSize = true;
            this.labelOutput.Location = new System.Drawing.Point(7, 89);
            this.labelOutput.Name = "labelOutput";
            this.labelOutput.Size = new System.Drawing.Size(39, 13);
            this.labelOutput.TabIndex = 7;
            this.labelOutput.Text = "Output";
            // 
            // NumericUpDownOutput
            // 
            this.NumericUpDownOutput.Location = new System.Drawing.Point(96, 87);
            this.NumericUpDownOutput.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownOutput.Name = "NumericUpDownOutput";
            this.NumericUpDownOutput.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownOutput.TabIndex = 6;
            this.NumericUpDownOutput.ValueChanged += new System.EventHandler(this.NumericUpDownOutput_ValueChanged);
            // 
            // labelReset
            // 
            this.labelReset.AutoSize = true;
            this.labelReset.Location = new System.Drawing.Point(7, 65);
            this.labelReset.Name = "labelReset";
            this.labelReset.Size = new System.Drawing.Size(35, 13);
            this.labelReset.TabIndex = 5;
            this.labelReset.Text = "Reset";
            // 
            // NumericUpDownReset
            // 
            this.NumericUpDownReset.Location = new System.Drawing.Point(96, 63);
            this.NumericUpDownReset.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownReset.Name = "NumericUpDownReset";
            this.NumericUpDownReset.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownReset.TabIndex = 4;
            this.NumericUpDownReset.ValueChanged += new System.EventHandler(this.NumericUpDownReset_ValueChanged);
            // 
            // labelLoad
            // 
            this.labelLoad.AutoSize = true;
            this.labelLoad.Location = new System.Drawing.Point(7, 43);
            this.labelLoad.Name = "labelLoad";
            this.labelLoad.Size = new System.Drawing.Size(31, 13);
            this.labelLoad.TabIndex = 3;
            this.labelLoad.Text = "Load";
            // 
            // NumericUpDownLoad
            // 
            this.NumericUpDownLoad.Location = new System.Drawing.Point(96, 41);
            this.NumericUpDownLoad.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownLoad.Name = "NumericUpDownLoad";
            this.NumericUpDownLoad.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownLoad.TabIndex = 2;
            this.NumericUpDownLoad.ValueChanged += new System.EventHandler(this.NumericUpDownLoad_ValueChanged);
            // 
            // labelACPower
            // 
            this.labelACPower.AutoSize = true;
            this.labelACPower.Location = new System.Drawing.Point(7, 20);
            this.labelACPower.Name = "labelACPower";
            this.labelACPower.Size = new System.Drawing.Size(54, 13);
            this.labelACPower.TabIndex = 1;
            this.labelACPower.Text = "AC Power";
            // 
            // NumericUpDownACPower
            // 
            this.NumericUpDownACPower.Location = new System.Drawing.Point(96, 18);
            this.NumericUpDownACPower.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownACPower.Name = "NumericUpDownACPower";
            this.NumericUpDownACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownACPower.TabIndex = 0;
            this.NumericUpDownACPower.ValueChanged += new System.EventHandler(this.NumericUpDownACPower_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "0x";
            // 
            // FormDigitalPortTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(235, 348);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxDIOLines);
            this.Controls.Add(this.channelParametersGroupBox);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.dataLabel);
            this.Controls.Add(this.NumericUpDowndataToWrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "FormDigitalPortTest";
            this.Text = "Write to Digital Port";
            this.channelParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDowndataToWrite)).EndInit();
            this.groupBoxDIOLines.ResumeLayout(false);
            this.groupBoxDIOLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownLoad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownACPower)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox channelParametersGroupBox;
        private System.Windows.Forms.ComboBox physicalChannelComboBox;
        private System.Windows.Forms.Label physicalChannelLabel;
        private System.Windows.Forms.Button writeButton;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.NumericUpDown NumericUpDowndataToWrite;
        private System.Windows.Forms.GroupBox groupBoxDIOLines;
        private System.Windows.Forms.Label labelOutput;
        public System.Windows.Forms.NumericUpDown NumericUpDownOutput;
        private System.Windows.Forms.Label labelReset;
        public System.Windows.Forms.NumericUpDown NumericUpDownReset;
        private System.Windows.Forms.Label labelLoad;
        public System.Windows.Forms.NumericUpDown NumericUpDownLoad;
        private System.Windows.Forms.Label labelACPower;
        public System.Windows.Forms.NumericUpDown NumericUpDownACPower;
        private System.Windows.Forms.Label label1;
    }
}