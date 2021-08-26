using System.Windows;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.UI.Dialogs;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Wintellect.PowerCollections;
using System.Collections.ObjectModel;
using System.Linq;
using Idera.SQLdm.DesktopClient.Helpers;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    partial class ReportsView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.reportToolsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.reportsToolStrip = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomReportHeaderStrip();
            this.backDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.ToolStripButton();
            this.gettingStartedButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleFilterPanelButton = new System.Windows.Forms.ToolStripButton();
            this.resetFiltersButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deployActionButton = new System.Windows.Forms.ToolStripButton();
            this.scheduleEmailButton = new System.Windows.Forms.ToolStripButton();
            this.addReportButton = new System.Windows.Forms.ToolStripButton();
            this.editReportButton = new System.Windows.Forms.ToolStripButton();
            this.deleteReportButton = new System.Windows.Forms.ToolStripButton();
            this.importReportButton = new System.Windows.Forms.ToolStripButton();
            this.exportReportButton = new System.Windows.Forms.ToolStripButton();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip(false, true);
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.reportToolsPanel.SuspendLayout();
            this.reportsToolStrip.SuspendLayout();
            this.headerStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 57);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(791, 475);
            this.contentPanel.TabIndex = 1;
            // 
            // reportToolsPanel
            // 
            this.reportToolsPanel.Controls.Add(this.label1);
            this.reportToolsPanel.Controls.Add(this.reportsToolStrip);
            this.reportToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportToolsPanel.Location = new System.Drawing.Point(0, 25);
            this.reportToolsPanel.Name = "reportToolsPanel";
            this.reportToolsPanel.Size = new System.Drawing.Size(791, 32);
            this.reportToolsPanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(791, 2);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // reportsToolStrip
            // 
            this.reportsToolStrip.AutoSize = false;
            this.reportsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.reportsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backDropDownButton,
            this.forwardButton,
            this.toolStripSeparator1,
            this.refreshButton,
            this.cancelButton,
            this.gettingStartedButton,
            this.toolStripSeparator2,
            this.toggleFilterPanelButton,
            this.resetFiltersButton,
            this.toolStripSeparator3,
            this.deployActionButton,
            this.scheduleEmailButton,
            this.toolStripSeparator4,
            this.addReportButton,
            this.editReportButton,
            this.deleteReportButton,
            this.importReportButton,
            this.exportReportButton});
            this.reportsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.reportsToolStrip.Name = "reportsToolStrip";
            this.reportsToolStrip.Size = new System.Drawing.Size(791, 30);
            this.reportsToolStrip.TabIndex = 3;
            // 
            // backDropDownButton
            // 
            this.backDropDownButton.AutoToolTip = false;
            this.backDropDownButton.Enabled = false;
            this.backDropDownButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Back24x24;
            this.backDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.backDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backDropDownButton.Name = "backDropDownButton";
            this.backDropDownButton.Size = new System.Drawing.Size(72, 27);
            this.backDropDownButton.Text = "Back";
            this.backDropDownButton.ButtonClick += new System.EventHandler(this.backDropDownButton_ButtonClick);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Forward24x24;
            this.forwardButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(28, 27);
            this.forwardButton.Text = "Forward";
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Refresh24x24;
            this.refreshButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(28, 27);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cancelButton.Enabled = false;
            this.cancelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Cancel24x24;
            this.cancelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(28, 27);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            //deployActionButton
            //
            this.deployActionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deployActionButton.Enabled = true;
            this.deployActionButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Deploy24x24;
            this.deployActionButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deployActionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deployActionButton.Name = "deployActionButton";
            this.deployActionButton.Size = new System.Drawing.Size(28, 27);
            this.deployActionButton.Text = "Deploy Report";
            this.deployActionButton.Click += new System.EventHandler(this.DeployCustomReport_Click);

            //
            //scheduleEmailButton
            //
            this.scheduleEmailButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.scheduleEmailButton.Enabled = true;
            this.scheduleEmailButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Schedule24x24;
            this.scheduleEmailButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.scheduleEmailButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.scheduleEmailButton.Name = "scheduleEmailButton";
            this.scheduleEmailButton.Size = new System.Drawing.Size(28, 27);
            this.scheduleEmailButton.Text = "Schedule Email";
            this.scheduleEmailButton.Click += new System.EventHandler(this.ScheduleEmailCustomReport_Click);
            //
            //addReportButton
            //
            this.addReportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addReportButton.Enabled = false;
            this.addReportButton.Available = false;
            this.addReportButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.New24x24;
            this.addReportButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addReportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addReportButton.Name = "addReportButton";
            this.addReportButton.Size = new System.Drawing.Size(28, 27);
            this.addReportButton.Text = "Add Report";
            this.addReportButton.Click += new System.EventHandler(this.NewCustomReport_Click);

            //
            //editReportButton
            //
            this.editReportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editReportButton.Enabled = false;
            this.editReportButton.Available = false;
            this.editReportButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Edit24x24;
            this.editReportButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.editReportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editReportButton.Name = "editReportButton";
            this.editReportButton.Size = new System.Drawing.Size(28, 27);
            this.editReportButton.Text = "Edit Report";
            this.editReportButton.Click += new System.EventHandler(this.EditCustomReport_Click);
            //
            //deleteReportButton
            //
            this.deleteReportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteReportButton.Enabled = false;
            this.deleteReportButton.Available = false;
            this.deleteReportButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete24x24;
            this.deleteReportButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deleteReportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteReportButton.Name = "deleteReportButton";
            this.deleteReportButton.Size = new System.Drawing.Size(28, 27);
            this.deleteReportButton.Text = "Delete Report";
            this.deleteReportButton.Click += new System.EventHandler(this.DeleteCustomReport_Click);

            //
            //importReportButton
            //
            this.importReportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.importReportButton.Enabled = false;
            this.importReportButton.Available = false;
            this.importReportButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Import24x24;
            this.importReportButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.importReportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importReportButton.Name = "importReportButton";
            this.importReportButton.Size = new System.Drawing.Size(28, 27);
            this.importReportButton.Text = "Import Report";
            this.importReportButton.Click += new System.EventHandler(this.ImportCustomReport_Click);
            //
            //exportReportButton
            //
            this.exportReportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportReportButton.Enabled = false;
            this.exportReportButton.Available = false;
            this.exportReportButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export24x24;
            this.exportReportButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.exportReportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportReportButton.Name = "exportReportButton";
            this.exportReportButton.Size = new System.Drawing.Size(28, 27);
            this.exportReportButton.Text = "Export Report";
            this.exportReportButton.Click += new System.EventHandler(this.ExportCustomReport_Click);
            // 
            // gettingStartedButton
            // 
            this.gettingStartedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.gettingStartedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Home24x24;
            this.gettingStartedButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.gettingStartedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gettingStartedButton.Name = "gettingStartedButton";
            this.gettingStartedButton.Size = new System.Drawing.Size(28, 27);
            this.gettingStartedButton.Text = "Getting Started";
            this.gettingStartedButton.Click += new System.EventHandler(this.gettingStartedButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 30);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 30);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // toggleFilterPanelButton
            // 
            this.toggleFilterPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleFilterPanelButton.AutoToolTip = false;
            this.toggleFilterPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.OptionsGlyphUp16x16;
            this.toggleFilterPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleFilterPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleFilterPanelButton.Name = "toggleFilterPanelButton";
            this.toggleFilterPanelButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toggleFilterPanelButton.Size = new System.Drawing.Size(79, 27);
            this.toggleFilterPanelButton.Text = "Hide Filters";
            this.toggleFilterPanelButton.Click += new System.EventHandler(this.toggleFilterPanelButton_Click);
            // 
            // resetFiltersButton
            // 
            this.resetFiltersButton.AutoToolTip = false;
            this.resetFiltersButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Clear24x24;
            this.resetFiltersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetFiltersButton.Name = "resetFiltersButton";
            this.resetFiltersButton.ImageScaling = ToolStripItemImageScaling.None;
            this.resetFiltersButton.Size = new System.Drawing.Size(105, 27);
            this.resetFiltersButton.Text = "Reset Filters";
            this.resetFiltersButton.Click += new System.EventHandler(this.resetFiltersButton_Click);
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(66)))), ((int)(((byte)(16)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(791, 25);
            this.headerStrip.TabIndex = 0;
            this.headerStrip.Text = "headerStrip1";
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(70, 20);
            this.titleLabel.Text = "Reports";
            // 
            // ReportsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.reportToolsPanel);
            this.Controls.Add(this.headerStrip);
            this.Name = "ReportsView";
            this.Size = new System.Drawing.Size(791, 532);
            this.reportToolsPanel.ResumeLayout(false);
            this.reportsToolStrip.ResumeLayout(false);
            this.reportsToolStrip.PerformLayout();
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentPanel;
        private System.Windows.Forms.ToolStrip reportsToolStrip;
        private System.Windows.Forms.ToolStripSplitButton backDropDownButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton cancelButton;
        private System.Windows.Forms.ToolStripButton gettingStartedButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toggleFilterPanelButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  reportToolsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton resetFiltersButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton deployActionButton;
        private System.Windows.Forms.ToolStripButton scheduleEmailButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton addReportButton;
        private System.Windows.Forms.ToolStripButton editReportButton;
        private System.Windows.Forms.ToolStripButton deleteReportButton;
        private System.Windows.Forms.ToolStripButton importReportButton;
        private System.Windows.Forms.ToolStripButton exportReportButton;

        private void NewCustomReport_Click(object sender, EventArgs e)
        {
            this.NewCustomReport();

            // e.Handled = true;
        }
        public void NewCustomReport()
        {
            CustomReportWizard wizard = new CustomReportWizard();

            wizard.ShowDialog(this._window);
        }
        private void DeleteCustomReport_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.DeleteCustomReport();
        }

        private void PopulateActiveCustomReport()
        {
            ReportTypes report = ((ReportsView)ApplicationController.Default.ActiveView).ActiveReport.ReportType;
            if (report != Idera.SQLdm.DesktopClient.Views.Reports.ReportTypes.Custom)
            {
                selectedCustomReport = null;
            }

            switch (report)
            {
                case Idera.SQLdm.DesktopClient.Views.Reports.ReportTypes.Custom:
                    selectedCustomReport = ((ReportsView)ApplicationController.Default.ActiveView).ActiveReport.CustomReportName;

                    break;
                default:
                    break;
            }
        }
        private ObservableCollection<string> customReports = new ObservableCollection<string>();
        public ObservableCollection<string> CustomReports
        {
            get { return customReports; }
            set { customReports = value; }
        }
        #region CustomReports
        private const string DELETECUSTOMREPORT =
            "Are you sure you would like to delete \"{0}\"?.  This action cannot be undone.";
        public void DeleteCustomReport()
        {
            if (String.IsNullOrWhiteSpace(selectedCustomReport)) return;

            if (ApplicationMessageBox.ShowQuestion(_window, string.Format(DELETECUSTOMREPORT, selectedCustomReport)) != DialogResult.Yes) return;

            CustomReports.Remove(selectedCustomReport);
            
        }
        private void ImportCustomReport_Click(object sender, EventArgs e)
        {
            this.ImportCustomReport();

            //e.Handled = true;
        }
        public void ImportCustomReport()
        {
            Idera.SQLdm.Common.Objects.CustomReport report;
            DialogResult dialogResult = CustomReportImportWizard.ImportNewReport(_window, out report);

            //FirePropertyChanged("HasNoCustomReports");
            //FirePropertyChanged("CustomReports");
            //FirePropertyChanged("ReportIsSelected");

        }

        private void ExportCustomReport_Click(object sender, EventArgs e)
        {
            PopulateActiveCustomReport();
            this.ExportCustomReport();

            //e.Handled = true;
        }
        public void ExportCustomReport()
        {
            var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            var managementService = Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(connectionInfo);
            var selectedReport = selectedCustomReport;
            if (selectedReport != null)
            {
                try
                {
                    var _CurrentCustomReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                selectedReport);

                    //includes aggregation that may have been set up previously
                    var _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                    //populate the counters that have already been selected
                    if (_CurrentCustomReport.Metrics == null) _CurrentCustomReport.Metrics = new SortedDictionary<int, Idera.SQLdm.Common.Objects.CustomReportMetric>();

                    _CurrentCustomReport.Metrics.Clear();

                    foreach (System.Data.DataRow row in _selectedCountersDataTable.Rows)
                    {
                        string metricName = row["CounterName"].ToString();
                        string metricDescription = row["CounterShortDescription"].ToString();

                        //_selectedCounters.Add(metricName, metricDescription);

                        Idera.SQLdm.Common.Objects.CustomReport.CounterType type = (Idera.SQLdm.Common.Objects.CustomReport.CounterType)int.Parse(row["CounterType"].ToString());
                        Idera.SQLdm.Common.Objects.CustomReport.Aggregation aggregation = (Idera.SQLdm.Common.Objects.CustomReport.Aggregation)int.Parse(row["Aggregation"].ToString());

                        _CurrentCustomReport.Metrics.Add(int.Parse(row["GraphNumber"].ToString()),
                                    new Idera.SQLdm.Common.Objects.CustomReportMetric(metricName, metricDescription, type, aggregation));

                    }

                    string xml = Idera.SQLdm.DesktopClient.Helpers.CustomReportHelper.SerializeCustomReport(_CurrentCustomReport);

                    using (var sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "xml files (*.xml)|*.xml";
                        sfd.FileName = _CurrentCustomReport.Name + ".xml";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllText(sfd.FileName, xml);
                            ApplicationMessageBox.ShowInfo(_window, "Selected custom report exported successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(_window, "Export operation failed for selected report: ", ex);
                }
            }
        }
        private void EditCustomReport_Click(object sender, EventArgs e)
        {
            PopulateActiveCustomReport();
            this.EditSelectedCustomReport();

            //e.Handled = true;
        }

        public void EditSelectedCustomReport()
        {
            if (String.IsNullOrWhiteSpace(selectedCustomReport))
                throw new NullReferenceException("Must provide a custom report name.");

            TreeNode tn = new TreeNode(selectedCustomReport);
            tn.Name = selectedCustomReport;

            var p = new Pair<ReportsNavigationPane.ReportCategory, ReportTypes>();
            p.First = ReportsNavigationPane.ReportCategory.Custom;

            tn.Tag = p;

            CustomReportWizard wizard = new CustomReportWizard(tn);

            if (wizard.IsDisposed) return;

            wizard.ShowDialog(_window);

            // FirePropertyChanged("HasNoCustomReports");
            // FirePropertyChanged("CustomReports");
            // FirePropertyChanged("ReportIsSelected");
        }

        private void DeployCustomReport_Click(object sender, EventArgs e)
        {
            PopulateActiveCustomReport();
            RoutedEventArgs r;
            this.DeployCustomReport();

            //e.Handled = true;
        }
        // not sure about the business logic behind the below two fields and methods but keeping them for now
        private MainWindow _window = null;
        private MainWindowViewModel _viewModel;
        private string selectedCustomReport = null;

        public void ScheduleEmailCustomReport()
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol reportControl = ((ReportsView)ApplicationController.Default.ActiveView).ActiveReport;

                string message;

                if (reportControl.CanRunReport(out message))
                {
                    ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    DeployReportsWizard wizard =
                        new DeployReportsWizard(reportControl.ReportType, reportControl.GetReportParmeters(), selectedCustomReport, reports);
                    wizard.ShowDialog(_window);
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(_window, message);
                }
            }
        }

        public void DeployCustomReport()
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                DeployReportsWizard wizard =
                    new DeployReportsWizard(((ReportsView)
                ApplicationController.Default.ActiveView).ActiveReport.ReportType, null, selectedCustomReport, reports);

                wizard.ShowDialog(_window);
            }
        }

        private void ScheduleEmailCustomReport_Click(object sender, EventArgs e)
        {
            this.ScheduleEmailCustomReport();

            //e.Handled = true;
        }
    }
}
#endregion