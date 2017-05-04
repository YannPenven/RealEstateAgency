using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.App.Navigation
{
    public class MenuItem : Core.ViewModels.BaseNotifyPropertyChanged
    {
        public string Icon
        {
            get { return (string)GetProperty(); }
            set { SetProperty(value); }
        }

        public string Title
        {
            get { return (string)GetProperty(); }
            set { if (SetProperty(value)) OnPropertyChanged("DisplayTitleUppercase"); }
        }
        public string DisplayTitleUppercase
        {
            get { return (string.IsNullOrEmpty(Title) ? "" : Title.ToUpper()); }
        }

        public Type PageType
        {
            get { return (Type)GetProperty(); }
            set { SetProperty(value); }
        }

        public MenuItem() : this(false) { }
        public MenuItem(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
