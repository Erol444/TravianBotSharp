using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;

namespace TbsCore.Helpers
{
    public static class DriverHelper
    {
        /// <summary>
        /// Executes JS, waits for changes and re-loads the html
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="script">JavaScript</param>
        /// <param name="log">Log exception if it happens</param>
        /// <returns>Whether the execution was successful</returns>
        public static async Task<bool> ExecuteScript(Account acc, string script, bool log = true, bool update = true)
        {
            try
            {
                acc.Wb.ExecuteScript(script);
                if (update)
                {
                    await Task.Delay(AccountHelper.Delay(acc));
                    acc.Wb.UpdateHtml();
                }
                return true;
            }
            catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
            {
                throw e;
            }
            catch (Exception e)
            {
                if (log) acc.Logger.Error(e, $"Error executing JS script:\n{script}");
                return false;
            }
        }

        /// <summary>
        /// Gets JS object from the game. Query examples:
        /// window.TravianDefaults.Map.Size.top
        /// resources.maxStorage
        /// Travian.Game.speed
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="obj">JS object</param>
        /// <returns>Long for number, bool for boolean, string otherwise</returns>
        public static T GetJsObj<T>(Account acc, string obj, bool log = true)
        {
            try
            {
                return acc.Wb.GetJsObj<T>(obj);
            }
            catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
            {
                throw e;
            }
            catch (Exception e)
            {
                if (log) acc.Logger.Error(e, $"Error getting JS object '{obj}'!");
                return default;
            }
        }

        /// <summary>
        /// Get bearer token for Travian T4.5
        /// </summary>
        public static string GetBearerToken(Account acc, bool log = true)
        {
            try
            {
                return acc.Wb.GetBearerToken();
            }
            catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
            {
                throw e;
            }
            catch (Exception e)
            {
                if (log) acc.Logger.Error(e, "Error getting BearerToken!");
                return default;
            }
        }

        /// <summary>
        /// Write coordinates into the number inputs. Used when sending troops, resources etc.
        /// If coordinates are already there (embedded in url), skip this task.
        /// </summary>
        internal static async Task WriteCoordinates(Account acc, Coordinates coordinates)
        {
            if (string.IsNullOrEmpty(acc.Wb.Html.GetElementbyId("xCoordInput").GetAttributeValue("value", "")))
                await WriteById(acc, "xCoordInput", coordinates.x);
            if (string.IsNullOrEmpty(acc.Wb.Html.GetElementbyId("yCoordInput").GetAttributeValue("value", "")))
                await WriteById(acc, "yCoordInput", coordinates.y);
        }

        #region By Id

        public static async Task<bool> ClickById(Account acc, string query, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionClick(), log);

        public static async Task<bool> WriteById(Account acc, string query, object text, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionWrite(text), log);

        public static async Task<bool> CheckById(Account acc, string query, bool check, bool log = true, bool update = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionCheck(check), log, update);

        public static async Task<bool> SelectIndexById(Account acc, string query, int index, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionSelectIndex(index), log);

        #endregion By Id

        #region By Class Name

        public static async Task<bool> ClickByClassName(Account acc, string query, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query, qindex), new ActionClick(), log);

        public static async Task<bool> WriteByClassName(Account acc, string query, object text, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query, qindex), new ActionWrite(text), log);

        public static async Task<bool> CheckByClassName(Account acc, string query, bool check, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query, qindex), new ActionCheck(check), log);

        public static async Task<bool> SelectIndexByClassName(Account acc, string query, int index, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query, qindex), new ActionSelectIndex(index), log);

        #endregion By Class Name

        #region By Name

        public static async Task<bool> ClickByName(Account acc, string query, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query, qindex), new ActionClick(), log);

        public static async Task<bool> WriteByName(Account acc, string query, object text, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query, qindex), new ActionWrite(text), log);

        public static async Task<bool> CheckByName(Account acc, string query, bool check, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query, qindex), new ActionCheck(check), log);

        public static async Task<bool> SelectIndexByName(Account acc, string query, int index, int qindex = 0, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query, qindex), new ActionSelectIndex(index), log);

        #endregion By Name

        #region By Attribute Value

        public static async Task<bool> ClickByAttributeValue(Account acc, string attribute, string value, bool log = true) =>
            await ExecuteAction(acc, new QueryByAttributeVal(attribute, value), new ActionClick(), log);

        public static async Task<bool> WriteByAttributeValue(Account acc, string attribute, string value, object text, bool log = true, bool update = true) =>
            await ExecuteAction(acc, new QueryByAttributeVal(attribute, value), new ActionWrite(text), log, update);

        public static async Task<bool> CheckByAttributeValue(Account acc, string attribute, string value, bool check, bool log = true) =>
            await ExecuteAction(acc, new QueryByAttributeVal(attribute, value), new ActionCheck(check), log);

        public static async Task<bool> SelectByAttributeValue(Account acc, string attribute, string value, int index, bool log = true) =>
            await ExecuteAction(acc, new QueryByAttributeVal(attribute, value), new ActionSelectIndex(index), log);

        #endregion By Attribute Value

        private static async Task<bool> ExecuteAction(Account acc, Query query, Action action, bool log = true, bool update = true) =>
            await ExecuteScript(acc, $"document.{query.val}{action.val}", log, update);

        public class QueryById : Query

        { public QueryById(string str) => base.val = $"getElementById('{str}')"; }

        public class QueryByName : Query

        { public QueryByName(string str, int index = 0) => base.val = $"getElementsByName('{str}')[{index}]"; }

        public class QueryByClassName : Query

        { public QueryByClassName(string str, int index = 0) => base.val = $"getElementsByClassName('{str}')[{index}]"; }

        public class QueryByAttributeVal : Query

        { public QueryByAttributeVal(string attribute, string value) => base.val = $"querySelectorAll('[{attribute}=\"{value}\"]')[0]"; }

        public class ActionWrite : Action

        { public ActionWrite(object str) => base.val = $".value='{str}';"; }

        public class ActionClick : Action

        { public ActionClick() => base.val = ".click();"; }

        public class ActionCheck : Action

        { public ActionCheck(bool check) => base.val = $".checked={(check ? "true" : "false")};"; }

        public class ActionSelectIndex : Action

        { public ActionSelectIndex(int index) => base.val = $".selectedIndex = {index};"; }

        public abstract class Action
        { public string val; }

        public abstract class Query
        { public string val; }

        public static async Task WaitPageLoaded(Account acc, double delay = 1)
        {
            var wait = new WebDriverWait(acc.Wb.Driver, TimeSpan.FromMinutes(delay));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            acc.Wb.UpdateHtml();
            await TaskExecutor.PageLoaded(acc);
        }

        public static async Task WaitPageChange(Account acc, string part, double delay = 1)
        {
            var wait = new WebDriverWait(acc.Wb.Driver, TimeSpan.FromMinutes(delay));
            wait.Until(driver => driver.Url.Contains(part));
            await WaitPageLoaded(acc, delay);
        }

        public static async Task ReopenChrome(Account acc)
        {
            acc.Wb.Close();

            await acc.Wb.Init(acc, false);
        }
    }
}