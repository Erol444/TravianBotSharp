using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravBotSharp.UserControls
{
    public partial class ResourceDisplayUc : UserControl
    {
        public ResourceDisplayUc()
        {
            InitializeComponent();

            var img = FormsResources.resource;
            pictureBox1.Image = img.Clone(
                    new Rectangle(0, 0, 18, 18),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

            pictureBox2.Image = img.Clone(
                    new Rectangle(0, 25, 18, 18),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

            pictureBox3.Image = img.Clone(
                    new Rectangle(0, 52, 18, 18),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

            pictureBox4.Image = img.Clone(
                    new Rectangle(0, 77, 18, 18),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );
        }
    }
}