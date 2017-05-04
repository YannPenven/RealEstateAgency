using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateAgency.Core.Commands
{
    public class Command : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Func<Task> execute) : this(execute, null) { }

        public Command(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }


        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                _execute();
            }
        }
        public virtual async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                await _execute();
            }
        }
    }
}
