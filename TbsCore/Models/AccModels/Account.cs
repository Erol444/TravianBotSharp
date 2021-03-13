using Newtonsoft.Json;
using System.Collections.Generic;
using Discord.Webhook;
using TbsCore.Models.Access;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;

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
            Tasks = new List<BotTask>();
            Villages = new List<Village>();
            Access = new AccessInfo();
            Access.Init();
            AccInfo = new AccInfo();
            AccInfo.Init();
            Quests = new QuestsSettings();
            Quests.Init();
            Settings = new GeneralSettings();
            Settings.Init();
            Farming = new Farming();
            NewVillages = new NewVillageSettings();
            NewVillages.Init();
        }

        public AccInfo AccInfo { get; set; }
        public AccessInfo Access { get; set; }
        public List<Village> Villages { get; set; }
        public Farming Farming { get; set; }
        public Hero Hero { get; set; }
        public QuestsSettings Quests { get; set; }
        public NewVillageSettings NewVillages { get; set; }
        public GeneralSettings Settings { get; set; }

        [JsonIgnore]
        public WebBrowserInfo Wb { get; set; }

        [JsonIgnore]
        public List<BotTask> Tasks { get; set; }

        [JsonIgnore]
        public TaskTimer TaskTimer { get; set; }

        [JsonIgnore]
        public DiscordWebhookClient WebhookClient { get; set; }
    }
}