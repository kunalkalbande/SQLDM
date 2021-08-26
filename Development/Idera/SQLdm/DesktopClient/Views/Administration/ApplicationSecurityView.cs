using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using System.Diagnostics;
using Idera.SQLdm.Common.Auditing;
using Wintellect.PowerCollections;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    internal partial class ApplicationSecurityView : Idera.SQLdm.DesktopClient.Views.View
    {
        private bool m_HideSysadminLogins = false; // this needs to be set to true if we do not want to show sys admins by default.
        private bool m_IsSecurityEnabled = false;
        private UltraGridColumn m_SelectedColumn = null;

        public ApplicationSecurityView()
        {
            InitializeComponent();
            _permissionGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            _permissionGrid.DisplayLayout.GroupByBox.Hidden = true;

            // Perform auto resize column.
            _permissionGrid.DisplayLayout.Bands[0].Columns["Enabled"].PerformAutoResize();

            _tlbrsMngr.Tools["_btnShowSysadmins"].SharedProps.Visible = false; // for now hiding the show sysadmins button

            SuspendLayout();
            Configuration configuration = new Configuration();
            configuration.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            UpdateData(configuration);
            ResumeLayout();

            AdaptFontSize();
        }

        private void updateTools()
        {
            bool isSysAdmin = ApplicationModel.Default.UserToken.IsSysadmin;
            bool isSQLdmAdministrator = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            bool isGrouped = _permissionGrid.Rows.Count > 0 && _permissionGrid.Rows[0].IsGroupByRow;
            bool isGridSingleRowSelect = _permissionGrid.Selected.Rows.Count == 1;
            bool isRowEditable = false;
            if (isGridSingleRowSelect)
            {
                PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
                Debug.Assert(permission != null, "Null permission object");
                isRowEditable = !permission.System;
            }

            // Enable "Enable Security" button if security is not enabled and the user is a sysadmin.
            _tlbrsMngr.Tools["_btnEnableSecurity"].SharedProps.Enabled = !m_IsSecurityEnabled && isSysAdmin;
            _gettingStartedEnableSecurityLinkLabel.Visible = _tlbrsMngr.Tools["_btnEnableSecurity"].SharedProps.Enabled;

            // Show "Disable Security" button and "Permission" & "Show Hide" groups if security is enabled.
            // If security is not enabled the button and groups are hidden.
            _tlbrsMngr.Tools["_btnDisableSecurity"].SharedProps.Visible
                = _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpPermission"].Visible
                    = _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpShowHide"].Visible = m_IsSecurityEnabled;

            // Hide enable security if security is enabled.
            _tlbrsMngr.Tools["_btnEnableSecurity"].SharedProps.Visible = !m_IsSecurityEnabled;

            // If security is enabled and the user is a SQLdm administrator, then enable
            // "Disable Security", "Add", "Edit" and "Delete" permission buttons.
            if (m_IsSecurityEnabled)
            {
                _tlbrsMngr.Tools["_btnDisableSecurity"].SharedProps.Enabled 
                    = _tlbrsMngr.Tools["_btnAddPermission"].SharedProps.Enabled 
                        = isSQLdmAdministrator;

                _tlbrsMngr.Tools["_btnEditPermission"].SharedProps.Enabled
                    = _tlbrsMngr.Tools["_btnDeletePermission"].SharedProps.Enabled 
                        = _tlbrsMngr.Tools["_btnEnableDisable"].SharedProps.Enabled
                            = isSQLdmAdministrator && isGridSingleRowSelect && isRowEditable;

                _tlbrsMngr.Tools["_btnExpandAllGroups"].SharedProps.Enabled
                    = _tlbrsMngr.Tools["_btnCollapseAllGroups"].SharedProps.Enabled
                        = isGrouped;
            }
        }

        private void updatePanel(Dictionary<int, PermissionDefinition> permissions)
        {
            // Display the grid or getting started page.
            _pnlGrid.Visible = m_IsSecurityEnabled;
            _pnlGettingStarted.Visible = !_pnlGrid.Visible;

            // If grid is visible then update it.
            if (_pnlGrid.Visible)
            {
                // Save grid state.
                UltraGridHelper.GridState state = UltraGridHelper.GetGridState(_permissionGrid, "PermissionID");
                _permissionGrid.SuspendLayout();
                _permissionGrid.SuspendRowSynchronization();

                // Clear the binding source and add permissions.
                _permissionBindingSource.Clear();
                foreach (Pair<int, PermissionDefinition> pair in permissions)
                {
                    // Do not show sysadmin logins if user wants non-system
                    // permission.
                    if (!m_HideSysadminLogins || !pair.Second.System)
                    {
                        _permissionBindingSource.Add(pair.Second);
                    }
                    if (pair.Second.LoginType == LoginType.SQLLogin)
                            pair.Second.WebAppPermission = false;
                }

                // Restore grid state.
                UltraGridHelper.RestoreGridState(state);
                _permissionGrid.ResumeRowSynchronization();
                _permissionGrid.ResumeLayout();
            }
        }

        private bool isGridGrouped()
        {
            return (_permissionGrid.Rows.Count > 0 && _permissionGrid.Rows[0].IsGroupByRow);
        }

        private bool isGridExpanded()
        {
            return (_permissionGrid.Rows[0].IsExpanded);
        }

        private void enableSecurity()
        {
            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // If sysadmin proceed with enable security, else display error.
            if (userToken.IsSysadmin)
            {
                // Display confirmation message, and user says
                // yes, then enable security.
                if (ApplicationMessageBox.ShowQuestion(this, "Once Application Security is enabled the only users who will be able to access SQLDM are those users that belong to the sysadmin role on the SQLDM Repository. You must add SQLDM permissions for all other SQLDM users.   Do you wish to continue?") == DialogResult.Yes)
                {
                    try
                    {
                        AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Enable security.
                        managementService.EnableSecurity();

                        // Refresh the user token.
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while enabling SQLDM application security";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not sysadmin show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users belonging to the sysadmin role in the SQLDM Repository can enable SQLDM application security");
            }
        }

        private void disableSecurity()
        {
             // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

           // If administrator proceed with enable security, else display error.
            if (userToken.IsSQLdmAdministrator)
            {
                // Display confirmation message, and user says
                // yes, then enable security.
                if (ApplicationMessageBox.ShowQuestion(this, "When SQLDM application security is disabled, users can perform any SQLDM task and can access any monitored server.   The permissions that have been configured will be disabled.  Do you wish to continue?")
                        == DialogResult.Yes)
                {
                    try
                    {
                        AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Disable security.
                        managementService.DisableSecurity();

                        // Refresh the user token.
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while disabling SQLDM application security.";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not sysadmin show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can disable SQLDM application security");
            }
        }

        private void addPermission()
        {
             // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

           // If administrator proceed with add permission, else display error.
            if (userToken.IsSQLdmAdministrator)
            {
                // Display the add permisison wizard to get inputs and create the permission.
                AddPermissionWizard wiz = new AddPermissionWizard();
                if (wiz.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Add permission
                        managementService.AddPermission(wiz.User, wiz.IsSqlAuthentication,wiz.Password, wiz.Permission,
                                                        wiz.SelectedTags, wiz.SQLServerIDs, wiz.Comment, wiz.WebAppPermission);

                        // Refresh the user token.
                        ApplicationModel.Default.RefreshTags();
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while adding a new SQLDM permission.";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not administrator show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can add permissions.");
            }
        }

        private void editPermission()
        {
            Debug.Assert(_permissionGrid.Selected.Rows.Count != 0);

            // if selected row is a system permisison, do nothing.
            PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
            Debug.Assert(permission != null);
            if (permission.System)
            {
                ApplicationMessageBox.ShowInfo(this, "System permissions cannot be edited.");
                return;
            }

            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // If administrator proceed with edit permission.
            if (userToken.IsSQLdmAdministrator)
            {
                // Display the permission properties dialog.  The dialog handles the case
                // of admin power loss.   So no need to check this case in here.
                PermissionPropertyDialog dialog = new PermissionPropertyDialog(permission);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Edit permission
                        managementService.EditPermission(
                            permission.PermissionID, dialog.IsPermissionEnabled, dialog.Permission, 
                            dialog.SelectedTags, dialog.SelectedServers, dialog.Comment, dialog.WebAppPermission);

                        // Refresh tags and user token
                        ApplicationModel.Default.RefreshTags();
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while editing SQLDM permission.";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not administrator show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can edit permissions.");
            }
        }

        private void enablePermission()
        {
            Debug.Assert(_permissionGrid.Selected.Rows.Count != 0);

            // if selected row is a system permisison, do nothing.
            PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
            Debug.Assert(permission != null);
            if (permission.System) 
            {
                ApplicationMessageBox.ShowInfo(this, "System permissions cannot be disabled.");
                return; 
            }

            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // If administrator proceed with enable permission.
            if (userToken.IsSQLdmAdministrator)
            {
                // If permission is being disabled then check to see if the user loses admin permission
                // if so warn the user.
                bool isContinue = true;
                if (permission.Enabled)
                {
                    // Check if by disabling this permission the user will lose admin rights,
                    // if so warn the user then continue if user says yes.
                    if (!UserToken.IsUserAdministratorWithoutPermission
                                            (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                                                    permission.PermissionID))
                    {
                        isContinue = ApplicationMessageBox.ShowQuestion(this, "If you disable this permission you will no longer be an administrator.   Do you wish to continue?")
                            == DialogResult.Yes;
                    }
                }

                // Proceed if user is okay with it.
                if (isContinue)
                {
                    try
                    {
                        AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Toggle permission status.
                        managementService.SetPermissionStatus(permission.PermissionID, !permission.Enabled);

                        // Refresh the user token.
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while enabling/disabling permission.";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not administrator show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can enable/disable permissions.");
            }
        }

        private void GrantWebAccess()
        {
            Debug.Assert(_permissionGrid.Selected.Rows.Count != 0);

            // if selected row is a system permisison, do nothing.
            PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
            Debug.Assert(permission != null);

            if(permission.LoginType == LoginType.SQLLogin && !permission.WebAppPermission)
            {
                ApplicationMessageBox.ShowInfo(this, "Web application access cannot be granted for SQL permissions .");
                return;
            }

            if (permission.System)
            {
                ApplicationMessageBox.ShowInfo(this, "System web application access cannot be revoked.");
                return;
            }

            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // If administrator proceed with enable permission.
            if (userToken.IsSQLdmAdministrator)
            {
                try
                {
                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    // Toggle permission status.
                    managementService.SetWebAccessStatus(permission.PermissionID, !permission.WebAppPermission);

                    // Refresh the user token.
                    ApplicationModel.Default.RefreshUserToken();

                    // Refresh the view.
                    ApplicationController.Default.RefreshActiveView();
                }
                catch (Exception ex)
                {
                    string msg = "Error was encountered while granting/revoking web application access.";
                    ApplicationMessageBox.ShowError(this, msg, ex);
                    Log.Error(msg, ex);
                }
            }

            else // not administrator show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can grant/revoke web application access.");
            }
        }

        private void deletePermission()
        {
            Debug.Assert(_permissionGrid.Selected.Rows.Count != 0);

            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // if selected row is a system permisison, do nothing.
            PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
            Debug.Assert(permission != null);
            if (permission.System)
            {
                ApplicationMessageBox.ShowInfo(this, "System permissions cannot be edited.");
                return;
            }

            // If administrator proceed with delete permission.
            if (userToken.IsSQLdmAdministrator)
            {
                // Check if by deleting this permission the user will lose admin rights,
                // if so warn the user then delete if user says yes.
                bool isContinue = false;
                if (!UserToken.IsUserAdministratorWithoutPermission
                                        (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                                                permission.PermissionID))
                {
                    isContinue = ApplicationMessageBox.ShowQuestion(this, "If you delete this permission you will no longer be an administrator.   Do you wish to continue?")
                        == DialogResult.Yes;
                }
                else
                {
                    isContinue = ApplicationMessageBox.ShowQuestion(this, "Do you wish to delete this permission?")
                        == DialogResult.Yes;
                }

                // Delete if user says continue.
                if (isContinue)
                {
                    try
                    {
                        AuditingEngine.SetContextData(
                             Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                        // Delete permission.
                        managementService.DeletePermission(permission.PermissionID);

                        // Refresh the user token.
                        ApplicationModel.Default.RefreshTags();
                        ApplicationModel.Default.RefreshUserToken();

                        // Refresh the view.
                        ApplicationController.Default.RefreshActiveView();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error was encountered while deleting a permission.";
                        ApplicationMessageBox.ShowError(this, msg, ex);
                        Log.Error(msg, ex);
                    }
                }
            }
            else // not administrator show error
            {
                // Display error message that user is not sysadmin.
                ApplicationMessageBox.ShowError(this, "Only users with SQLDM Administrator power can delete permissions.");
            }
        }

        private void toggleGroupByBox()
        {
            _permissionGrid.DisplayLayout.GroupByBox.Hidden = !_permissionGrid.DisplayLayout.GroupByBox.Hidden;
            if (_permissionGrid.DisplayLayout.GroupByBox.Hidden)
            {
                _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpShowHide"].Tools["_btnGroupByBox"].InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
            }
            else
            {
                _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpShowHide"].Tools["_btnGroupByBox"].InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
            }
        }

        private void toggleShowSysaminLogins()
        {
            m_HideSysadminLogins = !m_HideSysadminLogins;
            if (m_HideSysadminLogins)
            {
                _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpShowHide"].Tools["_btnShowSysadmins"].InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
            }
            else
            {
                _tlbrsMngr.Ribbon.Tabs["Home"].Groups["_grpShowHide"].Tools["_btnShowSysadmins"].InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
            }

            // Refresh the view.
            ApplicationController.Default.RefreshActiveView();
        }

        private void collapseAllGroups()
        {
            _permissionGrid.Rows.CollapseAll(true);
        }

        private void expandAllGroups()
        {
            _permissionGrid.Rows.ExpandAll(true);
        }

        private void sortSelectedColumnAscending()
        {
            if (m_SelectedColumn != null)
            {
                _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Add(m_SelectedColumn, false, false);
            }
        }

        private void sortSelectedColumnDescending()
        {
            if (m_SelectedColumn != null)
            {
                _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Add(m_SelectedColumn, true, false);
            }
        }

        private void groupBySelectedColumn(bool groupBy)
        {
            if (m_SelectedColumn != null)
            {
                if (groupBy)
                {
                    _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Add(m_SelectedColumn, false, true);
                }
                else
                {
                    _permissionGrid.DisplayLayout.Bands[0].SortedColumns.Remove(m_SelectedColumn);
                }
            }
        }

        private void exportToExcel()
        {
            _saveFileDialog.DefaultExt = "xls";
            _saveFileDialog.FileName = "SQLDM Application Security Configuration";
            _saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            _saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Set header captions that are blank for display.
                _permissionGrid.DisplayLayout.Bands[0].Columns["Enabled"].Header.Caption = "Enabled";
                _permissionGrid.DisplayLayout.Bands[0].Columns["System"].Header.Caption = "System";

                // Export in excel format.
                try
                {
                    _ultraGridExcelExporter.Export(_permissionGrid, _saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                // Reset the header captions to blanks.
                _permissionGrid.DisplayLayout.Bands[0].Columns["Enabled"].Header.Caption = "";
                _permissionGrid.DisplayLayout.Bands[0].Columns["System"].Header.Caption = "";
            }
        }

        private void printGrid()
        {
            _ultraPrintPreviewDialog.Document = _ultraGridPrintDocument;
            _ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            _ultraGridPrintDocument.Header.TextLeft = string.Format("SQLDM Application Security Configuration as of {0}", DateTime.Now.ToString("G"));
            _ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            _ultraPrintPreviewDialog.ShowDialog();
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            Configuration configuration = managementService.GetSecurityConfiguration();
            return configuration;
        }

        public override void UpdateData(object data)
        {
            // Get the configuration object.
            Configuration configuration = data as Configuration;
            Debug.Assert(configuration != null);
            m_IsSecurityEnabled = configuration.IsSecurityEnabled;

            // Display getting started and grid.
            // See if the grouping is expanded, if so expand it after updating grid.
            updatePanel(configuration.Permissions);

            // Update tools based on configuration settings.
            updateTools();

            ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.ApplicationSecurityView);
        }

        private void _gettingStartedEnableSecurityLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            enableSecurity();
        }

        private void _permissionGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // Create value list for system column and attach it.
            ValueList vlSystem = e.Layout.ValueLists.Add("System");
            vlSystem.DisplayStyle = ValueListDisplayStyle.DisplayTextAndPicture;
            ValueListItem vlSystemDefined = vlSystem.ValueListItems.Add(true, "System");
            vlSystemDefined.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SystemDefinedPermission16x16;
            ValueListItem vlUser = vlSystem.ValueListItems.Add(false, "User");
            vlUser.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.UserDefinedPermission16x16;
            e.Layout.Bands[0].Columns["System"].ValueList = vlSystem;
            // set it to text editor so it does not do weird behavior when hovering over this column.
            e.Layout.Bands[0].Columns["System"].Editor = new EditorWithText();

            // Create value list for login type column and attach it.
            ValueList vlLoginType = e.Layout.ValueLists.Add("LoginType");
            vlLoginType.DisplayStyle = ValueListDisplayStyle.DisplayText;
            ValueListItem vlUnknown = vlLoginType.ValueListItems.Add(LoginType.Unknown, "Unknown");
            ValueListItem vlWindowsUser = vlLoginType.ValueListItems.Add(LoginType.WindowsUser, "Windows User");
            ValueListItem vlWindowsGroup = vlLoginType.ValueListItems.Add(LoginType.WindowsGroup, "Windows Group");
            ValueListItem vlSQLLogin = vlLoginType.ValueListItems.Add(LoginType.SQLLogin, "SQL Login");
            e.Layout.Bands[0].Columns["LoginType"].ValueList = vlLoginType;
        }

        private void _permissionGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            updateTools();
        }

        private void _permissionGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = _permissionGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (selectedElement is CheckIndicatorUIElement)
                {
                    UltraGridColumn col = selectedElement.GetContext() as UltraGridColumn;
                    if (col != null )
                    {
                        if (col.Key == "Enabled")
                        {
                            UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;
                            if (selectedRow != null)
                            {
                                enablePermission();
                            }
                        }
                        else if (col.Key == "WebAppPermission")
                        {
                            UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;
                            if (selectedRow != null)
                            {
                                GrantWebAccess();
                            }
                        }
                    }
                }
            }
        }

        private void _permissionGrid_MouseDown(object sender, MouseEventArgs e)
        {
            // set the grid context menu based on where the mouse clicked 
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement = ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                {
                    // Remember the column selected.
                    Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader = contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    m_SelectedColumn = columnHeader.Column;

                    // Set the group by this column image based on column state.
                    ((StateButtonTool)_tlbrsMngr.Tools["_btnGroupByThisColumn"]).Checked = m_SelectedColumn.IsGroupByColumn;

                    // Set context menu.
                    _tlbrsMngr.SetContextMenuUltra((UltraGrid)sender, "HeaderContextMenu");
                }
                else
                {
                    _tlbrsMngr.SetContextMenuUltra((UltraGrid)sender, "GridContextMenu");
                    UltraGridRow row = ((UltraGridRow)selectedElement.SelectableItem);
                    if (row != null)
                    {
                        _permissionGrid.Selected.Rows.Clear();
                        row.Activate();
                        row.Selected = true;
                    }
                }
                updateTools();
            } 

        }

        private void _permissionGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            editPermission();
        }

        private void _tlbrsMngr_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            // Get user token from repository.
            UserToken userToken = new UserToken();
            userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            // Get management service.
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            // Process based on the tool clicked.
            switch (e.Tool.Key)
            {
                case "_btnEnableSecurity":
                    enableSecurity();
                    break;

                case "_btnDisableSecurity":
                    disableSecurity();
                    break;

                case "_btnAddPermission":
                    addPermission();
                    break;

                case "_btnEditPermission":
                    editPermission();
                    break;

                case "_btnDeletePermission":
                    deletePermission();
                    break;

                case "_btnGroupByBox":
                    toggleGroupByBox();
                    break;

                case "_btnEnableDisable":
                    enablePermission();
                    break;

                case "_btnCollapseAllGroups":
                    collapseAllGroups();
                    break;

                case "_btnExpandAllGroups":
                    expandAllGroups();
                    break;

                case "_btnPrint":
                    printGrid();
                    break;

                case "_btnExportToExcel":
                    exportToExcel();
                    break;

                case "_btnShowSysadmins":
                    toggleShowSysaminLogins();
                    break;

                case "_btnSortAscending":
                    sortSelectedColumnAscending();
                    break;

                case "_btnSortDescending":
                    sortSelectedColumnDescending();
                    break;

                case "_btnGroupByThisColumn":
                    groupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;

                default:
                    Debug.Assert(false, "Unknown tool key - " + e.Tool.Key);
                    break;
            }
        }

        private void _tlbrsMngr_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "GridContextMenu")
            {
                // Change the enable/disable text based on permisison status
                if (_permissionGrid.Selected.Rows.Count == 1)
                {
                    PermissionDefinition permission = _permissionGrid.Selected.Rows[0].ListObject as PermissionDefinition;
                    Debug.Assert(permission != null, "Null permission object");
                    _tlbrsMngr.Tools["_btnEnableDisable"].SharedProps.Caption = (permission.Enabled ? "Disable" : "Enable");
                }

                // Enable/disable expand/collapse based on grid state.
                bool isGrouped = isGridGrouped();
                bool isExpanded = isGridExpanded();
                _tlbrsMngr.Tools["_btnExpandAllGroups"].SharedProps.Enabled = isGrouped && !isExpanded;
                _tlbrsMngr.Tools["_btnCollapseAllGroups"].SharedProps.Enabled = isGrouped && isExpanded;
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

