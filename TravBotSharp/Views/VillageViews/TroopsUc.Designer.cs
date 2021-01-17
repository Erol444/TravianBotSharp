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
            this.labelTroopsToTrain = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTroopsToTrain = new System.Windows.Forms.ComboBox();
            this.troopsInfo = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTroopsToTrain
            // 
            this.labelTroopsToTrain.AutoSize = true;
            this.labelTroopsToTrain.Location = new System.Drawing.Point(17, 61);
            this.labelTroopsToTrain.Name = "labelTroopsToTrain";
            this.labelTroopsToTrain.Size = new System.Drawing.Size(55, 13);
            this.labelTroopsToTrain.TabIndex = 26;
            this.labelTroopsToTrain.Text = "Selected: ";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(144, 33);
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
            // comboBoxTroopsToTrain
            // 
            this.comboBoxTroopsToTrain.FormattingEnabled = true;
            this.comboBoxTroopsToTrain.Location = new System.Drawing.Point(17, 33);
            this.comboBoxTroopsToTrain.Name = "comboBoxTroopsToTrain";
            this.comboBoxTroopsToTrain.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTroopsToTrain.TabIndex = 23;
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
            // TroopsUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.troopsInfo);
            this.Controls.Add(this.labelTroopsToTrain);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTroopsToTrain);
            this.Name = "TroopsUc";
            this.Size = new System.Drawing.Size(728, 292);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelTroopsToTrain;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTroopsToTrain;
        private System.Windows.Forms.RichTextBox troopsInfo;
        private System.Windows.Forms.Label label2;
    }
}
