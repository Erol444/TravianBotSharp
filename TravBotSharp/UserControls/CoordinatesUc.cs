using System.Windows.Forms;
using TbsCore.Models.MapModels;

namespace TravBotSharp.UserControls
{
    public partial class CoordinatesUc : UserControl
    {
        public CoordinatesUc()
        {
            InitializeComponent();
        }

        public Coordinates Coords
        {
            get
            {
                var x = (int) X.Value;
                var y = (int) Y.Value;
                return new Coordinates(x, y);
            }
            set
            {
                X.Value = value.x;
                Y.Value = value.y;
            }
        }
    }
}