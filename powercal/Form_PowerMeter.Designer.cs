namespace powercal
{
    partial class Form_PowerMeter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PowerMeter));
            this.labelPowerUUT = new System.Windows.Forms.Label();
            this.timerISAChan = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelCurrentUUT = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVoltageUUT = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPowerUUT
            // 
            this.labelPowerUUT.AutoSize = true;
            this.labelPowerUUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPowerUUT.Location = new System.Drawing.Point(35, 16);
            this.labelPowerUUT.Name = "labelPowerUUT";
            this.labelPowerUUT.Size = new System.Drawing.Size(118, 55);
            this.labelPowerUUT.TabIndex = 0;
            this.labelPowerUUT.Text = "0.00";
            // 
            // timerISAChan
            // 
            this.timerISAChan.Enabled = true;
            this.timerISAChan.Interval = 1000;
            this.timerISAChan.Tick += new System.EventHandler(this.timerISAChan_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelCurrentUUT);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.labelVoltageUUT);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.labelPowerUUT);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 174);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "UUT";
            // 
            // labelCurrentUUT
            // 
            this.labelCurrentUUT.AutoSize = true;
            this.labelCurrentUUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentUUT.Location = new System.Drawing.Point(84, 112);
            this.labelCurrentUUT.Name = "labelCurrentUUT";
            this.labelCurrentUUT.Size = new System.Drawing.Size(40, 20);
            this.labelCurrentUUT.TabIndex = 4;
            this.labelCurrentUUT.Text = "0.00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current:";
            // 
            // labelVoltageUUT
            // 
            this.labelVoltageUUT.AutoSize = true;
            this.labelVoltageUUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVoltageUUT.Location = new System.Drawing.Point(84, 87);
            this.labelVoltageUUT.Name = "labelVoltageUUT";
            this.labelVoltageUUT.Size = new System.Drawing.Size(40, 20);
            this.labelVoltageUUT.TabIndex = 2;
            this.labelVoltageUUT.Text = "0.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Voltage: ";
            // 
            // Form_PowerMeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 228);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_PowerMeter";
            this.Text = "Power Meter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_PowerMeter_FormClosing);
            this.Load += new System.EventHandler(this.Form_PowerMeter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPowerUUT;
        private System.Windows.Forms.Timer timerISAChan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelCurrentUUT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVoltageUUT;
        private System.Windows.Forms.Label label1;
    }
}