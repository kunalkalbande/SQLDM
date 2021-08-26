using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
using System.Threading;
    using System.ComponentModel;

    public partial class ServerSelectionDialog : BaseDialog
    {
        private IList<string> servers;
        private IList<string> selectedServers;
        private bool inUpdateButtons = false;
        private bool singleSelect = false;

        private ThreadStart postDelegate;

        public ServerSelectionDialog() 
        {
            this.DialogHeader = "Instances";
            InitializeComponent();
            if (postDelegate == null)
                postDelegate = new ThreadStart(UpdateButtons);
            AdaptFontSize();
        }

        public ServerSelectionDialog(bool singleSelect) : this()
        {
            if (singleSelect)
            {
                this.singleSelect = singleSelect;
                selectAllCheckBox.Visible = false;
                promptLabel.Text = "Select an instance.";
            }
        }

        public IList<string> Servers
        {
            get {
                if (servers == null)
                    servers = new List<string>();
                return servers;
            }
            set
            {
                servers = value;
            }
        }

        public IList<string> SelectedServers
        {
            get
            {
                if (selectedServers == null)
                    selectedServers = new List<string>();
                return selectedServers;
            }
            set { selectedServers = value; }
        }

        private void ServerSelectionDialog_Load(object sender, EventArgs e)
        {
            bool check = false;
            bool haveSelection = false;
            IList<string> selected = SelectedServers;
            serverListBox.Items.Clear();
            foreach (string name in Servers)
            {
                if (singleSelect && haveSelection)
                    check = false;
                else
                {
                    check = selected.Contains(name);
                    haveSelection = check;
                }
                serverListBox.Items.Add(name, check);
            }
            UpdateButtons();
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            IList<string> selected = SelectedServers;
            selected.Clear();
            foreach (string name in serverListBox.CheckedItems)
            {
                selected.Add(name);
            }
        }

        private void serverListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (singleSelect && e.NewValue == CheckState.Checked)
            {
                foreach (int i in serverListBox.CheckedIndices)
                {
                    if (i != e.Index)
                        serverListBox.SetItemChecked(i, false);
                }
            }
            serverListBox.BeginInvoke(postDelegate);
        }

        private void UpdateButtons()
        {
            inUpdateButtons = true;
            btnOK.Enabled = serverListBox.CheckedItems.Count > 0;
            selectAllCheckBox.Checked = serverListBox.CheckedItems.Count == serverListBox.Items.Count;
            inUpdateButtons = false;
        }

        private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (inUpdateButtons)
                return;

            bool selectAllState = selectAllCheckBox.Checked;
            int itemCount = serverListBox.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                serverListBox.SetItemChecked(i, selectAllState);        
            }
            serverListBox.BeginInvoke(postDelegate);
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