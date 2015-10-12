namespace powercal
{
    partial class Form_Settings
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
            this.TextBoxMeterCOM = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckBoxManualMultiMeter = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxDIO = new System.Windows.Forms.GroupBox();
            this.numericUpDown_Voltmeter = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDIOCtrollerTypes = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NumericUpDown_Ember = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericUpDown_Load = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDown_ACPower = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonEmberBinPathBrowse = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.TextBoxEmberBinPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxEmberInterface = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxEmberInterfaceAddress = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxDIO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBoxMeterCOM
            // 
            this.TextBoxMeterCOM.Location = new System.Drawing.Point(91, 29);
            this.TextBoxMeterCOM.Name = "TextBoxMeterCOM";
            this.TextBoxMeterCOM.Size = new System.Drawing.Size(58, 20);
            this.TextBoxMeterCOM.TabIndex = 5;
            this.TextBoxMeterCOM.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Multi-meter port";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CheckBoxManualMultiMeter);
            this.groupBox1.Controls.Add(this.TextBoxMeterCOM);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(250, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 140);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Measurement";
            // 
            // CheckBoxManualMultiMeter
            // 
            this.CheckBoxManualMultiMeter.AutoSize = true;
            this.CheckBoxManualMultiMeter.Location = new System.Drawing.Point(156, 31);
            this.CheckBoxManualMultiMeter.Name = "CheckBoxManualMultiMeter";
            this.CheckBoxManualMultiMeter.Size = new System.Drawing.Size(61, 17);
            this.CheckBoxManualMultiMeter.TabIndex = 6;
            this.CheckBoxManualMultiMeter.Text = "Manual";
            this.CheckBoxManualMultiMeter.UseVisualStyleBackColor = true;
            this.CheckBoxManualMultiMeter.CheckedChanged += new System.EventHandler(this.CheckBoxManualMultiMeter_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(170, 318);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(251, 318);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "&Cancel";
            // 
            // groupBoxDIO
            // 
            this.groupBoxDIO.Controls.Add(this.numericUpDown_Voltmeter);
            this.groupBoxDIO.Controls.Add(this.label5);
            this.groupBoxDIO.Controls.Add(this.label1);
            this.groupBoxDIO.Controls.Add(this.comboBoxDIOCtrollerTypes);
            this.groupBoxDIO.Controls.Add(this.label6);
            this.groupBoxDIO.Controls.Add(this.NumericUpDown_Ember);
            this.groupBoxDIO.Controls.Add(this.label4);
            this.groupBoxDIO.Controls.Add(this.NumericUpDown_Load);
            this.groupBoxDIO.Controls.Add(this.label3);
            this.groupBoxDIO.Controls.Add(this.NumericUpDown_ACPower);
            this.groupBoxDIO.Location = new System.Drawing.Point(12, 143);
            this.groupBoxDIO.Name = "groupBoxDIO";
            this.groupBoxDIO.Size = new System.Drawing.Size(217, 147);
            this.groupBoxDIO.TabIndex = 9;
            this.groupBoxDIO.TabStop = false;
            this.groupBoxDIO.Text = "DIO Line";
            // 
            // numericUpDown_Voltmeter
            // 
            this.numericUpDown_Voltmeter.Location = new System.Drawing.Point(168, 89);
            this.numericUpDown_Voltmeter.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown_Voltmeter.Name = "numericUpDown_Voltmeter";
            this.numericUpDown_Voltmeter.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_Voltmeter.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Voltmeter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Type";
            // 
            // comboBoxDIOCtrollerTypes
            // 
            this.comboBoxDIOCtrollerTypes.FormattingEnabled = true;
            this.comboBoxDIOCtrollerTypes.Location = new System.Drawing.Point(44, 21);
            this.comboBoxDIOCtrollerTypes.Name = "comboBoxDIOCtrollerTypes";
            this.comboBoxDIOCtrollerTypes.Size = new System.Drawing.Size(111, 21);
            this.comboBoxDIOCtrollerTypes.TabIndex = 8;
            this.comboBoxDIOCtrollerTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxDIOCtrollerTypes_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(111, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Ember";
            // 
            // NumericUpDown_Ember
            // 
            this.NumericUpDown_Ember.Location = new System.Drawing.Point(167, 61);
            this.NumericUpDown_Ember.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_Ember.Name = "NumericUpDown_Ember";
            this.NumericUpDown_Ember.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Ember.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Load";
            // 
            // NumericUpDown_Load
            // 
            this.NumericUpDown_Load.Location = new System.Drawing.Point(67, 84);
            this.NumericUpDown_Load.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_Load.Name = "NumericUpDown_Load";
            this.NumericUpDown_Load.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Load.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "AC Power";
            // 
            // NumericUpDown_ACPower
            // 
            this.NumericUpDown_ACPower.Location = new System.Drawing.Point(67, 61);
            this.NumericUpDown_ACPower.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_ACPower.Name = "NumericUpDown_ACPower";
            this.NumericUpDown_ACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_ACPower.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxEmberInterfaceAddress);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.comboBoxEmberInterface);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.buttonEmberBinPathBrowse);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.TextBoxEmberBinPath);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(468, 125);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ember";
            // 
            // buttonEmberBinPathBrowse
            // 
            this.buttonEmberBinPathBrowse.Location = new System.Drawing.Point(387, 75);
            this.buttonEmberBinPathBrowse.Name = "buttonEmberBinPathBrowse";
            this.buttonEmberBinPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonEmberBinPathBrowse.TabIndex = 15;
            this.buttonEmberBinPathBrowse.Text = "&Browse";
            this.buttonEmberBinPathBrowse.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Path:";
            // 
            // TextBoxEmberBinPath
            // 
            this.TextBoxEmberBinPath.Location = new System.Drawing.Point(37, 78);
            this.TextBoxEmberBinPath.Name = "TextBoxEmberBinPath";
            this.TextBoxEmberBinPath.Size = new System.Drawing.Size(344, 20);
            this.TextBoxEmberBinPath.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Interface:";
            // 
            // comboBoxEmberInterface
            // 
            this.comboBoxEmberInterface.FormattingEnabled = true;
            this.comboBoxEmberInterface.Items.AddRange(new object[] {
            "USB",
            "IP"});
            this.comboBoxEmberInterface.Location = new System.Drawing.Point(64, 35);
            this.comboBoxEmberInterface.Name = "comboBoxEmberInterface";
            this.comboBoxEmberInterface.Size = new System.Drawing.Size(64, 21);
            this.comboBoxEmberInterface.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(155, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Address:";
            // 
            // textBoxEmberInterfaceAddress
            // 
            this.textBoxEmberInterfaceAddress.Location = new System.Drawing.Point(209, 35);
            this.textBoxEmberInterfaceAddress.Name = "textBoxEmberInterfaceAddress";
            this.textBoxEmberInterfaceAddress.Size = new System.Drawing.Size(125, 20);
            this.textBoxEmberInterfaceAddress.TabIndex = 20;
            // 
            // Form_Settings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(492, 350);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBoxDIO);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form_Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxDIO.ResumeLayout(false);
            this.groupBoxDIO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.TextBox TextBoxMeterCOM;
        private System.Windows.Forms.GroupBox groupBoxDIO;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.CheckBox CheckBoxManualMultiMeter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Ember;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Load;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NumericUpDown_ACPower;
        public System.Windows.Forms.ComboBox comboBoxDIOCtrollerTypes;
        public System.Windows.Forms.NumericUpDown numericUpDown_Voltmeter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonEmberBinPathBrowse;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox TextBoxEmberBinPath;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox comboBoxEmberInterface;
        public System.Windows.Forms.TextBox textBoxEmberInterfaceAddress;
        private System.Windows.Forms.Label label9;
    }
}