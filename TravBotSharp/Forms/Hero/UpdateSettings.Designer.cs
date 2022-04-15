
namespace TravBotSharp.Forms.Hero
{
    partial class UpdateSetings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateSetings));
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.maxInterval = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.minInterval = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.button2, 1, 3);
            this.mainPanel.Controls.Add(this.label13, 0, 0);
            this.mainPanel.Controls.Add(this.button1, 0, 3);
            this.mainPanel.Controls.Add(this.maxInterval, 1, 2);
            this.mainPanel.Controls.Add(this.label14, 0, 2);
            this.mainPanel.Controls.Add(this.minInterval, 1, 1);
            this.mainPanel.Controls.Add(this.label15, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(10, 10);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(10);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 4;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(314, 104);
            this.mainPanel.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(160, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 33);
            this.button2.TabIndex = 24;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.mainPanel.SetColumnSpan(this.label13, 2);
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Location = new System.Drawing.Point(3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(308, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "Update hero page ( in minutes )";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 33);
            this.button1.TabIndex = 23;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // maxInterval
            // 
            this.maxInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maxInterval.Location = new System.Drawing.Point(160, 42);
            this.maxInterval.Maximum = new decimal(new int[] {
            10001,
            0,
            0,
            0});
            this.maxInterval.Name = "maxInterval";
            this.maxInterval.Size = new System.Drawing.Size(151, 20);
            this.maxInterval.TabIndex = 111;
            this.maxInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.maxInterval.ValueChanged += new System.EventHandler(this.maxInterval_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Location = new System.Drawing.Point(3, 39);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(151, 26);
            this.label14.TabIndex = 113;
            this.label14.Text = "Max";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minInterval
            // 
            this.minInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minInterval.Location = new System.Drawing.Point(160, 16);
            this.minInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minInterval.Name = "minInterval";
            this.minInterval.Size = new System.Drawing.Size(151, 20);
            this.minInterval.TabIndex = 110;
            this.minInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.minInterval.ValueChanged += new System.EventHandler(this.minInterval_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Location = new System.Drawing.Point(3, 13);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(151, 26);
            this.label15.TabIndex = 112;
            this.label15.Text = "Min";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateSetings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 124);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateSetings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update hero info delay";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown maxInterval;
        private System.Windows.Forms.NumericUpDown minInterval;
    }
}