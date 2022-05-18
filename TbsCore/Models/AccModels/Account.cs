using Discord.Webhook;
using Newtonsoft.Json;
using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.Models.Access;
using TbsCore.Models.Logging;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
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
            AccInfo = new AccInfo();
            AccInfo.Init();

            Access = new AccessInfo();
            Access.Init();

            Villages = new List<Village>();

            Farming = new Farming();

            Hero = new Hero();
            Hero.Init();

            Quests = new QuestsSettings();
            Quests.Init();

            NewVillages = new NewVillageSettings();
            NewVillages.Init();

            Settings = new GeneralSettings();
            Settings.Init();

            Server = new AccServerData();
            Server.Init();
        }

        public void Load()
        {
            Wb = new WebBrowserInfo();
            Tasks = new TaskList(this);
            TaskTimer = new TaskTimer(this);

            if (Settings.DiscordWebhook && !string.IsNullOrEmpty(AccInfo.WebhookUrl))
            {
                WebhookClient = new DiscordWebhookClient(AccInfo.WebhookUrl);
                if (Settings.DiscordOnlineAnnouncement)
                {
                    DiscordHelper.SendMessage(this, "Account loaded");
                }
            }

            LogOutput.Instance.AddUsername(AccInfo.Nickname);
            Logger = new Logger(AccInfo.Nickname);

            Villages.ForEach(vill => vill.UnfinishedTasks = new List<VillUnfinishedTask>());
            // x.Tasks.Load();
        }

        public void Dispose()
        {
            Wb.Dispose();
            TaskTimer.Dispose();
            WebhookClient?.Dispose();
        }

        public AccInfo AccInfo { get; set; }
        public AccessInfo Access { get; set; }
        public List<Village> Villages { get; set; }
        public Farming Farming { get; set; }
        public Hero Hero { get; set; }
        public QuestsSettings Quests { get; set; }
        public NewVillageSettings NewVillages { get; set; }
        public GeneralSettings Settings { get; set; }
        public AccServerData Server { get; private set; }

        [JsonIgnore]
        public WebBrowserInfo Wb { get; private set; }

        [JsonIgnore]
        public TaskList Tasks { get; private set; }

        [JsonIgnore]
        public TaskTimer TaskTimer { get; private set; }

        [JsonIgnore]
        public DiscordWebhookClient WebhookClient { get; set; }

        [JsonIgnore]
        public Logger Logger { get; private set; }

        [JsonIgnore]
        public Status Status;
    }

    public enum Status
    {
        Offline,
        Starting,
        Online,
        Pausing,
        Paused,
        Stopping,
    }
}