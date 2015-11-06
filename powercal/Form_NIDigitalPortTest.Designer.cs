namespace PowerCalibration
{
    partial class Form_NIDigitalPortTest
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
            this.numericUpDown_Voltmeter = new System.Windows.Forms.NumericUpDown();
            this.labelVoltmeter = new System.Windows.Forms.Label();
            this.labelEmber = new System.Windows.Forms.Label();
            this.NumericUpDown_Ember = new System.Windows.Forms.NumericUpDown();
            this.labelLoad = new System.Windows.Forms.Label();
            this.NumericUpDown_Load = new System.Windows.Forms.NumericUpDown();
            this.labelACPower = new System.Windows.Forms.Label();
            this.NumericUpDown_ACPower = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.channelParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDowndataToWrite)).BeginInit();
            this.groupBoxDIOLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).BeginInit();
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
            this.groupBoxDIOLines.Controls.Add(this.numericUpDown_Voltmeter);
            this.groupBoxDIOLines.Controls.Add(this.labelVoltmeter);
            this.groupBoxDIOLines.Controls.Add(this.labelEmber);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_Ember);
            this.groupBoxDIOLines.Controls.Add(this.labelLoad);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_Load);
            this.groupBoxDIOLines.Controls.Add(this.labelACPower);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_ACPower);
            this.groupBoxDIOLines.Location = new System.Drawing.Point(26, 171);
            this.groupBoxDIOLines.Name = "groupBoxDIOLines";
            this.groupBoxDIOLines.Size = new System.Drawing.Size(149, 151);
            this.groupBoxDIOLines.TabIndex = 10;
            this.groupBoxDIOLines.TabStop = false;
            this.groupBoxDIOLines.Text = "DIO Line Values";
            // 
            // numericUpDown_Voltmeter
            // 
            this.numericUpDown_Voltmeter.Location = new System.Drawing.Point(89, 113);
            this.numericUpDown_Voltmeter.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown_Voltmeter.Name = "numericUpDown_Voltmeter";
            this.numericUpDown_Voltmeter.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_Voltmeter.TabIndex = 15;
            this.numericUpDown_Voltmeter.ValueChanged += new System.EventHandler(this.NumericUpDownr_ValueChanged);
            // 
            // labelVoltmeter
            // 
            this.labelVoltmeter.AutoSize = true;
            this.labelVoltmeter.Location = new System.Drawing.Point(7, 115);
            this.labelVoltmeter.Name = "labelVoltmeter";
            this.labelVoltmeter.Size = new System.Drawing.Size(51, 13);
            this.labelVoltmeter.TabIndex = 14;
            this.labelVoltmeter.Text = "Voltmeter";
            // 
            // labelEmber
            // 
            this.labelEmber.AutoSize = true;
            this.labelEmber.Location = new System.Drawing.Point(7, 89);
            this.labelEmber.Name = "labelEmber";
            this.labelEmber.Size = new System.Drawing.Size(37, 13);
            this.labelEmber.TabIndex = 7;
            this.labelEmber.Text = "Ember";
            // 
            // NumericUpDown_Ember
            // 
            this.NumericUpDown_Ember.Location = new System.Drawing.Point(89, 87);
            this.NumericUpDown_Ember.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_Ember.Name = "NumericUpDown_Ember";
            this.NumericUpDown_Ember.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Ember.TabIndex = 6;
            this.NumericUpDown_Ember.ValueChanged += new System.EventHandler(this.NumericUpDownr_ValueChanged);
            this.NumericUpDown_Ember.VisibleChanged += new System.EventHandler(this.NumericUpDownr_ValueChanged);
            // 
            // labelLoad
            // 
            this.labelLoad.AutoSize = true;
            this.labelLoad.Location = new System.Drawing.Point(7, 63);
            this.labelLoad.Name = "labelLoad";
            this.labelLoad.Size = new System.Drawing.Size(31, 13);
            this.labelLoad.TabIndex = 3;
            this.labelLoad.Text = "Load";
            // 
            // NumericUpDown_Load
            // 
            this.NumericUpDown_Load.Location = new System.Drawing.Point(89, 61);
            this.NumericUpDown_Load.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_Load.Name = "NumericUpDown_Load";
            this.NumericUpDown_Load.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Load.TabIndex = 2;
            this.NumericUpDown_Load.ValueChanged += new System.EventHandler(this.NumericUpDownr_ValueChanged);
            // 
            // labelACPower
            // 
            this.labelACPower.AutoSize = true;
            this.labelACPower.Location = new System.Drawing.Point(7, 37);
            this.labelACPower.Name = "labelACPower";
            this.labelACPower.Size = new System.Drawing.Size(54, 13);
            this.labelACPower.TabIndex = 1;
            this.labelACPower.Text = "AC Power";
            // 
            // NumericUpDown_ACPower
            // 
            this.NumericUpDown_ACPower.Location = new System.Drawing.Point(89, 35);
            this.NumericUpDown_ACPower.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_ACPower.Name = "NumericUpDown_ACPower";
            this.NumericUpDown_ACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_ACPower.TabIndex = 0;
            this.NumericUpDown_ACPower.ValueChanged += new System.EventHandler(this.NumericUpDownr_ValueChanged);
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
            // Form_NIDigitalPortTest
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
            this.Name = "Form_NIDigitalPortTest";
            this.Text = "NI Digital Port";
            this.channelParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDowndataToWrite)).EndInit();
            this.groupBoxDIOLines.ResumeLayout(false);
            this.groupBoxDIOLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).EndInit();
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
        private System.Windows.Forms.Label labelEmber;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Ember;
        private System.Windows.Forms.Label labelLoad;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Load;
        private System.Windows.Forms.Label labelACPower;
        public System.Windows.Forms.NumericUpDown NumericUpDown_ACPower;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown numericUpDown_Voltmeter;
        private System.Windows.Forms.Label labelVoltmeter;
    }
}