namespace TbsWinformNet6.Views
{
    partial class OverviewTroopsUc
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.IdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VillageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BarracksColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.GBColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StableColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.GSColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.WorkshopColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AutoImproveColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SaveButton.Location = new System.Drawing.Point(2, 2);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(414, 25);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.button1_Click);
            
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SaveButton, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(418, 295);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdColumn,
            this.VillageColumn,
            this.BarracksColumn,
            this.GBColumn,
            this.StableColumn,
            this.GSColumn,
            this.WorkshopColumn,
            this.AutoImproveColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.Size = new System.Drawing.Size(412, 260);
            this.dataGridView1.TabIndex = 0;
            // 
            // IdColumn
            // 
            this.IdColumn.DataPropertyName = "Id";
            this.IdColumn.HeaderText = "Id";
            this.IdColumn.Name = "IdColumn";
            this.IdColumn.ReadOnly = true;
            this.IdColumn.Width = 41;
            // 
            // VillageColumn
            // 
            this.VillageColumn.DataPropertyName = "Village";
            this.VillageColumn.HeaderText = "Village";
            this.VillageColumn.Name = "VillageColumn";
            this.VillageColumn.ReadOnly = true;
            this.VillageColumn.Width = 63;
            // 
            // BarracksColumn
            // 
            this.BarracksColumn.DataPropertyName = "Barracks";
            this.BarracksColumn.HeaderText = "Barrack";
            this.BarracksColumn.Name = "BarracksColumn";
            this.BarracksColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.BarracksColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.BarracksColumn.Width = 69;
            // 
            // GBColumn
            // 
            this.GBColumn.DataPropertyName = "GB";
            this.GBColumn.HeaderText = "GB";
            this.GBColumn.Name = "GBColumn";
            this.GBColumn.Width = 28;
            // 
            // StableColumn
            // 
            this.StableColumn.DataPropertyName = "Stable";
            this.StableColumn.HeaderText = "Stable";
            this.StableColumn.Name = "StableColumn";
            this.StableColumn.Width = 43;
            // 
            // GSColumn
            // 
            this.GSColumn.DataPropertyName = "GS";
            this.GSColumn.HeaderText = "GS";
            this.GSColumn.Name = "GSColumn";
            this.GSColumn.Width = 28;
            // 
            // WorkshopColumn
            // 
            this.WorkshopColumn.DataPropertyName = "Workshop";
            this.WorkshopColumn.HeaderText = "Workshop";
            this.WorkshopColumn.Name = "WorkshopColumn";
            this.WorkshopColumn.Width = 62;
            // 
            // AutoImproveColumn
            // 
            this.AutoImproveColumn.DataPropertyName = "AutoImprove";
            this.AutoImproveColumn.HeaderText = "AutoImprove";
            this.AutoImproveColumn.Name = "AutoImproveColumn";
            this.AutoImproveColumn.Width = 73;
            // 
            // OverviewTroopsUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OverviewTroopsUc";
            this.Size = new System.Drawing.Size(418, 295);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VillageColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn BarracksColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn GBColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn StableColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn GSColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn WorkshopColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AutoImproveColumn;
    }
}