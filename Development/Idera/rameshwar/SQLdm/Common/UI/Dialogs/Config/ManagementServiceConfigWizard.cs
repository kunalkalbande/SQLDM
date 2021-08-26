namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Threading;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.Common.UI.Dialogs.Config;
    using BBS.TracerX;
    using System.Management;
    using Microsoft.Win32;


    public partial class ManagementServiceConfigWizard : Form
    {
        #region types & constants

        private delegate ManagementServiceConfigurationMessage GetManagementServiceConfiguration();
        private delegate void ShowHelperDialog();

        private class Pair<T, V>
        {
            public T First;
            public V Second;

            public Pair(
                    T first,
                    V second
                )
            {
                First = first;
                Second = second;
            }
        }

        private const string DefaultMSInstance = "Default";
        private const int DefaultMSPort = 5166;
        private const string MSProcessName = "SQLdmManagementService";
        private const string WmiQueryMSCOnfiguration = "SELECT * FROM ManagementServiceConfiguration";

        private const string BrowseForServersText = "<Browse for more...>";
        private const string BrowseForDatabasesText = "<Browse server...>";
        private const int WindowsAuthenticationIndex = 0;
        private const int SqlAuthenticationIndex = 1;

        #endregion

        #region members

        private ServiceInstance m_Service;
        private ManagementServiceConfigurationMessage m_Configuration;
        private BackgroundWorker m_BackgroundWorker;
        private Exception m_ConnectException;
        private Exception m_TestException;

        private readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("MSConfigWizard");

        #endregion

        #region ctors

        public ManagementServiceConfigWizard()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.SQLdmConfigWizard;

            // Set the wizard start page
            wizard.SelectedPage = _introductionPage; 
        }

        #endregion

        #region events

        #region load & introduction page

        private void ConfigWizardDialog_Load(object sender, EventArgs e)
        {
            showConnecting();
            try
            {
                configureRemoting();
            }
            catch (Exception ex)
            {
                Log.Warn("Exception encountered when configuring remoting, ", ex);
            }
            startConnectWorker();
        }

        private void wizard_Cancel(object sender, EventArgs e)
        {
            cancelWorker();
            Application.Exit();
        }

        private void _lnklbl_ConnectionStatus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_ConnectException != null)
            {
                ApplicationMessageBox.ShowError(this, "Error connecting to SQLdm Management Service", m_ConnectException);
            }
        }

        private void showConnecting()
        {
            _introductionPage.AllowMoveNext = false;

            _loadingCircle.Visible = _loadingCircle.Active = true;

            _lbl_Status.Visible = false;
            _lnklbl_ConnectionStatus.Text = "Connecting to SQLdm Management Service...";
            _lnklbl_ConnectionStatus.LinkArea = new LinkArea(0, 0);
        }

        private void showConnectingError()
        {
            _introductionPage.AllowMoveNext = false;

            _loadingCircle.Visible = _loadingCircle.Active = false;
            _loadingCircle.SendToBack();

            _lbl_Status.Visible = false;
            _lnklbl_ConnectionStatus.Text = "Failed to connect to the Management Service, click for details";
            _lnklbl_ConnectionStatus.LinkArea = new LinkArea(45, 17);
        }

        private void showClickNext()
        {
            _introductionPage.AllowMoveNext = true;

            _loadingCircle.Visible = _loadingCircle.Active = false;
            _pctrbx_IntroError.Visible = false;

            _lbl_Status.Text = "Connected to SQLdm Management Service on port " + m_Service.ServicePort.ToString() + ".";
            _lbl_Status.Visible = true;
            _lnklbl_ConnectionStatus.Location = new System.Drawing.Point(0, 259);
            _lnklbl_ConnectionStatus.Text = "To continue, click Next.";
            _lnklbl_ConnectionStatus.LinkArea = new LinkArea(0, 0);
        }

        private void configureRemoting()
        {
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

            // register a server channel
            IDictionary properties = new System.Collections.Specialized.ListDictionary();

            // register a client channel
            properties = new System.Collections.Specialized.ListDictionary();
            properties["name"] = "tcp-client";
//            properties["impersonationLevel"] = "None";
//            properties["impersonate"] = false;
            properties["secure"] = false;

            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();

            TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
            ChannelServices.RegisterChannel(tcpClientChannel, false);

            // register the server channel
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Debug.Print("Registered channel: " + channel.ChannelName);
            }
        }

        private void cancelWorker()
        {
            if (m_BackgroundWorker != null)
            {
                m_BackgroundWorker.CancelAsync();
            }
        }

        private void startConnectWorker()
        {
            // Create and start the background worker thread to connect.
            m_BackgroundWorker = null;
            m_BackgroundWorker = new BackgroundWorker();
            m_BackgroundWorker.WorkerSupportsCancellation = true;
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(connect_DoWork);
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(connect_RunWorkerCompleted);

            // Start the thread.
            m_BackgroundWorker.RunWorkerAsync();
        }

        private void connect_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the background thread.
            BackgroundWorker bgw = sender as BackgroundWorker;
            if (bgw == null)
            {
                Log.Error("The DoWork sender object is not a BackgroundWorker");
                throw (new Exception("The sender parameter is not a BackgroundWorker"));
            }

            // Check if MS is running.
            checkMSRunning(bgw);

            // Try to get MS configuration using the default port information.
            ServiceInstance service = new ServiceInstance(Environment.MachineName, DefaultMSInstance, DefaultMSPort);
            ManagementServiceConfigurationMessage config = null;
            try
            {
                if (!bgw.CancellationPending)
                {
                    config = getMSConfig(bgw, service);
                }
            }
            catch (Exception ex)
            {
                Log.Info("Failed to get MS configuration using default port settings, discover port settings and retry ", ex);
            }

            // If failed to get MS config, discover port settings and retry.
            if (!bgw.CancellationPending && config == null)
            {
                service = discoverMSInstance(service.MachineName);
                config = getMSConfig(bgw, service);
            }

            // Process the results.
            if (bgw.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                e.Result = new Pair<ServiceInstance, ManagementServiceConfigurationMessage>(service, config);
            }
        }

        private void connect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker bgw = sender as BackgroundWorker;
            Debug.Assert(bgw != null);

            if (!bgw.CancellationPending)
            {
                if (e.Error != null)
                {
                    Log.Error("Connection worker error, ", e.Error);
                    m_ConnectException = e.Error;
                    showConnectingError();
                }
                else
                {
                    Pair<ServiceInstance, ManagementServiceConfigurationMessage> result = e.Result as Pair<ServiceInstance, ManagementServiceConfigurationMessage>;
                    Debug.Assert(result != null);
                    m_Service = result.First;
                    m_Configuration = result.Second;
                    showClickNext();
                }
            }
            else
            {
                Log.Info("Connection worker thread cancelled.");
            }
        }

        private void checkMSRunning(
                BackgroundWorker bgw
            )
        {
            Debug.Assert(bgw != null);

            bool isRunning = false;
            try
            {
                Process[] currentProcesses = Process.GetProcesses();
                foreach (Process p in currentProcesses)
                {
                    if (bgw.CancellationPending)
                    {
                        break;
                    }

                    if (p.ProcessName == MSProcessName)
                    {
                        isRunning = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when checking if SQLdm Management Service is running, ", ex);
                throw (new Exception("Exception was encountered while checking if SQLdm Management Service is running.", ex));
            }
            finally
            {
                if (!bgw.CancellationPending && !isRunning)
                {
                    throw (new Exception("SQLdm Management Service is not running on " + Environment.MachineName));
                }
            }
        }

        private ManagementServiceConfigurationMessage getMSConfig(
                BackgroundWorker bgw,
                ServiceInstance si
            )
        {
            ManagementServiceConfigurationMessage config = null;
            try
            {
                // Invoke remote method to get MS configuration.
                Uri uri = new Uri(String.Format("tcp://{0}:{1}/Configuration",
                                        new object[] { si.MachineName, si.ServicePort }));
                IManagementServiceConfiguration service = RemotingHelper.GetObject<IManagementServiceConfiguration>(uri.ToString());
                GetManagementServiceConfiguration asyncDelegate = new GetManagementServiceConfiguration(service.GetManagementServiceConfiguration);
                IAsyncResult iar = asyncDelegate.BeginInvoke(null, null);

                // Wait for the results.
                while (!iar.IsCompleted && !bgw.CancellationPending)
                {
                    Thread.Sleep(250);
                }

                // Get results if not cancelled.
                config = !bgw.CancellationPending ? asyncDelegate.EndInvoke(iar) : null;
            }
            catch (Exception ex)
            {
                Log.Warn("Exception encountered when getting MS config info, ", ex);
                throw (new Exception("Exception encountered when retrieving SQLdm Management Service configuration", ex));
            }

            return config;
        }

        private ServiceInstance discoverMSInstance(
                string machineName
            )
        {
            // Construct path and query for MS configuration.
            string path = String.Format(@"\\{0}\root\Idera\SQLdm\ManagementService", machineName);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(path, WmiQueryMSCOnfiguration);

            // Get the first configuration, there should be only one.
            ServiceInstance si = null;
            try
            {
                int cntr = 0;
                foreach (ManagementObject mbo in searcher.Get())
                {
                    if (cntr++ == 0)
                    {
                        si = new ServiceInstance(machineName, mbo["InstanceName"].ToString(),
                            (int)mbo["ServicePort"]);
                    }
                }
                Log.Debug("Number of Management Services registered " + cntr);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get MS port information using WMI, ", ex);
                throw (new Exception("Exception was encountered when discovering SQLdm Management Service port", ex));
            }
            finally
            {
                if (si == null)
                {
                    throw (new Exception("Failed to get SQLdm Management Service port information."));
                }
            }

            return si;
        }

        #endregion

        #region settings page


        private bool settingsInitialized;
        private void settingsPage_BeforeDisplay(object sender, EventArgs e)
        {
            // Initialize only once.
            if (!settingsInitialized) {
                settingsInitialized = true;
                hideTestStatus();
                setServerDropDownItems(m_Configuration.RepositoryHost);
                setDatabaseDropDownItems(m_Configuration.RepositoryDatabase);
                cboAuthentication.SelectedIndex = m_Configuration.WindowsAuthentication ?
                                                    WindowsAuthenticationIndex : SqlAuthenticationIndex;
                if (!m_Configuration.WindowsAuthentication) {
                    txtLoginID.Text = m_Configuration.RepositoryUsername;
                    txtPassword.Text = m_Configuration.RepositoryPassword;
                    txtLoginID.Enabled = true;
                    txtPassword.Enabled = true;
                }
            }
        }

        private void settingsPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            //e.Cancel = true;
            //wizard.SelectedPage = selectServicePage;
        }

        private void settingsPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            if (isRepositoryInfoChanged())
            {
                try
                {
                    testRepositoryConnection(m_Service, getConnnectionInfo());
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered when testing connection info");
                    ApplicationMessageBox.ShowError(this, "The SQLdm Management Service was unable to validate the specified Repository information.", ex);
                    e.Cancel = true;
                }
            }
        }

        private void cboServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedServer = cboServer.SelectedItem as string;
            if (selectedServer != null)
            {
                if (selectedServer == BrowseForServersText)
                {
                    ShowHelperDialog method = delegate() {
                                SqlServerBrowserDialog ssbd = new SqlServerBrowserDialog();
                                if (ssbd.ShowDialog(this) == DialogResult.OK)
                                {
                                    string instance = ssbd.SelectedInstance;
                                    cboServer.SelectedIndex = cboServer.Items.IndexOf(instance);
                                    cboServer.Text = instance;
                                }
                                else
                                {
                                    setDefaultItem(cboServer);
                                }
                                updateSettingsPageControls();
                            };
                    BeginInvoke(method);
                }
                else
                {
                    cboServer.Text = selectedServer;
                    updateSettingsPageControls();
                }
            }
        }

        private void cboDatabase_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedDatabase = cboDatabase.SelectedItem as string;
            if (selectedDatabase != null)
            {
                if (selectedDatabase == BrowseForDatabasesText)
                {
                    ShowHelperDialog method = delegate() {
                                Idera.SQLdm.Common.Configuration.SqlConnectionInfo connectionInfo = getServerConnnectionInfo();
                                DatabaseBrowserDialog ssbd = new DatabaseBrowserDialog(connectionInfo);
                                if (ssbd.ShowDialog(this) == DialogResult.OK)
                                {
                                    string instance = ssbd.SelectedDatabase;
                                    cboDatabase.SelectedIndex = cboDatabase.Items.IndexOf(instance);
                                    cboDatabase.Text = instance;
                                }
                                else
                                {
                                    setDefaultItem(cboDatabase);
                                }
                                updateSettingsPageControls();
                            };
                    BeginInvoke(method);
                }
                else
                {
                    cboDatabase.Text = selectedDatabase;
                    updateSettingsPageControls();
                }
            }

            //object selectedItem = cboDatabase.SelectedItem;
            //if (selectedItem is string && selectedItem == BrowseForDatabasesText)
            //{
            //    ShowHelperDialog method =
            //        delegate()
            //        {
            //            Idera.SQLdm.Common.Configuration.SqlConnectionInfo connectionInfo = getConnnectionInfo();
            //            DatabaseBrowserDialog ssbd = new DatabaseBrowserDialog(connectionInfo);
            //            if (ssbd.ShowDialog(this) == DialogResult.OK)
            //            {
            //                string instance = ssbd.SelectedDatabase;
            //                cboDatabase.SelectedIndex = cboDatabase.Items.IndexOf(instance);
            //                cboDatabase.Text = instance;
            //            }
            //            else
            //                setDefaultItem(cboDatabase);
            //        };
            //    BeginInvoke(method);
            //}
        }

        private void cboServer_TextUpdate(object sender, EventArgs e)
        {
            updateSettingsPageControls();
        }

        private void cboDatabase_TextUpdate(object sender, EventArgs e)
        {
            updateSettingsPageControls();
        }

        private void cboAuthentication_TextUpdate(object sender, EventArgs e)
        {
            updateSettingsPageControls();
        }

        private void txtLoginID_TextChanged(object sender, EventArgs e)
        {
            updateSettingsPageControls();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            updateSettingsPageControls();
        }

        private void cboAuthentication_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtLoginID.Enabled = txtPassword.Enabled = cboAuthentication.SelectedIndex == SqlAuthenticationIndex;
            updateSettingsPageControls();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            showTestInProgress();
            startTestWorker();
        }

        private void testStatus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_TestException != null)
            {
                ApplicationMessageBox.ShowError(this, "Error connecting to the Repository", m_TestException);
            }
        }

        private void updateSettingsPageControls()
        {
            bool isEnable = !string.IsNullOrEmpty(cboServer.Text)
                            && !string.IsNullOrEmpty(cboDatabase.Text)
                            && (cboAuthentication.SelectedIndex == WindowsAuthenticationIndex
                                || (!string.IsNullOrEmpty(txtLoginID.Text)
                                        && !string.IsNullOrEmpty(txtPassword.Text)));
            btnTest.Enabled = settingsPage.AllowMoveNext = isEnable;
        }

        private void setServerDropDownItems(string server)
        {
            cboServer.Items.Clear();
            if (!String.IsNullOrEmpty(server))
                cboServer.Items.Add(server);
            cboServer.Items.Add(BrowseForServersText);
            setDefaultItem(cboServer);
        }

        private void setDatabaseDropDownItems(string database)
        {
            cboDatabase.Items.Clear();
            if (!String.IsNullOrEmpty(database))
                cboDatabase.Items.Add(database);
            cboDatabase.Items.Add(BrowseForDatabasesText);
            setDefaultItem(cboDatabase);
        }

        private void setDefaultItem(ComboBox cbo)
        {
            if (cbo.Items.Count > 1)
                cbo.SelectedIndex = 0;
            else
            {
                cbo.SelectedIndex = -1;
                cbo.SelectedText = "";
            }
        }

        private void startTestWorker()
        {
            // Create and start the background worker thread to connect.
            m_BackgroundWorker = null;
            m_BackgroundWorker = new BackgroundWorker();
            m_BackgroundWorker.WorkerSupportsCancellation = true;
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(test_DoWork);
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(test_RunWorkerCompleted);

            // Start the thread.
            m_BackgroundWorker.RunWorkerAsync(new Pair<ServiceInstance,SqlConnectionInfo>(m_Service, getConnnectionInfo()));
        }

        private void test_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the background thread.
            BackgroundWorker bgw = sender as BackgroundWorker;
            if (bgw == null)
            {
                Log.Error("The DoWork sender object is not a BackgroundWorker");
                throw (new Exception("The sender parameter is not a BackgroundWorker"));
            }

            // Get connection info.
            Pair<ServiceInstance, SqlConnectionInfo> param = e.Argument as Pair<ServiceInstance, SqlConnectionInfo>;
            if(param == null)
            {
                Log.Error("The DoWork argument object is not a SqlConnectionInfo");
                throw (new Exception("The argument object is not a Pair<ServiceInstance,SqlConnectionInfo>"));
            }

            // Test the connection.
            testRepositoryConnection(param.First, param.Second);

            // Process the results.
            if (bgw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void test_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker bgw = sender as BackgroundWorker;
            Debug.Assert(bgw != null);

            if (!bgw.CancellationPending)
            {
                if (e.Error != null)
                {
                    Log.Error("Test worker error, ", e.Error);
                    m_TestException = e.Error;
                    showTestFailure();
                }
                else
                {
                    showTestSuccessful();
                }
            }
            else
            {
                Log.Info("Connection worker thread cancelled.");
            }
        }

        private SqlConnectionInfo getConnnectionInfo()
        {
            SqlConnectionInfo ci = null;
            if (cboAuthentication.SelectedIndex == WindowsAuthenticationIndex)
            {
                ci = new SqlConnectionInfo(cboServer.Text.Trim(), cboDatabase.Text.Trim());
            }
            else
            {
                ci = new SqlConnectionInfo(cboServer.Text.Trim(), cboDatabase.Text.Trim(),
                                                txtLoginID.Text.Trim(), txtPassword.Text.Trim());
            }
            return ci;
        }

        private SqlConnectionInfo getServerConnnectionInfo()
        {
            SqlConnectionInfo ci = null;
            if (cboAuthentication.SelectedIndex == WindowsAuthenticationIndex)
            {
                ci = new SqlConnectionInfo(cboServer.Text.Trim());
            }
            else
            {
                ci = new SqlConnectionInfo(cboServer.Text.Trim(), txtLoginID.Text.Trim(), txtPassword.Text.Trim());
            }
            return ci;
        }

        private void testRepositoryConnection (
                ServiceInstance si,
                SqlConnectionInfo ci
            )
        {
            object[] args = new object[] { si.MachineName, si.ServicePort };
            Uri uri = new Uri(String.Format("tcp://{0}:{1}/Configuration", args));
            IManagementServiceConfiguration service = RemotingHelper.GetObject<IManagementServiceConfiguration>(uri.ToString());
            service.TestRepositoryConnection(ci);
        }

        private bool isRepositoryInfoChanged()
        {
            bool isChanged = string.Compare(m_Configuration.RepositoryHost, cboServer.Text.Trim(), true) != 0
                             || string.Compare(m_Configuration.RepositoryDatabase, cboDatabase.Text.Trim(), false) != 0
                             || (m_Configuration.WindowsAuthentication && cboAuthentication.SelectedIndex != WindowsAuthenticationIndex)
                             || (!m_Configuration.WindowsAuthentication && cboAuthentication.SelectedIndex != SqlAuthenticationIndex)
                             || (!m_Configuration.WindowsAuthentication && cboAuthentication.SelectedIndex == SqlAuthenticationIndex
                                        && (string.Compare(m_Configuration.RepositoryUsername,txtLoginID.Text.Trim(), false) != 0 
                                            || string.Compare(m_Configuration.RepositoryPassword, txtPassword.Text, false) != 0));
            return isChanged;
        }

        private void hideTestStatus()
        {
            testProgress.Visible = testProgress.Active = false;
            testErrorImage.Visible = false;
            testStatus.Visible = false;
        }

        private void showTestInProgress()
        {
            btnTest.Enabled = settingsPage.AllowMoveNext = false;

            testProgress.Visible = testProgress.Active = true;
            testProgress.BringToFront();
            testErrorImage.Visible = false;

            testStatus.Visible = true;
            testStatus.Text = "Connecting to the Repository";
            testStatus.LinkArea = new LinkArea(0, 0);
        }

        private void showTestSuccessful()
        {
            btnTest.Enabled = settingsPage.AllowMoveNext = true;

            testProgress.Visible = testProgress.Active = false;
            testProgress.SendToBack();
            testErrorImage.Visible = true;
            testErrorImage.Image = Properties.Resources.OK32x32;

            testStatus.Visible = true;
            testStatus.Text = "Successfully connected to the Repository";
            testStatus.LinkArea = new LinkArea(0, 0);
        }

        private void showTestFailure()
        {
            btnTest.Enabled = true;
            settingsPage.AllowMoveNext = false;

            testProgress.Visible = testProgress.Active = false;
            testProgress.SendToBack();
            testErrorImage.Visible = true;
            testErrorImage.Image = Properties.Resources.Critical32x32;

            testStatus.Visible = true;
            testStatus.Text = "Failed to connect to the Repository, click for details";
            testStatus.LinkArea = new LinkArea(37, 17);
        }

        #endregion

        #region finish page

        private void finishPage_BeforeDisplay(object sender, EventArgs e)
        {
//            if (isRepositoryInfoChanged())
//            {
                finishPage.FinishText = "Verify that the following configuration change is correct.  To make changes, click the Back button.";
                finishPage.ProceedText = "To update SQLdm Management Service configuration, click Finish.";
                oServer.Text = m_Configuration.RepositoryHost;
                oDatabase.Text = m_Configuration.RepositoryDatabase;
                oAuthentication.Text = m_Configuration.WindowsAuthentication ? "Windows Authentication" :
                                        ("SQL Authentication using login '" + m_Configuration.RepositoryUsername + "'");
                nServer.Text = cboServer.Text.Trim();
                nDatabase.Text = cboDatabase.Text.Trim();
                nAuthentication.Text = cboAuthentication.SelectedIndex == WindowsAuthenticationIndex ? "Windows Authentication" :
                                        ("SQL Authentication using login '" + txtLoginID.Text + "'");
                _pnl_FinishSummary.Visible = true;
//            }
//            else
//            {
//                _pnl_FinishSummary.Visible = false;
//                finishPage.FinishText = "No change has been made to the SQLdm Management Service configuration.  To make changes, click the Back button.";
//                finishPage.ProceedText = "To close this wizard, click Finish.";
//            }
        }

        private void wizard_Finish(object sender, EventArgs e)
        {
//            if (isRepositoryInfoChanged())
//            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    // Create config object.
                    ManagementServiceConfigurationMessage config = new ManagementServiceConfigurationMessage();
                    config.InstanceName = m_Configuration.InstanceName;
                    config.ServicePort = m_Configuration.ServicePort;
                    config.RepositoryHost = cboServer.Text.Trim();
                    config.RepositoryDatabase = cboDatabase.Text.Trim();
                    config.WindowsAuthentication = cboAuthentication.SelectedIndex == WindowsAuthenticationIndex;
                    if (!config.WindowsAuthentication)
                    {
                        config.RepositoryUsername = txtLoginID.Text.Trim();
                        config.RepositoryPassword = txtPassword.Text;
                    }

                    // Create service object.
                    object[] args = new object[] { m_Service.MachineName, m_Service.ServicePort };
                    Uri uri = new Uri(String.Format("tcp://{0}:{1}/Configuration", args));
                    IManagementServiceConfiguration service = RemotingHelper.GetObject<IManagementServiceConfiguration>(uri.ToString());

                    // Update service configuration.
                    service.SetManagementServiceConfiguration(config);

                    //Update the repository database name registry setting
                    RegistryKey rk = null;
                    RegistryKey rks = null;

                    rk = Registry.LocalMachine;
                    rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                    rks.SetValue("Database", config.RepositoryDatabase);

                    if (rks != null)
                        rks.Close();
                    rks = null;

                    if (rk != null)
                        rk.Close();
                    rk = null;
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered when updating config information");
                    ApplicationMessageBox.ShowError(this, "Failed to update SQLdm Management Service configuration.", ex);
                }
                this.Cursor = Cursors.Default;
 //           }
            Application.Exit();
        }

        #endregion

        #endregion
    }
}