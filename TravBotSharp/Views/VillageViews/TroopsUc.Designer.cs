namespace TravBotSharp.Views
{
    partial class TroopsUc
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
            this.button10 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.troopsInfo = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.troopSelectorTrain = new TravBotSharp.UserControls.TroopSelector();
            this.troopSelectorImprove = new TravBotSharp.UserControls.TroopSelector();
            this.scouts = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.scouts)).BeginInit();
            this.SuspendLayout();
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(171, 33);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(45, 23);
            this.button10.TabIndex = 25;
            this.button10.Text = "OK";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Troops to train";
            // 
            // troopsInfo
            // 
            this.troopsInfo.Location = new System.Drawing.Point(269, 33);
            this.troopsInfo.Name = "troopsInfo";
            this.troopsInfo.ReadOnly = true;
            this.troopsInfo.Size = new System.Drawing.Size(456, 231);
            this.troopsInfo.TabIndex = 28;
            this.troopsInfo.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(266, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 16);
            this.label2.TabIndex = 29;
            this.label2.Text = "Village troops info";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Troops to improve";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(171, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 23);
            this.button1.TabIndex = 32;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // troopSelectorTrain
            // 
            this.troopSelectorTrain.BackColor = System.Drawing.SystemColors.ControlLight;
            this.troopSelectorTrain.Location = new System.Drawing.Point(20, 30);
            this.troopSelectorTrain.Name = "troopSelectorTrain";
            this.troopSelectorTrain.SelectedTroop = null;
            this.troopSelectorTrain.Size = new System.Drawing.Size(145, 33);
            this.troopSelectorTrain.TabIndex = 33;
            // 
            // troopSelectorImprove
            // 
            this.troopSelectorImprove.BackColor = System.Drawing.SystemColors.ControlLight;
            this.troopSelectorImprove.Location = new System.Drawing.Point(20, 94);
            this.troopSelectorImprove.Name = "troopSelectorImprove";
            this.troopSelectorImprove.SelectedTroop = null;
            this.troopSelectorImprove.Size = new System.Drawing.Size(145, 33);
            this.troopSelectorImprove.TabIndex = 34;
            // 
            // scouts
            // 
            this.scouts.Location = new System.Drawing.Point(55, 174);
            this.scouts.Name = "scouts";
            this.scouts.Size = new System.Drawing.Size(110, 20);
            this.scouts.TabIndex = 35;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(171, 171);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(45, 23);
            this.button2.TabIndex = 36;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Send";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(52, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "to all your villages";
            // 
            // TroopsUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.scouts);
            this.Controls.Add(this.troopSelectorImprove);
            this.Controls.Add(this.troopSelectorTrain);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.troopsInfo);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.label1);
            this.Name = "TroopsUc";
            this.Size = new System.Drawing.Size(728, 292);
            ((System.ComponentModel.ISupportInitialize)(this.scouts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox troopsInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private UserControls.TroopSelector troopSelectorTrain;
        private UserControls.TroopSelector troopSelectorImprove;
        private System.Windows.Forms.NumericUpDown scouts;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
