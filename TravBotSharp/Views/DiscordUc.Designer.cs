
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
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textboxWebhookURL = new System.Windows.Forms.TextBox();
            this.textboxUserId = new System.Windows.Forms.TextBox();
            this.UserList = new System.Windows.Forms.ListView();
            this.discordIdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.discordNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BtnShow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.DiscordAll = new System.Windows.Forms.RadioButton();
            this.DiscordHere = new System.Windows.Forms.RadioButton();
            this.DiscordUserList = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.UseDiscordAlert = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnAdd
            // 
            this.BtnAdd.Location = new System.Drawing.Point(172, 1);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(78, 20);
            this.BtnAdd.TabIndex = 2;
            this.BtnAdd.Text = "Add";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(255, 1);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(78, 20);
            this.BtnDelete.TabIndex = 3;
            this.BtnDelete.Text = "Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnTest
            // 
            this.BtnTest.Location = new System.Drawing.Point(146, 355);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(333, 40);
            this.BtnTest.TabIndex = 4;
            this.BtnTest.Text = "TEST";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(this.BtnTest_Click);
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
            this.label2.Location = new System.Drawing.Point(97, 125);
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
            this.textboxUserId.Location = new System.Drawing.Point(1, 1);
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
            this.UserList.Location = new System.Drawing.Point(1, 27);
            this.UserList.MultiSelect = false;
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(333, 198);
            this.UserList.TabIndex = 31;
            this.UserList.UseCompatibleStateImageBehavior = false;
            this.UserList.View = System.Windows.Forms.View.Details;
            this.UserList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.UserList_ItemSelectionChanged);
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
            // BtnShow
            // 
            this.BtnShow.Location = new System.Drawing.Point(401, 66);
            this.BtnShow.Name = "BtnShow";
            this.BtnShow.Size = new System.Drawing.Size(78, 20);
            this.BtnShow.TabIndex = 32;
            this.BtnShow.Text = "Show";
            this.BtnShow.UseVisualStyleBackColor = true;
            this.BtnShow.Click += new System.EventHandler(this.BtnShow_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Who got ping:";
            // 
            // DiscordAll
            // 
            this.DiscordAll.AutoSize = true;
            this.DiscordAll.Checked = true;
            this.DiscordAll.Location = new System.Drawing.Point(5, 3);
            this.DiscordAll.Name = "DiscordAll";
            this.DiscordAll.Size = new System.Drawing.Size(36, 17);
            this.DiscordAll.TabIndex = 35;
            this.DiscordAll.TabStop = true;
            this.DiscordAll.Text = "All";
            this.DiscordAll.UseVisualStyleBackColor = true;
            // 
            // DiscordHere
            // 
            this.DiscordHere.AutoSize = true;
            this.DiscordHere.Location = new System.Drawing.Point(98, 3);
            this.DiscordHere.Name = "DiscordHere";
            this.DiscordHere.Size = new System.Drawing.Size(48, 17);
            this.DiscordHere.TabIndex = 36;
            this.DiscordHere.TabStop = true;
            this.DiscordHere.Text = "Here";
            this.DiscordHere.UseVisualStyleBackColor = true;
            // 
            // DiscordUserList
            // 
            this.DiscordUserList.AutoSize = true;
            this.DiscordUserList.Location = new System.Drawing.Point(184, 3);
            this.DiscordUserList.Name = "DiscordUserList";
            this.DiscordUserList.Size = new System.Drawing.Size(62, 17);
            this.DiscordUserList.TabIndex = 37;
            this.DiscordUserList.TabStop = true;
            this.DiscordUserList.Text = "User list";
            this.DiscordUserList.UseVisualStyleBackColor = true;
            this.DiscordUserList.CheckedChanged += new System.EventHandler(this.DiscordUserList_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DiscordAll);
            this.panel1.Controls.Add(this.DiscordUserList);
            this.panel1.Controls.Add(this.DiscordHere);
            this.panel1.Location = new System.Drawing.Point(146, 93);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 23);
            this.panel1.TabIndex = 38;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textboxUserId);
            this.panel2.Controls.Add(this.BtnAdd);
            this.panel2.Controls.Add(this.BtnDelete);
            this.panel2.Controls.Add(this.UserList);
            this.panel2.Location = new System.Drawing.Point(145, 125);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(333, 231);
            this.panel2.TabIndex = 39;
            // 
            // UseDiscordAlert
            // 
            this.UseDiscordAlert.AutoSize = true;
            this.UseDiscordAlert.Location = new System.Drawing.Point(149, 33);
            this.UseDiscordAlert.Name = "UseDiscordAlert";
            this.UseDiscordAlert.Size = new System.Drawing.Size(108, 17);
            this.UseDiscordAlert.TabIndex = 40;
            this.UseDiscordAlert.Text = "Use Discord Alert";
            this.UseDiscordAlert.UseVisualStyleBackColor = true;
            this.UseDiscordAlert.CheckedChanged += new System.EventHandler(this.UseDiscordAlert_CheckedChanged);
            // 
            // DiscordUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UseDiscordAlert);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnShow);
            this.Controls.Add(this.textboxWebhookURL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnTest);
            this.Name = "DiscordUc";
            this.Size = new System.Drawing.Size(950, 536);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textboxWebhookURL;
        private System.Windows.Forms.TextBox textboxUserId;
        private System.Windows.Forms.ListView UserList;
        private System.Windows.Forms.ColumnHeader discordIdHeader;
        private System.Windows.Forms.ColumnHeader discordNameHeader;
        private System.Windows.Forms.Button BtnShow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton DiscordAll;
        private System.Windows.Forms.RadioButton DiscordHere;
        private System.Windows.Forms.RadioButton DiscordUserList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox UseDiscordAlert;
    }
}
