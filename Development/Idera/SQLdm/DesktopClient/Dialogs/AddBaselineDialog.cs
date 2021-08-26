using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;//SQLdm 10.0 (Tarun Sapra)- Added here to be able to use BaselineConfiguration type of objects
using Idera.SQLdm.Common.Objects;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Helpers;
using System.Xml;
using System.IO;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Baseline;


namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AddBaselineDialog : Form
    {
        //To log errors
        Logger Log = Logger.GetLogger("AddBaselineDialog");
        private Dictionary<int, BaselineConfiguration> baselineConfigList;
        //private BaselineConfiguration baselineConfig;
        private BaselineConfiguration backupCOnfigToChangeBaselineName = new BaselineConfiguration();
        private const string CHOOSE_DROPDOWN_OPTION = "Choose appropriate option from the dropdown";
        private bool isRefresh;
        private bool isInit;
        private bool isNew;
        public AddBaselineDialog(Dictionary<int, BaselineConfiguration> baselineConfigList)
        {
            this.baselineConfigList = baselineConfigList;
            InitializeComponent();
            InIt();
        }


        /// <summary>
        /// Initialize Controls with values
        /// SQLdm 10.1 Srishti Purohit
        /// </summary>
        private void InIt()
        {
            isInit = true;
            comboBaselineName.Items.Clear();
            //SQLdm 10.1 (pulkit puri)--shifting days according to time zone of desktop client
            //change the remote service existing baseline days according to desktop client
            foreach (var config in baselineConfigList.Values)
            {
                //original template xml is only for those xml which are retrieved from db
                if (config.DesktopUTCtimezoneOffset != null && config.OriginalTimeZoneTemplateXML != null)
                {
                    config.Template.ScheduledSelectedDays = config.GetDaysOfWeekAfterShiftScheduledBaseline(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
                    config.Template.CalculationSelectedDays = config.GetDaysOfWeekAfterShiftCaclulationBaseline(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
                    config.DesktopUTCtimezoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString();
                }
            }
            if (baselineConfigList != null)
            {
                foreach (var config in baselineConfigList.Values)
                {
                    if (config == null)
                    {
                        RefreshPanel(new BaselineConfiguration());
                    }

                    if (config != null && config.Active)
                    {
                        comboBaselineName.Items.Add(config);
                    }
                }
                comboBaselineName.Items.Add(new BaselineConfiguration(Constants.ADD_BASELINE_NAME, true));
                comboBaselineName.DisplayMember = "config.BaselineName";
                comboBaselineName.ValueMember = "config.TemplateID";
            }
            addButton.Enabled = true;
            isNew = addButton.Enabled;
            deleteButton.Enabled = !addButton.Enabled;
            isInit = false;
        }

        /// <summary>
        ///  Added a list for multiple BaselineConfiguration
        /// SQLdm 10.1 Srishti Purohit
        /// </summary>
        private void RefreshPanel(BaselineConfiguration config)
        {
            isRefresh = true;
            BaselineTemplate template = new BaselineTemplate();
            template = config.Template;
            if (config.IsScheduledNotFound && !config.IsChanged && config.TemplateID > 1)
            {
                MessageBox.Show(this, "No schedule found for baseline.");
            }
            #region Automatic/Custom date range

            automaticBaselineRadioButton.Checked = template.UseDefault;
            customBaselineRadioButton.Checked = !template.UseDefault;
            customBaselineRadioButton_CheckedChanged(customBaselineRadioButton, EventArgs.Empty);

            customDateFromCombo.Enabled = !template.UseDefault;
            customDateToCombo.Enabled = !template.UseDefault;

            customDateFromCombo.Value = template.CalculationStartDate.Date;
            customDateToCombo.Value = template.CalculationEndDate.Date;
            #endregion
            //Show Enpty string if no baseline is selected
            comboBaselineName.Text = config.BaselineName == Common.Constants.DEFAULT_BASELINE_NAME ? string.Empty : config.BaselineName;
            beginCalculationTimeCombo.Time = template.CalculationStartDate.TimeOfDay;
            endCalculationTimeCombo.Time = template.CalculationEndDate.TimeOfDay;

            sundayCheckbox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, template.CalculationSelectedDays);
            mondayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, template.CalculationSelectedDays);
            tuesdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, template.CalculationSelectedDays);
            wednesdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, template.CalculationSelectedDays);
            thursdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, template.CalculationSelectedDays);
            fridayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, template.CalculationSelectedDays);
            saturdayCheckBox1.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, template.CalculationSelectedDays);

            #region Scheduling
            //SQLdm 10.1.1 (srishti purohit)schedule is Not mandatory field as per 10.1.1
            if (config.IsScheduledNotFound == false)
            {
                beginScheduleTimeCombo.Time = template.ScheduledStartDate.Value.TimeOfDay;
                endScheduleTimeCombo.Time = template.ScheduledEndDate.Value.TimeOfDay;

            }
            else
            {
                beginScheduleTimeCombo.Enabled = false;
                endScheduleTimeCombo.Enabled = false;
            }
            sundayCheckbox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, template.ScheduledSelectedDays);
            mondayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, template.ScheduledSelectedDays);
            tuesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, template.ScheduledSelectedDays);
            wednesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, template.ScheduledSelectedDays);
            thursdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, template.ScheduledSelectedDays);
            fridayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, template.ScheduledSelectedDays);
            saturdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, template.ScheduledSelectedDays);
            #endregion
            isRefresh = false;
        }
        private void Update(object sender, EventArgs e)
        {
            try
            {
                if (isInit || isRefresh || isNew)
                {
                    short daysToEnableTimeControls = 0;

                    if (sundayCheckbox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                    if (mondayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                    if (tuesdayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                    if (wednesdayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                    if (thursdayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                    if (fridayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                    if (saturdayCheckBox.Checked) daysToEnableTimeControls |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

                    if (daysToEnableTimeControls == 0)
                    {
                        beginScheduleTimeCombo.Enabled = false;
                        endScheduleTimeCombo.Enabled = false;
                    }
                    else
                    {
                        beginScheduleTimeCombo.Enabled = true;
                        endScheduleTimeCombo.Enabled = true;
                    }
                    return;
                }
                if (CheckForErrors())
                {
                    this.DialogResult = DialogResult.None;
                    Log.Info("baseline control's value validation failed");
                    return;
                }

                Log.Info("Editing baseline configuration.");
                BaselineConfiguration config = new BaselineConfiguration();
                if (comboBaselineName.Items.Count > 0 && string.IsNullOrWhiteSpace(comboBaselineName.Text))//SQLdm 10.0 (Tarun Sapra)- Skipping the check if there is no custom baseline
                {
                    //ResetView();
                    MessageBox.Show(this, "BaselineName can't be empty", Common.Constants.INVALID_BASELINE_NAME);
                    return;
                }
                // BaselineConfiguration config1 = new BaselineConfiguration();
                BaselineConfiguration config2 = new BaselineConfiguration();
                // config1.BaselineName = baselineConfig.BaselineName;
                // config1.TemplateID = baselineConfig.TemplateID;
                // config1.Active = baselineConfig.Active;
                short days = 0;

                if (sundayCheckbox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                if (mondayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                if (tuesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                if (wednesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                if (thursdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                if (fridayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                if (saturdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

                config2.Template.ScheduledSelectedDays = days;
                if (config2.Template.ScheduledSelectedDays == 0)
                {
                    beginScheduleTimeCombo.Enabled = false;
                    endScheduleTimeCombo.Enabled = false;
                    config2.Template.ScheduledStartDate = null;
                    config2.Template.ScheduledEndDate = null;
                }
                else
                {
                    config2.Template.ScheduledStartDate = (Convert.ToDateTime(customDateFromCombo.Value)).Date + beginScheduleTimeCombo.Time;
                    config2.Template.ScheduledEndDate = (Convert.ToDateTime(customDateToCombo.Value)).Date + endScheduleTimeCombo.Time;
                    beginScheduleTimeCombo.Enabled = true;
                    endScheduleTimeCombo.Enabled = true;
                }

                if (comboBaselineName.Items.Count > 0)
                {
                    if (comboBaselineName.SelectedItem == null)
                    {
                        //SQLdm 10.1 (Srishti purohit)
                        //defect SQLDM-25914 fix. Baseline name edit. if we change text of combo then selectedItem will be lost so bakedupBefore text is changed.
                        config = backupCOnfigToChangeBaselineName;
                        config.BaselineName = comboBaselineName.Text;
                    }
                    else
                        config = (BaselineConfiguration)comboBaselineName.SelectedItem;
                    //if (config == null)
                    //{
                    //    config2.BaselineName = maxBaselineConfig.BaselineName;
                    //    config2.TemplateID = maxBaselineConfig.TemplateID;
                    //    config2.Active = maxBaselineConfig.Active;
                    //}
                    //else
                    //{
                    config2.BaselineName = config.BaselineName;
                    config2.TemplateID = config.TemplateID;
                    config2.Active = config.Active;
                    //}
                    days = 0;
                    if (sundayCheckbox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                    if (mondayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                    if (tuesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                    if (wednesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                    if (thursdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                    if (fridayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                    if (saturdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
                    config2.Template.CalculationSelectedDays = days;
                    config2.BaselineName = comboBaselineName.Text;
                    config2.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + beginCalculationTimeCombo.Time;
                    config2.Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + endCalculationTimeCombo.Time;
                    /*  if (CheckOverlap(config2, config1))
                      {
                          return;
                      }
                      foreach (int key in baselineConfigList.Keys)
                      {
                          if (baselineConfigList[key].Active == true)//SQLdm 10.1 (Pulkit Puri)
                          {
                              if (CheckOverlap(config1, baselineConfigList[key]))
                              {
                                  return;
                              }
                          }
                      }*/
                    foreach (int key in baselineConfigList.Keys)
                    {
                        if (key != config2.TemplateID && baselineConfigList[key].Active == true)//SQLdm 10.1 (Pulkit Puri)
                            if (CheckOverlap(config2, baselineConfigList[key]))
                            {
                                return;
                            }
                    }
                    baselineConfigList[config2.TemplateID].Template.CalculationSelectedDays = config2.Template.CalculationSelectedDays;
                    baselineConfigList[config2.TemplateID].Template.CalculationStartDate = config2.Template.CalculationStartDate;
                    baselineConfigList[config2.TemplateID].Template.CalculationEndDate = config2.Template.CalculationEndDate;
                    baselineConfigList[config2.TemplateID].BaselineName = config2.BaselineName;
                    baselineConfigList[config2.TemplateID].Template.UseDefault = automaticBaselineRadioButton.Checked;

                    baselineConfigList[config2.TemplateID].Template.ScheduledSelectedDays = config2.Template.ScheduledSelectedDays;
                    baselineConfigList[config2.TemplateID].Template.ScheduledStartDate = config2.Template.ScheduledStartDate;
                    baselineConfigList[config2.TemplateID].Template.ScheduledEndDate = config2.Template.ScheduledEndDate;
                    baselineConfigList[config2.TemplateID].IsChanged = true;
                    config2.IsChanged = true;
                    if (config != null)
                    {
                        comboBaselineName.Items.Remove(config);
                        comboBaselineName.Items.Insert(0, baselineConfigList[config2.TemplateID]);
                        RefreshPanel(baselineConfigList[config2.TemplateID]);
                    }
                    //else
                    //{
                    //    comboBaselineName.Items.Remove(maxBaselineConfig);
                    //    comboBaselineName.Items.Insert(0, baselineConfigList[config2.TemplateID]);
                    //    RefreshPanel(baselineConfigList[config2.TemplateID]);
                    //}
                }

                //SQLDM 10.1 (Pulkit Puri)--Commenting below code as we cannot modify default baseline config in addbaseline dialog

                // baselineConfig.Template.CalculationSelectedDays = config1.Template.CalculationSelectedDays;
                // baselineConfig.Template.CalculationStartDate = config1.Template.CalculationStartDate;
                //baselineConfig.Template.CalculationEndDate = config1.Template.CalculationEndDate;

                foreach (int key in baselineConfigList.Keys)
                {
                    baselineConfigList[key].Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + baselineConfigList[key].Template.CalculationStartDate.TimeOfDay;
                    baselineConfigList[key].Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + baselineConfigList[key].Template.CalculationEndDate.TimeOfDay;
                }
                //Log.Info("Baseline config {0} updated successfully.", comboBaselineName.Text);
                //MessageBox.Show("Baseline config " + comboBaselineName.Text + " updated successfully.", "Baseline Configuration Update");
            }
            catch (Exception ex)
            {
                Log.Error("Exception while editing baseline configuration : ", ex);
                MessageBox.Show("Baseline configuration " + comboBaselineName.Text + " updation failed.", "Baseline Configuration Update");
            }
        }

        private void comboBaselineName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            BaselineConfiguration config = (BaselineConfiguration)box.SelectedItem;
            backupCOnfigToChangeBaselineName = config;
            if (config == null)
                config = new BaselineConfiguration();
            RefreshPanel(config);
            if (config.TemplateID == 0)
            {
                addButton.Enabled = true;
            }
            else
                addButton.Enabled = false;
            isNew = addButton.Enabled;
            deleteButton.Enabled = !addButton.Enabled;
        }
        private void automaticBaselineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (isInit)
                return;
            //SQLDM 10.1 (Pulkit Puri)--Commenting below code as we cannot modify default baseline config in addbaseline dialog
            // baselineConfig.Template.UseDefault = automaticBaselineRadioButton.Checked;
            // baselineConfig.Template.UseDefault = automaticBaselineRadioButton.Checked;
            // baselineConfig.IsChanged = true;
        }
        private void customBaselineRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (isInit)
                return;
            customDateFromLabel.Enabled =
            customDateFromCombo.Enabled =
            customDateToLabel.Enabled =
            customDateToCombo.Enabled = customBaselineRadioButton.Checked;

            //SQLDM 10.1 (Pulkit Puri)--Commenting below code as we cannot modify default baseline config in addbaseline dialog
            //baselineConfig.Template.UseDefault = !customBaselineRadioButton.Checked;
            // baselineConfig.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + baselineConfig.Template.CalculationStartDate.TimeOfDay;
            // baselineConfig.Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + baselineConfig.Template.CalculationEndDate.TimeOfDay;
        }

        //START: SQLdm 10.0 (Tarun Sapra) (Baseline Validation)- Validating the baseline in the Add Baseline dialog only
        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckForErrors())
                    this.DialogResult = DialogResult.None;
                else
                {

                    if (string.IsNullOrWhiteSpace(comboBaselineName.Text))
                    {
                        //ResetView();
                        MessageBox.Show(this, "BaselineName can't be empty", Common.Constants.INVALID_BASELINE_NAME);
                        return;
                    }

                    //START: SQLdm 10.0 (Tarun Sapra)- Custom baseline can not have name as "Default"
                    if (comboBaselineName.Text == Constants.DEFAULT_BASELINE_NAME)
                    {
                        //ResetView();
                        MessageBox.Show(this, "Baseline with same name exists", Common.Constants.INVALID_BASELINE_NAME);
                        return;
                    }
                    //END: SQLdm 10.0 (Tarun Sapra)- Custom baseline can not have name as "Default"



                    BaselineConfiguration config = new BaselineConfiguration();
                    config.DesktopUTCtimezoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString();
                    config.BaselineName = comboBaselineName.Text;
                    short days = 0;
                    if (sundayCheckbox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                    if (mondayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                    if (tuesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                    if (wednesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                    if (thursdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                    if (fridayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                    if (saturdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
                    config.Template.CalculationSelectedDays = days;
                    config.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + beginCalculationTimeCombo.Time;
                    config.Template.CalculationEndDate = ((DateTime)customDateToCombo.Value).Date + endCalculationTimeCombo.Time;

                    //4.1.12 Revise Multiple Baseline for Independent Scheduling
                    //SQLdm 10.1 (Srishti Purohit)
                    days = 0;
                    if (sundayCheckbox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                    if (mondayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                    if (tuesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                    if (wednesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                    if (thursdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                    if (fridayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                    if (saturdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
                    if (days != 0)
                    {
                        config.Template.ScheduledSelectedDays = days;
                        config.Template.ScheduledStartDate = ((DateTime)customDateFromCombo.Value).Date + beginScheduleTimeCombo.Time;
                        config.Template.ScheduledEndDate = ((DateTime)customDateToCombo.Value).Date + endScheduleTimeCombo.Time;
                    }
                    //SQLDM 10.1 (Pulkit Puri)--Commenting below code as we cannot modify default baseline config in addbaseline dialog
                    // if (CheckOverlap(config, baselineConfig))
                    //{
                    //    return;
                    //}
                    int minTemplateID = Int32.MaxValue;
                    foreach (BaselineConfiguration conf in baselineConfigList.Values)
                    {
                        if (minTemplateID > conf.TemplateID)
                        {
                            minTemplateID = conf.TemplateID;
                        }
                        if (conf.Active == true)//SQLdm 10.1 (Pulkit Puri)--SQldm -SQLDM-25974 fix
                        {
                            if (CheckOverlap(config, conf))
                            {
                                return;
                            }
                        }
                        //Removing extra text SQLdm 10.1 (Pulkit Puri)
                    }
                    comboBaselineName.DisplayMember = "config.BaselineName";
                    comboBaselineName.ValueMember = "config.TemplateID";
                    if (minTemplateID > 0)
                        minTemplateID = 0;
                    config.TemplateID = --minTemplateID;
                    baselineConfigList.Add(config.TemplateID, config);
                    comboBaselineName.Items.Insert(0, config);
                    RefreshPanel(config);
                    config.Active = true;//SQLdm 10.1 (Pulkit Puri)--Active flag must be true after baseline is added
                    //maxBaselineConfig = config;
                    Log.Info("Baseline config " + comboBaselineName.Text + " added successfully.");
                    MessageBox.Show("Baseline configuration " + comboBaselineName.Text + " added successfully.", "New Baseline Configuration");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Baseline config " + comboBaselineName.Text + " addition failed.", ex);
                MessageBox.Show("New Baseline configuration " + comboBaselineName.Text + " addition failed.", "New Baseline Configuration");
            }
        }
        //private void editButton_Click(object sender, EventArgs e)
        //{
        //    Update(sender, e);
        //}
        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                BaselineConfiguration config = (BaselineConfiguration)comboBaselineName.SelectedItem;

                if (deleteBaseline(config.TemplateID))
                {
                    //SQLdm 10.1 (Pulkit Puri)
                    //SQLDM-26278 -- If newly added baseline is getting added no need to save it
                    if (config.TemplateID < 0)
                        baselineConfigList.Remove(config.TemplateID);
                    InIt();
                    RefreshPanel(new BaselineConfiguration());
                    Log.Info("Baseline configuration {0} deleted.", config.BaselineName);
                    MessageBox.Show("Baseline configuration " + config.BaselineName + " deleted successfully.", "Baseline Configuration Delete");
                    config.Active = false;//SQLdm 10.1 (Pulkit Puri)--Active flag must be false after baseline is deleted
                }
                else
                {
                    MessageBox.Show("Baseline configuration " + comboBaselineName.Text + " could not be deleted.", "Baseline Configuration Delete");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Baseline configuration {0} could not deleted.", comboBaselineName.Text, ex);
                MessageBox.Show("Baseline configuration " + comboBaselineName.Text + " could not be deleted.", "Baseline Configuration Delete");
            }
        }

        private bool deleteBaseline(int templateID)
        {
            bool isBaselineDeleted = false;
            try
            {
                baselineConfigList[templateID].Active = false;
                baselineConfigList[templateID].IsChanged = true;
                isBaselineDeleted = true;

            }
            catch (Exception ex)
            {
                Log.Error("Baseline configuration {0} could not deleted.", comboBaselineName.Text, ex);
                isBaselineDeleted = false;
            }
            return isBaselineDeleted;
        }


        private bool CheckForErrors()
        {
            ///SQLdm(10.1) Srishti Purohit -- Checking time range if Time is crossing mid night
            //if (this.beginTimeCombo1.Time > this.endTimeCombo1.Time)
            //{
            //    MessageBox.Show(this, "The 'from' time must be before the 'to' time for the baseline period.", "Invalid Baseline Configuration");
            //    return true;
            //}
            //if (this.beginTimeCombo.Time > this.endTimeCombo.Time)
            //{
            //    MessageBox.Show(this, "The 'from' time must be before the 'to' time for the baseline schedule period.", "Invalid Baseline Configuration");
            //    return true;
            //}

            if (string.IsNullOrWhiteSpace(this.comboBaselineName.Text))
            {
                MessageBox.Show(this, "BaselineName can't be empty", Common.Constants.INVALID_BASELINE_NAME);
                return true;
            }
            //SQLdm 10.1 (Pulkit Puri)--remove case insensitivity
            if (string.Equals(this.comboBaselineName.Text, Common.Constants.DEFAULT_BASELINE_NAME, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show(this, "Baseline with same name exists", Common.Constants.INVALID_BASELINE_NAME);
                return true;
            }
            if (this.comboBaselineName.Text == Constants.ADD_BASELINE_NAME)
            {
                MessageBox.Show(this, "Baseline can't be " + Constants.ADD_BASELINE_NAME, Common.Constants.INVALID_BASELINE_NAME);
                return true;
            }
            //SQLdm  10.1 (Pulkit Puri) -- adding validation for baseline name
            if (this.comboBaselineName.Text == CHOOSE_DROPDOWN_OPTION)
            {
                MessageBox.Show(this, "Baseline can't be " + Constants.ADD_BASELINE_NAME, Common.Constants.INVALID_BASELINE_NAME);//SQLdm 10.1 (review fix)
                return true;
            }
            //SQLdm  10.1 (Pulkit Puri) -- adding validation of comma
            if (this.comboBaselineName.Text.Contains(","))
            {
                MessageBox.Show(this, "Baseline name is invalid. ',' in baseline name is not allowed.");
                return true;
            }

            BaselineConfiguration newConfig = new BaselineConfiguration();
            newConfig.BaselineName = this.comboBaselineName.Text;//this.textBox1.Text;
            short days = 0;
            if (this.sundayCheckbox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (this.mondayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (this.tuesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (this.wednesdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (this.thursdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (this.fridayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (this.saturdayCheckBox1.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
            newConfig.Template.CalculationSelectedDays = days;
            newConfig.Template.CalculationStartDate = ((DateTime)customDateFromCombo.Value).Date + this.beginCalculationTimeCombo.Time;
            newConfig.Template.CalculationEndDate = ((DateTime)customDateFromCombo.Value).Date + this.endCalculationTimeCombo.Time;

            days = 0;
            if (this.sundayCheckbox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (this.mondayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (this.tuesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (this.wednesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (this.thursdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (this.fridayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (this.saturdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);
            newConfig.Template.ScheduledSelectedDays = days;
            newConfig.Template.ScheduledStartDate = ((DateTime)customDateFromCombo.Value).Date + this.beginCalculationTimeCombo.Time;
            newConfig.Template.ScheduledEndDate = ((DateTime)customDateFromCombo.Value).Date + this.endCalculationTimeCombo.Time;

            //SQLDM 10.1 (Pulkit Puri)--Commenting below code as we cannot modify default baseline config in addbaseline dialog
            //if (CheckOverlap(newConfig, baselineConfig) == true)
            //   return true;

            //Commenting below code as per the new 10.1 requirement overlapping between two custom baseline config is allowed
            //10.1 SQLdm (Srishti Purohit)
            //int minTemplateID = Int32.MaxValue;
            //foreach (BaselineConfiguration conf in baselineConfigList.Values)
            //{
            //    if (minTemplateID > conf.TemplateID)
            //    {
            //        minTemplateID = conf.TemplateID;
            //    }
            //    if (CheckOverlap(newConfig, conf) == true)
            //        return true;
            //}

            if (newConfig.Template.CalculationSelectedDays == 0)
            {
                MessageBox.Show("The days of the week to include in the baseline must be selected.", "Invalid Baseline Configuration");
                return true;
            }
            //SQLDM-10.1 SQLDM-26048 FIX (Pulkit puri)
            if (((DateTime)customDateFromCombo.Value) > ((DateTime)customDateToCombo.Value))
            {
                MessageBox.Show(Common.Constants.BASELINE_DATE_VALIDATION_MSG);
                return true;
            }
            return false;
        }
        /// <summary>
        ///  Function to check whether the baseline is crossing midnight
        ///  SQLdm 10.1 (Pulkit Puri)
        /// </summary>
        private bool IsMidnightCrossedForScheduledTime(BaselineConfiguration config)
        {
            try
            {
                if (config.Template.ScheduledStartDate != null && config.Template.ScheduledStartDate.Value.TimeOfDay >= config.Template.ScheduledEndDate.Value.TimeOfDay)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while checking midnight crossing in baseline", ex);
                return false;
            }
        }
        /// <summary>
        ///  Function to check whether the day overlapping 
        ///  config1 would be the first and config2 would be next day if bool is false
        ///  bool IsDayOverlap would decide that overlap check
        ///  SQLdm 10.1 (Pulkit Puri)
        /// </summary>
        private bool CheckDayOverlapForScheduledTime(BaselineConfiguration config1, BaselineConfiguration config2, bool IsDayOverlap)
        {
            try
            {
                //If check is to be on previous day then IsSameDay would be false 
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    DayOfWeek daycheck = day;
                    if (!IsDayOverlap)
                    {
                        if (day == DayOfWeek.Saturday) daycheck = DayOfWeek.Sunday;
                        else daycheck = day + (int)BaselineDayShift.PostShift;
                    }
                    if (MonitoredSqlServer.MatchDayOfWeek(day, config1.Template.ScheduledSelectedDays)
                        && MonitoredSqlServer.MatchDayOfWeek(daycheck, config2.Template.ScheduledSelectedDays))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while checking day overlap in baseline", ex);
                return false;
            }
        }
        /// <summary>
        ///  Function to check whether the time is overlapping 
        ///  SQLdm 10.1 (Pulkit Puri)
        /// </summary>
        private bool CheckTimeOverlapForScheduledTime(BaselineConfiguration config1, BaselineConfiguration config2)
        {
            try
            {
                if (config1.IsScheduledNotFound && config2.IsScheduledNotFound)
                {
                    Log.Info("Baseline custom: No schedule found for baseline : " + config2.BaselineName);
                    return false;
                }
                //case 1: Both the baselines cross the midnight
                if (IsMidnightCrossedForScheduledTime(config1) && IsMidnightCrossedForScheduledTime(config2))
                {
                    if (config1.Template.ScheduledEndDate.Value.TimeOfDay > config2.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config1, config2, false)) return true;
                    }
                    if (config2.Template.ScheduledEndDate.Value.TimeOfDay > config1.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config2, config1, false)) return true;
                    }
                    if (CheckDayOverlapForScheduledTime(config1, config2, true)) return true;
                }
                // case2 : only baseline2 is crossing midnight
                if (!(IsMidnightCrossedForScheduledTime(config1)) && IsMidnightCrossedForScheduledTime(config2))
                {
                    if (config1.Template.ScheduledEndDate.Value.TimeOfDay > config2.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config1, config2, true)) return true; ;
                    }
                    if (config2.Template.ScheduledEndDate.Value.TimeOfDay > config1.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config2, config1, false)) return true;
                    }
                }
                // case3 : only baseline1 is crossing midnight
                if (!(IsMidnightCrossedForScheduledTime(config2)) && IsMidnightCrossedForScheduledTime(config1))
                {
                    if (config2.Template.ScheduledEndDate.Value.TimeOfDay > config1.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config2, config1, true)) return true;
                    }
                    if (config1.Template.ScheduledEndDate.Value.TimeOfDay > config2.Template.ScheduledStartDate.Value.TimeOfDay)
                    {
                        if (CheckDayOverlapForScheduledTime(config1, config2, false)) return true;
                    }
                }
                // case4 : none of the baseline is crossing midnight
                if (config1.Template.ScheduledStartDate.Value.TimeOfDay < config2.Template.ScheduledEndDate.Value.TimeOfDay
                 && config2.Template.ScheduledStartDate.Value.TimeOfDay < config1.Template.ScheduledEndDate.Value.TimeOfDay)
                {
                    if (CheckDayOverlapForScheduledTime(config2, config1, true)) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while checking time overlap in baseline", ex);
                return false;
            }
        }


        private bool CheckOverlap(BaselineConfiguration config1, BaselineConfiguration config2)
        {
            //SQLDM 10.1 (Pulkit Puri)-- remove case insentivity
            if (string.Equals(config1.BaselineName, config2.BaselineName, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show(this, "Baseline with same name exists", "Same Name Baseline Detected");
                return true;
            }
            //Default calculation time can also be overlapped with other baseline templates
            //if (config2.BaselineName == DEFAULT_BASELINE_NAME)
            //{
            //    foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
            //    {
            //        if (MonitoredSqlServer.MatchDayOfWeek(d, config1.Template.SelectedDays) && MonitoredSqlServer.MatchDayOfWeek(d, config2.Template.SelectedDays))
            //        {
            //            if (config1.Template.StartDate.TimeOfDay < config2.Template.EndDate.TimeOfDay && config2.Template.StartDate.TimeOfDay < config1.Template.EndDate.TimeOfDay)
            //            {
            //                MessageBox.Show(this, String.Format("Baseline {0} overlaps with baseline {1}", config1.BaselineName, config2.BaselineName), "Overlapping Detected");
            //                return true;
            //            }
            //        }
            //    }
            //}
            //else
            if (config2.BaselineName != Common.Constants.DEFAULT_BASELINE_NAME && config1.BaselineName != Common.Constants.DEFAULT_BASELINE_NAME)
            {
                //SQLdm 10.1 (pulkit Puri)-- for checking basleine overlap for 360 degree clock
                if (CheckTimeOverlapForScheduledTime(config1, config2))
                {
                    MessageBox.Show(this, String.Format("Baseline Scheduling {0} overlaps with baseline {1}", config1.BaselineName, config2.BaselineName), "Overlapping Schedule Detected");
                    return true;
                }
            }
            return false;
        }
        //END: SQLdm 10.0 (Tarun Sapra) (Baseline Validation)- Validating the baseline in the Add Baseline dialog only

        //SQLDM-25971-Wiki help link missing in 'Manage Baseline' window
        private void ManageBaseline_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerPropertiesBaseline);
        }
    }

}
