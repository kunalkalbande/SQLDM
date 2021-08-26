using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Data.SqlClient;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.DesktopClient.Controls;
    using Infragistics.Windows.Themes;
    using Properties;

    public partial class BaselineWizard : Form
    {
        private SqlConnectionInfo connectionInfo;
        private Dictionary<int,BaselineMetaDataItem> metaData;
        private MonitoredSqlServer instance;
        private DateTime startTime;
        private DateTime endTime;
        private DateTime collectionTime;

        private List<BaselineItemData> baseline;

        private BaselineWizard(int instanceId)
        {
            InitializeComponent();
            alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            instance = ApplicationModel.Default.ActiveInstances[instanceId];
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.alertsGrid);
        }

        public static DialogResult Show(IWin32Window owner, int instanceId, ref DateTime?  StartTime, ref DateTime? EndTime, out List<BaselineItemData> baselineData)
        {
            DialogResult result = DialogResult.None;
            using (BaselineWizard wizard = new BaselineWizard(instanceId))
            {
                if (EndTime == null)
                {
                    DateTime time = DateTime.Now;
                    time = time.Date + TimeSpan.FromHours(time.TimeOfDay.Hours);
                    wizard.endTime = time;
                }
                else
                    wizard.endTime = EndTime.Value;

                if (StartTime == null || StartTime.Value > wizard.endTime)
                {
                    wizard.startTime = wizard.endTime - TimeSpan.FromDays(14);
                }
                else
                    wizard.startTime = StartTime.Value;

                result = wizard.ShowDialog(owner);
                if (result == DialogResult.OK)
                {
                    StartTime = wizard.startTime;
                    EndTime = wizard.endTime;
                    baselineData = wizard.baseline;
                } else
                {
                    StartTime = EndTime = null;
                    baselineData = null;
                }
            }
            return result;
        }

        private void BaselineWizard_Load(object sender, EventArgs e)
        {
            if (instance == null)
            {
                ApplicationMessageBox.ShowError(Owner,
                                                "Unable to find monitored instance.  Someone may have deleted the monitored instance from a different desktop client.");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }
            this.Text = String.Format(this.Text, instance.InstanceName);
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
        }

        private void introductionPage1_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void configPage_BeforeDisplay(object sender, EventArgs e)
        {
            startDateEditor.DateTime = startTime;
            startTimeEditor.Time = startTime.TimeOfDay;
            endDateEditor.DateTime = endTime;
            endTimeEditor.Time = endTime.TimeOfDay;
        }

        private void waitPage_BeforeDisplay(object sender, EventArgs e)
        {
            startTime = startDateEditor.DateTime.Date + startTimeEditor.Time;
            endTime = endDateEditor.DateTime.Date + endTimeEditor.Time;

            loadingCircle1.Active = true;
  
            backgroundWorker.RunWorkerAsync();
        }

        private void wizard1_Cancel(object sender, EventArgs e)
        {
            cancelBackgroundOperation();
        }

        private void waitPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            cancelBackgroundOperation();
        }

        private void waitPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            loadingCircle1.Active = false;
        }

        private void cancelBackgroundOperation()
        {
            loadingCircle1.Active = false;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            //List<BaselineItemData> data = null;

            using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                if (metaData == null)
                {
                    List<BaselineMetaDataItem> metaDataList = BaselineHelpers.GetBaselineMetaData(connection);
                    metaData = new Dictionary<int, BaselineMetaDataItem>();
                    foreach (BaselineMetaDataItem item in metaDataList)
                    {
                        metaData.Add(item.ItemId, item);
                    }
                }

                DateTime cdt = DateTime.UtcNow;
     //           e.Result = BaselineHelpers.ExecuteBaselineQuery(connection, instance.Id, startTime.ToUniversalTime(), endTime.ToUniversalTime(), cdt, metaData);
                collectionTime = cdt;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Exception error = e.Error;
            if (error != null)
            {
                ApplicationMessageBox.ShowError(this, error);
                wizard1.SelectedPage = configPage;
                return;
            }
            if (e.Cancelled)
                return;

            bindingSource.Clear();
            baseline = e.Result as List<BaselineItemData>;
            if (baseline != null)
            {
                foreach (BaselineItemData item in baseline)
                {
                    bindingSource.Add(item);
                }
            }

            wizard1.GoNext();
        }



        private void wizard1_Finish(object sender, EventArgs e)
        {
           
            // write the baseline to the repository
            using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
 //               BaselineHelpers.SetBaseline(connection, instance.Id, startTime, endTime, collectionTime, baseline);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}