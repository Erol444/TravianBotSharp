using ReactiveUI;

namespace WPFUI.Models
{
    public class Access : ReactiveObject
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string ProxyHost { get; set; }
        public string ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        private string _proxyStatus;

        public string ProxyStatus
        {
            get => _proxyStatus;
            set => this.RaiseAndSetIfChanged(ref _proxyStatus, value);
        }
    }
}