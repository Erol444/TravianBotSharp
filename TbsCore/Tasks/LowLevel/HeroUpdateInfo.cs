﻿using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroUpdateInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await HeroHelper.NavigateToHeroAttributes(acc);

            HeroHelper.ParseHeroPage(acc);

            TaskExecutor.RemoveSameTasks(acc, typeof(HeroUpdateInfo), this);

            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                var ran = new Random();

                NextExecute = DateTime.Now.AddMinutes(
                    ran.Next(acc.Hero.Settings.MinUpdate, acc.Hero.Settings.MaxUpdate)
                );
                TaskExecutor.RemoveSameTasks(acc, this);
            }

            return TaskRes.Executed;
        }
    }
}