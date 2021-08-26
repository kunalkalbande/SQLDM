using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Principal;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Configuration;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.UI.Dialogs;
using BBS.TracerX;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class RepositoryConnectionDialog : BaseDialog
    {
        private static readonly Logger Log = Logger.GetLogger("RepositoryConnectionDialog");
        private static readonly Logger StartUpTimeLog = Logger.GetLogger(TextConstants.StartUpTimeLogName);//SqlDm 10.2 (Tushar)--Adding start up logger.
        private const string BrowseForServersText = "<Browse for more...>";
        private const string BrowseForDatabasesText = "<Browse server...>";
        private const string PasswordDecoyText = "XXXXXXXXXXXXXXX";

        private RepositoryConnection selectedConnection;
        private BackgroundWorker activeConnectWorker;
        private BackgroundWorker licenseManagerWorker; //SqlDM 10.2 (Tushar)--Adding background worker for writing LM registry values.
        private object backgroundLicenseManagerLock = new object(); //SqlDM 10.2 (Tushar)--Adding lock object for license manager background worker.

        static private string GetConnectionStringForLogging(SqlConnectionInfo sci)
        {
            System.Text.StringBuilder connectionString = new System.Text.StringBuilder();

            // Application name
            connectionString.AppendFormat("Application Name={0};", sci.ApplicationName);

            // Server name
            connectionString.AppendFormat("Server=\"{0}\";", sci.InstanceName);

            // Database name
            connectionString.AppendFormat("Database=\"{0}\";", sci.DatabaseName);

            // Authentication information
            if (sci.UseIntegratedSecurity)
            {
                connectionString.Append("Integrated Security=true;");
            }
            else
            {
                connectionString.AppendFormat("User ID=\"{0}\";Password=\"{1}\";", sci.UserName, PasswordDecoyText);
            }

            // Connection Timeout
            connectionString.AppendFormat("Connection Timeout={0};", sci.ConnectionTimeout);

            //Asynchronous Command Support
            connectionString.AppendFormat("Asynchronous Processing={0};", sci.AllowAsynchronousCommands);

            //SSL Support
            connectionString.AppendFormat("Encrypt={0};", sci.EncryptData);
            if (sci.EncryptData)
                connectionString.AppendFormat("TrustServerCertificate={0};", sci.TrustServerCertificate);

            return connectionString.ToString();
        }

        public RepositoryConnectionDialog()
        {
            this.DialogHeader = "Connect to SQLDM Repository";
            InitializeComponent();
            Icon = Resources.ConnectionIcon;
            AcceptButton = connectButton;
            CancelButton = cancelButton;
            authenticationDropDownList.SelectedIndex = 0;
            databaseDropDownList.DisplayMember = "DatabaseName";
            
            InitializeForm();

            // Set the DialogResult to Cancel; the only way this will get set to OK
            // is if a successful connection has been established.
            DialogResult = DialogResult.Cancel;
            AdaptFontSize();
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }

        private void InitializeForm()
        {
            serversDropDownList.Items.Add(BrowseForServersText);
            databaseDropDownList.Items.Add(BrowseForDatabasesText);

            Hashtable serverLookupTable = new Hashtable();

            foreach (RepositoryConnection connection in Settings.Default.RepositoryConnections)
            {
                if (!serverLookupTable.ContainsKey(connection.ServerName))
                {
                    serversDropDownList.Items.Insert(0, connection.ServerName);
                    serverLookupTable.Add(connection.ServerName, String.Empty);
                }
            }

            serversDropDownList.SelectedIndex = serversDropDownList.Items.Count > 1 ? 0 : -1;

            UpdateOptions();
        }

        private void UpdateOptions()
        {
            connectButton.Enabled = selectedConnection != null || IsFieldDataValid();
            //deleteConnectionButton.Enabled = selectedConnection != null;
        }

        private bool IsFieldDataValid()
        {
            return serversDropDownList.Text.Trim().Length > 0 && databaseDropDownList.Text.Trim().Length > 0;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            UpdateForm(true);
            ConnectAsync();
        }

        private void ConnectAsync()
        {
            activeConnectWorker = new BackgroundWorker();
            activeConnectWorker.WorkerSupportsCancellation = true;
            activeConnectWorker.DoWork += connectWorker_DoWork;
            activeConnectWorker.RunWorkerCompleted += connectWorker_RunWorkerCompleted;

            activeConnectWorker.RunWorkerAsync(BuildConnectionInfo());
        }

        private void UpdateForm(bool isConnecting)
        {
            if (isConnecting)
            {
                connectionProgressBar.Start();
                connectButton.Enabled = false;
                //deleteConnectionButton.Enabled = false;
                serverLabel.Enabled = false;
                serversDropDownList.Enabled = false;
                databaseLabel.Enabled = false;
                databaseDropDownList.Enabled = false;
                authenticationLabel.Enabled = false;
                authenticationDropDownList.Enabled = false;
                userNameLabel.Enabled = false;
                userNameTextbox.Enabled = false;
                passwordLabel.Enabled = false;
                passwordTextbox.Enabled = false;
                rememberPasswordCheckbox.Enabled = false;
            }
            else
            {
                connectionProgressBar.Stop();
                serverLabel.Enabled = true;
                serversDropDownList.Enabled = true;
                databaseLabel.Enabled = true;
                databaseDropDownList.Enabled = true;
                authenticationLabel.Enabled = true;
                authenticationDropDownList.Enabled = true;

                if (authenticationDropDownList.SelectedIndex == 0)
                {
                    WindowsAuthenticationSelected();
                }
                else
                {
                    SqlServerAuthenticationSelected(false);
                }

                UpdateOptions();
            }
        }

        private void RepositoryConnectionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (activeConnectWorker != null)
            {
                activeConnectWorker.CancelAsync();
            }
        }

        private void authenticationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authenticationDropDownList.SelectedIndex == 0)
            {
                WindowsAuthenticationSelected();
            }
            else
            {
                SqlServerAuthenticationSelected(true);
            }
        }

        private void WindowsAuthenticationSelected()
        {
            userNameLabel.Text = "User name:";
            userNameLabel.Enabled = false;
            userNameTextbox.Enabled = false;
            userNameTextbox.Text = WindowsIdentity.GetCurrent().Name;
            passwordLabel.Enabled = false;
            passwordTextbox.Text = String.Empty;
            passwordTextbox.Enabled = false;
            rememberPasswordCheckbox.Checked = false;
            rememberPasswordCheckbox.Enabled = false;
        }

        private void SqlServerAuthenticationSelected(bool clearUserName)
        {
            userNameLabel.Text = "Login:";
            userNameLabel.Enabled = true;
            userNameTextbox.Enabled = true;
            if (clearUserName)
            {
                userNameTextbox.Text = string.Empty;
            }
            passwordLabel.Enabled = true;
            passwordTextbox.Enabled = true;
            rememberPasswordCheckbox.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If the connection worker thread is actively attempting a connection,
            // just cancel the current operation, don't close the form
            if (activeConnectWorker != null && activeConnectWorker.IsBusy)
            {
                activeConnectWorker.CancelAsync();
                activeConnectWorker = null;
                UpdateForm(false);
                DialogResult = DialogResult.None;
            }
            else
            {
                Close();
            }
        }

        private void serversDropDownList_TextUpdate(object sender, EventArgs e)
        {
            if (selectedConnection != null)
            {
                passwordTextbox.Text = String.Empty;
                rememberPasswordCheckbox.Checked = false;
                selectedConnection = null;
            }

            UpdateOptions();
        }

        private void databaseDropDownList_TextUpdate(object sender, EventArgs e)
        {
            if (selectedConnection != null)
            {
                passwordTextbox.Text = String.Empty;
                rememberPasswordCheckbox.Checked = false;
                selectedConnection = null;
            }

            UpdateOptions();
        }

        private void databaseDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (databaseDropDownList.SelectedIndex == databaseDropDownList.Items.Count - 1)
            {
                databaseDropDownList.SelectedIndex = -1;
                selectedConnection = null;
                browseDatabasesWorker.RunWorkerAsync();
            }
            else
            {
                selectedConnection = databaseDropDownList.SelectedItem as RepositoryConnection;

                if (selectedConnection != null)
                {
                    if (selectedConnection.ConnectionInfo.UseIntegratedSecurity)
                    {
                        authenticationDropDownList.SelectedIndex = 0;
                    }
                    else
                    {
                        authenticationDropDownList.SelectedIndex = 1;
                        userNameTextbox.Text = selectedConnection.ConnectionInfo.UserName;
                        passwordTextbox.Text = selectedConnection.SavePassword ? PasswordDecoyText : String.Empty;
                        rememberPasswordCheckbox.Checked = selectedConnection.SavePassword;
                    }
                }

                UpdateOptions();
            }
        }

        private void serversDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serversDropDownList.SelectedIndex == serversDropDownList.Items.Count - 1)
            {
                serversDropDownList.SelectedIndex = -1;
                databaseDropDownList.SelectedIndex = -1;
                selectedConnection = null;
                passwordTextbox.Text = String.Empty;
                rememberPasswordCheckbox.Checked = false;
                browseServersWorker.RunWorkerAsync();
            }
            else
            {
                databaseDropDownList.Items.Clear();
                ReadOnlyCollection<RepositoryConnection> matchingConnections =
                    Settings.Default.FindRepositoryConnections(serversDropDownList.Text);

                foreach (RepositoryConnection connection in matchingConnections)
                {
                    databaseDropDownList.Items.Insert(0, connection);
                }

                databaseDropDownList.Items.Add(BrowseForDatabasesText);

                databaseDropDownList.SelectedIndex = databaseDropDownList.Items.Count > 1 ? 0 : -1;
            }
        }

        private void deleteConnectionButton_Click(object sender, EventArgs e)
        {
            if (ApplicationMessageBox.ShowWarning(this,
                                                  "Deleting this repository connection will cause all associated server views to be deleted as well. Would you like to continue?",
                                                  ExceptionMessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (selectedConnection != null)
                {
                    Settings.Default.RepositoryConnections.Remove(selectedConnection);
                    selectedConnection = null;
                    serversDropDownList.Items.RemoveAt(serversDropDownList.SelectedIndex);
                    databaseDropDownList.Items.RemoveAt(databaseDropDownList.SelectedIndex);
                    passwordTextbox.Text = String.Empty;
                    rememberPasswordCheckbox.Checked = false;
                    UpdateOptions();
                }
            }
        }

        private void browseServersWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SqlServerBrowserDialog dialog = new SqlServerBrowserDialog();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                serversDropDownList.Text = dialog.SelectedInstance;
                databaseDropDownList.Text = String.Empty;
            }

            UpdateOptions();
        }

        private void browseDatabasesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            databaseDropDownList.Text = BrowseServerForDatabase();
            UpdateOptions();
        }

        private string BrowseServerForDatabase()
        {
            if (serversDropDownList.Text == null || serversDropDownList.Text.Length == 0)
            {
                ApplicationMessageBox.ShowError(this, "To browse for a database, you must first specify a server name.",
                                                null);
                return null;
            }
            else
            {
                DatabaseBrowserDialog dialog = new DatabaseBrowserDialog(BuildConnectionInfo());

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    return dialog.SelectedDatabase;
                }
                else
                {
                    return null;
                }
            }
        }

        private SqlConnectionInfo BuildConnectionInfo()
        {
            SqlConnectionInfo connectionInfo = new SqlConnectionInfo();
            connectionInfo.ApplicationName = ApplicationHelper.AssemblyTitle;
            connectionInfo.InstanceName = serversDropDownList.Text;
            connectionInfo.DatabaseName = databaseDropDownList.Text;
            connectionInfo.UseIntegratedSecurity = authenticationDropDownList.SelectedIndex == 0;
            connectionInfo.AllowAsynchronousCommands = false;

            if (!connectionInfo.UseIntegratedSecurity)
            {
                connectionInfo.UserName = userNameTextbox.Text;
                
                if (selectedConnection == null)
                {
                    connectionInfo.Password = passwordTextbox.Text;
                }
                else
                {
                    if (String.CompareOrdinal(passwordTextbox.Text, PasswordDecoyText) != 0)
                    {
                        connectionInfo.Password = passwordTextbox.Text;
                    }
                    else
                    {
                        connectionInfo.Password = selectedConnection.ConnectionInfo.Password;
                    }
                }
            }

            Log.Info("connection string (BuildConnectionInfo) : ", GetConnectionStringForLogging(connectionInfo));

            return connectionInfo;
        }

        private static void connectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RepositoryConnectionWorker";

            BackgroundWorker connectWorker = sender as BackgroundWorker;

            if (connectWorker != null)
            {
                try
                {
                    e.Result = RepositoryHelper.GetRepositoryInfo(e.Argument as SqlConnectionInfo);
                }
                catch
                {
                    if (connectWorker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private void connectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender == activeConnectWorker)
            {
                activeConnectWorker = null;
            }

            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "An error occurred while establishing a connection to the specifed repository. Please resolve the following error in order to continue.",
                                                    e.Error);
                    UpdateForm(false);
                }
                else
                {
                    RepositoryInfo repositoryInfo = e.Result as RepositoryInfo;

                    if (repositoryInfo == null)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        Resources.InvalidRepositoryVersionMessage,
                                                        null);
                        UpdateForm(false);
                    }
                    else if (!repositoryInfo.IsValidVersion)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        string.Format(Resources.InvalidRepositoryVersionMessageWithMetadata, repositoryInfo.VersionString, Common.Constants.ValidRepositorySchemaVersion),
                                                        null);
                        UpdateForm(false);
                    }
                    else
                    {
                        // Remember the repository connection info stored in the settings file.
                        // And call SetActiveConnection so that the ManageServers dialog exposed
                        // in the license dialog works correctly.
                        RepositoryConnection temp = Settings.Default.ActiveRepositoryConnection;
                        SetActiveConnection();

                        if (LicenseChecker.CheckLicense(BuildConnectionInfo(), this, true)) 
                        {
                            //Start-SqlDM 10.2 (Tushar)--Adding background worker for writing LM registry values when active repository connection is changed.
                            lock (backgroundLicenseManagerLock)
                            {
                                licenseManagerWorker = new BackgroundWorker();
                                licenseManagerWorker.DoWork += LicenseManagerWorker_DoWork;
                                licenseManagerWorker.RunWorkerCompleted += LicenseManagerWorker_RunWorkerCompleted;
                                licenseManagerWorker.RunWorkerAsync();
                            }
                            ApplicationModel.Default.RepositoryConnectionChanged = true;
                            //End-SqlDM 10.2 (Tushar)--Adding background worker for writing LM registry values when active repository connection is changed.
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else 
                        {
                            // Restore the settings repository connection, if the 
                            // original setting was not null.
                            if (temp != null)
                            {
                                Settings.Default.SetActiveRepositoryConnection(temp);
                            }
                            UpdateForm(false);
                        }
                    }
                }
            }
        }

        //Start-SqlDM 10.2 (Tushar)--Adding background worker for writing LM registry values when active repository connection is changed.
        private void LicenseManagerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (e.Error is Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException)
            {
                ApplicationMessageBox.ShowError(null,
                                                "An error occurred while attempting to contact the Management Service.",
                                                e.Error);
            }
            else if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(null,
                                                Idera.SQLdm.DesktopClient.Properties.Resources.ExceptionUnhandled,
                                                e.Error);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken  by LicenseManagerWorker_RunWorkerCompleted : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void LicenseManagerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "LicenseManagerWorker";
            ManagementServiceHelper.IManagementService.RegisterLicenseManager();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken  by LicenseManagerWorker_DoWork : {0}", stopWatch.ElapsedMilliseconds);
        }
        //End-SqlDM 10.2 (Tushar)--Adding background worker for writing LM registry values when active repository connection is changed.

        private void SetActiveConnection()
        {
            RepositoryConnection connection =
                Settings.Default.FindRepositoryConnection(serversDropDownList.Text, databaseDropDownList.Text);

            if (connection == null)
            {
                connection = new RepositoryConnection();
                connection.ConnectionInfo.InstanceName = serversDropDownList.Text;
                connection.ConnectionInfo.DatabaseName = databaseDropDownList.Text;
            }
            connection.ConnectionInfo.AllowAsynchronousCommands = false;

            if (authenticationDropDownList.SelectedIndex == 0)
            {
                connection.ConnectionInfo.UseIntegratedSecurity = true;
            }
            else if (authenticationDropDownList.SelectedIndex == 1)
            {
                connection.ConnectionInfo.UseIntegratedSecurity = false;
                connection.ConnectionInfo.UserName = userNameTextbox.Text;

                string password;
                if (String.CompareOrdinal(passwordTextbox.Text, PasswordDecoyText) != 0)
                {
                    password = passwordTextbox.Text;
                }
                else
                {
                    password = connection.ConnectionInfo.Password;
                }

                connection.ConnectionInfo.Password = password;
                connection.SavePassword = rememberPasswordCheckbox.Checked;
            }

            connection.RefreshRepositoryInfo();

            Log.Info("connection string (SetActiveConnection()) : ", GetConnectionStringForLogging(connection.ConnectionInfo));
            Settings.Default.SetActiveRepositoryConnection(connection, false, ApplicationController.Default.IsHooked);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureRepositoryConnection);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureRepositoryConnection);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        
        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ScaleControlsAsPerResolution()
        {
            this.headerImage.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}