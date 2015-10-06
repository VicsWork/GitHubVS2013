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
            this.labelPowerCS = new System.Windows.Forms.Label();
            this.buttonStartCS = new System.Windows.Forms.Button();
            this.timerISAChan = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labelPowerCS
            // 
            this.labelPowerCS.AutoSize = true;
            this.labelPowerCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPowerCS.Location = new System.Drawing.Point(37, 29);
            this.labelPowerCS.Name = "labelPowerCS";
            this.labelPowerCS.Size = new System.Drawing.Size(172, 55);
            this.labelPowerCS.TabIndex = 0;
            this.labelPowerCS.Text = "0.0000";
            // 
            // buttonStartCS
            // 
            this.buttonStartCS.Location = new System.Drawing.Point(86, 119);
            this.buttonStartCS.Name = "buttonStartCS";
            this.buttonStartCS.Size = new System.Drawing.Size(75, 23);
            this.buttonStartCS.TabIndex = 1;
            this.buttonStartCS.Text = "Start";
            this.buttonStartCS.UseVisualStyleBackColor = true;
            this.buttonStartCS.Click += new System.EventHandler(this.buttonStartCS_Click);
            // 
            // timerISAChan
            // 
            this.timerISAChan.Enabled = true;
            this.timerISAChan.Interval = 1000;
            this.timerISAChan.Tick += new System.EventHandler(this.timerISAChan_Tick);
            // 
            // Form_PowerMeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 366);
            this.Controls.Add(this.buttonStartCS);
            this.Controls.Add(this.labelPowerCS);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_PowerMeter";
            this.Text = "Power Meter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_PowerMeter_FormClosing);
            this.Load += new System.EventHandler(this.Form_PowerMeter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPowerCS;
        private System.Windows.Forms.Button buttonStartCS;
        private System.Windows.Forms.Timer timerISAChan;
    }
}