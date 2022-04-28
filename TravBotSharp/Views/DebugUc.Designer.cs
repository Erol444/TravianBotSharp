namespace TravBotSharp.Views
{
    partial class DebugUc
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
            this.taskListView = new System.Windows.Forms.ListView();
            this.cH1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cH2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cH3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cHvill = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cH4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskListView
            // 
            this.taskListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cH1,
            this.cH2,
            this.cH3,
            this.cHvill,
            this.cH4});
            this.taskListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskListView.FullRowSelect = true;
            this.taskListView.GridLines = true;
            this.taskListView.HideSelection = false;
            this.taskListView.Location = new System.Drawing.Point(3, 52);
            this.taskListView.MultiSelect = false;
            this.taskListView.Name = "taskListView";
            this.taskListView.Size = new System.Drawing.Size(369, 574);
            this.taskListView.TabIndex = 35;
            this.taskListView.UseCompatibleStateImageBehavior = false;
            this.taskListView.View = System.Windows.Forms.View.Details;
            // 
            // cH1
            // 
            this.cH1.Text = "Task";
            this.cH1.Width = 80;
            // 
            // cH2
            // 
            this.cH2.Text = "Vill";
            this.cH2.Width = 56;
            // 
            // cH3
            // 
            this.cH3.Text = "Priority";
            // 
            // cHvill
            // 
            this.cHvill.Text = "Stage";
            // 
            // cH4
            // 
            this.cH4.Text = "Execute at";
            this.cH4.Width = 130;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 20);
            this.label1.TabIndex = 36;
            this.label1.Text = "Tasks to be executed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(378, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 37;
            this.label2.Text = "Log";
            // 
            // logTextBox
            // 
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Location = new System.Drawing.Point(378, 52);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(558, 574);
            this.logTextBox.TabIndex = 38;
            this.logTextBox.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.taskListView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.logTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(939, 629);
            this.tableLayoutPanel1.TabIndex = 39;
            // 
            // button1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.button1, 2);
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(933, 23);
            this.button1.TabIndex = 39;
            this.button1.Text = "Got problem when using TBS ?";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DebugUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DebugUc";
            this.Size = new System.Drawing.Size(939, 629);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView taskListView;
        private System.Windows.Forms.ColumnHeader cH1;
        private System.Windows.Forms.ColumnHeader cH2;
        private System.Windows.Forms.ColumnHeader cH3;
        private System.Windows.Forms.ColumnHeader cH4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader cHvill;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
    }
}
