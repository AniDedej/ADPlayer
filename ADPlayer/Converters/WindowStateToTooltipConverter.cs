using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ADPlayer.Converters
{
    public class WindowStateToTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowState state)
            {
                return state == WindowState.Maximized ? "Restore" : "Maximize";
            }
            return "Maximize";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
