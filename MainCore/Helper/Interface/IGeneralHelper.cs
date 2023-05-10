using FluentResults;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IGeneralHelper
    {
        Result Click(By by, bool waitPageLoaded = true);

        Result Click(By by, string path);

        Result Input(By by, string content);

        void DelayClick();

        int GetDelayClick();

        bool IsPageValid();

        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        Result Reload();

        Result Navigate(string url);

        Result SwitchTab(int index);

        Result SwitchVillage();

        Result ToAdventure();

        Result ToBuilding(int index);

        Result ToDorf(bool forceReload = false);

        Result ToDorf1(bool forceReload = false);

        Result ToDorf2(bool forceReload = false);

        Result ToHeroInventory();

        Result Wait(Func<IWebDriver, bool> condition);

        Result WaitPageChanged(string path);

        Result WaitPageLoaded();

        Result ClickStartAdventure(int x, int y);

        Result ClickStartFarm(int farmId);
    }
}