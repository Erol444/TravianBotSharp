
namespace TbsWinformNet6.Views
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
            this.troopsSelectorUc1 = new TbsWinformNet6.Forms.TroopsSelectorUc();
            this.button8 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.minPopNatar = new System.Windows.Forms.NumericUpDown();
            this.maxPopNatar = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minPopNatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPopNatar)).BeginInit();
            this.SuspendLayout();
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
            this.button2.Size = new System.Drawing.Size(70, 23);
            this.button2.TabIndex = 156;
            this.button2.Text = "Add";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(254, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 23);
            this.button3.TabIndex = 157;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(254, 86);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(70, 23);
            this.button4.TabIndex = 158;
            this.button4.Text = "Update";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(254, 144);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(70, 23);
            this.button5.TabIndex = 159;
            this.button5.Text = "Clear";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(254, 315);
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
            this.farmingList.Size = new System.Drawing.Size(131, 298);
            this.farmingList.TabIndex = 165;
            this.farmingList.UseCompatibleStateImageBehavior = false;
            this.farmingList.View = System.Windows.Forms.View.Details;
            this.farmingList.SelectedIndexChanged += new System.EventHandler(this.farmingList_SelectedIndexChanged);
            // 
            // troopsSelectorUc1
            // 
            this.troopsSelectorUc1.Hero = false;
            this.troopsSelectorUc1.Location = new System.Drawing.Point(374, 57);
            this.troopsSelectorUc1.Name = "troopsSelectorUc1";
            this.troopsSelectorUc1.Size = new System.Drawing.Size(139, 264);
            this.troopsSelectorUc1.TabIndex = 166;
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
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(38, 89);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(54, 26);
            this.button8.TabIndex = 168;
            this.button8.Text = "Delete";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(368, 386);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(342, 31);
            this.label3.TabIndex = 170;
            this.label3.Text = "USE AT YOUR OWN RISK";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(18, 332);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 174;
            this.button9.Text = "Add to FL";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 315);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 175;
            this.label2.Text = "Add from scouted";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Controls.Add(this.minPopNatar);
            this.panel1.Controls.Add(this.maxPopNatar);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Location = new System.Drawing.Point(545, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(165, 106);
            this.panel1.TabIndex = 176;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(35, 76);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 131;
            this.button10.Text = "Add farms";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // minPopNatar
            // 
            this.minPopNatar.Location = new System.Drawing.Point(65, 25);
            this.minPopNatar.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.minPopNatar.Name = "minPopNatar";
            this.minPopNatar.Size = new System.Drawing.Size(80, 20);
            this.minPopNatar.TabIndex = 129;
            // 
            // maxPopNatar
            // 
            this.maxPopNatar.Location = new System.Drawing.Point(65, 50);
            this.maxPopNatar.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.maxPopNatar.Name = "maxPopNatar";
            this.maxPopNatar.Size = new System.Drawing.Size(80, 20);
            this.maxPopNatar.TabIndex = 130;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 15);
            this.label4.TabIndex = 132;
            this.label4.Text = "Add Natar villages to this FL";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 133;
            this.label6.Text = "Min pop";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 134;
            this.label7.Text = "Max pop";
            // 
            // FarmingNonGoldUc
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.troopsSelectorUc1);
            this.Controls.Add(this.comboBox_NameList);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.farmingList);
            this.Name = "FarmingNonGoldUc";
            this.Size = new System.Drawing.Size(790, 446);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minPopNatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPopNatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private Forms.TroopsSelectorUc troopsSelectorUc1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.NumericUpDown minPopNatar;
        private System.Windows.Forms.NumericUpDown maxPopNatar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}
