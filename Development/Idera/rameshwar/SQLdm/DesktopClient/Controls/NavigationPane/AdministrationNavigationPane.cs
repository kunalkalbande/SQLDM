using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Views.Administration;


namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    using Helpers;
    using Idera.SQLdm.DesktopClient.Views.Administration;
    using Infragistics.Win.UltraWinListView;

    public partial class AdministrationNavigationPane : UserControl
    {
        private AdministrationView.AdministrationNode selectedNode;

        #region ctor
        public AdministrationNavigationPane()
        {
            InitializeComponent();

            selectedNode = AdministrationView.AdministrationNode.Administration;

            ApplicationController.Default.AdministrationViewChanged += new EventHandler<AdministrationViewChangedEventArgs>(administrationViewChanged);
        }
        #endregion

        #region methods

        private void administrationViewChanged(object sender, AdministrationViewChangedEventArgs e)
        {
            switch (e.Node)
            {
                case AdministrationView.AdministrationNode.ApplicationSecurity:
                    adminTree.SelectedNode = adminTree.Nodes[0].Nodes[0];
                    break;

                case AdministrationView.AdministrationNode.CustomCounters:
                    adminTree.SelectedNode = adminTree.Nodes[0].Nodes[1];
                    break;

                case AdministrationView.AdministrationNode.AuditedActions:
                    adminTree.SelectedNode = adminTree.Nodes[0].Nodes[2];
                    break;

                case AdministrationView.AdministrationNode.ImportExport:
                    adminTree.SelectedNode = adminTree.Nodes[0].Nodes[3];
                    break;

                default:
                    adminTree.SelectedNode = adminTree.Nodes[0];
                    break;
            }
        }

        #endregion

        #region events
        private void AdministrationNavigationPane_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            adminTree.ExpandAll();
            if (adminTree.SelectedNode == null)
            {
                adminTree.SelectedNode = adminTree.Nodes[0];
            }
        }

        private void adminTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Determine the node.
            switch (e.Node.Level)
            {
                case 0: // Administration
                    selectedNode = AdministrationView.AdministrationNode.Administration;
                    break;

                case 1:
                    switch (e.Node.Index)
                    {
                        case 0: // Application security
                            selectedNode = AdministrationView.AdministrationNode.ApplicationSecurity;
                            break;

                        case 1: // Custom counters
                            selectedNode = AdministrationView.AdministrationNode.CustomCounters;
                            break;

                        case 2: // Audited Actions
                            selectedNode = AdministrationView.AdministrationNode.AuditedActions;
                            break;
                        case 3:
                            selectedNode = AdministrationView.AdministrationNode.ImportExport;
                            break;

                    }
                    break;
            }

            // Show the node view.
            ApplicationController.Default.ShowAdministrationView(selectedNode);
        }
        #endregion
    }
}
