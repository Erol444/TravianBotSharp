using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Views
{
    /// <summary>
    /// Base class for TBS views
    /// </summary>
    public class TbsBaseUc : UserControl
    {
        public ControlPanel main;
        public void Init(object main)
        {
            this.main = (ControlPanel)main;
        }
        public Account GetSelectedAcc() => main?.GetSelectedAcc();
        //public Village GetSelectedVillage(Account acc = null) => main?.GetSelectedVillage(acc);

        // This won't work since it's partial class. Designer.cs class can't override this method
        //public abstract void UpdateTab();
    }
}
