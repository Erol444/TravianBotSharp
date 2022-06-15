namespace TbsWinformNet6.Views
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
            this.checkBoxAutoSendToAdventures = new System.Windows.Forms.CheckBox();
            this.autoSetHeroPoints = new System.Windows.Forms.CheckBox();
            this.autoReviveHero = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.heroItemsList = new System.Windows.Forms.ListView();
            this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.equiptList = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.label7 = new System.Windows.Forms.Label();
            this.refreshInfo = new System.Windows.Forms.CheckBox();
            this.autoEquip = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.heroInfo = new System.Windows.Forms.RichTextBox();
            this.helmetSwitcher = new System.Windows.Forms.CheckBox();
            this.adventures = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.centerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.leftPanel = new System.Windows.Forms.TableLayoutPanel();
            this.button5 = new System.Windows.Forms.Button();
            this.autoAuction = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.centerPanel.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buyAdventuresCheckBox
            // 
            this.buyAdventuresCheckBox.AutoSize = true;
            this.leftPanel.SetColumnSpan(this.buyAdventuresCheckBox, 2);
            this.buyAdventuresCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buyAdventuresCheckBox.Location = new System.Drawing.Point(4, 577);
            this.buyAdventuresCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buyAdventuresCheckBox.Name = "buyAdventuresCheckBox";
            this.buyAdventuresCheckBox.Size = new System.Drawing.Size(299, 80);
            this.buyAdventuresCheckBox.TabIndex = 7;
            this.buyAdventuresCheckBox.Text = "Auto buy adventures (TTWars only)";
            this.buyAdventuresCheckBox.UseVisualStyleBackColor = true;
            this.buyAdventuresCheckBox.CheckedChanged += new System.EventHandler(this.buyAdventuresCheckBox_CheckedChanged);
            // 
            // checkBoxAutoSendToAdventures
            // 
            this.checkBoxAutoSendToAdventures.AutoSize = true;
            this.checkBoxAutoSendToAdventures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxAutoSendToAdventures.Location = new System.Drawing.Point(4, 3);
            this.checkBoxAutoSendToAdventures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxAutoSendToAdventures.Name = "checkBoxAutoSendToAdventures";
            this.checkBoxAutoSendToAdventures.Size = new System.Drawing.Size(203, 76);
            this.checkBoxAutoSendToAdventures.TabIndex = 4;
            this.checkBoxAutoSendToAdventures.Text = "Auto send to adventures";
            this.checkBoxAutoSendToAdventures.UseVisualStyleBackColor = true;
            this.checkBoxAutoSendToAdventures.CheckedChanged += new System.EventHandler(this.checkBoxAutoSendToAdventures_CheckedChanged);
            // 
            // autoSetHeroPoints
            // 
            this.autoSetHeroPoints.AutoSize = true;
            this.autoSetHeroPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoSetHeroPoints.Location = new System.Drawing.Point(4, 495);
            this.autoSetHeroPoints.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoSetHeroPoints.Name = "autoSetHeroPoints";
            this.autoSetHeroPoints.Size = new System.Drawing.Size(203, 76);
            this.autoSetHeroPoints.TabIndex = 16;
            this.autoSetHeroPoints.Text = "Auto set hero points";
            this.autoSetHeroPoints.UseVisualStyleBackColor = true;
            this.autoSetHeroPoints.CheckedChanged += new System.EventHandler(this.autoSetHeroPoints_CheckedChanged);
            // 
            // autoReviveHero
            // 
            this.autoReviveHero.AutoSize = true;
            this.autoReviveHero.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoReviveHero.Location = new System.Drawing.Point(4, 85);
            this.autoReviveHero.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoReviveHero.Name = "autoReviveHero";
            this.autoReviveHero.Size = new System.Drawing.Size(203, 76);
            this.autoReviveHero.TabIndex = 19;
            this.autoReviveHero.Text = "Auto revive hero";
            this.autoReviveHero.UseVisualStyleBackColor = true;
            this.autoReviveHero.CheckedChanged += new System.EventHandler(this.autoReviveHero_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(4, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(510, 16);
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
            this.heroItemsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroItemsList.FullRowSelect = true;
            this.heroItemsList.GridLines = true;
            this.heroItemsList.Location = new System.Drawing.Point(4, 19);
            this.heroItemsList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.heroItemsList.MultiSelect = false;
            this.heroItemsList.Name = "heroItemsList";
            this.heroItemsList.Size = new System.Drawing.Size(510, 308);
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
            this.equiptList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.equiptList.FullRowSelect = true;
            this.equiptList.GridLines = true;
            this.equiptList.Location = new System.Drawing.Point(4, 349);
            this.equiptList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.equiptList.MultiSelect = false;
            this.equiptList.Name = "equiptList";
            this.equiptList.Size = new System.Drawing.Size(510, 308);
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
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(4, 330);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(510, 16);
            this.label7.TabIndex = 146;
            this.label7.Text = "Currently equipt";
            // 
            // refreshInfo
            // 
            this.refreshInfo.AutoSize = true;
            this.refreshInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshInfo.Location = new System.Drawing.Point(4, 167);
            this.refreshInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.refreshInfo.Name = "refreshInfo";
            this.refreshInfo.Size = new System.Drawing.Size(203, 76);
            this.refreshInfo.TabIndex = 147;
            this.refreshInfo.Text = "Auto refresh hero info";
            this.refreshInfo.UseVisualStyleBackColor = true;
            this.refreshInfo.CheckedChanged += new System.EventHandler(this.refreshInfo_CheckedChanged);
            // 
            // autoEquip
            // 
            this.autoEquip.AutoSize = true;
            this.leftPanel.SetColumnSpan(this.autoEquip, 2);
            this.autoEquip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoEquip.Location = new System.Drawing.Point(4, 249);
            this.autoEquip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoEquip.Name = "autoEquip";
            this.autoEquip.Size = new System.Drawing.Size(299, 76);
            this.autoEquip.TabIndex = 148;
            this.autoEquip.Text = "Auto equip item";
            this.autoEquip.UseVisualStyleBackColor = true;
            this.autoEquip.CheckedChanged += new System.EventHandler(this.autoEquip_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(4, 333);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(510, 27);
            this.button1.TabIndex = 151;
            this.button1.Text = "Refresh Hero info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(4, 363);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(510, 16);
            this.label8.TabIndex = 152;
            this.label8.Text = "Hero info:";
            // 
            // heroInfo
            // 
            this.heroInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroInfo.Location = new System.Drawing.Point(2, 381);
            this.heroInfo.Margin = new System.Windows.Forms.Padding(2);
            this.heroInfo.Name = "heroInfo";
            this.heroInfo.ReadOnly = true;
            this.heroInfo.Size = new System.Drawing.Size(514, 277);
            this.heroInfo.TabIndex = 153;
            this.heroInfo.Text = "";
            // 
            // helmetSwitcher
            // 
            this.helmetSwitcher.AutoSize = true;
            this.leftPanel.SetColumnSpan(this.helmetSwitcher, 2);
            this.helmetSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helmetSwitcher.Location = new System.Drawing.Point(4, 413);
            this.helmetSwitcher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.helmetSwitcher.Name = "helmetSwitcher";
            this.helmetSwitcher.Size = new System.Drawing.Size(299, 76);
            this.helmetSwitcher.TabIndex = 152;
            this.helmetSwitcher.Text = "Auto switch helmets (TTWars only)";
            this.helmetSwitcher.UseVisualStyleBackColor = true;
            this.helmetSwitcher.CheckedChanged += new System.EventHandler(this.helmetSwitcher_CheckedChanged);
            // 
            // adventures
            // 
            this.adventures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adventures.Location = new System.Drawing.Point(2, 51);
            this.adventures.Margin = new System.Windows.Forms.Padding(2);
            this.adventures.Name = "adventures";
            this.adventures.ReadOnly = true;
            this.adventures.Size = new System.Drawing.Size(514, 277);
            this.adventures.TabIndex = 155;
            this.adventures.Text = "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(4, 33);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(510, 16);
            this.label9.TabIndex = 147;
            this.label9.Text = "Adventures";
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(4, 3);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(510, 27);
            this.button2.TabIndex = 156;
            this.button2.Text = "Refresh Adventures";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 3;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.rightPanel, 2, 0);
            this.mainPanel.Controls.Add(this.centerPanel, 1, 0);
            this.mainPanel.Controls.Add(this.leftPanel, 0, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.mainPanel.RowCount = 1;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(1391, 690);
            this.mainPanel.TabIndex = 158;
            // 
            // rightPanel
            // 
            this.rightPanel.ColumnCount = 1;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.Controls.Add(this.label6, 0, 0);
            this.rightPanel.Controls.Add(this.heroItemsList, 0, 1);
            this.rightPanel.Controls.Add(this.label7, 0, 2);
            this.rightPanel.Controls.Add(this.equiptList, 0, 3);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(857, 15);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.RowCount = 4;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.Size = new System.Drawing.Size(518, 660);
            this.rightPanel.TabIndex = 0;
            // 
            // centerPanel
            // 
            this.centerPanel.ColumnCount = 1;
            this.centerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.centerPanel.Controls.Add(this.heroInfo, 0, 5);
            this.centerPanel.Controls.Add(this.button1, 0, 3);
            this.centerPanel.Controls.Add(this.label8, 0, 4);
            this.centerPanel.Controls.Add(this.button2, 0, 0);
            this.centerPanel.Controls.Add(this.adventures, 0, 2);
            this.centerPanel.Controls.Add(this.label9, 0, 1);
            this.centerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.centerPanel.Location = new System.Drawing.Point(331, 15);
            this.centerPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.RowCount = 6;
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.centerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.centerPanel.Size = new System.Drawing.Size(518, 660);
            this.centerPanel.TabIndex = 1;
            // 
            // leftPanel
            // 
            this.leftPanel.ColumnCount = 2;
            this.leftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.leftPanel.Controls.Add(this.button5, 1, 2);
            this.leftPanel.Controls.Add(this.checkBoxAutoSendToAdventures, 0, 0);
            this.leftPanel.Controls.Add(this.autoEquip, 0, 3);
            this.leftPanel.Controls.Add(this.autoReviveHero, 0, 1);
            this.leftPanel.Controls.Add(this.refreshInfo, 0, 2);
            this.leftPanel.Controls.Add(this.buyAdventuresCheckBox, 0, 7);
            this.leftPanel.Controls.Add(this.autoSetHeroPoints, 0, 6);
            this.leftPanel.Controls.Add(this.helmetSwitcher, 0, 5);
            this.leftPanel.Controls.Add(this.autoAuction, 0, 4);
            this.leftPanel.Controls.Add(this.button3, 1, 0);
            this.leftPanel.Controls.Add(this.button4, 1, 1);
            this.leftPanel.Controls.Add(this.button7, 1, 6);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(16, 15);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.RowCount = 8;
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.leftPanel.Size = new System.Drawing.Size(307, 660);
            this.leftPanel.TabIndex = 2;
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button5.Location = new System.Drawing.Point(215, 167);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(88, 76);
            this.button5.TabIndex = 156;
            this.button5.Text = "Setting";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // autoAuction
            // 
            this.autoAuction.AutoSize = true;
            this.leftPanel.SetColumnSpan(this.autoAuction, 2);
            this.autoAuction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoAuction.Enabled = false;
            this.autoAuction.Location = new System.Drawing.Point(4, 331);
            this.autoAuction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoAuction.Name = "autoAuction";
            this.autoAuction.Size = new System.Drawing.Size(299, 76);
            this.autoAuction.TabIndex = 153;
            this.autoAuction.Text = "Auto auction (Offcial only)";
            this.autoAuction.UseVisualStyleBackColor = true;
            this.autoAuction.CheckedChanged += new System.EventHandler(this.autoAuction_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.Location = new System.Drawing.Point(215, 3);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 76);
            this.button3.TabIndex = 154;
            this.button3.Text = "Setting";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button4.Location = new System.Drawing.Point(215, 85);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(88, 76);
            this.button4.TabIndex = 155;
            this.button4.Text = "Setting";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button7.Location = new System.Drawing.Point(215, 495);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(88, 76);
            this.button7.TabIndex = 158;
            this.button7.Text = "Setting";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // HeroUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "HeroUc";
            this.Size = new System.Drawing.Size(1391, 690);
            this.mainPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.centerPanel.ResumeLayout(false);
            this.centerPanel.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox buyAdventuresCheckBox;
        private System.Windows.Forms.CheckBox checkBoxAutoSendToAdventures;
        private System.Windows.Forms.CheckBox autoSetHeroPoints;
        private System.Windows.Forms.CheckBox autoReviveHero;
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox heroInfo;
        private System.Windows.Forms.RichTextBox adventures;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox helmetSwitcher;
        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private System.Windows.Forms.TableLayoutPanel centerPanel;
        private System.Windows.Forms.TableLayoutPanel leftPanel;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox autoAuction;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
    }
}
