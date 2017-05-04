using RealEstateAgency.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RealEstateAgency.App.Navigation
{
    public class NavigationService : INavigationService
    {

        private static NavigationService _instance = null;
        public static NavigationService Instance
        {
            get
            {
                if (_instance == null) _instance = new NavigationService();
                return _instance;
            }
        }

        private Shell _shell;
        private NavigationService()
        {
            _shell = Window.Current.Content as Shell;
        }


        public async Task Navigate(INavigationPage page, object parameter)
        {
            _shell.RootFrame.Navigate(page.PageType, parameter);
        }
    }
}
