
namespace TravBotSharp.Views
{
    partial class DiscordUc
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
            this.Add = new System.Windows.Forms.Button();
            this.Delete = new System.Windows.Forms.Button();
            this.Test = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textboxWebhookURL = new System.Windows.Forms.TextBox();
            this.textboxUserId = new System.Windows.Forms.TextBox();
            this.UserList = new System.Windows.Forms.ListView();
            this.discordIdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.discordNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Show = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(317, 94);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(78, 20);
            this.Add.TabIndex = 2;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            // 
            // Delete
            // 
            this.Delete.Location = new System.Drawing.Point(401, 94);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(78, 20);
            this.Delete.TabIndex = 3;
            this.Delete.Text = "Delete";
            this.Delete.UseVisualStyleBackColor = true;
            // 
            // Test
            // 
            this.Test.Location = new System.Drawing.Point(146, 324);
            this.Test.Name = "Test";
            this.Test.Size = new System.Drawing.Size(333, 40);
            this.Test.TabIndex = 4;
            this.Test.Text = "TEST";
            this.Test.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Webhook URL:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "User ID:";
            // 
            // textboxWebhookURL
            // 
            this.textboxWebhookURL.Location = new System.Drawing.Point(146, 66);
            this.textboxWebhookURL.Name = "textboxWebhookURL";
            this.textboxWebhookURL.PasswordChar = '*';
            this.textboxWebhookURL.Size = new System.Drawing.Size(249, 20);
            this.textboxWebhookURL.TabIndex = 9;
            // 
            // textboxUserId
            // 
            this.textboxUserId.Location = new System.Drawing.Point(146, 94);
            this.textboxUserId.Name = "textboxUserId";
            this.textboxUserId.Size = new System.Drawing.Size(165, 20);
            this.textboxUserId.TabIndex = 10;
            // 
            // UserList
            // 
            this.UserList.BackColor = System.Drawing.SystemColors.MenuText;
            this.UserList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.discordIdHeader,
            this.discordNameHeader});
            this.UserList.FullRowSelect = true;
            this.UserList.GridLines = true;
            this.UserList.HideSelection = false;
            this.UserList.Location = new System.Drawing.Point(146, 120);
            this.UserList.MultiSelect = false;
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(333, 198);
            this.UserList.TabIndex = 31;
            this.UserList.UseCompatibleStateImageBehavior = false;
            this.UserList.View = System.Windows.Forms.View.Details;
            // 
            // discordIdHeader
            // 
            this.discordIdHeader.Text = "Id";
            this.discordIdHeader.Width = 136;
            // 
            // discordNameHeader
            // 
            this.discordNameHeader.Text = "Name";
            this.discordNameHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.discordNameHeader.Width = 192;
            // 
            // Show
            // 
            this.Show.Location = new System.Drawing.Point(401, 66);
            this.Show.Name = "Show";
            this.Show.Size = new System.Drawing.Size(78, 20);
            this.Show.TabIndex = 32;
            this.Show.Text = "Show";
            this.Show.UseVisualStyleBackColor = true;
            // 
            // DiscordUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Show);
            this.Controls.Add(this.UserList);
            this.Controls.Add(this.textboxUserId);
            this.Controls.Add(this.textboxWebhookURL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Test);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.Add);
            this.Name = "DiscordUc";
            this.Size = new System.Drawing.Size(950, 536);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button Test;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textboxWebhookURL;
        private System.Windows.Forms.TextBox textboxUserId;
        private System.Windows.Forms.ListView UserList;
        private System.Windows.Forms.ColumnHeader discordIdHeader;
        private System.Windows.Forms.ColumnHeader discordNameHeader;
        private System.Windows.Forms.Button Show;
    }
}
