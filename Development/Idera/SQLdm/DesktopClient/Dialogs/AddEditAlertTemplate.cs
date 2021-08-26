using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Helpers;
    using Properties;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.DesktopClient;
    using Microsoft.SqlServer.MessageBox;
    using Infragistics.Win.Misc;
    using Objects;

    public partial class AddEditAlertTemplate : Form
    {
        /// <summary>
        /// Application logger.
        /// </summary>
        private readonly static Logger Log = Logger.GetLogger("AlertTemplateDialog");

        private const String AffectedServerTemplate = "Affected Server";

        private const String MasterTemplateName = "Master template";

        private const string BASEDON = "[{0}] {1}";
        private List<string> existingTemplateNames = new List<string>();
        private bool editing = false;
        private AlertTemplate template;
        private bool templateCreated = false;
        //private bool addNewTemplate = false;
        private AlertTemplate oldTemplateInfo = null;
        private bool copying = false;
        private int copyInstance;

        #region constructors

        public AddEditAlertTemplate()
        {
            InitializeComponent();
            Template = new AlertTemplate();
            templateCreated = false;
            //addNewTemplate = false;
            // need save old values information to be compared between templates
            oldTemplateInfo = new AlertTemplate(Template.Name, Template.Description, Template.TemplateID, Template.DefaultIndicator);
            AdaptFontSize();
        }

        public AddEditAlertTemplate(int sourceServer)
        {
            InitializeComponent();
            Template = new AlertTemplate();
            copyInstance = sourceServer;
            copying = true;
            templateCreated = false;
            // need save old values information to be compared between templates
            oldTemplateInfo = new AlertTemplate(Template.Name, Template.Description, Template.TemplateID, Template.DefaultIndicator);
            AdaptFontSize();
        }

        public AddEditAlertTemplate(AlertTemplate template)
        {
            InitializeComponent();
            Template = template;
            // need save old values information to be compared between templates
            oldTemplateInfo = new AlertTemplate(Template.Name, Template.Description, Template.TemplateID,Template.DefaultIndicator);
            templateCreated = editing = true;
            AdaptFontSize();
        }

        #endregion

        #region Properties

        public AlertTemplate Template
        {
            get { return template; }
            set { template = value; }
        }
        #endregion

        #region Events

        #region Button Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            Template.Name = templateName.Text;
            Template.Description = templateDescription.Text;
            Template.DefaultIndicator = chkMakeDefault.Enabled ? chkMakeDefault.Checked : Template.DefaultIndicator;

            //if we select Default template in case create new or edit Template we need audit 
            if(Template.DefaultIndicator)
            {
                //to log audit Set default template
                LogAuditAction(Template.GetAuditableEntity(), AuditableActionType.SetDefaultAlertTemplate);
            }

            if (!editing && !templateCreated)
            {
                SourceType sourceType = DetermineTemplateSourceType(cboBaseTemplate.SelectedItem.DisplayText);
                int sourceInstance = (int)cboBaseTemplate.SelectedItem.DataValue;

                Template.TemplateID =
                    RepositoryHelper.AddAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                      (int)sourceType,
                                                      sourceInstance,
                                                      Template.Name,
                                                      Template.Description);
                templateCreated = true;

                //To log add new alert template
                //addNewTemplate = true;
                LogAuditAction(Template.GetAuditableEntity(), AuditableActionType.AddAlertTemplate, Template.Name);
            }
            else
            {
                RepositoryHelper.UpdateAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                     Template.TemplateID,
                                                     Template.Name,
                                                     Template.Description);

                //To log edit alert template
                LogAuditAction(this.Template.GetAuditableEntity(oldTemplateInfo), AuditableActionType.EditAlertTemplate, Template.Name);
            }
            if (Template.DefaultIndicator)
                RepositoryHelper.SetDefaultAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, Template.TemplateID);

            try
            {
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                ApplicationMessageBox box = new ApplicationMessageBox();
                box.Text = "Would you like to apply this template to existing SQL Server instances?";
                box.ShowCheckBox = false;
                box.Symbol = ExceptionMessageBoxSymbol.Question;
                box.Caption = Text;
                box.Buttons = ExceptionMessageBoxButtons.YesNo;
                if (box.Show(this) == DialogResult.Yes)
                {
                    List<int> excluded = new List<int>();

                    foreach (ServerPermission instancePermission in ApplicationModel.Default.UserToken.AssignedServers)
                    {
                        if (instancePermission.PermissionType == PermissionType.View)
                        {
                            excluded.Add(instancePermission.Server.SQLServerID);
                        }
                    }

                    SelectServersDialog dialog =
                        new SelectServersDialog(
                            "Select the servers to which the alert template should be applied.",
                            excluded, false);

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        IList<int> targetServers = dialog.SelectedServers;

                        IList<MetricThresholdEntry> thresholds = managementService.GetDefaultMetricThresholds(Template.TemplateID);

                        // Log audit action.
                        LogApplyAlertTemplateAction();
                        managementService.ReplaceAlertConfiguration(thresholds, targetServers);
                        managementService.AddAlertTemplate(Template.TemplateID, targetServers);
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "Error applying the template to the selected servers.", exception);
            }

            this.DialogResult = DialogResult.OK;
        }

        //private void AddAuditableCopyAlertTemplate(IList<MetricThresholdEntry> thresholds, IManagementService managementService, IList<int> targetServers)
        //{
        //    if (addNewTemplate)
        //    {
        //        //iterate servers to be copy could be more then one
        //        foreach (int serverId in targetServers)
        //        {
        //            string fromServer = cboBaseTemplate.SelectedItem.DisplayText;
        //            string toServer = ApplicationModel.Default.ActiveInstances[serverId].ToString();

        //            // Log Audit copy alert acction
        //            AuditableEntity auditableEntity = new AuditableEntity();
        //            auditableEntity.Name = Template.Name;
        //            auditableEntity.AddMetadataProperty("Copy Alerts from ", fromServer);
        //            auditableEntity.AddMetadataProperty("Copy Alerts to server name", toServer);
        //            auditableEntity.AddMetadataProperty("Template name ", Template.Name);
        //            LogAuditAction(auditableEntity, AuditableActionType.CopyAlertConfigFromServerToServer, fromServer);

        //            //get threshold for the server to be copy the alert
        //            IList<MetricThresholdEntry> tagetThresholds = managementService.GetMetricThresholds(serverId);
        //            foreach (MetricThresholdEntry sourceThresholds in thresholds)
        //            {
        //                auditableEntity = null;
        //                var thresholdsOldItem = tagetThresholds.Where(item => item.MetricID == sourceThresholds.MetricID);

        //                foreach (MetricThresholdEntry metricThresholdEntry in thresholdsOldItem)
        //                {
        //                    auditableEntity = sourceThresholds.GetAuditableEntity(metricThresholdEntry);

        //                    //if the source alert have diferent configuration from target need to audit other case not
        //                    if (auditableEntity.MetadataProperties.Count > 1)
        //                    {
        //                        // Log Audit copy alert acction 
        //                        LogAuditAction(auditableEntity, AuditableActionType.CopyAlertConfigFromServerToServer,
        //                                       fromServer);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


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

       
        private void btnEditConfig_Click(object sender, EventArgs e)
        {
            Template.Name = templateName.Text;
            Template.Description = templateDescription.Text;
            Template.DefaultIndicator = chkMakeDefault.Enabled ? chkMakeDefault.Checked : Template.DefaultIndicator;

            if (!editing && !templateCreated)
            {
                SourceType sourceType = DetermineTemplateSourceType(cboBaseTemplate.SelectedItem.DisplayText);
                int sourceInstance = (int)cboBaseTemplate.SelectedItem.DataValue;

                Template.TemplateID =
                    RepositoryHelper.AddAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                      (int)sourceType,
                                                      sourceInstance,
                                                      Template.Name,
                                                      Template.Description);

                templateCreated = true;
            }

            AlertConfigurationDialog acd = new AlertConfigurationDialog(Template.TemplateID, true, Template.Name);
            acd.ShowDialog(this);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (templateCreated && (!editing))
                RepositoryHelper.DeleteAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, Template.TemplateID);

            this.DialogResult = DialogResult.Cancel;
        }
            

        #endregion

        #region Form and Control Events

        private void AddEditAlertTemplate_Load(object sender, EventArgs e)
        {
            chkMakeDefault.Checked = false;
            chkMakeDefault.Enabled = true;
            if (editing)
            {
                Text = string.Format(Text, "Edit");
                templateName.Text = Template.Name;
                templateDescription.Text = Template.Description;
                chkMakeDefault.Checked = Template.DefaultIndicator;
                chkMakeDefault.Enabled = !Template.DefaultIndicator;
            }
            else if (copying)
                Text = string.Format(Text, "Copy to");
            else
                Text = string.Format(Text, "Add");

            foreach (AlertTemplate template in RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo))
            {
                existingTemplateNames.Add(template.Name);
            }
            LoadBaseTemplateList();
            UpdateControls();
        }

        private void templateName_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void makeDefault_CheckedChanged(object sender, EventArgs e)
        {
            Template.DefaultIndicator = chkMakeDefault.Checked;
            UpdateControls();
        }

        #endregion

        #region Validation Events

        private void validator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            string value = e.Value == null ? null : e.Value.ToString().Trim();

            e.IsValid = true;

            if (String.IsNullOrEmpty(value))
            {
                e.IsValid = false;
            }
            else
            {
                if (e.ValidationSettings.ValidationGroup.Key == "checkName")
                {
                    if (editing && (Template.Name == value))
                        e.IsValid = true;
                    else if (existingTemplateNames.Contains(value))
                        e.IsValid = false;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        private void LoadBaseTemplateList()
        {
            List<Triple<string, int, string>> sources = RepositoryHelper.GetAlertThresholdOptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            string text;

            if (sources == null || sources.Count == 0)
            {
                cboBaseTemplate.Items.Add(0, "SQLDM Defaults");
                return;
            }

            if (editing)
            {
                text = "";
                text = string.Format(BASEDON, "Template", template.Name);
                cboBaseTemplate.Items.Add(Template.TemplateID,text);
            }
            else if (copying)
            {
                foreach(Triple<string, int, string> source in sources)
                {
                    if (source.First.Equals("Server") && ((int)source.Second == copyInstance))
                    {
                        text = "";
                        text = String.Format(BASEDON, source.First, source.Third);
                        cboBaseTemplate.Items.Add(source.Second, text);
                        break;
                    }
                }
            }
            else
            {
                foreach (Triple<string, int, string> source in sources)
                {
                    text = "";
                    text = string.Format(BASEDON, source.First, source.Third);
                    cboBaseTemplate.Items.Add(source.Second, text);
                }
            }

            cboBaseTemplate.SelectedIndex = 0;
        }


        private SourceType DetermineTemplateSourceType(string templateBase)
        {
            SourceType result = SourceType.Template;

            int start = templateBase.IndexOf("[") + 1;
            int length = templateBase.IndexOf("]") - start;
            string templateSource = templateBase.Substring(start, length).Trim();

            if (templateSource.Equals("Server"))
                result = SourceType.Server;

            return result;
        }

        private void UpdateControls()
        {
            Validation check = validator.Validate("checkName", true, false);
            btnOK.Enabled = btnEditConfig.Enabled = check.IsValid;
            cboBaseTemplate.Enabled = (!editing && !copying);
        }

        #endregion

        private enum SourceType
        {
            Template = 0,
            Server
        }

        private void cboBaseTemplate_ValueChanged(object sender, EventArgs e)
        {
            if (templateCreated && (!editing))
            {
                RepositoryHelper.DeleteAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, Template.TemplateID);
                templateCreated = false;
            }
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
            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("ApplyAlertTemplateToServer", new AuditAuxiliar<string>(Template.Name));
        }
    }
}
