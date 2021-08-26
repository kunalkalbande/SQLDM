using System.Windows;
using System.Windows.Interactivity;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Behaviors
{
    public class WindowSettingsBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Closing += AssociatedObject_Closing;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Closing -= AssociatedObject_Closing;
            base.OnDetaching();
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySettings();
        }
        
        void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        void ApplySettings()
        {
            if (AssociatedObject == null) return;

            double top;
            double left;
            double width;
            double height;

            var savedState = Settings.Default.MainWindowState;
            if (savedState == WindowState.Maximized)
            {
                var restoreBounds = Settings.Default.MainWindowRestoreBounds;
                top = restoreBounds.Top;
                left = restoreBounds.Left;
                width = restoreBounds.Width;
                height = restoreBounds.Height;
            }
            else
            {
                var p = Settings.Default.MainWindowLocation;
                var s = Settings.Default.MainWindowSize;
                top = p.Y;
                left = p.X;
                width = s.Width;
                height = s.Height;
            }

            var screen = new Rect(SystemParameters.VirtualScreenLeft, 
                                SystemParameters.VirtualScreenTop,
                                SystemParameters.VirtualScreenWidth, 
                                SystemParameters.VirtualScreenHeight);

            if (screen.Contains(left, top))
            {
                AssociatedObject.Left = left;
                AssociatedObject.Top = top;
            }
            if (AssociatedObject.ResizeMode != ResizeMode.NoResize)
            {
                AssociatedObject.Width = width;
                AssociatedObject.Height = height;
            }
            if (Settings.Default.MainWindowState != WindowState.Minimized)
                AssociatedObject.WindowState = Settings.Default.MainWindowState;
        }

        void SaveSettings()
        {
            if (AssociatedObject == null) return;

            if (AssociatedObject.WindowState == WindowState.Normal)
            {
                Settings.Default.MainWindowLocation = new Point(AssociatedObject.Left, AssociatedObject.Top);
                Settings.Default.MainWindowSize = new Size(AssociatedObject.Width, AssociatedObject.Height);
            }
            Settings.Default.MainWindowState = AssociatedObject.WindowState;
            Settings.Default.MainWindowRestoreBounds = AssociatedObject.RestoreBounds;

            Settings.Default.Save();
        }
    }
}
