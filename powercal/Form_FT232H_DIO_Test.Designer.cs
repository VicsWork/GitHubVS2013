namespace PowerCalibration
{
    partial class Form_FT232H_DIO_Test
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
            this.Label_physicalChannel = new System.Windows.Forms.Label();
            this.groupBoxDIOLines = new System.Windows.Forms.GroupBox();
            this.NumericUpDown_ACPower = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_VacVdc = new System.Windows.Forms.NumericUpDown();
            this.labelVacVdc = new System.Windows.Forms.Label();
            this.labelEmber = new System.Windows.Forms.Label();
            this.NumericUpDown_Ember = new System.Windows.Forms.NumericUpDown();
            this.labelLoad = new System.Windows.Forms.Label();
            this.NumericUpDown_Load = new System.Windows.Forms.NumericUpDown();
            this.labelACPower = new System.Windows.Forms.Label();
            this.buttonAllOff = new System.Windows.Forms.Button();
            this.numericUpDown_Test_VacVdc = new System.Windows.Forms.NumericUpDown();
            this.labelTest_VacVdc = new System.Windows.Forms.Label();
            this.channelParametersGroupBox.SuspendLayout();
            this.groupBoxDIOLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_VacVdc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Test_VacVdc)).BeginInit();
            this.SuspendLayout();
            // 
            // channelParametersGroupBox
            // 
            this.channelParametersGroupBox.Controls.Add(this.Label_physicalChannel);
            this.channelParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.channelParametersGroupBox.Location = new System.Drawing.Point(12, 12);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(184, 72);
            this.channelParametersGroupBox.TabIndex = 6;
            this.channelParametersGroupBox.TabStop = false;
            this.channelParametersGroupBox.Text = "Channel Parameters";
            // 
            // Label_physicalChannel
            // 
            this.Label_physicalChannel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Label_physicalChannel.Location = new System.Drawing.Point(16, 20);
            this.Label_physicalChannel.Name = "Label_physicalChannel";
            this.Label_physicalChannel.Size = new System.Drawing.Size(246, 49);
            this.Label_physicalChannel.TabIndex = 0;
            this.Label_physicalChannel.Text = "Port";
            // 
            // groupBoxDIOLines
            // 
            this.groupBoxDIOLines.Controls.Add(this.numericUpDown_Test_VacVdc);
            this.groupBoxDIOLines.Controls.Add(this.labelTest_VacVdc);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_ACPower);
            this.groupBoxDIOLines.Controls.Add(this.numericUpDown_VacVdc);
            this.groupBoxDIOLines.Controls.Add(this.labelVacVdc);
            this.groupBoxDIOLines.Controls.Add(this.labelEmber);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_Ember);
            this.groupBoxDIOLines.Controls.Add(this.labelLoad);
            this.groupBoxDIOLines.Controls.Add(this.NumericUpDown_Load);
            this.groupBoxDIOLines.Controls.Add(this.labelACPower);
            this.groupBoxDIOLines.Location = new System.Drawing.Point(28, 102);
            this.groupBoxDIOLines.Name = "groupBoxDIOLines";
            this.groupBoxDIOLines.Size = new System.Drawing.Size(258, 130);
            this.groupBoxDIOLines.TabIndex = 0;
            this.groupBoxDIOLines.TabStop = false;
            this.groupBoxDIOLines.Text = "DIO Line Values";
            // 
            // NumericUpDown_ACPower
            // 
            this.NumericUpDown_ACPower.Location = new System.Drawing.Point(80, 35);
            this.NumericUpDown_ACPower.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_ACPower.Name = "NumericUpDown_ACPower";
            this.NumericUpDown_ACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_ACPower.TabIndex = 0;
            this.NumericUpDown_ACPower.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDown_VacVdc
            // 
            this.numericUpDown_VacVdc.Location = new System.Drawing.Point(213, 66);
            this.numericUpDown_VacVdc.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_VacVdc.Name = "numericUpDown_VacVdc";
            this.numericUpDown_VacVdc.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_VacVdc.TabIndex = 3;
            this.numericUpDown_VacVdc.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // labelVacVdc
            // 
            this.labelVacVdc.AutoSize = true;
            this.labelVacVdc.Location = new System.Drawing.Point(131, 68);
            this.labelVacVdc.Name = "labelVacVdc";
            this.labelVacVdc.Size = new System.Drawing.Size(50, 13);
            this.labelVacVdc.TabIndex = 12;
            this.labelVacVdc.Text = "Vac/Vdc";
            // 
            // labelEmber
            // 
            this.labelEmber.AutoSize = true;
            this.labelEmber.Location = new System.Drawing.Point(131, 42);
            this.labelEmber.Name = "labelEmber";
            this.labelEmber.Size = new System.Drawing.Size(37, 13);
            this.labelEmber.TabIndex = 7;
            this.labelEmber.Text = "Ember";
            // 
            // NumericUpDown_Ember
            // 
            this.NumericUpDown_Ember.Location = new System.Drawing.Point(213, 30);
            this.NumericUpDown_Ember.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_Ember.Name = "NumericUpDown_Ember";
            this.NumericUpDown_Ember.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Ember.TabIndex = 1;
            this.NumericUpDown_Ember.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
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
            // NumericUpDown_Load
            // 
            this.NumericUpDown_Load.Location = new System.Drawing.Point(80, 66);
            this.NumericUpDown_Load.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_Load.Name = "NumericUpDown_Load";
            this.NumericUpDown_Load.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Load.TabIndex = 2;
            this.NumericUpDown_Load.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
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
            // buttonAllOff
            // 
            this.buttonAllOff.Location = new System.Drawing.Point(211, 32);
            this.buttonAllOff.Name = "buttonAllOff";
            this.buttonAllOff.Size = new System.Drawing.Size(75, 23);
            this.buttonAllOff.TabIndex = 7;
            this.buttonAllOff.Text = "All O&ff";
            this.buttonAllOff.UseVisualStyleBackColor = true;
            this.buttonAllOff.Click += new System.EventHandler(this.buttonAllOff_Click);
            // 
            // numericUpDown_Test_VacVdc
            // 
            this.numericUpDown_Test_VacVdc.Location = new System.Drawing.Point(143, 102);
            this.numericUpDown_Test_VacVdc.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Test_VacVdc.Name = "numericUpDown_Test_VacVdc";
            this.numericUpDown_Test_VacVdc.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_Test_VacVdc.TabIndex = 13;
            this.numericUpDown_Test_VacVdc.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // labelTest_VacVdc
            // 
            this.labelTest_VacVdc.AutoSize = true;
            this.labelTest_VacVdc.Location = new System.Drawing.Point(49, 104);
            this.labelTest_VacVdc.Name = "labelTest_VacVdc";
            this.labelTest_VacVdc.Size = new System.Drawing.Size(77, 13);
            this.labelTest_VacVdc.TabIndex = 14;
            this.labelTest_VacVdc.Text = "Test/Vac_Vdc";
            // 
            // Form_FT232H_DIO_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 244);
            this.Controls.Add(this.buttonAllOff);
            this.Controls.Add(this.groupBoxDIOLines);
            this.Controls.Add(this.channelParametersGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form_FT232H_DIO_Test";
            this.Text = "FormFT232H_DIO_Test";
            this.channelParametersGroupBox.ResumeLayout(false);
            this.groupBoxDIOLines.ResumeLayout(false);
            this.groupBoxDIOLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_VacVdc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Test_VacVdc)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox channelParametersGroupBox;
        private System.Windows.Forms.GroupBox groupBoxDIOLines;
        private System.Windows.Forms.Label labelEmber;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Ember;
        private System.Windows.Forms.Label labelLoad;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Load;
        private System.Windows.Forms.Label labelACPower;
        public System.Windows.Forms.NumericUpDown NumericUpDown_ACPower;
        public System.Windows.Forms.Label Label_physicalChannel;
        public System.Windows.Forms.NumericUpDown numericUpDown_VacVdc;
        private System.Windows.Forms.Label labelVacVdc;
        private System.Windows.Forms.Button buttonAllOff;
        public System.Windows.Forms.NumericUpDown numericUpDown_Test_VacVdc;
        private System.Windows.Forms.Label labelTest_VacVdc;
    }
}