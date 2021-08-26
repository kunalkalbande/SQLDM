
namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.DesktopClient.Controls;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.DesktopClient.Objects;
    using Idera.SQLdm.DesktopClient.Properties;
    using System.Windows.Forms;
    using Idera.SQLdm.DesktopClient.Views.Reports.ReportControls;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Infragistics.Win.UltraWinToolbars;
    using Wintellect.PowerCollections;
    using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Windows.Themes;

    enum PeriodFilter
    {
        AnyDay,
        Today,
        Last7Days,
        Last30Days,
        CustomRange
    }

    internal partial class AuditedActionsView : Idera.SQLdm.DesktopClient.Views.View
    {
        private UltraGridColumn selectedColumn = null;
        private PeriodFilter period;
        private ComboBoxTool _comboActivityLogTime;
        protected List<DateRangeOffset> customDates; // Currently specified custom dates.
        private DateTime currentDate = DateTime.Today;

        public AuditedActionsView()
        {
            InitializeComponent();
            period = PeriodFilter.Last7Days;
            _comboActivityLogTime = (ComboBoxTool)this.contextMenuManager1.Tools["TimeButton"];
            ((ComboBoxTool) this.contextMenuManager1.Tools["TimeButton"]).SelectedIndex = (int)period;
            TimeButton_OptionChanged();
            this.SetDetailsVisibility(false);

            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.ultraGridAuditActions);
            //START:DarkTheme 4.12 QAIssue152-Period Dropdown Styling : Babita Manral
            if (Settings.Default.ColorScheme == "Dark")
            {                              
                this.valueListItem3.Appearance.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
                this.valueListItem3.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
                this.valueListItem4.Appearance.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
                this.valueListItem4.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
                this.valueListItem5.Appearance.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
                this.valueListItem5.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
                this.valueListItem6.Appearance.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
                this.valueListItem6.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
                this.valueListItem7.Appearance.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
                this.valueListItem7.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            }
            else
            {
                this.valueListItem3.ResetAppearance();
                this.valueListItem4.ResetAppearance();
                this.valueListItem5.ResetAppearance();
                this.valueListItem6.ResetAppearance();
                this.valueListItem7.ResetAppearance();
            }
            //END:DarkTheme 4.12 QAIssue152-Period Dropdown Styling : Babita Manral            
        }

        private void SetDetailsVisibility(bool showDetails)
        {
            if(showDetails)
            {
                this.tableLayoutMain.BringToFront();
            }
            else
            {
                this.labelNoRowSelected.BringToFront();
            }
        }

        public override void UpdateData(object data)
        {
            if(currentDate != DateTime.Today)
            {
                currentDate = DateTime.Today;
                TimeButton_OptionChanged();
            }

            UltraGridHelper.GridState state = UltraGridHelper.GetGridState(ultraGridAuditActions, "AuditableEventID");

            ultraGridAuditActions.SuspendLayout();
            ultraGridAuditActions.SuspendRowSynchronization();
            // resume binding - will cause group by rows to be recreated
            auditedActionsViewDataSource.ResumeBindingNotifications();

            RepositoryHelper
                        .LoadDataSource(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        auditedActionsViewDataSource,
                        "p_GetAllAuditableEvents", customDates[0].UtcStart, customDates[customDates.Count - 1].UtcEnd, Settings.Default.AuditEventsTopCount);
            
            UltraGridHelper.RestoreGridState(state);
            ultraGridAuditActions.ResumeRowSynchronization();
            ultraGridAuditActions.ResumeLayout();

            ApplicationController.Default
                    .OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        private void ultraGridAuditActions_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    var columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)this.contextMenuManager1.Tools["GroupByThisColumnMenuOption"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    this.contextMenuManager1.SetContextMenuUltra((UltraGrid)sender, "ColumnContext");
                }
                else
                {
                    UltraGridRow row = ((UltraGridRow) selectedElement.SelectableItem);
                    if (row != null && row.IsDataRow)
                    {
                        this.contextMenuManager1.SetContextMenuUltra((UltraGrid)sender, "GridRowMenu");
                        this.ultraGridAuditActions.Selected.Rows.Clear();
                        row.Activate();
                        row.Selected = true;
                    }
                    else
                    {
                        this.contextMenuManager1.SetContextMenuUltra((UltraGrid)sender, null);
                    }
                }
            } 
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ultraGridAuditActions_Initializelayout(object sender, InitializeLayoutEventArgs e)
        {
            if (AutoScaleSizeHelper.isScalingRequired)
                AutoScaleSizeHelper.Default.AutoScaleControl(this.ultraGridAuditActions, AutoScaleSizeHelper.ControlType.UltraGridCheckbox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "PrintMenuOption": // ButtonTool
                    Print();
                    break;
                case "ExcelMenuOption": // ButtonTool
                    ExportToExcel();
                    break;
                case "CopyToClipboardMenuOption":
                    UltraGridHelper.CopyToClipboard(this.ultraGridAuditActions, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "CollapseAllGroupsMenuOption":
                    CollapseAllGroups();
                    break;
                case "ExpandAllGroupsMenuOption":
                    ExpandAllGroups();
                    break;
                // Column Context
                case "SortAscendingMenuOption":
                    SortSelectedColumnAscending();
                    break;
                case "SortDescendingMenuOption":
                    SortSelectedColumnDescending();
                    break;
                case "GroupByBoxMenuOption":
                    ToggleGroupByBox();
                    break;
                case "GroupByThisColumnMenuOption":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "RemoveThisColumnMenuOption":
                    RemoveSelectedColumn();
                    break;
                case "ColumnChooserMenuOption":
                    ShowColumnChooser();
                    break;
            }
        }

        private void TimeButton_OptionChanged()
        {
            period = (PeriodFilter)_comboActivityLogTime.SelectedIndex;

            if(period == PeriodFilter.CustomRange)
            {
                DateTime minimumDate = this.GetMinAuditedLocalDate();

                var dialog = new PeriodSelectionDialog(minimumDate.ToLocalTime().Date, DateTime.Now, DateTime.Now);
                dialog.Location = this.GetToolLocation("TimeButton");

                if (dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    customDates = new List<DateRangeOffset>();
                    DateRangeOffset.AddDateRange(customDates, dialog.SelectedStartDate, dialog.SelectedEndDate);
                }
            }
            
            this.SetCustomDates();
            this.UpdateDateRangeControl();
            this.UpdateData(null);
        }

        private Point GetToolLocation(string tool)
        {
            ToolBase rootTool = this.contextMenuManager1.Tools[tool];
            ToolBase instanceTool = rootTool.SharedProps.ToolInstances[0];
            UIElement toolElement = instanceTool.UIElement;

            var periodComboLocation = new Point();

            if(toolElement != null)
            {
                Rectangle screenRect = toolElement.Control.RectangleToScreen(toolElement.Rect);

                periodComboLocation = new Point
                {
                    X = screenRect.Location.X,
                    Y = screenRect.Location.Y + screenRect.Height
                };
            }

            return periodComboLocation;
        }

        private void UpdateDateRangeControl()
        {
            ((LabelTool)this.contextMenuManager1.Tools["DateRangeMenuLabel"]).SharedPropsInternal.Caption = string.Format("{0} - {1}", customDates[0].UtcStart.ToLocalTime().ToString("d"), customDates[customDates.Count - 1].UtcEnd.ToLocalTime().ToString("d"));
        }

        private void SetCustomDates()
        {
            Pair<DateTime, DateTime> range = GetSelectedRange();
            customDates = new List<DateRangeOffset>();
            DateRangeOffset.AddDateRange(customDates, range.First, range.Second);
        }

        protected virtual Pair<DateTime, DateTime> GetSelectedRange()
        {
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = startDate;

            if (period == PeriodFilter.Today)
            {
                // Nothing to do
            }
            else if (period == PeriodFilter.Last7Days)
            {
                startDate = endDate.AddDays(-7);
            }
            else if (period == PeriodFilter.Last30Days)
            {
                startDate = endDate.AddDays(-30);
            }
            else if (period == PeriodFilter.CustomRange)
            {
                startDate = customDates[0].UtcStart.ToLocalTime();
                endDate = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            }
            else if (period == PeriodFilter.AnyDay)
            {
                startDate = GetMinAuditedLocalDate().ToLocalTime();
            }

            return new Pair<DateTime, DateTime>(startDate, endDate.Date.Add(new TimeSpan(23, 59, 59)));
        }

        /// <summary>
        /// Exports grids data to Excel
        /// </summary>
        private void ExportToExcel()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "Audited Events";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(ultraGridAuditActions, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        /// <summary>
        /// Prints grid data
        /// </summary>
        private void Print()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft = Text;
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";
            ultraPrintPreviewDialog.ShowDialog();
        }

        private void ultraGridAuditActions_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            SetDetailsVisibility(true);
        }

        private void CollapseAllGroups()
        {
            this.ultraGridAuditActions.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            this.ultraGridAuditActions.Rows.ExpandAll(true);
        }

        private void contextMenuManager1_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "ColumnContext")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(this.ultraGridAuditActions);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["RemoveThisColumnMenuOption"].SharedProps.Enabled = enableTool;
            }
        }

        private void ultraGridAuditActions_AfterSortChange(object sender, BandEventArgs e)
        {
            if (this.ultraGridAuditActions.Rows.Count > 0)
            {
                SetCollExpandAvailability(this.ultraGridAuditActions.Rows[0].IsGroupByRow);   
            }
            else
            {
                SetCollExpandAvailability(false);
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    this.ultraGridAuditActions.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void ShowColumnChooser()
        {
            var dialog = new SimpleUltraGridColumnChooserDialog(this.ultraGridAuditActions);
            dialog.Show(this);
        }

        private void ToggleGroupByBox()
        {
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.Hidden = !this.ultraGridAuditActions.DisplayLayout.GroupByBox.Hidden;
            UpdateToggleGroupByBoxButton();
        }

        private void UpdateToggleGroupByBoxButton()
        {
            if (this.ultraGridAuditActions.DisplayLayout.GroupByBox.Hidden)
            {
                this.contextMenuManager1.Tools["GroupByBoxMenuOption"].SharedProps.AppearancesSmall.
                    Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
            }
            else
            {
                this.contextMenuManager1.Tools["GroupByBoxMenuOption"].SharedProps.AppearancesSmall.
                    Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
            }
        }

        /// <summary>
        /// Sets the Collapse and Expand availability
        /// </summary>
        /// <param name="enabled"></param>
        private void SetCollExpandAvailability(bool enabled)
        {
            contextMenuManager1.Tools["CollapseAllGroupsMenuOption"].SharedProps.Enabled = enabled;
            contextMenuManager1.Tools["ExpandAllGroupsMenuOption"].SharedProps.Enabled = enabled;
        }

        void contextMenuManager1_AfterToolCloseup(object sender, Infragistics.Win.UltraWinToolbars.ToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                // Filter
                case "TimeButton":
                    TimeButton_OptionChanged();
                    break;
            }
        }

        private DateTime GetMinAuditedLocalDate()
        {
            string query = "select TOP 1 [DateTime] from AuditableEvents order by [DateTime] asc";
            return (RepositoryHelper.ExecuteQuery(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, query)).ToLocalTime();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// Sets the name of the Help file related to Change Log View
        /// </summary>
        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(Common.HelpTopics.ChangeLogHelp);
        }
    }
}
