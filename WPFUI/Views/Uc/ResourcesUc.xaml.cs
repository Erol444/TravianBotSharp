﻿using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for ResourcesUc.xaml
    /// </summary>
    public partial class ResourcesUc : ReactiveUserControl<ResourcesViewModel>
    {
        public ResourcesUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Wood, v => v.Wood.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Clay, v => v.Clay.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Iron, v => v.Iron.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Crop, v => v.Crop.Value).DisposeWith(d);
            });
        }
    }
}