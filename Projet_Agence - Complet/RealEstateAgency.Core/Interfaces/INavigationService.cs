using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Interfaces
{
    public interface INavigationService
    {
        Task Navigate(INavigationPage page, object parameter);
    }
}
