using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Presenters
{
    internal class ServerTreeViewPresenter
    {
        private enum IconStates
        {
            Servers = 0,
            ServerInformation = 1,
            ServerOk = 2,
            ServerWarning = 3,
            ServerCritical = 4,
            ServerMaintenanceMode = 5,
            Tag = 6,
            ServersCritical = 7,
            ServersWarning = 8,
            ServersMaintenanceMode = 9,
            ServersOK = 10,
            Server = 11
        }

        Dictionary<int,MonitoredSqlServerWrapper> _servers;
        private ImageList _images;
        private const string AllServersUserViewName = "All Servers";
        private TreeNode _mainNode;
        private TreeView _treeView;

        /// <summary>
        /// Initializes the treeView with all servers
        /// </summary>
        /// <param name="treeView"></param>
        public ServerTreeViewPresenter(TreeView treeView)
        {
            _treeView = treeView;
            _servers = ApplicationModel.Default.ActiveInstances.ToDictionary(a=>a.Id);
            CreateMainNode(AllServersUserViewName, AllServersUserViewName, 0);
            InitializeObjects();
        }

        /// <summary>
        /// Initializes the treeView with thes server for the tag
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="tag"></param>
        public ServerTreeViewPresenter(TreeView treeView, Tag tag)
        {
            this._treeView = treeView;
            _servers = ApplicationModel.Default.ActiveInstances.Where(a => tag.Instances.Contains(a.Id)).ToDictionary(a=>a.Id);
            CreateMainNode(tag.Name, tag, (int)IconStates.Tag);
            InitializeObjects();
        }

        /// <summary>
        /// Initializes the treeView with thes server for the UserView
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="view"></param>
        public ServerTreeViewPresenter(TreeView treeView, UserView view)
        {
            this._treeView = treeView;
            _servers = ApplicationModel.Default.ActiveInstances.Where(a => view.Instances.Contains(a.Id)).ToDictionary(a => a.Id);

            IconStates icon = GetIcon(view);

            CreateMainNode(view.Name, view, (int)icon);
            InitializeObjects();
        }

        /// <summary>
        /// Return the loaded servers
        /// </summary>
        public Dictionary<int, MonitoredSqlServerWrapper> Servers
        {
            get { return _servers; }
        }

        /// <summary>
        /// Return the list of selected servers
        /// </summary>
        public List<MonitoredSqlServerWrapper> SelectedServers
        {
            get
            {
                List<MonitoredSqlServerWrapper> result=new List<MonitoredSqlServerWrapper>();

                foreach (TreeNode node in _mainNode.Nodes)
                {
                    if(node.Checked)
                    {                        
                        result.Add(_servers[(int) node.Tag]);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Return the corresponding icon for the userview
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private IconStates GetIcon(UserView view)
        {
            IconStates result = IconStates.Servers;
            if (view.Id.Equals(SearchUserView.CriticalUserViewID))
            {
                result = IconStates.ServersCritical;
            }
            else if (view.Id.Equals(SearchUserView.WarningUserViewID))
            {
                result = IconStates.ServersWarning;
            }
            else if (view.Id.Equals(SearchUserView.MaintenanceModeUserViewID))
            {
                result = IconStates.ServersMaintenanceMode;
            }
            else if (view.Id.Equals(SearchUserView.OKUserViewID))
            {
                result = IconStates.ServersOK;
            }

            return result;
        }

        /// <summary>
        /// Load images for the treeview
        /// </summary>
        private void LoadImages()
        {
            _images = new ImageList();
            _images.ColorDepth = ColorDepth.Depth32Bit;
            _images.ImageSize = new Size(16, 16);

            _images.Images.Add(IconStates.Servers.ToString(), Resources.Servers);
            _images.Images.Add(IconStates.ServerInformation.ToString(), Resources.ServerInformation);
            _images.Images.Add(IconStates.ServerOk.ToString(), Resources.ServerOK16x16);
            _images.Images.Add(IconStates.ServerWarning.ToString(), Resources.ServerWarning16x16);
            _images.Images.Add(IconStates.ServerCritical.ToString(), Resources.ServersMaintenanceMode);
            _images.Images.Add(IconStates.ServerMaintenanceMode.ToString(), Resources.ServerMaintenanceMode);
            _images.Images.Add(IconStates.Tag.ToString(), Resources.Tag16x16);
            _images.Images.Add(IconStates.ServersCritical.ToString(), Resources.ServerGroupCritical16x16);
            _images.Images.Add(IconStates.ServersWarning.ToString(), Resources.ServerGroupWarning16x16);            
            _images.Images.Add(IconStates.ServersMaintenanceMode.ToString(), Resources.ServersMaintenanceMode);
            _images.Images.Add(IconStates.ServersOK.ToString(), Resources.ServerGroupOK16x16);
            _images.Images.Add(IconStates.Server.ToString(), Resources.Server);
        }

        private void InitializeObjects()
        {
            LoadImages();

            _treeView.CheckBoxes = true;
            _treeView.ImageList = _images;

            foreach (var server in _servers.Values)
            {
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(server.Id);

                PermissionType permissionType = ApplicationModel.Default.UserToken.GetServerPermission(server.Id);

                if (permissionType < PermissionType.Modify)
                {
                    continue;
                }
                
                TreeNode node = new TreeNode();
                node.Text = server.InstanceName;
                node.Tag = server.Id;

                int imageIndex = (int) IconStates.Server;
                switch (status.Severity)
                {                    
                    case MonitoredState.OK:
                        imageIndex = (int) IconStates.ServerOk;
                        break;
                    case MonitoredState.Informational:
                        imageIndex = (int)IconStates.ServerInformation;
                        break;
                    case MonitoredState.Warning:
                        imageIndex = (int)IconStates.ServerWarning;
                        break;
                    case MonitoredState.Critical:
                        imageIndex = (int)IconStates.ServerCritical;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }                

                if (server.MaintenanceModeEnabled)
                {
                    imageIndex  = (int)IconStates.ServerMaintenanceMode;
                }

                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;

                node.ToolTipText = "";

                _mainNode.Nodes.Add(node);
            }

            _treeView.AfterCheck += new TreeViewEventHandler(_treeView_AfterCheck);            

            _mainNode.Checked = true;

            _mainNode.Expand();
        }

        void _treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var action = e.Action;
            bool status = e.Node.Checked;

            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = status;
            }
        }

        /// <summary>
        /// Create the main node with the specific text, object and the image index
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="imageIndex"></param>
        private void CreateMainNode(string text, object tag, int imageIndex)
        {
            _mainNode = new TreeNode();
            _mainNode.Text = text;
            _mainNode.Tag = tag;
            _mainNode.ImageIndex = imageIndex;
            _mainNode.SelectedImageIndex = imageIndex;

            _treeView.Nodes.Add(_mainNode);
        }

        void SetSubtreeChecked(TreeNode parent_node,bool is_checked)
        {
            parent_node.Checked = is_checked;

            foreach (TreeNode node in parent_node.Nodes)
            {
                SetSubtreeChecked(node, is_checked);
            }
        }
    }
}
