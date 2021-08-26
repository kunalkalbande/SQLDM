namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Notification;
    using System.ComponentModel;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class StateChangeDialog : BaseDialog
    {
        public MetricStateChangeRule rule;

        public StateChangeDialog()
        {
            this.DialogHeader = "State Change";
            InitializeComponent();
            AdaptFontSize();
        }

        public MetricStateChangeRule Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        private void StateChangeDialog_Load(object sender, EventArgs e)
        {
            previousListView.Items.Clear();
            AddListViewItem(MonitoredState.OK, previousListView, rule.PreviousState);
            AddListViewItem(MonitoredState.Warning, previousListView, rule.PreviousState);
            AddListViewItem(MonitoredState.Critical, previousListView, rule.PreviousState);

            newListView.Items.Clear();
            AddListViewItem(MonitoredState.OK, newListView, rule.NewState);
            AddListViewItem(MonitoredState.Warning, newListView, rule.NewState);
            AddListViewItem(MonitoredState.Critical, newListView, rule.NewState);

            UpdateForm();
        }

        private void AddListViewItem(MonitoredState state, ListView listView, IList<MonitoredState> selected)
        {
            ListViewItem item = new ListViewItem(state.ToString("F"));
            item.Tag = state;
            item.Checked = selected.Contains(state);
            listView.Items.Add(item);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            rule.NewState = GetSelected(newListView);
            rule.PreviousState = GetSelected(previousListView);
        }

        private bool IsValid() {
            // both lists must have a checked item
            IList<MonitoredState> prevSelected = GetSelected(previousListView);
            if (prevSelected.Count == 0)
                return false;

            IList<MonitoredState> newSelected = GetSelected(newListView);
            if (newSelected.Count == 0)
                return false;

            // if either list has only 1 checked item, that item must
            // not be checked in the other list
            if (newSelected.Count == 1 || prevSelected.Count == 1)
            {
                if (newSelected.Count == 1)
                    return !prevSelected.Contains(newSelected[0]);

                return !newSelected.Contains(prevSelected[0]);
            }
            return true;
        }

        private List<MonitoredState> GetSelected(ListView listView)
        {
            List<MonitoredState> result = new List<MonitoredState>();
            foreach (ListViewItem item in listView.CheckedItems)
            {
                result.Add((MonitoredState) item.Tag);
            }
            return result;
        }

        private void previousListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateForm();
        }

        private void newListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            btnOK.Enabled = IsValid();
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
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