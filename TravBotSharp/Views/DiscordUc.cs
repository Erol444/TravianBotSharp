using System.Windows.Forms;
using System.Drawing;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class DiscordUc : TbsBaseUc, ITbsUc
    {
        public DiscordUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            BtnAdd.Enabled = false;
            BtnDelete.Enabled = false;
            UserList.Enabled = false;
        }

        private void BtnShow_Click(object sender, System.EventArgs e)
        {
            if (textboxWebhookURL.PasswordChar.Equals('*'))
            {
                textboxWebhookURL.PasswordChar = '\0';
                BtnShow.Text = "Hide";
            }
            else
            {
                textboxWebhookURL.PasswordChar = '*';
                BtnShow.Text = "Show";
            }
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            var item = new ListViewItem();
            item.SubItems[0].Text = textboxUserId.Text;
            item.ForeColor = Color.White;
            UserList.Items.Add(item);

            textboxUserId.Text = "";
        }

        private void BtnDelete_Click(object sender, System.EventArgs e)
        {
            if (UserList.SelectedItems.Count < 1) return;

            UserList.Items.RemoveAt(UserList.SelectedItems[0].Index);

            textboxUserId.Text = "";
        }

        private void UserList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (UserList.SelectedItems.Count < 1) return;

            textboxUserId.Text = UserList.Items[UserList.SelectedItems[0].Index].Text;
        }

        private void DiscordUserList_CheckedChanged(object sender, System.EventArgs e)
        {
            if (DiscordUserList.Checked)
            {
                BtnAdd.Enabled = true;
                BtnDelete.Enabled = true;
                UserList.Enabled = true;
            }
            else
            {
                BtnAdd.Enabled = false;
                BtnDelete.Enabled = false;
                UserList.Enabled = false;
            }
        }
    }
}