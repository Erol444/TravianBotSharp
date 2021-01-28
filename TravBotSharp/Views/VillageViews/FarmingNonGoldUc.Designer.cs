
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
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_TroopToFarm = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.Amount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.comboBox_NameList = new System.Windows.Forms.ComboBox();
            this.button8 = new System.Windows.Forms.Button();
            this.farmingIdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingXHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingYHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingTroopHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingAmountHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.farmingList = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Amount)).BeginInit();
            this.SuspendLayout();
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(56, 74);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(58, 20);
            this.X.TabIndex = 0;
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(176, 74);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(58, 20);
            this.Y.TabIndex = 1;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(34, 74);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(16, 16);
            this.label38.TabIndex = 148;
            this.label38.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(154, 74);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(34, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 153;
            this.label3.Text = "Troop";
            // 
            // comboBox_TroopToFarm
            // 
            this.comboBox_TroopToFarm.FormattingEnabled = true;
            this.comboBox_TroopToFarm.Location = new System.Drawing.Point(103, 107);
            this.comboBox_TroopToFarm.Name = "comboBox_TroopToFarm";
            this.comboBox_TroopToFarm.Size = new System.Drawing.Size(131, 21);
            this.comboBox_TroopToFarm.TabIndex = 154;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(37, 177);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 23);
            this.button1.TabIndex = 155;
            this.button1.Text = "New";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(108, 177);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 23);
            this.button2.TabIndex = 156;
            this.button2.Text = "Add";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(109, 206);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(56, 23);
            this.button3.TabIndex = 157;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(178, 177);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 23);
            this.button4.TabIndex = 158;
            this.button4.Text = "Update";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(178, 206);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(56, 23);
            this.button5.TabIndex = 159;
            this.button5.Text = "Clear";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // Amount
            // 
            this.Amount.Location = new System.Drawing.Point(103, 138);
            this.Amount.Name = "Amount";
            this.Amount.Size = new System.Drawing.Size(58, 20);
            this.Amount.TabIndex = 160;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(34, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 16);
            this.label4.TabIndex = 161;
            this.label4.Text = "Amount";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(37, 241);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(88, 23);
            this.button6.TabIndex = 162;
            this.button6.Text = "Attack one";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(146, 241);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(88, 23);
            this.button7.TabIndex = 163;
            this.button7.Text = "Attack list";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // comboBox_NameList
            // 
            this.comboBox_NameList.FormattingEnabled = true;
            this.comboBox_NameList.Location = new System.Drawing.Point(103, 30);
            this.comboBox_NameList.Name = "comboBox_NameList";
            this.comboBox_NameList.Size = new System.Drawing.Size(131, 21);
            this.comboBox_NameList.TabIndex = 164;
            this.comboBox_NameList.SelectedIndexChanged += new System.EventHandler(this.comboBox_NameList_SelectedIndexChanged);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(37, 206);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(58, 23);
            this.button8.TabIndex = 166;
            this.button8.Text = "Save";
            this.button8.UseVisualStyleBackColor = true;
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
            this.farmingXHeader.Width = 30;
            // 
            // farmingYHeader
            // 
            this.farmingYHeader.Text = "Y";
            this.farmingYHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.farmingYHeader.Width = 30;
            // 
            // farmingTroopHeader
            // 
            this.farmingTroopHeader.Text = "Troop";
            this.farmingTroopHeader.Width = 100;
            // 
            // farmingAmountHeader
            // 
            this.farmingAmountHeader.Text = "Amount";
            this.farmingAmountHeader.Width = 50;
            // 
            // farmingList
            // 
            this.farmingList.BackColor = System.Drawing.SystemColors.MenuText;
            this.farmingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.farmingIdHeader,
            this.farmingXHeader,
            this.farmingYHeader,
            this.farmingTroopHeader,
            this.farmingAmountHeader});
            this.farmingList.FullRowSelect = true;
            this.farmingList.GridLines = true;
            this.farmingList.HideSelection = false;
            this.farmingList.Location = new System.Drawing.Point(263, 30);
            this.farmingList.MultiSelect = false;
            this.farmingList.Name = "farmingList";
            this.farmingList.Size = new System.Drawing.Size(253, 385);
            this.farmingList.TabIndex = 165;
            this.farmingList.UseCompatibleStateImageBehavior = false;
            this.farmingList.View = System.Windows.Forms.View.Details;
            // 
            // FarmingNonGoldUc
            // 
            this.Controls.Add(this.button8);
            this.Controls.Add(this.farmingList);
            this.Controls.Add(this.comboBox_NameList);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
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
            this.Name = "FarmingNonGoldUc";
            this.Size = new System.Drawing.Size(702, 446);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_TroopToFarm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown Amount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBox_NameList;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ColumnHeader farmingIdHeader;
        private System.Windows.Forms.ColumnHeader farmingXHeader;
        private System.Windows.Forms.ColumnHeader farmingYHeader;
        private System.Windows.Forms.ColumnHeader farmingTroopHeader;
        private System.Windows.Forms.ColumnHeader farmingAmountHeader;
        private System.Windows.Forms.ListView farmingList;
    }
}
