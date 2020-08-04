using System;
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
                if(i == 0)
                {
                    textBox2.Text = access.Password;
                    textBox3.Text = access.Proxy;
                    numericUpDown1.Value = access.ProxyPort;
                }

                var item = new ListViewItem();
                item.SubItems[0].Text = access.Password;
                item.SubItems.Add(access.Proxy);
                item.SubItems.Add(access.ProxyPort.ToString());

                accessListView.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Add a new access
        {
            var input = GetAccessInput();

            Acc.Access.AddNewAccess(input.Password, input.Proxy, input.ProxyPort);
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
                if(indicies[0] < this.Acc.Access.AllAccess.Count)
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
            access.ProxyPort= input.ProxyPort;
            UpdateWindow();
        }
        private AccessRaw GetAccessInput()
        {
            AccessRaw accessRaw = new AccessRaw
            {
                Password = textBox2.Text,
                Proxy = textBox3.Text,
                ProxyPort = (int)numericUpDown1.Value
            };
            return accessRaw;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Acc.AccInfo.Nickname = textBox1.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Acc.AccInfo.ServerUrl = textBox4.Text;
            if (!Acc.AccInfo.ServerUrl.Contains("https://") &&
                !Acc.AccInfo.ServerUrl.Contains("http://"))
            {
                //add https:// if user forgot
                Acc.AccInfo.ServerUrl = "https://" + Acc.AccInfo.ServerUrl;
            }
        }

        /// <summary>
        /// When a user selects an access from the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var access = GetCurrentAccess();
            textBox2.Text = access.Password;
            textBox3.Text = access.Proxy;
            numericUpDown1.Value = access.ProxyPort;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}