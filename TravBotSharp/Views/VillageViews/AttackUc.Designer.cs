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
            TbsCore.Models.MapModels.Coordinates coordinates3 = new TbsCore.Models.MapModels.Coordinates();
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
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.coordinatesUc1 = new TravBotSharp.UserControls.CoordinatesUc();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.oasisMinTroops = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.oasisStrategy = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.oasisPower = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.oasisDistance = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.oasisDelay = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.oasisEnabled = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.playerId = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.WavesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wavesPerSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.catasPerWave)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.oasisMinTroops)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisDelay)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerId)).BeginInit();
            this.SuspendLayout();
            // 
            // confirmNewVill
            // 
            this.confirmNewVill.Location = new System.Drawing.Point(46, 214);
            this.confirmNewVill.Name = "confirmNewVill";
            this.confirmNewVill.Size = new System.Drawing.Size(71, 22);
            this.confirmNewVill.TabIndex = 142;
            this.confirmNewVill.Text = "Add real";
            this.confirmNewVill.UseVisualStyleBackColor = true;
            this.confirmNewVill.Click += new System.EventHandler(this.confirmNewVill_Click);
            // 
            // WavesCount
            // 
            this.WavesCount.Location = new System.Drawing.Point(39, 36);
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
            this.label1.Location = new System.Drawing.Point(116, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 149;
            this.label1.Text = "Waves";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(34, 23);
            this.dateTimePicker1.MaxDate = new System.DateTime(2048, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker1.MinDate = new System.DateTime(2020, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(108, 20);
            this.dateTimePicker1.TabIndex = 150;
            this.dateTimePicker1.Value = new System.DateTime(2020, 5, 10, 0, 0, 0, 0);
            // 
            // wavesPerSec
            // 
            this.wavesPerSec.Location = new System.Drawing.Point(39, 63);
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
            this.label2.Location = new System.Drawing.Point(116, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 152;
            this.label2.Text = "waves / sec";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 16);
            this.label3.TabIndex = 153;
            this.label3.Text = "Arrive at";
            // 
            // sendNow
            // 
            this.sendNow.AutoSize = true;
            this.sendNow.Location = new System.Drawing.Point(80, 5);
            this.sendNow.Name = "sendNow";
            this.sendNow.Size = new System.Drawing.Size(74, 17);
            this.sendNow.TabIndex = 154;
            this.sendNow.Text = "Send now";
            this.sendNow.UseVisualStyleBackColor = true;
            this.sendNow.CheckedChanged += new System.EventHandler(this.sendNow_CheckedChanged);
            // 
            // catasPerWave
            // 
            this.catasPerWave.Location = new System.Drawing.Point(39, 87);
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
            this.label4.Location = new System.Drawing.Point(116, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 156;
            this.label4.Text = "catapults / wave";
            // 
            // hero
            // 
            this.hero.AutoSize = true;
            this.hero.Location = new System.Drawing.Point(82, 192);
            this.hero.Name = "hero";
            this.hero.Size = new System.Drawing.Size(126, 17);
            this.hero.TabIndex = 157;
            this.hero.Text = "Send hero (1. attack)";
            this.hero.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(288, 37);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(208, 223);
            this.richTextBox1.TabIndex = 158;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(343, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 159;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(131, 214);
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
            this.label5.Location = new System.Drawing.Point(5, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 12);
            this.label5.TabIndex = 161;
            this.label5.Text = "(server time, NOT computer time)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(4, 56);
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
            this.currentlyBuildinglistView.Location = new System.Drawing.Point(3, 260);
            this.currentlyBuildinglistView.MultiSelect = false;
            this.currentlyBuildinglistView.Name = "currentlyBuildinglistView";
            this.currentlyBuildinglistView.Size = new System.Drawing.Size(493, 168);
            this.currentlyBuildinglistView.TabIndex = 163;
            this.currentlyBuildinglistView.UseCompatibleStateImageBehavior = false;
            this.currentlyBuildinglistView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From vill";
            this.columnHeader1.Width = 87;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "To coords";
            this.columnHeader2.Width = 84;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 71;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ExecuteAt";
            this.columnHeader4.Width = 155;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Arrival time";
            this.columnHeader5.Width = 147;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(5, 241);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(149, 20);
            this.label7.TabIndex = 164;
            this.label7.Text = "Send wave tasks:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Controls.Add(this.coordinatesUc1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.currentlyBuildinglistView);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.WavesCount);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.hero);
            this.panel1.Controls.Add(this.wavesPerSec);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.catasPerWave);
            this.panel1.Controls.Add(this.confirmNewVill);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(498, 433);
            this.panel1.TabIndex = 167;
            // 
            // coordinatesUc1
            // 
            this.coordinatesUc1.BackColor = System.Drawing.SystemColors.ControlDark;
            coordinates3.x = 0;
            coordinates3.y = 0;
            this.coordinatesUc1.Coords = coordinates3;
            this.coordinatesUc1.Location = new System.Drawing.Point(8, 121);
            this.coordinatesUc1.Name = "coordinatesUc1";
            this.coordinatesUc1.Size = new System.Drawing.Size(113, 56);
            this.coordinatesUc1.TabIndex = 171;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.dateTimePicker1);
            this.panel3.Controls.Add(this.sendNow);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(131, 111);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(156, 74);
            this.panel3.TabIndex = 170;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(128, 10);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(133, 24);
            this.label16.TabIndex = 169;
            this.label16.Text = "Wave builder";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.oasisMinTroops);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.oasisStrategy);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.oasisPower);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.oasisDistance);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.oasisDelay);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.oasisEnabled);
            this.panel2.Location = new System.Drawing.Point(505, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(226, 260);
            this.panel2.TabIndex = 168;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(89, 231);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(126, 16);
            this.label15.TabIndex = 180;
            this.label15.Text = "Min. troops to attack";
            // 
            // oasisMinTroops
            // 
            this.oasisMinTroops.Location = new System.Drawing.Point(10, 228);
            this.oasisMinTroops.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.oasisMinTroops.Name = "oasisMinTroops";
            this.oasisMinTroops.Size = new System.Drawing.Size(75, 20);
            this.oasisMinTroops.TabIndex = 179;
            this.oasisMinTroops.ValueChanged += new System.EventHandler(this.oasisMinTroops_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(10, 212);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(118, 13);
            this.label14.TabIndex = 178;
            this.label14.Text = "-1 will ignore deff power";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(63, 65);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(118, 18);
            this.label13.TabIndex = 177;
            this.label13.Text = "Farming strategy";
            // 
            // oasisStrategy
            // 
            this.oasisStrategy.FormattingEnabled = true;
            this.oasisStrategy.Items.AddRange(new object[] {
            "Nearest first",
            "Least deff first",
            "Maximum res first"});
            this.oasisStrategy.Location = new System.Drawing.Point(44, 84);
            this.oasisStrategy.Name = "oasisStrategy";
            this.oasisStrategy.Size = new System.Drawing.Size(132, 21);
            this.oasisStrategy.TabIndex = 176;
            this.oasisStrategy.SelectedIndexChanged += new System.EventHandler(this.oasisStrategy_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(86, 197);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(134, 16);
            this.label12.TabIndex = 175;
            this.label12.Text = "oasis max deff power";
            // 
            // oasisPower
            // 
            this.oasisPower.Location = new System.Drawing.Point(10, 195);
            this.oasisPower.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.oasisPower.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.oasisPower.Name = "oasisPower";
            this.oasisPower.Size = new System.Drawing.Size(75, 20);
            this.oasisPower.TabIndex = 174;
            this.oasisPower.ValueChanged += new System.EventHandler(this.oasisPower_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(86, 169);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 16);
            this.label11.TabIndex = 173;
            this.label11.Text = "oasis max distance";
            // 
            // oasisDistance
            // 
            this.oasisDistance.Location = new System.Drawing.Point(10, 167);
            this.oasisDistance.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.oasisDistance.Name = "oasisDistance";
            this.oasisDistance.Size = new System.Drawing.Size(75, 20);
            this.oasisDistance.TabIndex = 172;
            this.oasisDistance.ValueChanged += new System.EventHandler(this.oasisDistance_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(193, 16);
            this.label10.TabIndex = 171;
            this.label10.Text = "attacking the same oasis again";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(89, 124);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(134, 16);
            this.label9.TabIndex = 170;
            this.label9.Text = "hours of delay before";
            // 
            // oasisDelay
            // 
            this.oasisDelay.Location = new System.Drawing.Point(10, 121);
            this.oasisDelay.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.oasisDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.oasisDelay.Name = "oasisDelay";
            this.oasisDelay.Size = new System.Drawing.Size(75, 20);
            this.oasisDelay.TabIndex = 169;
            this.oasisDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.oasisDelay.ValueChanged += new System.EventHandler(this.oasisDelay_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(49, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(137, 24);
            this.label8.TabIndex = 168;
            this.label8.Text = "Oasis farming";
            // 
            // oasisEnabled
            // 
            this.oasisEnabled.AutoSize = true;
            this.oasisEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.oasisEnabled.Location = new System.Drawing.Point(30, 37);
            this.oasisEnabled.Name = "oasisEnabled";
            this.oasisEnabled.Size = new System.Drawing.Size(186, 24);
            this.oasisEnabled.TabIndex = 167;
            this.oasisEnabled.Text = "Oasis farming enabled";
            this.oasisEnabled.UseVisualStyleBackColor = true;
            this.oasisEnabled.CheckedChanged += new System.EventHandler(this.oasisEnabled_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel4.Controls.Add(this.button3);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.playerId);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Location = new System.Drawing.Point(504, 266);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(226, 118);
            this.panel4.TabIndex = 169;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(160, 41);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 23);
            this.button3.TabIndex = 171;
            this.button3.Text = "OK";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(86, 44);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(63, 16);
            this.label23.TabIndex = 170;
            this.label23.Text = "Player ID";
            // 
            // playerId
            // 
            this.playerId.Location = new System.Drawing.Point(10, 42);
            this.playerId.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.playerId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.playerId.Name = "playerId";
            this.playerId.Size = new System.Drawing.Size(75, 20);
            this.playerId.TabIndex = 169;
            this.playerId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(49, 10);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(126, 24);
            this.label24.TabIndex = 168;
            this.label24.Text = "Scout player";
            // 
            // AttackUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "AttackUc";
            this.Size = new System.Drawing.Size(734, 557);
            ((System.ComponentModel.ISupportInitialize)(this.WavesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wavesPerSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.catasPerWave)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.oasisMinTroops)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oasisDelay)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerId)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown oasisDelay;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox oasisEnabled;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown oasisPower;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown oasisDistance;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox oasisStrategy;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown oasisMinTroops;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel3;
        private UserControls.CoordinatesUc coordinatesUc1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown playerId;
        private System.Windows.Forms.Label label24;
    }
}
