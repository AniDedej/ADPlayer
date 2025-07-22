using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ADPlayer.Converters
{
    public class WindowStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as WindowState? ?? WindowState.Normal;
            return state == WindowState.Maximized ? "\uF032" : "\uF031";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
