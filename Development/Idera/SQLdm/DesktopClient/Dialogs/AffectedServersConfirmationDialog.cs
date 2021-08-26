using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AffectedServersConfirmationDialog: Form
    {
        private string helpLink = String.Empty;

        public AffectedServersConfirmationDialog()
        {
            InitializeComponent();
            AdaptFontSize();
        }

        public string InfoText
        {
            get { return informationBox.Text; }
            set { informationBox.Text = value; }
        }

        public string NoInstancesText
        {
            get { return noneSelectedLabel.Text; }
            set { noneSelectedLabel.Text = value; }
        }

        public void SetTagsAndInstances(IEnumerable<int> tags, IEnumerable<int> instances)
        {
            serverList.BeginUpdate();

            foreach (int tagId in tags)
            {
                if (ApplicationModel.Default.Tags.Contains(tagId))
                {
                    Tag tag = ApplicationModel.Default.Tags[tagId];

                    foreach (int instanceId in tag.Instances)
                    {
                        if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                        {
                            MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
                            StringBuilder itemName = new StringBuilder(instance.InstanceName);
                            itemName.Append(" - [");
                            itemName.Append(tag.Name);
                            itemName.Append("]");
                            serverList.Items.Add(itemName.ToString());
                        }
                    }
                }
            }

            foreach (int instanceID in instances)
            {
                if (ApplicationModel.Default.ActiveInstances.Contains(instanceID))
                {
                    serverList.Items.Add(ApplicationModel.Default.ActiveInstances[instanceID].InstanceName);
                }
            }

            serverList.EndUpdate();
            stackLayoutPanel1.ActiveControl = serverList;

            if (serverList.Items.Count == 0)
            {
                stackLayoutPanel1.ActiveControl = noneSelectedLabel;
            }
        }

        public string HelpLink
        {
            get { return helpLink; }
            set
            {
                helpLink = value;
                HelpButton = !String.IsNullOrEmpty(helpLink);
            }
        }

        private void ShowHelp()
        {
            if (String.IsNullOrEmpty(helpLink))
                HelpLink = HelpTopics.HelpStartPage;

            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpLink);
        }

        private void AffectedServersConfirmationDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ShowHelp();
        }

        private void AffectedServersConfirmationDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (!String.IsNullOrEmpty(helpLink))
            {
                if (hlpevent != null) hlpevent.Handled = true;
                ShowHelp();
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