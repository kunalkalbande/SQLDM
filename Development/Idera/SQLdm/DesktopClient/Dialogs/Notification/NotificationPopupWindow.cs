using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using System.Linq;
using Idera.SQLdm.Common.Configuration;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class NotificationPopupWindow : Form
    {
        private enum PopupState
        {
            Showing,
            FullyVisible,
            Hiding
        }

        private static readonly Logger Log = Logger.GetLogger("NotificationPopupWindow");

        /// <summary>
        /// Indicates the name for the notification sound file.
        /// </summary>
        private const string NotificationSoundFileName = @"Notification.wav";

        private const int RenderInterval = 1;
        private const int VisibleInterval = 5000;
        private const int VisibleIntervalAfterMouseLeave = 1500;
        private const int RenderMoveDistance = 5;
        private const int RightPadding = 17;

        /// <summary>
        /// Contains the full path to get the notification sound file.
        /// </summary>
        private static readonly String NotificationSoundFile = GetSoundFileFullPath();

        private PopupState currentState;

        public NotificationPopupWindow()
        {
            InitializeComponent();
            messageLabel.Visible = false;
            linksLayoutPanel.Visible = false;
            SoundAlertEnabled = true;
            AdaptFontSize();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        public bool SoundAlertEnabled { get; set; }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public new void Show()
        {
            Show(true);
        }

        public void Show(bool showAnimated)
        {
            if (showAnimated)
            {
                Location =
                    new Point(Screen.PrimaryScreen.WorkingArea.Width - Width - RightPadding,
                              Screen.PrimaryScreen.WorkingArea.Bottom);

                int taskbar = FindWindow("Shell_TrayWnd", null);
                SetWindowPos(Handle, taskbar, Left, Top, Width, Height, SWP_NOACTIVATE);
                ShowWindow(Handle, SW_SHOWNOACTIVATE);

                currentState = PopupState.Showing;
                renderTimer.Interval = RenderInterval;
                renderTimer.Start();
            }
            else
            {
                Location =
                    new Point(Screen.PrimaryScreen.WorkingArea.Width - Width - RightPadding,
                              Screen.PrimaryScreen.WorkingArea.Bottom - Height);

                int taskbar = FindWindow("Shell_TrayWnd", null);
                SetWindowPos(Handle, taskbar, Left, Top, Width, Height, SWP_NOACTIVATE);
                ShowWindow(Handle, SW_SHOWNOACTIVATE);

                currentState = PopupState.FullyVisible;
                renderTimer.Interval = VisibleInterval;
                renderTimer.Start();
            }

            PlayNotificationSound();
        }

        public new void Hide()
        {
            renderTimer.Stop();
            base.Hide();
        }

        public void ResetInstanceStatus()
        {
            ResetLinkLabel(linkLabel1);
            ResetLinkLabel(linkLabel2);
            ResetLinkLabel(linkLabel3);
            ResetLinkLabel(linkLabel4);
            ResetLinkLabel(linkLabel5);
        }

        private void ResetLinkLabel(LinkLabel linkLabel)
        {
            linkLabel.Text = String.Empty;
            linkLabel.Image = null;
            linkLabel.Tag = null;
        }

        public void SetMessage(string message)
        {
            linksLayoutPanel.Visible = false;
            messageLabel.Text = message;
            messageLabel.Visible = true;
        }

        public void SetInstanceStatus(IList<MonitoredSqlServerStatus> instanceStatus)
        {
            messageLabel.Visible = false;
            linksLayoutPanel.Visible = true;
            instanceStatus = instanceStatus.OrderByDescending(x => x.Severity).ToList();
            ResetInstanceStatus();

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

        /// <summary>
        /// Plays the notification sound file if it exist, if it does not found, push a warning in
        /// the log.
        /// </summary>
        private void PlayNotificationSound()
        {
            if (File.Exists(NotificationSoundFile))
            {
                Log.Debug(String.Format("The notification sound alert are enabled: {0}",
                          SoundAlertEnabled));

                if (SoundAlertEnabled)
                {
                    // Plays the notication sound.
                    try
                    {
                        // Used the 'using' to release the sound resources, after to play the
                        // notification.
                        using (SoundPlayer player
                               = new SoundPlayer(NotificationSoundFile))
                        {
                            // Plays the notication sound.
                            player.Play();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log if have any exception for opening the sound file.
                        Log.Error(String.Format("An error occurs when the '{0}' file is trying to opening.",
                                  NotificationSoundFile), ex);
                    }
                }
            }
            else
            {
                // Occurs if the sound file was not found.
                Log.Warn(String.Format("Unable to play the notification sound file. The file '{0}' "
                                       + "does not found.", NotificationSoundFile));
            }
        }

        private void FormatLink(LinkLabel linkLabel, MonitoredSqlServerStatus monitoredSqlServerStatus)
        {
            int InstanceId = monitoredSqlServerStatus.InstanceID;
            int CloudID=RepositoryHelper.GetCloudByInstanceID(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,InstanceId);
            linkLabel.Text = String.Format("{0} is now in the {1} state.", monitoredSqlServerStatus.InstanceName, monitoredSqlServerStatus.Severity);
            if (Common.Constants.AmazonRDSId == CloudID ||Common.Constants.MicrosoftAzureId==CloudID)
            {
                linkLabel.Image = MonitoredSqlServerStatus.GetSeverityAzureImage(monitoredSqlServerStatus.Severity);
            }
            else
            {
                linkLabel.Image = MonitoredSqlServerStatus.GetSeverityImage(monitoredSqlServerStatus.Severity);
            }
            linkLabel.Tag = monitoredSqlServerStatus;
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.Image = Resources.CloseHelpPopupHotTracked;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Image = Resources.CloseHelpPopup;
        }

        private void closeButton_MouseClick(object sender, MouseEventArgs e)
        {
            Hide();
        }

        private void messageLabel_MouseClick(object sender, MouseEventArgs e)
        {
            ApplicationController.Default.ShowConsole();
            Hide();
        }

        private void contentPanel_MouseEnter(object sender, EventArgs e)
        {
            if (currentState == PopupState.FullyVisible)
            {
                renderTimer.Stop();
            }
        }

        private void contentPanel_MouseLeave(object sender, EventArgs e)
        {
            if (currentState == PopupState.FullyVisible)
            {
                renderTimer.Interval = VisibleIntervalAfterMouseLeave;
                renderTimer.Start();
            }
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();

            MonitoredSqlServerStatus monitoredSqlServerStatus = ((Control)sender).Tag as MonitoredSqlServerStatus;
            if (monitoredSqlServerStatus != null)
            {
                AlertFilter filter = new AlertFilter();
                filter.Instance = monitoredSqlServerStatus.InstanceName;
                filter.ActiveOnly = true;
                ShowAlertsView(filter);
            }
        }

        private void OnLinkMouseEnter(object sender, EventArgs e)
        {
            if (currentState == PopupState.FullyVisible)
            {
                renderTimer.Stop();
            }
        }

        private void OnLinkMouseLeave(object sender, EventArgs e)
        {
            if (currentState == PopupState.FullyVisible)
            {
                renderTimer.Interval = VisibleIntervalAfterMouseLeave;
                renderTimer.Start();
            }
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            switch (currentState)
            {
                case PopupState.Showing:
                    Top = Top - RenderMoveDistance < Screen.PrimaryScreen.WorkingArea.Bottom - Height
                              ? Screen.PrimaryScreen.WorkingArea.Bottom - Height
                              : Top - RenderMoveDistance;

                    if (Top == Screen.PrimaryScreen.WorkingArea.Bottom - Height)
                    {
                        renderTimer.Stop();
                        currentState = PopupState.FullyVisible;
                        renderTimer.Interval = VisibleInterval;
                        renderTimer.Start();
                    }
                    break;
                case PopupState.FullyVisible:
                    renderTimer.Stop();
                    currentState = PopupState.Hiding;
                    renderTimer.Interval = RenderInterval;
                    renderTimer.Start();
                    break;
                case PopupState.Hiding:
                    Top = Top + RenderMoveDistance > Screen.PrimaryScreen.WorkingArea.Bottom
                              ? Screen.PrimaryScreen.WorkingArea.Bottom
                              : Top + RenderMoveDistance;

                    if (Top == Screen.PrimaryScreen.WorkingArea.Bottom)
                    {
                        Hide();
                    }
                    break;
            }
        }

        private void ShowAlertsView(AlertFilter filter)
        {
            ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Active, filter);
            ApplicationController.Default.ShowConsole();
        }

        private void showActiveAlertsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            AlertFilter filter = new AlertFilter();
            filter.ActiveOnly = true;
            ShowAlertsView(filter);
        }

        /// <summary>
        /// Get the full path on which is located the notification sound file. Build this path based
        /// on the application execution path, and the name of the notification sound path.
        /// </summary>
        /// 
        /// <returns>The full path on which is located the notification sound file.</returns>
        private static String GetSoundFileFullPath()
        {
            String soundFileFullPath;
            String appExecutablePath = Path.GetDirectoryName(Application.ExecutablePath);

            if (String.IsNullOrEmpty(appExecutablePath))
            {
                //Occusr if the path of the application cannot be gotten.
                soundFileFullPath = String.Empty;
                Log.Warn("The Path of the application cannot be gotten.");
            }
            else
            {
                // Get Absolute path for the sound notification file.
                soundFileFullPath = Path.Combine(appExecutablePath, NotificationSoundFileName);
            }

            return soundFileFullPath;
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                headerPanel.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                headerPanel.BackColor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                contentPanel.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                contentPanel.BackColor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
            }
            else
            {
                headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                headerPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
                contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
                contentPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(243)))), ((int)(((byte)(246)))));
            }
            
        }
        #region P/Invoke

        protected const Int32 HWND_TOPMOST = -1;
        protected const Int32 SWP_NOACTIVATE = 0x0010;
        protected const Int32 SW_SHOWNOACTIVATE = 4;

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        protected static extern bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, uint uFlags);
        [DllImport("user32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, Int32 flags);

        #endregion
    }
}
