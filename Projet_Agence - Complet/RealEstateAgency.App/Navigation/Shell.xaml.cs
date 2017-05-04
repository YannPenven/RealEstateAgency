using RealEstateAgency.App.Views;
using RealEstateAgency.Core.Interfaces;
using RealEstateAgency.Core.Tools;
using RealEstateAgency.Core.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RealEstateAgency.App.Navigation
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();

            var vm = new ShellViewModel();

            vm.TopItems.Add(new MenuItem { Icon = "", Title = "Accueil", PageType = typeof(HomePage) });
            vm.TopItems.Add(new MenuItem { Icon = "", Title = "Gestion du patrimoine", PageType = typeof(EstatesPage) });

            vm.BottomItems.Add(new MenuItem { Icon = "", Title = "Paramètres", PageType = typeof(SettingsPage) });


            // select the first menu item
            vm.SelectedMenuItem = vm.TopItems.First();

            this.ViewModel = vm;

            // add entry animations
            var transitions = new TransitionCollection { };
            var transition = new NavigationThemeTransition { };
            transitions.Add(transition);
            this.Frame.ContentTransitions = transitions;
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame RootFrame
        {
            get
            {
                return this.Frame;
            }
        }
    }
}
