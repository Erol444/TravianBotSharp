using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.Helpers
{
    public static class DriverHelper
    {
        /// <summary>
        /// Executes JS, waits for changes and re-loads the html
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="script">JavaScript</param>
        /// <returns>/</returns>
        public static async Task ExecuteScript(Account acc, string script)
        {
            try
            {
                acc.Wb.Driver.ExecuteScript(script);
                await Task.Delay(AccountHelper.Delay() * 2);
                acc.Wb.Html.LoadHtml(acc.Wb.Driver.PageSource);
            }
            catch(Exception e) { }
        }
    }
}
