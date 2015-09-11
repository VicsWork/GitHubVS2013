namespace powercal
{
    partial class FormSettings
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TextBoxEmberBinPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonEmberBinPathBrowse = new System.Windows.Forms.Button();
            this.NumericUpDownACPower = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDownLoad = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericUpDownEmber = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxDIOCtrollerTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBoxDIO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownACPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownLoad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEmber)).BeginInit();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 140);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comunications";
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
            this.buttonOK.Location = new System.Drawing.Point(154, 211);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(235, 211);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "&Cancel";
            // 
            // groupBoxDIO
            // 
            this.groupBoxDIO.Controls.Add(this.label1);
            this.groupBoxDIO.Controls.Add(this.comboBoxDIOCtrollerTypes);
            this.groupBoxDIO.Controls.Add(this.label6);
            this.groupBoxDIO.Controls.Add(this.NumericUpDownEmber);
            this.groupBoxDIO.Controls.Add(this.label4);
            this.groupBoxDIO.Controls.Add(this.NumericUpDownLoad);
            this.groupBoxDIO.Controls.Add(this.label3);
            this.groupBoxDIO.Controls.Add(this.NumericUpDownACPower);
            this.groupBoxDIO.Location = new System.Drawing.Point(277, 12);
            this.groupBoxDIO.Name = "groupBoxDIO";
            this.groupBoxDIO.Size = new System.Drawing.Size(161, 140);
            this.groupBoxDIO.TabIndex = 9;
            this.groupBoxDIO.TabStop = false;
            this.groupBoxDIO.Text = "DIO Line";
            // 
            // TextBoxEmberBinPath
            // 
            this.TextBoxEmberBinPath.Location = new System.Drawing.Point(99, 173);
            this.TextBoxEmberBinPath.Name = "TextBoxEmberBinPath";
            this.TextBoxEmberBinPath.Size = new System.Drawing.Size(258, 20);
            this.TextBoxEmberBinPath.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Ember bin path:";
            // 
            // buttonEmberBinPathBrowse
            // 
            this.buttonEmberBinPathBrowse.Location = new System.Drawing.Point(363, 171);
            this.buttonEmberBinPathBrowse.Name = "buttonEmberBinPathBrowse";
            this.buttonEmberBinPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonEmberBinPathBrowse.TabIndex = 12;
            this.buttonEmberBinPathBrowse.Text = "&Browse";
            this.buttonEmberBinPathBrowse.UseVisualStyleBackColor = true;
            this.buttonEmberBinPathBrowse.Click += new System.EventHandler(this.buttonEmberBinPathBrowse_Click);
            // 
            // NumericUpDownACPower
            // 
            this.NumericUpDownACPower.Location = new System.Drawing.Point(86, 60);
            this.NumericUpDownACPower.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDownACPower.Name = "NumericUpDownACPower";
            this.NumericUpDownACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownACPower.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "AC Power";
            // 
            // NumericUpDownLoad
            // 
            this.NumericUpDownLoad.Location = new System.Drawing.Point(86, 83);
            this.NumericUpDownLoad.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDownLoad.Name = "NumericUpDownLoad";
            this.NumericUpDownLoad.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownLoad.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Load";
            // 
            // NumericUpDownEmber
            // 
            this.NumericUpDownEmber.Location = new System.Drawing.Point(86, 105);
            this.NumericUpDownEmber.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDownEmber.Name = "NumericUpDownEmber";
            this.NumericUpDownEmber.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDownEmber.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Ember";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Type";
            // 
            // FormSettings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(560, 311);
            this.Controls.Add(this.buttonEmberBinPathBrowse);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TextBoxEmberBinPath);
            this.Controls.Add(this.groupBoxDIO);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxDIO.ResumeLayout(false);
            this.groupBoxDIO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownACPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownLoad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEmber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox TextBoxEmberBinPath;
        private System.Windows.Forms.Button buttonEmberBinPathBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NumericUpDownEmber;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NumericUpDownLoad;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NumericUpDownACPower;
        public System.Windows.Forms.ComboBox comboBoxDIOCtrollerTypes;
    }
}