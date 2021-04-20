using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;

namespace TravBotSharp.Views
{
    /// <summary>
    ///     Base class for TBS village views
    /// </summary>
    public class BaseVillageUc : UserControl
    {
        private VillagesUc villageUc;

        public void Init(object villageUc)
        {
            this.villageUc = (VillagesUc) villageUc;

            // Init TbsBaseUc
        }

        public Village GetSelectedVillage(Account acc = null)
        {
            return villageUc?.GetSelectedVillage(acc);
        }

        public Account GetSelectedAcc()
        {
            return villageUc?.GetSelectedAcc();
        }
    }
}