using System.Collections;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Microsoft.SqlServer.MessageBox;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;
    using System.Management;
    using System.Data.SqlClient;
    using System.Text;
    using AzureConfigurations;
    using Common.Events.AzureMonitor.Interfaces;
    using System.Linq;

    public partial class CustomCounterWizard : Form
    {
        private readonly CustomCounterDefinition counterDefinition;

        private class CustomCounterProfile
        {
            public string SqlServer { get; set; }
            public string ProfileName { get; set; }
            public string ResourceType { get; set; }
            public string ResourceName { get; set; }
            public string MetricName { get; set; }
        }

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CustomCounterWizard");

        private const string PlaceHolderStart = "Enter";
        private const string PlaceHolderEnd = "...";
        private const string SELECT_A_SERVER = "< Select a server >";
        private const string SELECT_A_RESOURCE_TYPE = "< Select a Resource Type >";
        private const string SELECT_A_DATABASE = "< Select a database >";
        private const string SELECT_A_RESOURCE = "< Select a resource >";
        private const string SELECT_A_PROFILE = "< Select a profile >";
        private const string SELECT_A_METRIC = "< Select a metric >";
        private const string SELECT_AN_OBJECT = "< Select an object >";
        private const string SELECT_A_COUNTER = "< Select a counter >";
        private const string SELECT_AN_INSTANCE = "< Select an instance >";
        private const string SPECIAL_COLUMN = "IsSpecial";
        private const string SINGLETON_COLUMN = "Singleton";
        private const long THRESHOLD_MAX = 999999999999999L;
        private CustomCounterProfile profile = new CustomCounterProfile();
        private const string INVALIDCOUNTERCHARS = @"/?:&\*<>|#%,;@""'$+=~!^(){}[]-";
        private static readonly string[] RESERVEDNAMES ={"CON","AUX","PRN","COM1","LPT2",".","..","nul","com2",
        "com3","com4","com5","com6","com7","com8","com9","lpt1","lpt3","lpt4","lpt5","lpt6",
        "lpt7","lpt8","lpt9","clock$"};
        private const string INVALIDCOUNTERNAME =
            "The custom counter name must be CLS-compliant.\r\n" + 
            "The custom counter name cannot:\r\n" +
            "- be empty strings\r\n" +
            "- contain any of the following characters: /?:&&\\*\"<>|#%,;@$+=\'~!^(){}[]-\r\n" +
            "- contain Unicode control characters\r\n" +
            "- contain surrogate characters\r\n" +
            "- be system reserved names including 'CON', 'AUX', 'PRN', 'COM1' or 'LPT2'\r\n" +
            "- be '.' or '..'\r\n\r\n" +
            "Please enter a valid name";


        private SqlConnectionInfo connectionInfo;
        private string currentSqlServerName = String.Empty;
        private string currentSqlCounterName = String.Empty;
        private string currentSqlObjectName = String.Empty;
        private string currentWmiServerName = String.Empty;
        private string currentAzureServerName = string.Empty;
        private string currentWmiCounterName = String.Empty;
        private string currentWmiObjectName = String.Empty;
        private string currentVmServerName = String.Empty;
        private string currentVmCounterName = String.Empty;
        private string currentVmObjectName = String.Empty;
        
        // Azure Wizard Edit and Import
        private string azureApplicationProfileId = string.Empty;
        private string azureResourceName = string.Empty;
        private string azureResourceType = string.Empty;
        private string azureMetricDisplayName = string.Empty;
        private string azureServerId = string.Empty;

        private bool addMode = true;
        private IManagementService managementService;

        private int metric = -1;
        private Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>? counterUpdated;

        private DataTable wmiObjectsDataTable;
        private DataTable wmiCountersDataTable;
        private DataTable wmiInstancesDataTable;
        private DataTable sqlObjectsDataTable;
        private DataTable sqlCountersDataTable;
        private DataTable sqlInstancesDataTable;
        private DataTable vmObjectsDataTable;
        private DataTable vmCountersDataTable;
        private DataTable vmInstancesDataTable;
        private decimal? infoThresholdValue = null;
        private decimal? warningThresholdValue = null;
        private decimal? criticalThresholdValue = null;
        private bool alertOnFailureToCollect = false;
        private bool infoThresholdEnabled = true;
        private DataTable azureEditModeData;
        private bool alertOnFailureToCollectModified = false;

        private List<IAzureProfile> azureProfiles;

        private BackgroundWorkerWithProgressDialog backgroundWorker;


        private CustomCounterWizard()
        {
            InitializeComponent();
            infoThresholdSpinner.TextChanged += new EventHandler(warningThresholdSpinner_TextChanged);
            warningThresholdSpinner.TextChanged += new EventHandler(warningThresholdSpinner_TextChanged);
            criticalThresholdSpinner.TextChanged += new EventHandler(warningThresholdSpinner_TextChanged);

            // set max values too long for me to type in designer
            infoThresholdSpinner.Maximum = 
                warningThresholdSpinner.Maximum = 
                criticalThresholdSpinner.Maximum = THRESHOLD_MAX;

            InitializeWmiDataTables();
            InitializeSqlDataTables();
            InitializeVmDataTables();

            // load combo boxes
            LoadScaleComboBox(counterScaleComboBox);

            this.AdaptFontSize();

            // get repository and management service interface
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
            
            wmiStatusLabel.Text = String.Empty;
            sqlStatusLabel.Text = String.Empty;
            vmStatusLabel.Text  = String.Empty;
            wmiObjectNameComboBox.Tag = wmiObjectNameTextBox;
            wmiCounterNameComboBox.Tag = wmiCounterNameTextBox;
            wmiInstanceNameComboBox.Tag = wmiInstanceNameTextBox;
            sqlObjectNameComboBox.Tag = sqlObjectNameTextBox;
            sqlCounterNameComboBox.Tag = sqlCounterNameTextBox;
            sqlInstanceNameComboBox.Tag = sqlInstanceNameTextBox;
            vmObjectNameComboBox.Tag = vmObjectNameTextBox;
            vmCounterNameComboBox.Tag = vmCounterNameTextBox;
            vmInstanceNameComboBox.Tag = vmInstanceNameTextBox;
            alertCategoryComboBox.Text = "Custom";
            counterDefinition = new CustomCounterDefinition();
            InitializeAzureComboboxes();
        }

        private void InitializeAzureComboboxes()
        {
            AddDefault(azureServerComboBox, SELECT_A_SERVER);
            AddDefault(azureProfileComboBox, SELECT_A_PROFILE);
            AddDefault(azureResourceTypeNameComboBox, SELECT_A_RESOURCE_TYPE);
            AddDefault(azureResourceNameComboBox, SELECT_A_RESOURCE);
            AddDefault(azureMetricNameComboBox, SELECT_A_METRIC);
        }

        private void AddDefault(ComboBox combobox, string defaultValue)
        {
            var index = combobox.Items.IndexOf(defaultValue);
            if (index == -1)
            {
                combobox.Items.Insert(0, defaultValue);
                combobox.SelectedIndex = 0;
            }
            else
            {
                combobox.SelectedIndex = index;
            }
        }

        private CustomCounterWizard(MetricDefinition metricDefinition, MetricDescription metricDescription,
                                    CustomCounterDefinition counterDefinition)
            : this()
        {
            this.counterDefinition = counterDefinition;
            counterTypePage.PreviousPage = null;
            Divelements.WizardFramework.WizardPage page = counterTypePage;

            addMode = false;
            metric = metricDefinition.MetricID;

            alertNameTextBox.Text = metricDescription.Name;
            alertCategoryComboBox.Text = metricDescription.Category;
            alertDescriptionTextBox.Text = metricDescription.Description;
            switch (counterDefinition.MetricType)
            {
                case MetricType.WMI:
                    wmiCounterButton.Checked = true;
                    wmiManualModeButton.Checked = true;
                    wmiShowAllCountersButton.Checked = true;
                    wmiObjectNameTextBox.Text = counterDefinition.ObjectName;
                    wmiCounterNameTextBox.Text = counterDefinition.CounterName;
                    wmiInstanceNameTextBox.Text = counterDefinition.InstanceName;
                    page = wmiCounterPage;
                    wmiCounterPage_BeforeDisplay(counterTypePage, EventArgs.Empty);
                    break;
                case MetricType.SQLCounter:
                    sqlCounterButton.Checked = true;
                    sqlManualModeButton.Checked = true;
                    sqlObjectNameTextBox.Text = counterDefinition.ObjectName;
                    sqlCounterNameTextBox.Text = counterDefinition.CounterName;
                    sqlInstanceNameTextBox.Text = counterDefinition.InstanceName;
                    page = sqlCounterPage;
                    sqlCounterPage_BeforeDisplay(counterTypePage, EventArgs.Empty);
                    break;
                case MetricType.VMCounter:
                    vmCounterButton.Checked = true;
                    vmManualModeButton.Checked = true;
                    vmObjectNameTextBox.Text = counterDefinition.ObjectName;
                    vmCounterNameTextBox.Text = counterDefinition.CounterName;
                    vmInstanceNameTextBox.Text = counterDefinition.InstanceName;
                    page = vmCounterPage;
                    vmCounterPage_BeforeDisplay(counterTypePage, EventArgs.Empty);
                    break;
                case MetricType.SQLStatement:
                    sqlBatchButton.Checked = true;
                    sqlBatchTextBox.Text = counterDefinition.SqlStatement;
                    page = sqlBatchPage;
                    sqlBatchPage_BeforeDisplay(counterTypePage, EventArgs.Empty);
                    break;
                case MetricType.AzureCounter:
                    InitializeAzureComboboxes();
                    azureCounterButton.Checked = true;
                    if (!addMode)
                    {
                        GetDataForSavedAzureCounter(counterDefinition);
                    }
                    AzureServerConfigPage_BeforeDisplay(counterTypePage, EventArgs.Empty);
                    page = azureServerConfigPage;

                    break;


            }
            // set the scale
            if (!counterScaleComboBox.Items.Contains(counterDefinition.Scale))
                counterScaleComboBox.Items.Add(counterDefinition.Scale);
            counterScaleComboBox.SelectedItem = counterDefinition.Scale;

            infoThresholdSpinner.Maximum = metricDefinition.MaxValue;
            warningThresholdSpinner.Maximum = metricDefinition.MaxValue;
            criticalThresholdSpinner.Maximum = metricDefinition.MaxValue;

            // set both the spinner and its shadow value 
            AlertConfiguration alertConfig = RepositoryHelper.GetDefaultAlertConfiguration(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                RepositoryHelper.GetTemplateID(Settings.Default.ActiveRepositoryConnection.ConnectionInfo),
                //true,
                metric);
            AlertConfigurationItem[] alertConfigItems = alertConfig.ItemList;
            if (alertConfigItems.Length == 1)
            {
                object infoValue = alertConfigItems[0].ThresholdEntry.InfoThreshold.Value;
                if (infoValue != null)
                    infoThresholdValue = Convert.ToDecimal(infoValue);
                    infoThresholdEnabled = alertConfigItems[0].ThresholdEntry.InfoThreshold.Enabled;
                object warningValue = alertConfigItems[0].ThresholdEntry.WarningThreshold.Value;
                if (warningValue != null)
                    warningThresholdValue = Convert.ToDecimal(warningValue);
                object criticalValue = alertConfigItems[0].ThresholdEntry.CriticalThreshold.Value;
                if (criticalValue != null)
                    criticalThresholdValue = Convert.ToDecimal(criticalValue);

                enableAlertCheckBox.Checked = alertConfigItems[0].Enabled;

                object data = alertConfigItems[0].ThresholdEntry.Data;
                if (data is AdvancedAlertConfigurationSettings)
                    alertOnFailureToCollect = ((AdvancedAlertConfigurationSettings)data).TreatCustomCounterFailureAsCritical;
                else
                if (data is bool)
                    alertOnFailureToCollect = (bool)data;
            }
            else
            {
                if (metricDefinition.DefaultInfoThresholdValue < 0)
                    infoThresholdValue = null;
                else
                    infoThresholdValue = metricDefinition.DefaultInfoThresholdValue;
                warningThresholdValue = metricDefinition.DefaultWarningThresholdValue;
                criticalThresholdValue = metricDefinition.DefaultCriticalThresholdValue;
                enableAlertCheckBox.Checked = metricDefinition.AlertEnabledByDefault;
            }
            if (infoThresholdEnabled && infoThresholdValue.HasValue)
                infoThresholdSpinner.Value = infoThresholdValue.Value;
            else
                infoThresholdSpinner.Text = "";
            warningThresholdSpinner.Value = warningThresholdValue.Value;
            criticalThresholdSpinner.Value = criticalThresholdValue.Value;

            if (counterDefinition.CalculationType == CalculationType.Value)
                calcTypeValueButton.Checked = true;
            else
                calcTypeDeltaButton.Checked = true;

            if (metricDefinition.ComparisonType == ComparisonType.GE)
                comparisonTypeGreaterThanButton.Checked = true;
            else
                comparisonTypeLessThanButton.Checked = true;

            alertNameTextBox.Enabled = false;
            alertCategoryComboBox.Enabled = false;

            // can't change the counter type
            sqlBatchButton.Enabled = sqlBatchButton.Checked;
            sqlCounterButton.Enabled = sqlCounterButton.Checked;
            wmiCounterButton.Enabled = wmiCounterButton.Checked;
            vmCounterButton.Enabled  = vmCounterButton.Checked;
            azureCounterButton.Enabled = azureCounterButton.Checked;

            // configure the wizard pages to edit this type of counter
            counterTypePage_BeforeMoveNext(null, new CancelEventArgs());
            // set the initial page
            wizard.SelectedPage = page;
            // prevent moving to previous on initial page
            page.PreviousPage = null;
        }

        private void GetDataForSavedAzureCounter(CustomCounterDefinition def)
        {
            azureEditModeData = RepositoryHelper.GetAzureProfileForResourceUri(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, def.InstanceName, def.ServerType.ToString(), def.MetricID);

            foreach (DataRow row in azureEditModeData.Rows)
            {
                //TODO remove unnecessary fields
                azureApplicationProfileId = row["ProfileId"].ToString();
                azureResourceName = row["ResourceName"].ToString();
                azureResourceType = row["ResourceType"].ToString();
                azureMetricDisplayName = row["MetricDisplayName"].ToString();
                azureServerId = row["AzureSqlServerId"].ToString();
                break;
            }

            if (counterDefinition == null) {
                return;
            }

            // Import case
            // Ignore azureApplicationProfileId to prevent invalid linking of the profiles
            if (string.IsNullOrWhiteSpace(azureServerId) && !string.IsNullOrWhiteSpace(counterDefinition.InstanceName))
            {
                var server =
                    ApplicationModel.Default.ActiveInstances.FirstOrDefault(
                        s => s.InstanceName == counterDefinition.InstanceName);
                
                if (server != null)
                {
                    azureServerId = server.Id.ToString();
                }
            }

            if (string.IsNullOrEmpty(azureResourceName))
            {
                azureResourceName = counterDefinition.ObjectName;
            }

            if (string.IsNullOrEmpty(azureResourceType))
            {
                azureResourceType = counterDefinition.SqlStatement;
            }

            if (string.IsNullOrEmpty(azureMetricDisplayName))
            {
                azureMetricDisplayName = counterDefinition.CounterName;
            }
        }

        private void InitializeWmiDataTables()
        {
            wmiObjectsDataTable = new DataTable("Object Names");
            wmiObjectsDataTable.Columns.Add("Name", typeof(string));
            wmiObjectsDataTable.Columns.Add("Description", typeof(string));
            wmiObjectsDataTable.Columns.Add("DisplayName", typeof(string));
            wmiObjectsDataTable.Columns.Add("Singleton", typeof(bool));
            wmiObjectsDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            wmiCountersDataTable = new DataTable("Counter Names");
            wmiCountersDataTable.Columns.Add("Name", typeof(string));
            wmiCountersDataTable.Columns.Add("Description", typeof(string));
            wmiCountersDataTable.Columns.Add("DisplayName", typeof(string));
            wmiCountersDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            wmiInstancesDataTable = new DataTable("Instances");
            wmiInstancesDataTable.Columns.Add("Name", typeof(string));
            wmiInstancesDataTable.Columns.Add("DisplayName", typeof(string));
            wmiInstancesDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            wmiObjectNameComboBox.DataSource = new DataView(wmiObjectsDataTable);
            wmiCounterNameComboBox.DataSource = new DataView(wmiCountersDataTable);
            wmiInstanceNameComboBox.DataSource = new DataView(wmiInstancesDataTable);

            wmiObjectNameComboBox.ValueMember = "Name";
            wmiCounterNameComboBox.ValueMember = "Name";
            wmiInstanceNameComboBox.ValueMember = "Name";

            wmiShowAllCountersButton_CheckedChanged(wmiShowAllCountersButton, EventArgs.Empty);
        }

        private void InitializeSqlDataTables()
        {
            sqlObjectsDataTable = new DataTable("Object Names");
            sqlObjectsDataTable.Columns.Add("Name", typeof(string));
            sqlObjectsDataTable.Columns.Add("Description", typeof(string));
            sqlObjectsDataTable.Columns.Add("DisplayName", typeof(string));
            sqlObjectsDataTable.Columns.Add("Singleton", typeof(bool));
            sqlObjectsDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            sqlCountersDataTable = new DataTable("Counter Names");
            sqlCountersDataTable.Columns.Add("Name", typeof(string));
            sqlCountersDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            sqlInstancesDataTable = new DataTable("Instances");
            sqlInstancesDataTable.Columns.Add("Name", typeof(string));
            sqlInstancesDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            sqlObjectNameComboBox.DataSource = new DataView(sqlObjectsDataTable);
            sqlCounterNameComboBox.DataSource = new DataView(sqlCountersDataTable);
            sqlInstanceNameComboBox.DataSource = new DataView(sqlInstancesDataTable);

            sqlObjectNameComboBox.ValueMember = "Name";
            sqlCounterNameComboBox.ValueMember = "Name";
            sqlInstanceNameComboBox.ValueMember = "Name";

            SetSqlSelection();
        }

        private void InitializeVmDataTables()
        {
            vmObjectsDataTable = new DataTable("Object Names");
            vmObjectsDataTable.Columns.Add("Name", typeof(string));
            vmObjectsDataTable.Columns.Add("Description", typeof(string));
            vmObjectsDataTable.Columns.Add("DisplayName", typeof(string));
            vmObjectsDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));
            vmObjectsDataTable.Columns.Add("Source", typeof (string));
            vmObjectsDataTable.Columns.Add("Group", typeof (string));

            vmCountersDataTable = new DataTable("Counter Names");
            vmCountersDataTable.Columns.Add("Name", typeof(string));
            vmCountersDataTable.Columns.Add("Description", typeof(string));
            vmCountersDataTable.Columns.Add("DisplayName", typeof(string));
            vmCountersDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));
            vmCountersDataTable.Columns.Add("Source", typeof(string));
            vmCountersDataTable.Columns.Add("Group", typeof(string));
            vmCountersDataTable.Columns.Add("CounterKey", typeof(int));
            vmCountersDataTable.Columns.Add("Singleton", typeof (bool));

            vmInstancesDataTable = new DataTable("Instances");
            vmInstancesDataTable.Columns.Add("Name", typeof(string));
            vmInstancesDataTable.Columns.Add("DisplayName", typeof(string));
            vmInstancesDataTable.Columns.Add(SPECIAL_COLUMN, typeof(bool));

            vmObjectNameComboBox.DataSource = new DataView(vmObjectsDataTable);
            vmCounterNameComboBox.DataSource = new DataView(vmCountersDataTable);
            vmInstanceNameComboBox.DataSource = new DataView(vmInstancesDataTable);

            vmObjectNameComboBox.ValueMember = "Name";
            vmCounterNameComboBox.ValueMember = "Name";
            vmInstanceNameComboBox.ValueMember = "Name";

            vmObjectNameComboBox.DisplayMember = "DisplayName";
            vmCounterNameComboBox.DisplayMember = "DisplayName";
            vmInstanceNameComboBox.DisplayMember = "DisplayName";
        }

        /// <summary>
        /// Test this string for chars that are not allowabel in a custom counter name
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        private static bool CounterNameIsValid(string counterName)
        {
            if (string.IsNullOrEmpty(counterName)) return false;

            foreach (char c in counterName)
            {
                if (char.IsControl(c)) return false;
                if (char.IsSurrogate(c)) return false;
                if (INVALIDCOUNTERCHARS.Contains(c.ToString())) return false;
            }
            foreach (string reservedName in RESERVEDNAMES)
            {
                if (counterName.ToLower().Equals(reservedName.ToLower())) return false;
            }
            if (!Regex.IsMatch(counterName, "^[A-Za-z]+[A-Za-z0-9_ ]+$"))
            {
                return false;
            }
            return true;
        }

        private bool IsSqlCounterPageComplete
        {
            get
            {
                if (sqlBrowseModeButton.Checked)
                {
                    DataRowView row = sqlObjectNameComboBox.SelectedItem as DataRowView;
                    if (row != null && !IsSpecialRow(row))
                    {
                        bool singleton = (bool)row[SINGLETON_COLUMN];
                        row = sqlCounterNameComboBox.SelectedItem as DataRowView;
                        if (row != null && !IsSpecialRow(row))
                        {
                            row = sqlInstanceNameComboBox.SelectedItem as DataRowView;
                            if (singleton || (row != null && !IsSpecialRow(row)))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    string text = sqlObjectNameTextBox.Text.TrimEnd();
                    if (text.Length > 0)
                    {
                        text = sqlCounterNameTextBox.Text.TrimEnd();
                        if (text.Length > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool IsWmiCounterPageComplete
        {
            get
            {
                if (wmiBrowseModeButton.Checked)
                {
                    DataRowView row = wmiObjectNameComboBox.SelectedItem as DataRowView;
                    if (row != null && !IsSpecialRow(row))
                    {
                        bool singleton = (bool)row[SINGLETON_COLUMN];
                        row = wmiCounterNameComboBox.SelectedItem as DataRowView;
                        if (row != null && !IsSpecialRow(row))
                        {
                            row = wmiInstanceNameComboBox.SelectedItem as DataRowView;
                            if (singleton || (row != null && !IsSpecialRow(row)))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    string text = wmiObjectNameTextBox.Text.TrimEnd();
                    if (text.Length > 0)
                    {
                        text = wmiCounterNameTextBox.Text.TrimEnd();
                        if (text.Length > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool IsVmCounterPageComplete
        {
            get
            {
                if (vmBrowseModeButton.Checked)
                {
                    DataRowView row = vmObjectNameComboBox.SelectedItem as DataRowView;
                    if (row != null && !IsSpecialRow(row))
                    {
                        row = vmCounterNameComboBox.SelectedItem as DataRowView;
                        if (row != null && !IsSpecialRow(row))
                        {
                            bool singleton = (bool)row[SINGLETON_COLUMN];
                            row = vmInstanceNameComboBox.SelectedItem as DataRowView;
                            if (singleton || (row != null && !IsSpecialRow(row)))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    string text = vmObjectNameTextBox.Text.TrimEnd();
                    if (text.Length > 0)
                    {
                        text = vmCounterNameTextBox.Text.TrimEnd();
                        if (text.Length > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool IsAzureServerConfigPageComplete
        {
            get
            {
                return azureServerComboBox.SelectedIndex != azureServerComboBox.Items.IndexOf(SELECT_A_SERVER) &&
          azureProfileComboBox.SelectedIndex != azureProfileComboBox.Items.IndexOf(SELECT_A_PROFILE) &&
          azureResourceTypeNameComboBox.SelectedIndex != azureResourceTypeNameComboBox.Items.IndexOf(SELECT_A_RESOURCE_TYPE) &&
              azureResourceNameComboBox.SelectedIndex != azureResourceNameComboBox.Items.IndexOf(SELECT_A_RESOURCE);
            }
        }

        private bool IsAzureServerMetricsPageComplete
        {
            get { return azureMetricNameComboBox.SelectedIndex != azureMetricNameComboBox.Items.IndexOf(SELECT_A_METRIC); }
        }

        private bool IsSqlBatchPageComplete
        {
            get { return IsNotEmpty(sqlBatchTextBox.Text); }
        }

        private bool IsCalculationTypePageComplete
        {
            get
            {
                return IsNotEmpty(counterScaleComboBox.Text);
            }
        }

        private bool IsCounterNamePageComplete
        {
            get
            {
                return IsNotEmpty(alertNameTextBox.Text) &&
                       IsNotEmpty(alertCategoryComboBox.Text);
            }
        }

        private bool IsAlertDefinitionPageComplete
        {
            get
            {
                if (!warningThresholdValue.HasValue || !criticalThresholdValue.HasValue)
                    return false;
                if (comparisonTypeGreaterThanButton.Checked)
                {
                    if (infoThresholdValue.HasValue)
                        return ((infoThresholdValue.Value <= warningThresholdValue.Value) && (warningThresholdValue.Value <= criticalThresholdValue.Value));
                    else
                        return (warningThresholdValue.Value <= criticalThresholdValue.Value);
                }
                else
                {
                    if (infoThresholdValue.HasValue)
                        return ((infoThresholdValue.Value >= warningThresholdValue.Value) && (warningThresholdValue.Value >= criticalThresholdValue.Value));
                    else
                        return (warningThresholdValue.Value >= criticalThresholdValue.Value);
                }
            }
        }

        internal static DialogResult CreateNewCounter(IWin32Window owner, out Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>? newCounter)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            CustomCounterWizard dialog = null;
            try
            {
                dialog = new CustomCounterWizard();
                dialog.Text = "Add Custom Counter";
                dialogResult = dialog.ShowDialog(owner);
                newCounter = dialog.counterUpdated;
            }
            catch (Exception e)
            {
                newCounter = null;
                if (dialog != null)
                {
                    dialog.Dispose();
                    dialog = null;
                }
                ApplicationMessageBox.ShowError(owner,
                                                "Error attempting to add custom counter.  Please resolve the error and try again.",
                                                e);
            }
            finally
            {
                if (dialog != null)
                    dialog.Dispose();
            }
            return dialogResult;
        }

        internal static DialogResult ChangeCounter(IWin32Window owner, MetricDefinition metricDefinition,
                                           MetricDescription metricDescription,
                                           CustomCounterDefinition counterDefinition,
                                           out Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>? changedCounter)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            CustomCounterWizard dialog = null;
            try
            {
                dialog = new CustomCounterWizard(metricDefinition, metricDescription, counterDefinition);
                dialog.Text = "Edit Custom Counter";
                dialogResult = dialog.ShowDialog(owner);
                changedCounter = dialog.counterUpdated;
            }
            catch (Exception e)
            {
                changedCounter = null;
                if (dialog != null)
                {
                    dialog.Dispose();
                    dialog = null;
                }
                ApplicationMessageBox.ShowError(owner,
                                                "Error attempting to edit custom counter.  Please resolve the error and try again.",
                                                e);
            }
            finally
            {
                if (dialog != null)
                    dialog.Dispose();
            }
            return dialogResult;
        }

        private void counterIdentificationPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
        }

        private void sqlCounterPage_BeforeDisplay(object sender, EventArgs e)
        {
            UpdateSqlCounterPageControls();
            LoadCounterSource(sqlServerComboBox);
            DisableSqlComboBoxes();
            EnableSqlComboBoxes(true, true);
            sqlCounterPage.AllowMoveNext = IsSqlCounterPageComplete;
            sqlObjectNameComboBox.Focus();
        }

        private void wmiCounterPage_BeforeDisplay(object sender, EventArgs e)
        {
            UpdateWmiCounterPageControls();
            // DM 10.3 (Varun Chopra) SQLDM-28744 - CustomCounters for Non Linux Instances
            LoadCounterSource(wmiServerComboBox, false, true);
            wmiCounterPage.AllowMoveNext = IsWmiCounterPageComplete;
            wmiServerComboBox.Focus();
        }

        private void sqlBatchPage_BeforeDisplay(object sender, EventArgs e)
        {
            sqlBatchPage.AllowMoveNext = IsSqlBatchPageComplete;
            sqlBatchTextBox.Focus();
        }

        private void vmCounterPage_BeforeDisplay(object sender, EventArgs e)
        {
            UpdateVmCounterPageControls();
            LoadCounterSource(vmServerComboBox, true);
            vmCounterPage.AllowMoveNext = IsVmCounterPageComplete;
            vmServerComboBox.Focus();
        }

        private void AzureServerConfigPage_BeforeDisplay(object sender, EventArgs e)
        {
            if (azureServerComboBox.Items.Count == 1)
            {
                // Allow Azure only Servers for Azure Counters
                LoadCounterSource(azureServerComboBox, false, false, true);
                LoadAzureControls(azureResourceTypeNameComboBox);
            }

            azureServerConfigPage.AllowMoveNext = IsAzureServerConfigPageComplete;
            azureServerComboBox.Focus();
        }

        private void azureServerMetricsPage_BeforeDisplay(object sender, EventArgs e)
        {
            if (profile.ResourceType != azureResourceTypeNameComboBox.Text)
            {
                profile.ResourceType = azureResourceTypeNameComboBox.Text;
                
                // Load the azure metric controls
                LoadAzureMetricControls(azureMetricNameComboBox);
            }
            else
            {
                azureMetricPageResourceGroupNameLbl.Text = azureResourceName;
                // Edit mode
                if (!string.IsNullOrEmpty(azureMetricDisplayName))
                {
                    foreach (var item in azureMetricNameComboBox.Items)
                    {
                        var azureMetric = (string) item;
                        if (azureMetricDisplayName.Equals(azureMetric, StringComparison.InvariantCultureIgnoreCase))
                        {
                            azureMetricNameComboBox.SelectedIndex = azureMetricNameComboBox.Items.IndexOf(item);
                            break;
                        }
                    }

                    azureMetricDisplayName = azureMetricNameComboBox.SelectedIndex > 0
                        ? azureMetricNameComboBox.Text
                        : azureMetricDisplayName;
                }
                else
                {
                    azureMetricDisplayName = azureMetricNameComboBox.SelectedIndex > 0
                        ? azureMetricNameComboBox.Text
                        : azureMetricDisplayName;
                }
            }

            azureServerMetricsPage.AllowMoveNext = IsAzureServerMetricsPageComplete;
            azureMetricNameComboBox.Focus();
        }

        private void UpdateVmCounterPageControls()
        {
            bool browseMode = vmBrowseModeButton.Checked;
            vmBrowseModeContentPanel.Visible = browseMode;
            vmManualModeContentPanel.Visible = !browseMode;
        }

        private void counterTypePage_BeforeDisplay(object sender, EventArgs e)
        {
        }

//        private void counterIdentificationPage_BeforeDisplay(object sender, EventArgs e)
//        {
//            counterIdentificationPage.AllowMoveNext = IsCounterIdentificationPageComplete;
//        }

        private static bool IsNotEmpty(string value)
        {
            return value.Trim().Length > 0;
        }

        private void alertDefinitionPage_BeforeDisplay(object sender, EventArgs e)
        {
            alertDefinitionPage.AllowMoveNext = IsAlertDefinitionPageComplete;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            CustomCounterDefinition counterDefinition = GetCounterDefinition();

            string name = alertNameTextBox.Text.TrimEnd();
            string desc = alertDescriptionTextBox.Text.TrimEnd();
            string category = alertCategoryComboBox.Text.TrimEnd();
           

            MetricDescription metricDescription = new MetricDescription(name, category, desc, String.Empty,0);

            using (TestCustomCounterDialog tccd = new TestCustomCounterDialog(counterDefinition, metricDescription, false))
            {
                tccd.ShowDialog(this);
            }
        }

        internal CustomCounterDefinition GetCounterDefinition()
        {
            CustomCounterDefinition counterDefinition = new CustomCounterDefinition();
            counterDefinition.MetricID = metric;

            if (wmiCounterButton.Checked)
            {
                counterDefinition.MetricType = MetricType.WMI;
                counterDefinition.ObjectName = wmiObjectNameTextBox.Text;
                counterDefinition.CounterName = wmiCounterNameTextBox.Text;
                counterDefinition.InstanceName = wmiInstanceNameTextBox.Text;
                if (counterDefinition.InstanceName.Equals(SELECT_AN_INSTANCE))
                    counterDefinition.InstanceName = String.Empty;
            }
            else if (sqlCounterButton.Checked)
            {
                counterDefinition.MetricType = MetricType.SQLCounter;
                counterDefinition.ObjectName = sqlObjectNameTextBox.Text.TrimEnd();
                counterDefinition.CounterName = sqlCounterNameTextBox.Text.TrimEnd();
                counterDefinition.InstanceName = sqlInstanceNameTextBox.Text.TrimEnd();
                if (counterDefinition.InstanceName.Equals(SELECT_AN_INSTANCE))
                    counterDefinition.InstanceName = String.Empty;
            }
            else if (vmCounterButton.Checked)
            {
                counterDefinition.MetricType = MetricType.VMCounter;
                counterDefinition.ObjectName = vmObjectNameTextBox.Text.TrimEnd();
                counterDefinition.CounterName = vmCounterNameTextBox.Text.TrimEnd();
                counterDefinition.InstanceName = vmInstanceNameTextBox.Text.TrimEnd();
                WrappedSqlServerObject wrapper = vmServerComboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper != null)
                {
                    MonitoredSqlServerWrapper instance = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    if (instance != null)
                    {
                        counterDefinition.ServerType = instance.Instance.VirtualizationConfiguration.VCServerType;
                    }
                }
                
                if (counterDefinition.InstanceName.Equals(SELECT_AN_INSTANCE))
                    counterDefinition.InstanceName = String.Empty;
            }
            else if (azureCounterButton.Checked)
            {
                counterDefinition.MetricType = MetricType.AzureCounter;
                counterDefinition.ObjectName = ((TextItem<IAzureResource>)azureResourceNameComboBox.SelectedItem).Text;
                counterDefinition.CounterName = azureMetricNameComboBox.Text;
                counterDefinition.InstanceName = azureServerComboBox.Text;
                counterDefinition.SqlStatement = ((TextItem<IAzureResource>)azureResourceNameComboBox.SelectedItem).Value.Type;
                counterDefinition.ServerType = ((TextItem<IAzureResource>)azureResourceNameComboBox.SelectedItem).Value.Uri;
                counterDefinition.ProfileId = Convert.ToInt32(((TextItem<IAzureProfile>)azureProfileComboBox.SelectedItem).Value.Id);
            }
            else
            {
                counterDefinition.MetricType = MetricType.SQLStatement;
                counterDefinition.SqlStatement = sqlBatchTextBox.Text.TrimEnd();
            }
            counterDefinition.Scale = GetScale(counterScaleComboBox);
            counterDefinition.CalculationType = calcTypeValueButton.Checked
                                                    ? CalculationType.Value
                                                    : CalculationType.Delta;

            return counterDefinition;
        }

        private MetricDescription GetMetricDescription()
        {
            return new MetricDescription(alertNameTextBox.Text, alertCategoryComboBox.Text, alertDescriptionTextBox.Text,null, 0);
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            counterNamePage.AllowMoveNext = IsCounterNamePageComplete;
        }

        private void categoryComboBox_TextChanged(object sender, EventArgs e)
        {
            counterNamePage.AllowMoveNext = IsCounterNamePageComplete;
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            counterNamePage.AllowMoveNext = IsCounterNamePageComplete;
        }

        private void sqlBatchTextBox_TextChanged(object sender, EventArgs e)
        {
            sqlBatchPage.AllowMoveNext = IsSqlBatchPageComplete;
        }

        private void categoryComboBox_DropDown(object sender, EventArgs args)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string[] items = RepositoryHelper.GetCounterCategories(connectionInfo);
                alertCategoryComboBox.Items.Clear();
                alertCategoryComboBox.Items.AddRange(items);
            }
            catch
            {
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CustomCounterWizard_Load(object sender, EventArgs e)
        {
            hideWelcomePageCheckBox.Checked = Settings.Default.HideCustomCounterWizardWelcomePage;

            if (addMode && Settings.Default.HideCustomCounterWizardWelcomePage)
            {
                wizard.GoNext();
            }

            if (addMode)
            {
                infoThresholdSpinner.Text = "";
                warningThresholdSpinner.Text = "";
                criticalThresholdSpinner.Text = "";
            }
//            if (sqlCounterButton.Checked)
//                counterTypeRadioButton_CheckedChanged(sqlCounterButton, EventArgs.Empty);
//            else
//            if (wmiCounterButton.Checked)
//                counterTypeRadioButton_CheckedChanged(wmiCounterButton, EventArgs.Empty);
//            else
//            if (sqlBatchButton.Checked)
//                counterTypeRadioButton_CheckedChanged(sqlBatchButton, EventArgs.Empty);
        }

        private void wizard_Finish(object sender, EventArgs args)
        {
            CustomCounterDefinition counterDefinition = GetCounterDefinition();

            MetricDescription metricDescription = GetMetricDescription();

            MetricDefinition metricDefinition = new MetricDefinition(counterDefinition.MetricID);

            metricDefinition.AlertEnabledByDefault = enableAlertCheckBox.Checked;
            metricDefinition.ComparisonType = comparisonTypeGreaterThanButton.Checked
                                                  ? ComparisonType.GE
                                                  : ComparisonType.LE;

            metricDefinition.DefaultMessageID = 0;
            metricDefinition.MinValue = Convert.ToInt32(infoThresholdSpinner.Minimum);
            metricDefinition.MaxValue = Convert.ToInt64(criticalThresholdSpinner.Maximum);

            metricDefinition.DefaultInfoThresholdValue = infoThresholdEnabled ? (long)infoThresholdSpinner.Value : (long)metricDefinition.MinValue - 1;
            metricDefinition.DefaultWarningThresholdValue = (long)warningThresholdSpinner.Value;
            metricDefinition.DefaultCriticalThresholdValue = (long)criticalThresholdSpinner.Value;

            metricDefinition.ProcessNotifications = true;
            metricDefinition.Rank = 50;
            metricDefinition.Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue |
                                       ThresholdOptions.CalculateMaxValue | ThresholdOptions.AdditionalData;

            try
            {
                if (addMode)
                {
                    int newMetricID =
                        managementService.AddCustomCounter(metricDefinition, metricDescription, counterDefinition,
                                                           alertOnFailureToCollect);
                    metricDefinition.MetricID = newMetricID;
                    counterDefinition.MetricID = newMetricID;
                }
                else
                {
                    managementService.UpdateCustomCounter(metricDefinition, metricDescription, counterDefinition,
                                                          alertOnFailureToCollect);
                }
                counterUpdated =
                    new Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>(metricDefinition,
                                                                                             metricDescription,
                                                                                             counterDefinition);
            } catch (Exception e)
            {
                ApplicationMessageBox.ShowError(
                    this,
                    String.Format("There was an error trying to {0} the custom counter.  Please resolve the error and try again.", addMode ? "add" : "change"),
                    e);
                DialogResult = DialogResult.None;
            }
        }

        private double GetScale(ComboBox scaleComboBox)
        {
            if (scaleComboBox.SelectedItem is double)
                return (double)scaleComboBox.SelectedItem;

            try
            {
                return Convert.ToDouble(scaleComboBox.Text.Trim());
            }
            catch
            {
            }
            return 1.0d;
        }

        private static void LoadScaleComboBox(ComboBox scaleCombo)
        {
            scaleCombo.Items.Clear();
            ComboBox.ObjectCollection items = scaleCombo.Items;
            items.Add(0.0000001d);
            items.Add(0.000001d);
            items.Add(0.00001d);
            items.Add(0.0001d);
            items.Add(0.001d);
            items.Add(0.01d);
            items.Add(0.1d);
            items.Add(1d);
            items.Add(10d);
            items.Add(100d);
            items.Add(1000d);
            items.Add(10000d);
            items.Add(100000d);
            items.Add(1000000d);
            items.Add(10000000d);

            scaleCombo.SelectedItem = 1d;
        }

        #region Nested type: EnumWrapper
        /*
        public class EnumWrapper<T>
        {
            public readonly T Value;

            public EnumWrapper(T value)
            {
                this.Value = value;
            }

            public override string ToString()
            {
                return ApplicationHelper.GetEnumDescription(Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Value.Equals(obj);
            }
        }
        */
        #endregion

        #region Nested type: ObjectWrapper
        /*
        public class TripleWrapper
        {
            Triple<string, string, string> info;

            public TripleWrapper(Triple<string, string, string> info)
            {
                this.info = info;
            }

            public string First
            {
                get { return info.First; }
                set { info.First = value; }
            }
            public string Second
            {
                get { return info.Second; }
                set { info.Second = value; }
            }
            public string Third
            {
                get { return info.Third; }
                set { info.Third = value; }
            }
        }
        */
        /*
        public class ObjectWrapper<T>
        {
            private static MemberInfo displayMember = null;

            public readonly T Value;
            private string displayPropertyName;

            public ObjectWrapper(T value, string displayProperty)
            {
                this.Value = value;
                this.displayPropertyName = displayProperty;
                SetGetter();
            }

            private void SetGetter()
            {
                if (displayMember == null)
                {
                    Type t = typeof(T);

                    PropertyInfo pi = t.GetProperty(displayPropertyName);
                    if (pi != null)
                        displayMember = pi.GetGetMethod();
                    else
                    {
                        FieldInfo fi = t.GetField(displayPropertyName);
                        if (fi != null)
                            displayMember = fi;
                    }
                    if (displayMember == null)
                        throw new ArgumentException("Unable to resolve getter for display property", "displayProperty");
                }
            }

            public override string ToString()
            {
                object result = null;
                if (displayMember is MethodInfo)
                    result = ((MethodInfo)displayMember).Invoke(Value, null);
                else
                    if (displayMember is FieldInfo)
                        result = ((FieldInfo)displayMember).GetValue(Value);

                return result != null ? result.ToString() : Value.ToString();
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj != null)
                    return String.Equals(this.ToString(), obj.ToString());
                return false;
            }
        }
        */
        #endregion

        private void counterTypePage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            // reconfigure the pages based on the type of counter being defined
            // set default alert for azure only
            if (!alertOnFailureToCollectModified)
            {
                alertOnFailureToCollect = azureCounterButton.Checked;
            }
            if (wmiCounterButton.Checked)
            {
                counterTypePage.NextPage = wmiCounterPage;
                wmiCounterPage.NextPage = calculationTypePage;
                wmiCounterPage.PreviousPage = counterTypePage;
                calculationTypePage.PreviousPage = wmiCounterPage;
            }
            else if (sqlCounterButton.Checked)
            {
                counterTypePage.NextPage = sqlCounterPage;
                sqlCounterPage.NextPage = calculationTypePage;
                sqlCounterPage.PreviousPage = counterTypePage;
                calculationTypePage.PreviousPage = sqlCounterPage;
            }
            else if (vmCounterButton.Checked)
            {
                counterTypePage.NextPage = vmCounterPage;
                vmCounterPage.NextPage = calculationTypePage;
                vmCounterPage.PreviousPage = counterTypePage;
                calculationTypePage.PreviousPage = vmCounterPage;
            }
            else if (azureCounterButton.Checked)
            {
                // Define the next and previous page
                counterTypePage.NextPage = azureServerConfigPage;
                azureServerConfigPage.NextPage = azureServerMetricsPage;
                azureServerMetricsPage.NextPage = calculationTypePage;
                calculationTypePage.PreviousPage = azureServerMetricsPage;
                azureServerMetricsPage.PreviousPage = azureServerConfigPage;
                azureServerConfigPage.PreviousPage = counterTypePage;
            }
            else
            {
                counterTypePage.NextPage = sqlBatchPage;
                sqlBatchPage.NextPage = calculationTypePage;
                sqlBatchPage.PreviousPage = counterTypePage;
                calculationTypePage.PreviousPage = sqlBatchPage;
            }
        }

        private string GetSelectedWmiObjectDescription()
        {
            string result = null;
            if (wmiObjectNameComboBox.Items.Count == 0)
            {
                wmiObjectNameComboBox_DropDown(wmiObjectNameComboBox, EventArgs.Empty);
            }
            if (wmiObjectNameComboBox.SelectedItem == null)
            {   // select the item corresponding to the text
                int i = wmiObjectNameComboBox.Items.IndexOf(wmiObjectNameComboBox.Text);
                if (i >= 0)
                    wmiObjectNameComboBox.SelectedIndex = i;
            }
            DataRowView oItem = wmiObjectNameComboBox.SelectedItem as DataRowView;
            if (oItem != null)
            {
                result = oItem["Description"] as string;
            }

            return result ?? String.Empty;
        }

        private string GetSelectedWmiCounterDescription()
        {
            string result = null;
            if (wmiObjectNameComboBox.Text.Trim().Length > 0 &&
                wmiCounterNameComboBox.Items.Count == 0)
            {
                wmiCounterNameComboBox_DropDown(wmiCounterNameComboBox, EventArgs.Empty);
            }
            if (wmiCounterNameComboBox.SelectedItem == null)
            {   // select the item corresponding to the text
                int i = wmiCounterNameComboBox.Items.IndexOf(wmiCounterNameComboBox.Text);
                if (i >= 0)
                    wmiCounterNameComboBox.SelectedIndex = i;
            }

            DataRowView cItem = wmiCounterNameComboBox.SelectedItem as DataRowView;
            if (cItem != null)
            {
                result = cItem["Description"] as string;
            }

            return result != null ? result.TrimEnd() : String.Empty;
        }


        private void wmiInfoButton_Click(object sender, EventArgs e)
        {
            string objectName = wmiObjectNameComboBox.Text;
            string objectDesc = GetSelectedWmiObjectDescription();
            string counterName = wmiCounterNameComboBox.Text;
            string counterDesc = GetSelectedWmiCounterDescription();
            using (WmiObjectInfoDialog dialog = new WmiObjectInfoDialog(objectName, objectDesc, counterName, counterDesc))
            {
                dialog.ShowDialog(ParentForm);
            }
        }
           
        private void introPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void wmiShowAllCountersButton_CheckedChanged(object sender, EventArgs e)
        {
            string selectedObject = null;
            string counterObject = null;
            string instanceObject = null;

            DataRowView row = wmiObjectNameComboBox.SelectedItem as DataRowView;
            if (row != null)
                selectedObject = row[0] as string;
            row = wmiCounterNameComboBox.SelectedItem as DataRowView;
            if (row != null)
                counterObject = row[0] as string;
            row = wmiInstanceNameComboBox.SelectedItem as DataRowView;
            if (row != null)
                instanceObject = row[0] as string;

            string displayMember = "Name";
            string objectFilter = null;
            string counterFilter = null;

            if (wmiShowPerfCountersOnlyButton.Checked)
            {
                displayMember = "DisplayName";
                counterFilter = "LEN(ISNULL(DisplayName,'')) > 0";
                objectFilter = String.Format(
                    "(Name LIKE 'Win32_PerfFormattedData_*' OR {0} = true) AND {1}",
                    SPECIAL_COLUMN,
                    counterFilter);
            }

            string sort = String.Format("{0} DESC, {1}", SPECIAL_COLUMN, displayMember);

            try
            {
                Cursor = Cursors.WaitCursor;

                wmiObjectNameComboBox.BeginUpdate();
                wmiCounterNameComboBox.BeginUpdate();
                wmiInstanceNameComboBox.BeginUpdate();

                // set the filter and source on the wmiObjectNameComboBox datasource
                ((DataView)wmiObjectNameComboBox.DataSource).RowFilter = objectFilter;
                ((DataView)wmiObjectNameComboBox.DataSource).Sort = sort;
                // set the filter and source on the wmiCounterNameComboBox datasource
                ((DataView)wmiCounterNameComboBox.DataSource).RowFilter = counterFilter;
                ((DataView)wmiCounterNameComboBox.DataSource).Sort = sort;
                // set the filter and source on the wmiInstanceNameComboBox datasource
                ((DataView)wmiInstanceNameComboBox.DataSource).RowFilter = counterFilter;
                ((DataView)wmiInstanceNameComboBox.DataSource).Sort = sort;

                // set the column to show in the drop down
                wmiObjectNameComboBox.DisplayMember = displayMember;
                wmiCounterNameComboBox.DisplayMember = displayMember;
                wmiInstanceNameComboBox.DisplayMember = displayMember;

                // reapply selection to the combo boxes
                if (selectedObject != null)
                {
                    string selectedValue = ReselectRow(wmiObjectNameComboBox, selectedObject, SELECT_AN_OBJECT);
                    if (selectedValue == null || selectedValue.Equals(SELECT_AN_OBJECT))
                    {
                        InitWmiCounterPickListTable();
                        wmiCounterNameComboBox.Enabled = false;
                        InitWmiInstancePickListTable();
                        wmiInstanceNameComboBox.Enabled = false;
                    } 
                    if (counterObject != null)
                        ReselectRow(wmiCounterNameComboBox, counterObject, SELECT_A_COUNTER);
                    if (instanceObject != null)
                        ReselectRow(wmiInstanceNameComboBox, instanceObject, SELECT_AN_INSTANCE);
                }

                EnableWmiTestButton();
            }
            finally
            {
                wmiObjectNameComboBox.EndUpdate();
                wmiCounterNameComboBox.EndUpdate();
                wmiInstanceNameComboBox.EndUpdate();

                Cursor = Cursors.Default;
            }
        }

        private string ReselectRow(ComboBox comboBox, string rowIdentifier, string specialText)
        {
            DataRowView selectedRow = null;
            foreach (DataRowView row in comboBox.Items)
            {
                string value = row[0] as string;
                if (value != null && value.Equals(rowIdentifier))
                {
                    selectedRow = row;
                    break;
                }
            }
            if (selectedRow != null)
                comboBox.SelectedItem = selectedRow;
            else
                SetComboSelection(comboBox, specialText);

            selectedRow = comboBox.SelectedItem as DataRowView;
            return (selectedRow == null) ? specialText : selectedRow[0] as string;
        }

//        private void wmiBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            DataTable names = null;
//            ComboBox comboBox = null;
//            
//            if (e.Argument is Pair<ComboBox,int>)
//            {
//                Pair<ComboBox, int> context = (Pair<ComboBox, int>)e.Argument;
//                comboBox = context.First;
//                names = managementService.GetWmiObjectList(context.Second);                
//            } else
//            if (e.Argument is Triple<ComboBox,int,string>)
//            {
//                Triple<ComboBox,int,string> context = (Triple<ComboBox,int,string>)e.Argument;
//                comboBox = context.First;
//                if (comboBox == wmiObjectNameComboBox)
//                    names = managementService.GetWmiObjectList(context.Second);
//                else
//                if (comboBox == wmiCounterNameComboBox)
//                    names = managementService.GetWmiCounterList(context.Second, context.Third);
//                else
//                if (comboBox == wmiInstanceNameComboBox)
//                    names = managementService.GetWmiInstanceList(context.Second, context.Third);
//            }
//            
//            e.Result = new Pair<ComboBox, DataTable>(comboBox, names);
//        }
//
//        private void wmiBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            ComboBox comboBox = null;
//            DataTable comboDataSource = null;
//            try
//            {
//                if (e.Error != null)
//                    throw e.Error;
//
//                Pair<ComboBox, DataTable> result = (Pair<ComboBox, DataTable>)e.Result;
//                comboBox = result.First;
//
//                DataView view = comboBox.DataSource as DataView;
//                if (view != null)
//                {
//                    Cursor = Cursors.WaitCursor;
//
//                    comboDataSource = view.Table;
//                    comboDataSource.BeginLoadData();
//                    comboDataSource.Merge(result.Second, false);
//                }
//            }
//            catch
//            {
//                
//            }
//            finally
//            {
//                if (comboDataSource != null)
//                {
//                    comboDataSource.EndLoadData();
//                }
//
//                wmiObjectNameComboBox.Enabled = true;
//                if (wmiObjectNameComboBox.Text.Length > 0)
//                {
//                    wmiCounterNameComboBox.Enabled = true;
//                    wmiInstanceNameComboBox.Enabled = true;
//                }
//                
//                Cursor = Cursors.Default;
//            }
//        }

        private void wmiBrowseModeButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWmiCounterPageControls();
        }

        private void UpdateWmiCounterPageControls()
        {
            bool browseMode = wmiBrowseModeButton.Checked;
            wmiBrowseModeContentPanel.Visible = browseMode;
            wmiManualModeContentPanel.Visible = !browseMode;
            wmiInfoButton.Visible = browseMode;
        }

        #region Server Pick Lists

        private void serverPickList_DropDownClosed(object sender, EventArgs e)
        {
            SelectSelectAServerItem(sender as ComboBox, false);
        }

        private void LoadCounterSource(ComboBox comboBox, bool vmOnly = false, bool nonLinuxOnly = false, bool azureOnly = false)
        {
            string currentText = comboBox.Text.TrimEnd();

            comboBox.Items.Clear();

            var serverNames = new Dictionary<string, WrappedSqlServerObject>();
            foreach (var server in ApplicationModel.Default.ActiveInstances)
            {
                if (vmOnly && (server.Instance.VirtualizationConfiguration == null || String.IsNullOrEmpty(server.Instance.VirtualizationConfiguration.VCAddress)))
                    continue;

                // DM 10.3 (Varun Chopra) SQLDM-28744 - CustomCounters for Non Linux Instances
                if (nonLinuxOnly && server.Instance.CloudProviderId == Common.Constants.LinuxId)
                {
                    // exclude linux providers from windows counters
                    continue;
                }

                // To support populating servers for Azure Counters
                if (azureOnly && server.Instance.CloudProviderId != Common.Constants.MicrosoftAzureId && server.Instance.CloudProviderId !=
                                  Common.Constants.MicrosoftAzureManagedInstanceId)
                {
                    continue;
                }

                serverNames.Add(server.InstanceName,WrappedSqlServerObject.Wrap(server));
            }
            if (serverNames.Count > 0)
            {   // order the list of keys 
                string[] keys = Algorithms.ToArray(serverNames.Keys);
                Array.Sort(keys);
                foreach (string key in keys)
                {
                    comboBox.Items.Add(serverNames[key]);
                }
            }

            if (!string.IsNullOrEmpty(azureServerId))
            {
                var activeServer =
                    ApplicationModel.Default.ActiveInstances.FirstOrDefault(x =>
                        x.Id.Equals(Convert.ToInt32(azureServerId)));
                if (activeServer != null)
                {
                    var serverKey = activeServer.InstanceName;
                    var i = comboBox.Items.IndexOf(serverNames[serverKey]);
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
            if (serverNames.ContainsKey(currentText))
            {
                comboBox.SelectedItem = serverNames[currentText];
            }
            else
            {   // make the first item < select a server > and select it
                SelectSelectAServerItem(comboBox, true);
            }
        }

        private void SelectSelectAServerItem(ComboBox comboBox, bool selectSpecialRow)
        {
            if (comboBox != null)
            {
                WrappedSqlServerObject wrapper = comboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper == null || selectSpecialRow)
                {
                    int i = comboBox.Items.IndexOf(SELECT_A_SERVER);
                    if (i == -1)
                    {
                        wrapper = WrappedSqlServerObject.Wrap(SELECT_A_SERVER);
                        comboBox.Items.Insert(0, wrapper);
                        comboBox.SelectedIndex = 0;
                    }
                    else
                        comboBox.SelectedIndex = i;
                }
            }
        }
        private void SelectSelectedItem(ComboBox comboBox, bool selectSpecialRow)
        {
            if (comboBox != null)
            {
                WrappedSqlServerObject wrapper = comboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper == null || selectSpecialRow)
                {
                    int i = comboBox.Items.IndexOf("ABCD");
                    if (i == -1)
                    {
                        wrapper = WrappedSqlServerObject.Wrap(SELECT_A_SERVER);
                        comboBox.Items.Insert(0, wrapper);
                        comboBox.SelectedIndex = 0;
                    }
                    else
                        comboBox.SelectedIndex = i;
                }
            }
        }

        private void InitWmiObjectPickListTable()
        {
            wmiObjectsDataTable.Clear();
            DataRow row = wmiObjectsDataTable.NewRow();
            row[0] = SELECT_AN_OBJECT;
            row[1] = "";
            row[2] = SELECT_AN_OBJECT;
            row[3] = true;
            row[SPECIAL_COLUMN] = true;
            wmiObjectsDataTable.Rows.Add(row);

            InitWmiCounterPickListTable();
            InitWmiInstancePickListTable();
        }

        private void InitWmiInstancePickListTable()
        {
            wmiCountersDataTable.Clear();
            DataRow row = wmiCountersDataTable.NewRow();
            row[0] = SELECT_A_COUNTER;
            row[1] = "";
            row[2] = SELECT_A_COUNTER;
            row[SPECIAL_COLUMN] = true;
            wmiCountersDataTable.Rows.Add(row);
        }

        private void InitWmiCounterPickListTable()
        {
            wmiInstancesDataTable.Clear();
            DataRow row = wmiInstancesDataTable.NewRow();
            row[0] = SELECT_AN_INSTANCE;
            row[1] = SELECT_AN_INSTANCE;
            row[SPECIAL_COLUMN] = true;
            wmiInstancesDataTable.Rows.Add(row);
        }

        private void wmiServerPickList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && currentWmiServerName != comboBox.Text)
            {
                currentWmiServerName = comboBox.Text;
                WrappedSqlServerObject wrapper = wmiServerComboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper != null)
                {
                    MonitoredSqlServerWrapper instance = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    //DM 10.3 (Manali H): SQLDM-28744 fix: supressed the call for Linux instance as WMI is windows specific.
                    if (instance != null && 
                        instance.Instance != null &&
                        instance.Instance.CloudProviderId != Idera.SQLdm.Common.Constants.LinuxId)
                    {
                        DisableWmiComboBoxes();
                        InitWmiObjectPickListTable();
                        wmiObjectNameComboBox.SelectedIndex = 0;

//                        wmiStatusLabel.Text = String.Empty;
//                        wmiStatusLabel.Visible = true;
//                        wmiLoadingCircle.Visible = true;
//                        wmiLoadingCircle.Active = true;

                        StartBackgroundWorker(wmiBackgroundWorker_GetWmiObjectNames,
                                               wmiBackgroundWorker_GetWmiObjectNamesComplete,
                                               backgroundWorker_WmiProgress,
                                               instance);
                        return;
                    }
                } 
                SelectSelectAServerItem(sender as ComboBox, false);
            }
        }



        private void serverPickList_DropDown(object sender, EventArgs e)
        {
            // make sure the select a server node does not exist in the combo
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int i = comboBox.Items.IndexOf(SELECT_A_SERVER);
                if (i >= 0)
                {
                    comboBox.Items.RemoveAt(i);
                }
            }
        }

        #region Server ComboBox Wrapper Object

        private class WrappedSqlServerObject
        {
            private object wrappedObject;

            public object WrappedObject
            {
                get { return wrappedObject; }
                set { wrappedObject = value; }
            }

            public string MachineName
            {
                get
                {
                    if (wrappedObject is MonitoredSqlServerWrapper)
                        return ((MonitoredSqlServerWrapper)wrappedObject).MachineName;

                    return ToString();
                }
            }

            public override int GetHashCode()
            {
                return wrappedObject.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                string objCompareObject = obj.ToString();
                if (obj is WrappedSqlServerObject)
                {
                    obj = ((WrappedSqlServerObject)obj).wrappedObject;
                    objCompareObject = obj.ToString();
                    if (obj is MonitoredSqlServerWrapper)
                        objCompareObject = ((MonitoredSqlServerWrapper)obj).InstanceName;
                }
                string thisCompareObject = wrappedObject.ToString();
                if (obj is MonitoredSqlServerWrapper)
                    thisCompareObject = ((MonitoredSqlServerWrapper)wrappedObject).InstanceName;

                return thisCompareObject.Equals(objCompareObject);
            }

            public override string ToString()
            {
                return wrappedObject.ToString();
            }

            public static WrappedSqlServerObject Wrap(MonitoredSqlServerWrapper mssw)
            {
                WrappedSqlServerObject wrapper = new WrappedSqlServerObject();
                wrapper.WrappedObject = mssw;
                return wrapper;
            }

            public static WrappedSqlServerObject Wrap(string s)
            {
                WrappedSqlServerObject wrapper = new WrappedSqlServerObject();
                wrapper.WrappedObject = s;
                return wrapper;
            }
        }
        #endregion

        #endregion

        #region WMI Object Name Selection

        private void wmiObjectNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && currentWmiObjectName != comboBox.Text)
            {
                currentWmiObjectName = comboBox.Text;
                DataRowView rowView = comboBox.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    string objectName = rowView[0] as string;
                    if (objectName == null || IsSpecialRow(rowView))
                    {
                        wmiObjectNameTextBox.Text = String.Empty;
                        EnableWmiTestButton();
                        return;
                    }
                    wmiObjectNameTextBox.Text = objectName; 

                    WrappedSqlServerObject wrapper = wmiServerComboBox.SelectedItem as WrappedSqlServerObject;
                    MonitoredSqlServerWrapper server = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    bool singleton = (bool) rowView[3];

                    DisableWmiComboBoxes();

                    InitWmiCounterPickListTable();
                    InitWmiInstancePickListTable();
                    wmiInstanceNameComboBox.Visible = !(bool)rowView[3];

                    Triple<MonitoredSqlServerWrapper,string,bool> parm = new Triple<MonitoredSqlServerWrapper,string,bool>(server, objectName, singleton);
                    StartBackgroundWorker(wmiBackgroundWorker_GetWmiCountersAndInstances,
                                           wmiBackgroundWorker_GetWmiCountersAndInstanceComplete, 
                                           backgroundWorker_WmiProgress,
                                           parm);
                } else
                    SelectSpecialRow(sender as ComboBox, SELECT_AN_OBJECT);
            }
            EnableWmiTestButton();
        }

        private void DisableWmiComboBoxes()
        {
            wmiServerComboBox.Enabled = false;
            wmiBrowseModeButton.Enabled = false;
            wmiManualModeButton.Enabled = false;
            wmiObjectNameComboBox.Enabled = false;
            wmiCounterNameComboBox.Enabled = false;
            wmiInstanceNameComboBox.Enabled = false;
            wmiInfoButton.Enabled = false;
        }

        private void EnableWmiComboBoxes(bool setCounterComboBox, bool setInstancesComboBox)
        {
            WrappedSqlServerObject selectedServer = wmiServerComboBox.SelectedItem as WrappedSqlServerObject;
            wmiServerComboBox.Enabled = true;
            wmiBrowseModeButton.Enabled = true;
            wmiManualModeButton.Enabled = true;
            if (selectedServer != null && selectedServer.WrappedObject is MonitoredSqlServerWrapper) 
            {
                wmiObjectNameComboBox.Enabled = true;
                DataRowView objectRow = wmiObjectNameComboBox.SelectedItem as DataRowView;
                bool objectSelected = !IsSpecialRow(objectRow);
                wmiCounterNameComboBox.Enabled = objectSelected;
                if (setCounterComboBox)
                    SetComboSelection(wmiCounterNameComboBox, SELECT_A_COUNTER);  
                wmiInstanceNameComboBox.Enabled = objectSelected;
                if (setInstancesComboBox)
                    SetComboSelection(wmiInstanceNameComboBox, SELECT_AN_INSTANCE);
                wmiInfoButton.Enabled = objectSelected;
            }
            EnableWmiTestButton();
        }

        private void EnableWmiTestButton()
        {
            wmiCounterPage.AllowMoveNext = IsWmiCounterPageComplete;
        }

        private void SetComboSelection(ComboBox comboBox, string specialText)
        {
            if (comboBox.Items.Count == 2 && comboBox.SelectedIndex == 0)
            {
                DataRowView row = (DataRowView)comboBox.Items[0];
                if (IsSpecialRow(row))
                    row.DataView.Table.Rows.Remove(row.Row);
            }

            if (comboBox.Items.Count == 1)
            {
                comboBox.SelectedIndex = 0;
            }
            else
                SelectSpecialRow(comboBox, specialText);

            UpdateTextBox(comboBox);
        }

        private void UpdateTextBox(ComboBox comboBox)
        {
            if (comboBox != null)
            {
                string text = String.Empty;
                TextBox textBox = comboBox.Tag as TextBox;
                if (textBox != null)
                {
                    DataRowView selectedItem = comboBox.SelectedItem as DataRowView;
                    if (selectedItem != null && !IsSpecialRow(selectedItem))
                    {
                        text = selectedItem[0] as string;
                    }
                    textBox.Text = text;
                }
            }
        }

        public bool IsSpecialRow(DataRowView row)
        {
            if (row != null && row[0] is string)
            {
                if (((string)row[0]).StartsWith("< Select "))
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleWmiException(Exception exception, string firstMessage)
        {
            int hresult = GetLowestLevelHResult(exception);
            string message = null;
            switch (hresult)
            {
                case -2147023174: // 0x800706BA RPC server could not be contacted
                    message = "The RPC Server on the remote system could not be contacted.  This error usually indicates that the RPC traffic is not getting to the remote server, or there is no RPC listener available on the remote server.";
                    break;
                case -2147024891: // 0x80070005 DCOM Access Denied
                    message = "The DCOM Server on the remote system rejected the request because of insufficient privileges.  This error is usually caused by the 'SQLDM Collection Service' service account not being a member of the local Administrators group on the target system.";
                    break;
                case -2147217405: // 0x80041003 WMI Access Denied
                    message = "The Windows Management Server on the remote system rejected the request because of insufficient privileges.  This error is usually caused by the 'SQLDM Collection Service' service account not being a member of the local Administrators group on the target system.";
                    break;
                default:
                    LOG.Error(firstMessage, exception);
                    ApplicationMessageBox.ShowError(this, firstMessage, exception);
                    return;
            }

            ApplicationMessageBox messageBox = new ApplicationMessageBox();
            messageBox.Caption = this.Text;
            messageBox.Message = new Exception(String.Format("{0}.  {1}", firstMessage, message), exception);
            messageBox.Symbol = ExceptionMessageBoxSymbol.Error;
            messageBox.Buttons = ExceptionMessageBoxButtons.Custom;
            messageBox.DefaultButton = ExceptionMessageBoxDefaultButton.Button2;
            messageBox.Beep = true;
            messageBox.SetButtonText("Help", "OK");
            messageBox.Show(this);
            if (messageBox.CustomDialogResult == ExceptionMessageBoxDialogResult.Button1)
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.WmiConnectionError);
            }
        }

        private void HandleVmException(Exception exception, string firstMessage)
        {
            int hresult = GetLowestLevelHResult(exception);
            string message = null;
            switch (hresult)
            {
                case -2147023174: // 0x800706BA RPC server could not be contacted
                    message = "The RPC Server on the remote system could not be contacted.  This error usually indicates that the RPC traffic is not getting to the remote server, or there is no RPC listener available on the remote server.";
                    break;
                case -2147024891: // 0x80070005 DCOM Access Denied
                    message = "The DCOM Server on the remote system rejected the request because of insufficient privileges.  This error is usually caused by the 'SQLDM Collection Service' service account not being a member of the local Administrators group on the target system.";
                    break;
                case -2147217405: // 0x80041003 WMI Access Denied
                    message = "The Windows Management Server on the remote system rejected the request because of insufficient privileges.  This error is usually caused by the 'SQLDM Collection Service' service account not being a member of the local Administrators group on the target system.";
                    break;
                default:
                    LOG.Error(firstMessage, exception);
                    ApplicationMessageBox.ShowError(this, firstMessage, exception);
                    return;
            }

            ApplicationMessageBox messageBox = new ApplicationMessageBox();
            messageBox.Caption = this.Text;
            messageBox.Message = new Exception(String.Format("{0}.  {1}", firstMessage, message), exception);
            messageBox.Symbol = ExceptionMessageBoxSymbol.Error;
            messageBox.Buttons = ExceptionMessageBoxButtons.Custom;
            messageBox.DefaultButton = ExceptionMessageBoxDefaultButton.Button2;
            messageBox.Beep = true;
            messageBox.SetButtonText("Help", "OK");
            messageBox.Show(this);
            if (messageBox.CustomDialogResult == ExceptionMessageBoxDialogResult.Button1)
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.WmiConnectionError);
            }
        }

        public void azureBackgroundWorker_GetDatabaseDetails(object sender, DoWorkEventArgs args)
        {
            if (args.Cancel)
                return;

            var param = args.Argument as MonitoredSqlServerWrapper;
            
            var worker = (BackgroundWorkerWithProgressDialog)sender;
            //            worker.ExternalTaskLabel = wmiStatusLabel;
            worker.SetStatusText("Retrieving list of objects...");

            // servername.dnsZone.database.windows.net
            DataTable result = managementService.GetAzureDatabases(param.Id);
            args.Result = result;

            worker.SetStatusText("Done.");
        }

        public void wmiBackgroundWorker_GetWmiObjectNames(object sender, DoWorkEventArgs args)
        {
            if (args.Cancel)
                return;

            MonitoredSqlServerWrapper parm = args.Argument as MonitoredSqlServerWrapper;

            Pair<string,DataTable> result;
            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog) sender;
//            worker.ExternalTaskLabel = wmiStatusLabel;
            worker.SetStatusText("Retrieving list of objects...");

            result = managementService.GetWmiObjectList(parm.Id, null);
            if (result != null && !String.IsNullOrEmpty(result.First))
                parm.MachineName = result.First;

            args.Result = result.Second;

            worker.SetStatusText("Done.");
        }

        public void wmiBackgroundWorker_GetWmiObjectNamesComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            bool cancelledOrError = false;
            bool rowReselected = false;
            try
            {
                if (args.Cancelled)
                {
                    // reset server selection
                    cancelledOrError = true;
                    return;
                }

                if (args.Error != null)
                {
                    string firstMessage = "Error retrieving list of object names";
                    HandleWmiException(args.Error, firstMessage);
                    cancelledOrError = true;
                    return;
                }
                DataTable result = args.Result as DataTable;
                if (result != null)
                {
                    wmiObjectsDataTable.Merge(result);

                    if (wmiShowPerfCountersOnlyButton.Checked)
                    {
                        // we got data but no rows are perf counters
                        if (wmiObjectNameComboBox.Items.Count <= 1 && result.Rows.Count > 0)
                        {
                            string message = String.Format(
                                "No Windows System Performance Monitor (Perfmon) counters were returned for {0}. Please note that Perfmon counters will not be returned from Windows 2000 servers because this platform does not provide information about these counters; however, you can view Windows Management Instrumentation (WMI) counters on these servers.  Would you like to show WMI counters for this server now?",
                                wmiServerComboBox.Text.TrimEnd());

                            ApplicationMessageBox amb = new ApplicationMessageBox();
                            amb.HelpLink = "";
                            if (
                                ApplicationMessageBox.ShowQuestion(this, message,
                                                                   Microsoft.SqlServer.MessageBox.
                                                                       ExceptionMessageBoxButtons.YesNo) ==
                                DialogResult.Yes)
                            {
                                // switch to show all counters mode
                                wmiShowAllCountersButton.Checked = true;
                            }
                            
                        }
                    }
                    // see if something was entered when in text mode
                    string objectText = wmiObjectNameTextBox.Text.TrimEnd();
                    if (objectText.Length != 0)
                    {
                        // attempt to select the matching data table row
                        rowReselected =
                            !ReselectRow(wmiObjectNameComboBox, objectText, SELECT_AN_OBJECT).Equals(
                                 SELECT_AN_OBJECT);
                        if (rowReselected)
                        {
                            currentWmiObjectName = String.Empty;
                            wmiObjectNameComboBox_SelectionChangeCommitted(wmiObjectNameComboBox, EventArgs.Empty);
                        }
                    }
                }
            }
            finally
            {
                wmiStatusLabel.Text = String.Empty;
                if (!rowReselected)
                {
                    EnableWmiComboBoxes(true, true);

                    // if there was an error then disable the object name box
                    if (cancelledOrError)
                    {
                        wmiObjectNameComboBox.Enabled = false;
                        currentWmiServerName = String.Empty;
                        SelectSelectAServerItem(wmiServerComboBox, true);
                    }
                }
            }
        }

        private int GetLowestLevelHResult(Exception exception)
        {
            int result = 0;
            Exception inner = exception;
            while(inner.InnerException != null && inner.InnerException != inner)
            {
                inner = inner.InnerException;
            }
            if (inner is ExternalException)
                result = ((ExternalException) inner).ErrorCode;
            else
            if (inner is UnauthorizedAccessException)
                result = -2147024891;
            else
            if (inner is ManagementException)
                result = (int)((ManagementException)inner).ErrorCode;
            return result;
        }

        private void backgroundWorker_WmiProgress(object sender, ProgressChangedEventArgs args)
        {
            wmiStatusLabel.Text = args.UserState.ToString();   
        }

        private void backgroundWorker_SqlProgress(object sender, ProgressChangedEventArgs args)
        {
            sqlStatusLabel.Text = args.UserState.ToString();
        }

        private void backgroundWorker_VmProgress(object sender, ProgressChangedEventArgs args)
        {
            vmStatusLabel.Text = args.UserState.ToString();
        }

        public void wmiBackgroundWorker_GetWmiCountersAndInstances(object sender, DoWorkEventArgs args)
        {
            Pair<DataTable, DataTable> dataPair = new Pair<DataTable, DataTable>();
            Exception firstException = null;

            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
 //           worker.ExternalTaskLabel = wmiStatusLabel;

            Triple<MonitoredSqlServerWrapper,string,bool> parm = (Triple<MonitoredSqlServerWrapper,string,bool>)args.Argument;

            if (args.Cancel)
                return;

            worker.SetStatusText("Retrieving list of counters...");

            try
            {
                dataPair.First = managementService.GetWmiCounterList(parm.First.Id, parm.First.MachineName, parm.Second, null);
            }
            catch (CollectionServiceException cse)
            {
                firstException = cse;
                // see if there is partial data
                if (cse.Data.Contains("DataTable"))
                {
                    object resultObject = cse.Data["DataTable"];
                    if (resultObject is Serialized<DataTable>)
                        dataPair.First = (Serialized<DataTable>) resultObject;
                    else
                        dataPair.First = resultObject as DataTable;
                }
            }
            if (!args.Cancel && !parm.Third)
            {
                // don't bother getting instance list for singleton objects
                worker.SetStatusText("Retrieving list of instances...");
                try
                {
                    dataPair.Second = managementService.GetWmiInstanceList(parm.First.Id, parm.First.MachineName, parm.Second, null);
                }
                catch (CollectionServiceException cse)
                {
                    if (firstException == null)
                        firstException = cse;

                    // see if there is partial data available
                    if (cse.Data.Contains("DataTable"))
                    {
                        object resultObject = cse.Data["DataTable"];
                        if (resultObject is Serialized<DataTable>)
                            dataPair.Second = (Serialized<DataTable>) resultObject;
                        else
                            dataPair.Second = resultObject as DataTable;
                    }
                }
            }
            // rethrow the first exception encountered
            if (firstException != null)
                worker.SetStatusText("Error.");
            else
                worker.SetStatusText("Done.");

            args.Result = new Triple<Exception,Pair<DataTable,DataTable>,Triple<MonitoredSqlServerWrapper,string,bool>>(firstException, dataPair, parm);
        }

        public void wmiBackgroundWorker_GetWmiCountersAndInstanceComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            bool cancelledOrError = false;
            bool resetCounterComboBox = true;
            bool resetInstanceComboBox = true;

            try
            {
                if (args.Cancelled)
                {
                    cancelledOrError = true;
                    return;
                }

                if (args.Error != null)
                {
                    // this is from an unhandled exception
                    HandleWmiException(args.Error, "Error retrieving list of counters and instances");
                    cancelledOrError = true;
                    return;
                }

                Triple<Exception,Pair<DataTable,DataTable>,Triple<MonitoredSqlServerWrapper,string,bool>> result =
                    (Triple<Exception,Pair<DataTable,DataTable>,Triple<MonitoredSqlServerWrapper,string,bool>>)args.Result;

                if (result.First != null)
                {
                    HandleWmiException(result.First, "Error retrieving list of counters and instances");
                    if (result.Second.First == null || result.Second.First.Rows.Count == 0 ||
                        result.Second.Second == null || result.Second.Second.Rows.Count == 0)
                    {
                        SelectSpecialRow(wmiObjectNameComboBox, SELECT_AN_OBJECT);
                        cancelledOrError = true;
                        return;
                    }
                }
                if (result.Second.First != null)
                {
                    wmiCountersDataTable.Merge(result.Second.First);
                }
                if (result.Second.Second != null)
                {
                    wmiInstancesDataTable.Merge(result.Second.Second);
                }
                string text = wmiCounterNameTextBox.Text.TrimEnd();
                if (text.Length > 0)
                {
                    ReselectRow(wmiCounterNameComboBox, text, SELECT_A_COUNTER);
                    resetCounterComboBox = false;
                }
                text = wmiInstanceNameTextBox.Text.TrimEnd();
                if (text.Length > 0)
                {
                    ReselectRow(wmiInstanceNameComboBox, text, SELECT_AN_INSTANCE);
                    resetInstanceComboBox = false;
                }
            }
            finally
            {
                EnableWmiComboBoxes(resetCounterComboBox, resetInstanceComboBox);

                if (cancelledOrError)
                {
                    currentWmiObjectName = String.Empty;
                    wmiCounterNameComboBox.Enabled = false;
                    wmiInstanceNameComboBox.Enabled = false;
                    SelectSpecialRow(wmiObjectNameComboBox, SELECT_AN_OBJECT);
                }
            }
        }

        public void backgroundWorker_GetVmObjectNames(object sender, DoWorkEventArgs args)
        {
            int parm = (int)args.Argument;
            if (args.Cancel)
                return;

            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
            worker.SetStatusText("Retrieving list of objects...");

            DataTable result =  managementService.GetVmCounterObjectList(parm);

            args.Result = result;

            worker.SetStatusText("Done.");
        }

        public void backgroundWorker_GetVmObjectNamesComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            bool cancelledOrError = false;
            bool rowReselected = false;
            try
            {
                if (args.Cancelled)
                {
                    // reset server selection
                    cancelledOrError = true;
                    return;
                }

                if (args.Error != null)
                {
                    string firstMessage = "Error retrieving list of object names";
                    HandleVmException(args.Error, firstMessage);
                    cancelledOrError = true;
                    return;
                }

                DataTable result = args.Result as DataTable;
                if (vmObjectsDataTable.ExtendedProperties.ContainsKey("Source"))
                    vmObjectsDataTable.ExtendedProperties.Remove("Source");

                if (result != null)
                {
                    vmObjectsDataTable.ExtendedProperties.Add("Source", result);
                    Set<string> sourceSet = new Set<string>();
                    DataTable subset = vmObjectsDataTable.Clone();

                    foreach (DataRow row in result.Rows)
                    {
                        System.Diagnostics.Debug.Print("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                            row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]);
                        
                        string source = (string)row["Source"];
                        string group = (string) row["Group"];
                        string groupLabel = (string) row["GroupLabel"] ?? group;

                        string objectkey = source + "." + group;
                        if (sourceSet.Contains(objectkey)) continue;
                        sourceSet.Add(objectkey);

                        DataRow newRow = subset.NewRow();
                        newRow["Name"] = objectkey;
                        newRow["Description"] = (string) row["GroupSummary"];
                        newRow["DisplayName"] = String.Format("{0} - {1}", source, groupLabel);
                        newRow[SPECIAL_COLUMN] = false;
                        newRow["Source"] = source;
                        newRow["Group"] = group;
                        subset.Rows.Add(newRow);
                    }

                    vmObjectsDataTable.Merge(subset);

                    // see if something was entered when in text mode
                    string objectText = vmObjectNameTextBox.Text.TrimEnd();
                    if (objectText.Length != 0)
                    {
                        // attempt to select the matching data table row
                        rowReselected = !ReselectRow(vmObjectNameComboBox, objectText, SELECT_AN_OBJECT).Equals(SELECT_AN_OBJECT);
                        if (rowReselected)
                        {
                            currentVmObjectName = String.Empty;
                            vmObjectNameComboBox_SelectionChangeCommitted(vmObjectNameComboBox, EventArgs.Empty);
                        }
                    }
                }
            }
            finally
            {
                vmStatusLabel.Text = String.Empty;
                if (!rowReselected)
                {
                    EnableVmComboBoxes(false, false);

                    // if there was an error then disable the object name box
                    if (cancelledOrError)
                    {
                        vmObjectNameComboBox.Enabled = false;
                        currentVmServerName = String.Empty;
                        SelectSelectAServerItem(vmServerComboBox, true);
                    }
                }
            }
        }

        private void StartBackgroundWorker(DoWorkEventHandler doWorkHandler, RunWorkerCompletedEventHandler workCompleteHandler, ProgressChangedEventHandler progressHandler, object argument)
        {
            if (backgroundWorker != null && backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();

            backgroundWorker = new BackgroundWorkerWithProgressDialog(this);
            backgroundWorker.Delay = 2000;
            backgroundWorker.DoWork += doWorkHandler;
            backgroundWorker.RunWorkerCompleted += workCompleteHandler;
            backgroundWorker.ProgressChanged += progressHandler;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync(argument);
        }

        private void SelectSpecialRow(ComboBox comboBox, string specialText)
        {
            if (comboBox != null)
            {
                DataView dataView = (DataView) comboBox.DataSource;
                DataTable dataTable = dataView.Table;
                object r1c1 = null;
                if (dataTable.Rows.Count > 0)
                {
                    r1c1 = dataTable.Rows[0][0];
                }
                if (r1c1 == null || !r1c1.Equals(specialText))
                {
                    DataRow row = dataTable.NewRow();
                    if (dataTable == wmiObjectsDataTable)
                    {
                        row[0] = specialText;
                        row[1] = "";
                        row[2] = specialText;
                        row[3] = true;
                        row[SPECIAL_COLUMN] = true;
                    }
                    else if (dataTable == wmiCountersDataTable)
                    {
                        row[0] = specialText;
                        row[1] = "";
                        row[2] = specialText;
                        row[SPECIAL_COLUMN] = true;
                    }
                    else if (dataTable == wmiInstancesDataTable)
                    {
                        row[0] = specialText;
                        row[1] = specialText;
                        row[SPECIAL_COLUMN] = true;
                    }
                    if (dataTable == sqlObjectsDataTable)
                    {
                        row[0] = specialText;
                        row[1] = true;
                        row[SPECIAL_COLUMN] = true;
                    }
                    else if (dataTable == sqlCountersDataTable)
                    {
                        row[0] = specialText;
                        row[SPECIAL_COLUMN] = true;
                    }
                    else if (dataTable == sqlInstancesDataTable)
                    {
                        row[0] = specialText;
                        row[SPECIAL_COLUMN] = true;
                    }
                    else
                        return;

                    dataTable.Rows.InsertAt(row, 0);
                }
                // select the first row
                comboBox.SelectedIndex = 0;
            }
        }

        private void wmiObjectNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = wmiObjectNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sender as ComboBox, SELECT_AN_OBJECT);
            }
        }

        private void RemoveSpecialRow(ComboBox comboBox)
        {
            if (comboBox != null)
            {
                DataView dataView = (DataView)comboBox.DataSource;
                DataTable dataTable = dataView.Table;
                if (dataTable.Rows.Count > 0)
                {
                    if (comboBox.SelectedIndex != 0)
                    {
                        object r1c1 = dataTable.Rows[0][0];
                        if (r1c1 is string && ((string)r1c1).StartsWith("< Select "))
                            dataTable.Rows.RemoveAt(0);
                    }
                }
            }
        }

        private void wmiObjectNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        #endregion

        private void wmiCounterNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTextBox(sender as ComboBox);
            EnableWmiTestButton();
        }

        private void wmiCounterNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void wmiCounterNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = wmiObjectNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sender as ComboBox, SELECT_A_COUNTER);
            }
        }

        private void wmiInstanceNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = wmiInstanceNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sender as ComboBox, SELECT_AN_INSTANCE);
            }
        }

        private void wmiInstanceNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void wmiInstanceNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTextBox(sender as ComboBox);
            EnableWmiTestButton();
        }

        private void wmiObjectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (wmiManualModeButton.Checked)
            {
                EnableWmiTestButton();
            }
        }

        private void wmiCounterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (wmiManualModeButton.Checked)
            {
                EnableWmiTestButton();
            }
        }

        private void wmiInstanceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (wmiManualModeButton.Checked)
            {
                EnableWmiTestButton();
            }
        }
        


        private void vmServerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && currentSqlServerName != comboBox.Text)
            {
                currentSqlServerName = comboBox.Text;
                WrappedSqlServerObject wrapper = comboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper != null)
                {
                    MonitoredSqlServerWrapper instance = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    if (instance != null)
                    {
                        DisableVmComboBoxes();
                        InitVmObjectPickListTable();
                        vmObjectNameComboBox.SelectedIndex = 0;

                        //                        sqlLoadingCircle.Visible = true;
                        //                        sqlLoadingCircle.Active = true;

                        StartBackgroundWorker(backgroundWorker_GetVmObjectNames,
                                              backgroundWorker_GetVmObjectNamesComplete,
                                              backgroundWorker_VmProgress,
                                              instance.Id);
                        return;
                    }
                }
                SelectSelectAServerItem(sender as ComboBox, false);
            }

        }


        public void SetSqlSelection()
        {
            string selectedObject = SELECT_AN_OBJECT;
            string selectedCounter = SELECT_A_COUNTER;
            string selectedInstance = SELECT_AN_INSTANCE;

            if (sqlBrowseModeButton.Checked)
            {
                selectedObject = sqlObjectNameTextBox.Text.TrimEnd();
                selectedCounter = sqlCounterNameTextBox.Text.TrimEnd();
                selectedInstance = sqlInstanceNameTextBox.Text.TrimEnd();
            }
            else
            {
                selectedObject = sqlObjectNameComboBox.Text;
                selectedCounter = sqlCounterNameComboBox.Text;
                selectedInstance = sqlInstanceNameComboBox.Text;
            }

            if (selectedObject != null)
            {
                string selectedValue = ReselectRow(wmiObjectNameComboBox, selectedObject, SELECT_AN_OBJECT);
                if (selectedValue == null || selectedValue.Equals(SELECT_AN_OBJECT))
                {
                    InitWmiCounterPickListTable();
                    wmiCounterNameComboBox.Enabled = false;
                    InitWmiInstancePickListTable();
                    wmiInstanceNameComboBox.Enabled = false;
                }
                if (selectedCounter != null)
                    ReselectRow(wmiCounterNameComboBox, selectedCounter, SELECT_A_COUNTER);
                if (selectedInstance != null)
                    ReselectRow(wmiInstanceNameComboBox, selectedInstance, SELECT_AN_INSTANCE);
            }

            EnableSqlTestButton();
        }

        private void sqlBrowseModeButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSqlCounterPageControls();
        }

        private void vmBrowseModeButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVmCounterPageControls();
        }

        private void UpdateSqlCounterPageControls()
        {
            bool browseMode = sqlBrowseModeButton.Checked;
            sqlBrowseModeContentPanel.Visible = browseMode;
            sqlManualModeContentPanel.Visible = !browseMode;
        }

        private void sqlServerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && currentSqlServerName != comboBox.Text)
            {
                currentSqlServerName = comboBox.Text;
                WrappedSqlServerObject wrapper = comboBox.SelectedItem as WrappedSqlServerObject;
                if (wrapper != null)
                {
                    MonitoredSqlServerWrapper instance = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    if (instance != null)
                    {
                        DisableSqlComboBoxes();
                        InitSqlObjectPickListTable();
                        sqlObjectNameComboBox.SelectedIndex = 0;

//                        sqlLoadingCircle.Visible = true;
//                        sqlLoadingCircle.Active = true;

                        StartBackgroundWorker(backgroundWorker_GetSysPerfObjectNames,
                                               backgroundWorker_GetSysPerfObjectNamesComplete,
                                               backgroundWorker_SqlProgress,
                                               instance.Id);
                        return;
                    }
                }
                SelectSelectAServerItem(sender as ComboBox, false);
            }

        }

        private void InitSqlObjectPickListTable()
        {
            sqlObjectsDataTable.Clear();
            DataRow row = sqlObjectsDataTable.NewRow();
            row[0] = SELECT_AN_OBJECT;
            row[1] = true;
            row[SPECIAL_COLUMN] = true;
            sqlObjectsDataTable.Rows.Add(row);

            InitSqlCounterPickListTable();
            InitSqlInstancePickListTable();
        }

        private void InitSqlInstancePickListTable()
        {
            sqlInstancesDataTable.Clear();
            DataRow row = sqlInstancesDataTable.NewRow();
            row[0] = SELECT_AN_INSTANCE;
            row[SPECIAL_COLUMN] = true;
            sqlInstancesDataTable.Rows.Add(row);
        }

        private void InitSqlCounterPickListTable()
        {
            sqlCountersDataTable.Clear();
            DataRow row = sqlCountersDataTable.NewRow();
            row[0] = SELECT_A_COUNTER;
            row[SPECIAL_COLUMN] = true;
            sqlCountersDataTable.Rows.Add(row);
        }


        private void InitVmObjectPickListTable()
        {
            vmObjectsDataTable.Clear();
            DataRow row = vmObjectsDataTable.NewRow();
            row[0] = SELECT_AN_OBJECT;
            row[1] = "";
            row[2] = SELECT_AN_OBJECT;
            row[SPECIAL_COLUMN] = true;
            vmObjectsDataTable.Rows.Add(row);

            InitVmInstancePickListTable();
        }

        private void InitVmInstancePickListTable()
        {
            vmCountersDataTable.Clear();
            DataRow row = vmCountersDataTable.NewRow();
            row[0] = SELECT_A_COUNTER;
            row[1] = "";
            row[2] = SELECT_A_COUNTER;
            row[SPECIAL_COLUMN] = true;
            row[SINGLETON_COLUMN] = true;
            vmCountersDataTable.Rows.Add(row);

            InitVmCounterPickListTable();
        }

        private void InitVmCounterPickListTable()
        {
            vmInstancesDataTable.Clear();
            DataRow row = vmInstancesDataTable.NewRow();
            row[0] = SELECT_AN_INSTANCE;
            row[1] = SELECT_AN_INSTANCE;
            row[SPECIAL_COLUMN] = true;
            vmInstancesDataTable.Rows.Add(row);
        }

        public void backgroundWorker_GetSysPerfObjectNames(object sender, DoWorkEventArgs args)
        {
            int parm = (int)args.Argument;
            if (args.Cancel)
                return;

            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
            worker.SetStatusText("Retrieving list of objects...");

            DataTable result = managementService.GetSysPerfInfoObjectList(parm);
            args.Result = result;

            worker.SetStatusText("Done.");
        }

        public void backgroundWorker_GetSysPerfObjectNamesComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            bool cancelledOrError = false;
            bool rowReselected = false;
            try
            {
                if (args.Cancelled)
                {
                    cancelledOrError = true;
                    return;
                }
                if (args.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, "Error retrieving list of object names: ", args.Error);
                    cancelledOrError = true;
                    return;
                }
                DataTable result = args.Result as DataTable;
                if (result != null)
                {
                    sqlObjectsDataTable.Merge(result);
                    string objectText = sqlObjectNameTextBox.Text.TrimEnd();
                    if (objectText.Length != 0)
                    {
                        rowReselected =
                            ReselectRow(sqlObjectNameComboBox, objectText, SELECT_AN_OBJECT).Equals(objectText);
                        if (rowReselected)
                        {
                            // kick off process to retrieve the sql counters and instances for the selected object
                            currentSqlObjectName = String.Empty;
                            sqlObjectNameComboBox_SelectionChangeCommitted(sqlObjectNameComboBox, EventArgs.Empty);
                        }
                    }
                }
            }
            finally
            {
                if (!rowReselected)
                {
                    EnableSqlComboBoxes(true, true);
                }
                if (cancelledOrError)
                {
                    currentSqlServerName = String.Empty;
                    sqlObjectNameComboBox.Enabled = false;
                    SelectSelectAServerItem(sqlServerComboBox, true);
                }
            }
        }

        private void sqlObjectNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void sqlCounterNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void sqlInstanceNameComboBox_DropDown(object sender, EventArgs args)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void sqlObjectNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = sqlObjectNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sqlObjectNameComboBox, SELECT_AN_OBJECT);
            }
        }

        private void sqlCounterNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = sqlCounterNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sqlCounterNameComboBox, SELECT_A_COUNTER);
            }
        }

        private void sqlInstanceNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = sqlInstanceNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sqlInstanceNameComboBox, SELECT_AN_INSTANCE);
            }
        }

        private void sqlObjectNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && currentSqlObjectName != comboBox.Text)
            {
                currentSqlObjectName = comboBox.Text;
                DataRowView rowView = comboBox.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    string objectName = rowView[0] as string;
                    if (objectName == null || IsSpecialRow(rowView))
                    {
                        sqlObjectNameTextBox.Text = String.Empty;
                        EnableSqlTestButton();
                        return;
                    }
                    sqlObjectNameTextBox.Text = objectName;

                    WrappedSqlServerObject wrapper = sqlServerComboBox.SelectedItem as WrappedSqlServerObject;
                    MonitoredSqlServerWrapper server = wrapper.WrappedObject as MonitoredSqlServerWrapper;
                    bool singleton = (bool)rowView[SINGLETON_COLUMN];

                    DisableSqlComboBoxes();

                    InitSqlCounterPickListTable();
                    InitSqlInstancePickListTable();
                    sqlInstanceNameComboBox.Visible = !singleton;

//                    sqlLoadingCircle.Visible = true;
//                    sqlLoadingCircle.Active = true;

                    Triple<int, string, bool> parm = new Triple<int, string, bool>(server.Id, objectName, singleton);
                    StartBackgroundWorker(backgroundWorker_GetSysPerfCountersAndInstances,
                                           backgroundWorker_GetSysPerfCountersAndInstanceComplete,
                                           backgroundWorker_SqlProgress,
                                           parm);
                }
                else
                    SelectSpecialRow(sender as ComboBox, SELECT_AN_OBJECT);
            }
            EnableSqlTestButton();
        }

        private void EnableSqlTestButton()
        {
            sqlCounterPage.AllowMoveNext = IsSqlCounterPageComplete;
        }

        private void EnableSqlComboBoxes(bool setCounterComboBox, bool setInstanceComboBox)
        {
            WrappedSqlServerObject selectedServer = sqlServerComboBox.SelectedItem as WrappedSqlServerObject;
            sqlServerComboBox.Enabled = true;
            sqlBrowseModeButton.Enabled = true;
            sqlManualModeButton.Enabled = true;
            if (selectedServer != null && selectedServer.WrappedObject is MonitoredSqlServerWrapper)
            {
                sqlObjectNameComboBox.Enabled = true;
                DataRowView objectRow = sqlObjectNameComboBox.SelectedItem as DataRowView;
                bool objectSelected = !IsSpecialRow(objectRow);
                sqlCounterNameComboBox.Enabled = objectSelected;
                if (setCounterComboBox)
                    SetComboSelection(sqlCounterNameComboBox, SELECT_A_COUNTER);
                sqlInstanceNameComboBox.Enabled = objectSelected;
                if (setInstanceComboBox)
                    SetComboSelection(sqlInstanceNameComboBox, SELECT_AN_INSTANCE);
            }
            EnableSqlTestButton();
        }

        private void DisableSqlComboBoxes()
        {
            sqlServerComboBox.Enabled = false;
            sqlBrowseModeButton.Enabled = false;
            sqlManualModeButton.Enabled = false;
            sqlObjectNameComboBox.Enabled = false;
            sqlCounterNameComboBox.Enabled = false;
            sqlInstanceNameComboBox.Enabled = false;
        }

        private void DisableVmComboBoxes()
        {
            vmServerComboBox.Enabled = false;
            vmBrowseModeButton.Enabled = false;
            vmManualModeButton.Enabled = false;
            vmObjectNameComboBox.Enabled = false;
            vmCounterNameComboBox.Enabled = false;
            vmInstanceNameComboBox.Enabled = false;
        }

        public void backgroundWorker_GetSysPerfCountersAndInstances(object sender, DoWorkEventArgs args)
        {
            DataTable instanceNames = null;
            DataTable counterNames = null;

            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
            Triple<int, string, bool> parm = (Triple<int, string, bool>)args.Argument;
            try
            {
                if (args.Cancel)
                    return;

                worker.SetStatusText("Retrieving list of counters...");

                counterNames = managementService.GetSysPerfInfoCounterList(parm.First, parm.Second);
                if (!args.Cancel && !parm.Third)
                {
                    worker.SetStatusText("Retrieving list of instances...");
                    instanceNames = managementService.GetSysPerfInfoInstanceList(parm.First, parm.Second);
                }
            }
            catch
            {
                worker.SetStatusText("Error.");
                args.Result = new Triple<DataTable, DataTable, Triple<int, string, bool>>(counterNames, instanceNames, parm);
                throw;
            }

            worker.SetStatusText("Done.");
            args.Result = new Triple<DataTable, DataTable, Triple<int, string, bool>>(counterNames, instanceNames, parm);
        }

        public void backgroundWorker_GetSysPerfCountersAndInstanceComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            bool cancelledOrError = false;
            bool resetCounterComboBox = true;
            bool resetInstanceComboBox = true;
            try
            {
                if (args.Cancelled)
                {
                    cancelledOrError = true;
                    return;
                }
                if (args.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, "Error retrieving list of counters and instances for object", args.Error);
                    cancelledOrError = true;
                    return;
                }
                Triple<DataTable, DataTable, Triple<int, string, bool>> result =
                    (Triple<DataTable, DataTable, Triple<int, string, bool>>)args.Result;
                if (result.First != null)
                {
                    sqlCountersDataTable.Merge(result.First);
                }
                if (result.Second != null)
                {
                    sqlInstancesDataTable.Merge(result.Second);
                }
                string text = sqlCounterNameTextBox.Text.TrimEnd();
                if (text.Length > 0)
                {
                    ReselectRow(sqlCounterNameComboBox, text, SELECT_A_COUNTER);
                    resetCounterComboBox = false;
                }
                text = sqlInstanceNameTextBox.Text.TrimEnd();
                if (text.Length > 0)
                {
                    ReselectRow(sqlInstanceNameComboBox, text, SELECT_AN_INSTANCE);
                    resetInstanceComboBox = false;
                }
            }
            finally
            {
                EnableSqlComboBoxes(resetCounterComboBox, resetInstanceComboBox);
                if (cancelledOrError)
                {
                    currentSqlObjectName = String.Empty;
                    sqlCounterNameComboBox.Enabled = false;
                    sqlInstanceNameComboBox.Enabled = false;
                    SelectSpecialRow(sqlObjectNameComboBox, SELECT_AN_OBJECT);
                }
            }
        }

        private void sqlCounterNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTextBox(sender as ComboBox);
            EnableSqlTestButton();
        }

        private void sqlInstanceNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTextBox(sender as ComboBox);
            EnableSqlTestButton();
        }

        //private void warningThresholdSpinner_ValueChanged(object sender, EventArgs e)
        //{
        //    if (sender == warningThresholdSpinner)
        //        warningThresholdValue = warningThresholdSpinner.Value;
        //    else
        //    if (sender == criticalThresholdSpinner)
        //        criticalThresholdValue = criticalThresholdSpinner.Value;

        //    alertDefinitionPage.AllowMoveNext = IsAlertDefinitionPageComplete;
        //}

        private void calculationTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            alertDefinitionPage.AllowMoveNext = IsAlertDefinitionPageComplete;
        }

        private void CustomCounterWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (backgroundWorker != null && backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void hideWelcomePageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideCustomCounterWizardWelcomePage = hideWelcomePageCheckBox.Checked;
        }

        private void calculationTypePage_BeforeDisplay(object sender, EventArgs e)
        {
            calculationTypePage.AllowMoveNext = IsCalculationTypePageComplete;
        }

        private void counterScaleComboBox_TextChanged(object sender, EventArgs e)
        {
            calculationTypePage.AllowMoveNext = IsCalculationTypePageComplete;
        }

        void warningThresholdSpinner_TextChanged(object sender, EventArgs e)
        {
            if (sender == warningThresholdSpinner)
            {
                if (IsNotEmpty(warningThresholdSpinner.Text))
                    warningThresholdValue = warningThresholdSpinner.Value;
                else
                    warningThresholdValue = null;
            }
            else
                if (sender == criticalThresholdSpinner)
                {
                    if (IsNotEmpty(criticalThresholdSpinner.Text))
                        criticalThresholdValue = criticalThresholdSpinner.Value;
                    else
                        criticalThresholdValue = null;                    
                }
            else
                if (sender == infoThresholdSpinner)
                {
                    if (IsNotEmpty(infoThresholdSpinner.Text))
                    {
                        infoThresholdEnabled = true;
                        infoThresholdValue = infoThresholdSpinner.Value;
                    }
                    else
                    {
                        infoThresholdEnabled = false;
                        infoThresholdValue = null;
                    }
                }
            alertDefinitionPage.AllowMoveNext = IsAlertDefinitionPageComplete;
        }

        private void comparisonTypeGreaterThanButton_CheckedChanged(object sender, EventArgs e)
        {
            if (comparisonTypeGreaterThanButton.Checked)
            {
                if (infoThresholdSpinner.Value > criticalThresholdSpinner.Value)
                {
                    decimal temp = criticalThresholdSpinner.Value;
                    criticalThresholdSpinner.Value = infoThresholdSpinner.Value;
                    infoThresholdSpinner.Value = temp;
                }
            }
            else
            {
                if (infoThresholdSpinner.Value < criticalThresholdSpinner.Value)
                {
                    decimal temp = criticalThresholdSpinner.Value;
                    criticalThresholdSpinner.Value = infoThresholdSpinner.Value;
                    infoThresholdSpinner.Value = temp;
                }
            }
            alertDefinitionPage.AllowMoveNext = IsAlertDefinitionPageComplete;
        }

        private void counterNamePage_BeforeDisplay(object sender, EventArgs e)
        {
            counterNamePage.AllowMoveNext = IsCounterNamePageComplete;
            if (wmiCounterButton.Checked)
            {
                if (alertDescriptionTextBox.Text.TrimEnd().Length == 0)
                    alertDescriptionTextBox.Text = GetSelectedWmiCounterDescription();
            } 
            else if (azureCounterButton.Checked)
            {
                if (alertDescriptionTextBox.Text.TrimEnd().Length == 0)
                {
                    alertDescriptionTextBox.Text = azureMetricNameComboBox.Text;
                }
            }
            alertNameTextBox.Focus();
        }

        private void counterNamePage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            if (wmiCounterButton.Checked)
            {
                string description = alertDescriptionTextBox.Text.TrimEnd();
                if (description.Length > 0)
                {
                    if (GetSelectedWmiCounterDescription().Equals(description))
                        alertDescriptionTextBox.Text = String.Empty;
                }
            }
            else if (azureCounterButton.Checked)
            {
                string description = alertDescriptionTextBox.Text.TrimEnd();
                if (description.Length > 0)
                {
                    if (azureMetricNameComboBox.Text.Equals(description))
                        alertDescriptionTextBox.Text = String.Empty;
                }
            }
        }

        private void counterNamePage_BeforeMoveNext(object sender, CancelEventArgs args)
        {
            // don't move next if the name selected is already in use (add mode only)
            if (addMode)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                 
                    string counterName = alertNameTextBox.Text.Trim();
                    if (!CounterNameIsValid(counterName))
                    {
                        ApplicationMessageBox.ShowError(this, INVALIDCOUNTERNAME);
                        args.Cancel = true;
                        return;
                    }

                    bool available = "all".Equals(counterName, StringComparison.InvariantCultureIgnoreCase) == false;
                    if (available)
                        available = RepositoryHelper.IsCustomCounterNameAvailable(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counterName);
                    
                    if (!available)
                    {
                        ApplicationMessageBox.ShowError(
                            this,
                            String.Format(
                                "The counter name '{0}' has already been used.  Please enter a different name.",
                                counterName));
                        alertNameTextBox.Focus();
                        args.Cancel = true;
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void sqlObjectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sqlManualModeButton.Checked)
            {
                EnableSqlTestButton();
            }
        }

        private void alertAdvancedButton_Click(object sender, EventArgs e)
        {
            alertOnFailureToCollectModified = true;
            // advanced options normally configured in the alert configuration dialog
            using (FailureIsCriticalOptionDialog dialog = new FailureIsCriticalOptionDialog())
            {
                dialog.GenerateAlert = alertOnFailureToCollect;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    alertOnFailureToCollect = dialog.GenerateAlert;
            }
        }

        private string GetHelpTopic()
        {
            string topic = this.addMode ? HelpTopics.CustomCountersWizardAdd : HelpTopics.CustomCountersWizardEdit;

            if (wizard.SelectedPage == counterTypePage)
                topic = HelpTopics.CustomCountersWizardSelectCounterType;
            else
            if (wizard.SelectedPage == wmiCounterPage)
                topic = HelpTopics.CustomCountersWizardSelectSystemCounter;
            else
            if (wizard.SelectedPage == sqlCounterPage)
                topic = HelpTopics.CustomCountersWizardSelectSQLCounter;
            else
            if (wizard.SelectedPage == sqlBatchPage)
                topic = HelpTopics.CustomCountersWizardProvideSQLCounter;
            else
            if (wizard.SelectedPage == calculationTypePage)
                topic = HelpTopics.CustomCountersWizardCustomizeCounterValue;
            else
            if (wizard.SelectedPage == counterNamePage)
                topic = HelpTopics.CustomCountersWizardCustomizeCounterDefinition;
            else
            if (wizard.SelectedPage == alertDefinitionPage)
                topic = HelpTopics.CustomCountersWizardConfigureAlertSettings;
            else
            if (wizard.SelectedPage == vmCounterPage)
                topic = HelpTopics.CustomCountersWizardSelectVMCounter;
            return topic;
        }

        private void CustomCounterWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void CustomCounterWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void vmObjectNameComboBox_DropDown(object sender, EventArgs e)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void vmObjectNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = sqlObjectNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(sqlObjectNameComboBox, SELECT_AN_OBJECT);
            }
        }

        private void vmObjectNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            bool setCounter = true;
            bool setInstance = true;
            try
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox != null && currentVmObjectName != comboBox.Text)
                {
                    currentVmObjectName = comboBox.Text;
                    DataRowView rowView = comboBox.SelectedItem as DataRowView;
                    if (rowView != null)
                    {
                        string objectName = rowView[0] as string;
                        if (objectName == null || IsSpecialRow(rowView))
                        {
                            vmObjectNameTextBox.Text = String.Empty;
                            EnableVmTestButton();
                            return;
                        }
                        vmObjectNameTextBox.Text = objectName;

                        InitVmInstancePickListTable();

                        // instance name is visible but disabled
                        vmInstanceNameComboBox.Visible = true;
                        vmInstanceNameComboBox.Enabled = false;

                        if (vmObjectsDataTable.ExtendedProperties.ContainsKey("Source"))
                        {
                            DataTable source = (DataTable)vmObjectsDataTable.ExtendedProperties["Source"];
                            DataTable data = vmCountersDataTable.Clone();

                            Set<int> counterSet = new Set<int>();

                            foreach (DataRow row in source.Rows)
                            {
                                if (rowView["Source"].Equals(row["Source"]) && rowView["Group"].Equals(row["Group"]))
                                {
                                    int counterkey = (int) row["CounterKey"];
                                    if (counterSet.Contains(counterkey) || row["CounterLabel"].Equals("")) continue;
                                    counterSet.Add(counterkey);

                                    DataRow newRow = data.NewRow();
                                    newRow["Name"] = row["Counter"];
                                    newRow["Description"] = row["CounterSummary"];
                                    newRow["DisplayName"] = row["CounterLabel"];
                                    newRow[SPECIAL_COLUMN] = false;
                                    newRow["Source"] = row["Source"];
                                    newRow["Group"] = row["Group"];
                                    newRow["CounterKey"] = row["CounterKey"];
                                    newRow[SINGLETON_COLUMN] = row[SINGLETON_COLUMN];
                                    data.Rows.Add(newRow);
                                }
                            }
                            vmCountersDataTable.Merge(data);

                            // reselect counter and instance combo values
                            string text = vmCounterNameTextBox.Text.TrimEnd();
                            if (text.Length > 0)
                            {
                                setCounter = false;
                                ReselectRow(vmCounterNameComboBox, text, SELECT_A_COUNTER);
                            }
                            text = vmInstanceNameTextBox.Text.TrimEnd();
                            if (text.Length > 0)
                            {
                                setInstance = false;
                                ReselectRow(vmInstanceNameComboBox, text, SELECT_AN_INSTANCE);
                            }
                        }
                    }
                    else
                        SelectSpecialRow(sender as ComboBox, SELECT_AN_OBJECT);
                }
                EnableVmTestButton();
            }
            finally
            {
                EnableVmComboBoxes(setCounter, setInstance);
            }
        }

        private void vmCounterNameComboBox_DropDown(object sender, EventArgs e)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void vmCounterNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = vmCounterNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(vmCounterNameComboBox, SELECT_A_COUNTER);
            }
        }

        private void vmCounterNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox != null && currentVmCounterName != comboBox.Text)
                {
                    currentVmCounterName = comboBox.Text;

             //       DataRowView objectRow = vmObjectNameComboBox.SelectedItem as DataRowView;
                    DataRowView rowView = comboBox.SelectedItem as DataRowView;
                    if (rowView != null)
                    {
                        string counterName = rowView[0] as string;
                        if (counterName == null || IsSpecialRow(rowView))
                        {
                            vmObjectNameTextBox.Text = String.Empty;
                            EnableVmTestButton();
                            return;
                        }
                        vmCounterNameTextBox.Text = counterName;

                        bool singleton = (bool)rowView[SINGLETON_COLUMN];

                        InitVmCounterPickListTable();

                        if (!singleton && vmObjectsDataTable.ExtendedProperties.ContainsKey("Source"))
                        {
                            DataTable source = (DataTable)vmObjectsDataTable.ExtendedProperties["Source"];
                            DataTable data = vmInstancesDataTable.Clone();

                            foreach (DataRow row in source.Rows)
                            {
                                int counterkey1 = (int)row["CounterKey"];
                                int counterkey2 = (int) rowView["CounterKey"];
                                if (counterkey1 != counterkey2) continue;
                                if (rowView["Source"].Equals(row["Source"]) && rowView["Group"].Equals(row["Group"]) && rowView["CounterKey"].Equals(row["CounterKey"]))
                                {
                                    DataRow newRow = data.NewRow();
                                    newRow["Name"] = row["Instance"];
                                    newRow["DisplayName"] = row["Instance"];
                                    newRow[SPECIAL_COLUMN] = false;
                                    data.Rows.Add(newRow);
                                }
                            }
                            vmInstancesDataTable.Merge(data);
                        }

                        singleton = vmInstancesDataTable.Rows.Count <= 1;

                        vmInstanceNameComboBox.Enabled = !singleton;
                        vmInstanceNameComboBox.Visible = !singleton;
                    }
                    else
                        SelectSpecialRow(sender as ComboBox, SELECT_AN_INSTANCE);
                }
                EnableVmTestButton();
            }
            finally
            {
                EnableVmComboBoxes(false, true);
            }
        }

        private void vmInstanceNameComboBox_DropDown(object sender, EventArgs e)
        {
            RemoveSpecialRow(sender as ComboBox);
        }

        private void vmInstanceNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataRowView selectedRow = vmInstanceNameComboBox.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                SelectSpecialRow(vmInstanceNameComboBox, SELECT_AN_INSTANCE);
            }
        }

        private void vmInstanceNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTextBox(sender as ComboBox);
            EnableVmTestButton();
        }

        private void EnableVmComboBoxes(bool setCounterComboBox, bool setInstancesComboBox)
        {
            WrappedSqlServerObject selectedServer = vmServerComboBox.SelectedItem as WrappedSqlServerObject;
            vmServerComboBox.Enabled = true;
            vmBrowseModeButton.Enabled = true;
            vmManualModeButton.Enabled = true;
            if (selectedServer != null && selectedServer.WrappedObject is MonitoredSqlServerWrapper)
            {
                vmObjectNameComboBox.Enabled = true;
                DataRowView objectRow = vmObjectNameComboBox.SelectedItem as DataRowView;
                bool objectSelected = !IsSpecialRow(objectRow);
                vmCounterNameComboBox.Enabled = objectSelected;
                if (setCounterComboBox)
                    SetComboSelection(vmCounterNameComboBox, SELECT_A_COUNTER);

                DataRowView counterRow = vmCounterNameComboBox.SelectedItem as DataRowView;
                bool counterSelected = counterRow != null && !IsSpecialRow(counterRow);
                vmInstanceNameComboBox.Enabled = counterSelected;
                if (setInstancesComboBox)
                    SetComboSelection(vmInstanceNameComboBox, SELECT_AN_INSTANCE);
            }
            EnableVmTestButton();
        }

        private void EnableVmTestButton()
        {
            vmCounterPage.AllowMoveNext = IsVmCounterPageComplete;
        }

		/// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        #region Azure Server

        private void EnableConfigNextButton()
        {
            azureServerConfigPage.AllowMoveNext = IsAzureServerConfigPageComplete;
        }

        private void LoadAzureControls(ComboBox azureResourceNameComboBox)
        {
                azureServerComboBox.Enabled = true;
                azureProfileComboBox.Enabled = false;

                azureServerContentPanel.Visible = true;
                EnableConfigNextButton();
                if (azureServerComboBox.SelectedIndex ==
                    azureServerComboBox.Items.IndexOf(SELECT_A_SERVER))
                { 
                    return;
                }
                // Edit mode
                azureProfileComboBox.Enabled = azureProfileComboBox.Items.Count > 0;

                if (azureProfileComboBox.SelectedIndex > 0)
                {
                    azureResourceTypeNameComboBox.Enabled = true;
                    LoadResourceTypeNameValues();
                }
                if(azureResourceTypeNameComboBox.SelectedIndex > 0)
                {
                    this.azureResourceNameComboBox.Enabled = true;
                    LoadResourceNameValues();
                }
        }

        private void BindAzureProfiles()
        {
            if(azureProfiles == null)
            {
                azureProfiles = LoadAzureProfile();
                azureProfiles = azureProfiles ?? new List<IAzureProfile>();
            }
            azureProfileComboBox.Items.Clear();
            azureProfiles = azureProfiles.Where(p => p.ApplicationProfile != null).ToList();
            azureProfiles.Sort(
                (p1, p2) => string.CompareOrdinal(p1.ApplicationProfile.Name, p2.ApplicationProfile.Name));
            foreach (var azureProfile in azureProfiles)
            {
                var item = new TextItem<IAzureProfile>
                {
                    Text = azureProfile.ApplicationProfile.Name,
                    Value = azureProfile
                };
                azureProfileComboBox.Items.Add(item);
            }

            var index = azureProfileComboBox.Items.IndexOf(SELECT_A_PROFILE);
            if (index == -1)
            {
                azureProfileComboBox.Items.Insert(0, SELECT_A_PROFILE);
                azureProfileComboBox.SelectedIndex = 0;
            }
            else
            {
                azureProfileComboBox.SelectedIndex = index;
            }

            if (string.IsNullOrEmpty(azureApplicationProfileId)) {
                var azureProfileItem = azureProfileComboBox.SelectedItem as TextItem<IAzureProfile>;
                azureApplicationProfileId = azureProfileItem != null && azureProfileItem.Value != null &&
                                            azureProfileItem.Value.ApplicationProfile != null
                    ? azureProfileItem.Value.ApplicationProfile.Id.ToString()
                    : azureApplicationProfileId;
                return;
            }

            // Edit area
            var activeProfile = azureProfiles.FirstOrDefault(x =>
                x.ApplicationProfile.Id.Equals(Convert.ToInt64(azureApplicationProfileId)));

            if (activeProfile == null)
            {
                return;
            }

            var profileItem = new TextItem<IAzureProfile>
            {
                Text = activeProfile.ApplicationProfile.Name,
                Value = activeProfile
            };
            foreach (var item in azureProfileComboBox.Items)
            {
                if (!(item is TextItem<IAzureProfile>))
                {
                    continue;
                }

                var azureProfileTextItem = item as TextItem<IAzureProfile>;
                if (profileItem.Value != null && !item.ToString().Equals(SELECT_A_PROFILE) &&
                    profileItem.Text.Equals(azureProfileTextItem.Text, StringComparison.InvariantCultureIgnoreCase) &&
                    profileItem.Value.ApplicationProfile.Id == azureProfileTextItem.Value.ApplicationProfile.Id)
                {
                    azureProfileComboBox.SelectedIndex = azureProfileComboBox.Items.IndexOf(item);
                    HandleProfileChanged(azureProfileComboBox);
                    break;
                }
            }

            var azureProfileWrapper = azureProfileComboBox.SelectedItem as TextItem<IAzureProfile>;
            azureApplicationProfileId = azureProfileWrapper != null && azureProfileWrapper.Value != null &&
                                        azureProfileWrapper.Value.ApplicationProfile != null
                ? azureProfileWrapper.Value.ApplicationProfile.Id.ToString()
                : azureApplicationProfileId;
        }

        private void ResourceTypeName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!(sender is ComboBox))
            {
                return;
            }

            ComboBox comboBox = (ComboBox)sender;
            if (azureProfileComboBox.Text == SELECT_A_PROFILE)
            {
                EnableConfigNextButton();
                return;
            }
            HandleResourceTypeChanged(comboBox);

        }

        private void HandleResourceTypeChanged(ComboBox comboBox)
        {
            if (azureResourceTypeNameComboBox.Text == SELECT_A_RESOURCE_TYPE)
            {
                azureResourceNameComboBox.Enabled = false; 
                EnableConfigNextButton(); 
                return;
            }

            azureResourceType = azureResourceTypeNameComboBox.Text;
            // Toggle display
            azureResourceNameComboBox.Enabled = true;
            ToggleAzureConfigurationPageControls(true);
            EnableConfigNextButton();
            LoadResourceNameValues();
        }

        private void Profile_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (azureProfileComboBox.Text == SELECT_A_PROFILE)
            {
                azureResourceNameComboBox.Enabled = false;
                azureResourceTypeNameComboBox.Enabled = false;
                EnableConfigNextButton();
                return;
            }
            ComboBox comboBox = (ComboBox)sender;
            currentAzureServerName = azureServerComboBox.Text;
            if (azureServerComboBox.SelectedItem is WrappedSqlServerObject)
            {
                WrappedSqlServerObject wrapper = (WrappedSqlServerObject)azureServerComboBox.SelectedItem;
                if (wrapper.WrappedObject is MonitoredSqlServerWrapper)
                {

                    MonitoredSqlServerWrapper instance = (MonitoredSqlServerWrapper)wrapper.WrappedObject;
                    if (instance.Instance != null &&
                        (instance.Instance.CloudProviderId == Common.Constants.MicrosoftAzureId ||
                         instance.Instance.CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                    {
                        HandleProfileChanged(comboBox);
                        EnableConfigNextButton();
                        return;
                    }
                }
            }

        }

        private void ResourceName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (azureResourceNameComboBox.Text != SELECT_A_RESOURCE)
            {
                azureResourceName = azureResourceNameComboBox.Text;
            }
            EnableConfigNextButton();
        }

        private void azureServerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (azureServerComboBox.Text == SELECT_A_SERVER)
            {
                azureProfileComboBox.Enabled = false;
                azureResourceNameComboBox.Enabled = false;
                azureResourceTypeNameComboBox.Enabled = false;
            }
            else
            {
                var server = azureServerComboBox.SelectedItem as WrappedSqlServerObject;
                if (server != null && server.WrappedObject != null && server.WrappedObject is MonitoredSqlServerWrapper)
                {
                    azureServerId = ((MonitoredSqlServerWrapper)server.WrappedObject).Id.ToString();
                }
                
                azureProfileComboBox.Enabled = true;
                azureProfileComboBox.Items.Insert(0, SELECT_A_PROFILE);
                azureProfileComboBox.SelectedIndex = 0;
                LoadAzureProfile();
                BindAzureProfiles();
                if (azureProfileComboBox.Enabled && azureProfileComboBox.SelectedIndex > 0)
                {
                    azureResourceTypeNameComboBox.Enabled = true;
                }
                else
                {
                    azureResourceTypeNameComboBox.SelectedIndex = 0;
                    azureResourceTypeNameComboBox.Enabled = false;
                }
                if (azureResourceTypeNameComboBox.Enabled && azureResourceTypeNameComboBox.SelectedIndex > 0)
                {
                    azureResourceNameComboBox.Enabled = true;
                }
                else
                {
                    azureResourceNameComboBox.SelectedIndex = 0;
                    azureResourceNameComboBox.Enabled = false;
                }
            }
            EnableConfigNextButton();
        }

        private void HandleProfileChanged(ComboBox comboBox)
        {
            azureResourceNameComboBox.Enabled = false;
            if (azureProfileComboBox.Text != SELECT_A_PROFILE)
            {
                var azureProfileWrapper = azureProfileComboBox.SelectedItem as TextItem<IAzureProfile>;
                azureApplicationProfileId = azureProfileWrapper != null && azureProfileWrapper.Value != null &&
                                            azureProfileWrapper.Value.ApplicationProfile != null
                    ? azureProfileWrapper.Value.ApplicationProfile.Id.ToString()
                    : azureApplicationProfileId;
                azureResourceTypeNameComboBox.Enabled = true;
                LoadResourceTypeNameValues();
                if (azureResourceTypeNameComboBox.SelectedIndex > 0)
                {
                    azureResourceNameComboBox.Enabled = true;
                }
                if (!string.IsNullOrWhiteSpace(azureResourceName))
                {
                    if (azureResourceTypeNameComboBox.SelectedIndex > 0)
                    {
                        ResourceTypeName_SelectionChangeCommitted(azureResourceTypeNameComboBox, null);
                    }
                }
             }
            else
            {
                azureResourceTypeNameComboBox.Enabled = false;
                azureResourceNameComboBox.Enabled = false;
            }
            EnableConfigNextButton();
        }

        private void ToggleAzureConfigurationPageControls(bool resourceTypeToggle)
        {

            azureResourceNameLabel.Visible = resourceTypeToggle;
            azureResourceNameComboBox.Visible = resourceTypeToggle;
        }

        private void LoadResourceTypeNameValues()
        {
            var profileItem = azureProfileComboBox.SelectedItem as TextItem<IAzureProfile>;
            if (profileItem != null)
            {
                var selectedProfile = profileItem.Value;
                var resources = selectedProfile.ApplicationProfile.Resources;
                var groupedResources = resources
                    .Where(r => !string.IsNullOrWhiteSpace(r.Type) && !string.IsNullOrWhiteSpace(r.Name))
                    .OrderBy(resource => resource.Type)
                    .GroupBy(resource => resource.Type)
                    .ToDictionary(groupedResource => groupedResource.Key,
                        groupedResource => groupedResource.ToList());
                azureResourceTypeNameComboBox.Items.Clear();

                foreach (var groupedResource in groupedResources)
                {
                    groupedResource.Value.Sort((r1, r2) => string.CompareOrdinal(r1.Name, r2.Name));
                    var item = new TextItem<List<IAzureResource>>
                    {
                        Text = groupedResource.Key,
                        Value = groupedResource.Value
                    };
                    azureResourceTypeNameComboBox.Items.Add(item);
                }

                var index = azureResourceTypeNameComboBox.Items.IndexOf(SELECT_A_RESOURCE_TYPE);
                if (index == -1)
                {
                    azureResourceTypeNameComboBox.Items.Insert(0, SELECT_A_RESOURCE_TYPE);
                    azureResourceTypeNameComboBox.SelectedIndex = 0;
                }
                else
                {
                    azureResourceTypeNameComboBox.SelectedIndex = index;
                }

                if (string.IsNullOrEmpty(azureResourceType))
                {
                    azureResourceType = azureResourceTypeNameComboBox.SelectedIndex > 0
                        ? azureResourceTypeNameComboBox.Text
                        : azureResourceType;
                    return;
                }

                // Edit Case
                foreach (var item in azureResourceTypeNameComboBox.Items)
                {
                    if (item.ToString().Equals(SELECT_A_RESOURCE_TYPE) || !item.ToString()
                            .Equals(azureResourceType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    azureResourceTypeNameComboBox.SelectedItem = item;
                    break;
                }

                azureResourceType = azureResourceTypeNameComboBox.SelectedIndex > 0
                    ? azureResourceTypeNameComboBox.Text
                    : azureResourceType;
            }
            else
            {
                azureResourceTypeNameComboBox.Enabled = false;
            }
        }

        private void LoadResourceNameValues()
        {
            var azureResourcesPair = (TextItem<List<IAzureResource>>)azureResourceTypeNameComboBox.SelectedItem;
            if (azureResourcesPair != null)
            {
                azureResourceNameComboBox.Items.Clear();
                if (azureResourcesPair.Value != null)
                {
                    azureResourcesPair.Value.Sort((r1, r2) =>
                        string.CompareOrdinal(r1.Name, r2.Name));
                    foreach (IAzureResource resource in azureResourcesPair.Value)
                    {
                        var item = new TextItem<IAzureResource>
                        {
                            Text = resource.Name,
                            Value = resource
                        };
                        azureResourceNameComboBox.Items.Add(item);
                    }
                }

                var index = azureResourceNameComboBox.Items.IndexOf(SELECT_A_RESOURCE);
                if (index == -1)
                {
                    azureResourceNameComboBox.Items.Insert(0, SELECT_A_RESOURCE);
                    azureResourceNameComboBox.SelectedIndex = 0;
                }
                else
                {
                    azureResourceNameComboBox.SelectedIndex = index;
                }

                if (string.IsNullOrEmpty(azureResourceName))
                {
                    azureResourceName = azureResourceNameComboBox.SelectedIndex > 0
                        ? azureResourceNameComboBox.Text
                        : azureResourceName;
                    return;
                }

                // Edit mode
                foreach (var item in azureResourceNameComboBox.Items)
                {
                    if (!(item is TextItem<IAzureResource>))
                    {
                        continue;
                    }

                    var azureResource = (TextItem<IAzureResource>) item;
                    if (azureResourceName.Equals(azureResource.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        azureResourceNameComboBox.SelectedIndex =
                            azureResourceNameComboBox.Items.IndexOf(item);
                        break;
                    }
                }

                azureResourceName = azureResourceNameComboBox.SelectedIndex > 0
                    ? azureResourceNameComboBox.Text
                    : azureResourceName;
            }
            else
                azureResourceNameComboBox.Enabled = false;
        }

        private List<IAzureProfile> LoadAzureProfile()
        {
            var selectedSqlServerId = ((MonitoredSqlServerWrapper)((WrappedSqlServerObject)azureServerComboBox.SelectedItem).WrappedObject).Id;
            azureProfiles =  RepositoryHelper.GetAzureProfiles(Settings.Default.ActiveRepositoryConnection.ConnectionInfo).Where(p => p.SqlServerId == selectedSqlServerId).ToList();
            return azureProfiles;
            
    }

        private MonitorManagementConfiguration GetMonitorConfig()
        {
            MonitorManagementConfiguration config = new MonitorManagementConfiguration();
            IAzureProfile selectedProfile = ((TextItem<IAzureProfile>) azureProfileComboBox.SelectedItem).Value;
            IAzureResource resource = ((TextItem<IAzureResource>) azureResourceNameComboBox.SelectedItem).Value;
            config.Profile = selectedProfile;
            config.MonitorParameters = new AzureMonitorParameters
            {
                Resource = resource
            };
            return config;
        }

        private void LoadAzureMetricControls(ComboBox azureMetricComboBox)
        {
            azureMetricPageServerNameLbl.Text = azureServerComboBox.Text;
            azureMetricPageResourceGroupNameLbl.Text = azureResourceName;
            azureMetricPageObjectNameLbl.Text = ((TextItem<List<IAzureResource>>) azureResourceTypeNameComboBox.SelectedItem).Text;
            var instanceId = ((MonitoredSqlServerWrapper)((WrappedSqlServerObject)azureServerComboBox.SelectedItem).WrappedObject).Id;

            DataTable metrics = null;
            try
            {
                metrics = managementService.GetAzureMonitorDefinitions(instanceId, GetMonitorConfig());
                azureMetricLabel.Text = metrics == null || metrics.Rows.Count == 0
                    ? "Note: Metrics with dimensions are not supported, please change the resource type and try again."
                    : string.Empty;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, string.Format(
                    "Please ensure that the selected application profile {0} has required access to the resource: {1}",
                    azureProfileComboBox.Text, azureResourceNameComboBox.Text), ex);
                azureMetricLabel.Text = metrics == null || metrics.Rows.Count == 0 ? "Please ensure that the selected application profile has sufficient access." : string.Empty;
            }
            azureMetricComboBox.Items.Clear();
            if (metrics != null && metrics.Rows.Count > 0)
            {
                foreach (DataRow row in metrics.Rows)
                {
                    azureMetricNameComboBox.Items.Add(row.ItemArray[2].ToString());
                }
            }
            var index = azureMetricNameComboBox.Items.IndexOf(SELECT_A_METRIC);
            if (index == -1)
            {
                azureMetricNameComboBox.Items.Insert(0, SELECT_A_METRIC);
                azureMetricNameComboBox.SelectedIndex = 0;
            }
            else
            {
                azureMetricNameComboBox.SelectedIndex = index;
            }
            azureServerMetricsPanel.Visible = true;
            if (string.IsNullOrEmpty(azureMetricDisplayName)) {
                azureMetricDisplayName = azureMetricNameComboBox.SelectedIndex > 0
                ? azureMetricNameComboBox.Text
                : azureMetricDisplayName;
                return;
            }
            // Edit mode

            foreach (var item in azureMetricNameComboBox.Items)
            {
                var azureMetric = (string) item;
                if (azureMetricDisplayName.Equals(azureMetric, StringComparison.InvariantCultureIgnoreCase))
                {
                    azureMetricNameComboBox.SelectedIndex = azureMetricNameComboBox.Items.IndexOf(item);
                    break;
                }
            }

            azureMetricDisplayName = azureMetricNameComboBox.SelectedIndex > 0
                ? azureMetricNameComboBox.Text
                : azureMetricDisplayName;
        }

        private void azureProfileButton_click(object sender, EventArgs e)
        {
            var applicationDialog = new AzureProfilesConfiguration(true);
            applicationDialog.ShowDialog(this);
        }

        private void azureMetricNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            azureServerMetricsPage.AllowMoveNext = IsAzureServerMetricsPageComplete;
            azureMetricNameComboBox.Focus();
        }

        #endregion

    }
}
