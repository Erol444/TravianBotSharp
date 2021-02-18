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
        }
    }
}