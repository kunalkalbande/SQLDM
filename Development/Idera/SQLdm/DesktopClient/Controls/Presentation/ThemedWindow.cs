using System.ComponentModel;
using System.Windows;
using Infragistics.Windows.Ribbon;
using Infragistics.Windows.Themes;
using Infragistics.Windows.Themes.Internal;

namespace Idera.SQLdm.DesktopClient.Controls.Presentation
{
    /// <summary>
    /// Interaction logic for ThemedWindow.xaml
    /// </summary>
    public class ThemedWindow : System.Windows.Window
    {
        public readonly static DependencyProperty ThemeProperty;
        public readonly static RoutedEvent ThemeChangedEvent;

        public event RoutedPropertyChangedEventHandler<string> ThemeChanged;

        static ThemedWindow()
        {
            ThemedWindow.ThemeProperty = ThemeManager.ThemeProperty.AddOwner(typeof(ThemedWindow), new FrameworkPropertyMetadata(ThemedWindow.OnThemeChanged));
            ThemedWindow.ThemeChangedEvent = ThemeManager.ThemeChangedEvent.AddOwner(typeof(ThemedWindow));

            ThemeManager.RegisterGroupings(typeof(ThemedWindow), GetThemeGroupings());
        }

        private static string[] GetThemeGroupings()
        {
            string[] grouping = new string[3];
            grouping[0] = PrimitivesGeneric.Location.Grouping;
            grouping[1] = EditorsGeneric.Location.Grouping;
            grouping[2] = RibbonGeneric.Location.Grouping;
            return grouping;
        }

        public ThemedWindow()
        {
            SetValue(ThemeProperty, "[Current]");
        }

        [TypeConverter(typeof(RibbonThemeTypeConverter))]
        [Bindable(true)]
        [DefaultValue(null)]
        public string Theme
        {
            get
            {
                return (string)base.GetValue(ThemedWindow.ThemeProperty);
            }
            set
            {
                base.SetValue(ThemedWindow.ThemeProperty, value);
            }
        }


        private static void OnThemeChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var window = target as ThemedWindow;
            if (window == null) return;
            window.UpdateThemeResources();
            window.OnThemeChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void UpdateThemeResources()
        {
            ThemeManager.OnThemeChanged(this, this.Theme, GetThemeGroupings());

            if (base.IsInitialized)
            {
                base.InvalidateMeasure();
                base.UpdateLayout();
            }
        }

        protected virtual void OnThemeChanged(string previousValue, string currentValue)
        {
            var routedPropertyChangedEventArg = new RoutedPropertyChangedEventArgs<string>(previousValue, currentValue);
            routedPropertyChangedEventArg.RoutedEvent = ThemedWindow.ThemeChangedEvent;
            routedPropertyChangedEventArg.Source = this;
            base.RaiseEvent(routedPropertyChangedEventArg);
        }

    }
}
