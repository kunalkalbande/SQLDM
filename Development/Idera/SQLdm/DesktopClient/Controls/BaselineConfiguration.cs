using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using System.Data.SqlClient;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using System.IO;
using System.Web;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class BaselineConfigurationPage : UserControl
    {
        private BaselineConfiguration baselineConfig;
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private Dictionary<int, BaselineConfiguration> baselineConfigList;
        private BaselineConfiguration maxBaselineConfig;
        private bool isRefresh;
        private bool isReset;
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private bool isInit;
        private string errorMessage;
        private int instanceId;
        private DateTime? earliestDataAvailable;
        private bool isMultiEdit;

        private const string multiEditMainHeader = "Configure a performance baseline for your monitored SQL Server(s).";
        private const string multiEditSubHeader = "What date range should be used to calculate the performance baseline for the server(s)?";
        private const string multiEditInfoHeader = "You should select a date range that represents the typical operation of your monitored SQL Server(s). This baseline will be used to offer recommendations to help you better configure your alerts in the future.";
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BaselineConfiguration BaselineConfig
        {
            get { return baselineConfig; }
            set { baselineConfig = value; Init(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        public Dictionary<int, BaselineConfiguration> BaselineConfigList
        {
            get { return baselineConfigList; }
            set { baselineConfigList = value; Init(); }
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; GetEarliestDataAvailable(); }
        }

        [Browsable(false)]
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        public bool HeaderVisible
        {
            get { return this.headerStrip1.Visible; }
            set { this.headerStrip1.Visible = value; }
        }

        public bool IsMultiEdit
        {
            get { return isMultiEdit; }
            set { isMultiEdit = value; }
        }

        public Color BackColor
        {
            get { return this.office2007PropertyPage1.BackColor; }
            set { this.office2007PropertyPage1.BackColor = this.baselineMainContainer.BackColor = value; }
        }
        public int BorderWidth
        {
            get { return this.office2007PropertyPage1.BorderWidth; }
            set { this.office2007PropertyPage1.BorderWidth = value; }
        }

        public void ChangeBaselineTittleControlStyle()
        {
            this.headerStrip1.Visible = false;
            this.propertiesHeaderStrip1.Visible = false;
            this.propertiesHeaderStripSelectServers.Visible = false;
            this.header1label.Visible = true;
            this.header2label.Visible = true;
            this.header3label.Visible = true;
            this.baselineMainContainer.Dock = DockStyle.Fill;
        }

        public BaselineConfigurationPage()
        {
            InitializeComponent();
            baselineConfig = new BaselineConfiguration();
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            baselineConfigList = new Dictionary<int, BaselineConfiguration>();
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            Init();
            //SQLDM-30848, adapting resolutions, Saurabh
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            SetPropertiesTheme();
            Infragistics.Windows.Themes.ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        public bool CheckForErrors()
        {
            errorMessage = string.Empty;
            BaselineTemplate template = baselineConfig.Template;


            if (!template.UseDefault)
            {
                if (template.CalculationStartDate > template.CalculationEndDate)
                {
                    errorMessage = "The start date must be before the end date for the baseline period.";
                    return true;
                }
            }

            ///SQLdm(10.1) Srishti Purohit -- Checking time range if Time is crossing mid night
            //if (template.StartDate.TimeOfDay > template.EndDate.TimeOfDay)
            //{
            //    errorMessage = "The 'from' time must be before the 'to' time for the baseline period.";
            //    return true;
            //}

            if (template.CalculationSelectedDays == 0)
            {
                errorMessage = "The days of the week to include in the baseline must be selected.";
                return true;
            }
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            if (baselineConfigList != null)
            {
                foreach (BaselineConfiguration config in baselineConfigList.Values)
                {
                    template = config.Template;


                    if (!template.UseDefault)
                    {
                        if (template.CalculationStartDate > template.CalculationEndDate)
                        {
                            errorMessage = config.BaselineName + ": The start date must be before the end date for the baseline period.";
                            return true;
                        }
                    }

                    ///SQLdm(10.1) Srishti Purohit -- Checking time range if Time is crossing mid night
                    //if (template.StartDate.TimeOfDay > template.EndDate.TimeOfDay)
                    //{
                    //    errorMessage = config.BaselineName + ": The 'from' time must be before the 'to' time for the baseline period.";
                    //    return true;
                    //}

                    //if (template.SelectedDays == 0)
                    //{
                    //    errorMessage = config.BaselineName + ": The days of the week to include in the baseline must be selected.";
                    //    return true;
                    //}

                }
            }
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            return false;
        }

        public void SetVisibleBaseline(bool visible)
        {
            header2label.Visible = visible;
            daysChkContainer.Visible = visible;
            timeDateContainer.Visible = visible;
            header3label.Visible = visible;
            btnSelectOtherServers.Visible = visible;
        }

        public void Init()
        {
            if (baselineConfig == null)
                baselineConfig = new BaselineConfiguration();

            BaselineTemplate template = baselineConfig.Template;
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            if (baselineConfigList != null)// vineet -- fixing de45620
            {
                foreach (var config in baselineConfigList.Values)
                {
                    if (maxBaselineConfig == null)
                    {
                        maxBaselineConfig = new BaselineConfiguration();
                        maxBaselineConfig = config;
                        RefreshPanel(maxBaselineConfig);
                    }
                    //comboBox1.Items.Add(config);
                    //comboBox1.DisplayMember = "config.BaselineName";
                    //comboBox1.ValueMember = "config.TemplateID";
                }
            }
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            isInit = true;

            if (isMultiEdit)
            {
                office2007PropertyPage1.Text = multiEditMainHeader;
                headerStrip1.Text = multiEditSubHeader;
                informationBox1.Text = multiEditInfoHeader;
                propertiesHeaderStripSelectServers.Visible = false;
                this.header3label.Visible = false;
                btnSelectOtherServers.Visible = false;
            }

            automaticBaselineRadioButton.Checked = template.UseDefault;
            customBaselineRadioButton.Checked = !template.UseDefault;
            customBaselineRadioButton_CheckedChanged(customBaselineRadioButton, EventArgs.Empty);

            customDateFromCombo.Enabled = !template.UseDefault;
            customDateToCombo.Enabled = !template.UseDefault;

            customDateFromCombo.Value = template.CalculationStartDate.Date;
            customDateToCombo.Value = template.CalculationEndDate.Date;
            beginTimeCombo.Time = template.CalculationStartDate.TimeOfDay;
            endTimeCombo.Time = template.CalculationEndDate.TimeOfDay;

            sundayCheckbox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, template.CalculationSelectedDays);
            mondayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, template.CalculationSelectedDays);
            tuesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, template.CalculationSelectedDays);
            wednesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, template.CalculationSelectedDays);
            thursdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, template.CalculationSelectedDays);
            fridayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, template.CalculationSelectedDays);
            saturdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, template.CalculationSelectedDays);

            isInit = false;
        }

        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void RefreshPanel(BaselineConfiguration config)
        {
            isRefresh = true;
            BaselineTemplate template = new BaselineTemplate();
            template = config.Template;
            //textBox1.Text = config.BaselineName;
            //comboBox1.Text = config.BaselineName;
            beginTimeCombo1.Time = template.CalculationStartDate.TimeOfDay;
            endTimeCombo1.Time = template.CalculationEndDate.TimeOfDay;

            //sundayCheckbox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, template.SelectedDays);
            //mondayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, template.SelectedDays);
            //tuesdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, template.SelectedDays);
            //wednesdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, template.SelectedDays);
            //thursdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, template.SelectedDays);
            //fridayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, template.SelectedDays);
            //saturdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, template.SelectedDays);
            isRefresh = false;
        }
        private bool CheckOverlap(BaselineConfiguration config1, BaselineConfiguration config2)
        {
            if (config1.BaselineName.Equals(config2.BaselineName))
            {
                ResetView();
                MessageBox.Show(this, "Baseline with same name exists", "Same Name Baseline Detected");
                return true;
            }
            foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (MonitoredSqlServer.MatchDayOfWeek(d, config1.Template.CalculationSelectedDays) && MonitoredSqlServer.MatchDayOfWeek(d, config2.Template.CalculationSelectedDays))
                {
                    if (config1.Template.CalculationStartDate.TimeOfDay < config2.Template.CalculationEndDate.TimeOfDay && config2.Template.CalculationStartDate.TimeOfDay < config1.Template.CalculationEndDate.TimeOfDay)
                    {
                        ResetView();
                        MessageBox.Show(this, String.Format("Baseline {0} overlaps with baseline {1}", config1.BaselineName, config2.BaselineName), "Overlapping Schedule Detected");
                        return true;
                    }
                }
            }
            return false;
        }

        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - comboBox_SelectedEvent Changed
        private void GetEarliestDataAvailable()
        {
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                BaselineHelpers.GetEarliestAvailableData(connection, instanceId, out earliestDataAvailable);
            }
        }

        private void customBaselineRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (isInit)
                return;

            customDateFromLabel.Enabled =
            customDateFromCombo.Enabled =
            customDateToLabel.Enabled =
            customDateToCombo.Enabled = customBaselineRadioButton.Checked;
        }
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void addButton_Click(object sender, System.EventArgs e)
        {
            Dictionary<int, BaselineConfiguration> clone = ObjectHelper.Clone(BaselineConfigList);
            AddBaselineDialog dlg = new AddBaselineDialog(BaselineConfigList);
            //dlg.Parent = this;
            DialogResult resultDialog = dlg.ShowDialog();
            if (resultDialog == DialogResult.Cancel)
            {
                BaselineConfigList = clone;
            }

        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void automaticBaselineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (isInit)
                return;
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            baselineConfig.Template.UseDefault = automaticBaselineRadioButton.Checked;
            foreach (int key in baselineConfigList.Keys)
            {
                baselineConfigList[key].Template.UseDefault = automaticBaselineRadioButton.Checked;
                baselineConfigList[key].IsChanged = true;
            }
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        }

        private void customBaselineRadioButton_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isInit)
                return;

            customDateFromCombo.Enabled = customBaselineRadioButton.Checked;
            customDateToCombo.Enabled = customBaselineRadioButton.Checked;
            customDateFromLabel.Enabled = customBaselineRadioButton.Checked;
            customDateToLabel.Enabled = customBaselineRadioButton.Checked;

            baselineConfig.Template.UseDefault = !customBaselineRadioButton.Checked;
            baselineConfig.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + baselineConfig.Template.CalculationStartDate.TimeOfDay;
            baselineConfig.Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + baselineConfig.Template.CalculationEndDate.TimeOfDay;
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            foreach (int key in baselineConfigList.Keys)
            {
                baselineConfigList[key].Template.UseDefault = !customBaselineRadioButton.Checked;
                baselineConfigList[key].Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + baselineConfigList[key].Template.CalculationStartDate.TimeOfDay;
                baselineConfigList[key].Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + baselineConfigList[key].Template.CalculationEndDate.TimeOfDay;
                baselineConfigList[key].IsChanged = true;
            }
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        }

        //SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void Update(object sender, EventArgs e)
        {
            if (isInit || isRefresh || isReset)
                return;
            //if(comboBox1.Items.Count> 0 && string.IsNullOrWhiteSpace(textBox1.Text))//SQLdm 10.0 (Tarun Sapra)- Skipping the check if there is no custom baseline
            //{
            //    ResetView();
            //    MessageBox.Show(this,"BaselineName can't be empty", "Invalid Baseline Name");
            //    return ;
            //}
            BaselineConfiguration config1 = new BaselineConfiguration();
            BaselineConfiguration config2 = new BaselineConfiguration();
            config1.BaselineName = baselineConfig.BaselineName;
            config1.TemplateID = baselineConfig.TemplateID;
            config1.Active = baselineConfig.Active;
            short days = 0;

            if (sundayCheckbox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (mondayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (tuesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (wednesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (thursdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (fridayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (saturdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

            config1.Template.CalculationSelectedDays = days;
            try
            {
                config1.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + beginTimeCombo.Time;
                config1.Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + endTimeCombo.Time;
            }
            catch { }

            //if (comboBox1.Items.Count>0)
            //{
            //    BaselineConfiguration config = (BaselineConfiguration)comboBox1.SelectedItem;
            //    if (config == null)
            //    {
            //        config2.BaselineName = maxBaselineConfig.BaselineName;
            //        config2.TemplateID = maxBaselineConfig.TemplateID;
            //        config2.Active = maxBaselineConfig.Active;
            //    }
            //    else
            //    {
            //        config2.BaselineName = config.BaselineName;
            //        config2.TemplateID = config.TemplateID;
            //        config2.Active = config.Active;
            //    }
            //    days = 0;
            //    //if (sundayCheckbox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            //    //if (mondayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            //    //if (tuesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            //    //if (wednesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            //    //if (thursdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            //    //if (fridayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            //    //if (saturdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
            //    config2.Template.SelectedDays = days;
            //    config2.BaselineName = textBox1.Text;
            //    config2.Template.StartDate = ((DateTime)customDateFromCombo.Value).Date + beginTimeCombo1.Time;
            //    config2.Template.EndDate = ((DateTime)customDateToCombo.Value).Date + endTimeCombo1.Time;
            //    if (CheckOverlap(config1, config2))
            //    {                    
            //        return;
            //    }
            //    foreach (int key in baselineConfigList.Keys)
            //    {
            //        if (CheckOverlap(config1, baselineConfigList[key]))
            //        {                        
            //            return;
            //        }
            //    }
            //    foreach (int key in baselineConfigList.Keys)
            //    {
            //        if (key != config2.TemplateID)
            //            if (CheckOverlap(config2, baselineConfigList[key]))
            //            {                            
            //                return;
            //            }
            //    }
            //    baselineConfigList[config2.TemplateID].Template.SelectedDays = config2.Template.SelectedDays;
            //    baselineConfigList[config2.TemplateID].Template.StartDate = config2.Template.StartDate;
            //    baselineConfigList[config2.TemplateID].Template.EndDate = config2.Template.EndDate;
            //    baselineConfigList[config2.TemplateID].BaselineName = config2.BaselineName;
            //    baselineConfigList[config2.TemplateID].IsChanged = true;
            //    config2.IsChanged = true;
            //    //if (config != null)
            //    //{
            //        //comboBox1.Items.Remove(config);
            //        //comboBox1.Items.Insert(0, baselineConfigList[config2.TemplateID]);
            //        RefreshPanel(baselineConfigList[config2.TemplateID]);
            //    //}
            //    //else
            //    //{
            //    //    //comboBox1.Items.Remove(maxBaselineConfig);
            //    //    //comboBox1.Items.Insert(0, baselineConfigList[config2.TemplateID]);
            //    //    RefreshPanel(baselineConfigList[config2.TemplateID]);
            //    //}
            //}
            baselineConfig.Template.CalculationSelectedDays = config1.Template.CalculationSelectedDays;
            baselineConfig.Template.CalculationStartDate = config1.Template.CalculationStartDate;
            baselineConfig.Template.CalculationEndDate = config1.Template.CalculationEndDate;

            foreach (int key in baselineConfigList.Keys)
            {
                baselineConfigList[key].Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + baselineConfigList[key].Template.CalculationStartDate.TimeOfDay;
                baselineConfigList[key].Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + baselineConfigList[key].Template.CalculationEndDate.TimeOfDay;
            }
        }
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void ResetView()
        {
            isReset = true;
            //BaselineConfiguration config = new BaselineConfiguration();
            //config = (BaselineConfiguration)comboBox1.SelectedItem;
            //if (config == null)
            //    config = maxBaselineConfig;
            //if (maxBaselineConfig == null)
            //    return;
            //RefreshPanel(config);

            //if (config != null) RefreshPanel(config); //SQLdm 10.0 (Tarun Sapra)- if there is no other config then default, don't update the UI

            BaselineTemplate template = new BaselineTemplate();
            template = baselineConfig.Template;
            customDateFromCombo.Value = template.CalculationStartDate.Date;
            customDateToCombo.Value = template.CalculationEndDate.Date;
            beginTimeCombo.Time = template.CalculationStartDate.TimeOfDay;
            endTimeCombo.Time = template.CalculationEndDate.TimeOfDay;

            sundayCheckbox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, template.CalculationSelectedDays);
            mondayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, template.CalculationSelectedDays);
            tuesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, template.CalculationSelectedDays);
            wednesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, template.CalculationSelectedDays);
            thursdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, template.CalculationSelectedDays);
            fridayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, template.CalculationSelectedDays);
            saturdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, template.CalculationSelectedDays);
            isReset = false;
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private void btnSelectOtherServers_Click(object sender, EventArgs e)
        {
            using (SelectMonitoredServersDialog dialog = new SelectMonitoredServersDialog())
            {
                dialog.HeaderText = string.Format("Select the SQL Server instances you wish to apply the baseline configuration to.");
                dialog.HelpTopic = HelpTopics.ServerPropertiesBaselineApplyToOthers;

                if (ApplicationModel.Default.AllInstances.ContainsKey(instanceId))
                    dialog.InstancesToExclude.Add(ApplicationModel.Default.AllInstances[instanceId].InstanceName);

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    IList<MonitoredSqlServer> instances = dialog.AddedInstances;

                    foreach (MonitoredSqlServer instance in instances)
                    {
                        try
                        {
                            //START: SQLdm 10.0 (Tarun Sapra)- make a unified xml for all the config
                            string xmlString = Common.Configuration.BaselineConfiguration.SerializeAllBaselineConfigurations(instance.Id, baselineConfig, baselineConfigList);
                            StringWriter writer = new StringWriter();
                            // Decode the encoded string.
                            HttpUtility.HtmlDecode(xmlString, writer);
                            xmlString = writer.ToString();
                            xmlString = xmlString.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                            RepositoryHelper.SaveMultipleBaselineTemplate(instance.Id, xmlString);
                            //RepositoryHelper.SaveBaselineTemplate(instance.Id, baselineConfig);
                            //END: SQLdm 10.0 (Tarun Sapra)- make a unified xml for all the config

                            // Log audit event.
                            AuditableEntity entity = baselineConfig.GetAuditableEntity();
                            entity.Name = instance.InstanceName;

                            AuditingEngine.Instance.ManagementService =
                                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                            AuditingEngine.Instance.SQLUser =
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                                AuditingEngine.GetWorkstationUser() :
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
                            AuditingEngine.Instance.LogAction(entity, AuditableActionType.BaselineConfigurationChanged);
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this, "Error trying to save baseline config for " + instance.InstanceName, ex);
                            return; // abort
                        }
                    }
                }
            }



        }

        //START SQLdm 10.0 (Tarun Sapra) : Add event for BaselineAssistant button click 
        //private void btnBaseLineAssistant_Click(object sender, EventArgs e)
        //{
        //    BaselineAssistantDialog dialog = new BaselineAssistantDialog(instanceId);
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {

        //    } 
        //}
        //END SQLdm 10.0 (Tarun Sapra) : Add event for BaselineAssistant button click 



        //SQLDM-30848, adapting resolutions, Saurabh
        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isLargeSize)
            {
                this.baselineMainContainer.Size = new System.Drawing.Size(695, 800);
                this.btnSelectOtherServers.Size = new Size(this.btnSelectOtherServers.Width, this.btnSelectOtherServers.Height - 2);
                this.baselineMainContainer.AutoScroll = true;
                this.btnSelectOtherServers.Location = new Point(this.btnSelectOtherServers.Location.X, this.btnSelectOtherServers.Location.Y - 10);
                return;
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                this.baselineMainContainer.Size = new System.Drawing.Size(900, 1000);
                this.btnSelectOtherServers.Size = new Size(this.btnSelectOtherServers.Width, this.btnSelectOtherServers.Height - 2);
                this.baselineMainContainer.AutoScroll = true;
                return;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                this.baselineMainContainer.Size = new System.Drawing.Size(900, 1000);
                this.btnSelectOtherServers.Size = new Size(this.btnSelectOtherServers.Width, this.btnSelectOtherServers.Height - 2);
                this.baselineMainContainer.AutoScroll = true;
                return;
            }
        }
    }
}
