using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ADPlayer.Converters
{
    public class VolumeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vol = value is float f ? f : 0f;
            if (vol < 0.01f) return "\uE04F";
            else if (vol < 0.33f) return "\uE04E";
            else if (vol < 0.66f) return "\uE04D";
            else return "\uE050";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
