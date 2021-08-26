using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Drawing;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Wintellect.PowerCollections;
using TracerX;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Analysis;

namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    public partial class FiltersSettingsTab : UserControl
    {
        private static Logger _log = Logger.GetLogger("GeneralSettingsTab");

        private bool _connectionSettingsChanged = false;
        private bool _modified = false;


        public bool Modified { get { return (_modified); } }
        public bool ConnectionSettingsChanged { get { return (_connectionSettingsChanged); } }
        public event EventHandler SettingsChanged;

        private int sqlServerID;
        private int includeDatabase;
        private List<DatabaseInformation> _databases = new List<DatabaseInformation>(ServerSettingsDialog.Databases != null ? ServerSettingsDialog.Databases : new List<DatabaseInformation>());

        private string filterApplication;
        public int IncludeDatabase
        {
            get { return includeDatabase; }
            set { includeDatabase = value; }
        }
        public string FilterApplication
        {
            get { return filterApplication; }
            set { filterApplication = value; }
        }

        public FiltersSettingsTab(int sqlServerID, int database, string filterApplication)
        {
            this.sqlServerID = sqlServerID;
            InitializeComponent();
            populateDatabaseCombobox();
            if (!string.IsNullOrEmpty(filterApplication))
                _applicationNameTextBox.Text = filterApplication;
            if (database != 0)
                _databaseFilterTypeComboBox.SelectedValue = database;
        }

        private void populateDatabaseCombobox()
        {
            try
            {
                ////Appending null check for Settings.Default.ActiveRepositoryConnection
                //if (Settings.Default == null || Settings.Default.ActiveRepositoryConnection == null || Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                //{
                //    _log.Error("No databases found because, Could not get Connection Info of active repository. Object 'Settings.Default.ActiveRepositoryConnection.ConnectionInfo' value is null.");
                //    _databases = null;
                //}
                //else
                //    _databases = RepositoryHelper.GetDatabasesForInstance(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, sqlServerID);
                if (null == _databases)
                {
                    _databases = new List<DatabaseInformation>();
                }
                DatabaseInformation defaultDI = new DatabaseInformation();
                defaultDI.DatabaseID = 0;
                defaultDI.DatabaseName = "Select Database to filter.";
                defaultDI.IsSystemDatabase = false;
                _databases.Insert(0, defaultDI);

                _databaseFilterTypeComboBox.DataSource = _databases;

                _databaseFilterTypeComboBox.ValueMember = "DatabaseID";
                _databaseFilterTypeComboBox.DisplayMember = "DatabaseName";

            }
            catch (Exception ex)
            {

                ApplicationMessageBox.ShowError(this,
                                                     "Not able to populate data bases.", ex);
                throw;
            }
        }

        private void OnSettingsChanged(EventArgs e)
        {
            //if (_loading) return;
            _modified = true;
            if (null != SettingsChanged) SettingsChanged(this, e);
        }

        public bool SaveChanges()
        {
            if (!_modified) return (true);

            filterApplication = _applicationNameTextBox.Text;
            includeDatabase = ((DatabaseInformation)_databaseFilterTypeComboBox.SelectedItem).DatabaseID;

            //var selectedFolder = _applicationNameTextBox.Text.Trim();

            //// PR# DR1312
            //if (!Directory.Exists(selectedFolder))
            //{
            //    try
            //    {
            //        Directory.CreateDirectory(selectedFolder);
            //    }
            //    catch (Exception ex)
            //    {
            //        _applicationNameTextBox.Focus();
            //        throw ex;
            //    }
            //}

            return (true);
        }

        private void filterApplicationText_TextChanged(object sender, EventArgs e)
        {
            OnSettingsChanged(EventArgs.Empty);
            filterApplication = _applicationNameTextBox.Text.ToString();
        }


        private void _databaseFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            includeDatabase = ((DatabaseInformation)_databaseFilterTypeComboBox.SelectedItem).DatabaseID;

            OnSettingsChanged(EventArgs.Empty);
        }

        private void EnableDialog(bool enable)
        {
            Form f = this.FindForm();
            if (null != f) f.Enabled = enable;
        }

    }
}
