using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    /// <summary>
    /// This dialog is used for selecting single or multiple servers for reports.
    /// </summary>
    internal partial class ReportServersDialog : Idera.SQLdm.DesktopClient.Dialogs.BaseDialog {
        private static readonly Logger Log = Logger.GetLogger("ReportServersDialog");

        // Should be set before calling ShowDialog() to pre-select items in the list box.
        // After ShowDialog, this will contain the servers selected by the user.
        public List<MonitoredSqlServer> SelectedServers;
        private IList<string> excludedServers;
        private bool activeOnly;

        /// <summary>
        /// Set this to true to run the dialog in single-selection mode.
        /// </summary>
        public bool SingleSelect {
            set {
                Log.Debug("Setting SingleSelect to ", value);
                _singleSelect = value;
                _suppressItemCheck = value;
            }

            get { return _singleSelect; }
        }
        private bool _singleSelect;
        
        public ReportServersDialog() {
            InitializeComponent();

            // Adapt font size.
            AdaptFontSize();
        }

        public ReportServersDialog(string title, string description) 
            : this()
        {
            Text = title;
            descriptionLabel.Text = description;
        }

        public ReportServersDialog(string title, string description, IList<string> excluded) 
            : this(title, description)
        {
            excludedServers = excluded;
        }

        public bool ActiveServersOnly
        {
            get { return activeOnly;  }
            set { activeOnly = value; }
        }

        protected override void OnLoad(EventArgs e) {
            using (Log.DebugCall()) {
                Log.Debug("SingleSelect = ", SingleSelect);
                base.OnLoad(e);

                checkedListBox1.Cursor = Cursors.WaitCursor;

                if (SingleSelect) {
                    // In single-select mode, hide the "Select All" button and the checkboxes.
                    selectAll.Visible = false;

                    // Adjust the size and location of the checkedListBox so the checkboxes
                    // are not visible.
                    checkedListBox1.Dock = DockStyle.Fill;
                    Size size = checkedListBox1.Size;
                    Point location = checkedListBox1.Location;
                    checkedListBox1.Dock = DockStyle.None;
                    size.Width += 18;
                    checkedListBox1.Size = size;
                    location.X -= 18;
                    checkedListBox1.Location = location;
                    checkedListBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                }

                // Refresh the list of servers.
                if (ActiveServersOnly)
                {
                    foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                    {
                        if (excludedServers == null || !excludedServers.Contains(server.InstanceName))
                        {
                            checkedListBox1.Items.Add(server);
                        }
                    }
                }
                else
                {
                    foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                    {
                        if (excludedServers == null || !excludedServers.Contains(server.InstanceName))
                        {
                            checkedListBox1.Items.Add(server);
                        }
                    }
                }

                // Now pre-select the servers that are listed in SelectedServers.
                PreSelectServers();

                checkedListBox1.Cursor = Cursors.Default;
            }
        }

        private void selectAll_CheckedChanged(object sender, EventArgs e) 
        {
            if (_suppressItemCheck)
                return;
            _suppressItemCheck = true;
            bool isChecked = selectAll.Checked;
            for (int i = 0; i < checkedListBox1.Items.Count; ++i) {
                checkedListBox1.SetItemChecked(i, isChecked);
            }
            okButton.Enabled = checkedListBox1.CheckedIndices.Count > 0;
            _suppressItemCheck = false;
        }

        // Parse the comma-separated list and select the matching items in the ListBox.
        private void PreSelectServers() {
            if (SelectedServers != null) {
                foreach (MonitoredSqlServer selected in SelectedServers) {
                    if (selected != null) {
                        for (int i = 0; i < checkedListBox1.Items.Count; ++i) {
                            MonitoredSqlServer listed = (MonitoredSqlServer)checkedListBox1.Items[i];
                            if (selected.Id == listed.Id) {
                                if (SingleSelect) {
                                    checkedListBox1.SelectedIndex = i;
                                } else {
                                    checkedListBox1.SetItemChecked(i, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
        }          

        private void okButton_Click(object sender, EventArgs e) {
            using (Log.DebugCall()) {
                if (SingleSelect) {
                    SelectedServers = new List<MonitoredSqlServer>();
                    SelectedServers.Add((MonitoredSqlServer)checkedListBox1.SelectedItem);
                } else {
                    if (checkedListBox1.CheckedItems.Count > 5 && Properties.Settings.Default.ShowMessage_ManyServersSlowsReporting) {
                        ApplicationMessageBox box = new ApplicationMessageBox();
                        box.Text = "Selecting a large number of servers can lead to slow report generation.";
                        box.Symbol = ExceptionMessageBoxSymbol.Information;
                        box.Buttons = ExceptionMessageBoxButtons.OK;
                        box.ShowCheckBox = true;
                        box.CheckBoxText = "Don't show this message again.";
                        box.Caption = this.Text;
                        box.Show(this);                                     
                        Properties.Settings.Default.ShowMessage_ManyServersSlowsReporting = !box.IsCheckBoxChecked;
                    }

                    MonitoredSqlServer[] serverArray = new MonitoredSqlServer[checkedListBox1.CheckedItems.Count];
                    checkedListBox1.CheckedItems.CopyTo(serverArray, 0);
                    SelectedServers = new List<MonitoredSqlServer>(serverArray);
                }
            }
        }


        private bool _suppressItemCheck;
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressItemCheck) return; // recursion check.
            
            _suppressItemCheck = true;

            if (e.NewValue == CheckState.Checked) {
                okButton.Enabled = true;

                if (SingleSelect) {
                    // uncheck all other boxes.
                    for (int i = 0; i < checkedListBox1.Items.Count; ++i) {
                        if (i != e.Index) checkedListBox1.SetItemChecked(i, false);
                    }
                }
                selectAll.Checked = (checkedListBox1.CheckedIndices.Count + 1 == checkedListBox1.Items.Count);
            } else {
                selectAll.Checked = false;
                okButton.Enabled = checkedListBox1.CheckedIndices.Count > 1;
            }

            _suppressItemCheck = false;
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (!SingleSelect) return;
            okButton.Enabled = checkedListBox1.SelectedItem != null;
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