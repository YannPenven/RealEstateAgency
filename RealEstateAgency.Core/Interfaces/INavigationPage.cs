using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Interfaces
{
    public interface INavigationPage
    {
        string NavigationToCommandName { get; set; }
        object DataContext { get; set; }
        Type PageType { get; }

    }
}
