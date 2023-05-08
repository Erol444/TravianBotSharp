using FluentResults;
using OpenQA.Selenium;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IGeneralHelper
    {
        Result Click(By by, bool waitPageLoaded = true);

        Result Click(By by, string path);

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

        Result ToDorf();

        Result ToDorf1(bool forceReload = false);

        Result ToDorf2(bool forceReload = false);

        Result ToHeroInventory();

        Result WaitPageChanged(string path);

        Result WaitPageLoaded();
    }
}