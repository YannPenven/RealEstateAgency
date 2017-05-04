using RealEstateAgency.App.Navigation;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RealEstateAgency.App
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Initialize database
                await InitializeDatabase();

                // Create a Shell which navigates to the first page
                shell = new Shell();

                // hook-up shell root frame navigation events
                shell.RootFrame.NavigationFailed += OnNavigationFailed;
                shell.RootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // set the Shell as content
                Window.Current.Content = shell;

                // listen for back button clicks (both soft- and hardware)
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed += OnBackPressed;
                }

                UpdateBackButtonVisibility();
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        // handle hardware back button press
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        // handle software back button press
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            deferral.Complete();
        }



        private void UpdateBackButtonVisibility()
        {
            var shell = (Shell)Window.Current.Content;

            var visibility = AppViewBackButtonVisibility.Collapsed;
            if (shell.RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        private async Task<bool> InitializeDatabase()
        {
            // Initialisation de la base de données
            Core.DataAccess.Connection dbConn;

            dbConn = await Core.DataAccess.Connection.GetCurrentAsync(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT());
            if (await Tools.Notifications.ShowErrors())
            {
                Application.Current.Exit();
                return false;
            }

            await dbConn.InitializeDatabaseAsync(Tools.InfoService.Get());
            if (await Tools.Notifications.ShowErrors())
            {
                Application.Current.Exit();
                return false;
            }
            // ----------------------------------------
            // Ajout de biens à la base de données
            // ----------------------------------------

            await dbConn.DeleteAsync<Core.Model.Estate>();
            Core.Model.Estate e;

            e = new Core.Model.Estate()
            {
                Address = "71 rue Peter Fink",
                AnnualCharges = 20000,
                City = "BOURG EN BRESSE",
                Elevator = true,
                EstimatedPrice = 1500000,
                FloorNumber = 0,
                FloorsCount = 2,
                PropertyTaxes = 3500,
                RoomsCount = 40,
                Surface = 2000,
                Type = Core.Model.Estate.EstateType.CommercialLocal,
                Zip = "01000"
             };
            await dbConn.InsertAsync(e);
            if (await Tools.Notifications.ShowErrors())
            {
                Application.Current.Exit();
                return false;
            }
            return true;
        }
    }
}
