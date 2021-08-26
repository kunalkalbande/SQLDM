using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Query
{
    using System;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Helpers;
    using System.ComponentModel;
    using System.IO;

    //using QueryPlanParser.Utilities;

    public partial class QueryPlanViewerDialog : Form
    {
        private string PlanHandle { get; set; }
        public QueryPlanViewerDialog(string planHandle)
        {
            if (null == System.Windows.Application.Current)
            {
                new System.Windows.Application();
            }
            PlanHandle = planHandle;
            InitializeComponent();
            // Autoscale fontsize.
            AdaptFontSize();
        }

        private void QueryPlanForm_Load(object sender, EventArgs e)
        {

            //// Initialize
            //var xmlInput = File.ReadAllText(@"F:\Projects\Idera\SQLdm\10.4\QueryPlanPoC\QueryPlanParserTests\ExecutionPlans\sql2016sp1\QueryPlanXML.xml");
            //var xmlInputTwoStatements = File.ReadAllText(@"F:\Projects\Idera\SQLdm\10.4\QueryPlanPoC\QueryPlanParserTests\ExecutionPlans\sql2016sp1\twoSelectStatements.xml");
            //var xmlInputSimple = File.ReadAllText(@"F:\Projects\Idera\SQLdm\10.4\QueryPlanPoC\QueryPlanParserTests\ExecutionPlans\sql2016sp1\SimpleSelect.xml");
            //var xmlInputSimpleMultipleRows = File.ReadAllText(@"F:\Projects\Idera\SQLdm\10.4\QueryPlanPoC\QueryPlanParserTests\ExecutionPlans\sql2016sp1\basicSqlPlan.sqlplan");

            //var showPlan = Utilities.GetShowPlan<QueryPlanPoC.Resources.Classes.sql2016sp1.ShowPlanXML>(this.PlanHandle);

            //this.queryPlanViewerUserControl1.Fill(showPlan);
            //Dummy control
            this.queryPlanViewerUserControl1.FillText(PlanHandle);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureNotificationRulesAndProviders);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureNotificationRulesAndProviders);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

   }

}