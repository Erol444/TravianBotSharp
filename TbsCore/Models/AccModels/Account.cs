﻿using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Discord.Webhook;
using TbsCore.Models.Access;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCore.Models.Logging;
using TbsCore.Models.AccModels;
using TbsCore.Tasks;

using Serilog;
using TbsCore.Models.World;

namespace TbsCore.Models.AccModels
{
    public class Account
    {
        /// <summary>
        /// This method is called when new accounts is created
        /// </summary>
        public void Init()
        {
            Hero = new Hero();
            Hero.init();

            Villages = new List<Village>();
            Access = new AccessInfo();
            Access.Init();
            AccInfo = new AccInfo();
            AccInfo.Init();
            Tasks = new TaskList(AccInfo.Nickname);
            Quests = new QuestsSettings();
            Quests.Init();
            Settings = new GeneralSettings();
            Settings.Init();
            Farming = new Farming();
            NewVillages = new NewVillageSettings();
            NewVillages.Init();

            Server = new AccServerData();
            Server.Init();
        }

        public AccInfo AccInfo { get; set; }
        public AccessInfo Access { get; set; }
        public List<Village> Villages { get; set; }
        public Farming Farming { get; set; }
        public Hero Hero { get; set; }
        public QuestsSettings Quests { get; set; }
        public NewVillageSettings NewVillages { get; set; }
        public GeneralSettings Settings { get; set; }
        public AccServerData Server { get; set; }

        [JsonIgnore]
        public WebBrowserInfo Wb { get; set; }

        [JsonIgnore]
        public TaskList Tasks;

        [JsonIgnore]
        public TaskTimer TaskTimer { get; set; }

        [JsonIgnore]
        public DiscordWebhookClient WebhookClient { get; set; }

        [JsonIgnore]
        public Logger Logger;
    }
}