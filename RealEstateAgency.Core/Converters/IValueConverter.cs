using System;

namespace RealEstateAgency.Core.Converters
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter, string language);
        object ConvertBack(object value, Type targetType, object parameter, string language);
    }
}
