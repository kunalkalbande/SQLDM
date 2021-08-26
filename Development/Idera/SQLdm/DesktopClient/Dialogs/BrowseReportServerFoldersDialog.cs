using System;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Helpers.WebServices;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class BrowseReportServerFoldersDialog : Form
    {
        private readonly ReportServerData serverData;

        public BrowseReportServerFoldersDialog(ReportServerData serverData)
        {
            this.serverData = serverData;
            InitializeComponent();
            reportServerFolderTreeView.ImageList.Images.Add("ServerRoot", Resources.Server);
            reportServerFolderTreeView.ImageList.Images.Add("Folder", Resources.FolderClosed16x16);
            RefreshReportFolders();
            AdaptFontSize();
        }

        private void RefreshReportFolders()
        {
            loadingPogressControl.Visible =
                loadingLabel.Visible = 
                loadingPogressControl.Active = serverData != null;

            if (serverData != null)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        public void UpdateReportServerFolders(CatalogItem[] items)
        {
            string[] folders;
            TreeNode parent;
            TreeNode node;

            reportServerFolderTreeView.Nodes.Clear();
            reportServerFolderTreeView.PathSeparator = "/";
            
            TreeNode root = new TreeNode();
            root.Text = "Report Server <Root Folder>";
            root.ImageKey = "ServerRoot";
            root.SelectedImageKey = "ServerRoot";
            reportServerFolderTreeView.Nodes.Add(root);
            reportServerFolderTreeView.SelectedNode = root;
            
            try
            {
                foreach (CatalogItem ci in items)
                {
                    if (ci.Type == ItemTypeEnum.Folder)
                    {
                        folders = ci.Path.Split('/');
                        parent = root;
                        
                        foreach (string folder in folders)
                        {
                            // skip empty strings and the root
                            if ((String.IsNullOrEmpty(folder)) || (folder == "/"))
                            {
                                continue;
                            }
                            node = new TreeNode();
                            node.Text = folder;
                            node.Name = folder;
                            node.ImageKey = "Folder";
                            node.SelectedImageKey = "Folder";
                            TreeNode[] nodes = parent.Nodes.Find(folder, false);
                            
                            // if we don't find the folder, add it
                            if ((nodes == null) || (nodes.GetLength(0) == 0))
                            {
                                parent.Nodes.Add(node);
                                parent = node;
                            }
                            else
                            {
                                parent = nodes[0];
                            }
                        }
                    }
                }
                reportServerFolderTreeView.ExpandAll();
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(Parent,
                                                "An error occurred attemping to display the report server folders.",
                                                exception);
            }
        }

        public string GetSelectedFolder()
        {
            string path = reportServerFolderTreeView.SelectedNode.FullPath;
            int index = path.IndexOf("/");
            return index != -1 ? path.Substring(index + 1) : String.Empty;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReportingService2005 webService = new ReportingService2005();
            webService.UseDefaultCredentials = true;
            webService.Url = serverData.reportServerURL;
            serverData.reportFolders = webService.ListChildren("/", true);
            e.Cancel = backgroundWorker.CancellationPending;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                loadingPogressControl.Visible =
                    loadingLabel.Visible =
                    loadingPogressControl.Active = false;

                if (e.Error == null)
                {
                    UpdateReportServerFolders(serverData.reportFolders);
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while loading the report server folders.", e.Error);
                    Close();
                }
            }
        }

        private void reportServerFolderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            okButton.Enabled = reportServerFolderTreeView.SelectedNode != null;
        }

        private void BrowseReportServerFoldersDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();
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
