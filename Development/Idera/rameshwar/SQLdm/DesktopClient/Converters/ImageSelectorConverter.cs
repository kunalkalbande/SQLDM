using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Idera.SQLdm.DesktopClient.Converters
{
    public class ImageSelectorConverter : MarkupExtension, IValueConverter
    {
        public Dictionary<string, ImageSource> Values { get; set; }
        public string DefaultValueKey { get; set; }

        public ImageSelectorConverter()
        {
            Values = new Dictionary<string, ImageSource>();
        }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource image = null;
            if (value == null) value = DefaultValueKey;
            if (value != null)
            {
                var key = value.ToString();
                if (!Values.TryGetValue(key, out image))
                {
                    var defaultKey = DefaultValueKey ?? key;
                    if (key != defaultKey)
                    {
                        Values.TryGetValue(defaultKey, out image);
                    }
                }
            }
            return image ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


}
