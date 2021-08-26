using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public static class WpfHelpers
    {
        public static IWin32Window GetWinformWindow(this Window window)
        {
            return new WinformWindow(window);
        }

        public static IWin32Window GetWinformWindow(this DependencyObject dependencyObject)
        {
            var window = Window.GetWindow(dependencyObject);
            return window != null ? window.GetWinformWindow() : null;
        }

        public static void SetVisibility(this UIElement element, bool condition)
        {
            element.Visibility = condition ? Visibility.Visible : Visibility.Collapsed;
        }

        public static ImageSource GetImageSource(Uri uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();
            return image;
        }
    }

    public class WinformWindow : IWin32Window
    {
        private WindowInteropHelper _helper;

        public WinformWindow(Window window)
        {
            _helper = new WindowInteropHelper(window);
        }

        public IntPtr Handle
        {
            get { return _helper.Handle; }
        }

        public static explicit operator WinformWindow(Window window)
        {
            return new WinformWindow(window);
        }
    }
}
