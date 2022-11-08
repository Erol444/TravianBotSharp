using ReactiveUI;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;

namespace UI.ViewModels.UserControls
{
    public sealed class LoadingOverlayViewModel : ViewModelBase
    {
        private readonly ConcurrentQueue<bool> _loadingStates = new();

        public void Load()
        {
            _loadingStates.Enqueue(true);
            RxApp.MainThreadScheduler.Schedule(() => this.RaisePropertyChanged(nameof(IsLoading)));
        }

        public void Unload()
        {
            if (_loadingStates.IsEmpty) return;
            _loadingStates.TryDequeue(out _);
            RxApp.MainThreadScheduler.Schedule(() => this.RaisePropertyChanged(nameof(IsLoading)));
        }

        public bool IsLoading => !_loadingStates.IsEmpty;

        private string _loadingText;

        public string LoadingText
        {
            get => _loadingText;
            set => RxApp.MainThreadScheduler.Schedule(() => this.RaiseAndSetIfChanged(ref _loadingText, value));
        }
    }
}