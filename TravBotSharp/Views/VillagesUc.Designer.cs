namespace TravBotSharp.Views
{
    partial class VillagesUc
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
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.panel1.SuspendLayout();
            this.villageTabController.SuspendLayout();
            this.villTabBuild.SuspendLayout();
            this.villTabMarket.SuspendLayout();
            this.villTabTroops.SuspendLayout();
            this.villTabAttack.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RefreshAllVills);
            this.panel1.Controls.Add(this.RefreshVill);
            this.panel1.Controls.Add(this.VillagesListView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 635);
            this.panel1.TabIndex = 0;
            // 
            // RefreshAllVills
            // 
            this.RefreshAllVills.Location = new System.Drawing.Point(96, 606);
            this.RefreshAllVills.Name = "RefreshAllVills";
            this.RefreshAllVills.Size = new System.Drawing.Size(90, 23);
            this.RefreshAllVills.TabIndex = 12;
            this.RefreshAllVills.Text = "Refresh all vills";
            this.RefreshAllVills.UseVisualStyleBackColor = true;
            this.RefreshAllVills.Click += new System.EventHandler(this.RefreshAllVills_Click);
            // 
            // RefreshVill
            // 
            this.RefreshVill.Location = new System.Drawing.Point(3, 606);
            this.RefreshVill.Name = "RefreshVill";
            this.RefreshVill.Size = new System.Drawing.Size(87, 23);
            this.RefreshVill.TabIndex = 11;
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
            this.VillagesListView.Location = new System.Drawing.Point(0, 0);
            this.VillagesListView.MultiSelect = false;
            this.VillagesListView.Name = "VillagesListView";
            this.VillagesListView.Size = new System.Drawing.Size(200, 600);
            this.VillagesListView.TabIndex = 10;
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
            this.villageTabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.villageTabController.Location = new System.Drawing.Point(200, 0);
            this.villageTabController.Name = "villageTabController";
            this.villageTabController.SelectedIndex = 0;
            this.villageTabController.Size = new System.Drawing.Size(745, 635);
            this.villageTabController.TabIndex = 2;
            this.villageTabController.SelectedIndexChanged += new System.EventHandler(this.villageTabController_SelectedIndexChanged);
            // 
            // villTabBuild
            // 
            this.villTabBuild.Controls.Add(this.buildUc1);
            this.villTabBuild.Location = new System.Drawing.Point(4, 22);
            this.villTabBuild.Name = "villTabBuild";
            this.villTabBuild.Padding = new System.Windows.Forms.Padding(3);
            this.villTabBuild.Size = new System.Drawing.Size(737, 609);
            this.villTabBuild.TabIndex = 0;
            this.villTabBuild.Text = "Build";
            this.villTabBuild.UseVisualStyleBackColor = true;
            // 
            // buildUc1
            // 
            this.buildUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buildUc1.Location = new System.Drawing.Point(3, 3);
            this.buildUc1.Margin = new System.Windows.Forms.Padding(4);
            this.buildUc1.Name = "buildUc1";
            this.buildUc1.Size = new System.Drawing.Size(731, 603);
            this.buildUc1.TabIndex = 0;
            // 
            // villTabMarket
            // 
            this.villTabMarket.Controls.Add(this.marketUc1);
            this.villTabMarket.Location = new System.Drawing.Point(4, 22);
            this.villTabMarket.Name = "villTabMarket";
            this.villTabMarket.Padding = new System.Windows.Forms.Padding(3);
            this.villTabMarket.Size = new System.Drawing.Size(733, 603);
            this.villTabMarket.TabIndex = 1;
            this.villTabMarket.Text = "Market";
            this.villTabMarket.UseVisualStyleBackColor = true;
            // 
            // marketUc1
            // 
            this.marketUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.marketUc1.Location = new System.Drawing.Point(3, 3);
            this.marketUc1.Margin = new System.Windows.Forms.Padding(4);
            this.marketUc1.Name = "marketUc1";
            this.marketUc1.Size = new System.Drawing.Size(727, 597);
            this.marketUc1.TabIndex = 0;
            // 
            // villTabTroops
            // 
            this.villTabTroops.Controls.Add(this.troopsUc1);
            this.villTabTroops.Location = new System.Drawing.Point(4, 22);
            this.villTabTroops.Name = "villTabTroops";
            this.villTabTroops.Size = new System.Drawing.Size(733, 603);
            this.villTabTroops.TabIndex = 2;
            this.villTabTroops.Text = "Troops";
            this.villTabTroops.UseVisualStyleBackColor = true;
            // 
            // troopsUc1
            // 
            this.troopsUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.troopsUc1.Location = new System.Drawing.Point(0, 0);
            this.troopsUc1.Margin = new System.Windows.Forms.Padding(4);
            this.troopsUc1.Name = "troopsUc1";
            this.troopsUc1.Size = new System.Drawing.Size(733, 603);
            this.troopsUc1.TabIndex = 0;
            // 
            // villTabAttack
            // 
            this.villTabAttack.Controls.Add(this.attackUc1);
            this.villTabAttack.Location = new System.Drawing.Point(4, 22);
            this.villTabAttack.Name = "villTabAttack";
            this.villTabAttack.Padding = new System.Windows.Forms.Padding(3);
            this.villTabAttack.Size = new System.Drawing.Size(733, 603);
            this.villTabAttack.TabIndex = 3;
            this.villTabAttack.Text = "Attack";
            this.villTabAttack.UseVisualStyleBackColor = true;
            // 
            // attackUc1
            // 
            this.attackUc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attackUc1.Location = new System.Drawing.Point(3, 3);
            this.attackUc1.Margin = new System.Windows.Forms.Padding(4);
            this.attackUc1.Name = "attackUc1";
            this.attackUc1.Size = new System.Drawing.Size(727, 597);
            this.attackUc1.TabIndex = 0;
            // 
            // VillagesUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.villageTabController);
            this.Controls.Add(this.panel1);
            this.Name = "VillagesUc";
            this.Size = new System.Drawing.Size(945, 635);
            this.panel1.ResumeLayout(false);
            this.villageTabController.ResumeLayout(false);
            this.villTabBuild.ResumeLayout(false);
            this.villTabMarket.ResumeLayout(false);
            this.villTabTroops.ResumeLayout(false);
            this.villTabAttack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button RefreshAllVills;
        private System.Windows.Forms.Button RefreshVill;
        private System.Windows.Forms.ListView VillagesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabControl villageTabController;
        private System.Windows.Forms.TabPage villTabBuild;
        private BuildUc buildUc1;
        private System.Windows.Forms.TabPage villTabMarket;
        private MarketUc marketUc1;
        private System.Windows.Forms.TabPage villTabTroops;
        private TroopsUc troopsUc1;
        private System.Windows.Forms.TabPage villTabAttack;
        private AttackUc attackUc1;
    }
}
