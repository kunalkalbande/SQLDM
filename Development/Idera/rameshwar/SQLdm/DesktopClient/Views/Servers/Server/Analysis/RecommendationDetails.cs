using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Helpers;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using TracerX;
//------------------------------------------------------------------------------
// <copyright file="RecommendationDetails.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// Author : Srishti Purohit
// Date : 21Jul2015
// Release : SQLdm 10.0
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    public partial class RecommendationDetails : UserControl
    {
        private readonly TracerX.Logger _logX = TracerX.Logger.GetLogger(typeof(RecommendationDetails));

        private Font _fontProperty = new Font("Arial", 8.25f, FontStyle.Bold);
        private Font _fontFinding = new Font("Arial", 12.0f, FontStyle.Bold);
        private Font _fontHeading = new Font("Arial", 9.0f, FontStyle.Bold);
        private Font _fontNormal = new Font("Arial", 9.0f, FontStyle.Regular);
        private Color _colorFinding = Color.FromArgb(59, 89, 152);
        private Color _colorNormal = Color.FromArgb(100, 100, 100);
        private Color _colorProperty = Color.FromArgb(59, 89, 152);
        private const int TEXT_INDENT = 14;
        private const int MAX_DESCRIPTION_LENGTH_ALLOWED = 165;
        private const int TRUNCATE_DESCRIPTION_LENGTH = 160;
        //To log events
        private readonly Logger log = Logger.GetLogger("RecommendationDetails");

        private IRecommendation _recommendation = null;
        public int InstanceID { get; set; }
        public IRecommendation Recommendation
        {
            get { return (_recommendation); }
            set { _recommendation = value; UpdateDetails(); }
        }

        public RecommendationDetails(int instanceID)
        {
            InitializeComponent();
            txtRecommendation.SelectionIndent = 5;
            this.InstanceID = instanceID;
        }

        private void UpdateDetails()
        {
            txtRecommendation.Clear();
            if (null == _recommendation)
            {
                footer.Visible = false;
                return;
            }
            footer.Visible = true;
            priority.Value = Recommendation.ComputedRankFactor;
            lblRecommendationType.Text = Recommendation.ID;
            //lblRecommendationDescription.Text = MasterRecommendations.GetDescription(Recommendation.ID);
            //To truncate description if length is great than 120 . Defect fix DE46013
            if (_recommendation.DescriptionText.Length > MAX_DESCRIPTION_LENGTH_ALLOWED)
            {
                lblRecommendationDescription.Text = string.Format("{0}...", _recommendation.DescriptionText.Substring(0, TRUNCATE_DESCRIPTION_LENGTH));
            }
            else
            {
                lblRecommendationDescription.Text = _recommendation.DescriptionText;
            }

            BuildRecommendationText();
            BuildRecommendationProperties();
            BuildSourceObjectLinks();
            BuildAffectedBatches();
            BuildResourceLinks();
        }

        private void BuildRecommendationProperties()
        {
            int start = txtRecommendation.SelectionStart;
            var ipd = Recommendation as IProvideDatabase;
            var ipt = Recommendation as IProvideTableName;
            var ipa = Recommendation as IProvideApplicationName;
            var ipu = Recommendation as IProvideUserName;
            var iph = Recommendation as IProvideHostName;

            if (null != ipd) AppendProperty("Database:", ipd.Database);
            if (null != ipt)
            {
                string table = string.IsNullOrEmpty(ipt.Schema) ? ipt.Table : string.Format("{0}.{1}", ipt.Schema, ipt.Table);
                if (!String.IsNullOrEmpty(table))
                {
                    AppendProperty("Table:", table);
                }
            }
            if (null != ipa) AppendProperty("Application:", ipa.ApplicationName);
            if (null != ipu) AppendProperty("Login:", ipu.UserName);
            if (null != iph) AppendProperty("Workstation:", iph.HostName);

            txtRecommendation.SelectionColor = _colorNormal;
            txtRecommendation.SelectionFont = _fontNormal;
            if (start != txtRecommendation.SelectionStart) txtRecommendation.AppendText(Environment.NewLine);
        }

        private void AppendProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            txtRecommendation.SelectionFont = _fontProperty;
            txtRecommendation.SelectionColor = _colorNormal;
            txtRecommendation.AppendText(name);
            if (name.Length < 20) txtRecommendation.AppendText(new string(' ', 20 - name.Length));
            txtRecommendation.AppendText("\t");
            txtRecommendation.SelectionColor = _colorProperty;
            txtRecommendation.AppendText(value);
            txtRecommendation.SelectionColor = _colorNormal;
            txtRecommendation.AppendText(Environment.NewLine);
        }

        private void BuildRecommendationText()
        {
            int indent = txtRecommendation.SelectionIndent;

            txtRecommendation.SelectionFont = _fontFinding;
            txtRecommendation.SelectionColor = _colorFinding;
            txtRecommendation.AppendText(_recommendation.FindingText);
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.AppendText(Environment.NewLine);

            txtRecommendation.SelectionColor = _colorNormal;
            txtRecommendation.SelectionFont = _fontNormal;
            txtRecommendation.AppendText(_recommendation.ImpactExplanationText);
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.AppendText(Environment.NewLine);
            if (!string.IsNullOrEmpty(_recommendation.AdditionalConsiderations))
            {
                txtRecommendation.SelectionFont = _fontHeading;
                txtRecommendation.AppendText("When is this not a problem?");
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionFont = _fontNormal;
                txtRecommendation.SelectionBullet = true;
                txtRecommendation.SelectionIndent = TEXT_INDENT;
                txtRecommendation.AppendText(PAFormatHelper.RemoveBullets(_recommendation.AdditionalConsiderations));
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionBullet = false;
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionIndent = indent;
            }
            if (!string.IsNullOrEmpty(_recommendation.ProblemExplanationText))
            {
                txtRecommendation.SelectionFont = _fontHeading;
                txtRecommendation.AppendText("Why is this a problem?");
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionFont = _fontNormal;
                txtRecommendation.SelectionBullet = true;
                txtRecommendation.SelectionIndent = TEXT_INDENT;
                txtRecommendation.AppendText(PAFormatHelper.RemoveBullets(_recommendation.ProblemExplanationText));
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionBullet = false;
                txtRecommendation.AppendText(Environment.NewLine);
                txtRecommendation.SelectionIndent = indent;
            }
            txtRecommendation.SelectionFont = _fontHeading;
            txtRecommendation.AppendText("Recommendation:");
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.SelectionFont = _fontNormal;
            txtRecommendation.AppendText(_recommendation.RecommendationText);
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.AppendText(Environment.NewLine);
        }

        private void BuildResourceLinks()
        {
            if (null == Recommendation) return;
            RecommendationLinks links = Recommendation.Links;
            bool added = false;
            if (links == null)
                return;
            if (links != null) links = links.Filtered(Recommendation, GetProductVersion(), string.Empty);
            if (links != null)
            {
                string url;
                string title;
                foreach (RecommendationLink link in links)
                {
                    url = String.IsNullOrEmpty(link.Link) ? link.CondensedLink : link.Link;
                    if (String.IsNullOrEmpty(url)) continue;

                    title = link.Title;
                    if (String.IsNullOrEmpty(title)) title = url;
                    if (!added)
                    {
                        AddLinkSection("Learn more about:");
                        added = true;
                    }
                    AddLink(title, link);
                }
            }
            if (added) txtRecommendation.AppendText(Environment.NewLine);

            if (Recommendation is DeadlockRecommendation)
            {
                string xdl = ((DeadlockRecommendation)Recommendation).XDL;
                if (!String.IsNullOrEmpty(xdl))
                {
                    AddLinkSection("Additional details:");
                    AddLink("Deadlock Details", Recommendation);
                }
            }
        }
        //To get product version of current selected instance
        private string GetProductVersion()
        {
            MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[InstanceID];
            var productVersion = string.Empty;
            if (instance != null)
            {
                try
                {
                    if (instance.MostRecentSQLVersion == null)
                    {
                        if (instance.ConnectionInfo != null)
                        {
                            //SQLdm 10.0.2 (Barkha Khatri) get Product version in background
                            productVersion = ManagementServiceHelper.GetDefaultService().GetProductVersion(instance.Id).Version;
                            //productVersion = RepositoryHelper.GetProductVersion(instance.ConnectionInfo.ConnectionString).Version;

                        }
                    }
                    else
                        productVersion = instance.MostRecentSQLVersion.DisplayVersion;
                    return productVersion;

                }
                catch (Exception ex)
                {
                    log.Error("Error in getting prodcut version for RecommendationDetails links. " + ex);
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        private void BuildSourceObjectLinks()
        {
            if (null == Recommendation) return;
            IEnumerable<DatabaseObjectName> names = Recommendation.SourceObjects;
            bool added = false;
            if (names == null) return;
            foreach (DatabaseObjectName name in names)
            {
                if (null == name) continue;
                if (String.IsNullOrEmpty(name.ObjectName)) continue;

                if (!added)
                {
                    AddLinkSection("Dependent objects:");
                    added = true;
                }
                AddLink(name.ObjectName, name);
            }
            if (added) txtRecommendation.AppendText(Environment.NewLine);
        }

        private void BuildAffectedBatches()
        {
            var ipab = Recommendation as IProvideAffectedBatches;
            if (null == ipab) return;
            // Start: SQLDm 10.0 - Srishti Purohit - New Recommendations - changing title text of link section based on the Interface
            var ipqb = Recommendation as IProvideQueryBatches;
            string linkSectionTitle = "Affected batches:";
            if (null != ipqb) { linkSectionTitle = "List of Queries:"; }
            //End: SQLDm 10.0 - Srishti Purohit - New Recommendations - changing title text of link section based on the Interface

            AffectedBatches abs = ipab.GetAffectedBatches();
            if (null == abs) return;
            bool added = false;
            foreach (AffectedBatch ab in abs)
            {
                if (null == ab) continue;
                if (string.IsNullOrEmpty(ab.Name)) continue;
                if (!added)
                {
                    AddLinkSection(linkSectionTitle); //SQLDm 10.0 - Srishti Purohit- New Recommendations
                    added = true;
                }
                AddLink(ab.Name, ab);
            }
            if (added) txtRecommendation.AppendText(Environment.NewLine);
        }

        private void AddLink(string title, object data)
        {
            txtRecommendation.SelectionFont = _fontProperty;
            txtRecommendation.AppendText("\t");
            txtRecommendation.InsertLink(title, data);
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.SelectionFont = _fontNormal;
        }

        private void AddLinkSection(string p)
        {
            txtRecommendation.SelectionFont = _fontProperty;
            txtRecommendation.AppendText(p);
            txtRecommendation.AppendText(Environment.NewLine);
            txtRecommendation.SelectionFont = _fontNormal;
        }

        private void txtRecommendation_ObjectLinkClicked(object sender, LinkClickedEventArgs e, object linkObject)
        {
            if (HandleLinkClick(linkObject as RecommendationLink)) return;
            if (HandleLinkClick(linkObject as DeadlockRecommendation)) return;
            if (HandleLinkClick(linkObject as DatabaseObjectName)) return;
            if (HandleLinkClick(linkObject as AffectedBatch)) return;
        }

        private bool HandleLinkClick(AffectedBatch ab)
        {
            if (null == ab) return (false);
            using (ShowSqlDialog ssd = new ShowSqlDialog())
            {
                ssd.TSQL = ab.Batch;
                ssd.ShowDialog(FindForm());
            }
            return (true);
        }

        private bool HandleLinkClick(DatabaseObjectName don)
        {
            using (_logX.DebugCall("HandleLinkClick"))
            {
                if (null == don) return (false);
                _logX.Debug("DatabaseObjectName:" + don);
                using (DependentObjectDialog dod = new DependentObjectDialog(InstanceID))
                {
                    dod.ObjectName = don;
                    _logX.Debug("Show DependentObjectDialog");
                    dod.ShowDialog(FindForm());
                }
                return (true);
            }
        }
        private bool HandleLinkClick(DeadlockRecommendation r)
        {
            if (null == r) return (false);
            DeadlockDialog.Show(FindForm(), r);
            return (true);
        }
        private bool HandleLinkClick(RecommendationLink link)
        {
            if (null == link) return (false);
            string url = String.IsNullOrEmpty(link.Link) ? link.CondensedLink : link.Link;
            url = url.Trim();
            if (!String.IsNullOrEmpty(url))
            {
                try
                {
                    if (url.StartsWith("http:") || url.StartsWith("https:"))
                    {
                        System.Diagnostics.Process.Start(url);
                    }
                    else
                    {
                        //HelpTopics.ShowHelpTopic(url);
                    }
                }
                catch (Exception ex)
                {
                    _logX.Error("RecommendationDetailsControl.Resource_LinkClicked() Exception: ", ex);
                    throw ex;
                }
            }
            return (true);
        }

        private void menuRichText_Opening(object sender, CancelEventArgs e)
        {
            menuCopyRecommendationText.Enabled = !(string.IsNullOrEmpty(txtRecommendation.SelectedText) && string.IsNullOrEmpty(txtRecommendation.SelectedRtf));
            menuSelectAll.Enabled = !string.IsNullOrEmpty(txtRecommendation.Text);
        }

        private void menuCopyRecommendationText_Click(object sender, EventArgs e)
        {
            txtRecommendation.Copy();
        }

        private void menuSelectAll_Click(object sender, EventArgs e)
        {
            txtRecommendation.SelectAll();
        }

    }
}
