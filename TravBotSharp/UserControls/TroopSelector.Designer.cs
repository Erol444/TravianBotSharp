
namespace TravBotSharp.UserControls
{
    partial class TroopSelector
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
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBox
            // 
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(6, 6);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(133, 21);
            this.comboBox.TabIndex = 26;
            // 
            // TroopSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.comboBox);
            this.Name = "TroopSelector";
            this.Size = new System.Drawing.Size(145, 33);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox;
    }
}
