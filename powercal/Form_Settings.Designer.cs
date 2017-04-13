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
            this.components = new System.ComponentModel.Container();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPageEmber = new System.Windows.Forms.TabPage();
            this.textBox_EmberInterfaceAddress = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_EmberInterface = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonEmberBinPathBrowse = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_EmberBinPath = new System.Windows.Forms.TextBox();
            this.tabPageDIO = new System.Windows.Forms.TabPage();
            this.numericUpDown_VacVdc = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_DIOCtrollerTypes = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NumericUpDown_Ember = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericUpDown_Load = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDown_ACPower = new System.Windows.Forms.NumericUpDown();
            this.tabPageCalibration = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_LoadVoltageValue = new System.Windows.Forms.TextBox();
            this.textBox_LoadResitorValue = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_LoadPower = new System.Windows.Forms.TextBox();
            this.textBox_LoadCurrent = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox_GainCurrentMax = new System.Windows.Forms.TextBox();
            this.textBox_GainCurrentMin = new System.Windows.Forms.TextBox();
            this.textBox_GainVoltageMax = new System.Windows.Forms.TextBox();
            this.textBox_GainVoltageMin = new System.Windows.Forms.TextBox();
            this.tagPageMeasurement = new System.Windows.Forms.TabPage();
            this.checkBox_PreProTest = new System.Windows.Forms.CheckBox();
            this.checkBox_ManualMultiMeter = new System.Windows.Forms.CheckBox();
            this.TextBox_MeterCOM = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageMisc = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBox_PlaySounds = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxCode_MinOnPass = new System.Windows.Forms.CheckBox();
            this.tabPageSuper = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelDBConnectStr = new System.Windows.Forms.Label();
            this.checkBox_EnableDBReporting = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_disableRdProtectionBeforeCode = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_enableRdProt = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TabControl.SuspendLayout();
            this.tabPageEmber.SuspendLayout();
            this.tabPageDIO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_VacVdc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).BeginInit();
            this.tabPageCalibration.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tagPageMeasurement.SuspendLayout();
            this.tabPageMisc.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPageSuper.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.tabPageEmber);
            this.TabControl.Controls.Add(this.tabPageDIO);
            this.TabControl.Controls.Add(this.tabPageCalibration);
            this.TabControl.Controls.Add(this.tagPageMeasurement);
            this.TabControl.Controls.Add(this.tabPageMisc);
            this.TabControl.Controls.Add(this.tabPageSuper);
            this.TabControl.Location = new System.Drawing.Point(9, 12);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(372, 231);
            this.TabControl.TabIndex = 0;
            // 
            // tabPageEmber
            // 
            this.tabPageEmber.Controls.Add(this.textBox_EmberInterfaceAddress);
            this.tabPageEmber.Controls.Add(this.label9);
            this.tabPageEmber.Controls.Add(this.comboBox_EmberInterface);
            this.tabPageEmber.Controls.Add(this.label8);
            this.tabPageEmber.Controls.Add(this.buttonEmberBinPathBrowse);
            this.tabPageEmber.Controls.Add(this.label7);
            this.tabPageEmber.Controls.Add(this.textBox_EmberBinPath);
            this.tabPageEmber.Location = new System.Drawing.Point(4, 22);
            this.tabPageEmber.Name = "tabPageEmber";
            this.tabPageEmber.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEmber.Size = new System.Drawing.Size(364, 205);
            this.tabPageEmber.TabIndex = 0;
            this.tabPageEmber.Text = "Ember";
            this.tabPageEmber.UseVisualStyleBackColor = true;
            // 
            // textBox_EmberInterfaceAddress
            // 
            this.textBox_EmberInterfaceAddress.Location = new System.Drawing.Point(206, 10);
            this.textBox_EmberInterfaceAddress.Name = "textBox_EmberInterfaceAddress";
            this.textBox_EmberInterfaceAddress.Size = new System.Drawing.Size(125, 20);
            this.textBox_EmberInterfaceAddress.TabIndex = 1;
            this.textBox_EmberInterfaceAddress.TextChanged += new System.EventHandler(this.textBox_EmberInterfaceAddress_TextChanged);
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
            // comboBox_EmberInterface
            // 
            this.comboBox_EmberInterface.FormattingEnabled = true;
            this.comboBox_EmberInterface.Items.AddRange(new object[] {
            "USB",
            "IP"});
            this.comboBox_EmberInterface.Location = new System.Drawing.Point(61, 10);
            this.comboBox_EmberInterface.Name = "comboBox_EmberInterface";
            this.comboBox_EmberInterface.Size = new System.Drawing.Size(64, 21);
            this.comboBox_EmberInterface.TabIndex = 0;
            this.comboBox_EmberInterface.SelectedIndexChanged += new System.EventHandler(this.comboBox_EmberInterface_SelectedIndexChanged);
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
            this.buttonEmberBinPathBrowse.Click += new System.EventHandler(this.button_EmberBinPathBrowse_Click);
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
            // textBox_EmberBinPath
            // 
            this.textBox_EmberBinPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_EmberBinPath.Location = new System.Drawing.Point(41, 53);
            this.textBox_EmberBinPath.Name = "textBox_EmberBinPath";
            this.textBox_EmberBinPath.Size = new System.Drawing.Size(236, 20);
            this.textBox_EmberBinPath.TabIndex = 2;
            this.textBox_EmberBinPath.TextChanged += new System.EventHandler(this.TextBox_EmberBinPath_TextChanged);
            // 
            // tabPageDIO
            // 
            this.tabPageDIO.Controls.Add(this.numericUpDown_VacVdc);
            this.tabPageDIO.Controls.Add(this.label5);
            this.tabPageDIO.Controls.Add(this.label1);
            this.tabPageDIO.Controls.Add(this.comboBox_DIOCtrollerTypes);
            this.tabPageDIO.Controls.Add(this.label6);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_Ember);
            this.tabPageDIO.Controls.Add(this.label4);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_Load);
            this.tabPageDIO.Controls.Add(this.label3);
            this.tabPageDIO.Controls.Add(this.NumericUpDown_ACPower);
            this.tabPageDIO.Location = new System.Drawing.Point(4, 22);
            this.tabPageDIO.Name = "tabPageDIO";
            this.tabPageDIO.Size = new System.Drawing.Size(364, 205);
            this.tabPageDIO.TabIndex = 2;
            this.tabPageDIO.Text = "DIO";
            this.tabPageDIO.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_VacVdc
            // 
            this.numericUpDown_VacVdc.Location = new System.Drawing.Point(188, 126);
            this.numericUpDown_VacVdc.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown_VacVdc.Name = "numericUpDown_VacVdc";
            this.numericUpDown_VacVdc.Size = new System.Drawing.Size(31, 20);
            this.numericUpDown_VacVdc.TabIndex = 4;
            this.numericUpDown_VacVdc.ValueChanged += new System.EventHandler(this.NumericUpDown_DIOLine_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(101, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Vac/Vdc";
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
            // comboBox_DIOCtrollerTypes
            // 
            this.comboBox_DIOCtrollerTypes.FormattingEnabled = true;
            this.comboBox_DIOCtrollerTypes.Location = new System.Drawing.Point(150, 14);
            this.comboBox_DIOCtrollerTypes.Name = "comboBox_DIOCtrollerTypes";
            this.comboBox_DIOCtrollerTypes.Size = new System.Drawing.Size(111, 21);
            this.comboBox_DIOCtrollerTypes.TabIndex = 0;
            this.comboBox_DIOCtrollerTypes.SelectedIndexChanged += new System.EventHandler(this.comboBox_DIOCtrollerTypes_SelectedIndexChanged);
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
            // tabPageCalibration
            // 
            this.tabPageCalibration.Controls.Add(this.tableLayoutPanel1);
            this.tabPageCalibration.Location = new System.Drawing.Point(4, 22);
            this.tabPageCalibration.Name = "tabPageCalibration";
            this.tabPageCalibration.Size = new System.Drawing.Size(364, 205);
            this.tabPageCalibration.TabIndex = 5;
            this.tabPageCalibration.Text = "Calibration";
            this.tabPageCalibration.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label10, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_LoadVoltageValue, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_LoadResitorValue, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label15, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label16, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox_LoadPower, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox_LoadCurrent, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.textBox_GainCurrentMax, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBox_GainCurrentMin, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.textBox_GainVoltageMax, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.textBox_GainVoltageMin, 1, 8);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(358, 188);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(109, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Value";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Load Voltage (VAC)";
            // 
            // textBox_LoadVoltageValue
            // 
            this.textBox_LoadVoltageValue.Location = new System.Drawing.Point(109, 23);
            this.textBox_LoadVoltageValue.Name = "textBox_LoadVoltageValue";
            this.textBox_LoadVoltageValue.Size = new System.Drawing.Size(100, 20);
            this.textBox_LoadVoltageValue.TabIndex = 0;
            this.textBox_LoadVoltageValue.Tag = "voltage";
            this.textBox_LoadVoltageValue.Text = "120";
            this.textBox_LoadVoltageValue.TextChanged += new System.EventHandler(this.textBox_loadValues_TextChanged);
            // 
            // textBox_LoadResitorValue
            // 
            this.textBox_LoadResitorValue.Location = new System.Drawing.Point(109, 43);
            this.textBox_LoadResitorValue.Name = "textBox_LoadResitorValue";
            this.textBox_LoadResitorValue.Size = new System.Drawing.Size(100, 20);
            this.textBox_LoadResitorValue.TabIndex = 1;
            this.textBox_LoadResitorValue.Tag = "resistance";
            this.textBox_LoadResitorValue.Text = "500";
            this.textBox_LoadResitorValue.TextChanged += new System.EventHandler(this.textBox_loadValues_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 40);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Load (Ohms)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 80);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "Load Current (A)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 60);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(84, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Load Power (W)";
            // 
            // textBox_LoadPower
            // 
            this.textBox_LoadPower.Location = new System.Drawing.Point(109, 63);
            this.textBox_LoadPower.Name = "textBox_LoadPower";
            this.textBox_LoadPower.Size = new System.Drawing.Size(100, 20);
            this.textBox_LoadPower.TabIndex = 2;
            this.textBox_LoadPower.Tag = "power";
            this.textBox_LoadPower.TextChanged += new System.EventHandler(this.textBox_loadValues_TextChanged);
            // 
            // textBox_LoadCurrent
            // 
            this.textBox_LoadCurrent.Location = new System.Drawing.Point(109, 83);
            this.textBox_LoadCurrent.Name = "textBox_LoadCurrent";
            this.textBox_LoadCurrent.ReadOnly = true;
            this.textBox_LoadCurrent.Size = new System.Drawing.Size(100, 20);
            this.textBox_LoadCurrent.TabIndex = 3;
            this.textBox_LoadCurrent.Tag = "current";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Current Gain Max";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 120);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Current Gain Min";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 140);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 13;
            this.label17.Text = "Voltage Gain Max";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 160);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(88, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Voltage Gain Min";
            // 
            // textBox_GainCurrentMax
            // 
            this.textBox_GainCurrentMax.Location = new System.Drawing.Point(109, 103);
            this.textBox_GainCurrentMax.Name = "textBox_GainCurrentMax";
            this.textBox_GainCurrentMax.Size = new System.Drawing.Size(100, 20);
            this.textBox_GainCurrentMax.TabIndex = 15;
            this.textBox_GainCurrentMax.TextChanged += new System.EventHandler(this.textBox_GainCurrentMax_TextChanged);
            // 
            // textBox_GainCurrentMin
            // 
            this.textBox_GainCurrentMin.Location = new System.Drawing.Point(109, 123);
            this.textBox_GainCurrentMin.Name = "textBox_GainCurrentMin";
            this.textBox_GainCurrentMin.Size = new System.Drawing.Size(100, 20);
            this.textBox_GainCurrentMin.TabIndex = 16;
            this.textBox_GainCurrentMin.TextChanged += new System.EventHandler(this.textBox_GainCurrentMin_TextChanged);
            // 
            // textBox_GainVoltageMax
            // 
            this.textBox_GainVoltageMax.Location = new System.Drawing.Point(109, 143);
            this.textBox_GainVoltageMax.Name = "textBox_GainVoltageMax";
            this.textBox_GainVoltageMax.Size = new System.Drawing.Size(100, 20);
            this.textBox_GainVoltageMax.TabIndex = 17;
            this.textBox_GainVoltageMax.TextChanged += new System.EventHandler(this.textBox_GainVolatgeMax_TextChanged);
            // 
            // textBox_GainVolatgeMin
            // 
            this.textBox_GainVoltageMin.Location = new System.Drawing.Point(109, 163);
            this.textBox_GainVoltageMin.Name = "textBox_GainVolatgeMin";
            this.textBox_GainVoltageMin.Size = new System.Drawing.Size(100, 20);
            this.textBox_GainVoltageMin.TabIndex = 18;
            this.textBox_GainVoltageMin.TextChanged += new System.EventHandler(this.textBox_GainVolatgeMin_TextChanged);
            // 
            // tagPageMeasurement
            // 
            this.tagPageMeasurement.Controls.Add(this.checkBox_PreProTest);
            this.tagPageMeasurement.Controls.Add(this.checkBox_ManualMultiMeter);
            this.tagPageMeasurement.Controls.Add(this.TextBox_MeterCOM);
            this.tagPageMeasurement.Controls.Add(this.label2);
            this.tagPageMeasurement.Location = new System.Drawing.Point(4, 22);
            this.tagPageMeasurement.Name = "tagPageMeasurement";
            this.tagPageMeasurement.Size = new System.Drawing.Size(364, 205);
            this.tagPageMeasurement.TabIndex = 3;
            this.tagPageMeasurement.Text = "Measurement";
            this.tagPageMeasurement.UseVisualStyleBackColor = true;
            // 
            // checkBox_PreProTest
            // 
            this.checkBox_PreProTest.AutoSize = true;
            this.checkBox_PreProTest.Checked = true;
            this.checkBox_PreProTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_PreProTest.Location = new System.Drawing.Point(4, 40);
            this.checkBox_PreProTest.Name = "checkBox_PreProTest";
            this.checkBox_PreProTest.Size = new System.Drawing.Size(92, 17);
            this.checkBox_PreProTest.TabIndex = 11;
            this.checkBox_PreProTest.Text = "Pre/Post Test";
            this.checkBox_PreProTest.UseVisualStyleBackColor = true;
            this.checkBox_PreProTest.CheckedChanged += new System.EventHandler(this.checkBoxPreProTest_CheckedChanged);
            // 
            // checkBox_ManualMultiMeter
            // 
            this.checkBox_ManualMultiMeter.AutoSize = true;
            this.checkBox_ManualMultiMeter.Location = new System.Drawing.Point(153, 14);
            this.checkBox_ManualMultiMeter.Name = "checkBox_ManualMultiMeter";
            this.checkBox_ManualMultiMeter.Size = new System.Drawing.Size(61, 17);
            this.checkBox_ManualMultiMeter.TabIndex = 10;
            this.checkBox_ManualMultiMeter.Text = "Manual";
            this.checkBox_ManualMultiMeter.UseVisualStyleBackColor = true;
            this.checkBox_ManualMultiMeter.CheckedChanged += new System.EventHandler(this.checkBox_manualMultiMeter_CheckedChanged);
            // 
            // TextBox_MeterCOM
            // 
            this.TextBox_MeterCOM.Location = new System.Drawing.Point(88, 12);
            this.TextBox_MeterCOM.Name = "TextBox_MeterCOM";
            this.TextBox_MeterCOM.Size = new System.Drawing.Size(58, 20);
            this.TextBox_MeterCOM.TabIndex = 9;
            this.TextBox_MeterCOM.WordWrap = false;
            this.TextBox_MeterCOM.TextChanged += new System.EventHandler(this.TextBoxMeterCOM_TextChanged);
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
            // tabPageMisc
            // 
            this.tabPageMisc.Controls.Add(this.groupBox4);
            this.tabPageMisc.Controls.Add(this.groupBox3);
            this.tabPageMisc.Location = new System.Drawing.Point(4, 22);
            this.tabPageMisc.Name = "tabPageMisc";
            this.tabPageMisc.Size = new System.Drawing.Size(364, 205);
            this.tabPageMisc.TabIndex = 6;
            this.tabPageMisc.Text = "Misc";
            this.tabPageMisc.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.checkBox_PlaySounds);
            this.groupBox4.Location = new System.Drawing.Point(3, 65);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(358, 51);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Sounds";
            // 
            // checkBox_PlaySounds
            // 
            this.checkBox_PlaySounds.AutoSize = true;
            this.checkBox_PlaySounds.Location = new System.Drawing.Point(7, 20);
            this.checkBox_PlaySounds.Name = "checkBox_PlaySounds";
            this.checkBox_PlaySounds.Size = new System.Drawing.Size(85, 17);
            this.checkBox_PlaySounds.TabIndex = 0;
            this.checkBox_PlaySounds.Text = "Play Sounds";
            this.checkBox_PlaySounds.UseVisualStyleBackColor = true;
            this.checkBox_PlaySounds.CheckedChanged += new System.EventHandler(this.checkBox_playSounds_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.checkBoxCode_MinOnPass);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(358, 55);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Coding";
            // 
            // checkBoxCode_MinOnPass
            // 
            this.checkBoxCode_MinOnPass.AutoSize = true;
            this.checkBoxCode_MinOnPass.Location = new System.Drawing.Point(6, 17);
            this.checkBoxCode_MinOnPass.Name = "checkBoxCode_MinOnPass";
            this.checkBoxCode_MinOnPass.Size = new System.Drawing.Size(66, 17);
            this.checkBoxCode_MinOnPass.TabIndex = 7;
            this.checkBoxCode_MinOnPass.Text = "Minimize";
            this.toolTip1.SetToolTip(this.checkBoxCode_MinOnPass, "Minimizes the coding windows upon a successfull completion");
            this.checkBoxCode_MinOnPass.UseVisualStyleBackColor = true;
            this.checkBoxCode_MinOnPass.CheckedChanged += new System.EventHandler(this.checkBox_codeMinOnPass_CheckedChanged);
            // 
            // tabPageSuper
            // 
            this.tabPageSuper.Controls.Add(this.groupBox2);
            this.tabPageSuper.Controls.Add(this.groupBox5);
            this.tabPageSuper.Controls.Add(this.groupBox1);
            this.tabPageSuper.Location = new System.Drawing.Point(4, 22);
            this.tabPageSuper.Name = "tabPageSuper";
            this.tabPageSuper.Size = new System.Drawing.Size(364, 205);
            this.tabPageSuper.TabIndex = 7;
            this.tabPageSuper.Text = "Super";
            this.tabPageSuper.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelDBConnectStr);
            this.groupBox2.Controls.Add(this.checkBox_EnableDBReporting);
            this.groupBox2.Location = new System.Drawing.Point(3, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(358, 62);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database";
            // 
            // labelDBConnectStr
            // 
            this.labelDBConnectStr.AutoSize = true;
            this.labelDBConnectStr.Location = new System.Drawing.Point(7, 43);
            this.labelDBConnectStr.Name = "labelDBConnectStr";
            this.labelDBConnectStr.Size = new System.Drawing.Size(0, 13);
            this.labelDBConnectStr.TabIndex = 7;
            // 
            // checkBox_EnableDBReporting
            // 
            this.checkBox_EnableDBReporting.AutoSize = true;
            this.checkBox_EnableDBReporting.Location = new System.Drawing.Point(6, 22);
            this.checkBox_EnableDBReporting.Name = "checkBox_EnableDBReporting";
            this.checkBox_EnableDBReporting.Size = new System.Drawing.Size(108, 17);
            this.checkBox_EnableDBReporting.TabIndex = 6;
            this.checkBox_EnableDBReporting.Text = "Enable Reporting";
            this.toolTip1.SetToolTip(this.checkBox_EnableDBReporting, "Enables reporting to the database");
            this.checkBox_EnableDBReporting.UseVisualStyleBackColor = true;
            this.checkBox_EnableDBReporting.CheckedChanged += new System.EventHandler(this.checkBox_enableDBReporting_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.checkBox_disableRdProtectionBeforeCode);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(358, 55);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Coding";
            // 
            // checkBox_disableRdProtectionBeforeCode
            // 
            this.checkBox_disableRdProtectionBeforeCode.AutoSize = true;
            this.checkBox_disableRdProtectionBeforeCode.Location = new System.Drawing.Point(6, 17);
            this.checkBox_disableRdProtectionBeforeCode.Name = "checkBox_disableRdProtectionBeforeCode";
            this.checkBox_disableRdProtectionBeforeCode.Size = new System.Drawing.Size(203, 17);
            this.checkBox_disableRdProtectionBeforeCode.TabIndex = 7;
            this.checkBox_disableRdProtectionBeforeCode.Text = "Disable read protection before coding";
            this.checkBox_disableRdProtectionBeforeCode.UseVisualStyleBackColor = true;
            this.checkBox_disableRdProtectionBeforeCode.CheckedChanged += new System.EventHandler(this.checkBox_disableRdProtectionBeforeCode_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBox_enableRdProt);
            this.groupBox1.Location = new System.Drawing.Point(3, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 55);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Post Process";
            // 
            // checkBox_enableRdProt
            // 
            this.checkBox_enableRdProt.AutoSize = true;
            this.checkBox_enableRdProt.Location = new System.Drawing.Point(6, 17);
            this.checkBox_enableRdProt.Name = "checkBox_enableRdProt";
            this.checkBox_enableRdProt.Size = new System.Drawing.Size(211, 17);
            this.checkBox_enableRdProt.TabIndex = 7;
            this.checkBox_enableRdProt.Text = "Enable read protection after completion";
            this.checkBox_enableRdProt.UseVisualStyleBackColor = true;
            this.checkBox_enableRdProt.CheckedChanged += new System.EventHandler(this.checkBox_enableRdProt_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(198, 249);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "&Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(117, 249);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // Form_Settings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(390, 284);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.TabControl);
            this.MinimumSize = new System.Drawing.Size(406, 322);
            this.Name = "Form_Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            this.TabControl.ResumeLayout(false);
            this.tabPageEmber.ResumeLayout(false);
            this.tabPageEmber.PerformLayout();
            this.tabPageDIO.ResumeLayout(false);
            this.tabPageDIO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_VacVdc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Ember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ACPower)).EndInit();
            this.tabPageCalibration.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tagPageMeasurement.ResumeLayout(false);
            this.tagPageMeasurement.PerformLayout();
            this.tabPageMisc.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPageSuper.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageEmber;
        public System.Windows.Forms.TextBox textBox_EmberInterfaceAddress;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.ComboBox comboBox_EmberInterface;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonEmberBinPathBrowse;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox textBox_EmberBinPath;
        private System.Windows.Forms.TabPage tabPageDIO;
        public System.Windows.Forms.NumericUpDown numericUpDown_VacVdc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBox_DIOCtrollerTypes;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Ember;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NumericUpDown_Load;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NumericUpDown_ACPower;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabPage tagPageMeasurement;
        private System.Windows.Forms.CheckBox checkBox_PreProTest;
        public System.Windows.Forms.CheckBox checkBox_ManualMultiMeter;
        public System.Windows.Forms.TextBox TextBox_MeterCOM;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabPageCalibration;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_LoadVoltageValue;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_LoadResitorValue;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox_LoadCurrent;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox_LoadPower;
        private System.Windows.Forms.TabPage tabPageMisc;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxCode_MinOnPass;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabPageSuper;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_enableRdProt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelDBConnectStr;
        private System.Windows.Forms.CheckBox checkBox_EnableDBReporting;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBox_PlaySounds;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBox_disableRdProtectionBeforeCode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox_GainCurrentMax;
        private System.Windows.Forms.TextBox textBox_GainCurrentMin;
        private System.Windows.Forms.TextBox textBox_GainVoltageMax;
        private System.Windows.Forms.TextBox textBox_GainVoltageMin;
    }
}