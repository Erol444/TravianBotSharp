using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using System;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class AddAccountsWindow : ReactiveWindow<AddAccountsViewModel>
    {
        public AddAccountsWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = new();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Close(ViewModel.GetAccounts());
        }
    }
}