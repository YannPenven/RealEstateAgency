using RealEstateAgency.Core.Interfaces;
using RealEstateAgency.Core.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.ViewModels
{
    public abstract class NavigateViewModel : InitializableViewModel
    {
        protected object _navParameter = null;
        protected Action _navParameterChanged = null;

        public ObservableCollection<string> Errors
        {
            get { return (ObservableCollection<string>)GetProperty(); }
            set { if (SetProperty(value)) base.OnPropertyChanged("ErrorsExists"); }
        }
        public bool ErrorsExists
        {
            get { return Errors.Count > 0; }
        }

        public object NavParameter
        {
            protected get { return _navParameter; }
            set
            {
                if (_navParameter != value)
                {
                    _navParameter = value;
                    _navParameterChanged?.Invoke();
                }
            }
        }


        public NavigateViewModel(INavigationService navigationService, params INavigationPage[] pages) : this(false, navigationService, pages) { }
        public NavigateViewModel(bool synchronizeWithContext, INavigationService navigationService, params INavigationPage[] pages) : base(synchronizeWithContext)
        {
            LoadCommands(navigationService, pages);
            Errors = new ObservableCollection<string>();
            Errors.CollectionChanged += Errors_CollectionChanged;
        }

        private void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged("ErrorsExists");
        }

        private void LoadCommands(INavigationService navigationService, params INavigationPage[] pages)
        {
            if (navigationService == null) return;
            if (pages == null || pages.Length == 0) return;

            TypeInfo ti = this.GetType().GetTypeInfo();
            foreach (INavigationPage p in pages)
            {
                PropertyInfo prop = ti.GetDeclaredProperty(p.NavigationToCommandName);
                if (prop != null)
                {
                    prop.SetValue(this, new Commands.Command<object>(async (parameter) => {
                        await navigationService.Navigate(p, parameter);
                    }));
                }
            }

        }
    }
}
