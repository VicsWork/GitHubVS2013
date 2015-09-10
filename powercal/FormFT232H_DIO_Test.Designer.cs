namespace powercal
{
    partial class FormFT232H_DIO_Test
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
            this.groupBoxDIOLines = new System.Windows.Forms.GroupBox();
            this.labelOutput = new System.Windows.Forms.Label();
            this.NumericUpDownOutput = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDownReset = new System.Windows.Forms.NumericUpDown();
            this.labelLoad = new System.Windows.Forms.Label();
            this.NumericUpDownLoad = new System.Windows.Forms.NumericUpDown();
            this.labelACPower = new System.Windows.Forms.Label();
            this.NumericUpDownACPower = new System.Windows.Forms.NumericUpDown();
            this.channelParametersGroupBox.SuspendLayout();
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
            this.channelParametersGroupBox.Location = new System.Drawing.Point(12, 12);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(184, 72);
            this.channelParametersGroupBox.TabIndex = 6;
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
            // groupBoxDIOLines
            // 
            this.groupBoxDIOLines.Controls.Add(this.labelOutput);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownOutput);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownReset);
            this.groupBoxDIOLines.Controls.Add(this.labelLoad);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownLoad);
            this.groupBoxDIOLines.Controls.Add(this.labelACPower);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDownACPower);
            this.groupBoxDIOLines.Location = new System.Drawing.Point(28, 102);
            this.groupBoxDIOLines.Name = "groupBoxDIOLines";
            this.groupBoxDIOLines.Size = new System.Drawing.Size(149, 120);
            this.groupBoxDIOLines.TabIndex = 11;
            this.groupBoxDIOLines.TabStop = false;
            this.groupBoxDIOLines.Text = "DIO Line Values";
            // 
            // labelOutput
            // 
            this.labelOutput.AutoSize = true;
            this.labelOutput.Location = new System.Drawing.Point(7, 94);
            this.labelOutput.Name = "labelOutput";
            this.labelOutput.Size = new System.Drawing.Size(37, 13);
            this.labelOutput.TabIndex = 7;
            this.labelOutput.Text = "Ember";
            // 
            // NumericUpDownOutput
            // 
            this.NumericUpDownOutput.Location = new System.Drawing.Point(89, 87);
            this.NumericUpDownOutput.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownOutput.Name = "NumericUpDownOutput";
            this.NumericUpDownOutput.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownOutput.TabIndex = 6;
            // 
            // NumericUpDownReset
            // 
            this.NumericUpDownReset.Location = new System.Drawing.Point(0, 0);
            this.NumericUpDownReset.Name = "NumericUpDownReset";
            this.NumericUpDownReset.Size = new System.Drawing.Size(120, 20);
            this.NumericUpDownReset.TabIndex = 8;
            // 
            // labelLoad
            // 
            this.labelLoad.AutoSize = true;
            this.labelLoad.Location = new System.Drawing.Point(7, 68);
            this.labelLoad.Name = "labelLoad";
            this.labelLoad.Size = new System.Drawing.Size(31, 13);
            this.labelLoad.TabIndex = 3;
            this.labelLoad.Text = "Load";
            // 
            // NumericUpDownLoad
            // 
            this.NumericUpDownLoad.Location = new System.Drawing.Point(89, 61);
            this.NumericUpDownLoad.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownLoad.Name = "NumericUpDownLoad";
            this.NumericUpDownLoad.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownLoad.TabIndex = 2;
            // 
            // labelACPower
            // 
            this.labelACPower.AutoSize = true;
            this.labelACPower.Location = new System.Drawing.Point(6, 42);
            this.labelACPower.Name = "labelACPower";
            this.labelACPower.Size = new System.Drawing.Size(54, 13);
            this.labelACPower.TabIndex = 1;
            this.labelACPower.Text = "AC Power";
            // 
            // NumericUpDownACPower
            // 
            this.NumericUpDownACPower.Location = new System.Drawing.Point(89, 35);
            this.NumericUpDownACPower.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDownACPower.Name = "NumericUpDownACPower";
            this.NumericUpDownACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownACPower.TabIndex = 0;
            // 
            // FormFT232H_DIO_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 270);
            this.Controls.Add(this.groupBoxDIOLines);
            this.Controls.Add(this.channelParametersGroupBox);
            this.Name = "FormFT232H_DIO_Test";
            this.Text = "FormFT232H_DIO_Test";
            this.channelParametersGroupBox.ResumeLayout(false);
            this.groupBoxDIOLines.ResumeLayout(false);
            this.groupBoxDIOLines.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxDIOLines;
        private System.Windows.Forms.Label labelOutput;
        public System.Windows.Forms.NumericUpDown NumericUpDownOutput;
        public System.Windows.Forms.NumericUpDown NumericUpDownReset;
        private System.Windows.Forms.Label labelLoad;
        public System.Windows.Forms.NumericUpDown NumericUpDownLoad;
        private System.Windows.Forms.Label labelACPower;
        public System.Windows.Forms.NumericUpDown NumericUpDownACPower;
    }
}