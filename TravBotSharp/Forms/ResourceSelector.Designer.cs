
namespace TravBotSharp.Forms
{
    partial class ResourceSelector
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
            TbsCore.Models.ResourceModels.Resources resources1 = new TbsCore.Models.ResourceModels.Resources();
            this.resourceSelectorUc1 = new TravBotSharp.UserControls.ResourceSelectorUc();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // resourceSelectorUc1
            // 
            this.resourceSelectorUc1.Location = new System.Drawing.Point(16, 44);
            this.resourceSelectorUc1.Name = "resourceSelectorUc1";
            resources1.Clay = ((long)(0));
            resources1.Crop = ((long)(0));
            resources1.Iron = ((long)(0));
            resources1.Wood = ((long)(0));
            this.resourceSelectorUc1.resources = resources1;
            this.resourceSelectorUc1.Size = new System.Drawing.Size(146, 99);
            this.resourceSelectorUc1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(203, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 99);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "If value is less or equal 100, it will be based on percent";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Otherwise, it will be based on that number";
            // 
            // ResourceSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 155);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.resourceSelectorUc1);
            this.Name = "ResourceSelector";
            this.Text = "ResourceSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UserControls.ResourceSelectorUc resourceSelectorUc1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}