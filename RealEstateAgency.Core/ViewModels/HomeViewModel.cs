using RealEstateAgency.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RealEstateAgency.Core.Interfaces;
using System.Windows.Input;

namespace RealEstateAgency.Core.ViewModels
{
    public class HomeViewModel : NavigateViewModel
    {
        protected DataAccess.Connection DBConn
        {
            get { return DataAccess.Connection.
                    GetCurrentAsync().ExecuteSynchronously(); }
        }

                
        public int? EstatesCount
        {
            get { return (int?)GetProperty(); }
            set { SetProperty(value); }
        }
        public int? EstatesInSaleCount
        {
            get { return (int?)GetProperty(); }
            set { SetProperty(value); }
        }
        public ICommand NavigateToEstatesPageCommand
        {
            get { return (ICommand)GetProperty(); }
            internal set { SetProperty(value); }
        }


        public HomeViewModel(INavigationService navigationService, params INavigationPage[] pages) : this(false, navigationService, pages) { }
        public HomeViewModel(bool synchronizeWithContext, INavigationService navigationService, params INavigationPage[] pages) : base(synchronizeWithContext, navigationService, pages) { }


        public override async Task Initialize()
        {
            EstatesCount = await DBConn.SelectCountAsync<Model.Estate>();
            Errors.AddRange(DBConn.Errors);
            EstatesInSaleCount = await DBConn.SelectCountAsync<Model.Transaction>(
                t => !t.TransactionDone);
            Errors.AddRange(DBConn.Errors);
        }

    }
}
