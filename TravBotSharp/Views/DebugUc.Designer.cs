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
            this.label3 = new System.Windows.Forms.Label();
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
            this.taskListView.Location = new System.Drawing.Point(3, 30);
            this.taskListView.MultiSelect = false;
            this.taskListView.Name = "taskListView";
            this.taskListView.Size = new System.Drawing.Size(401, 596);
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
            this.label1.Location = new System.Drawing.Point(-1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 20);
            this.label1.TabIndex = 36;
            this.label1.Text = "Tasks to be executed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(417, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 37;
            this.label2.Text = "Log";
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(410, 30);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(522, 596);
            this.logTextBox.TabIndex = 38;
            this.logTextBox.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(459, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(308, 12);
            this.label3.TabIndex = 39;
            this.label3.Text = "Log will auto-update only if you access the Debug tab from some other tab!";
            // 
            // DebugUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.taskListView);
            this.Name = "DebugUc";
            this.Size = new System.Drawing.Size(939, 629);
            this.Enter += new System.EventHandler(this.DebugUc_Enter);
            this.Leave += new System.EventHandler(this.DebugUc_Leave);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.Label label3;
    }
}
