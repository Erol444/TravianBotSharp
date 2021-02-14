namespace TravBotSharp.Views
{
    partial class HeroUc
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
            this.buyAdventuresCheckBox = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.minHeroHealthUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoSendToAdventures = new System.Windows.Forms.CheckBox();
            this.strength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.offBonus = new System.Windows.Forms.NumericUpDown();
            this.deffBonus = new System.Windows.Forms.NumericUpDown();
            this.resources = new System.Windows.Forms.NumericUpDown();
            this.autoSetHeroPoints = new System.Windows.Forms.CheckBox();
            this.maxDistanceUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.autoReviveHero = new System.Windows.Forms.CheckBox();
            this.SupplyResourcesButton = new System.Windows.Forms.Button();
            this.SupplyResVillageSelected = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.SupplyResVillageComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.heroItemsList = new System.Windows.Forms.ListView();
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.equiptList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label7 = new System.Windows.Forms.Label();
            this.refreshInfo = new System.Windows.Forms.CheckBox();
            this.autoEquip = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.heroInfo = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.helmetSwitcher = new System.Windows.Forms.CheckBox();
            this.adventures = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.maxInterval = new System.Windows.Forms.NumericUpDown();
            this.minInterval = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resources)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // buyAdventuresCheckBox
            // 
            this.buyAdventuresCheckBox.AutoSize = true;
            this.buyAdventuresCheckBox.Location = new System.Drawing.Point(13, 203);
            this.buyAdventuresCheckBox.Name = "buyAdventuresCheckBox";
            this.buyAdventuresCheckBox.Size = new System.Drawing.Size(141, 17);
            this.buyAdventuresCheckBox.TabIndex = 7;
            this.buyAdventuresCheckBox.Text = "TTWars buy adventures";
            this.buyAdventuresCheckBox.UseVisualStyleBackColor = true;
            this.buyAdventuresCheckBox.CheckedChanged += new System.EventHandler(this.buyAdventuresCheckBox_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 101);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Minimum Health";
            // 
            // minHeroHealthUpDown
            // 
            this.minHeroHealthUpDown.Location = new System.Drawing.Point(13, 117);
            this.minHeroHealthUpDown.Name = "minHeroHealthUpDown";
            this.minHeroHealthUpDown.Size = new System.Drawing.Size(90, 20);
            this.minHeroHealthUpDown.TabIndex = 5;
            this.minHeroHealthUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.minHeroHealthUpDown.ValueChanged += new System.EventHandler(this.minHeroHealthUpDown_ValueChanged);
            // 
            // checkBoxAutoSendToAdventures
            // 
            this.checkBoxAutoSendToAdventures.AutoSize = true;
            this.checkBoxAutoSendToAdventures.Location = new System.Drawing.Point(13, 15);
            this.checkBoxAutoSendToAdventures.Name = "checkBoxAutoSendToAdventures";
            this.checkBoxAutoSendToAdventures.Size = new System.Drawing.Size(142, 17);
            this.checkBoxAutoSendToAdventures.TabIndex = 4;
            this.checkBoxAutoSendToAdventures.Text = "Auto send to adventures";
            this.checkBoxAutoSendToAdventures.UseVisualStyleBackColor = true;
            this.checkBoxAutoSendToAdventures.CheckedChanged += new System.EventHandler(this.checkBoxAutoSendToAdventures_CheckedChanged);
            // 
            // strength
            // 
            this.strength.Location = new System.Drawing.Point(103, 13);
            this.strength.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.strength.Name = "strength";
            this.strength.Size = new System.Drawing.Size(47, 20);
            this.strength.TabIndex = 8;
            this.strength.ValueChanged += new System.EventHandler(this.strength_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Fighting strength";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Off bonus";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Deff bonus";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Resources";
            // 
            // offBonus
            // 
            this.offBonus.Location = new System.Drawing.Point(103, 41);
            this.offBonus.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.offBonus.Name = "offBonus";
            this.offBonus.Size = new System.Drawing.Size(47, 20);
            this.offBonus.TabIndex = 13;
            this.offBonus.ValueChanged += new System.EventHandler(this.offBonus_ValueChanged);
            // 
            // deffBonus
            // 
            this.deffBonus.Location = new System.Drawing.Point(103, 67);
            this.deffBonus.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.deffBonus.Name = "deffBonus";
            this.deffBonus.Size = new System.Drawing.Size(47, 20);
            this.deffBonus.TabIndex = 14;
            this.deffBonus.ValueChanged += new System.EventHandler(this.deffBonus_ValueChanged);
            // 
            // resources
            // 
            this.resources.Location = new System.Drawing.Point(103, 93);
            this.resources.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.resources.Name = "resources";
            this.resources.Size = new System.Drawing.Size(47, 20);
            this.resources.TabIndex = 15;
            this.resources.ValueChanged += new System.EventHandler(this.resources_ValueChanged);
            // 
            // autoSetHeroPoints
            // 
            this.autoSetHeroPoints.AutoSize = true;
            this.autoSetHeroPoints.Location = new System.Drawing.Point(30, 119);
            this.autoSetHeroPoints.Name = "autoSetHeroPoints";
            this.autoSetHeroPoints.Size = new System.Drawing.Size(120, 17);
            this.autoSetHeroPoints.TabIndex = 16;
            this.autoSetHeroPoints.Text = "Auto set hero points";
            this.autoSetHeroPoints.UseVisualStyleBackColor = true;
            this.autoSetHeroPoints.CheckedChanged += new System.EventHandler(this.autoSetHeroPoints_CheckedChanged);
            // 
            // maxDistanceUpDown
            // 
            this.maxDistanceUpDown.Location = new System.Drawing.Point(13, 167);
            this.maxDistanceUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.maxDistanceUpDown.Name = "maxDistanceUpDown";
            this.maxDistanceUpDown.Size = new System.Drawing.Size(90, 20);
            this.maxDistanceUpDown.TabIndex = 17;
            this.maxDistanceUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.maxDistanceUpDown.ValueChanged += new System.EventHandler(this.maxDistanceUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Max distance";
            // 
            // autoReviveHero
            // 
            this.autoReviveHero.AutoSize = true;
            this.autoReviveHero.Location = new System.Drawing.Point(13, 38);
            this.autoReviveHero.Name = "autoReviveHero";
            this.autoReviveHero.Size = new System.Drawing.Size(104, 17);
            this.autoReviveHero.TabIndex = 19;
            this.autoReviveHero.Text = "Auto revive hero";
            this.autoReviveHero.UseVisualStyleBackColor = true;
            this.autoReviveHero.CheckedChanged += new System.EventHandler(this.autoReviveHero_CheckedChanged);
            // 
            // SupplyResourcesButton
            // 
            this.SupplyResourcesButton.Location = new System.Drawing.Point(148, 258);
            this.SupplyResourcesButton.Name = "SupplyResourcesButton";
            this.SupplyResourcesButton.Size = new System.Drawing.Size(71, 23);
            this.SupplyResourcesButton.TabIndex = 119;
            this.SupplyResourcesButton.Text = "OK";
            this.SupplyResourcesButton.UseVisualStyleBackColor = true;
            this.SupplyResourcesButton.Click += new System.EventHandler(this.SupplyResourcesButton_Click);
            // 
            // SupplyResVillageSelected
            // 
            this.SupplyResVillageSelected.AutoSize = true;
            this.SupplyResVillageSelected.Location = new System.Drawing.Point(11, 287);
            this.SupplyResVillageSelected.Name = "SupplyResVillageSelected";
            this.SupplyResVillageSelected.Size = new System.Drawing.Size(55, 13);
            this.SupplyResVillageSelected.TabIndex = 118;
            this.SupplyResVillageSelected.Text = "Selected: ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(11, 240);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(108, 16);
            this.label17.TabIndex = 117;
            this.label17.Text = "Revive hero in";
            // 
            // SupplyResVillageComboBox
            // 
            this.SupplyResVillageComboBox.FormattingEnabled = true;
            this.SupplyResVillageComboBox.Location = new System.Drawing.Point(11, 259);
            this.SupplyResVillageComboBox.Name = "SupplyResVillageComboBox";
            this.SupplyResVillageComboBox.Size = new System.Drawing.Size(130, 21);
            this.SupplyResVillageComboBox.TabIndex = 116;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(487, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 16);
            this.label6.TabIndex = 120;
            this.label6.Text = "Hero items";
            // 
            // heroItemsList
            // 
            this.heroItemsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader18,
            this.columnHeader17,
            this.columnHeader19,
            this.columnHeader1});
            this.heroItemsList.FullRowSelect = true;
            this.heroItemsList.GridLines = true;
            this.heroItemsList.HideSelection = false;
            this.heroItemsList.Location = new System.Drawing.Point(490, 33);
            this.heroItemsList.MultiSelect = false;
            this.heroItemsList.Name = "heroItemsList";
            this.heroItemsList.Size = new System.Drawing.Size(278, 367);
            this.heroItemsList.TabIndex = 143;
            this.heroItemsList.UseCompatibleStateImageBehavior = false;
            this.heroItemsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Category";
            this.columnHeader18.Width = 75;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Item";
            this.columnHeader17.Width = 82;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Tier";
            this.columnHeader19.Width = 39;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Amount";
            this.columnHeader1.Width = 71;
            // 
            // equiptList
            // 
            this.equiptList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.equiptList.FullRowSelect = true;
            this.equiptList.GridLines = true;
            this.equiptList.HideSelection = false;
            this.equiptList.Location = new System.Drawing.Point(490, 424);
            this.equiptList.MultiSelect = false;
            this.equiptList.Name = "equiptList";
            this.equiptList.Size = new System.Drawing.Size(278, 150);
            this.equiptList.TabIndex = 145;
            this.equiptList.UseCompatibleStateImageBehavior = false;
            this.equiptList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Category";
            this.columnHeader2.Width = 87;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Item";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Tier";
            this.columnHeader4.Width = 56;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(487, 405);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 16);
            this.label7.TabIndex = 146;
            this.label7.Text = "Currently equipt";
            // 
            // refreshInfo
            // 
            this.refreshInfo.AutoSize = true;
            this.refreshInfo.Location = new System.Drawing.Point(13, 61);
            this.refreshInfo.Name = "refreshInfo";
            this.refreshInfo.Size = new System.Drawing.Size(127, 17);
            this.refreshInfo.TabIndex = 147;
            this.refreshInfo.Text = "Auto refresh hero info";
            this.refreshInfo.UseVisualStyleBackColor = true;
            this.refreshInfo.CheckedChanged += new System.EventHandler(this.refreshInfo_CheckedChanged);
            // 
            // autoEquip
            // 
            this.autoEquip.AutoSize = true;
            this.autoEquip.Location = new System.Drawing.Point(11, 37);
            this.autoEquip.Name = "autoEquip";
            this.autoEquip.Size = new System.Drawing.Size(101, 17);
            this.autoEquip.TabIndex = 148;
            this.autoEquip.Text = "Auto equip hero";
            this.autoEquip.UseVisualStyleBackColor = true;
            this.autoEquip.CheckedChanged += new System.EventHandler(this.autoEquip_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.autoSetHeroPoints);
            this.panel1.Controls.Add(this.strength);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.offBonus);
            this.panel1.Controls.Add(this.deffBonus);
            this.panel1.Controls.Add(this.resources);
            this.panel1.Location = new System.Drawing.Point(254, 117);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(162, 149);
            this.panel1.TabIndex = 150;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 23);
            this.button1.TabIndex = 151;
            this.button1.Text = "Refresh Hero info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(11, 320);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 16);
            this.label8.TabIndex = 152;
            this.label8.Text = "Hero info:";
            // 
            // heroInfo
            // 
            this.heroInfo.Location = new System.Drawing.Point(13, 336);
            this.heroInfo.Margin = new System.Windows.Forms.Padding(2);
            this.heroInfo.Name = "heroInfo";
            this.heroInfo.ReadOnly = true;
            this.heroInfo.Size = new System.Drawing.Size(216, 177);
            this.heroInfo.TabIndex = 153;
            this.heroInfo.Text = "";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.helmetSwitcher);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.autoEquip);
            this.panel2.Location = new System.Drawing.Point(255, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(163, 86);
            this.panel2.TabIndex = 154;
            // 
            // helmetSwitcher
            // 
            this.helmetSwitcher.AutoSize = true;
            this.helmetSwitcher.Location = new System.Drawing.Point(11, 60);
            this.helmetSwitcher.Name = "helmetSwitcher";
            this.helmetSwitcher.Size = new System.Drawing.Size(120, 17);
            this.helmetSwitcher.TabIndex = 152;
            this.helmetSwitcher.Text = "Auto switch helmets";
            this.helmetSwitcher.UseVisualStyleBackColor = true;
            this.helmetSwitcher.CheckedChanged += new System.EventHandler(this.helmetSwitcher_CheckedChanged);
            // 
            // adventures
            // 
            this.adventures.Location = new System.Drawing.Point(252, 305);
            this.adventures.Margin = new System.Windows.Forms.Padding(2);
            this.adventures.Name = "adventures";
            this.adventures.ReadOnly = true;
            this.adventures.Size = new System.Drawing.Size(186, 133);
            this.adventures.TabIndex = 155;
            this.adventures.Text = "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(250, 287);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 16);
            this.label9.TabIndex = 147;
            this.label9.Text = "Adventures";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(271, 443);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(136, 23);
            this.button2.TabIndex = 156;
            this.button2.Text = "Refresh Adventures";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.label14);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Controls.Add(this.maxInterval);
            this.panel3.Controls.Add(this.minInterval);
            this.panel3.Location = new System.Drawing.Point(252, 484);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(175, 90);
            this.panel3.TabIndex = 157;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(8, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(145, 16);
            this.label10.TabIndex = 116;
            this.label10.Text = "Update hero frequency";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(122, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 13);
            this.label12.TabIndex = 115;
            this.label12.Text = "min";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(122, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 13);
            this.label11.TabIndex = 114;
            this.label11.Text = "min";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(27, 13);
            this.label14.TabIndex = 113;
            this.label14.Text = "Max";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 35);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(24, 13);
            this.label15.TabIndex = 112;
            this.label15.Text = "Min";
            // 
            // maxInterval
            // 
            this.maxInterval.Location = new System.Drawing.Point(38, 59);
            this.maxInterval.Maximum = new decimal(new int[] {
            10001,
            0,
            0,
            0});
            this.maxInterval.Name = "maxInterval";
            this.maxInterval.Size = new System.Drawing.Size(78, 20);
            this.maxInterval.TabIndex = 111;
            this.maxInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.maxInterval.ValueChanged += new System.EventHandler(this.maxInterval_ValueChanged);
            // 
            // minInterval
            // 
            this.minInterval.Location = new System.Drawing.Point(38, 33);
            this.minInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minInterval.Name = "minInterval";
            this.minInterval.Size = new System.Drawing.Size(78, 20);
            this.minInterval.TabIndex = 110;
            this.minInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.minInterval.ValueChanged += new System.EventHandler(this.minInterval_ValueChanged);
            // 
            // HeroUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.adventures);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.heroInfo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.refreshInfo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.equiptList);
            this.Controls.Add(this.heroItemsList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.SupplyResourcesButton);
            this.Controls.Add(this.SupplyResVillageSelected);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.SupplyResVillageComboBox);
            this.Controls.Add(this.autoReviveHero);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.maxDistanceUpDown);
            this.Controls.Add(this.buyAdventuresCheckBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.minHeroHealthUpDown);
            this.Controls.Add(this.checkBoxAutoSendToAdventures);
            this.Name = "HeroUc";
            this.Size = new System.Drawing.Size(843, 598);
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resources)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox buyAdventuresCheckBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown minHeroHealthUpDown;
        private System.Windows.Forms.CheckBox checkBoxAutoSendToAdventures;
        private System.Windows.Forms.NumericUpDown strength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown offBonus;
        private System.Windows.Forms.NumericUpDown deffBonus;
        private System.Windows.Forms.NumericUpDown resources;
        private System.Windows.Forms.CheckBox autoSetHeroPoints;
        private System.Windows.Forms.NumericUpDown maxDistanceUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox autoReviveHero;
        private System.Windows.Forms.Button SupplyResourcesButton;
        private System.Windows.Forms.Label SupplyResVillageSelected;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox SupplyResVillageComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView heroItemsList;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView equiptList;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox refreshInfo;
        private System.Windows.Forms.CheckBox autoEquip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox heroInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox adventures;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox helmetSwitcher;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown maxInterval;
        private System.Windows.Forms.NumericUpDown minInterval;
    }
}
