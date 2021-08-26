using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win.UltraWinGrid;
    using Microsoft.SqlServer.MessageBox;
    using Infragistics.Win;
    using System.Drawing;
    using System.ComponentModel;

    public partial class NotificationProviderViewPanel : UserControl
    {
        private const string NotificationProviderHasBeenEdited = "Notification Provider '{0}' has been edited.";
        private const string ActionDescriptionText = "Action";

        public event EventHandler ApplyStateChanged;
        private bool applyNeeded;
        private int updating;

        private Type providerType;
        private Type providerEditorType;
        public IManagementService managementService;

        public NotificationProviderViewPanel()
        {
            InitializeComponent();
            providerListView.DrawFilter = new Idera.SQLdm.DesktopClient.Helpers.HideFocusRectangleDrawFilter();

            updating = 0;
            AdaptFontSize();
        }

        public Type ProviderType
        {
            get { return providerType; }
            set
            {
                providerType = value;
                if (providerType != null)
                {
                    // update the ain't got none message
                    string providerTypeName = "";
                    if (providerType == typeof(SmtpNotificationProviderInfo))
                        providerTypeName = "SMTP ";
                    else if (providerType == typeof(SnmpNotificationProviderInfo))
                        providerTypeName = "SNMP ";
                    noProvidersLabel.Text = String.Format(this.noProvidersLabel.Text, providerTypeName);
                }
            }
        }

        public Type ProviderEditorType
        {
            get { return providerEditorType; }
            set { providerEditorType = value; }
        }

        public IManagementService GetManagementService()
        {
            return managementService;
        }

        public void LoadInstances(IManagementService managementService)
        {
            this.managementService = managementService;
            this.providerBindingSource.Clear();
            foreach (NotificationProviderInfo npi in managementService.GetNotificationProviders())
            {
                if (providerType != null && !providerType.IsAssignableFrom(npi.GetType()))
                    continue;

                if (npi is TaskNotificationProviderInfo && !ApplicationModel.Default.IsTasksViewEnabled)
                    continue;

                AddInstanceToView(npi);
            }

            UpdateControl();
        }

        private int AddInstanceToView(NotificationProviderInfo npi)
        {
            int listIndex = 0;
            try
            {
                updating++;
                listIndex = providerBindingSource.Add(new NotificationProviderWrapper(npi));
            } finally
            {
                updating--;
            }
            return listIndex;
        }

        public IList<NotificationProviderInfo> GetNotificationProviders()
        {
            IList<NotificationProviderInfo> result = new List<NotificationProviderInfo>();
            foreach (NotificationProviderWrapper wrapper in providerBindingSource)
            {
                NotificationProviderInfo npi = (NotificationProviderInfo)wrapper;
                result.Add(npi);
            }
            return result;
        }

        private bool IsNameCollision(string name)
        {
            foreach (NotificationProviderWrapper wrapper in providerBindingSource)
            {
                if (wrapper.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public DialogResult DoAdd(IWin32Window owner, ref NotificationProviderInfo notificationProviderInfo)
        {
            DialogResult result = DialogResult.None;

            AddActionProviderWizard addActionProvidersWizard = new AddActionProviderWizard(GetManagementService(), GetNotificationProviders());

            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            if (addActionProvidersWizard.ShowDialog(this) == DialogResult.OK)
            {
                providerListView.Selected.Rows.Clear();
                try
                {
                    NotificationProviderInfo newProvider = addActionProvidersWizard.NotificationProvider;
                    notificationProviderInfo = newProvider;
                    int listIndex = AddInstanceToView(newProvider);
                    // set the new row selected
                    UltraGridRow row = providerListView.Rows.GetRowWithListIndex(listIndex);
                    row.Selected = true;

                    // make sure that the rows show sorted in the grid
                    providerListView.Rows.EnsureSortedAndFiltered();

                    result = DialogResult.OK;

                    if (newProvider is PulseNotificationProviderInfo)
                    {
                        ApplicationModel.Default.RefreshPulseConfiguration();
                    }
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to save the notification provider.", e);
                }
            }
            UpdateControl();
            return result;
        }
 
        private void addButton_Click(object sender, EventArgs args)
        {
            NotificationProviderInfo newProviderInfo = null;
            DoAdd(this, ref newProviderInfo);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            UltraGridRow row = providerListView.Selected.Rows[0];
            NotificationProviderWrapper wrapper = (NotificationProviderWrapper) row.ListObject;
            NotificationProviderInfo original = wrapper;

            // create a copy so we don't modify the original
            NotificationProviderInfo providerInfo = (NotificationProviderInfo)original.Clone();
            // set enabled from the list item (list item could be checked and not applied yet)
            providerInfo.Enabled = wrapper.Enabled;

            ProviderType = wrapper.GetProviderType();
            if (providerType == typeof(SmtpNotificationProviderInfo))
                ProviderEditorType = typeof(SmtpProviderConfigDialog);
            else if (providerType == typeof(SnmpNotificationProviderInfo))
                ProviderEditorType = typeof(SnmpProviderConfigDialog);
            else if (providerType == typeof(PulseNotificationProviderInfo))
                ProviderEditorType = typeof(PulseProviderConfigDialog);
            else
            {
                // Default the editor to an SMTP editor
                ProviderEditorType = typeof(SmtpProviderConfigDialog);
            }

            Form editor = (Form)Activator.CreateInstance(providerEditorType);
            ((INotificationProviderConfigDialog)editor).SetManagementService(GetManagementService());
            ((INotificationProviderConfigDialog)editor).NotificationProvider = providerInfo;

            if (editor.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                AuditableEntity entity = providerInfo.GetAuditableEntity(original);

                wrapper.SetProvider(providerInfo);
                row.Refresh();
                UpdateApplyNeeded();

                if (entity.HasMetadataProperties())
                {
                    // Has changed.
                    entity.AddMetadataProperty(ActionDescriptionText, String.Format(NotificationProviderHasBeenEdited, providerInfo.Name));
                    LogAuditAction(entity, AuditableActionType.EditActionProvider);
                }
            }

            UpdateControl();
        }

        private void removeButton_Click(object sender, EventArgs args)
        {
            Form parentForm = ParentForm;
            if (ApplicationMessageBox.ShowWarning(parentForm, 
                                "Delete the selected notification providers?", 
                                ExceptionMessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    IManagementService managementService = GetManagementService();
                    UltraGridRow[] selectedRows = Collections.ToArray<UltraGridRow>(providerListView.Selected.Rows);
                    foreach (UltraGridRow row in selectedRows)
                    {
                        NotificationProviderWrapper wrapper = (NotificationProviderWrapper)row.ListObject;
                        NotificationProviderInfo npi = wrapper;

                        var entity = new AuditableEntity();
                        entity.Name = npi.Name;
                        entity.AddMetadataProperty("Action", wrapper.Name);
                        AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                        AuditingEngine.SetAuxiliarData("DeleteNotificationActionEntity", entity);

                        managementService.DeleteNotificationProvider(npi.Id);
                        providerBindingSource.Remove(wrapper);
                    }
                }catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(parentForm,
                                                    "Unable to remove the selected notification providers. Please resolve the following error and try again.",
                                                    e);
                }
            }
            ApplicationModel.Default.RefreshPulseConfiguration();
            UpdateControl();
            UpdateApplyNeeded();
        }

        /// <summary>
        /// Log an Audit action. Prepare the 'Auditing Engine' and push the 'Auditable Entity' to logged.
        /// </summary>
        /// <param name="entity">The entity to log.</param>
        /// <param name="actionType">The action type that has been performed.</param>
        private void LogAuditAction(AuditableEntity entity, AuditableActionType actionType)
        {
            AuditingEngine.Instance.ManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            AuditingEngine.Instance.SQLUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
            AuditingEngine.Instance.LogAction(entity, actionType, entity.Name);
        }

        private void UpdateControl()
        {
            if (providerBindingSource.Count == 0)
                stackLayoutPanel1.ActiveControl = noProvidersLabel;
            else
                stackLayoutPanel1.ActiveControl = providerListView;

            // only certain providers (currently smtp and snmp) can be added, edited and removed
            int editableProvidersSelected = 0;
            foreach(UltraGridRow row in providerListView.Selected.Rows)
            {
                NotificationProviderWrapper wrapper = (NotificationProviderWrapper)row.ListObject;
                NotificationProviderInfo provider = wrapper;
                if ((provider is SmtpNotificationProviderInfo) 
                    || (provider is SnmpNotificationProviderInfo)
                    || (provider is PulseNotificationProviderInfo))
                {
                    editableProvidersSelected++;
                }
            }

            int selected = providerListView.Selected.Rows.Count;
            editButton.Enabled = selected == 1 && editableProvidersSelected == 1;
            removeButton.Enabled = selected > 0 && selected == editableProvidersSelected;
        }

        public bool IsApplyNeeded
        {
            get { return applyNeeded; }
        }

        private void UpdateApplyNeeded()
        {
            bool result = false;

            foreach (NotificationProviderWrapper wrapper in providerBindingSource)
            {
                if (wrapper.NeedsUpdate())
                {
                    result = true;
                    break;
                }
            }
            if (result != applyNeeded)
            {
                applyNeeded = result;
                ApplyStateChanged(this, new EventArgs());
            }
        }

        public void ApplyChanges()
        {
            ParentForm.Cursor = Cursors.WaitCursor;
            try
            {
                foreach (NotificationProviderWrapper wrapper in providerBindingSource)
                {
                    if (wrapper.NeedsUpdate())
                    {
                        NotificationProviderInfo provider = wrapper;

                        if (!wrapper.Enabled)
                        {   // disabling the provider
                            NotificationRulesDialog nrd = ParentForm as NotificationRulesDialog;
                            if (nrd != null)
                            {
                                List<NotificationRule> selectedRules = new List<NotificationRule>();
                                foreach (NotificationRule rule in nrd.GetNotificationRules())
                                {
                                    foreach (NotificationDestinationInfo destination in rule.Destinations)
                                    {
                                        if (destination.ProviderID == provider.Id)
                                        {
                                            selectedRules.Add(rule);
                                        }
                                    }
                                }
                                if (selectedRules.Count > 0)
                                {
                                    using (AffectedRulesWarningDialog arwd = new AffectedRulesWarningDialog())
                                    {
                                        arwd.SetNotificationRules(selectedRules);
                                        arwd.InfoText =
                                            String.Format(
                                                "The following notification rules will no longer send notifications using the '{0}' provider.",
                                                provider.Name);

                                        if (arwd.ShowDialog(ParentForm) == DialogResult.Cancel)
                                        {
                                            wrapper.Refresh();
                                            continue;
                                        }
                                    }
                                }
                            }
                        }

                        provider.Enabled = wrapper.Enabled;
                        managementService.UpdateNotificationProvider(provider, false);
                        wrapper.Refresh();
                    }
                    providerListView.Rows.Refresh(RefreshRow.RefreshDisplay);
                }
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error was detected while updating a notification providers status.",
                                                e);
            } 
            finally
            {
                ParentForm.Cursor = Cursors.Default;
            }
            UpdateApplyNeeded();
        }

        private void providerListView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            NotificationProviderWrapper wrapper = (NotificationProviderWrapper)e.Row.ListObject;
            NotificationProviderInfo provider = wrapper;
            if ((provider is SmtpNotificationProviderInfo) 
                || (provider is SnmpNotificationProviderInfo)
                || (provider is PulseNotificationProviderInfo))
            {
                editButton_Click(editButton, e);
            }
        }

        #region Grid Mouse Click

        private void providerListView_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (!(selectedElement is CheckIndicatorUIElement))
                    return;

                // logic to handle toggling a checkbox in a non-editable (no cell selection) column
                object contextObject = selectedElement.GetContext();
                if (contextObject is UltraGridCell)
                    contextObject = ((UltraGridCell)contextObject).Row;

                if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
                {
                    if (((UltraGridColumn)contextObject).Key == "Enabled")
                    {
                        UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;

                        if (selectedRow != null)
                        {
                            bool newValue = true;
                            CurrencyManager cm =
                                ((ICurrencyManagerProvider)grid.DataSource).GetRelatedCurrencyManager(grid.DataMember);
                            PropertyDescriptor descriptor = cm.GetItemProperties()["Enabled"];
                            object selectedObject = selectedRow.ListObject;
                            object value = descriptor.GetValue(selectedRow.ListObject);
                            if (value is bool)
                            {
                                newValue = !((bool)value);
                            }
                            descriptor.SetValue(selectedRow.ListObject, newValue);
                            ApplyChanges();
                        }
                    }
                }
            }
        }
        #endregion

        private void providerListView_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateControl();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }

    internal class NotificationProviderWrapper
    {
        private NotificationProviderInfo provider;
        private bool enabled;
        //private Type providerType;

        internal NotificationProviderWrapper(NotificationProviderInfo provider)
        {
            SetProvider(provider);
        }

        public string Name
        {
            get { return provider.Name; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public static implicit operator NotificationProviderInfo(NotificationProviderWrapper wrapper)
        {
            return wrapper.provider;
        }

        public void SetProvider(NotificationProviderInfo provider)
        {
            this.provider = provider;
            this.enabled = provider.Enabled;
        }

        public Type GetProviderType()
        {
            if (provider != null)
                return this.provider.GetType();
            else
                return null;
        }

        public bool NeedsUpdate()
        {
            return provider.Enabled != enabled;
        }

        public void Refresh()
        {
            Enabled = provider.Enabled;
        }
    }
}
