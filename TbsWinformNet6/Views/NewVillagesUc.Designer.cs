namespace TbsWinformNet6.Views
{
    partial class NewVillagesUc
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
            this.removeNewVill = new System.Windows.Forms.Button();
            this.NewVillList = new System.Windows.Forms.ListView();
            this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.NewVillName = new System.Windows.Forms.TextBox();
            this.YNewVill = new System.Windows.Forms.NumericUpDown();
            this.XNewVill = new System.Windows.Forms.NumericUpDown();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.confirmNewVill = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.autoNewVillagesToSettle = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.villTypeView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Button25 = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.BuildTasksLocationTextBox = new System.Windows.Forms.TextBox();
            this.valleyType = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.villName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.YNewVill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XNewVill)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // removeNewVill
            // 
            this.removeNewVill.Location = new System.Drawing.Point(178, 330);
            this.removeNewVill.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.removeNewVill.Name = "removeNewVill";
            this.removeNewVill.Size = new System.Drawing.Size(68, 32);
            this.removeNewVill.TabIndex = 143;
            this.removeNewVill.Text = "Remove";
            this.removeNewVill.UseVisualStyleBackColor = true;
            this.removeNewVill.Click += new System.EventHandler(this.removeNewVill_Click);
            // 
            // NewVillList
            // 
            this.NewVillList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19});
            this.NewVillList.FullRowSelect = true;
            this.NewVillList.GridLines = true;
            this.NewVillList.Location = new System.Drawing.Point(40, 37);
            this.NewVillList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NewVillList.MultiSelect = false;
            this.NewVillList.Name = "NewVillList";
            this.NewVillList.Size = new System.Drawing.Size(206, 289);
            this.NewVillList.TabIndex = 142;
            this.NewVillList.UseCompatibleStateImageBehavior = false;
            this.NewVillList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "X";
            this.columnHeader17.Width = 49;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Y";
            this.columnHeader18.Width = 48;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Name";
            this.columnHeader19.Width = 75;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(10, 388);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(39, 15);
            this.label40.TabIndex = 141;
            this.label40.Text = "Name";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(35, 360);
            this.label39.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(14, 15);
            this.label39.TabIndex = 140;
            this.label39.Text = "Y";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(35, 333);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(14, 15);
            this.label38.TabIndex = 139;
            this.label38.Text = "X";
            // 
            // NewVillName
            // 
            this.NewVillName.Location = new System.Drawing.Point(58, 384);
            this.NewVillName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NewVillName.Name = "NewVillName";
            this.NewVillName.Size = new System.Drawing.Size(111, 23);
            this.NewVillName.TabIndex = 138;
            // 
            // YNewVill
            // 
            this.YNewVill.Location = new System.Drawing.Point(58, 358);
            this.YNewVill.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.YNewVill.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.YNewVill.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            -2147483648});
            this.YNewVill.Name = "YNewVill";
            this.YNewVill.Size = new System.Drawing.Size(88, 23);
            this.YNewVill.TabIndex = 137;
            // 
            // XNewVill
            // 
            this.XNewVill.Location = new System.Drawing.Point(58, 331);
            this.XNewVill.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.XNewVill.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.XNewVill.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            -2147483648});
            this.XNewVill.Name = "XNewVill";
            this.XNewVill.Size = new System.Drawing.Size(88, 23);
            this.XNewVill.TabIndex = 136;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.checkBox3.Location = new System.Drawing.Point(30, 69);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(189, 20);
            this.checkBox3.TabIndex = 135;
            this.checkBox3.Text = "Auto make new villages";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // confirmNewVill
            // 
            this.confirmNewVill.Location = new System.Drawing.Point(58, 414);
            this.confirmNewVill.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.confirmNewVill.Name = "confirmNewVill";
            this.confirmNewVill.Size = new System.Drawing.Size(83, 25);
            this.confirmNewVill.TabIndex = 134;
            this.confirmNewVill.Text = "OK";
            this.confirmNewVill.UseVisualStyleBackColor = true;
            this.confirmNewVill.Click += new System.EventHandler(this.confirmNewVill_Click);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label36.Location = new System.Drawing.Point(74, 13);
            this.label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(112, 18);
            this.label36.TabIndex = 133;
            this.label36.Text = "New villages list";
            // 
            // autoNewVillagesToSettle
            // 
            this.autoNewVillagesToSettle.AutoSize = true;
            this.autoNewVillagesToSettle.Location = new System.Drawing.Point(16, 69);
            this.autoNewVillagesToSettle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoNewVillagesToSettle.Name = "autoNewVillagesToSettle";
            this.autoNewVillagesToSettle.Size = new System.Drawing.Size(188, 19);
            this.autoNewVillagesToSettle.TabIndex = 144;
            this.autoNewVillagesToSettle.Text = "Auto find new villages to settle";
            this.autoNewVillagesToSettle.UseVisualStyleBackColor = true;
            this.autoNewVillagesToSettle.CheckedChanged += new System.EventHandler(this.autoNewVillagesToSettle_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 45);
            this.label1.TabIndex = 145;
            this.label1.Text = "When you run out of specified \r\nvillages to settle, find new ones \r\naround your m" +
    "ain village:";
            // 
            // villTypeView
            // 
            this.villTypeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.villTypeView.FullRowSelect = true;
            this.villTypeView.GridLines = true;
            this.villTypeView.Location = new System.Drawing.Point(16, 160);
            this.villTypeView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.villTypeView.MultiSelect = false;
            this.villTypeView.Name = "villTypeView";
            this.villTypeView.Size = new System.Drawing.Size(154, 261);
            this.villTypeView.TabIndex = 146;
            this.villTypeView.UseCompatibleStateImageBehavior = false;
            this.villTypeView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 124;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(13, 136);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 16);
            this.label2.TabIndex = 147;
            this.label2.Text = "Village types to settle (eg. 4446)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(161, 104);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 27);
            this.button1.TabIndex = 148;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(181, 242);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(68, 51);
            this.button2.TabIndex = 149;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(19, 426);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 13);
            this.label3.TabIndex = 150;
            this.label3.Text = "Leave empty to settle any village";
            // 
            // Button25
            // 
            this.Button25.Location = new System.Drawing.Point(15, 22);
            this.Button25.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Button25.Name = "Button25";
            this.Button25.Size = new System.Drawing.Size(52, 25);
            this.Button25.TabIndex = 153;
            this.Button25.Text = "Select";
            this.Button25.UseVisualStyleBackColor = true;
            this.Button25.Click += new System.EventHandler(this.Button25_Click);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(12, 3);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(165, 15);
            this.label34.TabIndex = 152;
            this.label34.Text = "Building tasks for new villages";
            // 
            // BuildTasksLocationTextBox
            // 
            this.BuildTasksLocationTextBox.Location = new System.Drawing.Point(75, 22);
            this.BuildTasksLocationTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BuildTasksLocationTextBox.Name = "BuildTasksLocationTextBox";
            this.BuildTasksLocationTextBox.ReadOnly = true;
            this.BuildTasksLocationTextBox.Size = new System.Drawing.Size(318, 23);
            this.BuildTasksLocationTextBox.TabIndex = 151;
            // 
            // valleyType
            // 
            this.valleyType.FormattingEnabled = true;
            this.valleyType.Items.AddRange(new object[] {
            "9c",
            "3456",
            "4446",
            "4536",
            "5346",
            "15c",
            "4437",
            "3447",
            "4347",
            "3547",
            "4356",
            "5436"});
            this.valleyType.Location = new System.Drawing.Point(16, 106);
            this.valleyType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.valleyType.Name = "valleyType";
            this.valleyType.Size = new System.Drawing.Size(139, 23);
            this.valleyType.TabIndex = 154;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(177, 367);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(68, 32);
            this.button3.TabIndex = 155;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // villName
            // 
            this.villName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.villName.Location = new System.Drawing.Point(36, 36);
            this.villName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.villName.Name = "villName";
            this.villName.Size = new System.Drawing.Size(194, 26);
            this.villName.TabIndex = 156;
            this.villName.TextChanged += new System.EventHandler(this.villName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(33, 14);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 16);
            this.label4.TabIndex = 157;
            this.label4.Text = "New village name template";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(34, 68);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 12);
            this.label5.TabIndex = 158;
            this.label5.Text = "#NUM# gets replaced with village count";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.autoNewVillagesToSettle);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.villTypeView);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.valleyType);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(23, 121);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 453);
            this.panel1.TabIndex = 159;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.villName);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(23, 16);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(260, 98);
            this.panel2.TabIndex = 160;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.confirmNewVill);
            this.panel3.Controls.Add(this.label36);
            this.panel3.Controls.Add(this.XNewVill);
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.YNewVill);
            this.panel3.Controls.Add(this.NewVillName);
            this.panel3.Controls.Add(this.label38);
            this.panel3.Controls.Add(this.label39);
            this.panel3.Controls.Add(this.removeNewVill);
            this.panel3.Controls.Add(this.label40);
            this.panel3.Controls.Add(this.NewVillList);
            this.panel3.Location = new System.Drawing.Point(296, 16);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(262, 498);
            this.panel3.TabIndex = 161;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(4, 468);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(255, 27);
            this.button4.TabIndex = 163;
            this.button4.Text = "Find new village to settle";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel4.Controls.Add(this.checkBox3);
            this.panel4.Controls.Add(this.BuildTasksLocationTextBox);
            this.panel4.Controls.Add(this.label34);
            this.panel4.Controls.Add(this.Button25);
            this.panel4.Location = new System.Drawing.Point(566, 16);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(421, 98);
            this.panel4.TabIndex = 162;
            // 
            // NewVillagesUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "NewVillagesUc";
            this.Size = new System.Drawing.Size(1065, 587);
            ((System.ComponentModel.ISupportInitialize)(this.YNewVill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XNewVill)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeNewVill;
        private System.Windows.Forms.ListView NewVillList;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox NewVillName;
        private System.Windows.Forms.NumericUpDown YNewVill;
        private System.Windows.Forms.NumericUpDown XNewVill;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Button confirmNewVill;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.CheckBox autoNewVillagesToSettle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView villTypeView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Button25;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox BuildTasksLocationTextBox;
        private System.Windows.Forms.ComboBox valleyType;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox villName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button4;
    }
}
