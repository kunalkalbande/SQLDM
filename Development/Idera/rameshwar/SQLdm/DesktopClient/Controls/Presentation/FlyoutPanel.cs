using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Infragistics.Windows;

namespace Idera.SQLdm.DesktopClient.Controls.Presentation
{
    public class FlyoutPanel : Popup
    {
        private bool _IsHooked;

        static FlyoutPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutPanel), new FrameworkPropertyMetadata(typeof(FlyoutPanel)));
        }

        public FlyoutPanel()
        {
            Placement = PlacementMode.Left;
        }

        protected override void OnOpened(EventArgs e)
        {
            if (!_IsHooked)
            {
                HookEvents();
            }

            base.OnOpened(e);

            // nudge the height to force resize callback
            this.Height -= 0.1;
        }

        private void HookEvents()
        {
            _IsHooked = true;
            var dockPanel = Parent as Grid;
            if (dockPanel != null)
            {
                UpdateHeight(dockPanel.Height);

                dockPanel.SizeChanged += dockingPoint_SizeChanged;
                var window = (Window)Utilities.GetAncestorFromType(dockPanel, typeof(Window), true);
                if (window != null)
                {
                    window.LocationChanged += window_LocationChanged;
                    window.SizeChanged += window_SizeChanged;
                }
            }
        }

        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLocation();
        }

        void window_LocationChanged(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        void dockingPoint_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLocation();
            UpdateHeight(e.NewSize.Height);
        }

        private void UpdateLocation()
        {
            var ho = HorizontalOffset;
            HorizontalOffset = ho + 0.01d;
            HorizontalOffset = ho;
        }

        private void UpdateHeight(double height)
        {
            var element = Child as FrameworkElement;
            if (element != null)
                element.Height = height;
        }

    }
}
