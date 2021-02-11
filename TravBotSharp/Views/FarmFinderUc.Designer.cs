
namespace TravBotSharp
{
    partial class FarmFinderUc
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxVillages = new System.Windows.Forms.ComboBox();
            this.X = new System.Windows.Forms.NumericUpDown();
            this.Y = new System.Windows.Forms.NumericUpDown();
            this.Distance = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.InactiveList = new System.Windows.Forms.ListView();
            lvwColumnSorter = new ListViewColumnSorter();
            this.InactiveList.ListViewItemSorter = lvwColumnSorter;
            this.InactiveId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveDis = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveCoord = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactivePlayer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveAlly = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveVill = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactivePop = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(261, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Suggest farm";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(113, 248);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxVillages
            // 
            this.comboBoxVillages.FormattingEnabled = true;
            this.comboBoxVillages.Location = new System.Drawing.Point(113, 75);
            this.comboBoxVillages.Name = "comboBoxVillages";
            this.comboBoxVillages.Size = new System.Drawing.Size(121, 21);
            this.comboBoxVillages.TabIndex = 2;
            this.comboBoxVillages.SelectedIndexChanged += new System.EventHandler(this.comboBoxVillages_SelectedIndexChanged);
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(113, 118);
            this.X.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.X.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(50, 20);
            this.X.TabIndex = 3;
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(113, 155);
            this.Y.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Y.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(50, 20);
            this.Y.TabIndex = 4;
            // 
            // Distance
            // 
            this.Distance.Location = new System.Drawing.Point(113, 196);
            this.Distance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Distance.Name = "Distance";
            this.Distance.Size = new System.Drawing.Size(50, 20);
            this.Distance.TabIndex = 5;
            this.Distance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(169, 118);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 98);
            this.button2.TabIndex = 6;
            this.button2.Text = "SERVER CODE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // InactiveList
            // 
            this.InactiveList.BackColor = System.Drawing.SystemColors.MenuText;
            this.InactiveList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.InactiveId,
            this.InactiveDis,
            this.InactiveCoord,
            this.InactivePlayer,
            this.InactiveAlly,
            this.InactiveVill,
            this.InactivePop});
            this.InactiveList.FullRowSelect = true;
            this.InactiveList.GridLines = true;
            this.InactiveList.HideSelection = false;
            this.InactiveList.Location = new System.Drawing.Point(310, 75);
            this.InactiveList.MultiSelect = false;
            this.InactiveList.Name = "InactiveList";
            this.InactiveList.Size = new System.Drawing.Size(440, 385);
            this.InactiveList.TabIndex = 166;
            this.InactiveList.UseCompatibleStateImageBehavior = false;
            this.InactiveList.View = System.Windows.Forms.View.Details;
            this.InactiveList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.InactiveList_ColumnClick);
            // 
            // InactiveId
            // 
            this.InactiveId.Text = "Id";
            this.InactiveId.Width = 27;
            // 
            // InactiveDis
            // 
            this.InactiveDis.Text = "Distance";
            this.InactiveDis.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveDis.Width = 68;
            // 
            // InactiveCoord
            // 
            this.InactiveCoord.Text = "Coordinates";
            this.InactiveCoord.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveCoord.Width = 76;
            // 
            // InactivePlayer
            // 
            this.InactivePlayer.Text = "Player";
            this.InactivePlayer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // InactiveAlly
            // 
            this.InactiveAlly.Text = "Alliance";
            this.InactiveAlly.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // InactiveVill
            // 
            this.InactiveVill.Text = "Village";
            this.InactiveVill.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // InactivePop
            // 
            this.InactivePop.Text = "Population";
            this.InactivePop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactivePop.Width = 71;
            // 
            // FarmFinderUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.InactiveList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Distance);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.comboBoxVillages);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "FarmFinderUc";
            this.Size = new System.Drawing.Size(766, 504);
            ((System.ComponentModel.ISupportInitialize)(this.X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxVillages;
        private System.Windows.Forms.NumericUpDown X;
        private System.Windows.Forms.NumericUpDown Y;
        private System.Windows.Forms.NumericUpDown Distance;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView InactiveList;
        private System.Windows.Forms.ColumnHeader InactiveId;
        private System.Windows.Forms.ColumnHeader InactiveDis;
        private System.Windows.Forms.ColumnHeader InactiveCoord;
        private System.Windows.Forms.ColumnHeader InactivePlayer;
        private System.Windows.Forms.ColumnHeader InactiveAlly;
        private System.Windows.Forms.ColumnHeader InactiveVill;
        private System.Windows.Forms.ColumnHeader InactivePop;

    }
}
