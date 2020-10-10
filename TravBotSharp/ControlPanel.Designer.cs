namespace TravBotSharp
{
    partial class ControlPanel
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
            this.button1 = new System.Windows.Forms.Button();
            this.accTabController = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.generalUc1 = new TravBotSharp.Views.GeneralUc();
            this.tabHero = new System.Windows.Forms.TabPage();
            this.heroUc1 = new TravBotSharp.Views.HeroUc();
            this.tabVillages = new System.Windows.Forms.TabPage();
            this.RefreshAllVills = new System.Windows.Forms.Button();
            this.RefreshVill = new System.Windows.Forms.Button();
            this.VillagesListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.villageTabController = new System.Windows.Forms.TabControl();
            this.villTabBuild = new System.Windows.Forms.TabPage();
            this.buildUc1 = new TravBotSharp.Views.BuildUc();
            this.villTabMarket = new System.Windows.Forms.TabPage();
            this.marketUc1 = new TravBotSharp.Views.MarketUc();
            this.villTabTroops = new System.Windows.Forms.TabPage();
            this.troopsUc1 = new TravBotSharp.Views.TroopsUc();
            this.villTabAttack = new System.Windows.Forms.TabPage();
            this.attackUc1 = new TravBotSharp.Views.AttackUc();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.overviewUc1 = new TravBotSharp.Views.OverviewUc();
            this.FarmingTab = new System.Windows.Forms.TabPage();
            this.farmingUc1 = new TravBotSharp.Views.FarmingUc();
            this.newVillagesTab = new System.Windows.Forms.TabPage();
            this.newVillagesUc1 = new TravBotSharp.Views.NewVillagesUc();
            this.deffendingTab = new System.Windows.Forms.TabPage();
            this.deffendingUc1 = new TravBotSharp.Views.DeffendingUc();
            this.questsTab = new System.Windows.Forms.TabPage();
            this.questsUc1 = new TravBotSharp.Views.QuestsUc();
            this.debugTab = new System.Windows.Forms.TabPage();
            this.debugUc1 = new TravBotSharp.Views.DebugUc();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.accListView = new System.Windows.Forms.ListView();
            this.accAccountHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accProxyOkHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accProxyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button7 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.accTabController.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabHero.SuspendLayout();
            this.tabVillages.SuspendLayout();
            this.villageTabController.SuspendLayout();
            this.villTabBuild.SuspendLayout();
            this.villTabMarket.SuspendLayout();
            this.villTabTroops.SuspendLayout();
            this.villTabAttack.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.FarmingTab.SuspendLayout();
            this.newVillagesTab.SuspendLayout();
            this.deffendingTab.SuspendLayout();
            this.questsTab.SuspendLayout();
            this.debugTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 16);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(237, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add account";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // accTabController
            // 
            this.accTabController.Controls.Add(this.tabGeneral);
            this.accTabController.Controls.Add(this.tabHero);
            this.accTabController.Controls.Add(this.tabVillages);
            this.accTabController.Controls.Add(this.tabOverview);
            this.accTabController.Controls.Add(this.FarmingTab);
            this.accTabController.Controls.Add(this.newVillagesTab);
            this.accTabController.Controls.Add(this.deffendingTab);
            this.accTabController.Controls.Add(this.questsTab);
            this.accTabController.Controls.Add(this.debugTab);
            this.accTabController.Dock = System.Windows.Forms.DockStyle.Right;
            this.accTabController.Location = new System.Drawing.Point(290, 0);
            this.accTabController.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.accTabController.Name = "accTabController";
            this.accTabController.SelectedIndex = 0;
            this.accTabController.Size = new System.Drawing.Size(1271, 814);
            this.accTabController.TabIndex = 3;
            this.accTabController.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.generalUc1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabGeneral.Size = new System.Drawing.Size(1263, 785);
            this.tabGeneral.TabIndex = 3;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // generalUc1
            // 
            this.generalUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalUc1.Location = new System.Drawing.Point(4, 4);
            this.generalUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.generalUc1.Name = "generalUc1";
            this.generalUc1.Size = new System.Drawing.Size(1255, 777);
            this.generalUc1.TabIndex = 0;
            // 
            // tabHero
            // 
            this.tabHero.Controls.Add(this.heroUc1);
            this.tabHero.Location = new System.Drawing.Point(4, 25);
            this.tabHero.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabHero.Name = "tabHero";
            this.tabHero.Size = new System.Drawing.Size(1263, 785);
            this.tabHero.TabIndex = 2;
            this.tabHero.Text = "Hero";
            this.tabHero.UseVisualStyleBackColor = true;
            // 
            // heroUc1
            // 
            this.heroUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroUc1.Location = new System.Drawing.Point(0, 0);
            this.heroUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.heroUc1.Name = "heroUc1";
            this.heroUc1.Size = new System.Drawing.Size(1263, 785);
            this.heroUc1.TabIndex = 0;
            // 
            // tabVillages
            // 
            this.tabVillages.Controls.Add(this.RefreshAllVills);
            this.tabVillages.Controls.Add(this.RefreshVill);
            this.tabVillages.Controls.Add(this.VillagesListView);
            this.tabVillages.Controls.Add(this.villageTabController);
            this.tabVillages.Location = new System.Drawing.Point(4, 25);
            this.tabVillages.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabVillages.Name = "tabVillages";
            this.tabVillages.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabVillages.Size = new System.Drawing.Size(1263, 785);
            this.tabVillages.TabIndex = 0;
            this.tabVillages.Text = "Villages";
            this.tabVillages.UseVisualStyleBackColor = true;
            // 
            // RefreshAllVills
            // 
            this.RefreshAllVills.Location = new System.Drawing.Point(145, 753);
            this.RefreshAllVills.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RefreshAllVills.Name = "RefreshAllVills";
            this.RefreshAllVills.Size = new System.Drawing.Size(120, 28);
            this.RefreshAllVills.TabIndex = 9;
            this.RefreshAllVills.Text = "Refresh all vills";
            this.RefreshAllVills.UseVisualStyleBackColor = true;
            this.RefreshAllVills.Click += new System.EventHandler(this.RefreshAllVills_Click);
            // 
            // RefreshVill
            // 
            this.RefreshVill.Location = new System.Drawing.Point(4, 753);
            this.RefreshVill.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RefreshVill.Name = "RefreshVill";
            this.RefreshVill.Size = new System.Drawing.Size(116, 28);
            this.RefreshVill.TabIndex = 8;
            this.RefreshVill.Text = "Refresh village";
            this.RefreshVill.UseVisualStyleBackColor = true;
            this.RefreshVill.Click += new System.EventHandler(this.RefreshVill_Click);
            // 
            // VillagesListView
            // 
            this.VillagesListView.BackColor = System.Drawing.SystemColors.Window;
            this.VillagesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.VillagesListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.VillagesListView.FullRowSelect = true;
            this.VillagesListView.GridLines = true;
            this.VillagesListView.HideSelection = false;
            this.VillagesListView.Location = new System.Drawing.Point(4, 4);
            this.VillagesListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.VillagesListView.MultiSelect = false;
            this.VillagesListView.Name = "VillagesListView";
            this.VillagesListView.Size = new System.Drawing.Size(267, 747);
            this.VillagesListView.TabIndex = 7;
            this.VillagesListView.UseCompatibleStateImageBehavior = false;
            this.VillagesListView.View = System.Windows.Forms.View.Details;
            this.VillagesListView.SelectedIndexChanged += new System.EventHandler(this.VillagesListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 38;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Coords";
            this.columnHeader2.Width = 46;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 43;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Resources";
            // 
            // villageTabController
            // 
            this.villageTabController.Controls.Add(this.villTabBuild);
            this.villageTabController.Controls.Add(this.villTabMarket);
            this.villageTabController.Controls.Add(this.villTabTroops);
            this.villageTabController.Controls.Add(this.villTabAttack);
            this.villageTabController.Dock = System.Windows.Forms.DockStyle.Right;
            this.villageTabController.Location = new System.Drawing.Point(271, 4);
            this.villageTabController.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villageTabController.Name = "villageTabController";
            this.villageTabController.SelectedIndex = 0;
            this.villageTabController.Size = new System.Drawing.Size(988, 777);
            this.villageTabController.TabIndex = 1;
            this.villageTabController.SelectedIndexChanged += new System.EventHandler(this.villageTabController_SelectedIndexChanged);
            // 
            // villTabBuild
            // 
            this.villTabBuild.Controls.Add(this.buildUc1);
            this.villTabBuild.Location = new System.Drawing.Point(4, 25);
            this.villTabBuild.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabBuild.Name = "villTabBuild";
            this.villTabBuild.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabBuild.Size = new System.Drawing.Size(980, 748);
            this.villTabBuild.TabIndex = 0;
            this.villTabBuild.Text = "Build";
            this.villTabBuild.UseVisualStyleBackColor = true;
            // 
            // buildUc1
            // 
            this.buildUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buildUc1.Location = new System.Drawing.Point(4, 4);
            this.buildUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buildUc1.Name = "buildUc1";
            this.buildUc1.Size = new System.Drawing.Size(972, 740);
            this.buildUc1.TabIndex = 0;
            // 
            // villTabMarket
            // 
            this.villTabMarket.Controls.Add(this.marketUc1);
            this.villTabMarket.Location = new System.Drawing.Point(4, 25);
            this.villTabMarket.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabMarket.Name = "villTabMarket";
            this.villTabMarket.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabMarket.Size = new System.Drawing.Size(980, 51);
            this.villTabMarket.TabIndex = 1;
            this.villTabMarket.Text = "Market";
            this.villTabMarket.UseVisualStyleBackColor = true;
            // 
            // marketUc1
            // 
            this.marketUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.marketUc1.Location = new System.Drawing.Point(4, 4);
            this.marketUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.marketUc1.Name = "marketUc1";
            this.marketUc1.Size = new System.Drawing.Size(972, 43);
            this.marketUc1.TabIndex = 0;
            // 
            // villTabTroops
            // 
            this.villTabTroops.Controls.Add(this.troopsUc1);
            this.villTabTroops.Location = new System.Drawing.Point(4, 25);
            this.villTabTroops.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabTroops.Name = "villTabTroops";
            this.villTabTroops.Size = new System.Drawing.Size(980, 51);
            this.villTabTroops.TabIndex = 2;
            this.villTabTroops.Text = "Troops";
            this.villTabTroops.UseVisualStyleBackColor = true;
            // 
            // troopsUc1
            // 
            this.troopsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.troopsUc1.Location = new System.Drawing.Point(0, 0);
            this.troopsUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.troopsUc1.Name = "troopsUc1";
            this.troopsUc1.Size = new System.Drawing.Size(980, 51);
            this.troopsUc1.TabIndex = 0;
            // 
            // villTabAttack
            // 
            this.villTabAttack.Controls.Add(this.attackUc1);
            this.villTabAttack.Location = new System.Drawing.Point(4, 25);
            this.villTabAttack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabAttack.Name = "villTabAttack";
            this.villTabAttack.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.villTabAttack.Size = new System.Drawing.Size(980, 748);
            this.villTabAttack.TabIndex = 3;
            this.villTabAttack.Text = "Attack";
            this.villTabAttack.UseVisualStyleBackColor = true;
            // 
            // attackUc1
            // 
            this.attackUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attackUc1.Location = new System.Drawing.Point(4, 4);
            this.attackUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.attackUc1.Name = "attackUc1";
            this.attackUc1.Size = new System.Drawing.Size(972, 740);
            this.attackUc1.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.overviewUc1);
            this.tabOverview.Location = new System.Drawing.Point(4, 25);
            this.tabOverview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabOverview.Size = new System.Drawing.Size(1263, 785);
            this.tabOverview.TabIndex = 4;
            this.tabOverview.Text = "Overview";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // overviewUc1
            // 
            this.overviewUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewUc1.Location = new System.Drawing.Point(4, 4);
            this.overviewUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.overviewUc1.Name = "overviewUc1";
            this.overviewUc1.Size = new System.Drawing.Size(1255, 777);
            this.overviewUc1.TabIndex = 0;
            // 
            // FarmingTab
            // 
            this.FarmingTab.Controls.Add(this.farmingUc1);
            this.FarmingTab.Location = new System.Drawing.Point(4, 25);
            this.FarmingTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FarmingTab.Name = "FarmingTab";
            this.FarmingTab.Size = new System.Drawing.Size(1263, 785);
            this.FarmingTab.TabIndex = 5;
            this.FarmingTab.Text = "Farming";
            this.FarmingTab.UseVisualStyleBackColor = true;
            // 
            // farmingUc1
            // 
            this.farmingUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.farmingUc1.Location = new System.Drawing.Point(0, 0);
            this.farmingUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.farmingUc1.Name = "farmingUc1";
            this.farmingUc1.Size = new System.Drawing.Size(1263, 785);
            this.farmingUc1.TabIndex = 0;
            // 
            // newVillagesTab
            // 
            this.newVillagesTab.Controls.Add(this.newVillagesUc1);
            this.newVillagesTab.Location = new System.Drawing.Point(4, 25);
            this.newVillagesTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.newVillagesTab.Name = "newVillagesTab";
            this.newVillagesTab.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.newVillagesTab.Size = new System.Drawing.Size(1263, 785);
            this.newVillagesTab.TabIndex = 6;
            this.newVillagesTab.Text = "New villages";
            this.newVillagesTab.UseVisualStyleBackColor = true;
            // 
            // newVillagesUc1
            // 
            this.newVillagesUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newVillagesUc1.Location = new System.Drawing.Point(4, 4);
            this.newVillagesUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.newVillagesUc1.Name = "newVillagesUc1";
            this.newVillagesUc1.Size = new System.Drawing.Size(1255, 777);
            this.newVillagesUc1.TabIndex = 0;
            // 
            // deffendingTab
            // 
            this.deffendingTab.Controls.Add(this.deffendingUc1);
            this.deffendingTab.Location = new System.Drawing.Point(4, 25);
            this.deffendingTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.deffendingTab.Name = "deffendingTab";
            this.deffendingTab.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.deffendingTab.Size = new System.Drawing.Size(1263, 785);
            this.deffendingTab.TabIndex = 7;
            this.deffendingTab.Text = "Deffending";
            this.deffendingTab.UseVisualStyleBackColor = true;
            // 
            // deffendingUc1
            // 
            this.deffendingUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deffendingUc1.Location = new System.Drawing.Point(4, 4);
            this.deffendingUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.deffendingUc1.Name = "deffendingUc1";
            this.deffendingUc1.Size = new System.Drawing.Size(1255, 777);
            this.deffendingUc1.TabIndex = 0;
            // 
            // questsTab
            // 
            this.questsTab.Controls.Add(this.questsUc1);
            this.questsTab.Location = new System.Drawing.Point(4, 25);
            this.questsTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.questsTab.Name = "questsTab";
            this.questsTab.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.questsTab.Size = new System.Drawing.Size(1263, 785);
            this.questsTab.TabIndex = 9;
            this.questsTab.Text = "Quests";
            this.questsTab.UseVisualStyleBackColor = true;
            // 
            // questsUc1
            // 
            this.questsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.questsUc1.Location = new System.Drawing.Point(4, 4);
            this.questsUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.questsUc1.Name = "questsUc1";
            this.questsUc1.Size = new System.Drawing.Size(1255, 777);
            this.questsUc1.TabIndex = 0;
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.debugUc1);
            this.debugTab.Location = new System.Drawing.Point(4, 25);
            this.debugTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.debugTab.Name = "debugTab";
            this.debugTab.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.debugTab.Size = new System.Drawing.Size(1263, 785);
            this.debugTab.TabIndex = 8;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // debugUc1
            // 
            this.debugUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugUc1.Location = new System.Drawing.Point(4, 4);
            this.debugUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.debugUc1.Name = "debugUc1";
            this.debugUc1.Size = new System.Drawing.Size(1255, 777);
            this.debugUc1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(17, 481);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 4;
            this.button2.Text = "Login";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(132, 517);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 28);
            this.button3.TabIndex = 5;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // accListView
            // 
            this.accListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.accAccountHeader,
            this.accProxyOkHeader,
            this.accProxyHeader});
            this.accListView.FullRowSelect = true;
            this.accListView.HideSelection = false;
            this.accListView.Location = new System.Drawing.Point(9, 52);
            this.accListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.accListView.MultiSelect = false;
            this.accListView.Name = "accListView";
            this.accListView.Size = new System.Drawing.Size(255, 421);
            this.accListView.TabIndex = 6;
            this.accListView.UseCompatibleStateImageBehavior = false;
            this.accListView.View = System.Windows.Forms.View.Details;
            this.accListView.SelectedIndexChanged += new System.EventHandler(this.accListView_SelectedIndexChanged);
            // 
            // accAccountHeader
            // 
            this.accAccountHeader.Text = "Account";
            this.accAccountHeader.Width = 79;
            // 
            // accProxyOkHeader
            // 
            this.accProxyOkHeader.Text = "Proxy OK";
            this.accProxyOkHeader.Width = 78;
            // 
            // accProxyHeader
            // 
            this.accProxyHeader.Text = "Proxy";
            this.accProxyHeader.Width = 88;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(17, 517);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(100, 28);
            this.button7.TabIndex = 7;
            this.button7.Text = "Edit";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(17, 770);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 28);
            this.button4.TabIndex = 8;
            this.button4.Text = "dont click";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(132, 481);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(104, 28);
            this.button5.TabIndex = 9;
            this.button5.Text = "Logout";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1561, 814);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.accListView);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.accTabController);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ControlPanel";
            this.Text = "Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanel_FormClosing);
            this.accTabController.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabHero.ResumeLayout(false);
            this.tabVillages.ResumeLayout(false);
            this.villageTabController.ResumeLayout(false);
            this.villTabBuild.ResumeLayout(false);
            this.villTabMarket.ResumeLayout(false);
            this.villTabTroops.ResumeLayout(false);
            this.villTabAttack.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.FarmingTab.ResumeLayout(false);
            this.newVillagesTab.ResumeLayout(false);
            this.deffendingTab.ResumeLayout(false);
            this.questsTab.ResumeLayout(false);
            this.debugTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl accTabController;
        private System.Windows.Forms.TabPage tabVillages;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl villageTabController;
        private System.Windows.Forms.TabPage villTabBuild;
        private System.Windows.Forms.TabPage villTabMarket;
        private System.Windows.Forms.ListView accListView;
        private System.Windows.Forms.ColumnHeader accAccountHeader;
        private System.Windows.Forms.ColumnHeader accProxyHeader;
        private System.Windows.Forms.ColumnHeader accProxyOkHeader;
        private System.Windows.Forms.ListView VillagesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TabPage tabHero;
        private System.Windows.Forms.TabPage villTabTroops;
        private System.Windows.Forms.TabPage tabGeneral;
        private Views.GeneralUc generalUc1;
        private Views.HeroUc heroUc1;
        private Views.BuildUc buildUc1;
        private Views.MarketUc marketUc1;
        private Views.TroopsUc troopsUc1;
        private System.Windows.Forms.TabPage tabOverview;
        private Views.OverviewUc overviewUc1;
        private System.Windows.Forms.Button RefreshAllVills;
        private System.Windows.Forms.Button RefreshVill;
        private System.Windows.Forms.TabPage FarmingTab;
        private Views.FarmingUc farmingUc1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabPage newVillagesTab;
        private Views.NewVillagesUc newVillagesUc1;
        private System.Windows.Forms.TabPage deffendingTab;
        private Views.DeffendingUc deffendingUc1;
        private System.Windows.Forms.TabPage villTabAttack;
        private Views.AttackUc attackUc1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage debugTab;
        private Views.DebugUc debugUc1;
        private System.Windows.Forms.TabPage questsTab;
        private Views.QuestsUc questsUc1;
    }
}