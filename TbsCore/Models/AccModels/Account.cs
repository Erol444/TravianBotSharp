using System;
using System.Collections.Generic;
using TravBotSharp.Files.Models.Settings;
using TravBotSharp.Files.Models.SideBarModels;
using TravBotSharp.Files.Tasks;

namespace TravBotSharp.Files.Models.AccModels
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
        public List<BotTask> Tasks { get; set; }
        public Hero Hero { get; set; }
        public WebBrowserInfo Wb { get; set; }
        public QuestsSettings Quests { get; set; }
        public TaskTimer TaskTimer { get; set; }
        public NewVillageSettings NewVillages { get; set; }
        public GeneralSettings Settings { get; set; }

    }
}
