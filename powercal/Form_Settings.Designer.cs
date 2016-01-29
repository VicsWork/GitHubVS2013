namespace PowerCalibration
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageEmber = new System.Windows.Forms.TabPage();
            this.textBoxEmberInterfaceAddress = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxEmberInterface = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonEmberBinPathBrowse = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.TextBoxEmberBinPath = new System.Windows.Forms.TextBox();
            this.tabPageDIO = new System.Windows.Forms.TabPage();
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
            this.tagPageMeasurement = new System.Windows.Forms.TabPage();
            this.checkBoxPreProTest = new System.Windows.Forms.CheckBox();
            this.CheckBoxManualMultiMeter = new System.Windows.Forms.CheckBox();
            this.TextBoxMeterCOM = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageShortcuts = new System.Windows.Forms.TabPage();
            this.comboBoxShortcutActions = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPageReporting = new System.Windows.Forms.TabPage();
            this.checkBox_EnableDBReporting = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageEmber.SuspendLayout();
            this.tabPageDIO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).BeginInit();
            this.tagPageMeasurement.SuspendLayout();
            this.tabPageShortcuts.SuspendLayout();
            this.tabPageReporting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageEmber);
            this.tabControl1.Controls.Add(this.tabPageDIO);
            this.tabControl1.Controls.Add(this.tagPageMeasurement);
            this.tabControl1.Controls.Add(this.tabPageShortcuts);
            this.tabControl1.Controls.Add(this.tabPageReporting);
            this.tabControl1.Location = new System.Drawing.Point(9, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(372, 217);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageEmber
            // 
            this.tabPageEmber.Controls.Add(this.textBoxEmberInterfaceAddress);
            this.tabPageEmber.Controls.Add(this.label9);
            this.tabPageEmber.Controls.Add(this.comboBoxEmberInterface);
            this.tabPageEmber.Controls.Add(this.label8);
            this.tabPageEmber.Controls.Add(this.buttonEmberBinPathBrowse);
            this.tabPageEmber.Controls.Add(this.label7);
            this.tabPageEmber.Controls.Add(this.TextBoxEmberBinPath);
            this.tabPageEmber.Location = new System.Drawing.Point(4, 22);
            this.tabPageEmber.Name = "tabPageEmber";
            this.tabPageEmber.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEmber.Size = new System.Drawing.Size(364, 191);
            this.tabPageEmber.TabIndex = 0;
            this.tabPageEmber.Text = "Ember";
            this.tabPageEmber.UseVisualStyleBackColor = true;
            // 
            // textBoxEmberInterfaceAddress
            // 
            this.textBoxEmberInterfaceAddress.Location = new System.Drawing.Point(206, 10);
            this.textBoxEmberInterfaceAddress.Name = "textBoxEmberInterfaceAddress";
            this.textBoxEmberInterfaceAddress.Size = new System.Drawing.Size(125, 20);
            this.textBoxEmberInterfaceAddress.TabIndex = 1;
            this.textBoxEmberInterfaceAddress.TextChanged += new System.EventHandler(this.textBoxEmberInterfaceAddress_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(152, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Address:";
            // 
            // comboBoxEmberInterface
            // 
            this.comboBoxEmberInterface.FormattingEnabled = true;
            this.comboBoxEmberInterface.Items.AddRange(new object[] {
            "USB",
            "IP"});
            this.comboBoxEmberInterface.Location = new System.Drawing.Point(61, 10);
            this.comboBoxEmberInterface.Name = "comboBoxEmberInterface";
            this.comboBoxEmberInterface.Size = new System.Drawing.Size(64, 21);
            this.comboBoxEmberInterface.TabIndex = 0;
            this.comboBoxEmberInterface.SelectedIndexChanged += new System.EventHandler(this.comboBoxEmberInterface_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Interface:";
            // 
            // buttonEmberBinPathBrowse
            // 
            this.buttonEmberBinPathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEmberBinPathBrowse.Location = new System.Drawing.Point(283, 51);
            this.buttonEmberBinPathBrowse.Name = "buttonEmberBinPathBrowse";
            this.buttonEmberBinPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonEmberBinPathBrowse.TabIndex = 3;
            this.buttonEmberBinPathBrowse.Text = "&Browse";
            this.buttonEmberBinPathBrowse.UseVisualStyleBackColor = true;
            this.buttonEmberBinPathBrowse.Click += new System.EventHandler(this.buttonEmberBinPathBrowse_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Path:";
            // 
            // TextBoxEmberBinPath
            // 
            this.TextBoxEmberBinPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxEmberBinPath.Location = new System.Drawing.Point(41, 53);
            this.TextBoxEmberBinPath.Name = "TextBoxEmberBinPath";
            this.TextBoxEmberBinPath.Size = new System.Drawing.Size(236, 20);
            this.TextBoxEmberBinPath.TabIndex = 2;
            this.TextBoxEmberBinPath.TextChanged += new System.EventHandler(this.TextBoxEmberBinPath_TextChanged);
            // 
            // tabPageDIO
            // 
            this.tabPageDIO.Controls.Add(this.numericUpDown_Voltmeter);
            this.tabPageDIO.Controls.Add(this.label5);
            this.tabPageDIO.Controls.Add(this.label1);
            this.tabPageDIO.Controls.Add(this.comboBoxDIOCtrollerTypes);
            this.tabPageDIO.Controls.Add(this.label6);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_Ember);
            this.tabPageDIO.Controls.Add(this.label4);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_Load);
            this.tabPageDIO.Controls.Add(this.label3);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_ACPower);
            this.tabPageDIO.Location = new System.Drawing.Point(4, 22);
            this.tabPageDIO.Name = "tabPageDIO";
            this.tabPageDIO.Size = new System.Drawing.Size(364, 191);
            this.tabPageDIO.TabIndex = 2;
            this.tabPageDIO.Text = "DIO";
            this.tabPageDIO.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_Voltmeter
            // 
            this.numericUpDown_Voltmeter.Location = new System.Drawing.Point(188, 126);
            this.numericUpDown_Voltmeter.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown_Voltmeter.Name = "numericUpDown_Voltmeter";
            this.numericUpDown_Voltmeter.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_Voltmeter.TabIndex = 4;
            this.numericUpDown_Voltmeter.ValueChanged += new System.EventHandler(this.NumericUpDown_DIOLine_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(101, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Voltmeter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Type";
            // 
            // comboBoxDIOCtrollerTypes
            // 
            this.comboBoxDIOCtrollerTypes.FormattingEnabled = true;
            this.comboBoxDIOCtrollerTypes.Location = new System.Drawing.Point(150, 14);
            this.comboBoxDIOCtrollerTypes.Name = "comboBoxDIOCtrollerTypes";
            this.comboBoxDIOCtrollerTypes.Size = new System.Drawing.Size(111, 21);
            this.comboBoxDIOCtrollerTypes.TabIndex = 0;
            this.comboBoxDIOCtrollerTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxDIOCtrollerTypes_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(101, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Ember";
            // 
            // NumericUpDown_Ember
            // 
            this.NumericUpDown_Ember.Location = new System.Drawing.Point(188, 104);
            this.NumericUpDown_Ember.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_Ember.Name = "NumericUpDown_Ember";
            this.NumericUpDown_Ember.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Ember.TabIndex = 3;
            this.NumericUpDown_Ember.ValueChanged += new System.EventHandler(this.NumericUpDown_DIOLine_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(103, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Load";
            // 
            // NumericUpDown_Load
            // 
            this.NumericUpDown_Load.Location = new System.Drawing.Point(188, 80);
            this.NumericUpDown_Load.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_Load.Name = "NumericUpDown_Load";
            this.NumericUpDown_Load.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_Load.TabIndex = 2;
            this.NumericUpDown_Load.ValueChanged += new System.EventHandler(this.NumericUpDown_DIOLine_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(101, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "AC Power";
            // 
            // NumericUpDown_ACPower
            // 
            this.NumericUpDown_ACPower.Location = new System.Drawing.Point(188, 57);
            this.NumericUpDown_ACPower.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NumericUpDown_ACPower.Name = "NumericUpDown_ACPower";
            this.NumericUpDown_ACPower.Size = new System.Drawing.Size(31, 20);
            this.NumericUpDown_ACPower.TabIndex = 1;
            this.NumericUpDown_ACPower.ValueChanged += new System.EventHandler(this.NumericUpDown_DIOLine_ValueChanged);
            // 
            // tagPageMeasurement
            // 
            this.tagPageMeasurement.Controls.Add(this.checkBoxPreProTest);
            this.tagPageMeasurement.Controls.Add(this.CheckBoxManualMultiMeter);
            this.tagPageMeasurement.Controls.Add(this.TextBoxMeterCOM);
            this.tagPageMeasurement.Controls.Add(this.label2);
            this.tagPageMeasurement.Location = new System.Drawing.Point(4, 22);
            this.tagPageMeasurement.Name = "tagPageMeasurement";
            this.tagPageMeasurement.Size = new System.Drawing.Size(364, 191);
            this.tagPageMeasurement.TabIndex = 3;
            this.tagPageMeasurement.Text = "Measurement";
            this.tagPageMeasurement.UseVisualStyleBackColor = true;
            // 
            // checkBoxPreProTest
            // 
            this.checkBoxPreProTest.AutoSize = true;
            this.checkBoxPreProTest.Checked = true;
            this.checkBoxPreProTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPreProTest.Location = new System.Drawing.Point(4, 40);
            this.checkBoxPreProTest.Name = "checkBoxPreProTest";
            this.checkBoxPreProTest.Size = new System.Drawing.Size(92, 17);
            this.checkBoxPreProTest.TabIndex = 11;
            this.checkBoxPreProTest.Text = "Pre/Post Test";
            this.checkBoxPreProTest.UseVisualStyleBackColor = true;
            this.checkBoxPreProTest.CheckedChanged += new System.EventHandler(this.checkBoxPreProTest_CheckedChanged);
            // 
            // CheckBoxManualMultiMeter
            // 
            this.CheckBoxManualMultiMeter.AutoSize = true;
            this.CheckBoxManualMultiMeter.Location = new System.Drawing.Point(153, 14);
            this.CheckBoxManualMultiMeter.Name = "CheckBoxManualMultiMeter";
            this.CheckBoxManualMultiMeter.Size = new System.Drawing.Size(61, 17);
            this.CheckBoxManualMultiMeter.TabIndex = 10;
            this.CheckBoxManualMultiMeter.Text = "Manual";
            this.CheckBoxManualMultiMeter.UseVisualStyleBackColor = true;
            this.CheckBoxManualMultiMeter.CheckedChanged += new System.EventHandler(this.CheckBoxManualMultiMeter_CheckedChanged);
            // 
            // TextBoxMeterCOM
            // 
            this.TextBoxMeterCOM.Location = new System.Drawing.Point(88, 12);
            this.TextBoxMeterCOM.Name = "TextBoxMeterCOM";
            this.TextBoxMeterCOM.Size = new System.Drawing.Size(58, 20);
            this.TextBoxMeterCOM.TabIndex = 9;
            this.TextBoxMeterCOM.WordWrap = false;
            this.TextBoxMeterCOM.TextChanged += new System.EventHandler(this.TextBoxMeterCOM_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Multi-meter port";
            // 
            // tabPageShortcuts
            // 
            this.tabPageShortcuts.Controls.Add(this.comboBoxShortcutActions);
            this.tabPageShortcuts.Controls.Add(this.label12);
            this.tabPageShortcuts.Location = new System.Drawing.Point(4, 22);
            this.tabPageShortcuts.Name = "tabPageShortcuts";
            this.tabPageShortcuts.Size = new System.Drawing.Size(364, 191);
            this.tabPageShortcuts.TabIndex = 4;
            this.tabPageShortcuts.Text = "Shortcuts";
            this.tabPageShortcuts.UseVisualStyleBackColor = true;
            // 
            // comboBoxShortcutActions
            // 
            this.comboBoxShortcutActions.FormattingEnabled = true;
            this.comboBoxShortcutActions.Items.AddRange(new object[] {
            "All",
            "Calibrate",
            "Code",
            "ReCode"});
            this.comboBoxShortcutActions.Location = new System.Drawing.Point(50, 7);
            this.comboBoxShortcutActions.Name = "comboBoxShortcutActions";
            this.comboBoxShortcutActions.Size = new System.Drawing.Size(121, 21);
            this.comboBoxShortcutActions.TabIndex = 3;
            this.comboBoxShortcutActions.SelectedIndexChanged += new System.EventHandler(this.comboBoxShortcutActions_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "SPACE";
            // 
            // tabPageReporting
            // 
            this.tabPageReporting.Controls.Add(this.checkBox_EnableDBReporting);
            this.tabPageReporting.Location = new System.Drawing.Point(4, 22);
            this.tabPageReporting.Name = "tabPageReporting";
            this.tabPageReporting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReporting.Size = new System.Drawing.Size(364, 191);
            this.tabPageReporting.TabIndex = 1;
            this.tabPageReporting.Text = "Reporting";
            this.tabPageReporting.UseVisualStyleBackColor = true;
            // 
            // checkBox_EnableDBReporting
            // 
            this.checkBox_EnableDBReporting.AutoSize = true;
            this.checkBox_EnableDBReporting.Location = new System.Drawing.Point(3, 6);
            this.checkBox_EnableDBReporting.Name = "checkBox_EnableDBReporting";
            this.checkBox_EnableDBReporting.Size = new System.Drawing.Size(157, 17);
            this.checkBox_EnableDBReporting.TabIndex = 0;
            this.checkBox_EnableDBReporting.Text = "Enable Database Reporting";
            this.checkBox_EnableDBReporting.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(198, 249);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "&Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(117, 249);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // Form_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 284);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(406, 322);
            this.Name = "Form_Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageEmber.ResumeLayout(false);
            this.tabPageEmber.PerformLayout();
            this.tabPageDIO.ResumeLayout(false);
            this.tabPageDIO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Voltmeter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).EndInit();
            this.tagPageMeasurement.ResumeLayout(false);
            this.tagPageMeasurement.PerformLayout();
            this.tabPageShortcuts.ResumeLayout(false);
            this.tabPageShortcuts.PerformLayout();
            this.tabPageReporting.ResumeLayout(false);
            this.tabPageReporting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageEmber;
        private System.Windows.Forms.TabPage tabPageReporting;
        private System.Windows.Forms.CheckBox checkBox_EnableDBReporting;
        public System.Windows.Forms.TextBox textBoxEmberInterfaceAddress;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.ComboBox comboBoxEmberInterface;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonEmberBinPathBrowse;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox TextBoxEmberBinPath;
        private System.Windows.Forms.TabPage tabPageDIO;
        public System.Windows.Forms.NumericUpDown numericUpDown_Voltmeter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBoxDIOCtrollerTypes;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Ember;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Load;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NumericUpDown_ACPower;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabPage tagPageMeasurement;
        private System.Windows.Forms.CheckBox checkBoxPreProTest;
        public System.Windows.Forms.CheckBox CheckBoxManualMultiMeter;
        public System.Windows.Forms.TextBox TextBoxMeterCOM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPageShortcuts;
        private System.Windows.Forms.ComboBox comboBoxShortcutActions;
        private System.Windows.Forms.Label label12;
    }
}