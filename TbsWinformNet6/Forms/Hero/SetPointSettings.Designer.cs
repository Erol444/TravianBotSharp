
namespace TbsWinformNet6.Forms.Hero
{
    partial class SetPointSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPointSettings));
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.resourcesNumberBox = new System.Windows.Forms.NumericUpDown();
            this.deffBonusNumberBox = new System.Windows.Forms.NumericUpDown();
            this.offBonusNumberBox = new System.Windows.Forms.NumericUpDown();
            this.strengthNumberBox = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resourcesNumberBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonusNumberBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonusNumberBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthNumberBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.resourcesNumberBox, 1, 3);
            this.mainPanel.Controls.Add(this.deffBonusNumberBox, 1, 2);
            this.mainPanel.Controls.Add(this.offBonusNumberBox, 1, 1);
            this.mainPanel.Controls.Add(this.strengthNumberBox, 1, 0);
            this.mainPanel.Controls.Add(this.button2, 1, 4);
            this.mainPanel.Controls.Add(this.label4, 0, 3);
            this.mainPanel.Controls.Add(this.label3, 0, 2);
            this.mainPanel.Controls.Add(this.label2, 0, 1);
            this.mainPanel.Controls.Add(this.label1, 0, 0);
            this.mainPanel.Controls.Add(this.button1, 0, 4);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(10, 10);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(10);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 5;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(251, 140);
            this.mainPanel.TabIndex = 0;
            // 
            // resourcesNumberBox
            // 
            this.resourcesNumberBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourcesNumberBox.Location = new System.Drawing.Point(128, 81);
            this.resourcesNumberBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.resourcesNumberBox.Name = "resourcesNumberBox";
            this.resourcesNumberBox.Size = new System.Drawing.Size(120, 20);
            this.resourcesNumberBox.TabIndex = 15;
            this.resourcesNumberBox.ValueChanged += new System.EventHandler(this.resources_ValueChanged);
            // 
            // deffBonusNumberBox
            // 
            this.deffBonusNumberBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deffBonusNumberBox.Location = new System.Drawing.Point(128, 55);
            this.deffBonusNumberBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.deffBonusNumberBox.Name = "deffBonusNumberBox";
            this.deffBonusNumberBox.Size = new System.Drawing.Size(120, 20);
            this.deffBonusNumberBox.TabIndex = 14;
            this.deffBonusNumberBox.ValueChanged += new System.EventHandler(this.deffBonus_ValueChanged);
            // 
            // offBonusNumberBox
            // 
            this.offBonusNumberBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.offBonusNumberBox.Location = new System.Drawing.Point(128, 29);
            this.offBonusNumberBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.offBonusNumberBox.Name = "offBonusNumberBox";
            this.offBonusNumberBox.Size = new System.Drawing.Size(120, 20);
            this.offBonusNumberBox.TabIndex = 13;
            this.offBonusNumberBox.ValueChanged += new System.EventHandler(this.offBonus_ValueChanged);
            // 
            // strengthNumberBox
            // 
            this.strengthNumberBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.strengthNumberBox.Location = new System.Drawing.Point(128, 3);
            this.strengthNumberBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.strengthNumberBox.Name = "strengthNumberBox";
            this.strengthNumberBox.Size = new System.Drawing.Size(120, 20);
            this.strengthNumberBox.TabIndex = 8;
            this.strengthNumberBox.ValueChanged += new System.EventHandler(this.strength_ValueChanged);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(128, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 30);
            this.button2.TabIndex = 24;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 26);
            this.label4.TabIndex = 12;
            this.label4.Text = "Resources";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 26);
            this.label3.TabIndex = 11;
            this.label3.Text = "Deff bonus";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 26);
            this.label2.TabIndex = 10;
            this.label2.Text = "Off bonus";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 26);
            this.label1.TabIndex = 9;
            this.label1.Text = "Fighting strength";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(3, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 30);
            this.button1.TabIndex = 23;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SetPointSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 160);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetPointSettings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set point hero (total 4 point)";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resourcesNumberBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonusNumberBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonusNumberBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthNumberBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown resourcesNumberBox;
        private System.Windows.Forms.NumericUpDown deffBonusNumberBox;
        private System.Windows.Forms.NumericUpDown offBonusNumberBox;
        private System.Windows.Forms.NumericUpDown strengthNumberBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}