using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win.UltraWinGrid;
    using Microsoft.SqlServer.MessageBox;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Infragistics.Win;
    using System.Drawing;
    using System.ComponentModel;
    using Wintellect.PowerCollections;
   // using Microsoft.VisualBasic.Logging;
    using System.IO;

    public partial class NotificationRulesViewPanel : UserControl
    {
        public event EventHandler ApplyStateChanged;
        private bool applyNeeded;
        private double lastSplitterPct = .60f;
        public IManagementService managementService;
        private MetricDefinitions metricDefinitions;
        private Dictionary<int, Pair<string, int>> tagMap;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationRuleViewPanel");
        
        public NotificationRulesViewPanel()
        {
            InitializeComponent();
            rulesListView.DrawFilter = new HideFocusRectangleDrawFilter();
            AdaptFontSize();
        }

        public void LoadInstances(IManagementService managementService)
        {
            this.managementService = managementService;
            Reload();
        }

        private int AddRuleToView(NotificationRule rule)
        {
            return rulesBindingSource.Add(new NotificationRuleWrapper(rule));
        }

        public void Reload()
        {
            if (metricDefinitions == null)
            {
                metricDefinitions = new MetricDefinitions(false, false, true);
                metricDefinitions.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            }

            if (tagMap == null)
            {
                tagMap = new Dictionary<int, Pair<string, int>>();

                foreach (Tag tag in ApplicationModel.Default.Tags)
                {
                    tagMap.Add(tag.Id, new Pair<string, int>(tag.Name, tag.Id));
                }
            }

            rulesBindingSource.Clear();

            foreach (NotificationRule rule in managementService.GetNotificationRules())
            {
                if (rule != null)
                    AddRuleToView(rule);
            }

            UpdateControl();            
        }

        public List<NotificationRule> GetRules()
        {
            List<NotificationRule> rules = new List<NotificationRule>();
            foreach (NotificationRuleWrapper rule in rulesBindingSource)
            {
                rules.Add(rule);
            }
            return rules;
        }

        private void addButton_Click(object sender, EventArgs args)
        {
            try
            {
                using (NotificationRuleDialog dialog = new NotificationRuleDialog(managementService, metricDefinitions))
                {
                    //dialog.NotificationRule = new NotificationRule();
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        int listIndex = AddRuleToView(dialog.NotificationRule);
                        UltraGridRow newRow = rulesListView.Rows.GetRowWithListIndex(listIndex);
                        newRow.Selected = true;
                    }
                }
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this.ParentForm, "There was an error adding the action rule.", e);
            }

            UpdateControl();
        }

        private void copyButton_Click(object sender, EventArgs args)
        {
            try
            {
                UltraGridRow row = GetFirstSelectedItem();
                NotificationRuleWrapper selected = (NotificationRuleWrapper) row.ListObject;
                NotificationRule rule = (NotificationRule) selected;

                using (NotificationRuleDialog dialog = new NotificationRuleDialog(managementService, metricDefinitions))
                {
                    // work off a clone
                    dialog.NotificationRule = (NotificationRule)rule.Clone();
                    dialog.NotificationRule.Description += " (Copy)";
                    // default the id so this will be treated as a new object
                    dialog.NotificationRule.Id = default(Guid);
                    // set enabled from the list view item
                    dialog.NotificationRule.Enabled = selected.Enabled;

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        #region Change Log action

                        AuditableEntity entity = dialog.GetAuditableEntity();
                        entity.AddMetadataProperty("EditType", "CopyActionResponse");
                        entity.AddMetadataProperty("EditActionCopiedFrom", rule.Description);

                        AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                        AuditingEngine.SetAuxiliarData("EditActionResponse", entity);

                        #endregion Change Log action

                        managementService.UpdateNotificationRule(dialog.NotificationRule);
                        int listIndex = AddRuleToView(dialog.NotificationRule);
                        UltraGridRow newRow = rulesListView.Rows.GetRowWithListIndex(listIndex);
                        newRow.Selected = true;
                        row.Selected = false;
                    }
                }
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this.ParentForm, "There was an error copying the selected action rule.", e);
            }
            UpdateControl();
        }

        //SQLdm 10.0 (Swati Gogia)
        private void exportButton_Click(object sender, EventArgs args)
        {
            string selectedDirectory = null;
            try
            {
                if (rulesListView.Selected.Rows.Count != 0)
                {
                    using (var sfd = new FolderBrowserDialog())
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            selectedDirectory = sfd.SelectedPath;
                            for (int i = 0; i < rulesListView.Selected.Rows.Count; i++)
                            {                                
                                NotificationRuleWrapper selected = (NotificationRuleWrapper)rulesListView.Selected.Rows[i].ListObject;
                                NotificationRule rule = (NotificationRule)selected;
                                string xml = Idera.SQLdm.DesktopClient.Helpers.NotificationRuleHelper.SerializeNotificationRule(rule);
                                string fileName = rule.Description + ".xml";
                                string fullFileName = Path.Combine(selectedDirectory, fileName);
                                File.WriteAllText(fullFileName, xml);
                            }

                            ApplicationMessageBox.ShowInfo(this, "Selected Alert Response exported to selected directory");
                        }
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(this, "Please Select Alert Response to be exported");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Export Alert Response operation failed : Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Export Alert Response operation failed : ", ex);
            }
        }

        private void importButton_Click(object sender, EventArgs args)
        {
            List<NotificationRule> newRules = null;
            try
            {
                if (NotificationRuleImportWizard.ImportNewRule(this, out newRules) == DialogResult.OK && newRules != null)                
                {
                    foreach (var newRule in newRules)
                    {
                        Guid ruleId = newRule.Id;

                        NotificationRule wrapper = newRule;
                        
                        int listIndex = AddRuleToView(newRule);
                        UltraGridRow newRow = rulesListView.Rows.GetRowWithListIndex(listIndex);
                        newRow.Selected = true;
                        
                    }
                }
                

            }
            catch (Exception ex)
            {
                LOG.Error("Error occurred while importing Alert Responses : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Import Alert Responses operation failed : ", ex);
            }
            
        }

        //END

        private void editButton_Click(object sender, EventArgs args)
        {
            try
            {
                UltraGridRow row = GetFirstSelectedItem();
                NotificationRuleWrapper wrapper = (NotificationRuleWrapper)row.ListObject;
                NotificationRule rule = wrapper;
                                
                using (NotificationRuleDialog dialog = new NotificationRuleDialog(managementService, metricDefinitions))
                {
                    // work off a clone
                    dialog.NotificationRule = (NotificationRule)rule.Clone();                   
                   
                    // set enabled from the list view item
                    dialog.NotificationRule.IsEditCheckAlertRankResponse = true;
                    dialog.NotificationRule.IsMetricsSheverityEdit = true;
                    dialog.NotificationRule.Enabled = wrapper.Enabled;

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        dialog.NotificationRule.IsMetricsSheverityEdit = false;
                        dialog.NotificationRule.IsMetricSheverityDialogOk = false;
                        dialog.NotificationRule.IsEditCheckAlertRankResponse = false;                       
                        wrapper.SetRule(dialog.NotificationRule);
                        row.Refresh();
                        UpdateRulePreview();                      
                    }                    
                }
            } catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this.ParentForm, "There was an error editing the selected action rule.", e);
            }
            UpdateControl();
            UpdateApplyNeeded();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (ApplicationMessageBox.ShowWarning(this.ParentForm,
                                "Delete the selected action rules?",
                                ExceptionMessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            Exception lastException = null;

            
            foreach (UltraGridRow row in rulesListView.Selected.Rows.All)
            {
                NotificationRuleWrapper wrapper = (NotificationRuleWrapper)row.ListObject;
                NotificationRule rule = wrapper;

                try
                {
                    #region Change Log action

                    var entity = new AuditableEntity() {Name = wrapper.Name};

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                    AuditingEngine.SetAuxiliarData("DeletedActionResponse", entity);

                    #endregion Change Log action

                    managementService.DeleteNotificationRule(rule.Id);
                    rulesBindingSource.Remove(wrapper);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            UpdateControl();
            UpdateApplyNeeded();
            UpdateRulePreview();

            if (lastException != null)
            {
                ApplicationMessageBox.ShowError(this.ParentForm, "One or more rules were not deleted.", lastException);
            }
            return;
        }

        private UltraGridRow GetFirstSelectedItem()
        {
            UltraGridRow row = null;

            if (rulesListView.Selected.Rows.Count > 0)
                row = rulesListView.Selected.Rows[0];

            return row;
        }

        private NotificationRuleWrapper GetFirstSelectedRule()
        {
            NotificationRuleWrapper wrapper = null;
            UltraGridRow row = GetFirstSelectedItem();
            if (row != null)
            {
                wrapper = (NotificationRuleWrapper)row.ListObject;
            }
            return wrapper;
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (previewGroupBox.Expanded)
            {
                double position = this.Size.Height * lastSplitterPct;
                splitContainer1.SplitterDistance = (int)Math.Floor(position);
            } else
            {
                lastSplitterPct = ((double)splitContainer1.SplitterDistance) / ((double)this.Size.Height);
                int height = this.Size.Height - previewGroupBox.Height - splitContainer1.SplitterWidth;
                splitContainer1.SplitterDistance = height - 1;
            }
        }

        private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            e.Cancel = !previewGroupBox.Expanded;
        }

        private void UpdateControl()
        {
            if (rulesListView.Rows.Count == 0)
                stackLayoutPanel1.ActiveControl = noRulesLabel;
            else
                stackLayoutPanel1.ActiveControl = rulesListView;

            int selected = rulesListView.Selected.Rows.Count;
            editButton.Enabled = selected == 1;
            copyButton.Enabled = selected == 1;
            removeButton.Enabled = selected > 0;
        }


        private void UpdateRulePreview()
        {
            if (rulesListView.Selected.Rows.Count != 1)
            {
                string message = "";
                rulePreview.DocumentText = String.Format("<html><body><center>{0}</center></body></html>", message);
            }
            else
            {
                rulePreview.DocumentText = GetRuleHtml(GetFirstSelectedRule());
            }
        }

        private string GetRuleHtml(NotificationRule rule)
        {
            string message = rule.Description;
            if (message == null)
                message = "{name is not set}";
            rule.IsClickOnGrid = true;
            return NotificationRuleLabelGenerator.RebuildLinks(managementService, rule, GetProviderTypes(rule), metricDefinitions, true, tagMap);
        }

        private List<NotificationProviderInfo> GetProviderTypes(NotificationRule rule)
        {
            List<NotificationProviderInfo> result = new List<NotificationProviderInfo>();
            List<Type> types = new List<Type>();

            foreach (NotificationDestinationInfo destination in Collections.ToArray(rule.Destinations))
            {
                if (destination.Provider == null)
                    NotificationRuleLabelGenerator.FixupDestinationProvider(managementService, rule);

                NotificationProviderInfo provider = destination.Provider;
                if (provider == null)
                {   // remove goofed up entries
                    rule.Destinations.Remove(destination);
                    continue;
                }

                if (!types.Contains(provider.ProviderType))
                    types.Add(provider.ProviderType);
            }

            foreach (NotificationProviderInfo provider in NotificationProviderInfo.GetAvailableProviders())
            {
                if (types.Contains(provider.ProviderType))
                {
                    result.Add(provider);
                }
            }

            return result;
        }

        private void rulePreview_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "internal")
                e.Cancel = true;
        }

        public bool IsApplyNeeded
        {
            get { return applyNeeded;  }
        }

        private void UpdateApplyNeeded()
        {
            bool result = false;

            foreach (UltraGridRow row in rulesListView.Rows)
            {
                NotificationRuleWrapper wrapper = (NotificationRuleWrapper)row.ListObject;
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
                foreach (UltraGridRow row in rulesListView.Rows)
                {
                    NotificationRuleWrapper wrapper = (NotificationRuleWrapper) row.ListObject;
                    if (wrapper.NeedsUpdate())
                    {
                        NotificationRule rule = wrapper;
                        rule.Enabled = wrapper.Enabled;
                        managementService.UpdateNotificationRule(rule);
                        row.Refresh();
                    }
                }
            } catch (Exception e)
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error was detected while updating a action rules status.", 
                                                e);
            }
            finally
            {
                ParentForm.Cursor = Cursors.Default;
            }
            UpdateApplyNeeded();
        }

        #region Grid Mouse Click

        private void rulesListView_MouseClick(object sender, MouseEventArgs e)
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
                    contextObject = ((UltraGridCell) contextObject).Row;

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

        private void rulesListView_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateRulePreview();
            UpdateControl();
            
        }

        private void rulesListView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            editButton_Click(sender, e);  
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }

    internal class NotificationRuleWrapper
    {
        private NotificationRule rule;
        private bool enabled;

        internal NotificationRuleWrapper(NotificationRule rule)
        {
            SetRule(rule);
        }

        public string Name
        {
            get { return rule.Description; }
        }

        public bool Enabled
        {
            get { return enabled;  }
            set { enabled = value; }
        }

        public static implicit operator NotificationRule(NotificationRuleWrapper wrapper)
        {
            return wrapper.rule;
        }

        public void SetRule(NotificationRule rule)
        {
            this.rule = rule;
            this.enabled = rule.Enabled;
        }

        public bool NeedsUpdate()
        {
            return rule.Enabled != enabled;
        }

        public void Refresh()
        {
            Enabled = rule.Enabled;
        }
    }
}
