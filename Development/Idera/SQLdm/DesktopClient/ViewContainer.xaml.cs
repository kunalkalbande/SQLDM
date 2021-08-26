using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Views;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using UserControl = System.Windows.Controls.UserControl;

namespace Idera.SQLdm.DesktopClient
{
    /// <summary>
    /// Interaction logic for ViewContainer.xaml
    /// </summary>
    public partial class ViewContainer : UserControl
    {
        private bool _disposed = false;

        public ViewContainer()
        {
            InitializeComponent();
            winformsHostPanel.SizeChanged += wpfHostPanel_SizeChanged;
        }

        #region disposal

        ~ViewContainer()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                winformsHostPanel.SizeChanged -= wpfHostPanel_SizeChanged;
                winformsHostPanel.Dispose();

                //this is removing the keyboard hook from the WindowsFormHost
                var ikis = windowsFormsHostControl as System.Windows.Interop.IKeyboardInputSink;

                if (ikis != null)
                {
                    var kis = ikis.KeyboardInputSite;
                    if (kis != null)
                        kis.Unregister();
                }
                windowsFormsHostControl.Dispose();
            }

            _disposed = true;
        }

        public bool IsDisposed { get { return _disposed; } }

        #endregion

        void wpfHostPanel_SizeChanged(object sender, EventArgs e)
        {
            Debug.Print("New winforms host panel size={0}", winformsHostPanel.Size);
        }

        public void SuspendLayout()
        {
            winformsHostPanel.SuspendLayout();
        }

        public void ResumeLayout()
        {
            winformsHostPanel.ResumeLayout();
        }

        public void Add(IView view)
        {
            this.Loading.Visibility = Visibility.Visible;//SqlDM 10.2 (Tushar)--Making loading label visible when a new view is getting added to viewContainer.
            if (view is System.Windows.Forms.Control)
            {
                Add((System.Windows.Forms.Control) view);
                this.Loading.Visibility = Visibility.Hidden;//SqlDM 10.2 (Tushar)--Making loading label visibility false when view addition to viewContainer is complete.
                return;
            }
            if (view is UIElement)
            {
                Add((UIElement) view);
                this.Loading.Visibility = Visibility.Hidden;//SqlDM 10.2 (Tushar)--Making loading label visibility false when view addition to viewContainer is complete.
                return;
            }
            throw new ArgumentException("Control must be a UIElement or a WinForms Control.");
        }

        public void Add(System.Windows.Forms.Control control)
        {
            winformsHostPanel.SuspendLayout();
            try
            {
                control.Size = winformsHostPanel.Size;
                control.Dock = DockStyle.Fill;
                
                if (!winformsHostPanel.Controls.Contains(control))
                    winformsHostPanel.Controls.Add(control);

                windowsFormsHostControl.Visibility = Visibility.Visible;
                control.BringToFront();
            }
            finally
            {
                wpfHostPanel.Visibility = Visibility.Collapsed;
                winformsHostPanel.ResumeLayout();
            }
        }

        private UIElement _currentElement = null;
        public void Add(UIElement control)
        {
            // collapse the last UI Element added to the host panel
            if (_currentElement != null && _currentElement != control)
                _currentElement.Visibility = Visibility.Collapsed;

            // make sure the control is in the wpf host panel
            if (!wpfHostPanel.Children.Contains(control))
                wpfHostPanel.Children.Add(control);

            _currentElement = control;

            // make the host panel and the control visible
            wpfHostPanel.Visibility = Visibility.Visible;
            _currentElement.Visibility = Visibility.Visible;

            // collapse the winforms host control
            windowsFormsHostControl.Visibility = Visibility.Collapsed;
        }

        public void Remove(UIElement control)
        {
            wpfHostPanel.Children.Remove(control);
            if (_currentElement.Equals(control))
                _currentElement = null;
        }
    }
}
