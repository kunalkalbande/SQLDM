using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Idera.SQLdm.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
   
    public class MonitoredStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is MonitoredState)) return null;
            switch ((MonitoredState)value)
            {
                case MonitoredState.Critical:
                   return new SolidColorBrush(Color.FromRgb(0xFF,0x00,0x00));
                case MonitoredState.Warning:
                    return new SolidColorBrush(Color.FromRgb(0xFF,0xD7,0x00));
                case MonitoredState.Informational:
                    return new SolidColorBrush(Color.FromRgb(0x00,0x00,0xFF));
                case MonitoredState.OK:
                    return new SolidColorBrush(Color.FromRgb(0x22,0x8B,0x22));
                    break;
            }
            return new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("MonitoredStateToColorConverter.ConvertBack isn't implemented, yo!");
        }
    }

    public class SeverityImageKeyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string)) return null;
            switch ((string)value)
            {
                case "ServerCritical":
                   return new SolidColorBrush(Color.FromRgb(0xFF,0x00,0x00));
                case "ServerWarning":
                    return new SolidColorBrush(Color.FromRgb(0x00,0x00,0x00));
                case "SeverityMaintenanceMode":
                    return new SolidColorBrush(Color.FromRgb(0x00,0x00,0xFF));
            }
            
            return new SolidColorBrush(Color.FromRgb(0x22,0x8B,0x22));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("SeverityImageKeyColorConverter.ConvertBack isn't implemented, yo!");
        }
    }


    public class SeverityImageKeyImageConverter : IValueConverter
    {
        private const string RESOURCE_PREFIX = "/SQLdmDesktopClient;component/Resources/";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string)) return null;
            bool smallImage = parameter != null && parameter.Equals("Small");
            string key = "StatusSummaryOKLarge.jpg";
            switch ((string)value)
            {
                case "ServerMaintenanceMode":
                    key = smallImage ? "StatusMaintenanceModeSmall.png" : "ServerMaintenanceMode.png";
                    break;
                case "ServerCritical":
                    key = smallImage ? "StatusCriticalSmall.png" : "StatusSummaryCriticalLarge.png";
                    break;
                case "ServerWarning":
                    key = smallImage ? "StatusWarningSmall.png" : "StatusSummaryWarningLarge.png";
                    break;
                case "ServerOK":
                    key = smallImage ? "StatusOKSmall.png" : "StatusSummaryOKLarge.png";
                    break;
                default:
                    key = smallImage ? "StatusInfoSmall.png" : "StatusSummaryInformationalLarge.png";
                    break;
            }
            return new BitmapImage(new Uri(RESOURCE_PREFIX + key, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("SeverityImageKeyImageConverter.ConvertBack isn't implemented, yo!");
        }
    }

    public class BoolToTransparencyConverter : IValueConverter
    {
        private const string RESOURCE_PREFIX = "/SQLdmDesktopClient;component/Resources/";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double opacity = 1.0d;

            if (value == null) return opacity;
            if (!(value is bool || value is Int32 || value is string)) return opacity;

            if (value is bool && (!(bool)value))
                return opacity;
            
            if (value is int && (0 != (int)value))
                return opacity;

            if (value is string)
            {
                if (String.IsNullOrEmpty((string)value))
                    return opacity;
                if ("0".Equals(value))
                    return opacity;
            }
            
            if (parameter != null && Double.TryParse(parameter.ToString(), out opacity))
                return opacity;

            return 0.30d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("BoolToTransparencyConverter.ConvertBack isn't implemented, yo!");
        }
    }

}
