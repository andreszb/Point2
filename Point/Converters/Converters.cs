using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Diagnostics;

namespace Point.Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            CultureInfo culture = new CultureInfo("es-MX");
            DateTime date = (DateTime)value;
            return String.Format(culture, "{0:D}", date.ToLocalTime());
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            CultureInfo culture = new CultureInfo("es-MX");
            DateTime date = (DateTime)value;
            return String.Format(culture, "{0:t}", date.ToLocalTime());
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class DoubleToCurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            CultureInfo culture = new CultureInfo("es-MX");
            return String.Format(culture, "{0:C2}", value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


}
