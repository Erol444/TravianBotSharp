using System.Collections.Generic;
using TravBotSharp.Files.Models.AccModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.Settings
{
    public class GeneralSettings
    {
        public void Init()
        {
            FillFor = 2;
            FillInAdvance = 4;
            AutoReadIgms = true;
            Time = new TimeSettings();
            Time.Init();
            Timing = new TimingData();
            Localization = new Dictionary<string, BuildingEnum>();
            OpenMinimized = false;
            WatchAdAbove = 80;
        }
        public bool AutoActivateProductionBoost { get; set; }

        /// <summary>
        /// Main village where resources will get sent and sent from
        /// </summary>
        public int MainVillage { get; set; }
        /// <summary>
        /// For selecting for how many hours in advance do we want to fill troops in barracks/stable/GB/GS/workshop
        /// TODO: make is selectable for each village and each building separately.
        /// </summary>
        public int FillInAdvance { get; set; }
        /// <summary>
        /// If we fall below FillInAdvance hours, for how many hours do you want to train in advance
        /// eg. We want to fill barracks for 4 hours in advance. When we fall below 4h, train for another 2h.
        /// </summary>
        public int FillFor { get; set; }
        /// <summary>
        /// If true, bot will auto read new IGMs (~5min after detecting one)
        /// </summary>
        public bool AutoReadIgms { get; set; }
        /// <summary>
        /// Chrome selenium driver disable fetching images to save on memory
        /// </summary>
        public bool DisableImages { get; set; }
        /// <summary>
        /// Initialize Chrome selenium driver in headless mode
        /// </summary>
        public bool HeadlessMode { get; set; }
        /// <summary>
        /// Time settings for the bot
        /// </summary>
        public TimeSettings Time { get; set; }
        /// <summary>
        /// Whether you want to Auto improve units
        /// </summary>
        public bool AutoImprove { get; set; }
        /// <summary>
        /// Data about when things last happened
        /// </summary>
        public TimingData Timing { get; set; }
        /// <summary>
        /// Whether to close and reopen chrome if there is no task in the next 5 min
        /// </summary>
        public bool AutoCloseDriver { get; set; }
        /// <summary>
        /// Localization strings for buildings
        /// </summary>
        public Dictionary<string, BuildingEnum> Localization { get; set; }
        /// <summary>
        /// Whether to automatically add random tasks when there is no other task to be executed
        /// </summary>
        public bool AutoRandomTasks { get; set; }
        /// <summary>
        /// Minimize the chrome right after opening it
        /// </summary>
        public bool OpenMinimized { get; set; }
        /// <summary>
        /// If building takes longer than {value} minutes, watch an ad!
        /// </summary>
        public int WatchAdAbove { get; set; }
    }
}
