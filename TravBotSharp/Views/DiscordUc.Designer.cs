
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
            this.BtnTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textboxWebhookURL = new System.Windows.Forms.TextBox();
            this.BtnShow = new System.Windows.Forms.Button();
            this.UseDiscordAlert = new System.Windows.Forms.CheckBox();
            this.onlineAnnouncement = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // BtnTest
            // 
            this.BtnTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.BtnTest.Location = new System.Drawing.Point(125, 121);
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
            this.label1.Location = new System.Drawing.Point(40, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Webhook URL:";
            // 
            // textboxWebhookURL
            // 
            this.textboxWebhookURL.Cursor = System.Windows.Forms.Cursors.Default;
            this.textboxWebhookURL.Location = new System.Drawing.Point(125, 95);
            this.textboxWebhookURL.Name = "textboxWebhookURL";
            this.textboxWebhookURL.PasswordChar = '*';
            this.textboxWebhookURL.Size = new System.Drawing.Size(249, 20);
            this.textboxWebhookURL.TabIndex = 9;
            // 
            // BtnShow
            // 
            this.BtnShow.Cursor = System.Windows.Forms.Cursors.Default;
            this.BtnShow.Location = new System.Drawing.Point(380, 95);
            this.BtnShow.Name = "BtnShow";
            this.BtnShow.Size = new System.Drawing.Size(78, 20);
            this.BtnShow.TabIndex = 32;
            this.BtnShow.Text = "Show";
            this.BtnShow.UseVisualStyleBackColor = true;
            this.BtnShow.Click += new System.EventHandler(this.BtnShow_Click);
            // 
            // UseDiscordAlert
            // 
            this.UseDiscordAlert.AutoSize = true;
            this.UseDiscordAlert.Location = new System.Drawing.Point(128, 62);
            this.UseDiscordAlert.Name = "UseDiscordAlert";
            this.UseDiscordAlert.Size = new System.Drawing.Size(108, 17);
            this.UseDiscordAlert.TabIndex = 40;
            this.UseDiscordAlert.Text = "Use Discord Alert";
            this.UseDiscordAlert.UseVisualStyleBackColor = true;
            this.UseDiscordAlert.CheckedChanged += new System.EventHandler(this.UseDiscordAlert_CheckedChanged);
            // 
            // onlineAnnouncement
            // 
            this.onlineAnnouncement.AutoSize = true;
            this.onlineAnnouncement.Location = new System.Drawing.Point(266, 62);
            this.onlineAnnouncement.Name = "onlineAnnouncement";
            this.onlineAnnouncement.Size = new System.Drawing.Size(187, 17);
            this.onlineAnnouncement.TabIndex = 41;
            this.onlineAnnouncement.Text = "Make announcement when online";
            this.onlineAnnouncement.UseVisualStyleBackColor = true;
            this.onlineAnnouncement.CheckedChanged += new System.EventHandler(this.onlineAnnouncement_CheckedChanged);
            // 
            // DiscordUc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.onlineAnnouncement);
            this.Controls.Add(this.UseDiscordAlert);
            this.Controls.Add(this.BtnShow);
            this.Controls.Add(this.textboxWebhookURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnTest);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "DiscordUc";
            this.Size = new System.Drawing.Size(950, 536);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textboxWebhookURL;
        private System.Windows.Forms.Button BtnShow;
        private System.Windows.Forms.CheckBox UseDiscordAlert;
        private System.Windows.Forms.CheckBox onlineAnnouncement;
    }
}
