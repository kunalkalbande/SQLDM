using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class PermissionPropertyDialog : Form
    {
        #region members

        private const string NewTagToolKey = "New tag...";

        private PermissionType m_OriginalPermission;
        private int m_PermissionID;
        private readonly List<int> selectedTags = new List<int>();
        private readonly List<int> selectedServers = new List<int>();
        private bool suppressSelectionChanged = false;

        #endregion

        #region ctors

        public PermissionPropertyDialog(PermissionDefinition permission)
        {
            InitializeComponent();
            _tabControl.DrawFilter = new HideFocusRectangleDrawFilter();

            // Remember original permission.
            m_OriginalPermission = permission.PermissionType;
            m_PermissionID = permission.PermissionID;

            // Make header text.
            Text = string.Format(Text, permission.Login);

            // Initialize general page.
            _generalChkBxEnabled.Checked = permission.Enabled;
            switch (permission.PermissionType)
            {
                case PermissionType.View:
                    _generalRdBtnView.Checked = true;
                    break;

                case PermissionType.Modify:
                    _generalRdBtnModify.Checked = true;
                    break;

                case PermissionType.Administrator:
                    _generalRdBtnAdministrator.Checked = true;
                    break;

                case PermissionType.ReadOnlyPlus:
                    _generalRdBtnReadOnlyPlus.Checked = true;
                    break;
             }
            _generalTxtBxComment.Text = permission.Comment;

            _generalChkBxWebAppPermission.Enabled = permission.LoginType !=null ? ((permission.LoginType != LoginType.SQLLogin)):false;
            _generalChkBxWebAppPermission.Checked = permission.LoginType !=null ? (permission.LoginType != LoginType.SQLLogin ? permission.WebAppPermission : false):false;


            _tabControl.Tabs["_generalTab"].Selected = true;
            _btnOK.Enabled = false;
            initializationWorker.RunWorkerAsync();
            this.AdaptFontSize();
        }

        #endregion

        #region properties

        public bool IsPermissionEnabled
        {
            get { return _generalChkBxEnabled.Checked; }
        }

        public PermissionType Permission
        {
            get
            {
                if (_generalRdBtnView.Checked) { return PermissionType.View; }
                else if (_generalRdBtnModify.Checked) { return PermissionType.Modify; }
                else if (_generalRdBtnReadOnlyPlus.Checked) { return PermissionType.ReadOnlyPlus; }
                else if (_generalRdBtnAdministrator.Checked) { return PermissionType.Administrator; }
                else { Debug.Assert(false, "Unknown permission"); return PermissionType.View; }
            }
        }

        public string Comment
        {
            get { return _generalTxtBxComment.Text; }
        }

        public bool WebAppPermission
        {
            get { return _generalChkBxWebAppPermission.Checked;}
        }

        public IEnumerable<int> SelectedTags
        {
            get { return selectedTags; }
        }

        public IEnumerable<int> SelectedServers
        {
            get
            {
                int cnt = _serversListSelectorControl.Selected.Items.Count;
                if (cnt != 0 && Permission != PermissionType.Administrator)
                {
                    return selectedServers;
                }
                else
                {
                    return null;
                }
            }
        }

        private string HelpTopic
        {
            get
            {
                if (_tabControl.Tabs["_generalTab"].Selected)
                {
                    return Idera.SQLdm.Common.HelpTopics.PermissionDialogGeneral;
                }
                else
                {
                    return Idera.SQLdm.Common.HelpTopics.PermissionDialogServers;
                }
            }
        }

        #endregion

        private void _generalChkBxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true; 
        }

        private void _generalChkBxWebAppPermission_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;// Ankit - need to check with Gaurav
        }

        private void _generalRdBtnView_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;
        }

        private void _generalRdBtnModify_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;
        }

        //Operator Security Role Changes - 10.3
        private void _generalRdBtnReadOnlyPlus_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;
        }

        private void _generalRdBtnAdministrator_CheckedChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;
            _tabControl.Tabs["_assignedServersTab"].Visible = !_generalRdBtnAdministrator.Checked;
        }

        private void _generalTxtBxComment_TextChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;
        }

        private void _serversListSelectorControl_SelectionChanged(object sender, EventArgs e)
        {
            _btnOK.Enabled = true;

            if (!suppressSelectionChanged)
            {
                selectedServers.Clear();

                foreach (object obj in _serversListSelectorControl.Selected.Items)
                {
                    if (obj is Server)
                    {
                        selectedServers.Add(((Server)obj).SQLServerID);
                    }
                }
            }
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            // If permission is changing from Admin to View/Modify, check to see if
            // the user is still an Admin from some other permission.
            bool isAdminPowerLoss = false;
            if (m_OriginalPermission == PermissionType.Administrator && m_OriginalPermission != Permission)
            {
                isAdminPowerLoss = !UserToken.IsUserAdministratorWithoutPermission
                                                (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                                                        m_PermissionID);
            }

            // If admin power loss confirm before continuing.
            if(isAdminPowerLoss)
            {
                if (ApplicationMessageBox.ShowQuestion(this, "You will lose administrator power after the permission is updated because you have changed the permission to View or Modify.   Do you wish to continue?") != DialogResult.Yes)
                {
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void PermissionPropertyDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopic);
        }

        private void PermissionPropertyDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopic);
        }

        private void UpdateTags()
        {
            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Clear();
            SortedDictionary<string, Tag> sortedTags = new SortedDictionary<string, Tag>();

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                sortedTags.Add(tag.Name, tag);
            }

            foreach (Tag tag in sortedTags.Values)
            {
                StateButtonTool tagTool;
                string tagIdString = tag.Id.ToString();

                if (toolbarsManager.Tools.Exists(tag.Id.ToString()))
                {
                    tagTool = (StateButtonTool)toolbarsManager.Tools[tagIdString];
                }
                else
                {
                    tagTool = new StateButtonTool(tag.Id.ToString());
                    tagTool.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
                    toolbarsManager.Tools.Add(tagTool);
                }

                tagTool.SharedProps.Caption = tag.Name;
                tagTool.Checked = selectedTags.Contains(tag.Id);
                ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Add(tagTool);
            }

            ToolBase newTagTool;
            if (!toolbarsManager.Tools.Exists(NewTagToolKey))
            {
                newTagTool = new StateButtonTool(NewTagToolKey);
                newTagTool.SharedProps.Caption = NewTagToolKey;
                toolbarsManager.Tools.Add(newTagTool);
            }
            else
            {
                newTagTool = toolbarsManager.Tools[NewTagToolKey];
            }

            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Add(newTagTool);
            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools[NewTagToolKey].InstanceProps.IsFirstInGroup = true;

            UpdateSelectedTags();
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (e.Tool.OwningMenu == toolbarsManager.Tools["tagsPopupMenu"])
            {
                try
                {
                    if (e.Tool.Key == NewTagToolKey)
                    {
                        AddTagDialog dialog = new AddTagDialog();

                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            selectedTags.Add(dialog.TagId);
                            UpdateTags();
                        }
                    }
                    else
                    {
                        int tagId = Convert.ToInt32(e.Tool.Key);

                        if (!selectedTags.Contains(tagId))
                        {
                            selectedTags.Add(tagId);
                        }
                        else
                        {
                            selectedTags.Remove(tagId);
                        }
                    }

                    UpdateSelectedTags();
                    _btnOK.Enabled = true;
                }
                catch { }
            }
        }

        private void UpdateSelectedTags()
        {
            string selectedTagsString = BuildSelectedTagsString();
            tagsDropDownButton.Text = selectedTagsString.Length != 0
                                          ? selectedTagsString
                                          : "< Click here to choose tags >";
            UpdateSelectedServers();
        }

        private void UpdateSelectedServers()
        {
            try
            {
                suppressSelectionChanged = true;
                _serversListSelectorControl.Selected.BeginUpdate();
                _serversListSelectorControl.Selected.Items.Clear();
                _serversListSelectorControl.Available.BeginUpdate();
                _serversListSelectorControl.Available.Items.Clear();

                foreach (int tagId in selectedTags)
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

                                ExtendedListItem listItem = new ExtendedListItem(itemName.ToString());
                                listItem.Tag = new Pair<int, int>(instanceId, tagId);
                                listItem.TextColor = SystemColors.GrayText;
                                listItem.CanRemove = false;
                                _serversListSelectorControl.Selected.Items.Add(listItem);
                            }
                        }
                    }
                }

                foreach (ServerPermission sp in ApplicationModel.Default.UserToken.AssignedServers)
                {
                    if (selectedServers.Contains(sp.Server.SQLServerID))
                    {
                        _serversListSelectorControl.Selected.Items.Add(sp.Server);
                    }
                    else
                    {
                        _serversListSelectorControl.Available.Items.Add(sp.Server);
                    }
                }
            }
            finally
            {
                suppressSelectionChanged = false;
                _serversListSelectorControl.Selected.EndUpdate();
                _serversListSelectorControl.Available.EndUpdate();
            }
        }

        private string BuildSelectedTagsString()
        {
            StringBuilder selectedTagsString = new StringBuilder();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Key != NewTagToolKey && tag.Checked)
                {
                    if (selectedTagsString.Length > 0)
                    {
                        selectedTagsString.Append(", ");
                    }

                    selectedTagsString.Append(tag.SharedProps.Caption);
                }
            }

            return selectedTagsString.ToString();
        }

        private void initializationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result =
                RepositoryHelper.GetPermissionTagsAndServers(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo, m_PermissionID);
            e.Cancel = initializationWorker.CancellationPending;
        }

        private void initializationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    if (e.Result is Triple<bool, IList<int>, IList<int>>)
                    {
                        Triple<bool, IList<int>, IList<int>> result = (Triple<bool, IList<int>, IList<int>>)e.Result;

                        selectedTags.AddRange(result.Second);
                        selectedServers.AddRange(result.Third);
                        UpdateTags();
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        "An unexpected type was encountered while initializing the permission properties.");
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "An error occurred while initializing the permission properties.",
                                                    e.Error);
                }
            }
        }

        private void PermissionPropertyDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            initializationWorker.CancelAsync();
        }

        private void PermissionPropertyDialog_SizeChanged(object sender, System.EventArgs e)
        {
            _serversListSelectorControl.Refresh();
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
