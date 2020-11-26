using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Views
{
    /// <summary>
    /// Base class for TBS village views
    /// </summary>
    public class BaseVillageUc : UserControl
    {
        private VillagesUc villageUc;

        public void Init(object villageUc)
        {
            this.villageUc = (VillagesUc)villageUc;

            // Init TbsBaseUc
        }
        public Village GetSelectedVillage(Account acc = null) => villageUc?.GetSelectedVillage(acc);
        public Account GetSelectedAcc() => villageUc?.GetSelectedAcc();
    }
}
