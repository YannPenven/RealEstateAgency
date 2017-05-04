using System;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Commands
{
    public class EventBindingCommand<TEventArgs, TCommandParam> : Command<EventBindingArgs<TEventArgs, TCommandParam>> where TEventArgs : EventArgs
    {
        public EventBindingCommand(Func<EventBindingArgs<TEventArgs, TCommandParam>, Task> execute) : base(execute, null) { }

        public EventBindingCommand(Func<EventBindingArgs<TEventArgs, TCommandParam>, Task> execute, Func<EventBindingArgs<TEventArgs, TCommandParam>, bool> canExecute) : base(execute, canExecute) { }
    }

    public class EventBindingCommand<TEventArgs, TCommandParam1, TCommandParam2> : Command<EventBindingArgs<TEventArgs, TCommandParam1, TCommandParam2>> where TEventArgs : EventArgs
    {
        public EventBindingCommand(Func<EventBindingArgs<TEventArgs, TCommandParam1, TCommandParam2>, Task> execute) : base(execute, null) { }

        public EventBindingCommand(Func<EventBindingArgs<TEventArgs, TCommandParam1, TCommandParam2>, Task> execute, Func<EventBindingArgs<TEventArgs, TCommandParam1, TCommandParam2>, bool> canExecute) : base(execute, canExecute) { }
    }
}
