namespace PowerCalibration
{
    partial class Form_Settings2
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
            // Form_Settings2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 284);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(406, 322);
            this.Name = "Form_Settings2";
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
    }
}