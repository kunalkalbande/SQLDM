using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Threading;
using Wintellect.PowerCollections;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.ImportWizard.Properties;
using Idera.SQLdm.ImportWizard.Helpers;
using Idera.SQLdm.ImportWizard.Objects;


namespace Idera.SQLdm.ImportWizard
{
    public partial class WizardMainForm : Form
    {
        #region types, constants & members

        private const string BROWSE_FOR_SERVERS = "<Browse for more...>";
        private const string BROWSE_FOR_DATABASE = "<Browse server...>";
        private const int WINDOWS_AUTHENTICATION_INDEX = 0;
        private const int SQL_AUTHENTICATION_INDEX = 1;

        private readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Wizard");

        private BackgroundWorker m_BackgroundWorker;
        private SqlConnectionInfo m_ConnectionInfo;

        #endregion

        #region Ctors

        public WizardMainForm (
                string hostIn,
                string dbIn,
                string userIn,
                string pwdIn
            )
        {
            // Init component.
            InitializeComponent();

            // Initialize all the pages as needed.
            initializePages(hostIn, dbIn, userIn, pwdIn);

            // Set the first page based on the settings option.
            if(Settings.Default.HideImportWizardWelcomePage)
            {
                _wizard.SelectedPage = _page_RepositoryConnection;
            }
            else
            {
                _wizard.SelectedPage = _page_Introduction;
            }
        }
        #endregion

        #region properties

        public SqlConnectionInfo RepositoryInfo
        {
            get { return buildConnectionInfo(); }
        }

        public DateTime SelectedImportDate
        {
            get { return ((DateTime)_dtEdtr_DatePicker.Value).Date; }
        }

        public List<SQLdm5x.MonitoredSqlServer> SelectedServers
        {
            get
            {
                List<SQLdm5x.MonitoredSqlServer> ret = new List<SQLdm5x.MonitoredSqlServer>();
                foreach (Object o in _lstbx_SelectedServers.Items)
                {
                    SQLdm5x.MonitoredSqlServer ms = o as SQLdm5x.MonitoredSqlServer;
                    Debug.Assert(ms != null);
                    ret.Add(ms);
                }
                return ret;
            }
        }

        #endregion

        #region methods

        #region private

        #region General

        private void initializePages(
                string hostIn,
                string dbIn,
                string userIn,
                string pwdIn
            )
        {
            // -- RepositoryConnection page --- 
            // Update fields based on the command line params.
            if (!string.IsNullOrEmpty(hostIn))
            {
                // Update server and db.
                _txtbx_Server.Text = hostIn;
                _txtbx_Database.Text = dbIn;

                // Update authentication, user and password.
                _cmbbx_Authentication.SelectedIndex = string.IsNullOrEmpty(userIn) ?
                            WINDOWS_AUTHENTICATION_INDEX : SQL_AUTHENTICATION_INDEX;
                if (_cmbbx_Authentication.SelectedIndex == SQL_AUTHENTICATION_INDEX)
                {
                    _txtbx_User.Text = userIn;
                    _txtbx_Password.Text = pwdIn;
                }
            }
            else
            {
                _cmbbx_Authentication.SelectedIndex = WINDOWS_AUTHENTICATION_INDEX;
            }

            // Select SQL Servers page
            _btn_AddServer.Enabled = _btn_RemoveServer.Enabled = false;

        }

        private SqlConnectionInfo buildConnectionInfo()
        {
            Debug.Assert(!string.IsNullOrEmpty(_txtbx_Server.Text));
            Debug.Assert(!string.IsNullOrEmpty(_txtbx_Database.Text));
            Debug.Assert(_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX
                         || (_cmbbx_Authentication.SelectedIndex == SQL_AUTHENTICATION_INDEX
                                && !string.IsNullOrEmpty(_txtbx_User.Text)
                                    && !string.IsNullOrEmpty(_txtbx_Password.Text)));

            // Construct connection info object.
            SqlConnectionInfo ci = new SqlConnectionInfo();
            ci.ApplicationName = Constants.ImportWizardConnectionStringApplicationName;
            ci.InstanceName = _txtbx_Server.Text;
            ci.DatabaseName = _txtbx_Database.Text;
            if (_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX)
            {
                ci.UseIntegratedSecurity = true;
            }
            else
            {
                ci.UseIntegratedSecurity = false;
                ci.UserName = _txtbx_User.Text;
                ci.Password = _txtbx_Password.Text;
            }

            return ci;
        }

