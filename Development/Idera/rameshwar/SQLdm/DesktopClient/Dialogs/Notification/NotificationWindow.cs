using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Idera.SQLdm.DesktopClient.Objects;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Views.Alerts;

    public partial class NotificationWindow : Form
    {
        #region Fields

        private Rectangle rScreen;

        private int VISIBLE_TIME_MS = 6000;
        private int VISIBLE_AFTER_EXIT_TIME_MS = 3000;

        private WindowStates windowState;

        private List<MonitoredSqlServerStatus> instanceStatus;

        #endregion

        #region Constructor

        public NotificationWindow()
        {
            InitializeComponent();

            rScreen = Screen.PrimaryScreen.WorkingArea;

            base.Show();
            base.Hide();
            AdaptFontSize();
        }

        #endregion

        public void SetInstanceStatus(List<MonitoredSqlServerStatus> instanceStatus)
        {
            this.instanceStatus = instanceStatus;
            if (instanceStatus.Count > 0)
                FormatLink(linkLabel1, instanceStatus[0]);
            if (instanceStatus.Count > 1)
                FormatLink(linkLabel2, instanceStatus[1]);
            if (instanceStatus.Count > 2)
                FormatLink(linkLabel3, instanceStatus[2]);
            if (instanceStatus.Count > 3)
                FormatLink(linkLabel4, instanceStatus[3]);
            if (instanceStatus.Count > 4)
                FormatLink(linkLabel5, instanceStatus[4]);
        }

        private void FormatLink(LinkLabel linkLabel, MonitoredSqlServerStatus monitoredSqlServerStatus)
        {
            linkLabel.Text = String.Format("{0} is now in the {1} state", monitoredSqlServerStatus.InstanceName, monitoredSqlServerStatus.Severity);
            linkLabel.Image = MonitoredSqlServerStatus.GetSeverityImage(monitoredSqlServerStatus.Severity);
            linkLabel.Tag = monitoredSqlServerStatus;
        }

        #region Properties

        #endregion

        public new void Show()
        {
            // position directly above the tray
            Top = rScreen.Bottom - Height;
            Left = rScreen.Width - Width - 11;

            // show and position the window so that it is not activated
            ShowWindow(Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(Handle, HWND_TOPMOST, rScreen.Width - Width - 11, rScreen.Bottom - Height, Width, Height, SWP_NOACTIVATE);
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        #region Fade Logic

        private void OnTimer(object sender, EventArgs e)
        {
            if (IsMouseOver())
            {
                SetFullyVisible();
                return;
            }

            switch (windowState)
            {
                case WindowStates.FadeIn:
                    if (Opacity < 0.99)
                    {
                        Opacity += 0.05;
                    }
                    else
                    {
                        this.timer.Interval = VISIBLE_TIME_MS;
                        windowState = WindowStates.Visible;
                    }
                    break;
                case WindowStates.Visible:
                    windowState = WindowStates.FadeOut;
                    timer.Interval = 100;
                    break;
                case WindowStates.FadeOut:
                    if (Opacity > 0.01)
                    {
                        Opacity -= 0.05;
                    }
                    else
                    {
                        this.Close();
                    }
                    break;
            }
        }

        private bool IsMouseOver()
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);
            return Bounds.Contains(cursor);
        }

        private void SetFullyVisible()
        {
            if (Opacity < 1.0)
                Opacity = 1.0;

            windowState = WindowStates.Visible;

            this.timer.Interval = VISIBLE_AFTER_EXIT_TIME_MS;
            this.timer.Enabled = true;
        }

        #endregion

        #region Event Handlers

        private void NotificationWindow_Load(object sender, EventArgs e)
        {
            this.windowState = WindowStates.FadeIn;
            this.timer.Enabled = true;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MonitoredSqlServerStatus monitoredSqlServerStatus = ((Control)sender).Tag as MonitoredSqlServerStatus;
            if (monitoredSqlServerStatus != null)
            {
                // switch to the alerts view
                AlertFilter filter = new AlertFilter();
                filter.Instance = monitoredSqlServerStatus.InstanceName;
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Active, filter);
                // if the console is minimized then restore it
                MainForm mainForm = Application.OpenForms["MainForm"] as MainForm;
                if (mainForm.WindowState == FormWindowState.Minimized)
                {
                    mainForm.ShowConsole();
                }
                Close();
                Dispose();
            }
        }

        #endregion

        #region P/Invoke
        protected const Int32 HWND_TOPMOST = -1;
        protected const Int32 SWP_NOACTIVATE = 0x0010;
        protected const Int32 SW_SHOWNOACTIVATE = 4;

        [DllImport("user32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, Int32 flags);
        [DllImport("user32.dll")]
        protected static extern bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, uint uFlags);
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        static public extern bool GetCursorPos(ref Point lpPoint); 


        #endregion

        public enum WindowStates
        {
            Hidden = 0,
            FadeIn = 1,
            Visible = 2,
            FadeOut = 3
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}