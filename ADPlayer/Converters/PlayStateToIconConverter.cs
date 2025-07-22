using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ADPlayer.Converters
{
    public class PlayStateToIconConverter : IValueConverter
    {
        public static PlayStateToIconConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool playing && playing ? "\uE034" : "\uE037";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
