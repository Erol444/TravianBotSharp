namespace TravBotSharp.Views
{
    partial class QuestsUc
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
            this.claimDaily = new System.Windows.Forms.CheckBox();
            this.claimBeginner = new System.Windows.Forms.CheckBox();
            this.claimVillButton = new System.Windows.Forms.Button();
            this.claimVillLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.claimVill = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // claimDaily
            // 
            this.claimDaily.AutoSize = true;
            this.claimDaily.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.claimDaily.Location = new System.Drawing.Point(20, 21);
            this.claimDaily.Name = "claimDaily";
            this.claimDaily.Size = new System.Drawing.Size(164, 20);
            this.claimDaily.TabIndex = 4;
            this.claimDaily.Text = "Auto claim daily quests";
            this.claimDaily.UseVisualStyleBackColor = true;
            this.claimDaily.CheckedChanged += new System.EventHandler(this.claimDailyQuests_CheckedChanged);
            // 
            // claimBeginner
            // 
            this.claimBeginner.AutoSize = true;
            this.claimBeginner.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.claimBeginner.Location = new System.Drawing.Point(20, 44);
            this.claimBeginner.Name = "claimBeginner";
            this.claimBeginner.Size = new System.Drawing.Size(188, 20);
            this.claimBeginner.TabIndex = 19;
            this.claimBeginner.Text = "Auto claim beginner quests";
            this.claimBeginner.UseVisualStyleBackColor = true;
            this.claimBeginner.CheckedChanged += new System.EventHandler(this.claimBeginnerQuests_CheckedChanged);
            // 
            // claimVillButton
            // 
            this.claimVillButton.Location = new System.Drawing.Point(392, 41);
            this.claimVillButton.Name = "claimVillButton";
            this.claimVillButton.Size = new System.Drawing.Size(71, 23);
            this.claimVillButton.TabIndex = 119;
            this.claimVillButton.Text = "OK";
            this.claimVillButton.UseVisualStyleBackColor = true;
            this.claimVillButton.Click += new System.EventHandler(this.claimVillButton_Click);
            // 
            // SupplyResVillageSelected
            // 
            this.claimVillLabel.AutoSize = true;
            this.claimVillLabel.Location = new System.Drawing.Point(255, 69);
            this.claimVillLabel.Name = "SupplyResVillageSelected";
            this.claimVillLabel.Size = new System.Drawing.Size(55, 13);
            this.claimVillLabel.TabIndex = 118;
            this.claimVillLabel.Text = "Selected: ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(255, 23);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(173, 16);
            this.label17.TabIndex = 117;
            this.label17.Text = "Claim rewards in village";
            // 
            // claimVill
            // 
            this.claimVill.FormattingEnabled = true;
            this.claimVill.Location = new System.Drawing.Point(255, 42);
            this.claimVill.Name = "claimVill";
            this.claimVill.Size = new System.Drawing.Size(130, 21);
            this.claimVill.TabIndex = 116;
            // 
            // QuestsUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.claimVillButton);
            this.Controls.Add(this.claimVillLabel);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.claimVill);
            this.Controls.Add(this.claimBeginner);
            this.Controls.Add(this.claimDaily);
            this.Name = "QuestsUc";
            this.Size = new System.Drawing.Size(513, 294);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox claimDaily;
        private System.Windows.Forms.CheckBox claimBeginner;
        private System.Windows.Forms.Button claimVillButton;
        private System.Windows.Forms.Label claimVillLabel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox claimVill;
    }
}
