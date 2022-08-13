using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Tabs
{
    public class HeroViewModel : ReactiveObject, IMainTabPage
    {
        public int AccountId { get; set; }

        public void OnActived()
        {
        }
    }
}