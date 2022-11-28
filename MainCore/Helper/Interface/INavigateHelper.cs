using FluentResults;
using MainCore.Services.Interface;
using OpenQA.Selenium;

namespace MainCore.Helper.Interface
{
    public interface INavigateHelper
    {
        Result AfterClicking(int accountId);
        Result Click(int accountId, IWebElement element);
        int GetDelayClick(int accountId);
        Result GoRandomDorf(int accountId);
        Result GoToBuilding(int accountId, int index);
        void Sleep(int min, int max);
        Result SwitchTab(int accountId, int index);
        Result SwitchVillage(int accountId, int villageId);
        Result ToAdventure(IChromeBrowser chromeBrowser, AppDbContext context, int accountId);
        Result ToDorf1(int accountId, bool isForce = false);
        Result ToDorf2(int accountId, bool isForce = false);
        Result ToHeroInventory(IChromeBrowser chromeBrowser, AppDbContext context, int accountId);
        Result WaitPageChanged(int accountId, string path);
        Result WaitPageLoaded(int accountId);
    }
}