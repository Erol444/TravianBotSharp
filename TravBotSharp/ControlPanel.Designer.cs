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
            this.villagesUc1 = new TravBotSharp.Views.VillagesUc();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.overviewUc1 = new TravBotSharp.Views.OverviewUc();
            this.troopsTab = new System.Windows.Forms.TabPage();
            this.overviewTroopsUc1 = new TravBotSharp.Views.OverviewTroopsUc();
            this.FarmingTab = new System.Windows.Forms.TabPage();
            this.farmingUc1 = new TravBotSharp.Views.FarmingUc();
            this.newVillagesTab = new System.Windows.Forms.TabPage();
            this.newVillagesUc1 = new TravBotSharp.Views.NewVillagesUc();
            this.deffendingTab = new System.Windows.Forms.TabPage();
            this.deffendingUc1 = new TravBotSharp.Views.DeffendingUc();
            this.questsTab = new System.Windows.Forms.TabPage();
            this.questsUc1 = new TravBotSharp.Views.QuestsUc();
            this.discordTab = new System.Windows.Forms.TabPage();
            this.debugTab = new System.Windows.Forms.TabPage();
            this.debugUc1 = new TravBotSharp.Views.DebugUc();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.accListView = new System.Windows.Forms.ListView();
            this.accAccountHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accProxyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.discordUc1 = new TravBotSharp.Views.DiscordUc();
            this.accTabController.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabHero.SuspendLayout();
            this.tabVillages.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.troopsTab.SuspendLayout();
            this.FarmingTab.SuspendLayout();
            this.newVillagesTab.SuspendLayout();
            this.deffendingTab.SuspendLayout();
            this.questsTab.SuspendLayout();
            this.discordTab.SuspendLayout();
            this.debugTab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 23);
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
            this.accTabController.Controls.Add(this.deffendingTab);
            this.accTabController.Controls.Add(this.questsTab);
            this.accTabController.Controls.Add(this.discordTab);
            this.accTabController.Controls.Add(this.debugTab);
            this.accTabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accTabController.Location = new System.Drawing.Point(0, 0);
            this.accTabController.Name = "accTabController";
            this.accTabController.SelectedIndex = 0;
            this.accTabController.Size = new System.Drawing.Size(971, 661);
            this.accTabController.TabIndex = 3;
            this.accTabController.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.generalUc1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(963, 635);
            this.tabGeneral.TabIndex = 3;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // generalUc1
            // 
            this.generalUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalUc1.Location = new System.Drawing.Point(3, 3);
            this.generalUc1.Margin = new System.Windows.Forms.Padding(4);
            this.generalUc1.Name = "generalUc1";
            this.generalUc1.Size = new System.Drawing.Size(957, 629);
            this.generalUc1.TabIndex = 0;
            // 
            // tabHero
            // 
            this.tabHero.Controls.Add(this.heroUc1);
            this.tabHero.Location = new System.Drawing.Point(4, 22);
            this.tabHero.Name = "tabHero";
            this.tabHero.Size = new System.Drawing.Size(963, 635);
            this.tabHero.TabIndex = 2;
            this.tabHero.Text = "Hero";
            this.tabHero.UseVisualStyleBackColor = true;
            // 
            // heroUc1
            // 
            this.heroUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroUc1.Location = new System.Drawing.Point(0, 0);
            this.heroUc1.Margin = new System.Windows.Forms.Padding(4);
            this.heroUc1.Name = "heroUc1";
            this.heroUc1.Size = new System.Drawing.Size(963, 635);
            this.heroUc1.TabIndex = 0;
            // 
            // tabVillages
            // 
            this.tabVillages.Controls.Add(this.villagesUc1);
            this.tabVillages.Location = new System.Drawing.Point(4, 22);
            this.tabVillages.Name = "tabVillages";
            this.tabVillages.Padding = new System.Windows.Forms.Padding(3);
            this.tabVillages.Size = new System.Drawing.Size(963, 635);
            this.tabVillages.TabIndex = 0;
            this.tabVillages.Text = "Villages";
            this.tabVillages.UseVisualStyleBackColor = true;
            // 
            // villagesUc1
            // 
            this.villagesUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.villagesUc1.Location = new System.Drawing.Point(3, 3);
            this.villagesUc1.Margin = new System.Windows.Forms.Padding(4);
            this.villagesUc1.Name = "villagesUc1";
            this.villagesUc1.Size = new System.Drawing.Size(957, 629);
            this.villagesUc1.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.overviewUc1);
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverview.Size = new System.Drawing.Size(963, 635);
            this.tabOverview.TabIndex = 4;
            this.tabOverview.Text = "Overview";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // overviewUc1
            // 
            this.overviewUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewUc1.Location = new System.Drawing.Point(3, 3);
            this.overviewUc1.Margin = new System.Windows.Forms.Padding(4);
            this.overviewUc1.Name = "overviewUc1";
            this.overviewUc1.Size = new System.Drawing.Size(957, 629);
            this.overviewUc1.TabIndex = 0;
            // 
            // troopsTab
            // 
            this.troopsTab.Controls.Add(this.overviewTroopsUc1);
            this.troopsTab.Location = new System.Drawing.Point(4, 22);
            this.troopsTab.Name = "troopsTab";
            this.troopsTab.Padding = new System.Windows.Forms.Padding(3);
            this.troopsTab.Size = new System.Drawing.Size(963, 635);
            this.troopsTab.TabIndex = 10;
            this.troopsTab.Text = "Troops";
            this.troopsTab.UseVisualStyleBackColor = true;
            // 
            // overviewTroopsUc1
            // 
            this.overviewTroopsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewTroopsUc1.Location = new System.Drawing.Point(3, 3);
            this.overviewTroopsUc1.Name = "overviewTroopsUc1";
            this.overviewTroopsUc1.Size = new System.Drawing.Size(957, 629);
            this.overviewTroopsUc1.TabIndex = 0;
            // 
            // FarmingTab
            // 
            this.FarmingTab.Controls.Add(this.farmingUc1);
            this.FarmingTab.Location = new System.Drawing.Point(4, 22);
            this.FarmingTab.Name = "FarmingTab";
            this.FarmingTab.Size = new System.Drawing.Size(963, 635);
            this.FarmingTab.TabIndex = 5;
            this.FarmingTab.Text = "Farming";
            this.FarmingTab.UseVisualStyleBackColor = true;
            // 
            // farmingUc1
            // 
            this.farmingUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.farmingUc1.Location = new System.Drawing.Point(0, 0);
            this.farmingUc1.Margin = new System.Windows.Forms.Padding(4);
            this.farmingUc1.Name = "farmingUc1";
            this.farmingUc1.Size = new System.Drawing.Size(963, 635);
            this.farmingUc1.TabIndex = 0;
            // 
            // newVillagesTab
            // 
            this.newVillagesTab.Controls.Add(this.newVillagesUc1);
            this.newVillagesTab.Location = new System.Drawing.Point(4, 22);
            this.newVillagesTab.Name = "newVillagesTab";
            this.newVillagesTab.Padding = new System.Windows.Forms.Padding(3);
            this.newVillagesTab.Size = new System.Drawing.Size(963, 635);
            this.newVillagesTab.TabIndex = 6;
            this.newVillagesTab.Text = "New villages";
            this.newVillagesTab.UseVisualStyleBackColor = true;
            // 
            // newVillagesUc1
            // 
            this.newVillagesUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newVillagesUc1.Location = new System.Drawing.Point(3, 3);
            this.newVillagesUc1.Margin = new System.Windows.Forms.Padding(4);
            this.newVillagesUc1.Name = "newVillagesUc1";
            this.newVillagesUc1.Size = new System.Drawing.Size(957, 629);
            this.newVillagesUc1.TabIndex = 0;
            // 
            // deffendingTab
            // 
            this.deffendingTab.Controls.Add(this.deffendingUc1);
            this.deffendingTab.Location = new System.Drawing.Point(4, 22);
            this.deffendingTab.Name = "deffendingTab";
            this.deffendingTab.Padding = new System.Windows.Forms.Padding(3);
            this.deffendingTab.Size = new System.Drawing.Size(963, 635);
            this.deffendingTab.TabIndex = 7;
            this.deffendingTab.Text = "Deffending";
            this.deffendingTab.UseVisualStyleBackColor = true;
            // 
            // deffendingUc1
            // 
            this.deffendingUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deffendingUc1.Location = new System.Drawing.Point(3, 3);
            this.deffendingUc1.Margin = new System.Windows.Forms.Padding(4);
            this.deffendingUc1.Name = "deffendingUc1";
            this.deffendingUc1.Size = new System.Drawing.Size(957, 629);
            this.deffendingUc1.TabIndex = 0;
            // 
            // questsTab
            // 
            this.questsTab.Controls.Add(this.questsUc1);
            this.questsTab.Location = new System.Drawing.Point(4, 22);
            this.questsTab.Name = "questsTab";
            this.questsTab.Padding = new System.Windows.Forms.Padding(3);
            this.questsTab.Size = new System.Drawing.Size(963, 635);
            this.questsTab.TabIndex = 9;
            this.questsTab.Text = "Quests";
            this.questsTab.UseVisualStyleBackColor = true;
            // 
            // questsUc1
            // 
            this.questsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.questsUc1.Location = new System.Drawing.Point(3, 3);
            this.questsUc1.Margin = new System.Windows.Forms.Padding(4);
            this.questsUc1.Name = "questsUc1";
            this.questsUc1.Size = new System.Drawing.Size(957, 629);
            this.questsUc1.TabIndex = 0;
            // 
            // discordTab
            // 
            this.discordTab.Controls.Add(this.discordUc1);
            this.discordTab.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.discordTab.Location = new System.Drawing.Point(4, 22);
            this.discordTab.Name = "discordTab";
            this.discordTab.Padding = new System.Windows.Forms.Padding(3);
            this.discordTab.Size = new System.Drawing.Size(963, 635);
            this.discordTab.TabIndex = 11;
            this.discordTab.Text = "Discord";
            this.discordTab.UseVisualStyleBackColor = true;
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.debugUc1);
            this.debugTab.Location = new System.Drawing.Point(4, 22);
            this.debugTab.Name = "debugTab";
            this.debugTab.Padding = new System.Windows.Forms.Padding(3);
            this.debugTab.Size = new System.Drawing.Size(963, 635);
            this.debugTab.TabIndex = 8;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // debugUc1
            // 
            this.debugUc1.CausesValidation = false;
            this.debugUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugUc1.Location = new System.Drawing.Point(3, 3);
            this.debugUc1.Margin = new System.Windows.Forms.Padding(4);
            this.debugUc1.Name = "debugUc1";
            this.debugUc1.Size = new System.Drawing.Size(957, 629);
            this.debugUc1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Login";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(101, 41);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(78, 23);
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
            this.accProxyHeader});
            this.accListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accListView.FullRowSelect = true;
            this.accListView.HideSelection = false;
            this.accListView.Location = new System.Drawing.Point(0, 0);
            this.accListView.MultiSelect = false;
            this.accListView.Name = "accListView";
            this.accListView.Size = new System.Drawing.Size(200, 509);
            this.accListView.TabIndex = 6;
            this.accListView.UseCompatibleStateImageBehavior = false;
            this.accListView.View = System.Windows.Forms.View.Details;
            this.accListView.SelectedIndexChanged += new System.EventHandler(this.accListView_SelectedIndexChanged);
            // 
            // accAccountHeader
            // 
            this.accAccountHeader.Text = "Account";
            this.accAccountHeader.Width = 93;
            // 
            // accProxyHeader
            // 
            this.accProxyHeader.Text = "Proxy";
            this.accProxyHeader.Width = 95;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(15, 41);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 7;
            this.button7.Text = "Edit";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(101, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(78, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Logout";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(15, 70);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 10;
            this.button6.Text = "Login all";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 661);
            this.panel1.TabIndex = 11;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.accListView);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 46);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 509);
            this.panel4.TabIndex = 13;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.button5);
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.button7);
            this.panel3.Controls.Add(this.button6);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 555);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 106);
            this.panel3.TabIndex = 12;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(101, 70);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(78, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Logout all";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 46);
            this.panel2.TabIndex = 11;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.accTabController);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(200, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(971, 661);
            this.panel5.TabIndex = 12;
            // 
            // discordUc1
            // 
            this.discordUc1.Location = new System.Drawing.Point(2, 2);
            this.discordUc1.Name = "discordUc1";
            this.discordUc1.Size = new System.Drawing.Size(950, 536);
            this.discordUc1.TabIndex = 0;
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 661);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Name = "ControlPanel";
            this.Text = "Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanel_FormClosing);
            this.accTabController.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabHero.ResumeLayout(false);
            this.tabVillages.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.troopsTab.ResumeLayout(false);
            this.FarmingTab.ResumeLayout(false);
            this.newVillagesTab.ResumeLayout(false);
            this.deffendingTab.ResumeLayout(false);
            this.questsTab.ResumeLayout(false);
            this.discordTab.ResumeLayout(false);
            this.debugTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
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
        private System.Windows.Forms.ColumnHeader accProxyHeader;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TabPage tabHero;
        private System.Windows.Forms.TabPage tabGeneral;
        private Views.GeneralUc generalUc1;
        private Views.HeroUc heroUc1;
        private System.Windows.Forms.TabPage tabOverview;
        private Views.OverviewUc overviewUc1;
        private System.Windows.Forms.TabPage FarmingTab;
        private Views.FarmingUc farmingUc1;
        private System.Windows.Forms.TabPage newVillagesTab;
        private Views.NewVillagesUc newVillagesUc1;
        private System.Windows.Forms.TabPage deffendingTab;
        private Views.DeffendingUc deffendingUc1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage debugTab;
        private Views.DebugUc debugUc1;
        private System.Windows.Forms.TabPage questsTab;
        private Views.QuestsUc questsUc1;
        private System.Windows.Forms.Button button6;
        private Views.VillagesUc villagesUc1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TabPage troopsTab;
        private Views.OverviewTroopsUc overviewTroopsUc1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabPage discordTab;
        private Views.DiscordUc discordUc1;
    }
}