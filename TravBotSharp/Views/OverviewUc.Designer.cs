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
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer1 = new XPTable.Renderers.DragDropRenderer();
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder2 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer2 = new XPTable.Renderers.DragDropRenderer();
            this.SaveButton = new System.Windows.Forms.Button();
            this.table1 = new XPTable.Models.Table();
            this.XpTableGlobal = new XPTable.Models.Table();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.villId = new XPTable.Models.TextColumn();
            this.vill = new XPTable.Models.TextColumn();
            this.type = new XPTable.Models.ComboBoxColumn();
            this.barracks = new XPTable.Models.ComboBoxColumn();
            this.gb = new XPTable.Models.CheckBoxColumn();
            this.stable = new XPTable.Models.ComboBoxColumn();
            this.gs = new XPTable.Models.CheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XpTableGlobal)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(457, 589);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(82, 25);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // table1
            // 
            this.table1.BorderColor = System.Drawing.Color.Black;
            this.table1.DataMember = null;
            this.table1.DataSourceColumnBinder = dataSourceColumnBinder1;
            dragDropRenderer1.ForeColor = System.Drawing.Color.Red;
            this.table1.DragDropRenderer = dragDropRenderer1;
            this.table1.GridLinesContrainedToData = false;
            this.table1.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.table1.Location = new System.Drawing.Point(0, 94);
            this.table1.MultiSelect = true;
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(984, 492);
            this.table1.TabIndex = 0;
            this.table1.Text = "table1";
            this.table1.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // XpTableGlobal
            // 
            this.XpTableGlobal.BorderColor = System.Drawing.Color.Black;
            this.XpTableGlobal.DataMember = null;
            this.XpTableGlobal.DataSourceColumnBinder = dataSourceColumnBinder2;
            dragDropRenderer2.ForeColor = System.Drawing.Color.Red;
            this.XpTableGlobal.DragDropRenderer = dragDropRenderer2;
            this.XpTableGlobal.GridLinesContrainedToData = false;
            this.XpTableGlobal.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XpTableGlobal.Location = new System.Drawing.Point(0, 3);
            this.XpTableGlobal.MultiSelect = true;
            this.XpTableGlobal.Name = "XpTableGlobal";
            this.XpTableGlobal.Size = new System.Drawing.Size(984, 55);
            this.XpTableGlobal.TabIndex = 2;
            this.XpTableGlobal.Text = "table2";
            this.XpTableGlobal.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(348, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(269, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Change selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(630, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Click save afterwards";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Import tasks";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
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
            // OverviewUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.XpTableGlobal);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.table1);
            this.Name = "OverviewUc";
            this.Size = new System.Drawing.Size(988, 617);
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XpTableGlobal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SaveButton;
        private XPTable.Models.Table table1;
        private XPTable.Models.Table XpTableGlobal;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private XPTable.Models.TextColumn villId;
        private XPTable.Models.TextColumn vill;
        private XPTable.Models.ComboBoxColumn type;
        private XPTable.Models.ComboBoxColumn barracks;
        private XPTable.Models.CheckBoxColumn gb;
        private XPTable.Models.ComboBoxColumn stable;
        private XPTable.Models.CheckBoxColumn gs;
    }
}