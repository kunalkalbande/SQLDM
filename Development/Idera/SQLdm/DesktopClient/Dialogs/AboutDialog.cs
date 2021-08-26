using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Microsoft.Win32;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.Newsfeed.Plugins.UI;
    using Infragistics.Windows.Themes;

    internal partial class AboutDialog : BaseDialog
    {
        private IManagementServiceConfiguration configService;
        private static CommonAssemblyInfo assemblyInfo = new CommonAssemblyInfo();
        private string msInfoPath;

        public AboutDialog()
        {
            this.DialogHeader = "AboutBox";
            InitializeComponent();

            Text = String.Format("About {0}", Application.ProductName);
            BuildComponentList();
            systemInfoButton.Visible = GetMsinfo32Path(out msInfoPath);
            AdaptFontSize();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        private void BuildComponentList()
        {
            ListViewItem desktopClientInfo = new ListViewItem(ApplicationHelper.AssemblyTitle);
            desktopClientInfo.SubItems.Add(ApplicationHelper.AssemblyVersion);

            RepositoryConnection repository = GetActiveConnection();
            ListViewItem repositoryInfo = new ListViewItem("IDERA SQL Diagnostic Manager Repository");
            repositoryInfo.SubItems.Add(repository != null ? repository.RepositoryInfo.VersionString : "No repository selected");

            ListViewItem managementServiceInfo = new ListViewItem("IDERA SQL Diagnostic Manager Management Service");
            managementServiceInfo.SubItems.Add(GetManagementServiceVersion());

            ListViewItem collectionServiceInfo = new ListViewItem("IDERA SQL Diagnostic Manager Collection Service");
            collectionServiceInfo.SubItems.Add(GetDefaultCollectionServiceVersion());

            Version mdac = GetMDACVersion();
            ListViewItem mdacInfo = new ListViewItem("Microsoft Data Access Component (MDAC)");
            mdacInfo.SubItems.Add(mdac != null ? mdac.ToString() : "Unknown");

            ListViewItem frameworkInfo = new ListViewItem("Microsoft .Net Framework");
            frameworkInfo.SubItems.Add(Environment.Version.ToString());

            ListViewItem osInfo = new ListViewItem("Microsoft Windows Operating System");
            osInfo.SubItems.Add(Environment.OSVersion.ToString());

            ListViewItem pulseVersion = new ListViewItem("SQLDM Mobile and Newsfeed Version");
            pulseVersion.SubItems.Add(GetPulseVersion());

            componentListView.Items.AddRange(
                new ListViewItem[]
                    {
                        desktopClientInfo, repositoryInfo, managementServiceInfo, collectionServiceInfo, mdacInfo,
                        frameworkInfo, osInfo, pulseVersion
                    });
        }

        private void systemInfoButton_Click(object sender, EventArgs e)
        {
            ShowSystemInfoDialog();
        }

        private void ShowSystemInfoDialog()
        {
            try
            {
                if (msInfoPath != null)
                {
                    Process.Start(msInfoPath);
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "The system information tool (MSInfo32.exe) is not installed.", null);
                }
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                                                "An error occurred while attempting to show the system information tool (MSInfo32.exe).",
                                                e);
            }
        }

        private string GetManagementServiceVersion()
        {
            RepositoryConnection repository = GetActiveConnection();
            if (repository != null) {
                ManagementServiceConfiguration serviceConfig = null;
                if (configService == null)
                {
                    try
                    {
                        // make dummy call to ManagementServiceHelper to ensure remoting is configured
                        string dummy = ManagementServiceHelper.ServerName;
                        // get the service config record from the repository
                        serviceConfig = RepositoryHelper.GetDefaultManagementService(repository.ConnectionInfo);
                        // create a url to the management service
                        string uri =
                            String.Format("tcp://{0}:{1}/Configuration", serviceConfig.Address, serviceConfig.Port);
                        // get the service interface
                        configService = RemotingHelper.GetObject<IManagementServiceConfiguration>(uri.ToString());
                    }
                    catch
                    {
                        return "Unable to connect to management service";
                    }
                }
            }
            try
            {
                return configService.GetCommonAssemblyVersion();    
            } catch (Exception e)
            {
                return "Unknown: " + e.Message;
            }
        }

        private string GetDefaultCollectionServiceVersion()
        {
            try
            {
                return configService.GetCollectionServiceCommonAssemblyVersion(null);
            }
            catch (Exception e)
            {
                return "Unknown: " + e.Message;
            }
        }

        public static RepositoryConnection GetActiveConnection()
        {
            return Settings.Default.ActiveRepositoryConnection;
        }

        public static Version GetMDACVersion()
        {
            // get the value of the FullInstallVer value under the HKLM\SOFTWARE\Microsoft\DataAccess key
            RegistryKey regKey = Registry.LocalMachine;
            if (regKey != null)
            {
                regKey = regKey.OpenSubKey("SOFTWARE\\Microsoft\\DataAccess");
                if (regKey != null)
                {
                    string ver = regKey.GetValue("FullInstallVer", "0.0.0.0") as string;
                    regKey.Close();

                    if (!String.IsNullOrEmpty(ver))
                        return new Version(ver);
                }
            }

            return null;
        }

        private static bool GetMsinfo32Path(out string path)
        {
            path = null;

            path = Path.Combine(Environment.SystemDirectory, "MSInfo32.exe");

            return File.Exists(path);
        }

        private string GetPulseVersion()
        {
            try
            {
                return PulseController.Default.GetVersion().ToString();
            }
            catch
            {
                return "N/A";
            }
        }

        private void copyInfoButton_Click(object sender, EventArgs e)
        {
            StringBuilder clipboardText = new StringBuilder();

            for (int i = 0; i < componentListView.Items.Count; i++)
            {
                ListViewItem componentItem = componentListView.Items[i];
                clipboardText.Append(componentItem.Text);
                clipboardText.Append("\t");
                clipboardText.Append(componentItem.SubItems[1].Text);

                if (i < componentListView.Items.Count - 1)
                {
                    clipboardText.Append("\r\n");
                }
            }

            Clipboard.SetText(clipboardText.ToString());
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            if (AutoScaleSizeHelper.isScalingRequired && !(Settings.Default.ColorScheme == "Dark"))
            {
                this.okButton.Text = "      OK        ";
            }
        }
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            if (Idera.SQLdm.DesktopClient.Properties.Settings.Default.ColorScheme == "Dark")
            {
                this.gradientPanel1.BackColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                this.gradientPanel1.BackColor2 = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
            }
        }
    }
}