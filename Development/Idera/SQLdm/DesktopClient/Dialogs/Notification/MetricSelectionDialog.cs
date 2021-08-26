using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.DesktopClient.Properties;
    using System.ComponentModel;

    public partial class MetricSelectionDialog : Form
    {
        private List<int> metrics;
        private List<int> selectedMetrics;
        private readonly MetricDefinitions metricDefinitions;
        private bool inUpdateButtons;
        private bool inSelectAll;

        public MetricSelectionDialog(MetricDefinitions metricDefinitions)
        {
            this.metricDefinitions = metricDefinitions;
            InitializeComponent();
            AdaptFontSize();
        }

        public List<int> Metrics
        {
            get
            {
                if (metrics == null)
                    metrics = new List<int>();
                return metrics;
            }
            set
            {
                metrics = value;
            }
        }

        public List<int> SelectedMetrics
        {
            get { return selectedMetrics; }
            set { selectedMetrics = value; }
        }

        private void MetricSelectionDialog_Load(object sender, EventArgs e)
        {
            metricsListView_SizeChanged(metricsListView, EventArgs.Empty);

            IList<int> selected = SelectedMetrics;
            metricsListView.Items.Clear();

            MetricDescription? metricDescription = null;
            foreach (int metricID in metricDefinitions.GetMetricDefinitionKeys())
            {
                switch (metricID)
                {
                    case (int)Metric.IndexRowHits:          // visual only
                    case (int)Metric.FullTextRefreshHours:  // visual only
                    case (int)Metric.MaintenanceMode:       // no notifications
                    case (int)Metric.Operational:           // no notifications 
                        continue;
                }
                metricDescription = metricDefinitions.GetMetricDescription(metricID);
                if (metricDescription.HasValue)
                {
                    ListViewItem item = new ListViewItem(metricDescription.Value.Name);
                    item.Tag = metricID;
                    item.Checked = selected.Contains(metricID);
                    metricsListView.Items.Add(item);
                }
            }

            UpdateButtons();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IList<int> selected = SelectedMetrics;
            selected.Clear();
            foreach (ListViewItem item in metricsListView.CheckedItems)
            {
                selected.Add((int)item.Tag);
            }
        }

        private void UpdateButtons()
        {
            if (inSelectAll) { return; }
            inUpdateButtons = true;
            btnOK.Enabled = metricsListView.CheckedItems.Count > 0;
            chkbxSelectAll.Checked = metricsListView.Items.Count == metricsListView.CheckedItems.Count;
            inUpdateButtons = false;
        }

        private void metricsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //            ListViewItem checkedItem = e.Item;
            //
            //            // only allow one item to be checked at a time
            //            if (metricsListView.CheckedItems.Count > 1)
            //            {
            //                foreach (ListViewItem item in Collections.ToArray<ListViewItem>(metricsListView.CheckedItems))
            //                {
            //                    if (item != e.Item)
            //                        item.Checked = false;
            //                }
            //            }

            UpdateButtons();
        }

        private void metricsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //            if (metricsListView.SelectedItems.Count > 0)
            //            {
            //                ListViewItem selectedItem = metricsListView.SelectedItems[0];
            //                if (!selectedItem.Checked)
            //                {
            //                    foreach (ListViewItem item in Collections.ToArray<ListViewItem>(metricsListView.CheckedItems))
            //                    {
            //                        item.Checked = false;
            //                    }
            //                    selectedItem.Checked = true;
            //                }
            //            }
            UpdateButtons();
        }

        private void metricsListView_DoubleClick(object sender, EventArgs e)
        {
            if (metricsListView.SelectedItems.Count > 0)
            {
                btnOK.PerformClick();
            }
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        private void metricsListView_SizeChanged(object sender, EventArgs e)
        {
            int width = metricsListView.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 5;
            if (width > 280)
                metricsListView.Columns[0].Width = width;
        }

        private void chkbxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (inUpdateButtons) { return; }
            inSelectAll = true;
            bool isSelected = chkbxSelectAll.Checked;
            int cnt = metricsListView.Items.Count;
            for (int i = 0; i < cnt; ++i)
            {
                metricsListView.Items[i].Checked = isSelected;
            }
            btnOK.Enabled = isSelected;
            inSelectAll = false;
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}