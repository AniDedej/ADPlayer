using System.Globalization;
using System;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia;

namespace ADPlayer.Converters
{
    public class NullToDefaultImageConverter : IValueConverter
    {
        private static Bitmap? _defaultBitmap;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Bitmap bitmap) return bitmap;

            if (_defaultBitmap == null)
            {
                try
                {
                    var uri = new Uri("avares://ADPlayer/Assets/adplayer_white.png");
                    _defaultBitmap = new Bitmap(AssetLoader.Open(uri));
                }
                catch
                {
                    return null;
                }
            }

            return _defaultBitmap;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
