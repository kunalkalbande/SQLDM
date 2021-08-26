using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.Common.UI.Dialogs;

    internal partial class HelpPopupDialog : BaseDialog
    {
        private const string HelpFileLinkPrefix = @"mk:@MSITStore:";
        private const string HelpFileNameReference = @"SQLdm Metrics Help.chm::";

        private readonly int instanceId;
        private readonly string helpFileLink;
        private bool isMoving = false;
        private Point mouseDownLocation;
        private string helpTopic = String.Empty;
        private ServerViews detailsView = ServerViews.OverviewSummary;
        private object detailsViewArgument = null;
        private Metric? metric = null;

        public HelpPopupDialog(int instanceId)
        {
            this.DialogHeader = "HelpPopupDialog";
            InitializeComponent();

            this.instanceId = instanceId;
            string applicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            helpFileLink = HelpFileLinkPrefix + Path.Combine(applicationDirectory, HelpFileNameReference);
            AdaptFontSize();
        }

        public string HelpTopic
        {
            get { return helpTopic; }
            set
            {
                helpTopic = value;
                webBrowser.Url = new Uri(helpFileLink + helpTopic);
            }
        }

        public ServerViews DetailsView
        {
            get { return detailsView; }
            set { detailsView = value; }
        }

        public object DetailsViewArgument
        {
            get { return detailsViewArgument; }
            set { detailsViewArgument = value; }
        }

        public Metric? Metric
        {
            get { return metric; }
            set { metric = value; }
        }

        public bool ConfigureAlertsOptionVisible
        {
            get { return configureAlertsLinkLabel.Visible; }
            set { configureAlertsLinkLabel.Visible = value; }
        }

        public bool ShowDetailsLinkLabel
        {
            get { return showDetailsLinkLabel.Visible; }
            set { showDetailsLinkLabel.Visible = value; }
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
            // Because this dialog is reused, clicking the close image will
            // only hide the dialog
            Hide();
        }

        public void SetStartPosition(Point point)
        {
            Rectangle workingArea = Screen.GetWorkingArea(point);

            if (point.X < workingArea.Left)
            {
                point.X = workingArea.Left;
            }

            if (point.X + Width > workingArea.Right)
            {
                point.X = workingArea.Right - Width;
            }

            if (point.Y < workingArea.Top)
            {
                point.Y = workingArea.Top;
            }

            if (point.Y + Height > workingArea.Bottom)
            {
                point.Y = workingArea.Bottom - Height;
            }

            Location = point;
        }

        private void OnHeaderMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMoving = true;
                mouseDownLocation = e.Location;
            }
        }

        private void OnHeaderMouseMove(MouseEventArgs e)
        {
            if (isMoving)
            {
                Location = new Point(Location.X + (e.X - mouseDownLocation.X),
                                     Location.Y + (e.Y - mouseDownLocation.Y));
            }
        }

        private void OnHeaderMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMoving = false;
            }
        }

        private void dialogIcon_MouseDown(object sender, MouseEventArgs e)
        {
            OnHeaderMouseDown(e);
        }

        private void dialogIcon_MouseMove(object sender, MouseEventArgs e)
        {
            OnHeaderMouseMove(e);
        }

        private void dialogIcon_MouseUp(object sender, MouseEventArgs e)
        {
            OnHeaderMouseUp(e);
        }

        private void titleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            OnHeaderMouseDown(e);
        }

        private void titleLabel_MouseMove(object sender, MouseEventArgs e)
        {
            OnHeaderMouseMove(e);
        }

        private void titleLabel_MouseUp(object sender, MouseEventArgs e)
        {
            OnHeaderMouseUp(e);
        }

        private void headerPanel_MouseDown(object sender, MouseEventArgs e)
        {
            OnHeaderMouseDown(e);
        }

        private void headerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            OnHeaderMouseMove(e);
        }

        private void headerPanel_MouseUp(object sender, MouseEventArgs e)
        {
            OnHeaderMouseUp(e);
        }

        private void showDetailsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();

            if (detailsViewArgument != null && metric.HasValue && metric.Value == Idera.SQLdm.Common.Events.Metric.Deadlock)
            {
                DeadlockDialog.Show(this, (long)detailsViewArgument);
                return;
            }

            if (detailsViewArgument == null)
            {
                ApplicationController.Default.ShowServerView(instanceId, detailsView);
            }
            else
            {
                ApplicationController.Default.ShowServerView(instanceId, detailsView, detailsViewArgument);
            }
        }

        private void configureAlertsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            try
            {
                using (AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false))
                {
                    if (metric.HasValue)
                    {
                        dialog.Select(metric.Value);
                    }

                    dialog.ShowDialog(ParentForm);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private void showHelpContentsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerDashboardViewNeedMoreHelp);
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