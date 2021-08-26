using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//using ActiproSoftware.SyntaxEditor;
using Infragistics.Win.UltraWinToolbars;
using TracerX;
using Wintellect.PowerCollections;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class OptimizeScriptDialog : Form
    {
        private static Logger LOG = Logger.GetLogger("OptimizeScriptDialog");
        private List<IRecommendation> _recommendations = new List<IRecommendation>();

        //private SqlConnectionInfo _connectionInfo;
        //private string _instance;
        private StringBuilder _text = new StringBuilder();
        private Result _results = null;
        private bool _allowRun = true;
        private int instanceID;
        #region Property
        public List<IRecommendation> Recommendations
        {
            get { return _recommendations; }
            set { _recommendations = value; }
        }
        #endregion

        private OptimizeScriptDialog()
        {
            InitializeComponent();
        }

        internal OptimizeScriptDialog(Result results)
            : this()
        {
            //_instance = instance;
            //Idera.SQLdm.Common.Configuration.ServerConfiguration currentServer = CommonSettings.Default.FindServer(_instance);
            //if (null != currentServer)
            //{
            //    //if (null != currentServer.ConnectionInfo) _connectionInfo = currentServer.ConnectionInfo.Clone();
            //}
            _results = results;
        }

        internal OptimizeScriptDialog(Result results, IRecommendation recommendation)
            : this(results)
        {
            _recommendations.Clear();
            _recommendations.Add(recommendation);
        }

        internal OptimizeScriptDialog(Result results, IEnumerable<IRecommendation> recommendations, bool allowRun, int instanceId)
            : this(results)
        {
            _allowRun = allowRun;
            _recommendations.Clear();
            _recommendations.AddRange(recommendations);
            this.instanceID = instanceId;
            if (!_allowRun)
            {
                btnCancel.Text = "Close";
                btnOptimizeNow.Visible = false;
            }
        }

        private void OptimizeScriptDialog_Load(object sender, EventArgs args)
        {
            try
            {
                System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                using (System.IO.Stream file = thisExe.GetManifestResourceStream("Idera.SQLdm.DesktopClient.ActiproSoftware.SQL.xml"))
                {
                    syntaxEditor1.Document.LoadLanguageFromXml(file, 0);
                    if (null == _recommendations) return;
                    StringBuilder script = new StringBuilder(1024);
                    script.Append(GetScriptHeader());
                    foreach (IRecommendation r in _recommendations)
                    {
                        if (workerLoad.CancellationPending) return;
                        if (null == r) continue;
                        try
                        {
                            if (r is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IScriptGeneratorProvider)
                            {
                                Idera.SQLdm.Common.Services.IManagementService managementService =
                           Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                               Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                                string tsql = managementService.GetPrescriptiveOptimizeScript(instanceID, r);
                                script.AppendLine("--  " + r.FindingText.Replace("\r", "").Replace("\n", ""));
                                script.AppendLine(tsql);
                                if (workerLoad.CancellationPending) return;
                            }
                            else
                            {
                                ApplicationMessageBox.ShowInfo(this,"'"+ r.FindingText +"' does not support optimize query");
                            }
                        }
                        catch (Exception ex)
                        {
                            for (Exception excp = ex; excp != null; excp = excp.InnerException)
                            {
                                script.Append("--    ");
                                script.AppendLine(excp.Message);
                            }
                        }

                        syntaxEditor1.Text = script.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Warn("Failed to load SQL syntax definitions for viewer.  ", e);
            }
            //if (!DesignMode)
            //{
            //    syntaxEditor1.Text = string.Empty;
            //    workerLoad.RunWorkerAsync();
            //    btnOptimizeNow.Enabled = false;
            //    lblLoading.Visible = true;
            //    _loading.Visible = true;
            //    _loading.Active = true;
            //}
            lblLoading.Visible = false;
            _loading.Visible = false;
            _loading.Active = false;
        }

        private void OptimizeScriptDialog_Shown(object sender, EventArgs e)
        {
        }

        private void workerLoad_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //if (null == _recommendations) return;
            //int n = 0;
            //StringBuilder script = new StringBuilder(1024);
            //script.Append(GetScriptHeader());
            //foreach (IRecommendation r in _recommendations)
            //{
            //    if (workerLoad.CancellationPending) return;
            //    if (null == r) continue;
            //    if (r.IsScriptGeneratorProvider)
            //    {
            //        IScriptGenerator generator = ((IScriptGeneratorProvider)r).GetScriptGenerator();
            //        if (generator != null)
            //        {
            //            try
            //            {
            //                script.AppendLine("--  " + r.FindingText.Replace("\r", "").Replace("\n", ""));
            //                string tsql = generator.GetTSqlFix(_connectionInfo);
            //                script.AppendLine(tsql);
            //                if (workerLoad.CancellationPending) return;
            //            }
            //            catch (Exception ex)
            //            {
            //                for (Exception excp = ex; excp != null; excp = excp.InnerException)
            //                {
            //                    script.Append("--    ");
            //                    script.AppendLine(excp.Message);
            //                }
            //            }
            //            script.AppendLine();
            //            workerLoad.ReportProgress(++n, script.ToString());
            //            script.Remove(0, script.Length);
            //        }
            //    }
            //}
        }

        private string GetScriptHeader()
        {
            if (null == _results) return (string.Empty);
            StringBuilder header = new StringBuilder();
            header.AppendLine("-- *******************************************************");
            header.AppendLine("-- This SQL doctor optimization script was created ");
            header.AppendLine("-- based on the recommendations you selected from ");
            header.AppendLine("-- the following analysis: ");
            header.AppendLine("-- ");
            //header.Append("-- SQLdoctor version: ");
            //header.AppendLine(_results.AssemblyVersion);
            header.Append("-- Date: ");
            header.AppendLine(_results.AnalysisStartTime.ToString("D"));
            header.Append("-- Time: ");
            header.AppendLine(_results.AnalysisStartTime.ToString("T"));
            //header.Append("-- SQL Server Instance: ");
            //header.AppendLine(_results.InstanceName);
            //header.Append("-- Analysis Type: "); 
            //header.AppendLine(GetAnalysisType());
            header.AppendLine("-- *******************************************************");
            header.AppendLine();
            header.AppendLine();
            return header.ToString();
        }

        //private string GetAnalysisType()
        //{
        //if (null == _results) return (string.Empty);
        //Idera.SQLdoctor.Common.Values.AnalysisValues av = _results.AnalysisValues;
        //if (av.AdHocBatchAnalysis) 
        //    return "Diagnose Queries";
        //if (string.IsNullOrEmpty(av.TargetCategory))
        //{
        //    StringBuilder analysisTypeString = new StringBuilder();
        //    if (av.RanSnapshotAnalysis) AppendLabel(analysisTypeString, "General Health Check");
        //    if (av.RanWorkloadAnalysis) AppendLabel(analysisTypeString, "Workload Analysis");
        //    if (av.RanDBObjectAnalysis) AppendLabel(analysisTypeString, "Database Object Analysis");
        //    return analysisTypeString.ToString();
        //}
        //else
        //    return string.Format("{0} Analysis", av.TargetCategory);
        //}

        private void AppendLabel(StringBuilder sb, string p)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            sb.Append(p);
        }

        private void workerLoad_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //if (IsDisposed) return;
            //if (null != e.UserState)
            //{
            //    _text.AppendLine(e.UserState.ToString());
            //}
        }

        private void workerLoad_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //if (IsDisposed) return;
            //syntaxEditor1.Text = _text.ToString();
            //btnOptimizeNow.Enabled = _allowRun;
            //btnCopy.Enabled = true;
            //lblLoading.Visible = false;
            //_loading.Visible = false;
            //_loading.Active = false;
        }

        private void OptimizeScriptDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (workerLoad.IsBusy) workerLoad.CancelAsync();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(syntaxEditor1.Document.Text != "" ? syntaxEditor1.Text : "No Text.");
            //Clipboard.SetText(txtActiSyntax.Text != "" ? txtActiSyntax.Text : "No Text");
            ApplicationMessageBox.ShowInfo(FindForm(), "Editor text copied to the clipboard.");
        }

        private void OptimizeScriptDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            //HelpTopics.ShowHelpTopic(HelpTopics.OptimizeNow);
        }

        private void btnOptimizeNow_Click(object sender, EventArgs e)
        {
            Idera.SQLdm.Common.Services.IManagementService managementService =
                          Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                              Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            #region GetMessages
            List<Pair<IRecommendation, List<string>>> recommendationsWithMessages = new List<Pair<IRecommendation, List<string>>>();

            foreach (IRecommendation r in _recommendations)
            {
                if (workerLoad.CancellationPending) return;
                if (null == r) continue;
                try
                {
                    List<string> messages = managementService.GetPrescriptiveOptimizeMessages(instanceID, r);
                    if (messages.Count > 0)
                    {
                        recommendationsWithMessages.Add(new Pair<IRecommendation, List<string>>(r, messages));
                    }

                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                              "A problem occurred while trying to get optimize warning messages. Make sure to verify connectivity to this server or you do not have permissions.");
                }
            }
            if (recommendationsWithMessages.Count > 0)
            {
                WarningListDialog warningDialog = new WarningListDialog(recommendationsWithMessages);
                if (DialogResult.Yes != warningDialog.ShowDialog())
                {
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
            }

            #endregion

            Idera.SQLdm.Common.Configuration.ServerActions.PrescriptiveScriptConfiguration config =
                new Idera.SQLdm.Common.Configuration.ServerActions.PrescriptiveScriptConfiguration(instanceID,
                    Common.Configuration.ServerActions.PrescriptiveScriptType.Optimize, _recommendations);
            var snapshot = managementService.ExecutePrescriptiveOptimization(instanceID, config);
            if (snapshot != null && snapshot.Error != null)
            {
                ApplicationMessageBox.ShowError(this,
                                                      "A problem occurred while trying to execute optimize script on the monitored server. Make sure to verify connectivity to this server or you do not have permissions.",
                                                      snapshot.Error);
            }
            else if (snapshot != null)
            {
                int successCounter = 0;
                int failCounter = 0;
                _recommendations = snapshot.Recommendations;
                foreach (IRecommendation recomm in _recommendations)
                {
                    if (recomm.OptimizationStatus == RecommendationOptimizationStatus.OptimizationCompleted)
                        successCounter++;
                    else
                        failCounter++;
                }
                ApplicationMessageBox.ShowInfo(this, "Optimize script executed successfully. \n"+successCounter + " is/are successfully run. "+failCounter+ " is/are failed.");
            }
            else
            {
                ApplicationMessageBox.ShowError(this,
                                                        "A problem occurred while trying to execute optimize script on the monitored server. Records are not updated or you do not have permissions.");
            }
        }

        //private RecommendationExecutionStatus GetExecutionStatus(IRecommendation r)
        //{
        //    //if (null != _results) return (_results.GetExecutionStatus(r));
        //    return (new RecommendationExecutionStatus(r));
        //}

    }
}
