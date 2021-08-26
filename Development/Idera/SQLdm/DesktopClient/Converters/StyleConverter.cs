using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Idera.SQLdm.DesktopClient.Converters
{
    public class StyleConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Style style = new Style(typeof(ComboBox));
            if (((string)value).IndexOf("Real Time", StringComparison.OrdinalIgnoreCase) != -1)
            {
                style.Setters.Add(new Setter(ComboBox.FontWeightProperty, FontWeights.Bold));
                return style;
            }

            return style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
