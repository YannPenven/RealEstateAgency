using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace RealEstateAgency.App.Navigation
{
    public class ShellViewModel : Core.ViewModels.BaseNotifyPropertyChanged
    {
        private ObservableCollection<MenuItem> topItems = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> bottomItems = new ObservableCollection<MenuItem>();


        public ICommand ToggleSplitViewPaneCommand { get; private set; }

        public bool IsSplitViewPaneOpen
        {
            get { return (bool)GetProperty(); }
            set { SetProperty(value); }
        }

        public MenuItem SelectedTopItem
        {
            get { return (MenuItem)GetProperty(); }
            set
            {
                if (SetProperty(value) && value != null)
                {
                    OnSelectedItemChanged(true);
                }
            }
        }

        public MenuItem SelectedBottomItem
        {
            get { return (MenuItem)GetProperty(); }
            set
            {
                if (SetProperty(value) && value != null)
                {
                    OnSelectedItemChanged(false);
                }
            }
        }

        public MenuItem SelectedMenuItem
        {
            get { return this.SelectedTopItem ?? this.SelectedBottomItem; }
            set
            {
                this.SelectedTopItem = this.topItems.FirstOrDefault(m => m == value);
                this.SelectedBottomItem = this.bottomItems.FirstOrDefault(m => m == value);
            }
        }

        public Type SelectedPageType
        {
            get
            {
                return this.SelectedMenuItem?.PageType;
            }
            set
            {
                // select associated menu item
                this.SelectedTopItem = this.topItems.FirstOrDefault(m => m.PageType == value);
                this.SelectedBottomItem = this.bottomItems.FirstOrDefault(m => m.PageType == value);
            }
        }

        public ObservableCollection<MenuItem> TopItems
        {
            get { return this.topItems; }
        }

        public ObservableCollection<MenuItem> BottomItems
        {
            get { return this.bottomItems; }
        }

        private void OnSelectedItemChanged(bool top)
        {
            if (top)
            {
                this.SelectedBottomItem = null;
            }
            else
            {
                this.SelectedTopItem = null;
            }
            OnPropertyChanged("SelectedMenuItem");
            OnPropertyChanged("SelectedPageType");

            this.IsSplitViewPaneOpen = false;
        }


        public ShellViewModel() : this(false) { }
        public ShellViewModel(bool synchronizeWithContext = false) : base(synchronizeWithContext)
        {
            this.ToggleSplitViewPaneCommand = new Core.Commands.Command(async () => this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen);
            this.IsSplitViewPaneOpen = false;
        }
    }
}
