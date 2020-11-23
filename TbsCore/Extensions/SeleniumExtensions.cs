using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.Extensions
{
    internal static class SeleniumExtensions
    {
        internal static async Task Write(this IWebElement element, object text) => await Write(element, text.ToString());
        /// <summary>
        /// Writes text into the element, one character at a time (to avoid Travian's bot-protection system)
        /// </summary>
        /// <param name="element">Element to write into</param>
        /// <param name="text">Text to be written</param>
        internal static async Task Write(this IWebElement element, string text)
        {
            Random ran = new Random();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                // Await 150 - 350 ms between each char
                await Task.Delay(ran.Next(150, 350));
            }
        }

        /// <summary>
        /// Moves mouse to the elements, clicks it and reloads HTML
        /// </summary>
        /// <param name="element">Element to be clicked</param>
        /// <param name="acc">Account</param>
        internal static async Task Click(this IWebElement element, Account acc)
        {
            var actions = new Actions(acc.Wb.Driver);

            actions.MoveToElement(element)
                .Click()
                .Build()
                .Perform();

            await Task.Delay(AccountHelper.Delay() * 2);
            acc.Wb.Html.LoadHtml(acc.Wb.Driver.PageSource);
        }

        //internal static async Task Click(this IWebElement element, Account acc)
        //{
        //    var actions = new Actions(acc.Wb.Driver);

        //    (await actions.MoveToElement(element)
        //        .Wait())
        //        .Build()
        //        .Perform();
        //}

        //private static async Task<Actions> Wait(this Actions actions, int delay = 0)
        //{
        //    Random ran = new Random();
        //    if (delay == 0) delay = ran.Next(500, 1000);

        //    await Task.Delay(delay);
        //    return actions;
        //}
    }
}
