namespace TravBotSharp.Views
{
    partial class OverviewUc
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
            this.components = new System.ComponentModel.Container();
            this.villId = new XPTable.Models.TextColumn();
            this.vill = new XPTable.Models.TextColumn();
            this.type = new XPTable.Models.ComboBoxColumn();
            this.barracks = new XPTable.Models.ComboBoxColumn();
            this.gb = new XPTable.Models.CheckBoxColumn();
            this.stable = new XPTable.Models.ComboBoxColumn();
            this.gs = new XPTable.Models.CheckBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Village = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutoExpandStorage = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UseHeroRes = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // villId
            // 
            this.villId.Editable = false;
            this.villId.IsTextTrimmed = false;
            this.villId.Text = "Id";
            this.villId.ToolTipText = "Village Id";
            this.villId.Width = 40;
            // 
            // vill
            // 
            this.vill.IsTextTrimmed = false;
            this.vill.Text = "Village";
            this.vill.ToolTipText = "Village name";
            this.vill.Width = 120;
            // 
            // type
            // 
            this.type.IsTextTrimmed = false;
            this.type.Text = "Type";
            this.type.ToolTipText = "Type of the village";
            this.type.Width = 100;
            // 
            // barracks
            // 
            this.barracks.IsTextTrimmed = false;
            this.barracks.Text = "Barracks";
            this.barracks.ToolTipText = "Troops to train in Barracks";
            this.barracks.Width = 100;
            // 
            // gb
            // 
            this.gb.IsTextTrimmed = false;
            this.gb.Text = "GB";
            this.gb.ToolTipText = "Train troops in Great Barracks";
            this.gb.Width = 40;
            // 
            // stable
            // 
            this.stable.IsTextTrimmed = false;
            this.stable.Text = "Stable";
            this.stable.ToolTipText = "Troops to train in Stable";
            this.stable.Width = 100;
            // 
            // gs
            // 
            this.gs.IsTextTrimmed = false;
            this.gs.Text = "GS";
            this.gs.ToolTipText = "Train troops in Great Stable";
            this.gs.Width = 40;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Village,
            this.AutoExpandStorage,
            this.UseHeroRes});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(533, 255);
            this.dataGridView1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(539, 290);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(533, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // Village
            // 
            this.Village.DataPropertyName = "Name";
            this.Village.HeaderText = "Village";
            this.Village.Name = "Village";
            this.Village.ReadOnly = true;
            // 
            // AutoExpandStorage
            // 
            this.AutoExpandStorage.DataPropertyName = "ExpandStorage";
            this.AutoExpandStorage.HeaderText = "AutoExpandStorage";
            this.AutoExpandStorage.Name = "AutoExpandStorage";
            this.AutoExpandStorage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AutoExpandStorage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // UseHeroRes
            // 
            this.UseHeroRes.DataPropertyName = "UseHeroRes";
            this.UseHeroRes.HeaderText = "UseHeroRes";
            this.UseHeroRes.Name = "UseHeroRes";
            // 
            // OverviewUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OverviewUc";
            this.Size = new System.Drawing.Size(539, 290);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private XPTable.Models.TextColumn villId;
        private XPTable.Models.TextColumn vill;
        private XPTable.Models.ComboBoxColumn type;
        private XPTable.Models.ComboBoxColumn barracks;
        private XPTable.Models.CheckBoxColumn gb;
        private XPTable.Models.ComboBoxColumn stable;
        private XPTable.Models.CheckBoxColumn gs;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Village;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AutoExpandStorage;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseHeroRes;
    }
}