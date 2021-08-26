using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using BBS.TracerX;
    using Controls;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.DesktopClient.Objects;
    using Idera.SQLdm.DesktopClient.Properties;
    using Infragistics.Win;
    using Ionic.Zip;
    using System.Diagnostics;
    using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;

    public partial class SystemDiagnosticsDialog : Form
    {
        private ValueList testStatusValueList;
        private ValueList testValueList;
        private ValueList componentValueList;
        private string configToolPath;
        private Process configToolProcess;
        private List<DiagnosticTest> testResults;

        public SystemDiagnosticsDialog()
        {
            InitializeComponent();
            _tabControl.DrawFilter = new HideFocusRectangleDrawFilter();
            _pnlPermissionServersGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            testStatusValueList = new ValueList(); 
            ValueListItem item = new ValueListItem(TestResult.Failed, "Failed");
            item.Appearance.Image = Resources.Critical32x32;
            item.Appearance.TextVAlign = VAlign.Middle;
            testStatusValueList.ValueListItems.Add(item);
            item = new ValueListItem(TestResult.Passed, "Passed");
            item.Appearance.Image = Resources.OK32x32;
            item.Appearance.TextVAlign = VAlign.Middle;
            testStatusValueList.ValueListItems.Add(item);
            item = new ValueListItem(TestResult.Unknown, "Unknown");
            item.Appearance.Image = Resources.Critical32x32;
            item.Appearance.TextVAlign = VAlign.Middle;
            testStatusValueList.ValueListItems.Add(item);
            item = new ValueListItem(TestResult.Info, "Info");
            item.Appearance.Image = Resources.Information32x32;
            item.Appearance.TextVAlign = VAlign.Middle;
            testStatusValueList.ValueListItems.Add(item);

            testValueList = new ValueList(); 
            item = new ValueListItem(Test.CanConnectToMS, "GUI to management service connection");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.DesktopAndMSUsingSameRepository, "GUI and services using same repository");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.MSRepositoryIsValid, "Management service repository connection");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.CanConnectToCS, "Management service to collection service connection");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.CSUsingSameMS, "Collection service configured to use correct management service");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.CSCanConnectToMS, "Collection service can connect to management service");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.CSHeartbeatStatus, "Collection service heartbeat status");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.NFLocation, "Newsfeed Service location");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.NFCanConnectToMS, "Newsfeed Service can connect to management service");
            testValueList.ValueListItems.Add(item);
            item = new ValueListItem(Test.NFRepositoryIsValid, "Newsfeed Service repository connection");
            testValueList.ValueListItems.Add(item);

            componentValueList = new ValueList(); 
            item = new ValueListItem(Component.CollectionService, "Collection Service");
            componentValueList.ValueListItems.Add(item);
            item = new ValueListItem(Component.DesktopClient, "Desktop Client");
            componentValueList.ValueListItems.Add(item);
            item = new ValueListItem(Component.ManagementService, "Management Service");
            componentValueList.ValueListItems.Add(item);
            item = new ValueListItem(Component.NewsFeedService, "Newsfeed Service");
            componentValueList.ValueListItems.Add(item);
            AdaptFontSize();
        }

        private void SystemDiagnosticsDialog_Load(object sender, EventArgs e)
        {
            RepositoryConnection rc = Settings.Default.ActiveRepositoryConnection;
            desktopRepositoryHostLabel.Text = rc.ConnectionInfo.InstanceName;
            desktopRepositoryDatabaseLabel.Text = rc.ConnectionInfo.DatabaseName;
            office2007PropertyPage2.Text =
                String.Format("SQLDM permissions assigned to {0}",
                              rc.ConnectionInfo.UseIntegratedSecurity
                                  ? Environment.UserDomainName + "\\" + Environment.UserName
                                  : rc.ConnectionInfo.UserName);

            // see if management service config tool exists
            string path = this.GetType().Assembly.Location;
            path = Path.GetDirectoryName(path);
            configToolPath = Path.Combine(path, "SQLdmManagementServiceConsole.exe");

            configToolButton.Visible = (File.Exists(configToolPath));
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SQLdmSystemDiagnostics);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SQLdmSystemDiagnostics);
        }

        private void LoadData()
        {
            stackLayoutPanel1.ActiveControl = panel2;
            viewStatusLabel.Text = "Initializing...";

            while (panel1.Controls.Count > 0)
            {
                using (Control control = panel1.Controls[0])
                {
                    panel1.Controls.Remove(control);
                }
            }

            // update security permissions tab
            if(ApplicationModel.Default.UserToken.IsSecurityEnabled)
            {
                if (ApplicationModel.Default.UserToken.IsAnySQLdmPermissionAssigned)
                {
                    stackLayoutPanelPermission.ActiveControl = _pnlPermissions;
                    _pnlPermissionSysadminVal.Text = (ApplicationModel.Default.UserToken.IsSysadmin ? "Yes" : "No");
                    _pnlPermissionAdminVal.Text = (ApplicationModel.Default.UserToken.IsSQLdmAdministrator ? "Yes" : "No");
                    bindingSource1.Clear();
                    foreach (ServerPermission sp in ApplicationModel.Default.UserToken.AssignedServers)
                    {
                        bindingSource1.Add(sp);
                    }
                }
                else
                {
                    _pnlNoPermissionInfo.Text = "  No SQL Diagnostic manager permission has been assigned to you.";
                    stackLayoutPanelPermission.ActiveControl = _pnlNoPermissions;
                }
            }
            else
            {
                _pnlNoPermissionInfo.Text = "  SQL Diagnostic Manager application security is disabled.";
                stackLayoutPanelPermission.ActiveControl = _pnlNoPermissions;
            }

            viewStatusConnectingCircle.Active = true;
            backgroundWorker.RunWorkerAsync();
            refreshButton.Enabled = false;
            collectAndLogButton.Enabled = false;
        }

        public void AddTestResult(DiagnosticTest test)
        {
            SystemDiagTestControl control = new SystemDiagTestControl();

            ValueListItem valueListItem = testStatusValueList.FindByDataValue(test.TestResult);
            control.SetStatusImage((Image)valueListItem.Appearance.Image);
            valueListItem = componentValueList.FindByDataValue(test.Component);
            control.SetComponentName(valueListItem.DisplayText);
            valueListItem = testValueList.FindByDataValue(test.Test);
            control.SetTestName(valueListItem.DisplayText);
            control.SetInstanceName(test.Instance.ToString());
            control.SetMessage(test.Message);
            control.Dock = DockStyle.Top;
            control.DividerVisible = true;
            panel1.Controls.Add(control);
            control.BringToFront();
         }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "SystemDiagnosticsWorker";

            List<DiagnosticTest> results = new List<DiagnosticTest>();

            BackgroundWorker worker = (BackgroundWorker)sender;
            worker.ReportProgress(10, "Retrieving management service list");

            RepositoryConnection rc = Settings.Default.ActiveRepositoryConnection;
            foreach (ManagementServiceConfiguration serviceConfig in RepositoryHelper.GetManagementServices(rc.ConnectionInfo, null))
            {
                IManagementService ims = ManagementServiceHelper.GetService(serviceConfig);
                worker.ReportProgress(20,String.Format("Getting status for management service ({0}/{1})", serviceConfig.MachineName, serviceConfig.InstanceName));
                try
                {
                    ManagementServiceStatus mss = ims.GetServiceStatus();
                    results.AddRange(AnalyzeMSStatus(mss));                    
                } catch (Exception e)
                {
                    ManagementServiceStatus mss = new ManagementServiceStatus();
                    mss.InstanceName = new InstanceName(serviceConfig.MachineName, serviceConfig.InstanceName);
                    mss.ServicePort = serviceConfig.Port;
                    mss.ConnectionException = e;
                    mss.Status = SQLdmServiceStatus.Unknown;
                    results.AddRange(AnalyzeMSStatus(mss));
                }

                // check newsfeed status
                AnalyzeNFStatus(ims, serviceConfig, results);
            }            

            worker.ReportProgress(100, "Done");
            args.Result = results;
        }

        private List<DiagnosticTest> AnalyzeMSStatus(ManagementServiceStatus mss)
        {

            List<DiagnosticTest> result = new List<DiagnosticTest>();
            bool bail = false;
            foreach (Test t in Enum.GetValues(typeof(Test)))
            {
                if (bail)
                    break;

                switch (t)
                {
                    case Test.CanConnectToMS:
                        DiagnosticTest test = new DiagnosticTest(t, Component.ManagementService, mss.InstanceName.ToString());
                        if (mss.ConnectionException != null)
                        {
                            test.TestResult = TestResult.Failed;
                            bail = true;
                        } else
                        {
                            test.TestResult = TestResult.Passed;                            
                        }
                        result.Add(test);
                        break;
                    case Test.DesktopAndMSUsingSameRepository:
                        test = new DiagnosticTest(t, Component.ManagementService, mss.InstanceName.ToString());
                        if (!mss.RepositoryHost.Equals(Settings.Default.ActiveRepositoryConnection.ServerName, StringComparison.OrdinalIgnoreCase))
                        {
                            test.TestResult = TestResult.Failed;
                            test.Message = "SQLDM Repository on different hosts";
                        }
                        else
                            if (!mss.RepositoryDatabase.Equals(Settings.Default.ActiveRepositoryConnection.DatabaseName, StringComparison.OrdinalIgnoreCase))
                            {
                                test.TestResult = TestResult.Failed;
                                test.Message = "SQLDM Repository Database is not the same";
                            }
                            else
                                test.TestResult = TestResult.Passed;
                        result.Add(test);
                        break;
                    case Test.MSRepositoryIsValid:
                        test = new DiagnosticTest(t, Component.ManagementService, mss.InstanceName.ToString());
                        test.TestResult = mss.RepositoryConnectionTestResult.Value;

                        if (mss.RepositoryConnectionTestException != null)
                        {
                            test.Message = mss.RepositoryConnectionTestException.Message;
                            bail = true;
                        }
                        result.Add(test);
                        break;
                    default:
                        break;
                }
            }
            foreach (CollectionServiceStatus css in mss.CollectionServices)
            {
                result.AddRange(AnalyzeCSStatus(mss, css));
            }

            return result;
        }

        private IEnumerable<DiagnosticTest> AnalyzeCSStatus(ManagementServiceStatus mss, CollectionServiceStatus css)
        {
            List<DiagnosticTest> result = new List<DiagnosticTest>();
            bool bail = false;
            foreach (Test t in Enum.GetValues(typeof(Test)))
            {
                if (bail)
                    break;
                switch (t)
                {
                    case Test.CanConnectToCS:
                        DiagnosticTest test = new DiagnosticTest(t, Component.CollectionService, css.InstanceName.ToString());
                        if (css.Status == SQLdmServiceStatus.Unknown)
                        {
                            test.TestResult = TestResult.Failed;
                            test.Message = css.CollectionServiceConnectionException.Message;
                            bail = true;
                        }
                        else
                            test.TestResult = TestResult.Passed;
                        result.Add(test);
                        break;
                    case Test.CSUsingSameMS:
                        test = new DiagnosticTest(t, Component.CollectionService, css.InstanceName.ToString());
                        if (!css.ManagementServiceAddress.Equals(mss.InstanceName.Machine, StringComparison.OrdinalIgnoreCase))
                        {
                            test.TestResult = TestResult.Failed;
                            test.Message = "Collection Service using a different Management Service";
                        } else
                        if (css.ManagementServicePort == null || css.ManagementServicePort.Value != mss.ServicePort.Value)
                        {
                            test.TestResult = TestResult.Failed;
                            test.Message = "Collection Service using a different Management Service";
                            bail = true;
                        } else
                            test.TestResult = TestResult.Passed;
                        result.Add(test);
                        break;
                    case Test.CSCanConnectToMS:
                        test = new DiagnosticTest(t, Component.CollectionService, css.InstanceName.ToString());
                        if (css.ManagementServiceTestResult != null)
                        {
                            test.TestResult = css.ManagementServiceTestResult.Value;
                            if (css.ManagementServiceTestException != null)
                                test.Message = css.ManagementServiceTestException.Message;
                        }
                        else
                            test.TestResult = TestResult.Unknown;
                        result.Add(test);
                        break;
                    case Test.CSHeartbeatStatus:
                        test = new DiagnosticTest(t, Component.CollectionService, css.InstanceName.ToString());
                        DateTime last;
                        DateTime next;
                        if (css.LastHeartbeatReceived == null)
                        {
                            test.TestResult = TestResult.Failed;
                            test.Message = "The management service has never received a heartbeat for this collection service";
                        }
                        else
                        {
                            last = css.LastHeartbeatReceived.Value.ToLocalTime();
                            if (css.NextHeartbeatExpected != null)
                                next = css.NextHeartbeatExpected.Value.ToLocalTime();
                            else
                                next = last;
                            if (next < DateTime.Now - TimeSpan.FromMinutes(1))
                            {
                                test.TestResult = TestResult.Failed;
                                test.Message = String.Format(
                                        "Last heartbeat received {0}.  Next heartbeat expected {1} and is past due.",
                                        last, next);
                            }
                            else
                            {
                                test.TestResult = TestResult.Passed;
                                test.Message =
                                    String.Format("Last heartbeat received {0}.  Next heartbeat expected {1}.",
                                                  last, next);
                            }
                        }
                        result.Add(test);
                        break;


                    default:
                        break;
                }
            }

            return result;
        }

        private void AnalyzeNFStatus(IManagementService ims, ManagementServiceConfiguration msServiceConfig, List<DiagnosticTest> results)
        {            
            try
            {
                IList<NotificationProviderInfo> npList = ims.GetNotificationProviders();

                foreach (NotificationProviderInfo provider in npList)
                {
                    if (provider.Name != "Newsfeed Action Provider")
                        continue;

                    string newsfeedhostname = string.Empty;

                    for (int i = 0; i < provider.Properties.Count; i++)
                    {
                        if (provider.Properties[i].Name == "PulseServer")
                        {
                            newsfeedhostname = provider.Properties[i].Value as string;
                            break;
                        }
                    }

                    DiagnosticTest test;

                    if (string.IsNullOrEmpty(newsfeedhostname))
                    {
                        test = new DiagnosticTest(Test.NFLocation, Component.NewsFeedService, newsfeedhostname);
                        test.TestResult = TestResult.Failed;
                        test.Message = "No Newsfeed service was found.";
                    }
                    else
                    {
                        test = new DiagnosticTest(Test.NFLocation, Component.NewsFeedService, newsfeedhostname);
                        test.TestResult = TestResult.Info;
                        test.Message = string.Format("Newsfeed on {0} is connected to management service on {1}.", newsfeedhostname, msServiceConfig.MachineName);
                    }

                    results.Add(test);

                    break;
                }
            }
            catch
            {
            }            
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            viewStatusLabel.Text = e.UserState.ToString();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Exception error = e.Error;
            if (error != null)
            {
                ApplicationMessageBox.ShowError(this,
                                "Unable to connect to the management service.  Please resolve the following error and try again.",
                                error);
            }
            else
            {
                viewStatusConnectingCircle.Active = false;
                testResults = (List<DiagnosticTest>)e.Result;
                foreach (DiagnosticTest test in (List<DiagnosticTest>) e.Result)
                {
                    AddTestResult(test);
                }
            }
            stackLayoutPanel1.ActiveControl = panel1;
            refreshButton.Enabled = true;
            collectAndLogButton.Enabled = true;
        }

        private void rulesListView_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            

        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        public class DiagnosticTest
        {
            private Test test;
            private Component component;
            private string instance;
            private TestResult result = TestResult.Unknown;
            private string message = "";
            private object source = null;

            public DiagnosticTest(Test test, Component component, string instance) 
            {
                this.test = test;
                this.instance = instance;
                Component = component;
            }

            public Test Test
            {
                get { return test; }
            }
            public TestResult TestResult
            {
                get { return result; }
                set { result = value; }
            }
            public Component Component
            {
                get { return component; }
                set { component = value; }
            }
            public string Instance
            {
                get { return instance; }
            }
            public string Message
            {
                get
                {
                    if (String.IsNullOrEmpty(message) && TestResult == TestResult.Passed)
                        return "Test completed successfully.";
                    return message;
                }
                set { message = value;  }
            }
            public object GetSource()
            {
                return source;
            } 
        }

        public enum Component
        {
            ManagementService,
            CollectionService,
            DesktopClient,
            NewsFeedService
        }

        public enum Test
        {
            CanConnectToMS,                     // Desktop connect to management service
            DesktopAndMSUsingSameRepository,    // Desktop using same repository as management service
            MSRepositoryIsValid,                // Management service can connect to repository and repository is valid
            CanConnectToCS,                     // Management service can connect to collection service
            CSUsingSameMS,                      // Collection service using same management service
            CSCanConnectToMS,                   // Collection service can connect to management service
            CSHeartbeatStatus,                  // Collection service heartbeat status
            NFCanConnectToMS,                   // Newsfeed can connect to management service
            NFRepositoryIsValid,                // Newsfeed can connect to its repository and repository is valid
            NFLocation                          // Newsfeed location (host)
        }

        private void SystemDiagnosticsDialog_Shown(object sender, EventArgs e)
        {
            LoadData();
        }

        private void configToolButton_Click(object sender, EventArgs e)
        {
            if (configToolProcess != null && !configToolProcess.HasExited)
            {
                IntPtr windowHandle = configToolProcess.MainWindowHandle;
                Helpers.NativeMethods.ShowWindow(windowHandle, Helpers.NativeMethods.SW_RESTORE);
                Helpers.NativeMethods.SetForegroundWindow(windowHandle);
                return;
            }

            configToolProcess = null;
            ProcessStartInfo psi = new ProcessStartInfo(configToolPath);
            psi.CreateNoWindow = true;
            psi.ErrorDialog = true;
            psi.ErrorDialogParentHandle = Handle;
            psi.WindowStyle = ProcessWindowStyle.Normal;

            configToolProcess = System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }


        //SQLdm 8.5 (Ankit Srivastava): for One Click Diagnostics - starts here
        private void collectAndLogButton_Click(object sender, EventArgs e)
        {
            LoadData();//Run the diagnostics

            // Show the dialog and get result.
            SaveFileDialog newDialog = new SaveFileDialog();
            newDialog.Filter = "Compressed file|*.zip|All files|*.*";
            newDialog.DefaultExt = "zip";
            newDialog.InitialDirectory = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
            newDialog.FileName = "SQLdmLogs" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            newDialog.Title = "Save the Compressed Log File";

            DialogResult result = newDialog.ShowDialog();

            if (result == DialogResult.OK && !String.IsNullOrWhiteSpace(newDialog.FileName))
            {
                
                string tempTxtFileName = String.Empty;
                if (testResults != null)
                {
                    StringBuilder data = new StringBuilder();
                    foreach (DiagnosticTest test in testResults)
                    {
                        WriteTestResult(data, test);
                    }

                //Create temp text file 
                    tempTxtFileName = Path.Combine(Path.GetTempPath(), "SystemDiagnosticsReport.txt");
                FileStream fs = new FileStream(tempTxtFileName, FileMode.OpenOrCreate);
                
                using (StreamWriter myStream = new StreamWriter(fs))
                {
                        myStream.Write(data);           //write the log into the temp text file
                }
                fs.Close();
                }

                //Get log paths
                string logForSqlDm = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Common.Constants.SQLdmClientLogs);                
                //string logForServices = Path.Combine(Directory.GetCurrentDirectory() ,"Logs");

                string logForServices = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),Common.Constants.SQLdmServicesLogs);
                

                //zip the text file
                using (ZipFile zip = new ZipFile())
                {
                    if(File.Exists(tempTxtFileName))
                    zip.AddFile(tempTxtFileName, String.Empty);

                    if (Directory.Exists(logForServices))
                        zip.AddDirectory(logForServices,Common.Constants.SysDiagnosticsLogs);

                    if (Directory.Exists(logForSqlDm))
                        zip.AddDirectory(logForSqlDm, Common.Constants.SysDiagnosticsLogs);
                    
                    zip.Save(newDialog.FileName);
                }
                //delete the temp text file
                if (File.Exists(tempTxtFileName))
                File.Delete(tempTxtFileName);
            }
        }

        /// <summary>
        /// Writes the Dignostics Test result in a StringBuilder
        /// </summary>
        /// <param name="data"></param>
        /// <param name="test"></param>
        private void WriteTestResult(StringBuilder data,DiagnosticTest test)
        {
            data.AppendLine(Common.Constants.SysDiaCompName + test.Component);
            data.AppendLine(Common.Constants.SysDiaInstName + test.Instance);
            data.AppendLine(Common.Constants.SysDiaTestName + test.Test);
            data.AppendLine(Common.Constants.SysDiaTestResult + test.TestResult);
            data.AppendLine(Common.Constants.SysDiaMessage + test.Message);
            data.AppendLine(Common.Constants.SysDiaSepartor);
            data.AppendLine();
        }
        //SQLdm 8.5 (Ankit Srivastava): for One Click Diagnostics - ends here
     
    }
}