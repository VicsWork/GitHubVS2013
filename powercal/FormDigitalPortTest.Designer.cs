namespace powercal
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
            this.dataToWriteNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NumericUpDownOutput = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NumericUpDownReset = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericUpDownLoad = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDownACPower = new System.Windows.Forms.NumericUpDown();
            this.channelParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataToWriteNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.writeButton.Location = new System.Drawing.Point(107, 118);
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
            // dataToWriteNumericUpDown
            // 
            this.dataToWriteNumericUpDown.Hexadecimal = true;
            this.dataToWriteNumericUpDown.Location = new System.Drawing.Point(26, 118);
            this.dataToWriteNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.dataToWriteNumericUpDown.Name = "dataToWriteNumericUpDown";
            this.dataToWriteNumericUpDown.Size = new System.Drawing.Size(64, 20);
            this.dataToWriteNumericUpDown.TabIndex = 7;
            this.dataToWriteNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.NumericUpDownOutput);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.NumericUpDownReset);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.NumericUpDownLoad);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.NumericUpDownACPower);
            this.groupBox2.Location = new System.Drawing.Point(26, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(149, 120);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DIO Line Values";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Output";
            // 
            // NumericUpDownOutput
            // 
            this.NumericUpDownOutput.Location = new System.Drawing.Point(67, 88);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Reset";
            // 
            // NumericUpDownReset
            // 
            this.NumericUpDownReset.Location = new System.Drawing.Point(67, 64);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Load";
            // 
            // NumericUpDownLoad
            // 
            this.NumericUpDownLoad.Location = new System.Drawing.Point(67, 42);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "AC Power";
            // 
            // NumericUpDownACPower
            // 
            this.NumericUpDownACPower.Location = new System.Drawing.Point(67, 19);
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
            // FormDigitalPortTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(334, 405);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.channelParametersGroupBox);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.dataLabel);
            this.Controls.Add(this.dataToWriteNumericUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "FormDigitalPortTest";
            this.Text = "Write to Digital Port";
            this.channelParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataToWriteNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownLoad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownACPower)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox channelParametersGroupBox;
        private System.Windows.Forms.ComboBox physicalChannelComboBox;
        private System.Windows.Forms.Label physicalChannelLabel;
        private System.Windows.Forms.Button writeButton;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.NumericUpDown dataToWriteNumericUpDown;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NumericUpDownOutput;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown NumericUpDownReset;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NumericUpDownLoad;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NumericUpDownACPower;
    }
}