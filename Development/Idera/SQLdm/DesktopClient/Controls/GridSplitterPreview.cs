using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Idera.SQLdm.DesktopClient.Controls
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public class GridSplitterPreview : Window
    {
        private static readonly Type TypeOfGridSplitterAdorner;
        private TranslateTransform _transform;
        private Point _ptForSplitterDrag;

        static GridSplitterPreview()
        {
            var splitterAssembly = typeof(GridSplitter).Assembly;
            TypeOfGridSplitterAdorner = splitterAssembly.GetType("System.Windows.Controls.GridSplitter.PreviewAdorner");
        }

        public GridSplitterPreview()
        {
            Background = new SolidColorBrush(Color.FromArgb(0x8D, 0, 0, 0));
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Focusable = false;
            BorderThickness = new Thickness(1);
            WindowStyle = WindowStyle.None;
            Visibility = Visibility.Collapsed;
            ShowInTaskbar = false;
            AllowsTransparency = true;
        }

        public void HookSplitter(GridSplitter splitter)
        {
            splitter.DragStarted += splitter_DragStarted;
            splitter.DragDelta += splitter_DragDelta;
            splitter.DragCompleted += splitter_DragCompleted;
        }

        private void splitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            var splitter = (GridSplitter)sender;

            _ptForSplitterDrag = splitter.PointToScreen(Mouse.GetPosition(splitter));
            _transform = GetTransform(splitter);
            var pt = splitter.PointToScreen(new Point());

            this.Left = pt.X;
            this.Top = pt.Y;
            this.Height = splitter.ActualHeight;
            this.Width = splitter.ActualWidth;
            this.Show();
        }

        private void splitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var splitter = (GridSplitter)sender;

            if (_transform != null)
            {
                var spt = splitter.PointToScreen(new Point());
                this.Left = _transform.Transform(spt).X;
                return;
            }

            var pt = splitter.PointToScreen(Mouse.GetPosition(splitter)) - _ptForSplitterDrag + splitter.PointToScreen(new Point());
            if (splitter.ResizeDirection == GridResizeDirection.Rows)
                this.Top = pt.Y;
            else
                this.Left = pt.X;
        }

        private void splitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private static TranslateTransform GetTransform(GridSplitter splitter)
        {
            var grid = splitter.Parent as Grid;
            if (grid == null) return null;

            var layer = AdornerLayer.GetAdornerLayer((Visual)splitter.Parent);
            if (layer == null) return null;

            var adorners = layer.GetAdorners(splitter);
            if (adorners == null || adorners.Length == 0) return null;

            var decorator = FindVisualChild<Decorator>(adorners[0]);
            if (decorator != null)
            {
                return decorator.RenderTransform as TranslateTransform;
            }

            return null;
        }

        private static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
}
