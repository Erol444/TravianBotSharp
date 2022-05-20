using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbsCrossPlatform.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        private string _message;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public bool CanClose { get; set; } = false;

        public void LoadingWindow_Closing(object sender, CancelEventArgs e)
        {
            if (CanClose) return;
            e.Cancel = true;
        }
    }
}