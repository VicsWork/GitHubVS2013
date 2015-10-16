namespace PowerCalibration
{
    partial class Form_Calculator
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
            this.textBoxHex24 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.buttonCalculate24bit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonCalculateGain = new System.Windows.Forms.Button();
            this.textBoxGainDec = new System.Windows.Forms.TextBox();
            this.textBoxGainHex = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxHex24
            // 
            this.textBoxHex24.Location = new System.Drawing.Point(12, 51);
            this.textBoxHex24.Name = "textBoxHex24";
            this.textBoxHex24.Size = new System.Drawing.Size(179, 20);
            this.textBoxHex24.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(223, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(179, 20);
            this.textBox2.TabIndex = 1;
            // 
            // buttonCalculate24bit
            // 
            this.buttonCalculate24bit.Location = new System.Drawing.Point(408, 48);
            this.buttonCalculate24bit.Name = "buttonCalculate24bit";
            this.buttonCalculate24bit.Size = new System.Drawing.Size(75, 23);
            this.buttonCalculate24bit.TabIndex = 2;
            this.buttonCalculate24bit.Text = "Calcuclate";
            this.buttonCalculate24bit.UseVisualStyleBackColor = true;
            this.buttonCalculate24bit.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "24bit hex";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "decimal";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Gain decimal";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Gain hex";
            // 
            // buttonCalculateGain
            // 
            this.buttonCalculateGain.Location = new System.Drawing.Point(411, 131);
            this.buttonCalculateGain.Name = "buttonCalculateGain";
            this.buttonCalculateGain.Size = new System.Drawing.Size(75, 23);
            this.buttonCalculateGain.TabIndex = 7;
            this.buttonCalculateGain.Text = "Calcuclate";
            this.buttonCalculateGain.UseVisualStyleBackColor = true;
            this.buttonCalculateGain.Click += new System.EventHandler(this.buttonCalculateGain_Click);
            // 
            // textBoxGainDec
            // 
            this.textBoxGainDec.Location = new System.Drawing.Point(226, 134);
            this.textBoxGainDec.Name = "textBoxGainDec";
            this.textBoxGainDec.Size = new System.Drawing.Size(179, 20);
            this.textBoxGainDec.TabIndex = 6;
            // 
            // textBoxGainHex
            // 
            this.textBoxGainHex.Location = new System.Drawing.Point(15, 134);
            this.textBoxGainHex.Name = "textBoxGainHex";
            this.textBoxGainHex.Size = new System.Drawing.Size(179, 20);
            this.textBoxGainHex.TabIndex = 5;
            // 
            // Form_Calculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 213);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCalculateGain);
            this.Controls.Add(this.textBoxGainDec);
            this.Controls.Add(this.textBoxGainHex);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCalculate24bit);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBoxHex24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form_Calculator";
            this.Text = "Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHex24;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonCalculate24bit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonCalculateGain;
        private System.Windows.Forms.TextBox textBoxGainDec;
        private System.Windows.Forms.TextBox textBoxGainHex;
    }
}