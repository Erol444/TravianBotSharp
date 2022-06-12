namespace TbsWinformNet6
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanel));
            this.button1 = new System.Windows.Forms.Button();
            this.accTabController = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.generalUc1 = new TbsWinformNet6.Views.GeneralUc();
            this.tabHero = new System.Windows.Forms.TabPage();
            this.heroUc1 = new TbsWinformNet6.Views.HeroUc();
            this.tabVillages = new System.Windows.Forms.TabPage();
            this.villagesUc1 = new TbsWinformNet6.Views.VillagesUc();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.overviewUc1 = new TbsWinformNet6.Views.OverviewUc();
            this.troopsTab = new System.Windows.Forms.TabPage();
            this.overviewTroopsUc1 = new TbsWinformNet6.Views.OverviewTroopsUc();
            this.FarmingTab = new System.Windows.Forms.TabPage();
            this.farmingUc1 = new TbsWinformNet6.Views.FarmingUc();
            this.newVillagesTab = new System.Windows.Forms.TabPage();
            this.newVillagesUc1 = new TbsWinformNet6.Views.NewVillagesUc();
            this.questsTab = new System.Windows.Forms.TabPage();
            this.questsUc1 = new TbsWinformNet6.Views.QuestsUc();
            this.debugTab = new System.Windows.Forms.TabPage();
            this.debugUc1 = new TbsWinformNet6.Views.DebugUc();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.accListView = new System.Windows.Forms.ListView();
            this.accAccountHeader = new System.Windows.Forms.ColumnHeader();
            this.accServerHeader = new System.Windows.Forms.ColumnHeader();
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.mainPannel = new System.Windows.Forms.TableLayoutPanel();
            this.sidePannel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonPannel = new System.Windows.Forms.TableLayoutPanel();
            this.button8 = new System.Windows.Forms.Button();
            this.accTabController.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabHero.SuspendLayout();
            this.tabVillages.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.troopsTab.SuspendLayout();
            this.FarmingTab.SuspendLayout();
            this.newVillagesTab.SuspendLayout();
            this.questsTab.SuspendLayout();
            this.debugTab.SuspendLayout();
            this.mainPannel.SuspendLayout();
            this.sidePannel.SuspendLayout();
            this.buttonPannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(4, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 31);
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
            this.accTabController.Controls.Add(this.troopsTab);
            this.accTabController.Controls.Add(this.FarmingTab);
            this.accTabController.Controls.Add(this.newVillagesTab);
            this.accTabController.Controls.Add(this.questsTab);
            this.accTabController.Controls.Add(this.debugTab);
            this.accTabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accTabController.Location = new System.Drawing.Point(252, 3);
            this.accTabController.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.accTabController.Name = "accTabController";
            this.accTabController.SelectedIndex = 0;
            this.accTabController.Size = new System.Drawing.Size(1110, 757);
            this.accTabController.TabIndex = 3;
            this.accTabController.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.generalUc1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGeneral.Size = new System.Drawing.Size(1102, 729);
            this.tabGeneral.TabIndex = 3;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // generalUc1
            // 
            this.generalUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalUc1.Location = new System.Drawing.Point(4, 3);
            this.generalUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.generalUc1.Name = "generalUc1";
            this.generalUc1.Size = new System.Drawing.Size(1094, 723);
            this.generalUc1.TabIndex = 0;
            // 
            // tabHero
            // 
            this.tabHero.Controls.Add(this.heroUc1);
            this.tabHero.Location = new System.Drawing.Point(4, 24);
            this.tabHero.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabHero.Name = "tabHero";
            this.tabHero.Size = new System.Drawing.Size(1104, 728);
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
            this.heroUc1.Size = new System.Drawing.Size(1104, 728);
            this.heroUc1.TabIndex = 0;
            // 
            // tabVillages
            // 
            this.tabVillages.Controls.Add(this.villagesUc1);
            this.tabVillages.Location = new System.Drawing.Point(4, 24);
            this.tabVillages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabVillages.Name = "tabVillages";
            this.tabVillages.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabVillages.Size = new System.Drawing.Size(1104, 728);
            this.tabVillages.TabIndex = 0;
            this.tabVillages.Text = "Villages";
            this.tabVillages.UseVisualStyleBackColor = true;
            // 
            // villagesUc1
            // 
            this.villagesUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.villagesUc1.Location = new System.Drawing.Point(4, 3);
            this.villagesUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.villagesUc1.Name = "villagesUc1";
            this.villagesUc1.Size = new System.Drawing.Size(1096, 722);
            this.villagesUc1.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.overviewUc1);
            this.tabOverview.Location = new System.Drawing.Point(4, 24);
            this.tabOverview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabOverview.Size = new System.Drawing.Size(1104, 728);
            this.tabOverview.TabIndex = 4;
            this.tabOverview.Text = "Overview";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // overviewUc1
            // 
            this.overviewUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewUc1.Location = new System.Drawing.Point(4, 3);
            this.overviewUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.overviewUc1.Name = "overviewUc1";
            this.overviewUc1.Size = new System.Drawing.Size(1096, 722);
            this.overviewUc1.TabIndex = 0;
            // 
            // troopsTab
            // 
            this.troopsTab.Controls.Add(this.overviewTroopsUc1);
            this.troopsTab.Location = new System.Drawing.Point(4, 24);
            this.troopsTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.troopsTab.Name = "troopsTab";
            this.troopsTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.troopsTab.Size = new System.Drawing.Size(1104, 728);
            this.troopsTab.TabIndex = 10;
            this.troopsTab.Text = "Troops";
            this.troopsTab.UseVisualStyleBackColor = true;
            // 
            // overviewTroopsUc1
            // 
            this.overviewTroopsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewTroopsUc1.Location = new System.Drawing.Point(4, 3);
            this.overviewTroopsUc1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.overviewTroopsUc1.Name = "overviewTroopsUc1";
            this.overviewTroopsUc1.Size = new System.Drawing.Size(1096, 722);
            this.overviewTroopsUc1.TabIndex = 0;
            // 
            // FarmingTab
            // 
            this.FarmingTab.Controls.Add(this.farmingUc1);
            this.FarmingTab.Location = new System.Drawing.Point(4, 24);
            this.FarmingTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FarmingTab.Name = "FarmingTab";
            this.FarmingTab.Size = new System.Drawing.Size(1104, 728);
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
            this.farmingUc1.Size = new System.Drawing.Size(1104, 728);
            this.farmingUc1.TabIndex = 0;
            // 
            // newVillagesTab
            // 
            this.newVillagesTab.Controls.Add(this.newVillagesUc1);
            this.newVillagesTab.Location = new System.Drawing.Point(4, 24);
            this.newVillagesTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.newVillagesTab.Name = "newVillagesTab";
            this.newVillagesTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.newVillagesTab.Size = new System.Drawing.Size(1104, 728);
            this.newVillagesTab.TabIndex = 6;
            this.newVillagesTab.Text = "New villages";
            this.newVillagesTab.UseVisualStyleBackColor = true;
            // 
            // newVillagesUc1
            // 
            this.newVillagesUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newVillagesUc1.Location = new System.Drawing.Point(4, 3);
            this.newVillagesUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.newVillagesUc1.Name = "newVillagesUc1";
            this.newVillagesUc1.Size = new System.Drawing.Size(1096, 722);
            this.newVillagesUc1.TabIndex = 0;
            // 
            // questsTab
            // 
            this.questsTab.Controls.Add(this.questsUc1);
            this.questsTab.Location = new System.Drawing.Point(4, 24);
            this.questsTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.questsTab.Name = "questsTab";
            this.questsTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.questsTab.Size = new System.Drawing.Size(1104, 728);
            this.questsTab.TabIndex = 9;
            this.questsTab.Text = "Quests";
            this.questsTab.UseVisualStyleBackColor = true;
            // 
            // questsUc1
            // 
            this.questsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.questsUc1.Location = new System.Drawing.Point(4, 3);
            this.questsUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.questsUc1.Name = "questsUc1";
            this.questsUc1.Size = new System.Drawing.Size(1096, 722);
            this.questsUc1.TabIndex = 0;
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.debugUc1);
            this.debugTab.Location = new System.Drawing.Point(4, 24);
            this.debugTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.debugTab.Name = "debugTab";
            this.debugTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.debugTab.Size = new System.Drawing.Size(1104, 728);
            this.debugTab.TabIndex = 8;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // debugUc1
            // 
            this.debugUc1.CausesValidation = false;
            this.debugUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugUc1.Location = new System.Drawing.Point(4, 3);
            this.debugUc1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.debugUc1.Name = "debugUc1";
            this.debugUc1.Size = new System.Drawing.Size(1096, 722);
            this.debugUc1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(4, 40);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 31);
            this.button2.TabIndex = 4;
            this.button2.Text = "Login";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.Location = new System.Drawing.Point(120, 77);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 31);
            this.button3.TabIndex = 5;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // accListView
            // 
            this.accListView.CausesValidation = false;
            this.accListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.accAccountHeader,
            this.accServerHeader});
            this.accListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accListView.FullRowSelect = true;
            this.accListView.Location = new System.Drawing.Point(4, 159);
            this.accListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.accListView.MultiSelect = false;
            this.accListView.Name = "accListView";
            this.accListView.Size = new System.Drawing.Size(232, 595);
            this.accListView.TabIndex = 6;
            this.accListView.UseCompatibleStateImageBehavior = false;
            this.accListView.View = System.Windows.Forms.View.Details;
            this.accListView.SelectedIndexChanged += new System.EventHandler(this.accListView_SelectedIndexChanged);
            // 
            // accAccountHeader
            // 
            this.accAccountHeader.Text = "Account";
            this.accAccountHeader.Width = 89;
            // 
            // accServerHeader
            // 
            this.accServerHeader.Text = "Server";
            this.accServerHeader.Width = 95;
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button7.Location = new System.Drawing.Point(4, 77);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(108, 31);
            this.button7.TabIndex = 7;
            this.button7.Text = "Edit";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button5.Location = new System.Drawing.Point(120, 40);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(108, 31);
            this.button5.TabIndex = 9;
            this.button5.Text = "Logout";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button6.Location = new System.Drawing.Point(4, 114);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(108, 33);
            this.button6.TabIndex = 10;
            this.button6.Text = "Login all";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button4.Location = new System.Drawing.Point(120, 114);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 33);
            this.button4.TabIndex = 11;
            this.button4.Text = "Logout all";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // mainPannel
            // 
            this.mainPannel.ColumnCount = 2;
            this.mainPannel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.18182F));
            this.mainPannel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.81818F));
            this.mainPannel.Controls.Add(this.sidePannel, 0, 0);
            this.mainPannel.Controls.Add(this.accTabController, 1, 0);
            this.mainPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPannel.Location = new System.Drawing.Point(0, 0);
            this.mainPannel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainPannel.Name = "mainPannel";
            this.mainPannel.RowCount = 1;
            this.mainPannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPannel.Size = new System.Drawing.Size(1366, 763);
            this.mainPannel.TabIndex = 13;
            // 
            // sidePannel
            // 
            this.sidePannel.ColumnCount = 1;
            this.sidePannel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.sidePannel.Controls.Add(this.buttonPannel, 0, 0);
            this.sidePannel.Controls.Add(this.accListView, 0, 1);
            this.sidePannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidePannel.Location = new System.Drawing.Point(4, 3);
            this.sidePannel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sidePannel.Name = "sidePannel";
            this.sidePannel.RowCount = 2;
            this.sidePannel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.sidePannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.sidePannel.Size = new System.Drawing.Size(240, 757);
            this.sidePannel.TabIndex = 14;
            // 
            // buttonPannel
            // 
            this.buttonPannel.ColumnCount = 2;
            this.buttonPannel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonPannel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonPannel.Controls.Add(this.button4, 1, 3);
            this.buttonPannel.Controls.Add(this.button1, 0, 0);
            this.buttonPannel.Controls.Add(this.button6, 0, 3);
            this.buttonPannel.Controls.Add(this.button3, 1, 2);
            this.buttonPannel.Controls.Add(this.button5, 1, 1);
            this.buttonPannel.Controls.Add(this.button7, 0, 2);
            this.buttonPannel.Controls.Add(this.button2, 0, 1);
            this.buttonPannel.Controls.Add(this.button8, 1, 0);
            this.buttonPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPannel.Location = new System.Drawing.Point(4, 3);
            this.buttonPannel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonPannel.Name = "buttonPannel";
            this.buttonPannel.RowCount = 4;
            this.buttonPannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.buttonPannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.buttonPannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.buttonPannel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.buttonPannel.Size = new System.Drawing.Size(232, 150);
            this.buttonPannel.TabIndex = 0;
            // 
            // button8
            // 
            this.button8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button8.Location = new System.Drawing.Point(120, 3);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(108, 31);
            this.button8.TabIndex = 12;
            this.button8.Text = "Add accounts";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1366, 763);
            this.Controls.Add(this.mainPannel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ControlPanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TravianBotSharp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanel_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlPanel_FormClosed);
            this.Load += new System.EventHandler(this.ControlPanel_Load);
            this.accTabController.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabHero.ResumeLayout(false);
            this.tabVillages.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.troopsTab.ResumeLayout(false);
            this.FarmingTab.ResumeLayout(false);
            this.newVillagesTab.ResumeLayout(false);
            this.questsTab.ResumeLayout(false);
            this.debugTab.ResumeLayout(false);
            this.mainPannel.ResumeLayout(false);
            this.sidePannel.ResumeLayout(false);
            this.buttonPannel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl accTabController;
        private System.Windows.Forms.TabPage tabVillages;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListView accListView;
        private System.Windows.Forms.ColumnHeader accAccountHeader;
        private System.Windows.Forms.ColumnHeader accServerHeader;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TabPage tabHero;
        private Views.HeroUc heroUc1;
        private System.Windows.Forms.TabPage tabOverview;
        private Views.OverviewUc overviewUc1;
        private System.Windows.Forms.TabPage FarmingTab;
        private Views.FarmingUc farmingUc1;
        private System.Windows.Forms.TabPage newVillagesTab;
        private Views.NewVillagesUc newVillagesUc1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage debugTab;
        private Views.DebugUc debugUc1;
        private System.Windows.Forms.TabPage questsTab;
        private Views.QuestsUc questsUc1;
        private System.Windows.Forms.Button button6;
        private Views.VillagesUc villagesUc1;
        private System.Windows.Forms.TabPage troopsTab;
        private Views.OverviewTroopsUc overviewTroopsUc1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TableLayoutPanel buttonPannel;
        private System.Windows.Forms.TableLayoutPanel mainPannel;
        private System.Windows.Forms.TableLayoutPanel sidePannel;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TabPage tabGeneral;
        private Views.GeneralUc generalUc1;
    }
}