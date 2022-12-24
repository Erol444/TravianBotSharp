using HtmlAgilityPack;
using MainCore.Services.Interface;

namespace MainCore.Helper.Interface
{
    public interface ICheckHelper
    {
        int GetCurrentVillageId(int accountId);

        bool IsBanMsg(HtmlDocument doc);

        bool IsCaptcha(HtmlDocument doc);

        bool IsContextualHelp(HtmlDocument doc);

        bool IsCorrectTab(int accountId, int tab);

        bool IsCorrectVillage(int accountId, int villageId);

        bool IsFarmListPage(int accountId);

        bool IsLoginScreen(HtmlDocument doc);

        bool IsMaintanance(HtmlDocument doc);

        public bool IsWWMsg(HtmlDocument doc);

        bool IsSkipTutorial(HtmlDocument doc);

        bool IsSysMsg(HtmlDocument doc);

        bool IsWWPage(IChromeBrowser chromeBrowser);
    }
}