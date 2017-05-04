using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.ViewModels
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, object> _propertyValues;
        protected bool _synchronizeWithContext;


        [Ignore]
        public bool SynchronizeWithContext
        {
            get
            {
                return _synchronizeWithContext;
            }
            private set { }
        }


        public BaseNotifyPropertyChanged(bool synchronizeWithContext = false)
        {
            _propertyValues = new Dictionary<string, object>();
            _synchronizeWithContext = synchronizeWithContext;
        }

        protected Commands.Command GetCommand(Func<Task> execute,
                                              Func<bool> canExecute = null,
                                              [CallerMemberName] string propertyName = null)
        {
            if (!_propertyValues.ContainsKey(propertyName)) _propertyValues[propertyName] = new Commands.Command(execute, canExecute);
            return (Commands.Command)_propertyValues[propertyName];
        }

        protected virtual object GetProperty([CallerMemberName] string propertyName = null)
        {
            if (_propertyValues.ContainsKey(propertyName)) return _propertyValues[propertyName];
            return null;
        }
        protected bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            object current = GetProperty(propertyName);

            if ((current == null && newValue == null) ||
                (current != null && EqualityComparer<T>.Default.Equals((T)current, newValue)))
            {
                return false;
            }

            _propertyValues[propertyName] = newValue;
            OnPropertyChanged(propertyName);

            return true;
        }
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                if (!_synchronizeWithContext || SynchronizationContext.Current == null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    SynchronizationContext.Current.Send(
                            (obj) =>
                            {
                                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                            },
                            null);
                }
            }
        }

    }
}
