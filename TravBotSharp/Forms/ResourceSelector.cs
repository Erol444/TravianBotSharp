using System.Windows.Forms;

using TbsCore.Models.ResourceModels;

namespace TravBotSharp.Forms
{
    public partial class ResourceSelector : Form
    {
        private Resources resources;

        public ResourceSelector(Resources res)
        {
            InitializeComponent();
            Resources = res;
        }

        public Resources Resources
        {
            get
            {
                return resources;
            }
            set
            {
                if (value != null)
                {
                    resources = value;
                    resourceSelectorUc1.resources = resources;
                }
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}