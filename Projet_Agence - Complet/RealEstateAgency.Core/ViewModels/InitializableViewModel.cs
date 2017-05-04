using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.ViewModels
{
    public abstract class InitializableViewModel : BaseNotifyPropertyChanged
    {
        public abstract Task Initialize();

        public InitializableViewModel() : this(false) { }
        public InitializableViewModel(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
