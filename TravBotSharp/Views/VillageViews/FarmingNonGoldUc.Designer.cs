
namespace TravBotSharp.Views
{
    partial class FarmingNonGoldUc
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
            this.X = new System.Windows.Forms.NumericUpDown();
            this.Y = new System.Windows.Forms.NumericUpDown();
            this.label38 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.comboBox_NameList = new System.Windows.Forms.ComboBox();
            this.farmingIdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingXHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingYHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingList = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_TroopToFarm = new System.Windows.Forms.ComboBox();
            this.Amount = new System.Windows.Forms.NumericUpDown();
            this.troopList = new System.Windows.Forms.ListView();
            this.farmingTroopId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingTroopType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingTroopAmount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Amount)).BeginInit();
            this.SuspendLayout();
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(358, 57);
            this.X.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.X.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            -2147483648});
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(58, 20);
            this.X.TabIndex = 0;
            this.X.ValueChanged += new System.EventHandler(this.X_ValueChanged);
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(478, 57);
            this.Y.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.Y.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            -2147483648});
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(58, 20);
            this.Y.TabIndex = 1;
            this.Y.ThousandsSeparator = true;
            this.Y.ValueChanged += new System.EventHandler(this.Y_ValueChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(336, 57);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(16, 16);
            this.label38.TabIndex = 148;
            this.label38.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(456, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 16);
            this.label2.TabIndex = 149;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 151;
            this.label1.Text = "Name list";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(37, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 23);
            this.button1.TabIndex = 155;
            this.button1.Text = "New";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(254, 57);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 23);
            this.button2.TabIndex = 156;
            this.button2.Text = "Add";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(254, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(56, 23);
            this.button3.TabIndex = 157;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(254, 86);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 23);
            this.button4.TabIndex = 158;
            this.button4.Text = "Update";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(254, 144);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(56, 23);
            this.button5.TabIndex = 159;
            this.button5.Text = "Clear";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(254, 386);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(70, 23);
            this.button7.TabIndex = 163;
            this.button7.Text = "ATTACK";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // comboBox_NameList
            // 
            this.comboBox_NameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_NameList.FormattingEnabled = true;
            this.comboBox_NameList.Location = new System.Drawing.Point(103, 30);
            this.comboBox_NameList.Name = "comboBox_NameList";
            this.comboBox_NameList.Size = new System.Drawing.Size(131, 21);
            this.comboBox_NameList.TabIndex = 164;
            this.comboBox_NameList.SelectedIndexChanged += new System.EventHandler(this.comboBox_NameList_SelectedIndexChanged);
            // 
            // farmingIdHeader
            // 
            this.farmingIdHeader.Text = "Id";
            this.farmingIdHeader.Width = 25;
            // 
            // farmingXHeader
            // 
            this.farmingXHeader.Text = "X";
            this.farmingXHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.farmingXHeader.Width = 50;
            // 
            // farmingYHeader
            // 
            this.farmingYHeader.Text = "Y";
            this.farmingYHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.farmingYHeader.Width = 51;
            // 
            // farmingList
            // 
            this.farmingList.BackColor = System.Drawing.SystemColors.MenuText;
            this.farmingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.farmingIdHeader,
            this.farmingXHeader,
            this.farmingYHeader});
            this.farmingList.FullRowSelect = true;
            this.farmingList.GridLines = true;
            this.farmingList.HideSelection = false;
            this.farmingList.Location = new System.Drawing.Point(103, 57);
            this.farmingList.MultiSelect = false;
            this.farmingList.Name = "farmingList";
            this.farmingList.Size = new System.Drawing.Size(131, 385);
            this.farmingList.TabIndex = 165;
            this.farmingList.UseCompatibleStateImageBehavior = false;
            this.farmingList.View = System.Windows.Forms.View.Details;
            this.farmingList.SelectedIndexChanged += new System.EventHandler(this.farmingList_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(346, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 16);
            this.label4.TabIndex = 161;
            this.label4.Text = "Amount";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(354, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 153;
            this.label3.Text = "Troop";
            // 
            // comboBox_TroopToFarm
            // 
            this.comboBox_TroopToFarm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TroopToFarm.FormattingEnabled = true;
            this.comboBox_TroopToFarm.Location = new System.Drawing.Point(405, 90);
            this.comboBox_TroopToFarm.Name = "comboBox_TroopToFarm";
            this.comboBox_TroopToFarm.Size = new System.Drawing.Size(131, 21);
            this.comboBox_TroopToFarm.TabIndex = 154;
            this.comboBox_TroopToFarm.SelectedIndexChanged += new System.EventHandler(this.comboBox_TroopToFarm_SelectedIndexChanged);
            // 
            // Amount
            // 
            this.Amount.Location = new System.Drawing.Point(405, 117);
            this.Amount.Name = "Amount";
            this.Amount.Size = new System.Drawing.Size(58, 20);
            this.Amount.TabIndex = 160;
            this.Amount.ValueChanged += new System.EventHandler(this.Amount_ValueChanged);
            // 
            // troopList
            // 
            this.troopList.BackColor = System.Drawing.SystemColors.MenuText;
            this.troopList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.farmingTroopId,
            this.farmingTroopType,
            this.farmingTroopAmount});
            this.troopList.FullRowSelect = true;
            this.troopList.GridLines = true;
            this.troopList.HideSelection = false;
            this.troopList.Location = new System.Drawing.Point(542, 57);
            this.troopList.MultiSelect = false;
            this.troopList.Name = "troopList";
            this.troopList.Size = new System.Drawing.Size(198, 130);
            this.troopList.TabIndex = 167;
            this.troopList.UseCompatibleStateImageBehavior = false;
            this.troopList.View = System.Windows.Forms.View.Details;
            // 
            // farmingTroopId
            // 
            this.farmingTroopId.Text = "Id";
            this.farmingTroopId.Width = 23;
            // 
            // farmingTroopType
            // 
            this.farmingTroopType.Text = "Type";
            this.farmingTroopType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.farmingTroopType.Width = 117;
            // 
            // farmingTroopAmount
            // 
            this.farmingTroopAmount.Text = "Amount";
            this.farmingTroopAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.farmingTroopAmount.Width = 53;
            // 
            // FarmingNonGoldUc
            // 
            this.Controls.Add(this.troopList);
            this.Controls.Add(this.comboBox_NameList);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Amount);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox_TroopToFarm);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.farmingList);
            this.Name = "FarmingNonGoldUc";
            this.Size = new System.Drawing.Size(790, 446);
            ((System.ComponentModel.ISupportInitialize)(this.X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Amount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown X;
        private System.Windows.Forms.NumericUpDown Y;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBox_NameList;
        private System.Windows.Forms.ColumnHeader farmingIdHeader;
        private System.Windows.Forms.ColumnHeader farmingXHeader;
        private System.Windows.Forms.ColumnHeader farmingYHeader;
        private System.Windows.Forms.ListView farmingList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_TroopToFarm;
        private System.Windows.Forms.NumericUpDown Amount;
        private System.Windows.Forms.ListView troopList;
        private System.Windows.Forms.ColumnHeader farmingTroopAmount;
        private System.Windows.Forms.ColumnHeader farmingTroopId;
        private System.Windows.Forms.ColumnHeader farmingTroopType;
    }
}
