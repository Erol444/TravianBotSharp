﻿
namespace TravBotSharp.Forms
{
    partial class TroopsSelector
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
            this.troopsSelectorUc1 = new TravBotSharp.Forms.TroopsSelectorUc();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // troopsSelectorUc1
            // 
            this.troopsSelectorUc1.Hero = false;
            this.troopsSelectorUc1.Location = new System.Drawing.Point(12, 12);
            this.troopsSelectorUc1.Name = "troopsSelectorUc1";
            this.troopsSelectorUc1.Size = new System.Drawing.Size(139, 264);
            this.troopsSelectorUc1.TabIndex = 0;
            this.troopsSelectorUc1.Troops = new int[] {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0};
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(197, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TroopsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 287);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.troopsSelectorUc1);
            this.Name = "TroopsSelector";
            this.Text = "TroopsSelector";
            this.ResumeLayout(false);

        }

        #endregion

        private TroopsSelectorUc troopsSelectorUc1;
        private System.Windows.Forms.Button button1;
    }
}