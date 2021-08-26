using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Thresholds;



namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Properties;
    using Objects;

    public partial class SelectAlertTemplateForm : Form
    {
        private const String AffectedServerTemplate = "Affected Server";

        private const String AlertTemplateToApply = "Template Applied";

        private int templateID;
        IList<int> servers = new List<int>();

        private String selectedAlertTemplateText = String.Empty;

        /// <summary>
        /// The server name on which is applied the alert template.
        /// </summary>
        private String selectedTitleName = String.Empty;

        /// <summary>
        /// SelectAlertTemplateForm Constructor. Initialize the dialog according the server id.
        /// </summary>
        /// <param name="serverId">The Server ID on which is applied the alert template.</param>
        public SelectAlertTemplateForm(int serverId)
        {
            var selectedServer  = ApplicationModel.Default.ActiveInstances[serverId];
            selectedTitleName = String.Format(TextConstants.ApplyAlertTemplateTitle, selectedServer.InstanceName);
            InitializeComponent();
            this.informationBox1.Text = TextConstants.ApplyAlertTemplateOverSingleServerDescription;
            this.servers.Add(serverId);
            AdaptFontSize();
        }

        /// <summary>
        /// SelectAlertTemplateForm Constructor. Initialize the dialog.
        /// </summary>
        /// <param name="serverIdList">The Server ID on which is applied the alert template.</param>
        /// <param name="tagName">The Server/Tag  name for title alert template windows.</param>
        public SelectAlertTemplateForm(IList<int> serverIdList,string tagName)
        {
            InitializeComponent();
            selectedTitleName = String.Format(TextConstants.ApplyAlertTemplateTitle, tagName);
            this.informationBox1.Text = TextConstants.ApplyAlertTemplateOverTagDescription;
            foreach (int serverId in serverIdList)
            {
                this.servers.Add(serverId);
            }
            AdaptFontSize();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            templateID = (int)cboAlertTemplates.SelectedItem.DataValue;

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            IList<MetricThresholdEntry> thresholds =
                            managementService.GetDefaultMetricThresholds(templateID);
            
            LogApplyAlertTemplateToServerAction();
            managementService.ReplaceAlertConfiguration(thresholds, servers);
            managementService.AddAlertTemplate(templateID, servers);
            this.selectedAlertTemplateText = this.cboAlertTemplates.SelectedItem.DisplayText;
            foreach (int serverId in servers)
            {
                RepositoryHelper.AddCustomCounters(serverId);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void SelectAlertTemplateForm_Load(object sender, EventArgs e)
        {
            this.Text = selectedTitleName;

            foreach (AlertTemplate template in RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo))
            {
                cboAlertTemplates.Items.Add(template.TemplateID, template.Name);
            }
            cboAlertTemplates.SelectedIndex = 0;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// In charge of calling the AuditableEngine in order to log the action
        /// </summary>
        public void LogApplyAlertTemplateToServerAction()
        {
            String templateName = this.cboAlertTemplates.SelectedItem.DisplayText;
            
            if (templateName != null)
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                AuditingEngine.SetAuxiliarData("ApplyAlertTemplateToServer", new AuditAuxiliar<string>(templateName));
            }
        }
    }
}