        private void updateConnectionInfo(
                out bool isChanged
            )
        {
            // Init return.
            isChanged = false;

            // Build the connection info based on information 
            SqlConnectionInfo sci = buildConnectionInfo();

            // If no previous connection info then set it.
            // Else compare to see if the repository location and credentials are different.
            if (m_ConnectionInfo == null)
            {
                m_ConnectionInfo = sci;
                isChanged = true;
            }
            else
            {
                if (string.Compare(m_ConnectionInfo.InstanceName, sci.InstanceName, true) != 0
                    || string.Compare(m_ConnectionInfo.DatabaseName, sci.DatabaseName, true) != 0
                    || (m_ConnectionInfo.UseIntegratedSecurity != sci.UseIntegratedSecurity)
                    || (m_ConnectionInfo.UseIntegratedSecurity == false
                            && string.Compare(m_ConnectionInfo.UserName, sci.UserName, true) != 0
                                && string.Compare(m_ConnectionInfo.Password, sci.Password, false) != 0))
                {
                    isChanged = true;
                    m_ConnectionInfo = sci;
                }
            }
        }

        #endregion

        #region Repository Connection Page

        private void updateRepositoryConnectionPage()
        {
            // Clear the connection status progress.
            _testConnectionStatus.HideStatus = true;

            // Update the database text box and browse, based on server being available.
            _btn_BrowseDatabase.Enabled = _txtbx_Database.Enabled = !string.IsNullOrEmpty(_txtbx_Server.Text);
            
            // Update Next and Test Connection buttons.
            bool enable = !string.IsNullOrEmpty(_txtbx_Server.Text)
                          && !string.IsNullOrEmpty(_txtbx_Database.Text)
                          && (_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX
                                || (!string.IsNullOrEmpty(_txtbx_User.Text)
                                        && !string.IsNullOrEmpty(_txtbx_Password.Text)));
            _page_RepositoryConnection.AllowMoveNext = _btn_TestRepositoryConnection.Enabled = enable;
        }

        private void _page_RepositoryConnection_BeforeDisplay(object sender, EventArgs e)
        {
            _txtbx_Server.Focus();
        }

        private void _cmbbx_Authentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            _txtbx_User.Enabled = _txtbx_Password.Enabled = _cmbbx_Authentication.SelectedIndex == SQL_AUTHENTICATION_INDEX;
            if (_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX)
            {
                _txtbx_Password.Text = _txtbx_User.Text = "";
            }
            updateRepositoryConnectionPage();
        }

