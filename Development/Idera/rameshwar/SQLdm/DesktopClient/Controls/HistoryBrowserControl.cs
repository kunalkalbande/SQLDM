using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Resources=Idera.SQLdm.DesktopClient.Properties.Resources;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class HistoryBrowserControl : UserControl
    {
        private bool minimized = false;
        private int expandedWidth = 150;
        private bool ignoreToolClick = false;
        private readonly ControlContainerDialog controlContainerDialog = new ControlContainerDialog();

        public event EventHandler CloseButtonClicked;
        public event EventHandler MinimizedChanged;
        public event EventHandler<HistoricalSnapshotSelectedEventArgs> HistoricalSnapshotSelected;
        public event EventHandler<EventArgs> HistoricalCustomRangeSelected; //SqlDM 10.2 (Anshul Aggarwal) : New History Browser
        //Sqldm - 28694 start
        public HistoryBrowserPane GetHistoryBrowserPane()
        {
            return historyBrowserPane;
        }
        //sqldm -28694 end
        public HistoryBrowserControl()
        {
            InitializeComponent();
            controlContainerDialog.VisibleChanged += controlContainerDialog_VisibleChanged;

            historyTreeImages.Images.Add("HistoryGroup", Resources.Calendar16x16);
            historyTreeImages.Images.Add("StatusOK", Resources.StatusOKSmall);
            historyTreeImages.Images.Add("StatusWarning", Resources.StatusWarningSmall);
            historyTreeImages.Images.Add("StatusCritical", Resources.StatusCriticalSmall);

            // Auto scale font size.
            AdaptFontSize();
        }

        [Browsable(false)]
        public bool Minimized
        {
            get { return minimized; }
            set
            {
                if (value)
                {
                    ShowMinimized();
                }
                else
                {
                    ShowExpanded();
                }

                if (MinimizedChanged != null)
                {
                    MinimizedChanged(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        public int ExpandedWidth
        {
            get { return expandedWidth; }
            set { expandedWidth = value; }
        }

        public void Initialize(int instanceId)
        {
            historyBrowserPane.Initialize(instanceId);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Handler for custom scale selection from history browser pane.
        /// </summary>
        private void historyBrowserPane_HistoricalCustomRangeSelected(object sender, EventArgs e)
        {
            var handler = HistoricalCustomRangeSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Method for drilldown functionality to notify about start-end dates
        /// </summary>
        public void SetHistoricalCustomRange(ChartDrilldownEventArgs e)
        {
            var changed = ApplicationModel.Default.HistoryTimeValue.SetCustomRange(e.HistoricalSnapshotDateTime.Subtract(TimeSpan.FromMinutes(e.Minutes)),
                e.HistoricalSnapshotDateTime);
            if (changed)
            {
                ApplicationController.Default.PersistUserSettings(); // Persist user settings on background thread.
            }
            historyBrowserPane_HistoricalCustomRangeSelected(this, e);
        }

        private void ShowMinimized()
        {
            if (!minimized)
            {
                historyBrowserPane.Visible = false;
                titleLabel.Visible = false;
                closeButton.Visible = false;
                toggleMinimizedButton.Image = Resources.LeftArrows;
                toggleMinimizedButton.ToolTipText = "Expand History Browser";
                minimized = true;
            }
        }

        public void ShowExpanded()
        {
            if (minimized)
            {
                if (!borderPanel.Controls.Contains(historyBrowserPane))
                {
                    controlContainerDialog.FreeControl();
                    borderPanel.Controls.Add(historyBrowserPane);
                    historyBrowserPane.BringToFront();
                }

                historyBrowserPane.Visible = true;
                titleLabel.Visible = true;
                closeButton.Visible = true;
                toggleMinimizedButton.Image = Resources.RightArrows;
                toggleMinimizedButton.ToolTipText = "Minimize History Browser";
                minimized = false;
            }
        }

        private void controlContainerDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (!controlContainerDialog.Visible)
            {
                historyBrowserHotTrackLabel.ClearState();
            }
        }

        private void toggleMinimizedButton_Click(object sender, EventArgs e)
        {
            Minimized = !Minimized;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            OnCloseButtonClicked();
        }

        private void OnCloseButtonClicked()
        {
            if (CloseButtonClicked != null)
            {
                CloseButtonClicked(this, EventArgs.Empty);
            }
        }

        private void historyBrowserHotTrackLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!controlContainerDialog.Visible)
                {
                    historyBrowserHotTrackLabel.ShowSelected();

                    if (controlContainerDialog.Control != historyBrowserPane)
                    {
                        borderPanel.Controls.Remove(historyBrowserPane);
                        controlContainerDialog.SetControl(historyBrowserPane);
                        historyBrowserPane.Visible = true;
                    }

                    Point location = PointToScreen(headerStrip.Location);

                    controlContainerDialog.Location =
                        new Point(location.X - expandedWidth,
                                  location.Y + headerStrip.Height - 1);

                    controlContainerDialog.Width = expandedWidth;
                    controlContainerDialog.Height = Height - headerStrip.Height;

                    controlContainerDialog.Show();
                }
                else
                {
                    controlContainerDialog.Hide();
                }
            }
        }

        private void historyBrowserPane_HistoricalSnapshotSelected(object sender, HistoricalSnapshotSelectedEventArgs e)
        {
            if (controlContainerDialog.Visible)
            {
                controlContainerDialog.Hide();
            }

            if (HistoricalSnapshotSelected != null)
            {
                HistoricalSnapshotSelected(this, e);
            }
        }

        public void AddRecentlyViewedSnapshot(DateTime snapshotDateTime)
        {
            historyBrowserPane.AddRecentlyViewedSnapshot(snapshotDateTime);
        }
        public void RefreshHistoryPaneTimer()
        {
            historyBrowserPane.RefreshTimer();
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (ignoreToolClick)
            {
                return;
            }

            switch (e.Tool.Key)
            {
                case "showMaximizedButton":
                    Minimized = !((StateButtonTool) toolbarsManager.Tools["showMaximizedButton"]).Checked;
                    break;
                case "showMinimizedButton":
                    Minimized = ((StateButtonTool)toolbarsManager.Tools["showMinimizedButton"]).Checked;
                    break;
                case "closeButton":
                    OnCloseButtonClicked();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "optionsContextMenu")
            {
                ignoreToolClick = true;
                ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["showMaximizedButton"]).Checked = !Minimized;
                ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["showMinimizedButton"]).Checked = Minimized;
                ignoreToolClick = false;
            }
        }

        public void ClearSnapshotSelection()
        {
            historyBrowserPane.ClearSnapshotSelection();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
