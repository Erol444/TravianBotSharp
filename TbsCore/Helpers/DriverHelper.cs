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
            catch(Exception e) 
            {
                if (log) acc.Wb.Log($"Error executing JS script:\n{script}", e);
                return false;
            }
        }
    }
}
