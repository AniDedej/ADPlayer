using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ADPlayer.Converters
{
    public class SecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sec)
                return TimeSpan.FromSeconds(sec).ToString(@"mm\:ss");
            return "00:00";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
