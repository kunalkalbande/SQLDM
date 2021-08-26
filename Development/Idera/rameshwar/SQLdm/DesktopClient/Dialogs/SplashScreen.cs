using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class SplashScreen : Form
    {
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        public SplashScreen()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeComponent();
            TopMost = true;
            SetStatus(SplashScreenStatus.None);
            Shown += SplashScreen_Shown;
            AdaptFontSize();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SplashScreen(): {0}", stopWatch.ElapsedMilliseconds);
        }

        void SplashScreen_Shown(object sender, System.EventArgs e)
        {
            // now that it is shown it no longer needs topmost status
            TopMost = false;
        }

        public void SetStatus(SplashScreenStatus status)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            SuspendLayout();

            switch (status)
            {
                case SplashScreenStatus.ConnectingToRepository:
                    progressControl.Active = true;
                    progressControl.Show();
                    statusLabel.Text = "Connecting to repository...";
                    statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
                    statusLabel.Location = new Point(41, 279);
                    break;
                case SplashScreenStatus.None:
                    progressControl.Active = false;
                    progressControl.Hide();
                    statusLabel.Text = string.Empty;
                    statusLabel.ForeColor = Color.FromArgb(254, 66, 16);
                    statusLabel.Location = new Point(15, 279);
                    break;
                //Start: SqlDm 10.2 (Tushar)--Added case for main window creation status.
                case SplashScreenStatus.LoadingUI:
                    progressControl.Active = true;
                    progressControl.Show();
                    statusLabel.Text = "LoadingUI...";
                    statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
                    statusLabel.Location = new Point(41, 279);
                    break;
                //End: SqlDm 10.2 (Tushar)--Added case for main window creation status.
            }

            ResumeLayout();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SplashScreen.SetStatus: {0}", stopWatch.ElapsedMilliseconds);
        }
		//SqlDM 10.2 (Tushar)--Event handler to close the splash screen.
        public void StopSplash(object sender, System.EventArgs e)
        {
            this.Close();
        }

        //SqlDM 10.2 (Tushar)--EventHandler to change status of splash screen.
        public void ChangeStatus(SplashScreenStatus status)
        {
            SetStatus(status);
        }

        private void AdaptFontSize()
        {
            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }

    internal enum SplashScreenStatus
    {
        ConnectingToRepository,
        None,
        LoadingUI //SqlDM 10.2 (Tushar)--Added status for main window creation.
    }
}