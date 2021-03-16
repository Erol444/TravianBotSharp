using TbsCore.Models.SideBarModels;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.AccModels
{
    public class AccInfo
    {
        public void Init()
        {
            CulturePoints = new CulturePoints();
        }

        /// <summary>
        /// Account username/nickname
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Url of the server
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Account tribe
        /// </summary>
        public Classificator.TribeEnum? Tribe { get; set; }

        /// <summary>
        /// Amount of gold account has
        /// </summary>
        public long Gold { get; set; }

        /// <summary>
        /// Amount of silver account has
        /// </summary>
        public long Silver { get; set; }

        /// <summary>
        /// If the account has a Plus Account enabled
        /// </summary>
        public bool PlusAccount { get; set; }

        /// <summary>
        /// CulturePoints object
        /// </summary>
        public CulturePoints CulturePoints { get; set; }

        /// <summary>
        /// Population rank of the account
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Account population
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Capital of the account (villageId)
        /// </summary>
        public int Capital { get; set; }

        /// <summary>
        /// If the account has Gold Account enabled
        /// </summary>
        public bool? GoldClub { get; set; }

        /// <summary>
        /// Map size if the server you are playing on
        /// If 100, Map is from -100/-100 to 100/100
        /// </summary>
        public int MapSize { get; set; }

        /// <summary>
        /// Speed of the server you are playing on. For time calculation usage (market transit time, troops movement time)
        /// </summary>
        public int ServerSpeed { get; set; }

        /// <summary>
        /// Version of the server account is playing on
        /// </summary>
        public Classificator.ServerVersionEnum ServerVersion { get; set; }

        /// <summary>
        /// Discord webhook url
        /// </summary>
        public string WebhookUrl { get; set; }
    }
}