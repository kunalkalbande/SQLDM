using System;
using System.Runtime.InteropServices;
using Idera.Newsfeed.Plugins.UI.Controls;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class PulsePlatformConnectionConfigurationControl : PulseConfigurationControlBase
    {
        public PulsePlatformConnectionConfigurationControl()
        {
            InitializeComponent();
            _testConnectionProgressControl.InnerCircleRadius = 7;
        }

        public override string Title
        {
            get { return "Identify the IDERA Newsfeed Platform computer"; }
        }

        public override string Description
        {
            get { return "Specify the name or IP address of the computer on which the IDERA Newsfeed Platform was installed."; }
        }

        public override void Initialize()
        {
            ApplicationModel.Default.RefreshPulseConfiguration();
            _pulseServerTextBox.Text = ApplicationModel.Default.PulseProvider != null
                                           ? ApplicationModel.Default.PulseProvider.PulseServer
                                           : string.Empty;
        }

        public override bool Save()
        {
            _testConnectionWorker.CancelAsync();

            string pulseServer = _pulseServerTextBox.Text.Trim();

            if (string.IsNullOrEmpty(pulseServer) || pulseServer.Contains(" "))
            {
                ApplicationMessageBox.ShowInfo(this, "Please specify a valid Machine Name or IP Address for the IDERA Newsfeed Platform.");
                return false;
            }

            try
            {
                PulseNotificationProviderInfo providerInfo;

                if (ApplicationModel.Default.PulseProvider != null)
                {
                    providerInfo = ApplicationModel.Default.PulseProvider;
                }
                else
                {
                    providerInfo = new PulseNotificationProviderInfo(true);
                    providerInfo.PulseServerPort = 5168;
                    
                    GuidAttribute attribute = Attribute.GetCustomAttribute(typeof(PulseNotificationProviderInfo),
                                                                        typeof(GuidAttribute)) as GuidAttribute;
                    if (attribute != null)
                    {
                        providerInfo.Id = new Guid(attribute.Value);
                    }
                }

                providerInfo.PulseServer = _pulseServerTextBox.Text.Trim();

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                if (ApplicationModel.Default.PulseProvider != null)
                {
                    managementService.UpdateNotificationProvider(providerInfo, false);
                }
                else
                {
                    managementService.AddNotificationProvider(providerInfo);
                }
                
                ApplicationModel.Default.RefreshPulseConfiguration();
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while saving the IDERA Newsfeed Platform connection information.", e);
                return false;
            }

            return true;
        }

        private void _testConnectionButton_Click(object sender, System.EventArgs e)
        {
            TestConnection();
        }

        private void TestConnection()
        {
            if (string.IsNullOrEmpty(_pulseServerTextBox.Text.Trim()))
            {
                ApplicationMessageBox.ShowInfo(this, "Please specify a Machine Name or IP Address.");
                return;
            }

            if (_testConnectionWorker.IsBusy) return;

            _testConnectionProgressControl.Active = _testConnectionProgressControl.Visible = true;
            _testConnectionButton.Enabled = false;
            _testConnectionWorker.RunWorkerAsync(_pulseServerTextBox.Text.Trim());
        }

        private void _testConnectionWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            PulseNotificationProviderInfo providerInfo = new PulseNotificationProviderInfo(true);
            providerInfo.PulseServer = e.Argument as string;
            providerInfo.PulseServerPort = 5168;
            IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            e.Result = managementService.TestAction(providerInfo, new PulseDestination(), null);
        }

        private void _testConnectionWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            _testConnectionProgressControl.Active = _testConnectionProgressControl.Visible = false;
            _testConnectionButton.Enabled = true;

            if (e.Error == null && (int)e.Result == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "A connection was successfully established to the specified IDERA Newsfeed Platform endpoint.");
            }
            else
            {
                ApplicationMessageBox.ShowWarning(this, "A connection could not be established to the specified IDERA Newsfeed Platform endpoint.");
            }
        }
    }
}
