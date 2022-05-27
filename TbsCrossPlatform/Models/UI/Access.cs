namespace TbsCrossPlatform.Models.UI
{
    public class Access
    {
        public int AccountId { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public bool Ok { get; set; }
    }
}