        private void _btn_BrowseServer_Click(object sender, EventArgs e)
        {
            SqlServerBrowserDialog dlg = new SqlServerBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _txtbx_Server.Text = dlg.SelectedInstance;
            }
        }

        private void _btn_BrowseDatabase_Click(object sender, EventArgs e)
        {
            // If server is specified.
            if (!string.IsNullOrEmpty(_txtbx_Server.Text))
            {
                // If SQL Server authentication specified without user/password
                // flag error.
                if (_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX
                    || (_cmbbx_Authentication.SelectedIndex == SQL_AUTHENTICATION_INDEX
                            && !string.IsNullOrEmpty(_txtbx_User.Text)
                                && !string.IsNullOrEmpty(_txtbx_Password.Text)))
                {
                    // Construct connection info object.
                    SqlConnectionInfo ci = new SqlConnectionInfo();
                    ci.InstanceName = _txtbx_Server.Text;
                    if (_cmbbx_Authentication.SelectedIndex == WINDOWS_AUTHENTICATION_INDEX)
                    {
                        ci.UseIntegratedSecurity = true;
                    }
                    else
                    {
                        ci.UseIntegratedSecurity = false;
                        ci.UserName = _txtbx_User.Text;
                        ci.Password = _txtbx_Password.Text;
                    }

                    // Launch the browse database dialog and make the selection.
                    DatabaseBrowserDialog dlg = new DatabaseBrowserDialog(ci);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _txtbx_Database.Text = dlg.SelectedDatabase;
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, Resources.NoUserPwdForSqlAuthError, null);
                    return;
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(this, Resources.NoServerForDbBrowsingError, null);
                return;
            }
        }

        private void _txtbx_Server_TextChanged(object sender, EventArgs e)
        {
            updateRepositoryConnectionPage();
        }

        private void _txtbx_Database_TextChanged(object sender, EventArgs e)
        {
            updateRepositoryConnectionPage();
        }

        private void _txtbx_User_TextChanged(object sender, EventArgs e)
        {
            updateRepositoryConnectionPage();
        }

        private void _txtbx_Password_TextChanged(object sender, EventArgs e)
        {
            updateRepositoryConnectionPage();
        }

        private void _btn_TestRepositoryConnection_Click(object sender, EventArgs e)
        {
            // Disable the Back/Next/test buttons.
            _btn_TestRepositoryConnection.Enabled = _page_RepositoryConnection.AllowMovePrevious
                = _page_RepositoryConnection.AllowMoveNext = false;

            // Show status information.
            _testConnectionStatus.ShowInProgress();

            // Create background worker thread and add handlers.
            m_BackgroundWorker = null;
            m_BackgroundWorker = new BackgroundWorker();
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(testConnection_RunWorkerCompleted);
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(testConnection_DoWork);

            // Start the worker thread.
            m_BackgroundWorker.RunWorkerAsync(buildConnectionInfo());
        }

        private void testConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            SqlConnectionInfo ci = e.Argument as SqlConnectionInfo;
            BackgroundWorker bw = sender as BackgroundWorker;
            Debug.Assert(ci != null && bw != null);

            try
            {
                e.Result = SQLdm5x.GetRepositoryInfo(ci);
            }
            catch
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    throw;
                }
            }
        }

        private void testConnection_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    _testConnectionStatus.ShowConnectionError(e.Error);
                    Log.Error("Test connection - " + e.Error.Message);
                }
                else
                {
                    SQLdm5x.RepositoryInfo repositoryInfo = e.Result as SQLdm5x.RepositoryInfo;

                    if (repositoryInfo == null || !repositoryInfo.IsValidVersion)
                    {
                        _testConnectionStatus.ShowInvalidRepository();
                        Log.Error("Test connection - invalid SQLdm Repository version");
                    }
                    else if (!repositoryInfo.IsSysAdmin)
                    {
                        _testConnectionStatus.ShowNoPermission();
                        Log.Error("Test connection - not connected as sysadmin");
                    }
                    else
                    {
                        _testConnectionStatus.ShowSucceeded();
                    }
                }
            }

            // Enable the Next/test buttons.
            _btn_TestRepositoryConnection.Enabled = _page_RepositoryConnection.AllowMovePrevious
                = _page_RepositoryConnection.AllowMoveNext = true;
        }

        #endregion

        #region Select SQL Servers Page

        private void updateAllowMoveNext()
        {
            _page_SelectServers.AllowMoveNext = _lstbx_SelectedServers.Items.Count > 0;
        }

        private void _page_SelectServers_BeforeDisplay(object sender, EventArgs e)
        {
            // Update the connection info, note that the flag returns if the connection
            // info has changed.
            bool isConnectionInfoChanged = false;
            updateConnectionInfo(out isConnectionInfoChanged);

            // Clear the available servers list box.
            _lstbx_AvailableServers.Items.Clear();

            // If the connection info has changed, clear the selected servers list box.
            // And add the server name to the combo box.
            if (isConnectionInfoChanged)
            {
                _lstbx_SelectedServers.Items.Clear();
            }
            
            // Disable Back/Next buttons, the reason we are doing this is to make
            // sure we do not move off this page till the available servers list is filled.
            _page_SelectServers.AllowMovePrevious = _page_SelectServers.AllowMoveNext = false;

            // Start the loading circle.
            _loadServersStatus.ShowInProgress();

            // Create worker thread and add event handlers.
            m_BackgroundWorker = null;
            m_BackgroundWorker = new BackgroundWorker();
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(serversAndData_RunWorkerCompleted);
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(serversAndData_DoWork);

            // Start the thread.
            m_BackgroundWorker.RunWorkerAsync(m_ConnectionInfo);
        }

        private void serversAndData_DoWork(object sender, DoWorkEventArgs e)
        {
            SqlConnectionInfo sci = e.Argument as SqlConnectionInfo;
            BackgroundWorker bw = sender as BackgroundWorker;
            Debug.Assert(sci != null && bw != null);

            try
            {
                // Get a list of servers registered in DM 5.x.
                List<SQLdm5x.MonitoredSqlServer> servers5x = SQLdm5x.RegisteredSQLServers(sci);

                // Get a list of servers registered in DM 4.x.
                OrderedSet<string> servers4x = SQLdm4x.RegisteredSQLServers();

                string localHost = Environment.MachineName.ToUpper();

                // Update the return list with those servers that are registered in
                // both DM versions.
                List<SQLdm5x.MonitoredSqlServer> validServers = new List<SQLdm5x.MonitoredSqlServer>();
                foreach (SQLdm5x.MonitoredSqlServer s in servers5x)
                {
                    if (servers4x.Contains(s.Instance))
                    {
                        validServers.Add(s);
                    } else if (s.Instance == localHost || s.Instance.StartsWith(localHost + "\\")) {
                        // It might be in the 4.x registry as one of the aliases.
                        string[] aliases = new string[] { "(LOCAL)", "LOCALHOST", ".", "127.0.0.1" };
                        
                        foreach (string alias in aliases) {
                            string candidate = alias + s.Instance.Substring(localHost.Length);
                            
                            if (servers4x.Contains(candidate)) {
                                s.RegKeyName = candidate;
                                validServers.Add(s);
                                break;
                            }
                        }
                    }
                }

                // Return the list of available servers.
                e.Result = validServers;
            }
            catch
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    throw;
                }
            }
        }

        private void serversAndData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Process based on completion status.
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    _loadServersStatus.ShowError(e.Error);
                    Log.Error("error loading servers", e.Error);
                }
                else
                {
                    // Get the result list from the worker thread.
                    List<SQLdm5x.MonitoredSqlServer> list = e.Result as List<SQLdm5x.MonitoredSqlServer>;
                    Debug.Assert(list != null);

                    // If no servers available display status.
                    // Else fill the available servers.
                    if (list.Count == 0)
                    {
                        _loadServersStatus.ShowNoAvailableServers();
                    }
                    else
                    {
                        // Fill available servers.
                        foreach (SQLdm5x.MonitoredSqlServer s in list)
                        {
                            // If the server is not in the selected servers, add
                            // to available servers.
                            if (!_lstbx_SelectedServers.Items.Contains(s))
                            {
                                _lstbx_AvailableServers.Items.Add(s);
                            }
                        }

                        // Select the first item in the list.
                        if (_lstbx_AvailableServers.Items.Count > 0)
                        {
                            _lstbx_AvailableServers.SelectedIndex = 0;
                        }

                        // Hide the status display, and display the list box.
                        _loadServersStatus.HideStatus = true;
                    }
                }
            }
            else
            {
                // Hide the status display.
                _loadServersStatus.HideStatus = true;
            }

            // Enable Back/Next buttons.
            _page_SelectServers.AllowMovePrevious = true;
            updateAllowMoveNext();
        }

        private void _lstbx_AvailableServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _btn_AddServer.Enabled = _lstbx_AvailableServers.SelectedItems.Count > 0;
        }

        private void _lstbx_AvailableServers_DoubleClick(object sender, EventArgs e)
        {
            if (_lstbx_AvailableServers.SelectedItem != null)
            {
                moveToSelectedServers(_lstbx_AvailableServers.SelectedItem);
            }
        }

        private void _btn_AddServer_Click(object sender, EventArgs e)
        {
            List<Object> selectedSrvrs = new List<object>();
            foreach (Object s in _lstbx_AvailableServers.SelectedItems)
            {
                selectedSrvrs.Add(s);
            }
            foreach (Object s in selectedSrvrs)
            {
                moveToSelectedServers(s);
            }
        }

        private void moveToSelectedServers(Object s)
        {
            Debug.Assert(s != null);

            // Move the item to selected servers.
            if (!_lstbx_SelectedServers.Items.Contains(s))
            {
                _lstbx_SelectedServers.Items.Add(s);
            }
            _lstbx_AvailableServers.Items.Remove(s);

            // Update buttons based on state.
            updateAllowMoveNext();
        }

        private void _lstbx_SelectedServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _btn_RemoveServer.Enabled = _lstbx_SelectedServers.SelectedItems.Count > 0;
        }

        private void _lstbx_SelectedServers_DoubleClick(object sender, EventArgs e)
        {
            if (_lstbx_SelectedServers.SelectedItem != null)
            {
                moveToAvailableServers(_lstbx_SelectedServers.SelectedItem);
            }
        }

        private void _btn_RemoveServer_Click(object sender, EventArgs e)
        {
            List<object> list = new List<object>();
            foreach (object s in _lstbx_SelectedServers.SelectedItems)
            {
                list.Add(s);
            }
            foreach (object s in list)
            {
                moveToAvailableServers(s);
            }
        }

        private void moveToAvailableServers(Object s)
        {
            Debug.Assert(s != null);

            // Remove from selected and add to available.
            _lstbx_SelectedServers.Items.Remove(s);
            _lstbx_AvailableServers.Items.Add(s);

            // Update buttons based on state.
            updateAllowMoveNext();
        }

        #endregion

        #region Specify Import Date Page

        private void _page_SpecifyImportDate_BeforeDisplay(object sender, EventArgs e)
        {
            Debug.Assert(_lstbx_SelectedServers.Items.Count > 0);

            // Calculate the effective import date range for the selected servers.
            DateTime startDate, endDate;
            SQLdm5x.MonitoredSqlServer.GetEffectiveImportDateRange(SelectedServers, out startDate, out endDate);

            // Clear the list view and repopulate with selected servers.
            _lstvw_SelectedServers.Items.Clear();

            // Add the items in the selected servers list, also figure out
            // what is the oldest and latest time that data has been imported.
            _lstvw_SelectedServers.BeginUpdate();
            foreach (SQLdm5x.MonitoredSqlServer s in SelectedServers)
            {
                // Fill the list view.
                ListViewItem lvi = _lstvw_SelectedServers.Items.Add(s.Instance);
                lvi.SubItems.Add(s.EffectiveImportDateLocalString);
                lvi.Tag = s;
            }
            _lstvw_SelectedServers.EndUpdate();

            // Update the date selection info.
            _lbl_LoadFrom.Text = "&Import historical data from " + endDate.ToShortDateString() + " back to: ";
            _dtEdtr_DatePicker.Value = startDate.Subtract(new TimeSpan(30, 0, 0, 0));
        }

        private void _page_SpecifyImportDate_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            // Check the import date by comparing with server registration and
            // last export time.   If the import date is before any registration 
            // date or last import date, then identify such servers.
            DateTime sd = SelectedImportDate;
            List<SQLdm5x.MonitoredSqlServer> excludedServersList = new List<SQLdm5x.MonitoredSqlServer>();
            foreach (ListViewItem lvi in _lstvw_SelectedServers.Items)
            {
                SQLdm5x.MonitoredSqlServer s = lvi.Tag as SQLdm5x.MonitoredSqlServer;
                Debug.Assert(s != null);
                if (!s.IsImportDateValid(sd))
                {
                    excludedServersList.Add(s);
                }
            }

            // If the import date excludes servers, then show warning message.
            // Continue or stop based on user choice.
            if (excludedServersList.Count > 0)
            {
                DialogResult rc = ApplicationMessageBox.ShowWarning(this, Properties.Resources.InvalidImportDateWarn, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo);
                if (rc == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Finish Page

        private void _page_Finish_BeforeDisplay(object sender, EventArgs e)
        {
            // Clear and update the selected servers list box.
            _lstbx_FinishServers.Items.Clear();
            foreach (SQLdm5x.MonitoredSqlServer s in SelectedServers)
            {
                _lstbx_FinishServers.Items.Add(s);
            }

            // Update the finish import date.
            _lbl_FinishImportDate.Text = SelectedImportDate.ToShortDateString();
        }

        #endregion

        #region Wizard

        private void _wizard_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void _wizard_Finish(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _wizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (_wizard.SelectedPage == _page_Introduction)
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, HelpTopics.ImportWizardIntroductionPage));
            }
            else if(_wizard.SelectedPage == _page_RepositoryConnection)
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, HelpTopics.ImportWizardSpecifyRepositoryPage));
            }
            else if (_wizard.SelectedPage == _page_SelectServers)
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder,HelpTopics.ImportWizardSpecifySQLServersPage));
            }
            else if (_wizard.SelectedPage == _page_SpecifyImportDate)
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder,HelpTopics.ImportWizardSpecifyImportDatePage));
            }
            else if (_wizard.SelectedPage == _page_Finish)
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, HelpTopics.ImportWizardFinishPage));
            }
            else
            {
                Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, HelpTopics.HelpStartPage));
            }
        }

        #endregion

        #endregion

        #endregion
    }
}