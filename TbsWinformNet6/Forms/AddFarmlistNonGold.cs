using System.Windows.Forms;

namespace TbsWinformNet6.Views
{
    public partial class AddNewFarmListNameForm : Form
    {
        public AddNewFarmListNameForm()
        {
            InitializeComponent();
        }

        public string getName()
        {
            return textBox1.Text;
        }
    }
}