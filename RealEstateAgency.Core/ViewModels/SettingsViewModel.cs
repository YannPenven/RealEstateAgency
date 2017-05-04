using RealEstateAgency.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RealEstateAgency.Core.Interfaces;

namespace RealEstateAgency.Core.ViewModels
{
    public class SettingsViewModel : NavigateViewModel
    {
        protected DataAccess.Connection DBConn
        {
            get
            {
                return DataAccess.Connection.
                  GetCurrentAsync().ExecuteSynchronously();
            }
        }




        public SettingsViewModel(INavigationService navigationService, params INavigationPage[] pages) : this(false, navigationService, pages) { }
        public SettingsViewModel(bool synchronizeWithContext, INavigationService navigationService, params INavigationPage[] pages) : base(synchronizeWithContext, navigationService, pages) { }


        public override async Task Initialize()
        {

        }

    }
}
