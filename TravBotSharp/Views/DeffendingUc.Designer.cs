namespace TravBotSharp.Views
{
    partial class DeffendingUc
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
            TbsCore.Models.MapModels.Coordinates coordinates1 = new TbsCore.Models.MapModels.Coordinates();
            this.SaveButton = new System.Windows.Forms.Button();
            this.table1 = new XPTable.Models.Table();
            this.XpTableGlobal = new XPTable.Models.Table();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.maxDeff = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.sendDeffCoords = new TravBotSharp.UserControls.CoordinatesUc();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XpTableGlobal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDeff)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(294, 599);
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
            this.table1.Location = new System.Drawing.Point(0, 82);
            this.table1.MultiSelect = true;
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(657, 513);
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
            this.XpTableGlobal.Size = new System.Drawing.Size(657, 44);
            this.XpTableGlobal.TabIndex = 2;
            this.XpTableGlobal.Text = "table2";
            this.XpTableGlobal.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(116, 52);
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
            this.label1.Location = new System.Drawing.Point(398, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Click save afterwards";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(810, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 22);
            this.button1.TabIndex = 6;
            this.button1.Text = "Send deff";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // maxDeff
            // 
            this.maxDeff.Location = new System.Drawing.Point(813, 131);
            this.maxDeff.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.maxDeff.Name = "maxDeff";
            this.maxDeff.Size = new System.Drawing.Size(92, 20);
            this.maxDeff.TabIndex = 141;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(753, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 15);
            this.label4.TabIndex = 142;
            this.label4.Text = "Max deff";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(800, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 12);
            this.label5.TabIndex = 143;
            this.label5.Text = "If 0, send all available deff";
            // 
            // sendDeffCoords
            // 
            this.sendDeffCoords.BackColor = System.Drawing.SystemColors.ControlDark;
            coordinates1.x = 0;
            coordinates1.y = 0;
            this.sendDeffCoords.Coords = coordinates1;
            this.sendDeffCoords.Location = new System.Drawing.Point(802, 69);
            this.sendDeffCoords.Name = "sendDeffCoords";
            this.sendDeffCoords.Size = new System.Drawing.Size(113, 56);
            this.sendDeffCoords.TabIndex = 144;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(810, 359);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 22);
            this.button3.TabIndex = 146;
            this.button3.Text = "Cut waves";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // DeffendingUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.sendDeffCoords);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxDeff);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.XpTableGlobal);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.table1);
            this.Name = "DeffendingUc";
            this.Size = new System.Drawing.Size(1011, 628);
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XpTableGlobal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDeff)).EndInit();
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
        private System.Windows.Forms.NumericUpDown maxDeff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private UserControls.CoordinatesUc sendDeffCoords;
        private System.Windows.Forms.Button button3;
    }
}