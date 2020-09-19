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
            this.taskListView.FullRowSelect = true;
            this.taskListView.GridLines = true;
            this.taskListView.HideSelection = false;
            this.taskListView.Location = new System.Drawing.Point(3, 42);
            this.taskListView.MultiSelect = false;
            this.taskListView.Name = "taskListView";
            this.taskListView.Size = new System.Drawing.Size(470, 394);
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
            this.cH4.Width = 190;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 20);
            this.label1.TabIndex = 36;
            this.label1.Text = "Tasks to be executed";
            // 
            // DebugUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.taskListView);
            this.Name = "DebugUc";
            this.Size = new System.Drawing.Size(842, 439);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView taskListView;
        private System.Windows.Forms.ColumnHeader cH1;
        private System.Windows.Forms.ColumnHeader cH2;
        private System.Windows.Forms.ColumnHeader cH3;
        private System.Windows.Forms.ColumnHeader cH4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader cHvill;
    }
}
