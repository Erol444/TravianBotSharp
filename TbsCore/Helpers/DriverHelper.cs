using OpenQA.Selenium;
using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TravBotSharp.Files.Helpers;

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
                acc.Wb.Driver.ExecuteScript(script);
                if (update)
                {
                    await Task.Delay(AccountHelper.Delay());
                    acc.Wb.UpdateHtml();
                }
                return true;
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
                IJavaScriptExecutor js = acc.Wb.Driver as IJavaScriptExecutor;
                return (T)js.ExecuteScript($"return {obj};");
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
        public static string GetBearerToken(Account acc)
        {
            IJavaScriptExecutor js = acc.Wb.Driver as IJavaScriptExecutor;
            return (string)js.ExecuteScript("for(let field in Travian) { if (Travian[field].length == 32) return Travian[field]; }");
        }

        /// <summary>
        /// Write coordinates into the number inputs. Used when sending troops, resources etc.
        /// </summary>
        internal static async Task WriteCoordinates(Account acc, Coordinates coordinates)
        {
            await WriteById(acc, "xCoordInput", coordinates.x);
            await WriteById(acc, "yCoordInput", coordinates.y);
        }

        public static async Task<bool> ClickById(Account acc, string query, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionClick(), log);

        public static async Task<bool> WriteById(Account acc, string query, object text, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionWrite(text), log);

        public static async Task<bool> CheckById(Account acc, string query, bool check, bool log = true, bool update = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionCheck(check), log, update);

        public static async Task<bool> SelectIndexById(Account acc, string query, int index, bool log = true) =>
            await ExecuteAction(acc, new QueryById(query), new ActionSelectIndex(index), log);

        public static async Task<bool> ClickByClassName(Account acc, string query, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionClick(), log);

        public static async Task<bool> WriteByClassName(Account acc, string query, object text, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionWrite(text), log);

        public static async Task<bool> CheckByClassName(Account acc, string query, bool check, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionCheck(check), log);

        public static async Task<bool> SelectIndexByClassName(Account acc, string query, int index, bool log = true) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionSelectIndex(index), log);

        public static async Task<bool> ClickByName(Account acc, string query, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionClick(), log);

        public static async Task<bool> WriteByName(Account acc, string query, object text, bool log = true, bool update = true) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionWrite(text), log, update);

        public static async Task<bool> CheckByName(Account acc, string query, bool check, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionCheck(check), log);

        public static async Task<bool> SelectIndexByName(Account acc, string query, int index, bool log = true) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionSelectIndex(index), log);

        private static async Task<bool> ExecuteAction(Account acc, Query query, Action action, bool log = true, bool update = true) =>
            await ExecuteScript(acc, $"document.{query.val}{action.val}", log, update);

        public class QueryById : Query { public QueryById(string str) => base.val = $"getElementById('{str}')"; }

        public class QueryByName : Query { public QueryByName(string str) => base.val = $"getElementsByName('{str}')[0]"; }

        public class QueryByClassName : Query { public QueryByClassName(string str) => base.val = $"getElementsByClassName('{str}')[0]"; }

        public class ActionWrite : Action { public ActionWrite(object str) => base.val = $".value='{str}';"; }

        public class ActionClick : Action { public ActionClick() => base.val = ".click();"; }

        public class ActionCheck : Action { public ActionCheck(bool check) => base.val = $".checked={(check ? "true" : "false")};"; }

        public class ActionSelectIndex : Action { public ActionSelectIndex(int index) => base.val = $".selectedIndex = {index};"; }

        public abstract class Action { public string val; }

        public abstract class Query { public string val; }
    }
}