using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RealEstateAgency.App.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value.GetType() != typeof(bool)) return Visibility.Collapsed;
            if (!(bool)value) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;
            if (value.GetType() != typeof(Visibility)) return false;
            if ((Visibility)value != Visibility.Visible) return false;
            return true;
        }
    }
}
