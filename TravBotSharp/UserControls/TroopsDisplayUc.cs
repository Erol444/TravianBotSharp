using System.Drawing;
using System.Windows.Forms;
using TbsCore.Helpers;

namespace TravBotSharp.Forms
{
    public partial class TroopsDisplayUc : UserControl
    {
        private PictureBox[] pictureBoxes;

        public TroopsDisplayUc()
        {
            InitializeComponent();

            pictureBoxes = new PictureBox[]
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5,
                pictureBox6,
                pictureBox7,
                pictureBox8,
                pictureBox9,
                pictureBox10,
                pictureBox11,
            };
        }

        public void Init(Classificator.TribeEnum tribe)
        {
            // Nature by default
            Bitmap tribeGif = FormsResources.Nature;

            switch (tribe)
            {
                case Classificator.TribeEnum.Romans:
                    tribeGif = FormsResources.Romans;
                    break;

                case Classificator.TribeEnum.Teutons:
                    tribeGif = FormsResources.Teutons;
                    break;

                case Classificator.TribeEnum.Gauls:
                    tribeGif = FormsResources.Gauls;
                    break;
                //case Classificator.TribeEnum.Nature:
                //    tribeGif = FormsResources.Nature;
                //    break;
                case Classificator.TribeEnum.Natars:
                    tribeGif = FormsResources.Natars;
                    break;

                case Classificator.TribeEnum.Egyptians:
                    tribeGif = FormsResources.Egyptians;
                    break;

                case Classificator.TribeEnum.Huns:
                    tribeGif = FormsResources.Huns;
                    break;
            }

            for (int i = 0; i < 10; i++)
            {
                pictureBoxes[i].Image = tribeGif.Clone(
                    new Rectangle(i * 19, 0, 16, 16),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                    );
            }

            // Hero display
            pictureBoxes[10].Image = FormsResources.Specials.Clone(
                    new Rectangle(2 * 19, 0, 16, 16),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                    );
        }
    }
}