using FluentResults;
using OpenQA.Selenium;
using System;

namespace MainCore.Helper.Interface
{
    public interface IGeneralHelper
    {
        Result Click(int accountId, By by, bool waitPageLoaded = true);

        Result Click(int accountId, By by, string path);

        void DelayClick(int accountId);

        int GetDelayClick(int accountId);

        Result Input(int accountId, By by, string content);

        Result Navigate(int accountId, string url);

        Result Reload(int accountId);

        Result SwitchTab(int accountId, int index);

        Result SwitchVillage(int accountId, int villageId);

        Result ToBothDorf(int accountId, int villageId);

        Result ToBothDorf(int accountId);

        Result ToBuilding(int accountId, int index);

        Result ToDorf(int accountId, bool forceReload = false);

        Result ToDorf(int accountId, int villageId, bool forceReload = false);

        Result ToDorf1(int accountId, bool forceReload = false);

        Result ToDorf2(int accountId, bool forceReload = false);

        Result ToHeroInventory(int accountId);

        Result Wait(int accountId, Func<IWebDriver, bool> condition);

        Result WaitPageChanged(int accountId, string path);

        Result WaitPageLoaded(int accountId);
    }
}