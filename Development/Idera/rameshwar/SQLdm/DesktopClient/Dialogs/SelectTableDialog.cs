using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Wintellect.PowerCollections;

    public partial class SelectTableDialog : Form, IEqualityComparer<string>
    {
        private const string AdhocInstructionText = "< Type semicolon separated names >";

        private BackgroundWorker loadTablesWorker = null;
        private BackgroundWorker loadDatabasesWorker = null;

        private MonitoredSqlServer monitoredSqlServer;
        private Set<string> selectedSet;

        public SelectTableDialog(MonitoredSqlServer monitoredSqlServer, Set<string> selectedTables)
        {
            this.monitoredSqlServer = monitoredSqlServer;
            this.selectedSet = selectedTables ?? new Set<string>(this);
            InitializeComponent();
            availableTablesTextBox.Text = AdhocInstructionText;
            AdaptFontSize();
        }

        public Set<string> SelectedTables
        {
            get { return selectedSet; }
        }

        private void availableTablesTextBox_Enter(object sender, EventArgs e)
        {
            if (string.CompareOrdinal(AdhocInstructionText, availableTablesTextBox.Text) == 0)
            {
                availableTablesTextBox.Clear();
            }

            availableTablesTextBox.ForeColor = SystemColors.WindowText;
        }

        private void availableTablesTextBox_Leave(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void availableTablesTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls(true);
        }

        private void UpdateControls()
        {
            UpdateControls(false);
        }

        private void UpdateControls(bool ignoreAvailableTextBox)
        {
            removeButton.Enabled = selectedTablesListBox.SelectedItems.Count > 0;
            addButton.Enabled = availableTablesListBox.SelectedItems.Count > 0 ||
                                         (availableTablesTextBox.Text.Trim().Length > 0 &&
                                         string.CompareOrdinal(AdhocInstructionText, availableTablesTextBox.Text.Trim()) != 0);


            if (addButton.Enabled)
            {
                this.AcceptButton = addButton;
            }
            else if (removeButton.Enabled)
            {
                this.AcceptButton = removeButton;
            }

            if (!ignoreAvailableTextBox && availableTablesTextBox.Text.Trim().Length == 0)
            {
                availableTablesTextBox.ForeColor = SystemColors.GrayText;
                availableTablesTextBox.Text = AdhocInstructionText;
            }
        }

        private void SelectTableDialog_Load(object sender, EventArgs e)
        {
            availableTablesStackPanel.ActiveControl = availableTablesMessageLabel;

            selectedTablesListBox.Items.AddRange(selectedSet.ToArray());

            StartDatabaseLoadWorker();
        }

        private void StartDatabaseLoadWorker()
        {
            loadDatabasesWorker = new BackgroundWorker();
            loadDatabasesWorker.WorkerSupportsCancellation = true;
            loadDatabasesWorker.DoWork += loadDatabasesWorker_DoWork;
            loadDatabasesWorker.RunWorkerCompleted += loadDatabasesWorker_RunWorkerCompleted;
            loadDatabasesWorker.RunWorkerAsync(monitoredSqlServer);
        }

        void loadDatabasesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker) sender;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            MonitoredSqlServer server = (MonitoredSqlServer)e.Argument;
            object result = managementService.GetDatabases(server.Id, true, true);
            if (worker.CancellationPending)
                e.Cancel = true;
            else
                e.Result = result;
        }

        void loadDatabasesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || IsDisposed)
                return;

            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
                return;
            }
            IDictionary<string, bool> databases = e.Result as IDictionary<string, bool>;
            if (databases != null)
            {
                databaseComboBox.Items.Clear();
                foreach (string database in databases.Keys)
                {
                    databaseComboBox.Items.Add(database, database);
                }
            }
            databaseComboBox.NullText = "< Select a database >";
            databaseComboBox.Enabled = true;
            availableTablesMessageLabel.Text = "Select a database to load it's tables.";
        }

        private void StartTableLoadWorker(string database)
        {
            if (loadTablesWorker != null && loadTablesWorker.IsBusy)
            {
                loadTablesWorker.CancelAsync();
            }

            loadTablesWorker = new BackgroundWorker();
            loadTablesWorker.WorkerSupportsCancellation = true;
            loadTablesWorker.DoWork += loadTablesWorker_DoWork;
            loadTablesWorker.RunWorkerCompleted += loadTablesWorker_RunWorkerCompleted;
            loadTablesWorker.RunWorkerAsync(new Pair<MonitoredSqlServer, string>(monitoredSqlServer, database));
            availableTablesStackPanel.ActiveControl = availableTablesMessageLabel;
            availableTablesMessageLabel.Text = "Loading tables...";
        }

        void loadTablesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            Pair<MonitoredSqlServer, string> args = (Pair<MonitoredSqlServer,string>)e.Argument;
            List<Triple<string,string,bool>> tables = managementService.GetTables(args.First.Id, args.Second, true, true);
            if (worker.CancellationPending)
                e.Cancel = true;
            else
            {
                e.Result = new Pair<string, List<Triple<string, string, bool>>>(args.Second, tables);
            }
        }

        void loadTablesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
                availableTablesMessageLabel.Text = "Select a database to load it's tables.";
                return;
            }

            Pair<string, List<Triple<string, string, bool>>> result = (Pair<string, List<Triple<string, string, bool>>>)e.Result;
            availableTablesListBox.Items.Clear();
            availableTablesListBox.Tag = result.First;
            foreach (Triple<string,string,bool> table in result.Second)
            {
                string qualifiedName = table.First + "." + table.Second;
                if (!selectedSet.Contains(qualifiedName))
                    availableTablesListBox.Items.Add(qualifiedName);
            }
            availableTablesStackPanel.ActiveControl = availableTablesListBox;
        }

        private void AddToSelected(string value)
        {
            if (!selectedSet.Contains(value))
            {
                selectedSet.Add(value);
                selectedTablesListBox.Items.Add(value);
            }
        }

        private void RemoveFromSelected(string value)
        {
            selectedSet.Remove(value);
            selectedTablesListBox.Items.Remove(value);
        }

        private void AddToAvailable(string value)
        {
            int i = value.IndexOf(".");
            if (i > 0)
            {
                string database = value.Substring(0, i);
                if (database.Equals(availableTablesListBox.Tag))
                {
                    value = value.Substring(i + 1);
                    if (!availableTablesListBox.Items.Contains(value))
                        availableTablesListBox.Items.Add(value);
                }
            }
       }

        private void selectedTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedTablesListBox.SelectedItems.Count > 0)
            {
                string qualifiedName = selectedTablesListBox.SelectedItem.ToString();
                AddToAvailable(qualifiedName);
                RemoveFromSelected(qualifiedName);
            }
        }

        private void selectedTablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeButton.Enabled = (selectedTablesListBox.SelectedIndex != -1);

        }

        private void availableTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (availableTablesListBox.SelectedItems.Count > 0)
            {
                String qualifiedName = availableTablesListBox.Tag.ToString() +
                                       "." +
                                       availableTablesListBox.SelectedItem.ToString();

                AddToSelected(qualifiedName);
                availableTablesListBox.Items.Remove(availableTablesListBox.SelectedItem);
            }
        }

        private void availableTablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addButton.Enabled = (availableTablesListBox.SelectedIndex != -1);
        }

        private void AddAdhocTables()
        {
            string adhocTablesText = availableTablesTextBox.Text.Trim();

            if (adhocTablesText.Length != 0 &&
                string.CompareOrdinal(adhocTablesText, AdhocInstructionText) != 0)
            {
                SortedList<string, string> alreadyMonitoredInstances = new SortedList<string, string>();
                string[] adhocTables = adhocTablesText.Split(new char[] { ';' });

                foreach (string adhocTable in adhocTables)
                {
                    string trimmedTableName = adhocTable.Trim();
                    AddToSelected(trimmedTableName);
                }
            }

            availableTablesTextBox.Clear();
        }

        private void AddSelectedTables()
        {
            while (availableTablesListBox.SelectedItems.Count > 0)
            {
                object firstItem = availableTablesListBox.SelectedItems[0];
                AddToSelected(availableTablesListBox.Tag.ToString() + "." + firstItem.ToString());
                availableTablesListBox.Items.Remove(firstItem);
            }   
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddAdhocTables();
            AddSelectedTables();
            UpdateControls();
            availableTablesTextBox.Focus();
            availableTablesTextBox_Enter(availableTablesTextBox, e);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            while (selectedTablesListBox.SelectedItems.Count > 0)
            {
                string firstItem = (string)selectedTablesListBox.SelectedItems[0];
                AddToAvailable(firstItem);
                RemoveFromSelected(firstItem);
            }
            UpdateControls();
        }

        private void databaseComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (databaseComboBox.SelectedItem != null &&
                    databaseComboBox.SelectedItem.DataValue != null)
            {
                string selectedDatabase = databaseComboBox.SelectedItem.DataValue as string;
                StartTableLoadWorker(selectedDatabase);
            }
            UpdateControls();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedSet.Clear();
            if (selectedTablesListBox.Items.Count > 0)
            {
                foreach (string table in selectedTablesListBox.Items)
                {
                    selectedSet.Add(table);
                }
            }
        }


        #region IEqualityComparer<string> Members

        bool IEqualityComparer<string>.Equals(string x, string y)
        {
            return String.Equals(x, y, StringComparison.CurrentCultureIgnoreCase);
        }

        int IEqualityComparer<string>.GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        #endregion
    }
}