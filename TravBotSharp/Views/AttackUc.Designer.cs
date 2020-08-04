namespace TravBotSharp.Views
{
    partial class AttackUc
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
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.Y = new System.Windows.Forms.NumericUpDown();
            this.X = new System.Windows.Forms.NumericUpDown();
            this.confirmNewVill = new System.Windows.Forms.Button();
            this.WavesCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.wavesPerSec = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sendNow = new System.Windows.Forms.CheckBox();
            this.catasPerWave = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.hero = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.currentlyBuildinglistView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label7 = new System.Windows.Forms.Label();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WavesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wavesPerSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.catasPerWave)).BeginInit();
            this.SuspendLayout();
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(158, 24);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(17, 16);
            this.label39.TabIndex = 147;
            this.label39.Text = "Y";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(43, 24);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(16, 16);
            this.label38.TabIndex = 146;
            this.label38.Text = "X";
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(178, 22);
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
            this.Y.Size = new System.Drawing.Size(75, 20);
            this.Y.TabIndex = 144;
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(63, 22);
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
            this.X.Size = new System.Drawing.Size(75, 20);
            this.X.TabIndex = 143;
            // 
            // confirmNewVill
            // 
            this.confirmNewVill.Location = new System.Drawing.Point(34, 240);
            this.confirmNewVill.Name = "confirmNewVill";
            this.confirmNewVill.Size = new System.Drawing.Size(71, 22);
            this.confirmNewVill.TabIndex = 142;
            this.confirmNewVill.Text = "Add real";
            this.confirmNewVill.UseVisualStyleBackColor = true;
            this.confirmNewVill.Click += new System.EventHandler(this.confirmNewVill_Click);
            // 
            // WavesCount
            // 
            this.WavesCount.Location = new System.Drawing.Point(63, 66);
            this.WavesCount.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.WavesCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WavesCount.Name = "WavesCount";
            this.WavesCount.Size = new System.Drawing.Size(75, 20);
            this.WavesCount.TabIndex = 148;
            this.WavesCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 149;
            this.label1.Text = "Waves";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(70, 151);
            this.dateTimePicker1.MaxDate = new System.DateTime(2048, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker1.MinDate = new System.DateTime(2020, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(90, 20);
            this.dateTimePicker1.TabIndex = 150;
            this.dateTimePicker1.Value = new System.DateTime(2020, 5, 10, 0, 0, 0, 0);
            // 
            // wavesPerSec
            // 
            this.wavesPerSec.Location = new System.Drawing.Point(178, 66);
            this.wavesPerSec.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.wavesPerSec.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.wavesPerSec.Name = "wavesPerSec";
            this.wavesPerSec.Size = new System.Drawing.Size(75, 20);
            this.wavesPerSec.TabIndex = 151;
            this.wavesPerSec.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(259, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 152;
            this.label2.Text = "waves / sec";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 153;
            this.label3.Text = "Arrive at";
            // 
            // sendNow
            // 
            this.sendNow.AutoSize = true;
            this.sendNow.Location = new System.Drawing.Point(176, 153);
            this.sendNow.Name = "sendNow";
            this.sendNow.Size = new System.Drawing.Size(74, 17);
            this.sendNow.TabIndex = 154;
            this.sendNow.Text = "Send now";
            this.sendNow.UseVisualStyleBackColor = true;
            this.sendNow.CheckedChanged += new System.EventHandler(this.sendNow_CheckedChanged);
            // 
            // catasPerWave
            // 
            this.catasPerWave.Location = new System.Drawing.Point(63, 103);
            this.catasPerWave.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.catasPerWave.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.catasPerWave.Name = "catasPerWave";
            this.catasPerWave.Size = new System.Drawing.Size(75, 20);
            this.catasPerWave.TabIndex = 155;
            this.catasPerWave.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(146, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 156;
            this.label4.Text = "catapults / wave";
            // 
            // hero
            // 
            this.hero.AutoSize = true;
            this.hero.Location = new System.Drawing.Point(70, 216);
            this.hero.Name = "hero";
            this.hero.Size = new System.Drawing.Size(126, 17);
            this.hero.TabIndex = 157;
            this.hero.Text = "Send hero (1. attack)";
            this.hero.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(475, 41);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(215, 433);
            this.richTextBox1.TabIndex = 158;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(541, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 159;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(149, 239);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 160;
            this.button2.Text = "Add fake";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(44, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 12);
            this.label5.TabIndex = 161;
            this.label5.Text = "(server time, NOT computer time)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(38, 185);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 12);
            this.label6.TabIndex = 162;
            this.label6.Text = "You can\'t set an attack for tomorrow.";
            // 
            // currentlyBuildinglistView
            // 
            this.currentlyBuildinglistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.currentlyBuildinglistView.FullRowSelect = true;
            this.currentlyBuildinglistView.HideSelection = false;
            this.currentlyBuildinglistView.Location = new System.Drawing.Point(34, 300);
            this.currentlyBuildinglistView.MultiSelect = false;
            this.currentlyBuildinglistView.Name = "currentlyBuildinglistView";
            this.currentlyBuildinglistView.Size = new System.Drawing.Size(419, 174);
            this.currentlyBuildinglistView.TabIndex = 163;
            this.currentlyBuildinglistView.UseCompatibleStateImageBehavior = false;
            this.currentlyBuildinglistView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From vill";
            this.columnHeader1.Width = 65;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "To coords";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 64;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ExecuteAt";
            this.columnHeader4.Width = 116;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(31, 281);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 16);
            this.label7.TabIndex = 164;
            this.label7.Text = "Send wave tasks:";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Arrival time";
            this.columnHeader5.Width = 107;
            // 
            // AttackUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.currentlyBuildinglistView);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.hero);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.catasPerWave);
            this.Controls.Add(this.sendNow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wavesPerSec);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WavesCount);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.confirmNewVill);
            this.Name = "AttackUc";
            this.Size = new System.Drawing.Size(724, 491);
            ((System.ComponentModel.ISupportInitialize)(this.Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WavesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wavesPerSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.catasPerWave)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.NumericUpDown Y;
        private System.Windows.Forms.NumericUpDown X;
        private System.Windows.Forms.Button confirmNewVill;
        private System.Windows.Forms.NumericUpDown WavesCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.NumericUpDown wavesPerSec;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox sendNow;
        private System.Windows.Forms.NumericUpDown catasPerWave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox hero;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView currentlyBuildinglistView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}
