using System;
using System.Net.Security;
using System.Windows.Forms;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp
{
    public partial class AddAccount : Form
    {
        public Account Acc { get; set; }
        public AddAccount(Account acc = null)
        {
            InitializeComponent();
            if (acc == null)
            {
                this.Acc = new Account();
                this.Acc.Init();
            }
            else this.Acc = acc;
        }
        public void UpdateWindow()
        {
            textBox4.Text = Acc.AccInfo.ServerUrl;
            textBox1.Text = Acc.AccInfo.Nickname;

            accessListView.Items.Clear();
            for (int i = 0; i < Acc.Access.AllAccess.Count; i++)
            {
                var access = Acc.Access.AllAccess[i];
                if (i == 0)
                {
                    UpdateAccessView(access);
                }

                var item = new ListViewItem();
                item.SubItems[0].Text = access.Password;
                item.SubItems.Add(access.Proxy);
                item.SubItems.Add(access.ProxyPort + "");
                item.SubItems.Add(access.ProxyUsername);

                accessListView.Items.Add(item);
            }
            AuthCheckbox();
        }

        private void button2_Click(object sender, EventArgs e) // Add a new access
        {
            Acc.Access.AddNewAccess(GetAccessInput());
            UpdateWindow();
        }

        private void button1_Click(object sender, EventArgs e) // Remove from the view
        {
            Acc.Access.AllAccess.Remove(GetCurrentAccess());
            UpdateWindow();
        }

        /// <summary>
        /// Gets the current selected access by the user
        /// </summary>
        /// <returns>Access object</returns>
        private Access GetCurrentAccess()
        {
            var sel = 0;
            var indicies = accessListView.SelectedIndices;
            if (indicies.Count > 0)
            {
                if (indicies[0] < this.Acc.Access.AllAccess.Count)
                {
                    sel = indicies[0];
                }
            }

            return this.Acc.Access.AllAccess[sel];
        }

        private void button3_Click(object sender, EventArgs e) // Update access
        {
            var access = GetCurrentAccess();
            var input = GetAccessInput();
            access.Password = input.Password;
            access.Proxy = input.Proxy;
            access.ProxyPort = input.ProxyPort;
            access.ProxyPassword = input.ProxyPassword;
            access.ProxyUsername = input.ProxyUsername;
            UpdateWindow();
        }
        private AccessRaw GetAccessInput()
        {
            AccessRaw accessRaw = new AccessRaw
            {
                Password = textBox2.Text,
                Proxy = textBox3.Text.Trim(),
                ProxyPort = (int)numericUpDown1.Value,
                ProxyUsername = proxyUsername.Text.Trim(),
                ProxyPassword = proxyPassword.Text.Trim()
            };
            return accessRaw;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Acc.AccInfo.Nickname = textBox1.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = textBox4.Text.Replace("https://", "").Replace("http://", "");
            if (textBox4.Text.Contains("/"))
            {
                var str = textBox4.Text.Split('/');
                textBox4.Text = str[0];
            }
            Acc.AccInfo.ServerUrl = "https://" + textBox4.Text;
        }

        /// <summary>
        /// When a user selects an access from the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAccessView(GetCurrentAccess());

        }
        private void UpdateAccessView(Access access)
        {
            textBox2.Text = access.Password;
            textBox3.Text = access.Proxy;
            numericUpDown1.Value = access.ProxyPort;

            proxyUsername.Text = access.ProxyUsername;
            proxyPassword.Text = access.ProxyPassword;
            var authEmpty = string.IsNullOrEmpty(access.ProxyUsername);
            checkBox1.Checked = !authEmpty;
            proxyUsername.ReadOnly = authEmpty;
            proxyPassword.ReadOnly = authEmpty;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AuthCheckbox();
        }

        private void AuthCheckbox()
        {
            var check = checkBox1.Checked;
            if (!check)
            {
                proxyUsername.Text = "";
                proxyPassword.Text = "";
            }
            proxyUsername.ReadOnly = !check;
            proxyPassword.ReadOnly = !check;
        }
    }
}