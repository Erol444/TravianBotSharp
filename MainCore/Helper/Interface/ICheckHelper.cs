using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;

namespace MainCore.Helper.Interface
{
    public interface ICheckHelper
    {
        int GetCurrentVillageId(int accountId);

        int[] GetResourceNeed(int accountId, BuildingEnums building, bool multiple = false);

        bool IsBanMsg(HtmlDocument doc);

        bool IsCaptcha(HtmlDocument doc);

        bool IsContextualHelp(HtmlDocument doc);

        bool IsCorrectTab(int accountId, int tab);

        bool IsCorrectVillage(int accountId, int villageId);

        bool IsFarmListPage(int accountId);

        bool IsLoginScreen(HtmlDocument doc);

        bool IsMaintanance(HtmlDocument doc);

        Result<bool> IsNeedAdsUpgrade(int accountId, int villageId, PlanTask buildingTask);

        public bool IsWWMsg(HtmlDocument doc);

        bool IsSkipTutorial(HtmlDocument doc);

        bool IsSysMsg(HtmlDocument doc);

        bool IsWWPage(IChromeBrowser chromeBrowser);
    }
}