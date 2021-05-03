namespace TravBotSharp.Views
{
    partial class InfoUc
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.villageInfo = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.maxInterval = new System.Windows.Forms.NumericUpDown();
            this.minInterval = new System.Windows.Forms.NumericUpDown();
            this.TrainSettlers = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // villageInfo
            // 
            this.villageInfo.Location = new System.Drawing.Point(12, 33);
            this.villageInfo.Name = "villageInfo";
            this.villageInfo.ReadOnly = true;
            this.villageInfo.Size = new System.Drawing.Size(448, 353);
            this.villageInfo.TabIndex = 28;
            this.villageInfo.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 16);
            this.label2.TabIndex = 29;
            this.label2.Text = "Village info";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.maxInterval);
            this.panel1.Controls.Add(this.minInterval);
            this.panel1.Location = new System.Drawing.Point(525, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 90);
            this.panel1.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 16);
            this.label1.TabIndex = 116;
            this.label1.Text = "Update village frequency";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(122, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 13);
            this.label12.TabIndex = 115;
            this.label12.Text = "min";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(122, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 13);
            this.label11.TabIndex = 114;
            this.label11.Text = "min";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 113;
            this.label10.Text = "Max";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 35);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 112;
            this.label9.Text = "Min";
            // 
            // maxInterval
            // 
            this.maxInterval.Location = new System.Drawing.Point(38, 59);
            this.maxInterval.Maximum = new decimal(new int[] {
            10001,
            0,
            0,
            0});
            this.maxInterval.Name = "maxInterval";
            this.maxInterval.Size = new System.Drawing.Size(78, 20);
            this.maxInterval.TabIndex = 111;
            this.maxInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.maxInterval.ValueChanged += new System.EventHandler(this.maxInterval_ValueChanged);
            // 
            // minInterval
            // 
            this.minInterval.Location = new System.Drawing.Point(38, 33);
            this.minInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minInterval.Name = "minInterval";
            this.minInterval.Size = new System.Drawing.Size(78, 20);
            this.minInterval.TabIndex = 110;
            this.minInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.minInterval.ValueChanged += new System.EventHandler(this.minInterval_ValueChanged);
            // 
            // TrainSettlers
            // 
            this.TrainSettlers.Location = new System.Drawing.Point(525, 144);
            this.TrainSettlers.Name = "TrainSettlers";
            this.TrainSettlers.Size = new System.Drawing.Size(75, 23);
            this.TrainSettlers.TabIndex = 31;
            this.TrainSettlers.Text = "Train settler";
            this.TrainSettlers.UseVisualStyleBackColor = true;
            this.TrainSettlers.Click += new System.EventHandler(this.TrainSettlers_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(525, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 32;
            this.button1.Text = "Send settler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // InfoUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TrainSettlers);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.villageInfo);
            this.Name = "InfoUc";
            this.Size = new System.Drawing.Size(851, 423);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox villageInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown maxInterval;
        private System.Windows.Forms.NumericUpDown minInterval;
        private System.Windows.Forms.Button TrainSettlers;
        private System.Windows.Forms.Button button1;
    }
}
