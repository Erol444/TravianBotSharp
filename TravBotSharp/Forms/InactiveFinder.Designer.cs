
namespace TravBotSharp.Forms
{
    partial class InactiveFinder
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
            TbsCore.Models.MapModels.Coordinates coordinates1 = new TbsCore.Models.MapModels.Coordinates();
            this.countFarmChose = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.InactivePop = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveVill = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveAlly = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactivePlayer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveCoord = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveDis = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InactiveId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.InactiveList = new System.Windows.Forms.ListView();
            this.Distance = new System.Windows.Forms.NumericUpDown();
            this.comboBoxVillages = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.flName = new System.Windows.Forms.Label();
            this.coordinatesUc1 = new TravBotSharp.UserControls.CoordinatesUc();
            this.troopsSelectorUc1 = new TravBotSharp.Forms.TroopsSelectorUc();
            this.tool = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).BeginInit();
            this.SuspendLayout();
            // 
            // countFarmChose
            // 
            this.countFarmChose.AutoSize = true;
            this.countFarmChose.Location = new System.Drawing.Point(851, 65);
            this.countFarmChose.Name = "countFarmChose";
            this.countFarmChose.Size = new System.Drawing.Size(13, 13);
            this.countFarmChose.TabIndex = 183;
            this.countFarmChose.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 180;
            this.label2.Text = "Distance:";
            // 
            // InactivePop
            // 
            this.InactivePop.Text = "Population";
            this.InactivePop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactivePop.Width = 71;
            // 
            // InactiveVill
            // 
            this.InactiveVill.Text = "Village";
            this.InactiveVill.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveVill.Width = 98;
            // 
            // InactiveAlly
            // 
            this.InactiveAlly.Text = "Alliance";
            this.InactiveAlly.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveAlly.Width = 77;
            // 
            // InactivePlayer
            // 
            this.InactivePlayer.Text = "Player";
            this.InactivePlayer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactivePlayer.Width = 90;
            // 
            // InactiveCoord
            // 
            this.InactiveCoord.Text = "Coordinates";
            this.InactiveCoord.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveCoord.Width = 76;
            // 
            // InactiveDis
            // 
            this.InactiveDis.Text = "Distance";
            this.InactiveDis.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InactiveDis.Width = 68;
            // 
            // InactiveId
            // 
            this.InactiveId.Text = "Id";
            this.InactiveId.Width = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(760, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 182;
            this.label3.Text = "Farms chosen:";
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
            this.InactiveList.Location = new System.Drawing.Point(215, 57);
            this.InactiveList.Name = "InactiveList";
            this.InactiveList.Size = new System.Drawing.Size(531, 352);
            this.InactiveList.TabIndex = 178;
            this.InactiveList.UseCompatibleStateImageBehavior = false;
            this.InactiveList.View = System.Windows.Forms.View.Details;
            this.InactiveList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.InactiveList_ColumnClick);
            this.InactiveList.SelectedIndexChanged += new System.EventHandler(this.InactiveList_SelectedIndexChanged);
            // 
            // Distance
            // 
            this.Distance.Location = new System.Drawing.Point(78, 111);
            this.Distance.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.Distance.Name = "Distance";
            this.Distance.Size = new System.Drawing.Size(120, 20);
            this.Distance.TabIndex = 176;
            this.Distance.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // comboBoxVillages
            // 
            this.comboBoxVillages.FormattingEnabled = true;
            this.comboBoxVillages.Location = new System.Drawing.Point(78, 84);
            this.comboBoxVillages.Name = "comboBoxVillages";
            this.comboBoxVillages.Size = new System.Drawing.Size(120, 21);
            this.comboBoxVillages.TabIndex = 175;
            this.comboBoxVillages.SelectedIndexChanged += new System.EventHandler(this.comboBoxVillages_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(20, 199);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 33);
            this.button2.TabIndex = 174;
            this.button2.Text = "Search";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 173;
            this.label1.Text = "Village:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(790, 365);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(111, 43);
            this.button3.TabIndex = 184;
            this.button3.Text = "Add";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // flName
            // 
            this.flName.AutoSize = true;
            this.flName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flName.Location = new System.Drawing.Point(27, 9);
            this.flName.Name = "flName";
            this.flName.Size = new System.Drawing.Size(0, 31);
            this.flName.TabIndex = 185;
            // 
            // coordinatesUc1
            // 
            this.coordinatesUc1.BackColor = System.Drawing.SystemColors.ControlDark;
            coordinates1.x = 0;
            coordinates1.y = 0;
            this.coordinatesUc1.Coords = coordinates1;
            this.coordinatesUc1.Location = new System.Drawing.Point(78, 137);
            this.coordinatesUc1.Name = "coordinatesUc1";
            this.coordinatesUc1.Size = new System.Drawing.Size(109, 56);
            this.coordinatesUc1.TabIndex = 179;
            // 
            // troopsSelectorUc1
            // 
            this.troopsSelectorUc1.Hero = false;
            this.troopsSelectorUc1.Location = new System.Drawing.Point(763, 96);
            this.troopsSelectorUc1.Name = "troopsSelectorUc1";
            this.troopsSelectorUc1.Size = new System.Drawing.Size(139, 264);
            this.troopsSelectorUc1.TabIndex = 181;
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
            // tool
            // 
            this.tool.FormattingEnabled = true;
            this.tool.Items.AddRange(new object[] {
            "travianstats.de",
            "inactivesearch.it"});
            this.tool.Location = new System.Drawing.Point(78, 57);
            this.tool.Name = "tool";
            this.tool.Size = new System.Drawing.Size(120, 21);
            this.tool.TabIndex = 186;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 187;
            this.label4.Text = "Tool:";
            // 
            // InactiveFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tool);
            this.Controls.Add(this.flName);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.countFarmChose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.coordinatesUc1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.troopsSelectorUc1);
            this.Controls.Add(this.InactiveList);
            this.Controls.Add(this.Distance);
            this.Controls.Add(this.comboBoxVillages);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Name = "InactiveFinder";
            this.Text = "FarmFinder";
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label countFarmChose;
        private System.Windows.Forms.Label label2;
        private UserControls.CoordinatesUc coordinatesUc1;
        private System.Windows.Forms.ColumnHeader InactivePop;
        private System.Windows.Forms.ColumnHeader InactiveVill;
        private System.Windows.Forms.ColumnHeader InactiveAlly;
        private System.Windows.Forms.ColumnHeader InactivePlayer;
        private System.Windows.Forms.ColumnHeader InactiveCoord;
        private System.Windows.Forms.ColumnHeader InactiveDis;
        private System.Windows.Forms.ColumnHeader InactiveId;
        private System.Windows.Forms.Label label3;
        private TroopsSelectorUc troopsSelectorUc1;
        private System.Windows.Forms.ListView InactiveList;
        private System.Windows.Forms.NumericUpDown Distance;
        private System.Windows.Forms.ComboBox comboBoxVillages;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label flName;
        private System.Windows.Forms.ComboBox tool;
        private System.Windows.Forms.Label label4;
    }
}