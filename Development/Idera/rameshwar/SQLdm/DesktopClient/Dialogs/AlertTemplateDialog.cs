using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.UI.Dialogs;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Common;
    using Helpers;
    using Properties;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Objects;
    using System.IO;


    public partial class AlertTemplateDialog : Form
    {
        /// <summary>
        /// Application logger.
        /// </summary>
        private readonly static Logger Log = Logger.GetLogger("AlertTemplateDialog");

        private const String AffectedServerTemplate = "Affected Server";

        private const String MasterTemplateToApply = "Master Template";

        List<AlertTemplate> alertTemplates;

        #region constructors

        public AlertTemplateDialog()
        {
            InitializeComponent();
            alertTemplatesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            this.AdaptFontSize();
        }

        #endregion

        #region Events

        #region Button Events

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) --  Import alert template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnImport_Click(object sender, System.EventArgs e)
        {
            DialogResult dialogResult = AlertTemplateImportWizard.ImportNewAlertTemplate(this);
            ReloadBindingSource();
        }

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) --  Export alert template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExport_Click(object sender, System.EventArgs e)
        {
            AlertTemplate template = (AlertTemplate)alertTemplatesGrid.Selected.Rows[0].ListObject;
            Idera.SQLdm.Common.Configuration.AlertConfiguration configuration = RepositoryHelper.GetDefaultAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, template.TemplateID);
            AlertTemplateSerializable temp = AlertTemplateHelper.GetAlertTemplateSerializable(template.Name, template.Description, configuration);
            string xml = AlertTemplateHelper.CreateXML(temp);
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "xml files (*.xml)|*.xml";
                sfd.FileName = template.Name + ".xml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, xml);
                        ApplicationMessageBox.ShowInfo(this, "Selected Alert Template Exported Successfully");
                    }
                    catch(Exception ex)
                    {
                        Log.ErrorFormat("Error occurred while exporting {} alert template. Erro details : {1}", template.Name, ex.Message);
                        ApplicationMessageBox.ShowError(this, "Error occurred while exporting selected alert template.");
                    }
                }
            }
            
        }

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) --  Open community site
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkCommunitySite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Common.Constants.CommunitySiteBaseUrl);
        }
        private void btnAddTemplate_Click(object sender, EventArgs e)
        {
            
            AddEditAlertTemplate aeat = new AddEditAlertTemplate();
            DialogResult result = aeat.ShowDialog(this);
            if (DialogResult.OK == result)
            {
                ReloadBindingSource();
            }
        }

        private void btnEditTemplate_Click(object sender, EventArgs e)
        {
            AlertTemplate template = (AlertTemplate)alertTemplatesGrid.Selected.Rows[0].ListObject;
            
            AddEditAlertTemplate aeat = new AddEditAlertTemplate(template);
            if (DialogResult.OK == aeat.ShowDialog(this))
            {
                ReloadBindingSource();
            }
        }

        private void btnApplyTemplate_Click(object sender, EventArgs e)
        {
            if (alertTemplatesGrid.Selected.Rows.Count != 1)
                return;

            AlertTemplate template = (AlertTemplate)alertTemplatesGrid.Selected.Rows[0].ListObject;

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            List<int> excluded = new List<int>();

            foreach (ServerPermission instancePermission in ApplicationModel.Default.UserToken.AssignedServers)
            {
                if (instancePermission.PermissionType == PermissionType.View)
                {
                    excluded.Add(instancePermission.Server.SQLServerID);
                }
            }

            SelectServersDialog dialog =
                new SelectServersDialog("Select the servers or tags to which you want to apply the alert template.", excluded, false);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                IList<int> targetServers = dialog.SelectedServers;
                IList<MetricThresholdEntry> thresholds =
                                managementService.GetDefaultMetricThresholds(template.TemplateID);

                // Audit action
                LogApplyAlertTemplateAction();
                managementService.ReplaceAlertConfiguration(thresholds, targetServers);

               managementService.AddAlertTemplate(template.TemplateID, targetServers);

            }

        }

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            AlertTemplate template = (AlertTemplate)alertTemplatesGrid.Selected.Rows[0].ListObject;

            RepositoryHelper.SetDefaultAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, template.TemplateID);

            ReloadBindingSource();

            //to log audit Set default template
            AuditableEntity auditableEntity = template.GetAuditableEntity();
            auditableEntity.AddMetadataProperty("Template", "Alert template is now set as 'Default Template'");
            LogAuditAction(auditableEntity, AuditableActionType.SetDefaultAlertTemplate);
        }

        private void btnDeleteTemplate_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning(ParentForm,
                                                      "Are you sure you want to delete the selected Alert Template?",
                                                      ExceptionMessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                AlertTemplate template = (AlertTemplate)alertTemplatesGrid.Selected.Rows[0].ListObject;
                RepositoryHelper.DeleteAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, template.TemplateID);
                ReloadBindingSource();
                
                //to log audit delete template
                LogAuditAction(template.GetAuditableEntity(), AuditableActionType.DeleteAlertTemplate, template.Name);
            }

        }

        #endregion

        #region grid events

        private void alertTemplatesGrid_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            UpdateControls();
        }

        #endregion

        #region form events

        private void AlertTemplateDialog_Load(object sender, EventArgs e)
        {
            ReloadBindingSource();
        }

        #endregion

        #endregion

        #region Methods


        /// <summary>
        /// Log the apply alert template audit action.
        /// </summary>
        /// <param name="serverNames">The selected servers names.</param>
        /// 
        private void LogAuditAction(AuditableEntity auditableEntity, AuditableActionType type, params object[] paramList)
        {
            AuditingEngine.Instance.ManagementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            AuditingEngine.Instance.SQLUser =
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
            AuditingEngine.Instance.LogAction(auditableEntity, type, paramList);
        }

        private void ReloadBindingSource()
        {
            alertTemplates = RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            alertTemplatesBindingSource.Clear();

            foreach (AlertTemplate template in alertTemplates)
            {
                alertTemplatesBindingSource.Add(template);
            }

            UpdateControls();
        }

        private void UpdateControls()
        {

            foreach (UltraGridRow row in alertTemplatesGrid.Rows)
            {
                if ((bool)row.Cells["DefaultIndicator"].Value)
                    row.Appearance.FontData.Bold = DefaultableBoolean.True;
            }

            btnEditTemplate.Enabled = btnExport.Enabled =
                btnApplyTemplate.Enabled = (alertTemplatesGrid.Selected.Rows.Count == 1);

            btnSetDefault.Enabled =
                btnDeleteTemplate.Enabled =
                    ((alertTemplatesGrid.Selected.Rows.Count == 1) && !((bool)alertTemplatesGrid.Selected.Rows[0].Cells["DefaultIndicator"].Value));

        }

        #endregion

        private void AlertTemplateDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureAlertTemplates);
        }

        private void AlertTemplateDialog_HelpRequested(object sender, HelpEventArgs e)
        {
            if (e != null) e.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureAlertTemplates);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// Log the apply alert template audit action.
        /// </summary>
        private void LogApplyAlertTemplateAction()
        {
            AlertTemplate alertTemplate = alertTemplatesGrid.Selected.Rows[0].ListObject as AlertTemplate;

            if (alertTemplate != null)
            {
                AuditingEngine.SetAuxiliarData("ApplyAlertTemplateToServer", new AuditAuxiliar<string>(alertTemplate.Name));
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            }
        }

       
    }
}
