using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Idera.SQLdm.DesktopClient.Converters
{
    public class BoolToImageConverter : MarkupExtension, IValueConverter
    {
        public BoolToImageConverter()
        {
            TrueValue = null;
            FalseValue = null;
        }

        public ImageSource TrueValue { get; set; }
        public ImageSource FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = System.Convert.ToBoolean(value);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TrueValue.Equals(value) ? true : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


}
