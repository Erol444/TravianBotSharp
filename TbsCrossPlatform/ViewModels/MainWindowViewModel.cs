using TbsCrossPlatform.Services;

namespace TbsCrossPlatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IWaitingService _waitingService;
        private readonly IDbContextPool _dbContextService;

        public MainWindowViewModel()
        {
            _waitingService = Program.GetService<IWaitingService>();
            _dbContextService = Program.GetService<IDbContextPool>();
        }

        public static string ContentButton => "Test";

        public async void OnClickCommand()
        {
            var task = _waitingService.Show();
            using var context = _dbContextService.CreateDbContext();
            _waitingService.Close();
            await task;
        }
    }
}