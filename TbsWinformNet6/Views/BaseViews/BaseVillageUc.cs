using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsWinformNet6.Views;

namespace TbsWinformNet6.Views.BaseViews
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