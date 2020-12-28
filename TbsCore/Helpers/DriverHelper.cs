using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
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
        public static async Task<bool> ExecuteScript(Account acc, string script, bool log = true)
        {
            try
            {
                acc.Wb.Driver.ExecuteScript(script);
                await Task.Delay(AccountHelper.Delay() * 2);
                acc.Wb.Html.LoadHtml(acc.Wb.Driver.PageSource);
                return true;
            }
            catch (Exception e)
            {
                if (log) acc.Wb.Log($"Error executing JS script:\n{script}", e);
                return false;
            }
        }

        public static async Task<bool> ClickById(Account acc, string query) =>
            await ExecuteAction(acc, new QueryById(query), new ActionClick());
        public static async Task<bool> WriteById(Account acc, string query, object text) =>
            await ExecuteAction(acc, new QueryById(query), new ActionWrite(text));
        public static async Task<bool> ClickByClassName(Account acc, string query) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionClick());
        public static async Task<bool> WriteByClassName(Account acc, string query, object text) =>
            await ExecuteAction(acc, new QueryByClassName(query), new ActionWrite(text));
        public static async Task<bool> ClickByName(Account acc, string query) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionClick());
        public static async Task<bool> WriteByName(Account acc, string query, object text) =>
            await ExecuteAction(acc, new QueryByName(query), new ActionWrite(text));

        private static async Task<bool> ExecuteAction(Account acc, Query query, Action action, bool log = true) =>
            await ExecuteScript(acc, $"document.{query.val}{action.val}", log);

        
        public class QueryById : Query { public QueryById(string str) => base.val = $"getElementById('{str}')"; }
        public class QueryByName : Query { public QueryByName(string str) => base.val = $"getElementsByName('{str}')[0]"; }
        public class QueryByClassName : Query { public QueryByClassName(string str) => base.val = $"getElementsByClassName('{str}')[0]"; }
        public class ActionWrite : Action { public ActionWrite(object str) => base.val = $".value='{str}';"; }
        public class ActionClick : Action { public ActionClick() => base.val = ".click();"; }

        public abstract class Action { public string val; }
        public abstract class Query { public string val; }
    }
}
