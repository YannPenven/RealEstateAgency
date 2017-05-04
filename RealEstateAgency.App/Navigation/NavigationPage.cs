using RealEstateAgency.Core.Interfaces;
using RealEstateAgency.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using RealEstateAgency.Core.ViewModels;

namespace RealEstateAgency.App.Navigation
{
    public static class NavigationPage
    {
        public static void Register(INavigationPage page, string navigationToCommandName)
        {
            page.NavigationToCommandName = navigationToCommandName;
        }
        public static void Register(INavigationPage page, BaseNotifyPropertyChanged viewModel, object parameter)
        {
            page.DataContext = viewModel;

            if (viewModel != null)
            {
                if (viewModel.GetType().IsChildOf(typeof(NavigateViewModel)))
                {
                    ((NavigateViewModel)viewModel).NavParameter = parameter;
                }
                if (viewModel.GetType().IsChildOf(typeof(InitializableViewModel)))
                {
                    ((Page)page).Loaded += NavigationPage_Loaded;
                }
            }
            if (viewModel != null && viewModel.GetType().IsChildOf(typeof(InitializableViewModel)))
            {
                ((Page)page).Loaded += NavigationPage_Loaded;
            }
        }
        public static void Unregister(Page page)
        {
            page.Loaded -= NavigationPage_Loaded;
        }

        private static async void NavigationPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender == null || (sender.GetType() != typeof(Page) && !sender.GetType().IsChildOf(typeof(Page)))) return;
            if (((Page)sender).DataContext != null && ((Page)sender).DataContext.GetType().IsChildOf(typeof(InitializableViewModel)))
            {
                await ((InitializableViewModel)((Page)sender).DataContext).Initialize();
            }
        }
    }
}
