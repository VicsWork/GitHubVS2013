namespace PowerCalibration
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

            if (disposing)
            {
                if(_tokenSrc != null)
                    _tokenSrc.Dispose();
                if (_task != null)
                    _task.Dispose();
                if (_telnet_connection != null)
                    _telnet_connection.Dispose();
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
            this.labelPowerUUT = new System.Windows.Forms.Label();
            this.timerISAChan = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelEUI = new System.Windows.Forms.Label();
            this.labelIGain = new System.Windows.Forms.Label();
            this.labelVGain = new System.Windows.Forms.Label();
            this.labelIFactor = new System.Windows.Forms.Label();
            this.labelVFactor = new System.Windows.Forms.Label();
            this.labelCurrentUUT = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVoltageUUT = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUUTError = new System.Windows.Forms.TextBox();
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
            this.labelPowerUUT.Click += new System.EventHandler(this.labelPowerUUT_Click);
            // 
            // timerISAChan
            // 
            this.timerISAChan.Enabled = true;
            this.timerISAChan.Interval = 1000;
            this.timerISAChan.Tick += new System.EventHandler(this.timerISAChan_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelEUI);
            this.groupBox1.Controls.Add(this.labelIGain);
            this.groupBox1.Controls.Add(this.labelVGain);
            this.groupBox1.Controls.Add(this.labelIFactor);
            this.groupBox1.Controls.Add(this.labelVFactor);
            this.groupBox1.Controls.Add(this.labelCurrentUUT);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.labelVoltageUUT);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.labelPowerUUT);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 275);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "UUT Power";
            // 
            // labelEUI
            // 
            this.labelEUI.AutoSize = true;
            this.labelEUI.Location = new System.Drawing.Point(13, 243);
            this.labelEUI.Name = "labelEUI";
            this.labelEUI.Size = new System.Drawing.Size(28, 13);
            this.labelEUI.TabIndex = 9;
            this.labelEUI.Text = "EUI:";
            // 
            // labelIGain
            // 
            this.labelIGain.AutoSize = true;
            this.labelIGain.Location = new System.Drawing.Point(13, 220);
            this.labelIGain.Name = "labelIGain";
            this.labelIGain.Size = new System.Drawing.Size(69, 13);
            this.labelIGain.TabIndex = 8;
            this.labelIGain.Text = "IGain Token:";
            // 
            // labelVGain
            // 
            this.labelVGain.AutoSize = true;
            this.labelVGain.Location = new System.Drawing.Point(13, 198);
            this.labelVGain.Name = "labelVGain";
            this.labelVGain.Size = new System.Drawing.Size(73, 13);
            this.labelVGain.TabIndex = 7;
            this.labelVGain.Text = "VGain Token:";
            // 
            // labelIFactor
            // 
            this.labelIFactor.AutoSize = true;
            this.labelIFactor.Location = new System.Drawing.Point(13, 175);
            this.labelIFactor.Name = "labelIFactor";
            this.labelIFactor.Size = new System.Drawing.Size(77, 13);
            this.labelIFactor.TabIndex = 7;
            this.labelIFactor.Text = "Current Factor:";
            // 
            // labelVFactor
            // 
            this.labelVFactor.AutoSize = true;
            this.labelVFactor.Location = new System.Drawing.Point(13, 153);
            this.labelVFactor.Name = "labelVFactor";
            this.labelVFactor.Size = new System.Drawing.Size(79, 13);
            this.labelVFactor.TabIndex = 6;
            this.labelVFactor.Text = "Voltage Factor:";
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
            // textBoxUUTError
            // 
            this.textBoxUUTError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxUUTError.Enabled = false;
            this.textBoxUUTError.ForeColor = System.Drawing.Color.Red;
            this.textBoxUUTError.Location = new System.Drawing.Point(18, 293);
            this.textBoxUUTError.Multiline = true;
            this.textBoxUUTError.Name = "textBoxUUTError";
            this.textBoxUUTError.ReadOnly = true;
            this.textBoxUUTError.Size = new System.Drawing.Size(188, 48);
            this.textBoxUUTError.TabIndex = 5;
            // 
            // Form_PowerMeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 353);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxUUTError);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form_PowerMeter";
            this.Text = "Power Meter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_PowerMeter_FormClosing);
            this.Load += new System.EventHandler(this.Form_PowerMeter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPowerUUT;
        private System.Windows.Forms.Timer timerISAChan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelCurrentUUT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVoltageUUT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUUTError;
        private System.Windows.Forms.Label labelIGain;
        private System.Windows.Forms.Label labelVGain;
        private System.Windows.Forms.Label labelIFactor;
        private System.Windows.Forms.Label labelVFactor;
        private System.Windows.Forms.Label labelEUI;
    }
}