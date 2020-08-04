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
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resources)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // buyAdventuresCheckBox
            // 
            this.buyAdventuresCheckBox.AutoSize = true;
            this.buyAdventuresCheckBox.Location = new System.Drawing.Point(13, 172);
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
            this.label13.Location = new System.Drawing.Point(10, 70);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Minimum Health";
            // 
            // minHeroHealthUpDown
            // 
            this.minHeroHealthUpDown.Location = new System.Drawing.Point(13, 86);
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
            this.strength.Location = new System.Drawing.Point(309, 14);
            this.strength.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.strength.Name = "strength";
            this.strength.Size = new System.Drawing.Size(47, 20);
            this.strength.TabIndex = 8;
            this.strength.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.strength.ValueChanged += new System.EventHandler(this.strength_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Fighting strength";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Off bonus";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(244, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Deff bonus";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(245, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Resources";
            // 
            // offBonus
            // 
            this.offBonus.Location = new System.Drawing.Point(309, 42);
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
            this.deffBonus.Location = new System.Drawing.Point(309, 68);
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
            this.resources.Location = new System.Drawing.Point(309, 94);
            this.resources.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.resources.Name = "resources";
            this.resources.Size = new System.Drawing.Size(47, 20);
            this.resources.TabIndex = 15;
            this.resources.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.resources.ValueChanged += new System.EventHandler(this.resources_ValueChanged);
            // 
            // autoSetHeroPoints
            // 
            this.autoSetHeroPoints.AutoSize = true;
            this.autoSetHeroPoints.Location = new System.Drawing.Point(236, 120);
            this.autoSetHeroPoints.Name = "autoSetHeroPoints";
            this.autoSetHeroPoints.Size = new System.Drawing.Size(120, 17);
            this.autoSetHeroPoints.TabIndex = 16;
            this.autoSetHeroPoints.Text = "Auto set hero points";
            this.autoSetHeroPoints.UseVisualStyleBackColor = true;
            this.autoSetHeroPoints.CheckedChanged += new System.EventHandler(this.autoSetHeroPoints_CheckedChanged);
            // 
            // maxDistanceUpDown
            // 
            this.maxDistanceUpDown.Location = new System.Drawing.Point(13, 136);
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
            this.label5.Location = new System.Drawing.Point(10, 120);
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
            this.SupplyResourcesButton.Location = new System.Drawing.Point(358, 205);
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
            this.SupplyResVillageSelected.Location = new System.Drawing.Point(221, 233);
            this.SupplyResVillageSelected.Name = "SupplyResVillageSelected";
            this.SupplyResVillageSelected.Size = new System.Drawing.Size(55, 13);
            this.SupplyResVillageSelected.TabIndex = 118;
            this.SupplyResVillageSelected.Text = "Selected: ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(221, 187);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(108, 16);
            this.label17.TabIndex = 117;
            this.label17.Text = "Revive hero in";
            // 
            // SupplyResVillageComboBox
            // 
            this.SupplyResVillageComboBox.FormattingEnabled = true;
            this.SupplyResVillageComboBox.Location = new System.Drawing.Point(221, 206);
            this.SupplyResVillageComboBox.Name = "SupplyResVillageComboBox";
            this.SupplyResVillageComboBox.Size = new System.Drawing.Size(130, 21);
            this.SupplyResVillageComboBox.TabIndex = 116;
            // 
            // HeroUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SupplyResourcesButton);
            this.Controls.Add(this.SupplyResVillageSelected);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.SupplyResVillageComboBox);
            this.Controls.Add(this.autoReviveHero);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.maxDistanceUpDown);
            this.Controls.Add(this.autoSetHeroPoints);
            this.Controls.Add(this.resources);
            this.Controls.Add(this.deffBonus);
            this.Controls.Add(this.offBonus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.strength);
            this.Controls.Add(this.buyAdventuresCheckBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.minHeroHealthUpDown);
            this.Controls.Add(this.checkBoxAutoSendToAdventures);
            this.Name = "HeroUc";
            this.Size = new System.Drawing.Size(513, 294);
            ((System.ComponentModel.ISupportInitialize)(this.minHeroHealthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deffBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resources)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDistanceUpDown)).EndInit();
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
    }
}
