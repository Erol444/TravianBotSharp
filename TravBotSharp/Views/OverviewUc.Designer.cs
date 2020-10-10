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
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XpTableGlobal)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(609, 725);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(109, 31);
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
            this.table1.Location = new System.Drawing.Point(0, 116);
            this.table1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.table1.MultiSelect = true;
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(1312, 606);
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
            this.XpTableGlobal.Location = new System.Drawing.Point(0, 4);
            this.XpTableGlobal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.XpTableGlobal.MultiSelect = true;
            this.XpTableGlobal.Name = "XpTableGlobal";
            this.XpTableGlobal.Size = new System.Drawing.Size(1312, 68);
            this.XpTableGlobal.TabIndex = 2;
            this.XpTableGlobal.Text = "table2";
            this.XpTableGlobal.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(464, 79);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(359, 28);
            this.button2.TabIndex = 4;
            this.button2.Text = "Change selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(840, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Click save afterwards";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(39, 79);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 6;
            this.button1.Text = "Import tasks";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1055, 80);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(165, 28);
            this.button3.TabIndex = 7;
            this.button3.Text = "Save new vill settings";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // OverviewUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.XpTableGlobal);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.table1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "OverviewUc";
            this.Size = new System.Drawing.Size(1317, 759);
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
        private System.Windows.Forms.Button button3;
    }
}