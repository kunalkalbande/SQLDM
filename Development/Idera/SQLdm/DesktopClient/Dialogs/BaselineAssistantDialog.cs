//SQLdm 10.0 (Tarun Sapra) - Displaying a dialog for the baseline definition assistant 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using ChartFX.WinForms;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class BaselineAssistantDialog : BaseDialog
    {
        private Dictionary<string, Tuple<int, string>> dict = new Dictionary<string, Tuple<int, string>>();
        private int instanceId=-1;

        public BaselineAssistantDialog(int instanceId)
        {
            this.DialogHeader = "Baseline Definition Visualizer";
            InitializeComponent();

            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;//disabling the resizing of dialog

            this.instanceId = instanceId;
            dict = RepositoryHelper.GetMetricList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            string[] array = dict.Keys.ToArray();
            this.SelectNumericMertic.Items.AddRange(array);

            chart.AxisX.LabelsFormat.Format = AxisFormat.Time;
            this.chart.Data.Clear();
        }

        public void DropDownSelectionChanged(object sender, EventArgs e)
        {
            if (this.SelectNumericMertic.SelectedItem == null || this.SelectNoOfWeeks.SelectedItem == null)
                return;
            this.chart.Data.Clear();
            this.chart.DataSourceSettings.Fields.Clear();
            int itemId = dict[this.SelectNumericMertic.SelectedItem.ToString()].Item1;
            int weekCount = Convert.ToInt32(this.SelectNoOfWeeks.SelectedItem);
            this.chart.AxisY.Title.Text = dict[this.SelectNumericMertic.SelectedItem.ToString()].Item2;//unit name on y-axis

            //START: Labelling the x-axis 
            this.Monday.Text = "Monday";
            this.Tuesday.Text = "Tuesday";
            this.Wednesday.Text = "Wednesday";
            this.Thrusday.Text = "Thursday";
            this.Friday.Text = "Friday";
            this.Saturday.Text = "Saturday";
            this.Sunday.Text = "Sunday";
            //END: Labelling the x-axis 

            DataTable result= RepositoryHelper.GetDataForBDA(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, itemId, weekCount);

            if (result.Rows.Count > 0)
            {
                FieldMap date = new FieldMap("Date", FieldUsage.XValue);
                this.chart.DataSourceSettings.Fields.Add(date);

                for (int i = 0; i < weekCount; i++)
                {
                    FieldMap temp = new FieldMap("Week"+(i+1).ToString(), FieldUsage.Value);
                    this.chart.DataSourceSettings.Fields.Add(temp);
                }
            }
            this.chart.DataSource = result;
        }



        //Added event handlers for help link in baseline assistant-- SQLdm 10.0 (Ankit Nagpal)

         private void BaselineAssistantDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        private void BaselineAssistantDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        private void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.BaselineAssistant);
        }

    }
}
