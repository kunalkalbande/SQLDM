using System;
using System.Windows.Media.Imaging;

namespace Idera.SQLdm.DesktopClient.Converters
{
    public class PathToImageConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var bitmap = new BitmapImage(new Uri(value.ToString()));
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The ability to convert an image back to a path has not been implemented.");
        }
    }
}
