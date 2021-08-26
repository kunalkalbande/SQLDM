using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Infragistics.Win.UltraWinToolTip;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    [Serializable]
    internal partial class DraggablePanel : UserControl
    {
        public DraggablePanel(DashboardPanelSelector panel, Image image)
        {
            InitializeComponent();

            Name = string.Format("{0}Panel", Helpers.ApplicationHelper.GetEnumDescription(panel.DashboardPanel));
            DashboardPanel = panel.DashboardPanel;
            mainPictureBox.Image = image;
            UltraToolTipInfo toolTipInfo = new UltraToolTipInfo
                                               {
                                                   ToolTipText = @"Click the panel for more information or drag it to the desired position on the dashboard."
                                               };
            ultraToolTipManager.SetUltraToolTip(mainPictureBox, toolTipInfo);
        }

        public DashboardPanel DashboardPanel { get; private set; }

        private void DraggablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left))
            {
                DataObject data = new DataObject();
                data.SetData(this);
                data.SetImage(mainPictureBox.Image);
                data.SetText(Name);
                DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void mainPictureBox_MouseHover(object sender, EventArgs e)
        {
            OnMouseHover(e);
        }

        private void mainPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }
    }

    internal class DropPanelEventArgs : EventArgs
    {
        public PanelDragType PanelDragType { get; private set; }
        public DashboardPanel DashboardPanel { get; private set; }
        public DashboardPanelConfiguration DraggedPanelConfiguration { get; private set; }

        public DropPanelEventArgs(DashboardPanel dashboardPanel, PanelDragType panelDragType)
        {
            DashboardPanel = dashboardPanel;
            PanelDragType = panelDragType;
        }

        public DropPanelEventArgs(DashboardPanelConfiguration panelConfiguration, PanelDragType panelDragType)
        {
            DraggedPanelConfiguration = panelConfiguration;
            DashboardPanel = DraggedPanelConfiguration.Panel;
            PanelDragType = panelDragType;
        }
    }

    internal enum PanelDragType
    {
        Drop,
        Swap
    }
}
