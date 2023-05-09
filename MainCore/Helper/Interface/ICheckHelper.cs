using HtmlAgilityPack;
using MainCore.Services.Interface;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ICheckHelper
    {
        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        int GetCurrentVillageId();

        bool IsBanMsg(HtmlDocument doc);

        bool IsCaptcha(HtmlDocument doc);

        bool IsContextualHelp(HtmlDocument doc);

        bool IsCorrectTab(int tab);

        bool IsCorrectVillage();

        bool IsFarmListPage();

        bool IsLoginScreen(HtmlDocument doc);

        bool IsMaintanance(HtmlDocument doc);

        public bool IsWWMsg(HtmlDocument doc);

        bool IsSkipTutorial(HtmlDocument doc);

        bool IsSysMsg(HtmlDocument doc);

        bool IsWWPage(IChromeBrowser chromeBrowser);
    }
}