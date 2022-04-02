
namespace TravBotSharp.Forms.Hero
{
    partial class AdventuresSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdventuresSettings));
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.minHeroHealthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.maxDistanceUpDown = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.button2, 1, 2);
            this.mainPanel.Controls.Add(this.minHeroHealthUpDown, 1, 0);
            this.mainPanel.Controls.Add(this.label5, 0, 1);
            this.mainPanel.Controls.Add(this.label13, 0, 0);
            this.mainPanel.Controls.Add(this.maxDistanceUpDown, 1, 1);
            this.mainPanel.Controls.Add(this.button1, 0, 2);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(10, 10);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(10);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainPanel.Size = new System.Drawing.Size(285, 103);
            this.mainPanel.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(145, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 45);
            this.button2.TabIndex = 24;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // minHeroHealthUpDown
            // 
            this.minHeroHealthUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minHeroHealthUpDown.Location = new System.Drawing.Point(145, 3);
            this.minHeroHealthUpDown.Name = "minHeroHealthUpDown";
            this.minHeroHealthUpDown.Size = new System.Drawing.Size(137, 20);
            this.minHeroHealthUpDown.TabIndex = 19;
            this.minHeroHealthUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 26);
            this.label5.TabIndex = 22;
            this.label5.Text = "Max distance";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Location = new System.Drawing.Point(3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(136, 26);
            this.label13.TabIndex = 20;
            this.label13.Text = "Minimum health";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maxDistanceUpDown
            // 
            this.maxDistanceUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maxDistanceUpDown.Location = new System.Drawing.Point(145, 29);
            this.maxDistanceUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.maxDistanceUpDown.Name = "maxDistanceUpDown";
            this.maxDistanceUpDown.Size = new System.Drawing.Size(137, 20);
            this.maxDistanceUpDown.TabIndex = 21;
            this.maxDistanceUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 45);
            this.button1.TabIndex = 23;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AdventuresSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 123);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdventuresSettings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Adventure condition";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.NumericUpDown minHeroHealthUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown maxDistanceUpDown;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}