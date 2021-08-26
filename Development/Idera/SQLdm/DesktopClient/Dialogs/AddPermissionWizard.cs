using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AddPermissionWizard : BaseDialog
    {
        private const string NewTagToolKey = "New tag...";
        private const int WindowsAuthenticationIndex = 0;
        private const int SQLServerAuthenticationIndex = 1;
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("AddPermissionWizard");
        private ServerVersion m_ServerVersion = null;
        private readonly List<int> selectedTags = new List<int>();
        private readonly List<int> selectedServers = new List<int>();
        private bool suppressSelectionChanged = false;

        public AddPermissionWizard()
        {
            InitializeComponent();

            // Get the repository SQL Server version.
            using (SqlConnection connection
                        = new SqlConnection(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString))
            {
                connection.Open();
                m_ServerVersion = new ServerVersion(connection.ServerVersion);
            }

            // Set the first page based on the settings option.
            if (Settings.Default.HideAddPermissionsWizardWelcomePage)
            {
                _wizard.SelectedPage = _userPage;
                _userPage.AllowMoveNext = false;
            }
            else
            {
                _wizard.SelectedPage = _introPage;
            }

            // Set default to Windows authentication.
            _userPageCmbBxAuthentication.SelectedIndex = WindowsAuthenticationIndex;

            // Hide the user page error message.
            hideUserPageErrMsg();

            UpdateTags();
            AdaptFontSize();
        }

        #region Properties

        public string User
        {
            get { return _userPageTxtBxUser.Text; }
        }

        public string Authentication
        {
            get { return (IsSqlAuthentication ? "SQL Server" : "Windows"); }
        }

        public bool IsSqlAuthentication
        {
            get { return _userPageCmbBxAuthentication.SelectedIndex == SQLServerAuthenticationIndex; }
        }

        [AuditableAttribute(true, true)]
        public string Password
        {
            get { return _passwordPageTxtBxPassword.Text; }
        }

        public PermissionType Permission
        {
            get
            {
                if (_permissionPageRdBtnView.Checked) { return PermissionType.View; }
                else if (_permissionPageRdBtnModify.Checked) { return PermissionType.Modify; }
                else if (_permissionPageRdBtnAdministrator.Checked) { return PermissionType.Administrator; }
                else if (_permissionPageRdBtnReadOnlyPlus.Checked) { return PermissionType.ReadOnlyPlus; }
                else
                {
                    Debug.Assert(false, "Invalid permission type");
                    return PermissionType.View;
                }
            }
        }

        public IEnumerable<int> SelectedTags
        {
            get { return selectedTags; }
        }

        public IEnumerable<int> SQLServerIDs
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

        public string Comment
        {
            get { return ""; }
        }


        public bool WebAppPermission
        {
            get { return _permissionPageChkBxWebAppAccess.Checked; }
        }

        private string HelpTopic
        {
            get
            {
                if (_wizard.SelectedPage == _introPage)
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizard;
                }
                else if (_wizard.SelectedPage == _userPage)
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizardUser;
                }
                else if (_wizard.SelectedPage == _permissionPage)
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizardPermission;
                }
                else if (_wizard.SelectedPage == _serversPage)
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizardServers;
                }
                else if (_wizard.SelectedPage == _finishPage)
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizardSummary;
                }
                else
                {
                    return Idera.SQLdm.Common.HelpTopics.AddPermissionWizard;
                }
            }
        }

        #endregion

        #region User page

        private void userPageAllowMoveNext()
        {
            _userPage.AllowMoveNext = _userPageTxtBxUser.Text.Length > 0;
        }

        private static SID_NAME_USE getWindowsAccountType(string account)
        {
            string refDom;
            byte[] bsid;
            SID_NAME_USE sidUse;
            LookupAccountName(null, account, out bsid, out refDom, out sidUse);
            return sidUse;
        }

        private static bool isWindowsGroup(SID_NAME_USE accountType)
        {
            return (accountType == SID_NAME_USE.SidTypeGroup
                            || accountType == SID_NAME_USE.SidTypeWellKnownGroup
                                || accountType == SID_NAME_USE.SidTypeAlias);
        }

        private void hideUserPageErrMsg()
        {
            _userPageErrMsg.Visible = false;
        }

        private void showUserPageErrMsg(string msg)
        {
            _userPageErrMsg.Visible = true;
            _userPageErrMsg.Text = msg;
        }

        private void _userPage_BeforeDisplay(object sender, EventArgs e)
        {
            userPageAllowMoveNext();
        }

        private void _userPageTxtBxUser_TextChanged(object sender, EventArgs e)
        {
            hideUserPageErrMsg();
            userPageAllowMoveNext();
        }

        private void _userPageCmbBxAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_userPageCmbBxAuthentication.SelectedIndex == SQLServerAuthenticationIndex)
            {
                _userPageLblUser.Text = "User name:";
                hideUserPageErrMsg();
                userPageAllowMoveNext();
            }
            else
            {
                _userPageLblUser.Text = @"Domain\&User name:";
            }
        }

        private void _userPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {

            // If the authentication is SQL, check if the SQL Login exists.  If SQL login
            // does not exist then prompt for password needed for creating the user.
            if (_userPageCmbBxAuthentication.SelectedIndex == SQLServerAuthenticationIndex)
            {
                // Check if login exists.
                bool needPassword = false;
                try
                {
                    IManagementService managementService = ManagementServiceHelper.GetDefaultService();
                    needPassword = !managementService.DoesLoginExist(_userPageTxtBxUser.Text);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered when checking if SQL Login exists", ex);
                    needPassword = true;
                }

                // If password is needed then setup the password page as next page,
                // else set next page to permission page.
                if (needPassword)
                {
                    _userPage.NextPage = _passwordPage;
                    _passwordPage.PreviousPage = _userPage;
                    _passwordPage.NextPage = _permissionPage;
                    _permissionPage.PreviousPage = _passwordPage;
                }
                else
                {
                    _userPage.NextPage = _permissionPage;
                    _permissionPage.PreviousPage = _userPage;


                    _permissionPageChkBxWebAppAccess.Checked = _permissionPageChkBxWebAppAccess.Enabled = false;
                }
            }
            else // Windows authentication
            {
                // Check if the specified windows account is valid.
                bool isAccountTypeValid = true;
                string errMsg = "";
                SID_NAME_USE accountType = getWindowsAccountType(_userPageTxtBxUser.Text);
                if (m_ServerVersion.Major == 8)
                {
                    if (accountType != SID_NAME_USE.SidTypeUser)
                    {
                        isAccountTypeValid = false;

                        if (accountType == SID_NAME_USE.SidTypeInvalid || accountType == SID_NAME_USE.SidTypeUnknown)
                        {
                            errMsg = "The specified Windows account is invalid. ";
                        }

                        errMsg += "Note that Windows Groups cannot be specified when the SQL Diagnostic Manager Repository is hosted on SQL Server 2000. Windows Groups are only supported for repositories hosted on SQL Server 2005 or higher.";
                    }
                }
                else
                {
                    if (accountType != SID_NAME_USE.SidTypeUser && !isWindowsGroup(accountType))
                    {
                        isAccountTypeValid = false;
                        if (accountType == SID_NAME_USE.SidTypeInvalid || accountType == SID_NAME_USE.SidTypeUnknown)
                        {
                            errMsg = "The specified Windows account is invalid. ";
                        }

                        errMsg += "When Windows Authentication is selected, only a Windows User or a Group can be specified as a login to assign to SQLdm.";
                    }
                }

                if (!isAccountTypeValid)
                {
                    showUserPageErrMsg(errMsg);
                    _userPage.AllowMoveNext = false;
                    e.Cancel = true;
                }
                else
                {
                    _permissionPageChkBxWebAppAccess.Checked = _permissionPageChkBxWebAppAccess.Enabled = true;
                    _userPage.NextPage = _permissionPage;
                    _permissionPage.PreviousPage = _userPage;
                }
            }
        }

        #endregion

        #region Password page
        private void passwordPageAllowMoveNext()
        {
            // The passwords should be non-zero and must match.
            _passwordPage.AllowMoveNext = _passwordPageTxtBxPassword.Text.Length > 0
                                          && _passwordPageTxtBxConfirmPassword.Text.Length > 0
                                          && _passwordPageTxtBxPassword.Text == _passwordPageTxtBxConfirmPassword.Text;


        }

        private void _passwordPage_BeforeMoveNext(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _permissionPageChkBxWebAppAccess.Enabled = !IsSqlAuthentication;
            _permissionPageChkBxWebAppAccess.Checked = _permissionPageChkBxWebAppAccess.Enabled;
        }



        private void _passwordPage_BeforeDisplay(object sender, EventArgs e)
        {
            passwordPageAllowMoveNext();
        }

        private void _passwordPageTxtBxPassword_TextChanged(object sender, EventArgs e)
        {
            passwordPageAllowMoveNext();
        }

        private void _passwordPageTxtBxConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            passwordPageAllowMoveNext();
        }
        #endregion

        #region Permission page
        private void _permissionPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            // If admin permission is checked, go to finish page.
            // Else go to the servers page.
            if (_permissionPageRdBtnAdministrator.Checked)
            {
                _permissionPage.NextPage = _finishPage;
                _finishPage.PreviousPage = _permissionPage;
            }
            else
            {
                _permissionPage.NextPage = _serversPage;
                _serversPage.PreviousPage = _permissionPage;
                _serversPage.NextPage = _finishPage;
                _finishPage.PreviousPage = _serversPage;
            }
        }
        #endregion

        #region Finish page

        private void _finishPage_BeforeDisplay(object sender, EventArgs e)
        {
            _finishPageLblUserVal.Text = User;
            _finishPageLblAuhthenticationVal.Text = Authentication;
            _finishPageLblPermissionVal.Text = TypeDescriptor.GetConverter(Permission).ConvertToString(Permission);
            string selectedTagsString = BuildSelectedTagsString();
            _finishLabelTagsValue.Text = selectedTagsString.Length != 0 ? selectedTagsString : "< None selected >";
            _finishPagelLstBxServers.Items.Clear();
            foreach (object s in _serversListSelectorControl.Selected.Items)
            {
                _finishPagelLstBxServers.Items.Add(s);
            }
        }

        #endregion

        #region Help stuff
        private void AddPermissionWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopic);
        }

        private void AddPermissionWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopic);
        }
        #endregion

        #region Interops
        private enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup,
            SidTypeDomain,
            SidTypeAlias,
            SidTypeWellKnownGroup,
            SidTypeDeletedAccount,
            SidTypeInvalid,
            SidTypeUnknown,
            SidTypeComputer
        };

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        private static bool LookupAccountName(
                String lpSystemName,
                String lpAccountName,
                out byte[] sid,
                out string refDomain,
                out SID_NAME_USE peUse
            )
        {
            // Init returns.
            sid = null;
            refDomain = null;
            peUse = SID_NAME_USE.SidTypeUnknown;

            // Validate inputs.
            if (lpAccountName == null || lpAccountName.Length == 0) { return false; }

            // Allocate buffers.
            byte[] bSid = new byte[40];
            if (bSid == null) { return false; }
            StringBuilder refDomBldr = new StringBuilder(100);
            if (refDomBldr == null) { return false; }
            uint bSidSize = (uint)bSid.Length;
            uint refDomBldrSize = (uint)refDomBldr.Capacity;

            // Lookup account by name.
            uint rc = 0;
            if (!_LookupAccountName(lpSystemName, lpAccountName, bSid, ref bSidSize,
                    refDomBldr, ref refDomBldrSize, out peUse))
            {
                rc = checked((uint)Marshal.GetLastWin32Error());
            }
            else
            {
                sid = bSid;
                refDomain = refDomBldr.ToString();
            }

            return rc == 0;
        }

        [DllImport("advapi32.dll", EntryPoint = "LookupAccountName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool _LookupAccountName(
                String lpSystemName,
                String lpAccountName,
                byte[] Sid,
                ref uint cbSid,
                StringBuilder lpReferencedDomainName,
                ref uint cchReferencedDomainName,
                out SID_NAME_USE peUse
            );

        #endregion

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void UpdateTags()
        {
            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Clear();
            SortedDictionary<string, Tag> sortedTags = new SortedDictionary<string, Tag>();

            foreach (Tag tag in ApplicationModel.Default.LocalTags)
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

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
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

        private void _serversListSelectorControl_SelectionChanged(object sender, EventArgs e)
        {
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
    }
}