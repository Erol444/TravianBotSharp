using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Tasks;

namespace TbsCore.Extensions
{
    public static class BotTaskExtensions
    {
        /// <summary>
        /// Gets name of the task
        /// </summary>
        /// <returns>Name of the task</returns>
        public static string GetName(this BotTask task)
        {
            var type = task.GetType().ToString().Split('.');
            if (type.Length == 0) return null;
            return type[type.Length - 1];
        }

    }
}
