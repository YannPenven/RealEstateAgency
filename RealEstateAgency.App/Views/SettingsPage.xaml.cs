using RealEstateAgency.App.Navigation;
using RealEstateAgency.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace RealEstateAgency.App.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, INavigationPage
    {
        public string NavigationToCommandName { get; set; }

        public Type PageType
        {
            get { return this.GetType(); }
        }

        public SettingsPage()
        {
            NavigationPage.Register(this, "NavigateToSettingsPageCommand");
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<INavigationPage> canNavigateTo = new List<INavigationPage>();
            // TODO : Ajouter les pages vers lesquelles le view-model permet de naviguer
            canNavigateTo.Add(new EstatesPage());

            NavigationPage.Register(this,
                                    new Core.ViewModels.SettingsViewModel(NavigationService.Instance, canNavigateTo.ToArray()),
                                    e.Parameter);

            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationPage.Unregister(this);
            base.OnNavigatedFrom(e);
        }
    }
}
