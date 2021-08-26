using System.Linq;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;

using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.DesktopClient.Presenters;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.DesktopClient.Dialogs;
    using Idera.SQLdm.DesktopClient.Objects;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Views;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server.Logs;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries;
    using Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinToolbars;
    using Infragistics.Win.UltraWinToolTip;
    using Microsoft.SqlServer.MessageBox;
    using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.Common;
    using Svg;
    using System.IO;
    using System.Windows.Controls.Primitives;

    public partial class ServersNavigationPane : UserControl, IThemeControl
    {
        #region Fields

        private const string DummyTag = "#dummy#";
        private const string AllServersUserViewName = "All Servers";
        private const string Refreshing_Tag = " (Refreshing...)";
        private const string Initializing_Tag = " (Initializing...)";
        private const int GroupPanelsMaxCount = 10;
        private const string SnoozeAllalertsServerButtonKey = "snoozeAllAlertsServerButton";
        private const string ResumeAllAlertsServerButtonKey = "resumeAllAlertsServerButton";

        private static readonly Logger Log = Logger.GetLogger(typeof(ServersNavigationPane));

        private readonly Dictionary<Guid, TreeNode> userViewTreeNodeLookupTable =
            new Dictionary<Guid, TreeNode>();

        private readonly Dictionary<int, TreeNode> tagTreeNodeLookupTable = new Dictionary<int, TreeNode>();

        private readonly Dictionary<int, TreeNode> instanceTreeNodeLookupTable =
            new Dictionary<int, TreeNode>();

        private delegate void PostMethodObject(object arg1);
        private static object userViewTreeViewLock = new object();
        private TreeNode userViewsSelectedNode;
        private TreeNode tagsSelectedNode;

        private UltraToolTipInfo userViewTreeViewToolTip;
        private TreeNode userViewTreeViewToolTipNode;
        private TreeNode treeViewReselectNode;

        private bool inActiveViewChanged;
        private bool inShowView;
        private bool maintainCurrentView;
        private List<int> serverListToResume = null;
        private List<int> serverListToSnooze = null;
        private List<int> serverListToApplyAlertTemplate = null;
        private string tagNameSelected = "";
        public TreeView UserViewsTreeView
        {
            get { return this.userViewsTreeView; }
        }
        public ToolStrip UserViewsHeader
        {
            get { return this.userViewsHeaderStrip; }
        }

        public TreeView UserViewTreeView
        {
            get { return this.userViewTreeView; }
        }

        public ToolStrip UserViewHeader
        {
            get { return this.userViewHeaderStrip; }
        }
        public TreeView TagsTreeView
        {
            get { return this.tagsTreeView; }
        }

        public ToolStrip TagsHeader
        {
            get { return this.tagsHeaderStrip; }
        }

        public Label ManageTagsLabel
        {
            get { return this.manageTagsLabel; }
        }

        #endregion

        public ServersNavigationPane()
        {
            InitializeComponent();
            userViewTreeViewToolTip = new UltraToolTipInfo();
            toolTipManager.SetUltraToolTip(userViewTreeView, userViewTreeViewToolTip);
        }

        private Color leftNavTextColor = ColorTranslator.FromHtml("#006089");
        private byte[] downArrowSvg = Resources.DownArrow_Light;
        private byte[] leftArrowSvg = Resources.LeftArrow_Light;
        private ServerViews currentServerSubTab = ServerViews.Overview;

        public void UpdateTheme(ThemeName themeName)
        {
            if (themeName == ThemeName.Light)
            {
                leftNavTextColor = ColorTranslator.FromHtml("#006089");
                var greyishBrown = System.Drawing.ColorTranslator.FromHtml("#483e2f");

                this.userViewTreeView.BackColor = Color.White;
                this.tagsTreeView.BackColor = Color.White;
                this.userViewsTreeView.BackColor = Color.White;

                var lightPeriwinkle = Color.FromArgb(127, 214, 215, 220);

                this.userViewTreeView.ImageList = images;
                this.userViewsTreeView.ImageList = images;
                this.tagsTreeView.ImageList = images;

                this.userViewsHeaderStripLabel.ForeColor = leftNavTextColor;
                this.toolStripLabel1.ForeColor = leftNavTextColor;
                this.serverGroupHeaderStripLabel.ForeColor = leftNavTextColor;
                this.userViewsHeaderStrip.BackColor = lightPeriwinkle;
                this.tagsHeaderStrip.BackColor = lightPeriwinkle;
                this.userViewHeaderStrip.BackColor = lightPeriwinkle;
                this.userViewsPanel.BackColor = Color.White;
                this.tagsPanel.BackColor = Color.White;
                this.serverGroupPanel.BackColor = Color.White;
                foreach (TreeNode node in userViewsTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                }
                foreach (TreeNode node in tagsTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                }
                foreach (TreeNode node in userViewTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                    foreach (TreeNode childNode in node.Nodes)
                    {
                        childNode.ForeColor = leftNavTextColor;
                    }
                }
                downArrowSvg = Resources.DownArrow_Light;
                leftArrowSvg = Resources.LeftArrow_Light;
                serverGroupHeaderStripLabel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Server;
                toolStripLabel1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Tag16x16;
                userViewsHeaderStripLabel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Views16x16;
            }
            else
            {
                var darkBackColor = System.Drawing.ColorTranslator.FromHtml("#021017");
                var midNightColor = System.Drawing.ColorTranslator.FromHtml("#006089");
                var halfMidNightColor = Color.FromArgb(127, 0, 96, 137);
                leftNavTextColor = ColorTranslator.FromHtml("#afb0b6");
                this.userViewTreeView.BackColor = darkBackColor;
                this.tagsTreeView.BackColor = darkBackColor;
                this.userViewsTreeView.BackColor = darkBackColor;

                this.userViewTreeView.ImageList = imagesDarkTheme;
                this.userViewsTreeView.ImageList = imagesDarkTheme;
                this.tagsTreeView.ImageList = imagesDarkTheme;

                this.userViewsHeaderStripLabel.ForeColor = leftNavTextColor;
                this.toolStripLabel1.ForeColor = leftNavTextColor;
                this.serverGroupHeaderStripLabel.ForeColor = leftNavTextColor;
                this.userViewsHeaderStrip.BackColor = halfMidNightColor;
                this.tagsHeaderStrip.BackColor = halfMidNightColor;
                this.userViewHeaderStrip.BackColor = halfMidNightColor;
                this.userViewsPanel.BackColor = darkBackColor;
                this.tagsPanel.BackColor = darkBackColor;
                this.serverGroupPanel.BackColor = darkBackColor;
                foreach (TreeNode node in userViewsTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                }
                foreach (TreeNode node in tagsTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                }
                foreach (TreeNode node in userViewTreeView.Nodes)
                {
                    node.ForeColor = leftNavTextColor;
                    foreach (TreeNode childNode in node.Nodes)
                    {
                        childNode.ForeColor = leftNavTextColor;
                    }
                }
                downArrowSvg = Resources.DownArrow_Dark;
                leftArrowSvg = Resources.LeftArrow_Dark;
                serverGroupHeaderStripLabel.Image = ImageHelper.GetBitmapFromSvgByteArray(Resources.Database_Dark);
                toolStripLabel1.Image = ImageHelper.GetBitmapFromSvgByteArray(Resources.Tags_Dark);
                userViewsHeaderStripLabel.Image = ImageHelper.GetBitmapFromSvgByteArray(Resources.MyViews_Dark);
            }
            UpdateUserViewsPanel();
            UpdateTagsPanel();
        }
        private void ServersNavigationPane_Load(object sender, EventArgs e)
        {
            if (images.Images.Count > 0) return;

            // configure the image list
            initializeTreeViewLightImageList();
            initializeTreeViewDarkImageList();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            Initialize();
        }

        private void initializeTreeViewLightImageList()
        {
            images.Images.Add("Servers", Resources.Servers);
            images.Images.Add("SearchServers", Resources.Servers);
            images.Images.Add("ServerInformation", Resources.ServerInformation);
            images.Images.Add("Sessions", Resources.Sessions);
            images.Images.Add("Queries", Resources.Queries);
            images.Images.Add("Resources", Resources.ResourcesSmall);
            images.Images.Add("Databases", Resources.Databases);
            images.Images.Add("Services", Resources.Services);
            images.Images.Add("Logs", Resources.Logs);
            images.Images.Add("ServerOK", Resources.StatusOKSmall);
            images.Images.Add("ServerWarning", Resources.StatusWarningSmall);
            images.Images.Add("ServerCritical", Resources.StatusCriticalSmall);
            images.Images.Add("ServerMaintenanceMode", Resources.ServerMaintenanceMode);
            images.Images.Add("SubView", Resources.GenericView16x16);
            images.Images.Add("Database", Resources.Database);
            images.Images.Add("Table", Resources.Table);
            images.Images.Add("Folder", Resources.FolderClosed16x16);
            images.Images.Add("FolderWarning", Resources.FolderClosedWarning16x16);
            images.Images.Add("FolderCritical", Resources.FolderClosedCritical16x16);
            images.Images.Add("SessionsWarning", Resources.SessionsWarning16x16);
            images.Images.Add("SessionsCritical", Resources.SessionsCritical16x16);
            images.Images.Add("QueriesWarning", Resources.QueriesWarning16x16);
            images.Images.Add("QueriesCritical", Resources.QueriesCritical16x16);
            images.Images.Add("DatabasesWarning", Resources.DatabasesWarning16x16);
            images.Images.Add("DatabasesCritical", Resources.DatabasesCritical16x16);
            images.Images.Add("ResourcesWarning", Resources.ResourcesWarning16x16);
            images.Images.Add("ResourcesCritical", Resources.ResourcesCritical16x16);
            images.Images.Add("ServicesWarning", Resources.ServicesWarning16x16);
            images.Images.Add("ServicesCritical", Resources.ServicesCritical16x16);
            images.Images.Add("TableWarning", Resources.TableWarning16x16);
            images.Images.Add("TableCritical", Resources.TableCritical16x16);
            images.Images.Add("DatabaseWarning", Resources.ServerWarning);
            images.Images.Add("DatabaseCritical", Resources.ServerCritical);
            images.Images.Add("SessionsDetails", Resources.SessionsDetailsViewSmall);
            images.Images.Add("SessionsLocks", Resources.SessionsLocksView16x16);
            images.Images.Add("SessionsBlocks", Resources.SessionsBlockingView16x16);
            images.Images.Add("ResourcesCpu", Resources.ResourcesCpuView16x16);
            images.Images.Add("ResourcesMemory", Resources.ResourcesMemoryView16x16);
            images.Images.Add("ResourcesDisk", Resources.ResourcesDiskView16x16);
            images.Images.Add("ResourcesProcedureCache", Resources.ResourcesProcedureCacheView16x16);
            images.Images.Add("DatabasesConfiguration", Resources.ConfigurationView16x16);
            images.Images.Add("DatabasesBackupsRestores", Resources.DatabasesBackupRestoreView16x16);
            images.Images.Add("DatabasesFiles", Resources.DatabasesFilesView16x16);
            images.Images.Add("ServicesSqlAgentJobs", Resources.ServicesSqlAgentJobsView16x16);
            images.Images.Add("ServicesFullTextSearch", Resources.ServicesFullTextSearchView16x16);
            images.Images.Add("ServicesReplication", Resources.ServicesReplicationView16x16);
            images.Images.Add("LogsSqlServer", Resources.LogsSqlServerFilter16x16);
            images.Images.Add("LogsSqlAgent", Resources.LogsSqlAgentFilter16x16);
            images.Images.Add("CustomCounter", Resources.CustomCounter16x16);

            images.Images.Add("ServersCritical", Resources.StatusCriticalSmall);
            images.Images.Add("ServersWarning", Resources.StatusWarningSmall);
            images.Images.Add("ServersOK", Resources.StatusOKSmall);
            images.Images.Add("ServersMaintenanceMode", Resources.StatusMainentanceModeSmall);
            images.Images.Add("SearchServersCritical", Resources.StatusCriticalSmall);
            images.Images.Add("SearchServersWarning", Resources.ServerGroupWarning16x16);
            images.Images.Add("SearchServersOK", Resources.ServerGroupOK16x16);
            images.Images.Add("SearchServersMaintenanceMode", Resources.ServersMaintenanceMode);
            images.Images.Add("TablesCritical", Resources.TablesViewCritical16x16);
            images.Images.Add("TablesWarning", Resources.TablesViewWarning16x16);
            images.Images.Add("CustomCounterWarning", Resources.CustomCounterWarning16x16);
            images.Images.Add("CustomCounterCritical", Resources.CustomCounterCritical16x16);
            images.Images.Add("Tag", Resources.Tag16x16);
            images.Images.Add("TagCritical", Resources.StatusCriticalSmall);
            images.Images.Add("TagWarning", Resources.StatusWarningSmall);
            images.Images.Add("Mirrored", Resources.DBmirroring_16);
            images.Images.Add("LogsWarning", Resources.LogsWarning16x16);
            images.Images.Add("LogsCritical", Resources.LogsCritical16x16);

            images.Images.Add("ResourcesWaitStats", Resources.Resources_server_waits_32x32);
            images.Images.Add("ResourcesActiveWaitStats", Resources.Resources_query_waits_32x32);

            images.Images.Add("QueriesSignatureMode", Resources.QueriesSignature);
            images.Images.Add("QueriesStatementMode", Resources.Queries);
            images.Images.Add("QueriesHistory", Resources.QueryHistory16);

            images.Images.Add("ResourcesFileActivity", Resources.ResourcesFileActivity32x32);

            // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding the disk size view image as a tree node 
            images.Images.Add("ResourcesDiskSize", Resources.ResourcesDiskSizeView16x16);
            images.Images.Add("Analysis", Resources.Analyze16);
            images.Images.Add("CloudCriticalSmall", Resources.cloud_database_icon_critical_16x16);
            images.Images.Add("CloudCritical", Resources.cloud_database_icon_critical_32x32);
            images.Images.Add("CloudOkSmall", Resources.cloud_database_icon_normal_16x16);
            images.Images.Add("CloudOk", Resources.cloud_database_icon_normal_32x32);
            images.Images.Add("CloudWarningSmall", Resources.cloud_database_icon_warning_16x16);
            images.Images.Add("CloudWarning", Resources.cloud_database_icon_warning_32x32);
            images.Images.Add("CloudDisabledSmall", Resources.cloud_database_icon_disabled_16x16);
            images.Images.Add("CloudDisabled", Resources.cloud_database_icon_disabled_32x32);

            images.Images.Add("CloudServerNormal", Resources.CloudServerNormal);
            images.Images.Add("CloudServerOk", Resources.CloudServerOk);
            images.Images.Add("CloudServerCritical", Resources.CloudServerCritical);
            images.Images.Add("CloudServerDisabled", Resources.CloudServerDisabled);
            images.Images.Add("CloudServerHover", Resources.CloudServerHover);
            images.Images.Add("CloudServerInformation", Resources.CloudServerInformation);
            images.Images.Add("CloudServerMaintenance", Resources.CloudServerMaintenance);
            images.Images.Add("CloudServerWarning", Resources.CloudServerWarning);
        }

        private void initializeTreeViewDarkImageList()
        {
            imagesDarkTheme.Images.Add("Servers", Resources.Servers);
            imagesDarkTheme.Images.Add("SearchServers", Resources.Servers);
            imagesDarkTheme.Images.Add("ServerInformation", Resources.ServerInformation);
            imagesDarkTheme.Images.Add("Sessions", Resources.Sessions);
            imagesDarkTheme.Images.Add("Queries", Resources.Queries);
            imagesDarkTheme.Images.Add("Resources", Resources.ResourcesSmall);
            imagesDarkTheme.Images.Add("Databases", Resources.Databases);
            imagesDarkTheme.Images.Add("Services", Resources.Services);
            imagesDarkTheme.Images.Add("Logs", Resources.Logs);
            imagesDarkTheme.Images.Add("ServerOK", ImageHelper.GetBitmapFromSvgByteArray(Resources.Ok_Dark));
            imagesDarkTheme.Images.Add("ServerWarning", ImageHelper.GetBitmapFromSvgByteArray(Resources.Warning_Dark));
            imagesDarkTheme.Images.Add("ServerCritical", ImageHelper.GetBitmapFromSvgByteArray(Resources.Critical_Dark));
            imagesDarkTheme.Images.Add("ServerMaintenanceMode", Resources.ServerMaintenanceMode);
            imagesDarkTheme.Images.Add("SubView", Resources.GenericView16x16);
            imagesDarkTheme.Images.Add("Database", Resources.Database);
            imagesDarkTheme.Images.Add("Table", Resources.Table);
            imagesDarkTheme.Images.Add("Folder", Resources.FolderClosed16x16);
            imagesDarkTheme.Images.Add("FolderWarning", Resources.FolderClosedWarning16x16);
            imagesDarkTheme.Images.Add("FolderCritical", Resources.FolderClosedCritical16x16);
            imagesDarkTheme.Images.Add("SessionsWarning", Resources.SessionsWarning16x16);
            imagesDarkTheme.Images.Add("SessionsCritical", Resources.SessionsCritical16x16);
            imagesDarkTheme.Images.Add("QueriesWarning", Resources.QueriesWarning16x16);
            imagesDarkTheme.Images.Add("QueriesCritical", Resources.QueriesCritical16x16);
            imagesDarkTheme.Images.Add("DatabasesWarning", Resources.DatabasesWarning16x16);
            imagesDarkTheme.Images.Add("DatabasesCritical", Resources.DatabasesCritical16x16);
            imagesDarkTheme.Images.Add("ResourcesWarning", Resources.ResourcesWarning16x16);
            imagesDarkTheme.Images.Add("ResourcesCritical", Resources.ResourcesCritical16x16);
            imagesDarkTheme.Images.Add("ServicesWarning", Resources.ServicesWarning16x16);
            imagesDarkTheme.Images.Add("ServicesCritical", Resources.ServicesCritical16x16);
            imagesDarkTheme.Images.Add("TableWarning", Resources.TableWarning16x16);
            imagesDarkTheme.Images.Add("TableCritical", Resources.TableCritical16x16);
            imagesDarkTheme.Images.Add("DatabaseWarning", Resources.ServerWarning);
            imagesDarkTheme.Images.Add("DatabaseCritical", Resources.ServerCritical);
            imagesDarkTheme.Images.Add("SessionsDetails", Resources.SessionsDetailsViewSmall);
            imagesDarkTheme.Images.Add("SessionsLocks", Resources.SessionsLocksView16x16);
            imagesDarkTheme.Images.Add("SessionsBlocks", Resources.SessionsBlockingView16x16);
            imagesDarkTheme.Images.Add("ResourcesCpu", Resources.ResourcesCpuView16x16);
            imagesDarkTheme.Images.Add("ResourcesMemory", Resources.ResourcesMemoryView16x16);
            imagesDarkTheme.Images.Add("ResourcesDisk", Resources.ResourcesDiskView16x16);
            imagesDarkTheme.Images.Add("ResourcesProcedureCache", Resources.ResourcesProcedureCacheView16x16);
            imagesDarkTheme.Images.Add("DatabasesConfiguration", Resources.ConfigurationView16x16);
            imagesDarkTheme.Images.Add("DatabasesBackupsRestores", Resources.DatabasesBackupRestoreView16x16);
            imagesDarkTheme.Images.Add("DatabasesFiles", Resources.DatabasesFilesView16x16);
            imagesDarkTheme.Images.Add("ServicesSqlAgentJobs", Resources.ServicesSqlAgentJobsView16x16);
            imagesDarkTheme.Images.Add("ServicesFullTextSearch", Resources.ServicesFullTextSearchView16x16);
            imagesDarkTheme.Images.Add("ServicesReplication", Resources.ServicesReplicationView16x16);
            imagesDarkTheme.Images.Add("LogsSqlServer", Resources.LogsSqlServerFilter16x16);
            imagesDarkTheme.Images.Add("LogsSqlAgent", Resources.LogsSqlAgentFilter16x16);
            imagesDarkTheme.Images.Add("CustomCounter", Resources.CustomCounter16x16);

            imagesDarkTheme.Images.Add("ServersCritical", ImageHelper.GetBitmapFromSvgByteArray(Resources.Critical_Dark));
            imagesDarkTheme.Images.Add("ServersWarning", ImageHelper.GetBitmapFromSvgByteArray(Resources.Warning_Dark));
            imagesDarkTheme.Images.Add("ServersOK", ImageHelper.GetBitmapFromSvgByteArray(Resources.Ok_Dark));
            imagesDarkTheme.Images.Add("ServersMaintenanceMode", ImageHelper.GetBitmapFromSvgByteArray(Resources.MaintenanceMode_Dark));
            imagesDarkTheme.Images.Add("SearchServersCritical", Resources.ServerGroupCritical16x16);
            imagesDarkTheme.Images.Add("SearchServersWarning", Resources.ServerGroupWarning16x16);
            imagesDarkTheme.Images.Add("SearchServersOK", Resources.ServerGroupOK16x16);
            imagesDarkTheme.Images.Add("SearchServersMaintenanceMode", Resources.ServersMaintenanceMode);
            imagesDarkTheme.Images.Add("TablesCritical", Resources.TablesViewCritical16x16);
            imagesDarkTheme.Images.Add("TablesWarning", Resources.TablesViewWarning16x16);
            imagesDarkTheme.Images.Add("CustomCounterWarning", Resources.CustomCounterWarning16x16);
            imagesDarkTheme.Images.Add("CustomCounterCritical", Resources.CustomCounterCritical16x16);
            imagesDarkTheme.Images.Add("Tag", ImageHelper.GetBitmapFromSvgByteArray(Resources.Tag_Dark));
            imagesDarkTheme.Images.Add("TagCritical", ImageHelper.GetBitmapFromSvgByteArray(Resources.Critical_Dark));
            imagesDarkTheme.Images.Add("TagWarning", ImageHelper.GetBitmapFromSvgByteArray(Resources.Warning_Dark));
            imagesDarkTheme.Images.Add("Mirrored", Resources.DBmirroring_16);
            imagesDarkTheme.Images.Add("LogsWarning", Resources.LogsWarning16x16);
            imagesDarkTheme.Images.Add("LogsCritical", Resources.LogsCritical16x16);

            imagesDarkTheme.Images.Add("ResourcesWaitStats", Resources.Resources_server_waits_32x32);
            imagesDarkTheme.Images.Add("ResourcesActiveWaitStats", Resources.Resources_query_waits_32x32);

            imagesDarkTheme.Images.Add("QueriesSignatureMode", Resources.QueriesSignature);
            imagesDarkTheme.Images.Add("QueriesStatementMode", Resources.Queries);
            imagesDarkTheme.Images.Add("QueriesHistory", Resources.QueryHistory16);

            imagesDarkTheme.Images.Add("ResourcesFileActivity", Resources.ResourcesFileActivity32x32);

            // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding the disk size view image as a tree node 
            imagesDarkTheme.Images.Add("ResourcesDiskSize", Resources.ResourcesDiskSizeView16x16);
            imagesDarkTheme.Images.Add("Analysis", Resources.Analyze16);
            imagesDarkTheme.Images.Add("CloudCriticalSmall", Resources.cloud_database_icon_critical_16x16);
            imagesDarkTheme.Images.Add("CloudCritical", Resources.cloud_database_icon_critical_32x32);
            imagesDarkTheme.Images.Add("CloudOkSmall", Resources.cloud_database_icon_normal_16x16);
            imagesDarkTheme.Images.Add("CloudOk", Resources.cloud_database_icon_normal_32x32);
            imagesDarkTheme.Images.Add("CloudWarningSmall", Resources.cloud_database_icon_warning_16x16);
            imagesDarkTheme.Images.Add("CloudWarning", Resources.cloud_database_icon_warning_32x32);
            imagesDarkTheme.Images.Add("CloudDisabledSmall", Resources.cloud_database_icon_disabled_16x16);
            imagesDarkTheme.Images.Add("CloudDisabled", Resources.cloud_database_icon_disabled_32x32);

            imagesDarkTheme.Images.Add("CloudServerNormal", Resources.CloudServerNormal);
            imagesDarkTheme.Images.Add("CloudServerOk", Resources.CloudServerOk);
            imagesDarkTheme.Images.Add("CloudServerCritical", Resources.CloudServerCritical);
            imagesDarkTheme.Images.Add("CloudServerDisabled", Resources.CloudServerDisabled);
            imagesDarkTheme.Images.Add("CloudServerHover", Resources.CloudServerHover);
            imagesDarkTheme.Images.Add("CloudServerInformation", Resources.CloudServerInformation);
            imagesDarkTheme.Images.Add("CloudServerMaintenance", Resources.CloudServerMaintenance);
            imagesDarkTheme.Images.Add("CloudServerWarning", Resources.CloudServerWarning);
        }

        private void Initialize()
        {
            Settings.Default.ActiveRepositoryConnectionChanging += Settings_ActiveRepositoryConnectionChanging;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;

            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed += UserViews_Changed;
            }
            foreach (var k in ApplicationModel.Default.RepoActiveInstances.Keys)
            {
                ApplicationModel.Default.RepoActiveInstances[k].Changed += ActiveInstances_Changed;
            }
            ApplicationModel.Default.Tags.Changed += Tags_Changed;
            ApplicationModel.Default.FocusObjectChanged += ApplicationModel_FocusObjectChanged;
            ApplicationController.Default.ActiveViewChanging += ApplicationController_ActiveViewChanging;
            ApplicationController.Default.ActiveViewChanged += ApplicationController_ActiveViewChanged;
            ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;

            InitializeUserViews();
            InitializeTagViews();
        }

        private void ApplicationModel_FocusObjectChanged(object sender, EventArgs e)
        {
            // tree is changing the view so don't react to focus changes
            if (inShowView)
                return;

            TreeNode instanceNode = null;
            object focusedObject = ApplicationModel.Default.FocusObject;
            MonitoredSqlServer focusServer = null;

            if (focusedObject is MonitoredSqlServerWrapper)
                focusServer = ((MonitoredSqlServerWrapper)focusedObject).Instance;
            else
                if (focusedObject is MonitoredSqlServer)
                focusServer = (MonitoredSqlServer)focusedObject;

            if (focusServer != null)
            {
                if (instanceTreeNodeLookupTable.TryGetValue(focusServer.ClusterRepoId, out instanceNode))
                {
                    instanceNode.EnsureVisible();
                    userViewTreeView.SelectedNode = instanceNode;
                }
                else
                {
                    // make sure the all servers user view is selected
                    userViewsTreeView.Nodes[0].EnsureVisible();
                    userViewsTreeView.SelectedNode = userViewsTreeView.Nodes[0];
                    LoadSelectedUserView();
                    if (instanceTreeNodeLookupTable.TryGetValue(focusServer.ClusterRepoId, out instanceNode))
                    {
                        instanceNode.EnsureVisible();
                        userViewTreeView.SelectedNode = instanceNode;
                    }
                }
            }
        }

        public void InitializeUserViews()
        {
            if (images.Images.Count == 0 && imagesDarkTheme.Images.Count == 0)
            {
                initializeTreeViewDarkImageList();
                initializeTreeViewLightImageList();
            }
            userViewsTreeView.BeginUpdate();
            userViewsPanel.SuspendLayout();
            userViewsTreeView.Nodes.Clear();

            foreach (var userViewTreeNode in userViewTreeNodeLookupTable.Values)
            {
                if (userViewTreeNode.Tag is UserView)
                {
                    UserView userView = userViewTreeNode.Tag as UserView;
                    userView.NameChanged -= new EventHandler(UserView_NameChanged);
                    userView.SeverityChanged -= new EventHandler(UserView_SeverityChanged);
                    userView.InstancesChanged -= new EventHandler<UserViewInstancesChangedEventArgs>(UserViewInstances_Changed);
                }
            }
            userViewTreeNodeLookupTable.Clear();
            userViewTreeStatusLabel.Show();

            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                AllServersUserView allServers = AllServersUserView.Instance;
                allServers.Update();
                AddUserView(allServers, true);

                foreach (UserView userView in Settings.Default.ActiveRepositoryConnection.UserViews)
                {
                    if (userView is SearchUserView)
                    {
                        userView.Update();
                    }

                    AddUserView(userView, true);
                }
            }

            userViewsTreeView.EndUpdate();
            userViewsPanel.ResumeLayout();
            UpdateUserViewsPanel();
            userViewTreeStatusLabel.Hide();
            foreach (var inst in ApplicationModel.Default.RepoActiveInstances.Values)
            {
                foreach (MonitoredSqlServerWrapper wrapper in inst)
                {   // must know about certain server changes
                    wrapper.Changed += instance_Changed;
                }
            }
            LoadUserView(AllServersUserView.Instance);

            //SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27156
            //Showing loading label only when connected repository have more that zero monitored servers.
            //Sql Dm 10.2 (Tushar)--Fix for  defect SQLDM-27157--Added check for number of instances in application model before making visibility of loading label true.
            if (Settings.Default.ActiveRepositoryConnection.RepositoryInfo != null && Settings.Default.ActiveRepositoryConnection.RepositoryInfo.IsValidVersion
                && (ApplicationModel.Default.ActiveInstances.Count == 0 && Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount != 0))
                userViewTreeStatusLabel.Visible = true;

            if (userViewsTreeView.Nodes.Count != 0)
            {
                userViewsTreeView.SelectedNode = userViewsTreeView.Nodes[0];
            }
        }

        public void InitializeTagViews()
        {
            if (images.Images.Count == 0 && imagesDarkTheme.Images.Count == 0)
            {
                initializeTreeViewDarkImageList();
                initializeTreeViewLightImageList();
            }
            tagsTreeView.BeginUpdate();
            tagsPanel.SuspendLayout();
            tagsTreeView.Nodes.Clear();
            tagTreeNodeLookupTable.Clear();

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                AddTagView(tag);
            }

            tagsTreeView.SelectedNode = null;
            tagsTreeView.EndUpdate();
            tagsPanel.ResumeLayout();
            UpdateTagsPanel();
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            // walk the tree and update the status of all the server nodes
            if (userViewTreeView.Nodes.Count > 0)
            {
                userViewTreeView.BeginUpdate();

                foreach (TreeNode instanceNode in userViewTreeView.Nodes[0].Nodes)
                {
                    if (instanceNode is ServerTreeNode)
                    {
                        int instanceId = RepositoryHelper.GetSelectedInstanceId((Int32)instanceNode.Tag);
                        MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId, Settings.Default.RepoId);
                        ((ServerTreeNode)instanceNode).UpdateStatus(status);


                        if (status != null)
                        {
                            ConfigureCustomCounterNode((TypedTreeNode)instanceNode, status.CustomCounterCount > 0);
                            //Start : dm 10.0 vineet -- fixing defcet of console hang when servers are not reachable. 
                            //configure analyse tab
                            bool showAnalyseNode = false;
                            if (status.InstanceVersion != null)
                                showAnalyseNode = status.InstanceVersion.Major >= 9;
                            ConfigureAnalyseNode((TypedTreeNode)instanceNode, showAnalyseNode);
                            //End : dm 10.0 vineet -- fixing defcet of console hang when servers are not reachable. 
                            UpdateInstanceNodes((TypedTreeNode)instanceNode, status, ApplicationModel.Default.GetInstanceDisplayName(instanceId));
                        }
                    }
                }

                userViewTreeView.EndUpdate();
            }

            if (userViewsTreeView.Nodes.Count > 5)
            {
                userViewsTreeView.BeginUpdate();
                foreach (UserView userView in Settings.Default.ActiveRepositoryConnection.UserViews)
                {
                    if (userView is StaticUserView)
                    {
                        userView.Update();
                    }
                }
                userViewsTreeView.EndUpdate();
            }

        }

        private void ConfigureCustomCounterNode(TypedTreeNode serverNode, bool showCounterNode)
        {
            if (serverNode.Nodes.Count > 0)
            {
                TypedTreeNode firstChild = (TypedTreeNode)serverNode.Nodes[0];
                if (showCounterNode)
                {
                    if (firstChild.NodeType != TreeNodeType.CustomCounters)
                    {
                        //serverNode.Nodes.Insert(0,
                        //    new TypedTreeNode(TreeNodeType.CustomCounters, "Custom Counters", 48,
                        //                      ServerViews.OverviewDetails));
                    }
                }
                else
                {
                    if (firstChild.NodeType == TreeNodeType.CustomCounters)
                        serverNode.Nodes.Remove(firstChild);
                }
            }
        }

        /// <summary>
        /// dm 10.0 vineet -- fixing defcet of console hang when servers are not reachable. 
        /// This will be called once background refresh is complete. Will configure analyse node based on version
        /// </summary>
        /// <param name="serverNode"></param>
        /// <param name="showAnalyseNode"></param>
        private void ConfigureAnalyseNode(TypedTreeNode serverNode, bool showAnalyseNode)
        {
            if (serverNode.Nodes.Count > 0)
            {
                TypedTreeNode lastChild = (TypedTreeNode)serverNode.Nodes[serverNode.Nodes.Count - 1];
                if (showAnalyseNode)
                {
                    if (lastChild.NodeType != TreeNodeType.Analysis)
                    {
                        serverNode.Nodes.Add(
                            new TypedTreeNode(TreeNodeType.Analysis, "Analyze", 74, ServerViews.Analysis));
                    }
                }
            }
            //SQLdm 10.0(Srishti Purohit)
            //Defect fix for : SQLDM-25422
            //Once analyze tab is visible, no need to hide it.
            //else
            //{
            //    if (lastChild.NodeType == TreeNodeType.Analysis)
            //        serverNode.Nodes.Remove(lastChild);
            //}
        }

        private void UpdateInstanceNodes(TypedTreeNode startNode, MonitoredSqlServerStatus status, String displayInstanceName)
        {
            if (status != null)
            {
                //SQLdm 10.0.2: checking for IsRefereshing so that when alerts are getting refreshed for a particular server its Text is not updated 
                if (startNode.NodeType == TreeNodeType.Instance && status.Instance != null && !status.Instance.IsRefreshing)
                {
                    startNode.Text = displayInstanceName;
                }

                IEnumerator<TreeNode> enumerator = startNode.GetBottomUpEnumerator();

                while (enumerator.MoveNext())
                {
                    UpdateInstanceNode(enumerator.Current, status);
                }
            }
        }

        private void UpdateInstanceNode(TreeNode node, MonitoredSqlServerStatus status)
        {
            if (status != null)
            {
                // skip dummy nodes
                if ((node.Tag is string && (string)node.Tag == DummyTag) || !(node is TypedTreeNode))
                    return;

                TypedTreeNode typedNode = (TypedTreeNode)node;

                ICollection<Issue> issues = null;
                Issue[] issueArray = null;
                int issueCount = 0;

                switch (typedNode.NodeType)
                {
                    case TreeNodeType.Database:
                        DatabaseStatus dbStatus;
                        if (status.DatabaseMap.TryGetValue(typedNode.DisplayText, out dbStatus))
                        {
                            issues = dbStatus.Issues;
                            issueCount = dbStatus.IssueCount;
                        }
                        SetNodeImage(node, issues, issueCount, "Database", "DatabaseWarning", "DatabaseCritical");
                        break;
                    case TreeNodeType.Table:
                        TypedTreeNode dbNode = GetAncestor(typedNode, TreeNodeType.Database);
                        if (status.DatabaseMap.TryGetValue(dbNode.DisplayText, out dbStatus))
                        {
                            TableStatus tableStatus;
                            if (dbStatus.TableStatus.TryGetValue(typedNode.DisplayText, out tableStatus))
                            {
                                issueArray = tableStatus.Issues;
                                issueCount = tableStatus.IssueCount;
                            }
                        }
                        SetNodeImage(node, issueArray, issueCount, "Table", "TableWarning", "TableCritical");
                        break;
                    case TreeNodeType.SystemDatabases:
                        OrderedSet<Issue> topIssues = new OrderedSet<Issue>();
                        foreach (DatabaseStatus dbstatus in status.DatabaseMap.Values)
                        {
                            if (dbstatus.IsSystemDatabase && dbstatus.IssueCount > 0)
                            {
                                for (int i = 0; i < dbstatus.IssueCount; i++)
                                    topIssues.Add(dbstatus.Issues[i]);
                            }
                            issueCount = topIssues.Count;
                            if (issueCount > 3)
                                issueCount = 3;
                        }
                        SetNodeImage(node, topIssues, issueCount, "Folder", "FolderWarning", "FolderCritical");
                        break;
                    case TreeNodeType.SystemTables:
                        topIssues = new OrderedSet<Issue>();
                        dbNode = GetAncestor(typedNode, TreeNodeType.Database);
                        if (status.DatabaseMap.TryGetValue(dbNode.DisplayText, out dbStatus))
                        {
                            foreach (TableStatus tableStatus in dbStatus.TableStatus.Values)
                            {
                                if (tableStatus.IsSystemTable && tableStatus.IssueCount > 0)
                                {
                                    for (int i = 0; i < tableStatus.IssueCount; i++)
                                        topIssues.Add(tableStatus.Issues[i]);
                                }
                            }
                            issueCount = topIssues.Count;
                            if (issueCount > 3)
                                issueCount = 3;
                        }
                        SetNodeImage(node, topIssues, issueCount, "Folder", "FolderWarning", "FolderCritical");
                        break;
                    case TreeNodeType.Tables:
                        topIssues = new OrderedSet<Issue>();
                        dbNode = GetAncestor(typedNode, TreeNodeType.Database);
                        if (status.DatabaseMap.TryGetValue(dbNode.DisplayText, out dbStatus))
                        {
                            foreach (TableStatus tableStatus in dbStatus.TableStatus.Values)
                            {
                                if (tableStatus.IssueCount > 0)
                                {
                                    for (int i = 0; i < tableStatus.IssueCount; i++)
                                        topIssues.Add(tableStatus.Issues[i]);
                                }
                            }
                            issueCount = topIssues.Count;
                            if (issueCount > 3)
                                issueCount = 3;
                        }
                        SetNodeImage(node, topIssues, issueCount, "SessionsDetails", "TablesWarning", "TablesCritical");
                        break;
                    case TreeNodeType.Databases:
                        issues = status[MetricCategory.Databases];
                        SetNodeImage(node, issues, issues.Count, "Databases", "DatabasesWarning", "DatabasesCritical");
                        break;
                    case TreeNodeType.Queries:
                        if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major <= 8)
                        {
                            if (node.Nodes.ContainsKey("Query Waits"))
                                node.Nodes.RemoveByKey("Query Waits");

                            //break;
                        }
                        issues = status[MetricCategory.Queries];
                        SetNodeImage(node, issues, issues.Count, "Queries", "QueriesWarning", "QueriesCritical");
                        break;
                    case TreeNodeType.Services:
                        issues = status[MetricCategory.Services];
                        SetNodeImage(node, issues, issues.Count, "Services", "ServicesWarning", "ServicesCritical");
                        break;
                    case TreeNodeType.Resources:
                        if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major <= 8)
                        {
                            if (node.Nodes.ContainsKey("Server Waits"))
                                node.Nodes.RemoveByKey("Server Waits");

                            //if( node.Nodes.ContainsKey( "Query Waits" ) )
                            //    node.Nodes.RemoveByKey( "Query Waits" );

                            break;
                        }
                        issues = status[MetricCategory.Resources];
                        SetNodeImage(node, issues, issues.Count, "Resources", "ResourcesWarning", "ResourcesCritical");
                        break;
                    case TreeNodeType.Sessions:
                        issues = status[MetricCategory.Sessions];
                        SetNodeImage(node, issues, issues.Count, "Sessions", "SessionsWarning", "SessionsCritical");
                        break;
                    case TreeNodeType.CustomCounters:
                        //                        if (status.CustomCounterCount > 0)
                        //                        {
                        issues = status[MetricCategory.Custom];
                        SetNodeImage(node, issues, issues.Count, "CustomCounter", "CustomCounterWarning", "CustomCounterCritical");
                        //                        } else
                        //                            node.Remove();
                        break;
                    case TreeNodeType.LogsServer:
                    case TreeNodeType.LogsAgent:
                        issues = status[MetricCategory.Logs];
                        TypedTreeNode logsNode = GetAncestor(typedNode, TreeNodeType.Logs);
                        SetNodeImage(logsNode, issues, issues.Count, "Logs", "LogsWarning", "LogsCritical");
                        break;
                }
            }
        }

        private void SetNodeImage(TreeNode node, IEnumerable<Issue> issues, int issuesCount, string ok, string warning, string critical)
        {
            Issue[] issueArray = null;
            MonitoredState severity = MonitoredState.OK;
            if (issues != null && issuesCount > 0)
            {
                issueArray = Algorithms.ToArray(issues);
                severity = issueArray[0].Severity;
            }
            switch (severity)
            {
                case MonitoredState.Informational:
                case MonitoredState.OK:
                    if (!ok.Equals(node.ImageKey))
                    {
                        node.ImageKey = ok;
                        node.SelectedImageKey = ok;
                    }
                    break;
                case MonitoredState.Warning:
                    if (!warning.Equals(node.ImageKey))
                    {
                        node.ImageKey = warning;
                        node.SelectedImageKey = warning;
                    }
                    break;
                case MonitoredState.Critical:
                    if (!critical.Equals(node.ImageKey))
                    {
                        node.ImageKey = critical;
                        node.SelectedImageKey = critical;
                    }
                    break;
            }

        }

        #region Toolbar Manager Events

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            // customize menu items with state before the menu is shown
            if (e.Tool.Key == "serverPopupMenu")
            {
                // get the selected instance
                TreeNode selectedNode = userViewTreeView.SelectedNode;
                if (selectedNode.Level == 2)
                {
                    int selectedInstanceId = (int)userViewTreeView.SelectedNode.Tag;
                    MonitoredSqlServerWrapper selectedInstance = ApplicationModel.Default.ActiveInstances[selectedInstanceId];

                    PopupMenuTool maintenanceModeTool = (PopupMenuTool)toolbarsManager.Tools[TextConstants.MaintenanceModeButtonKey];
                    ButtonTool changeMaintenanceModeTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeDisableButtonKey];
                    ButtonTool enabledMaintenanceModeTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeEnableButtonKey];
                    ButtonTool scheduleMaintenanceTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeScheduleButtonKey];
                    ButtonTool snoozeAlertsTool = (ButtonTool)toolbarsManager.Tools["snoozeAlertsButton"];
                    ButtonTool unsnoozeAlertsTool = (ButtonTool)toolbarsManager.Tools["unsnoozeAlertsButton"];
                    ButtonTool applyTemplateTool = (ButtonTool)toolbarsManager.Tools["applyAlertTemplateButton"];

                    if (selectedNode is ServerTreeNode)
                    {   // disable action if node is currently refreshing
                        ButtonTool refreshTool = (ButtonTool)toolbarsManager.Tools["refreshAlertsButton"];
                        refreshTool.SharedProps.Enabled = !selectedInstance.IsRefreshing;
                    }

                    PermissionType permissionType =
                        ApplicationModel.Default.UserToken.GetServerPermission(selectedInstanceId);
                    //Operator Security Role Changes - 10.3
                    snoozeAlertsTool.SharedProps.Visible =
                      unsnoozeAlertsTool.SharedProps.Visible =
                      permissionType >= PermissionType.Modify || permissionType == PermissionType.ReadOnlyPlus;
                    toolbarsManager.Tools["configureBaselineButton"].SharedProps.Visible = permissionType >= PermissionType.Modify;

                    scheduleMaintenanceTool.SharedProps.Enabled = permissionType >= PermissionType.Modify;
                    if (selectedInstance != null)
                    {
                        //Operator Security Role Changes - 10.3
                        maintenanceModeTool.SharedProps.Visible = permissionType >= PermissionType.Modify || permissionType == PermissionType.ReadOnlyPlus;
                        if (selectedInstance.MaintenanceModeEnabled)
                        {
                            maintenanceModeTool.InitializeChecked(true);
                            changeMaintenanceModeTool.SharedProps.Enabled = true;
                            enabledMaintenanceModeTool.SharedProps.Enabled = false;
                        }
                        else
                        {
                            changeMaintenanceModeTool.SharedProps.Enabled = false;
                            enabledMaintenanceModeTool.SharedProps.Enabled = true;
                            maintenanceModeTool.InitializeChecked(false);
                        }

                        MonitoredSqlServerStatus status = MonitoredSqlServerStatus.FromBackgroundRefresh(selectedInstanceId);
                        if (status == null)
                            status = MonitoredSqlServerStatus.GetStatus(selectedInstanceId);

                        if (status != null)
                        {
                            snoozeAlertsTool.SharedProps.Enabled = !status.IsInMaintenanceMode && !status.AreAllAlertsSnoozed;
                            unsnoozeAlertsTool.SharedProps.Enabled = !status.IsInMaintenanceMode && status.SnoozingAlertCount > 0;
                        }
                    }

                    applyTemplateTool.SharedProps.Visible = permissionType >= PermissionType.Modify;

                    // fix the News Feed buttons
                    if (ApplicationModel.Default.IsPulseConfigured)
                    {
                        bool pulseEnabled = false;
                        int pulseId;
                        if (PulseController.Default.GetPulseServerId(selectedInstance.DisplayInstanceName, out pulseId))
                        {
                            UpdatePulseFollowButton(pulseId);
                            pulseEnabled = true;
                        }
                        toolbarsManager.Tools["pulseProfileButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["pulseFollowButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["pulseProfileButton"].SharedProps.Enabled = pulseEnabled;
                        toolbarsManager.Tools["pulseFollowButton"].SharedProps.Enabled = pulseEnabled && PulseController.Default.IsLoggedIn;
                    }
                    else
                    {
                        toolbarsManager.Tools["pulseProfileButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["pulseFollowButton"].SharedProps.Visible = false;
                    }
                }
            }
            else if ((e.Tool.Key == "tagsPopupMenu") || (e.Tool.Key == "tagPopupMenu"))
            {
                Tag tagSelected = tagsTreeView.SelectedNode.Tag as Tag;

                if (tagSelected != null)
                {
                    bool isEnableSnoozeMenu =
                        ApplicationModel.Default.UserToken.IsHasModifyServerPermission(tagSelected.Instances) && tagSelected.Instances.Count > 0;

                    ButtonTool snoozeAllserverAlertsTool = (ButtonTool)toolbarsManager.Tools[TextConstants.SnoozeServersAlertsButtonButtonKey];
                    ButtonTool unSnoozeAllserverAlertsTool = (ButtonTool)toolbarsManager.Tools[TextConstants.ResumeServersAlertsButtonButtonKey];
                    ButtonTool applyAlertTemplateOverTagTool = (ButtonTool)toolbarsManager.Tools[TextConstants.ApplyAlertTemplateTagButtonKey];

                    snoozeAllserverAlertsTool.SharedProps.Visible =
                        unSnoozeAllserverAlertsTool.SharedProps.Visible =
                        applyAlertTemplateOverTagTool.SharedProps.Visible =
                        toolbarsManager.Tools["configureBaselineButton"].SharedProps.Visible = isEnableSnoozeMenu;

                    EnabledMaintenance(true, tagSelected.Instances);

                    if (isEnableSnoozeMenu)
                    {
                        DeployServerInstanceToSnoozeAndToResumeList(tagSelected, out serverListToSnooze, out serverListToResume);
                        DeployServerInstanceToApplyAlertServerList(tagSelected.Instances);

                        snoozeAllserverAlertsTool.SharedProps.Enabled = this.serverListToSnooze.Count > 0;
                        unSnoozeAllserverAlertsTool.SharedProps.Enabled = this.serverListToResume.Count > 0;
                        applyAlertTemplateOverTagTool.SharedProps.Enabled = this.serverListToApplyAlertTemplate.Count > 0;
                    }

                    //get the tag name selected
                    tagNameSelected = tagSelected.Name;
                }

            }
            else if ("userViewPopupMenu".Equals(e.Tool.Key))
            {
                var selectedTag = userViewsTreeView.SelectedNode.Tag as UserView;
                ButtonTool snoozeAllAlertsButtonTool = toolbarsManager.Tools[SnoozeAllalertsServerButtonKey] as ButtonTool;
                ButtonTool resumeAllAlertsButtonTool = toolbarsManager.Tools[ResumeAllAlertsServerButtonKey] as ButtonTool;


                if (selectedTag != null && selectedTag.Instances != null)
                {
                    bool isEnableSnoozeMenu =
                        ApplicationModel.Default.UserToken.IsHasModifyServerPermission(selectedTag.Instances.ToList()) && selectedTag.Instances.Count > 0;

                    snoozeAllAlertsButtonTool.SharedProps.Visible =
                       resumeAllAlertsButtonTool.SharedProps.Visible =
                       toolbarsManager.Tools["configureBaselineButton"].SharedProps.Visible = isEnableSnoozeMenu;

                    if (isEnableSnoozeMenu)
                    {
                        EnabledMaintenance(true, selectedTag.Instances);

                        DeployServerInstanceToSnoozeAndToResumeList(selectedTag, out serverListToSnooze,
                                                                    out serverListToResume);

                        snoozeAllAlertsButtonTool.SharedProps.Enabled = this.serverListToSnooze.Count > 0;

                        resumeAllAlertsButtonTool.SharedProps.Enabled = this.serverListToResume.Count > 0;

                        //get the tag name selected
                        tagNameSelected = selectedTag.Name;
                    }
                }
                else
                {
                    snoozeAllAlertsButtonTool.SharedProps.Visible = resumeAllAlertsButtonTool.SharedProps.Visible = false;
                }
            }
        }

        private void EnabledMaintenance(bool enabled, ICollection<int> servers)
        {
            PopupMenuTool maintenanceModeTool = (PopupMenuTool)toolbarsManager.Tools[TextConstants.MaintenanceModeButtonKey];
            ButtonTool changeMaintenanceModeTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeDisableButtonKey];
            ButtonTool enabledMaintenanceModeTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeEnableButtonKey];
            //SQLDM 10.3 Operator security role fix (Kunal Kishore)
            ButtonTool scheduleMaintenanceModeTool = (ButtonTool)toolbarsManager.Tools[TextConstants.MaintenanceModeScheduleButtonKey];
            maintenanceModeTool.SharedProps.Enabled = enabled;
            changeMaintenanceModeTool.SharedProps.Enabled = enabled;
            enabledMaintenanceModeTool.SharedProps.Enabled = enabled;
            scheduleMaintenanceModeTool.SharedProps.Visible = ApplicationModel.Default.UserToken.IsHasModifyPermission(servers.ToList());
            maintenanceModeTool.SharedProps.Visible = ApplicationModel.Default.UserToken.IsHasModifyServerPermission(servers.ToList());
        }


        /// <summary>
        /// Deploy the server lists to snooze or resume alerts, acording the selected tag.
        /// </summary>
        /// <param name="tagSelected">The selected tag.</param>
        /// <param name="serverInstanceToSnoozeList">The list of the servers to snooze.</param>
        /// <param name="serverInstanceToResumeList">The list of the servers to un-snooze.</param>
        private static void DeployServerInstanceToSnoozeAndToResumeList<T>(T tagSelected,
            out List<int> serverInstanceToSnoozeList, out List<int> serverInstanceToResumeList)
        {
            serverInstanceToSnoozeList = new List<int>();
            serverInstanceToResumeList = new List<int>();
            ICollection<int> instances;
            // Get full server list.
            if (tagSelected is Tag)
            {
                instances = (tagSelected as Tag).Instances;
            }
            else if (tagSelected is UserView)
            {
                instances = (tagSelected as UserView).Instances;
            }
            else
            {
                instances = new List<int>();
            }

            // Deploy servers to snooze / un-snooze list.
            if (tagSelected != null)
            {
                foreach (int instanceId in instances)
                {
                    MonitoredSqlServerStatus status = MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
                    PermissionType permissionType = ApplicationModel.Default.UserToken.GetServerPermission(instanceId);

                    if (status == null)
                    {
                        status = MonitoredSqlServerStatus.GetStatus(instanceId);
                    }
                    //SQL DM 10.3 Operator Security Role Fix
                    if (permissionType >= PermissionType.ReadOnlyPlus && status != null)
                    {
                        if (!status.AreAllAlertsSnoozed)
                        {
                            serverInstanceToSnoozeList.Add(instanceId);
                        }
                        if (status.SnoozingAlertCount > 0)
                        {
                            serverInstanceToResumeList.Add(instanceId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deploy the server lists to Apply Alert templates over acording the selected tag.
        /// </summary>
        /// <param name="serverList">The list of the servers to apply Template.</param>
        private void DeployServerInstanceToApplyAlertServerList(IList<int> serverList)
        {
            this.serverListToApplyAlertTemplate = new List<int>();

            foreach (int instanceId in serverList)
            {
                PermissionType permissionType = ApplicationModel.Default.UserToken.GetServerPermission(instanceId);

                if (permissionType >= PermissionType.Modify)
                {
                    this.serverListToApplyAlertTemplate.Add(instanceId);
                }
            }
        }

        private void toolbarsManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            // post a message to run the reselect node method (pass dummy data to the method)
            if (treeViewReselectNode != null)
            {
                BeginInvoke(new PostMethodObject(ReselectNode), sender);
            }
        }

        private void OpenTargetView(TreeNode targetNode, bool properties)
        {
            if (targetNode == null)
            {
                if (userViewsTreeView.Focused)
                {
                    LoadSelectedUserView();
                }
                else if (tagsTreeView.Focused)
                {
                    LoadSelectedTagView();
                }
            }
            else if (targetNode.TreeView == userViewsTreeView)
            {
                LoadSelectedUserView();
            }
            else if (targetNode.TreeView == tagsTreeView)
            {
                LoadSelectedTagView();
            }

            ShowViewForSelectedNode(properties);
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            TreeNode reselectNode = treeViewReselectNode;
            treeViewReselectNode = null;

            switch (e.Tool.Key)
            {
                case "openButton":
                    OpenTargetView(reselectNode, false);
                    reselectNode = null;
                    break;
                case "refreshButton":
                case "refreshAlertsButton":
                    RefreshSelectedObject();
                    break;
                case "addServersButton":
                    ShowAddServersWizard();
                    break;
                case "addUserViewButton":
                    ShowAddUserViewDialog();
                    break;
                case "deleteButton":
                    DeleteSelectedObject();
                    break;
                case "removeInstanceFromViewButton":
                    RemoveSelectedInstanceFromSelectedView();
                    break;
                case "propertiesButton":
                    ShowSelectedObjectProperties();
                    break;
                case "manageServersButton":
                    ShowManageServersDialog();
                    break;
                case "renameUserViewButton":
                    userViewsSelectedNode = userViewsTreeView.SelectedNode;
                    BeginInvoke(new MethodInvoker(BeginEditingSelectedUserView));
                    break;
                case "moveUserViewUpInListButton":
                    MoveSelectedUserViewUpInList();
                    break;
                case "moveUserViewDownInListButton":
                    MoveSelectedUserViewDownInList();
                    break;
                case "configureAlertsButton":
                    ShowConfigureAlertsDialog();
                    break;
                case "applyAlertTemplateButton":
                    ShowApplyAlertTemplateDialog();
                    break;
                case "configureBaselineButton":
                    ShowConfigureBaselineDialog();
                    break;
                case "maintenanceModeStateButton":
                    ToggleMaintenanceMode();
                    break;
                case "snoozeAlertsButton":
                    SnoozeAlerts();
                    break;
                case "unsnoozeAlertsButton":
                    UnsnoozeAlerts();
                    break;
                case "changeMaintenanceModeButton":
                    SetMaintenanceMode(false);
                    break;
                case "addTagButton":
                    AddNewTag();
                    reselectNode = null;
                    break;
                case "deleteTagButton":
                    DeleteSelectedTag();
                    reselectNode = null;
                    break;
                case "tagPropertiesButton":
                    ShowSelectedTagProperties();
                    break;
                case TextConstants.SnoozeServersAlertsButtonButtonKey:
                    SnoozeAllServerAlerts();
                    break;
                case TextConstants.ResumeServersAlertsButtonButtonKey:
                    UnsnoozeAllServerAlerts();
                    break;
                case "manageTagsButton":
                    ManageTags();
                    break;
                case "pulseProfileButton":
                    ViewPulseProfile();
                    break;
                case "pulseFollowButton":
                    TogglePulseFollow();
                    break;
                case "viewAllServerProperties":
                    OpenTargetView(reselectNode, true);
                    reselectNode = null;
                    break;
                case SnoozeAllalertsServerButtonKey:
                    SnoozeAllServerAlerts();
                    break;
                case ResumeAllAlertsServerButtonKey:
                    UnsnoozeAllServerAlerts();
                    break;
                case TextConstants.MaintenanceModeScheduleButtonKey:
                    MaintenanceModeScheduleMode(reselectNode);
                    break;
                case TextConstants.MaintenanceModeEnableButtonKey:
                    MaintenanceModeAllNow(true);
                    break;
                case TextConstants.MaintenanceModeDisableButtonKey:
                    MaintenanceModeAllNow(false);
                    break;
                case TextConstants.ApplyAlertTemplateTagButtonKey:
                    ApplyAlertTemplateDialog();
                    break;
                case "MaintenanceModeButtonKey":
                    treeViewReselectNode = reselectNode;
                    break;
            }

            if (reselectNode != null && reselectNode.TreeView != null
                && !e.Tool.Key.Equals("MaintenanceModeButtonKey"))
            {
                reselectNode.TreeView.SelectedNode = reselectNode;
            }
        }

        private void MaintenanceModeAllNow(bool status)
        {
            object data = null;

            if (userViewTreeView.Focused)
            {
                if (userViewTreeView.SelectedNode.Level == 1)
                {
                    SetMaintenanceMode(status);
                }
            }
            if (userViewsTreeView.Focused)
            {
                data = userViewsTreeView.SelectedNode.Tag;
            }
            else if (tagsTreeView.Focused)
            {
                data = tagsTreeView.SelectedNode.Tag;
            }
            else if (userViewTreeView.Focused)
            {
                data = userViewTreeView.SelectedNode.Tag;
            }

            if (data == null)
            {
                return;
            }

            Dictionary<int, MonitoredSqlServerWrapper> servers = new Dictionary<int, MonitoredSqlServerWrapper>();

            if (data is AllServersUserView)
            {
                servers = ApplicationModel.Default.ActiveInstances.ToDictionary(a => a.Id);
            }
            else if (data is UserView)
            {
                UserView view = (UserView)data;
                servers =
                    ApplicationModel.Default.ActiveInstances.Where(a => view.Instances.Contains(a.Id)).ToDictionary(
                        a => a.Id);
            }
            else if (data is Tag)
            {
                Tag view = (Tag)data;
                servers =
                    ApplicationModel.Default.ActiveInstances.Where(a => view.Instances.Contains(a.Id)).ToDictionary(
                        a => a.Id);
            }

            if (servers.Count <= 0)
            {
                return;
            }

            foreach (var server in servers)
            {
                SetMaintenanceMode(status, server.Key);
            }
        }

        private void MaintenanceModeScheduleMode(TreeNode selectedNode)
        {
            if (userViewTreeView.SelectedNode != null && userViewTreeView.Focused)
            {
                if (userViewTreeView.SelectedNode.Level == 1)
                {
                    ToggleMaintenanceMode();
                }
            }

            object data = null;

            if (userViewsTreeView.Focused)
            {
                data = userViewsTreeView.SelectedNode.Tag;
            }
            else if (tagsTreeView.Focused)
            {
                data = tagsTreeView.SelectedNode.Tag;
            }
            else if (userViewTreeView.Focused)
            {
                data = userViewTreeView.SelectedNode.Tag;
            }

            if (data == null)
            {
                return;
            }

            MassMaintenanceModeDialog dialog = null;

            if (data is AllServersUserView)
            {
                dialog = new MassMaintenanceModeDialog();
            }
            else if (data is UserView)
            {
                dialog = new MassMaintenanceModeDialog((UserView)data);
            }
            else if (data is Tag)
            {
                dialog = new MassMaintenanceModeDialog((Tag)data);
            }

            if (dialog == null)
            {
                return;
            }

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                foreach (var server in dialog.SelectedServers())
                {
                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    try
                    {
                        ApplicationModel.Default.UpdateMonitoredSqlServer(server.Key, server.Value);
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while changing maintenance mode status.", e);
                    }
                }
            }
        }

        #region Actions

        private void BeginEditingSelectedUserView()
        {
            if (userViewsSelectedNode != null)
            {
                UserView userView = userViewsSelectedNode.Tag as UserView;
                if (userView is StaticUserView)
                {
                    userViewsSelectedNode.Text = userView.Name;
                    userViewsTreeView.LabelEdit = true;
                    userViewsSelectedNode.BeginEdit();
                }
            }
        }

        private void DeleteSelectedServer()
        {
            if (userViewTreeView.SelectedNode != null && userViewTreeView.SelectedNode.Level == 1)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning(ParentForm,
                                                      "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                      "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                      "Would you like to retain the data collected for the selected instance(s)?",
                                                      ExceptionMessageBoxButtons.YesNoCancel);

                if (dialogResult != DialogResult.Cancel)
                {
                    int selectedInstanceId = (int)userViewTreeView.SelectedNode.Tag;

                    try
                    {
                        if (dialogResult == DialogResult.Yes)
                        {
                            ApplicationModel.Default.DeactivateMonitoredSqlServers(
                                new MonitoredSqlServerWrapper[] { ApplicationModel.Default.ActiveInstances[selectedInstanceId] });
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            ApplicationModel.Default.DeleteMonitoredSqlServers(
                                new MonitoredSqlServerWrapper[] { ApplicationModel.Default.ActiveInstances[selectedInstanceId] });
                        }
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(ParentForm,
                                                        "An error occurred while removing the selected SQL Server instance.",
                                                        e);
                    }
                    ApplicationController.Default.RefreshActiveView();
                }
            }
        }

        private void DeleteSelectedObject()
        {
            if (userViewsTreeView.Focused &&
                userViewsTreeView.SelectedNode != null)
            {
                DeleteSelectedUserView();
            }
            else if (tagsTreeView.Focused)
            {
                DeleteSelectedTag();
            }
            else if (userViewTreeView.Focused &&
                     userViewTreeView.SelectedNode != null)
            {
                if (userViewTreeView.SelectedNode.Tag is UserView)
                {
                    DeleteSelectedUserView();
                }
                else if (userViewTreeView.SelectedNode.Tag is Tag)
                {
                    DeleteSelectedTag();
                }
                else if (userViewTreeView.SelectedNode.Level == 1 &&
                         ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                {
                    DeleteSelectedServer();
                }
            }
        }

        private void DeleteSelectedUserView()
        {
            UserView selectedView = null;

            if (userViewsTreeView.Focused &&
                userViewsTreeView.SelectedNode != null)
            {
                selectedView = userViewsTreeView.SelectedNode.Tag as UserView;
            }
            else if (userViewTreeView.Focused && userViewTreeView.SelectedNode != null)
            {
                selectedView = userViewTreeView.SelectedNode.Tag as UserView;
            }

            if (selectedView != null && selectedView != AllServersUserView.Instance && !(selectedView is SearchUserView))
            {
                if (
                    ApplicationMessageBox.ShowWarning(ParentForm,
                                                      string.Format(
                                                          "Are you sure you want to delete the view \"{0}\"?",
                                                          selectedView.Name), ExceptionMessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    Settings.Default.ActiveRepositoryConnection.UserViews.Remove(selectedView);
                }
            }
        }

        private void MoveSelectedUserViewUpInList()
        {
            if (userViewsTreeView.SelectedNode != null &&
                userViewsTreeView.SelectedNode.Tag != AllServersUserView.Instance)
            {
                UserView selectedView = userViewsTreeView.SelectedNode.Tag as UserView;

                if (selectedView != null)
                {
                    int currentIndex = userViewsTreeView.SelectedNode.Index;
                    Settings.Default.ActiveRepositoryConnection.UserViews.Remove(selectedView);
                    Settings.Default.ActiveRepositoryConnection.UserViews.Insert(currentIndex - 2, selectedView);
                }
            }
        }

        private void MoveSelectedUserViewDownInList()
        {
            if (userViewsTreeView.SelectedNode != null &&
                userViewsTreeView.SelectedNode.Tag != AllServersUserView.Instance &&
                userViewsTreeView.SelectedNode.Index != userViewsTreeView.Nodes.Count - 1)
            {
                UserView selectedView = userViewsTreeView.SelectedNode.Tag as UserView;

                if (selectedView != null)
                {
                    int currentIndex = userViewsTreeView.SelectedNode.Index;
                    Settings.Default.ActiveRepositoryConnection.UserViews.Remove(selectedView);
                    Settings.Default.ActiveRepositoryConnection.UserViews.Insert(currentIndex, selectedView);
                }
            }
        }

        private void RefreshSelectedObject()
        {
            if (userViewsTreeView.Focused && userViewsTreeView.SelectedNode != null)
            {

            }
            else if (userViewTreeView.Focused && userViewTreeView.SelectedNode != null)
            {
                if (userViewTreeView.SelectedNode.Level == 0)
                {
                    string nodeName = String.Empty;

                    if (userViewTreeView.SelectedNode.Tag is UserView)
                    {
                        nodeName = ((UserView)userViewTreeView.SelectedNode.Tag).Name;
                    }
                    else if (userViewTreeView.SelectedNode.Tag is Tag)
                    {
                        nodeName = ((Tag)userViewTreeView.SelectedNode.Tag).Name;
                    }

                    if (nodeName != String.Empty)
                    {
                        userViewTreeView.SelectedNode.Text = nodeName + " (refreshing...)";
                        ApplicationModel.Default.RefreshActiveInstances();
                        userViewTreeView.SelectedNode.Text = nodeName;
                    }
                }
                else if (userViewTreeView.SelectedNode.Level == 1)
                {
                    foreach (TreeNode childNode in userViewTreeView.SelectedNode.Nodes)
                    {
                        if (childNode is LazyTreeNode)
                        {
                            ((LazyTreeNode)childNode).Reset();
                        }
                    }
                    // background action to force a scheduled refresh and update status
                    ((ServerTreeNode)userViewTreeView.SelectedNode).ForceScheduledRefresh();
                }
                else if (userViewTreeView.SelectedNode is LazyTreeNode)
                {
                    LazyTreeNode lazyNode = (LazyTreeNode)userViewTreeView.SelectedNode;
                    lazyNode.Refresh();
                }
            }
        }

        private void RemoveSelectedInstanceFromSelectedView()
        {
            if (userViewTreeView.Nodes.Count > 0 &&
                userViewTreeView.SelectedNode.Level == 1 &&
                userViewTreeView.Nodes[0].Tag is StaticUserView)
            {
                StaticUserView view = userViewTreeView.Nodes[0].Tag as StaticUserView;

                if (view != null)
                {
                    view.RemoveInstance((int)userViewTreeView.SelectedNode.Tag);
                    userViewTreeView.SelectedNode = userViewTreeView.Nodes[0];
                    UserView userView = userViewTreeView.Nodes[0].Tag as UserView;

                    if (userView != null)
                    {
                        ApplicationModel.Default.FocusObject = userView;
                        ApplicationController.Default.ShowUserView(userView.Id);
                    }
                }
            }
        }

        private void ShowConfigureAlertsDialog()
        {
            if (userViewTreeView.SelectedNode.Level == 1)
            {
                int selectedInstanceId = (int)userViewTreeView.SelectedNode.Tag;
                try
                {
                    using (AlertConfigurationDialog dialog = new AlertConfigurationDialog(selectedInstanceId, false))
                    {
                        dialog.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                    ex);
                }
            }
        }

        private void ApplyAlertTemplateDialog()
        {
            if (this.serverListToApplyAlertTemplate != null && this.serverListToApplyAlertTemplate.Count > 0)
            {
                try
                {
                    using (SelectAlertTemplateForm applyAlertTemplate = new SelectAlertTemplateForm(serverListToApplyAlertTemplate, tagNameSelected))
                    {
                        applyAlertTemplate.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to apply the selected Alert template.  Please contact support for further assistance.",
                                                    ex);
                }
            }
        }

        private void ShowApplyAlertTemplateDialog()
        {
            if (userViewTreeView.SelectedNode.Level == 1)
            {
                int selectedInstanceId = (int)userViewTreeView.SelectedNode.Tag;
                try
                {
                    using (SelectAlertTemplateForm applyAlertTemplate = new SelectAlertTemplateForm(selectedInstanceId))
                    {
                        applyAlertTemplate.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to apply the selected Alert template.  Please contact support for further assistance.",
                                                    ex);
                }
            }
        }

        private void ShowConfigureBaselineDialog()
        {
            if (userViewTreeView.SelectedNode.Level == 1)
            {
                //int selectedInstanceId = (int)userViewTreeView.SelectedNode.Tag;
                //ConfigureBaselineDialog dialog = new ConfigureBaselineDialog(selectedInstanceId);
                //dialog.ShowDialog(this);

                MonitoredSqlServerInstancePropertiesDialog dialog = new MonitoredSqlServerInstancePropertiesDialog((int)userViewTreeView.SelectedNode.Tag);
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline;
                DialogResult result = dialog.ShowDialog(this);
            }
        }

        private void ShowSelectedObjectProperties()
        {
            if (userViewsTreeView.Focused && userViewsTreeView.SelectedNode != null)
            {
                if (userViewsTreeView.SelectedNode.Tag == AllServersUserView.Instance)
                {
                    ShowManageServersDialog();
                }
                else if (userViewsTreeView.SelectedNode.Tag is StaticUserView)
                {
                    StaticViewPropertiesDialog dialog =
                        new StaticViewPropertiesDialog(userViewsTreeView.SelectedNode.Tag as StaticUserView);
                    dialog.ShowDialog(this);
                }
            }
            else if (tagsTreeView.Focused)
            {
                ShowSelectedTagProperties();
            }
            else if (userViewTreeView.Focused && userViewTreeView.SelectedNode != null)
            {
                if (userViewTreeView.SelectedNode.Level == 0)
                {
                    if (userViewTreeView.SelectedNode.Tag == AllServersUserView.Instance)
                    {
                        ShowManageServersDialog();
                    }
                    else if (userViewTreeView.SelectedNode.Tag is StaticUserView)
                    {
                        StaticViewPropertiesDialog dialog =
                            new StaticViewPropertiesDialog(userViewTreeView.SelectedNode.Tag as StaticUserView);
                        dialog.ShowDialog(this);
                    }
                }
                else if (userViewTreeView.SelectedNode.Level == 1)
                {
                    try
                    {
                        MonitoredSqlServerInstancePropertiesDialog dialog =
                            new MonitoredSqlServerInstancePropertiesDialog((int)userViewTreeView.SelectedNode.Tag);
                        dialog.ShowDialog(this);
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(ParentForm,
                                                        string.Format("Unable to show properties for {0}.",
                                                                      userViewTreeView.SelectedNode.Text), e);
                    }
                }
            }
        }

        private void ShowAddServersWizard()
        {
            Dictionary<string, object> existingServers = new Dictionary<string, object>();

            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                existingServers.Add(instance.DisplayInstanceName, existingServers);
            }

            AddServersWizard wizard = new AddServersWizard(true, existingServers);
            wizard.ShowDialog(this);
        }

        private void ShowAddUserViewDialog()
        {
            StaticViewPropertiesDialog dialog = new StaticViewPropertiesDialog();
            dialog.ShowDialog(this);
        }

        private void ShowManageServersDialog()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                ManageServersDialog dialog = new ManageServersDialog();
                dialog.ShowDialog(this);
            }
        }

        private void ToggleMaintenanceMode()
        {
            if (userViewTreeView.SelectedNode.Level == 1)
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                MonitoredSqlServerInstancePropertiesDialog dialog =
                    new MonitoredSqlServerInstancePropertiesDialog((int)userViewTreeView.SelectedNode.Tag);
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
                DialogResult result = dialog.ShowDialog(this);

                try
                {
                    if (result == DialogResult.OK)
                    {
                        ApplicationModel.Default.ConfigureMaintenanceMode((int)userViewTreeView.SelectedNode.Tag, dialog.Configuration.MaintenanceModeEnabled);
                    }
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while changing maintenance mode status.", e);
                }
            }
        }

        private void SetMaintenanceMode(bool enabled, int serverId)
        {
            try
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                MonitoredSqlServerConfiguration config = ApplicationModel.Default.ActiveInstances[serverId].Instance.GetConfiguration();

                if (enabled)
                {
                    if (config.MaintenanceMode.MaintenanceModeType == MaintenanceModeType.Never)
                        config.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Always;
                }
                else
                {
                    if (config.MaintenanceMode.MaintenanceModeType == MaintenanceModeType.Always)
                        config.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                }

                config.MaintenanceMode.MaintenanceModeOnDemand = enabled;
                config.MaintenanceModeEnabled = enabled;
                ApplicationModel.Default.UpdateMonitoredSqlServer(serverId, config);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while changing maintenance mode status.", e);
            }
        }

        private void SetMaintenanceMode(bool state)
        {
            try
            {
                int serverId = (int)userViewTreeView.SelectedNode.Tag;
                SetMaintenanceMode(state, serverId);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while changing maintenance mode status.", e);
            }
        }

        /// <summary>
        /// Create a new Entity object with the AuditableActionType.MaintenanceModeManuallyDisabled and log it throw the AuditingEngine
        /// </summary>
        /// <param name="config"></param>
        private void AddAuditableLog(MonitoredSqlServerConfiguration config)
        {
            AuditableEntity entity = config.GetAuditableEntity();
            entity.AddMetadataProperty("Maintenance Mode selection changed", "Never");
            AuditingEngine.Instance.ManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            AuditingEngine.Instance.SQLUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
            AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeManuallyDisabled);
        }

        private void SnoozeAllServerAlerts()
        {
            if (this.serverListToSnooze != null && this.serverListToSnooze.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Snooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(this.FindForm(), this.serverListToSnooze, action, tagNameSelected);
            }
        }

        private void UnsnoozeAllServerAlerts()
        {
            if (this.serverListToResume != null && this.serverListToResume.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Unsnooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(FindForm(), this.serverListToResume, action, tagNameSelected);
            }
        }

        private void SnoozeAlerts()
        {
            SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Snooze;
            int instanceId = (int)userViewTreeView.SelectedNode.Tag;
            SnoozeAlertsDialog.SnoozeAllAlerts(FindForm(), instanceId, action);
        }

        private void UnsnoozeAlerts()
        {
            SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Unsnooze;
            int instanceId = (int)userViewTreeView.SelectedNode.Tag;
            SnoozeAlertsDialog.SnoozeAllAlerts(FindForm(), instanceId, action);
        }

        #endregion

        #endregion

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {

            userViewTreeView.BeginUpdate();

            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach (MonitoredSqlServerWrapper instance in e.Instances)
                    {
                        var id = Settings.Default.RepoId;
                        AddInstance(instance);
                        instance.Changed += instance_Changed;
                    }
                    break;
                case KeyedCollectionChangeType.Removed:
                    foreach (MonitoredSqlServerWrapper instance in e.Instances)
                    {
                        RemoveInstance(instance);
                        instance.Changed -= instance_Changed;
                    }
                    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach (MonitoredSqlServerWrapper instance in e.Instances)
                    {
                        UpdateInstance(instance);
                        instance.Changed += instance_Changed;
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    userViewTreeView.Nodes[0].Nodes.Clear(); // Clear below the root node
                    instanceTreeNodeLookupTable.Clear();
                    break;
            }

            userViewTreeView.EndUpdate();
            userViewTreeStatusLabel.Visible = false;//SqlDM 10.2 (Tushar)--Making loading label false when instances are added to tree.
            if (userViewsTreeView.Nodes.Count > 0)
            {
                userViewsTreeView.Nodes[0].Text = GetFormattedUserViewName(null);
            }
        }

        private void Tags_Changed(object server, TagCollectionChangedEventArgs e)
        {
            tagsTreeView.BeginUpdate();

            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        AddTagView(tag);
                    }
                    break;
                case KeyedCollectionChangeType.Removed:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        RemoveTagView(tag);
                    }
                    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        UpdateTagView(tag);
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    tagsTreeView.Nodes.Clear();
                    tagTreeNodeLookupTable.Clear();
                    break;
            }

            tagsTreeView.EndUpdate();
        }

        private void AddTagView(Tag tag)
        {
            if (tag != null)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator || tag.Instances.Count > 0)
                {
                    TreeNode selectedNode = tagsTreeView.SelectedNode;
                    TreeNode node = new TreeNode(String.Format("{0} ({1})", tag.Name, tag.Instances.Count));
                    node.Tag = tag;
                    node.ForeColor = leftNavTextColor;
                    node.NodeFont = new Font("Tahoma", 11f, GraphicsUnit.Pixel);
                    tagsTreeView.Nodes.Add(node);
                    tagsTreeView.Sort();
                    tagTreeNodeLookupTable.Add(tag.Id, node);

                    if (selectedNode != null)
                    {
                        tagsTreeView.SelectedNode = selectedNode;
                    }

                    AdjustTagsPanelHeight();
                    UpdateTagStatus(tag.Id);
                }
            }
        }

        private void RemoveTagView(Tag tag)
        {
            if (tag != null)
            {
                TreeNode existingNode;

                if (tagTreeNodeLookupTable.TryGetValue(tag.Id, out existingNode))
                {
                    if (existingNode == tagsTreeView.SelectedNode)
                    {
                        tagsTreeView.SelectedNode = null;
                        userViewsTreeView.SelectedNode = userViewsTreeView.Nodes[0];
                        LoadSelectedUserView();
                        ShowSelectedUserView();
                    }

                    existingNode.Remove();
                    tagTreeNodeLookupTable.Remove(tag.Id);
                    AdjustTagsPanelHeight();
                }
            }
        }

        private void UpdateTagView(Tag tag)
        {
            if (tag != null)
            {
                TreeNode existingNode;
                TreeNode selectedNode = tagsTreeView.SelectedNode;

                if (tagTreeNodeLookupTable.TryGetValue(tag.Id, out existingNode))
                {
                    if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator || tag.Instances.Count > 0)
                    {
                        Tag previousTag = (Tag)existingNode.Tag;
                        existingNode.Text = String.Format("{0} ({1})", tag.Name, tag.Instances.Count);
                        existingNode.Tag = tag;
                        tagsTreeView.Sort();

                        if (selectedNode != null)
                        {
                            tagsTreeView.SelectedNode = selectedNode;

                            if (previousTag == userViewTreeView.Nodes[0].Tag)
                            {
                                lock (userViewTreeViewLock)
                                {
                                    userViewTreeView.BeginUpdate();

                                    userViewTreeView.Nodes[0].Text = tag.Name;
                                    userViewTreeView.Nodes[0].Tag = tag;

                                    List<int> newInstances = new List<int>();
                                    List<int> diffInstances = new List<int>(previousTag.Instances);

                                    foreach (int instanceId in tag.Instances)
                                    {
                                        if (previousTag.Instances.Contains(instanceId))
                                        {
                                            diffInstances.Remove(instanceId);
                                        }
                                        else
                                        {
                                            newInstances.Add(instanceId);
                                        }
                                    }

                                    foreach (int instanceId in diffInstances)
                                    {
                                        TreeNode existingInstanceNode;
                                        if (
                                            instanceTreeNodeLookupTable.TryGetValue(instanceId, out existingInstanceNode))
                                        {
                                            instanceTreeNodeLookupTable.Remove(instanceId);
                                            existingInstanceNode.Remove();
                                        }
                                    }

                                    foreach (int instanceId in newInstances)
                                    {
                                        AddInstanceNode(instanceId, userViewTreeView.Nodes[0]);
                                    }

                                    userViewTreeView.EndUpdate();
                                }
                            }
                        }

                        UpdateTagStatus(tag.Id);
                    }
                    else
                    {
                        RemoveTagView(tag);
                    }
                }
                else
                {
                    AddTagView(tag);
                }
            }
        }

        private void LoadSelectedTagView()
        {
            if (tagsTreeView.SelectedNode != null)
            {
                userViewsTreeView.SelectedNode = null;
                LoadTagView(tagsTreeView.SelectedNode.Tag as Tag);

                if (!maintainCurrentView && userViewTreeView.Nodes.Count > 0)
                {
                    userViewTreeView.SelectedNode = userViewTreeView.Nodes[0];
                }
            }
        }

        private void LoadTagView(Tag tag)
        {
            lock (userViewTreeViewLock)
            {
                userViewTreeView.BeginUpdate();
                instanceTreeNodeLookupTable.Clear();
                userViewTreeView.Nodes.Clear();

                TreeNode node = new TreeNode(tag.Name);
                node.ImageKey =
                    node.SelectedImageKey = tagTreeNodeLookupTable[tag.Id].ImageKey;
                node.Tag = tag;
                node.NodeFont = new Font("Tahoma", 11f, GraphicsUnit.Pixel);
                node.ForeColor = leftNavTextColor;
                userViewTreeView.Nodes.Add(node);

                if (tag.Instances != null)
                {
                    foreach (int instanceId in tag.Instances)
                    {
                        AddInstanceNode(instanceId, node);
                    }
                }

                userViewTreeView.EndUpdate();
            }
        }

        private void ShowSelectedTagView()
        {
            if (inActiveViewChanged)
            {
                tagsSelectedNode = tagsTreeView.SelectedNode;
                return;
            }

            if (!maintainCurrentView)
            {
                Tag tag = tagsTreeView.SelectedNode.Tag as Tag;

                if (tag != null)
                {
                    ApplicationModel.Default.FocusObject = tag;
                    ApplicationController.Default.ShowTagView(tag.Id);
                }
            }

            tagsSelectedNode = tagsTreeView.SelectedNode;
        }

        void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            TreeNode node = null;
            if (instanceTreeNodeLookupTable.TryGetValue(e.Instance.InstanceId, out node))
            {
                ((ServerTreeNode)node).Refresh();

                bool customCountersExist = InstanceHasCustomCounters(e.Instance);
                ConfigureCustomCounterNode((TypedTreeNode)node, customCountersExist);

                UpdateInstanceNodes((ServerTreeNode)node, ApplicationModel.Default.GetInstanceStatus(e.Instance.Id, e.Instance.RepoId), e.Instance.DisplayInstanceName);//10.3 Bug fix - SQLDM-25513 : [Friend names keep being deleted]
            }
        }

        private void AddInstance(MonitoredSqlServer instance)
        {
            if (instance != null && userViewTreeView.Nodes.Count != 0)
            {
                Settings.Default.CurrentRepoId = instance.RepoId;
                if (userViewTreeView.Nodes[0].Tag == AllServersUserView.Instance)
                {
                    int repoId = Settings.Default.CurrentRepoId;
                    bool obj = userViewTreeView.Nodes[0].Nodes.ContainsKey(repoId.ToString());
                    if (!obj)
                    {
                        UserViewTreeView.Nodes[0].Nodes.Add(repoId.ToString(), "Syn-Cluster" + repoId + "");
                    }
                    int nodeSeqId = UserViewTreeView.Nodes[0].Nodes.Count;
                    AddInstanceNode(instance.Id, userViewTreeView.Nodes[0].Nodes[nodeSeqId - 1]);
                }
                else if (userViewTreeView.Nodes[0].Tag is StaticUserView)
                {
                    StaticUserView view = userViewTreeView.Nodes[0].Tag as StaticUserView;

                    if (view != null)
                    {
                        view.AddInstance(instance.Id);
                    }
                }
            }
        }

        private void RemoveInstance(MonitoredSqlServer instance)
        {
            if (instance != null)
            {
                lock (userViewTreeViewLock)
                {
                    TreeNode existingServerNode;

                    if (instanceTreeNodeLookupTable.TryGetValue(instance.ClusterRepoId, out existingServerNode))
                    {
                        TreeNode viewNode = existingServerNode.Parent;
                        userViewTreeView.SelectedNode = viewNode;

                        existingServerNode.Remove();
                        instanceTreeNodeLookupTable.Remove(instance.ClusterRepoId);

                    }
                }
                // force update of all views if the instance changed
                foreach (UserView view in Settings.Default.ActiveRepositoryConnection.UserViews)
                {
                    view.Update();
                }
            }
        }

        private void UpdateInstance(MonitoredSqlServer instance)
        {
            if (instance != null)
            {
                lock (userViewTreeViewLock)
                {
                    TreeNode existingServerNode;

                    if (instanceTreeNodeLookupTable.TryGetValue(instance.ClusterRepoId, out existingServerNode))
                    {
                        existingServerNode.ImageKey = MonitoredSqlServerStatus.GetServerImageKey(instance.Id);
                        existingServerNode.SelectedImageKey = existingServerNode.ImageKey;
                        existingServerNode.Text = instance.DisplayInstanceName;
                        bool customCountersExist = InstanceHasCustomCounters(instance);
                        ConfigureCustomCounterNode((TypedTreeNode)existingServerNode, customCountersExist);
                    }
                }
            }
        }

        private void UserViews_Changed(object sender, UserViewCollectionChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    AddUserView(e.UserView, false);
                    break;
                case KeyedCollectionChangeType.Removed:
                    RemoveUserView(e.UserView);
                    break;
            }
        }

        private string GetFormattedUserViewName(UserView userView)
        {
            string name = userView != null ? userView.Name : AllServersUserViewName;
            int count = 0;

            if (userView != null)
            {
                if (userView.Instances != null)
                {
                    count = userView.Instances.Count;
                }
            }
            else
            {
                count = 0;
                foreach (var inst in ApplicationModel.Default.RepoActiveInstances.Values)
                    count = count + inst.Count;
            }

            return String.Format("{0} ({1})", name, count);
        }

        Dictionary<string, string> userViewNodeIconDictionary = new Dictionary<string, string>
        {
            { "Warning", "ServersWarning" },
            { "Critical", "ServersCritical" },
            { "OK", "ServersOk" },
            { "Maintenance Mode", "ServersMaintenanceMode" }
        };

        private void AddUserView(UserView userView, bool suppressSelect)
        {
            if (userView != null)
            {
                userView.NameChanged += new EventHandler(UserView_NameChanged);
                userView.SeverityChanged += new EventHandler(UserView_SeverityChanged);
                userView.InstancesChanged += new EventHandler<UserViewInstancesChangedEventArgs>(UserViewInstances_Changed);

                TreeNode userViewNode = new TreeNode(GetFormattedUserViewName(userView));
                userViewNode.Tag = userView;
                userViewNode.ForeColor = leftNavTextColor;
                userViewNode.NodeFont = new Font("Tahoma", 11f, GraphicsUnit.Pixel);
                string imageKey = string.Empty;
                if (userViewNodeIconDictionary.TryGetValue(userView.Name, out imageKey))
                {
                    userViewNode.ImageKey = imageKey;
                    userViewNode.SelectedImageKey = imageKey;
                }
                int index = Settings.Default.ActiveRepositoryConnection.UserViews.IndexOf(userView);
                userViewsTreeView.Nodes.Insert(index + 1, userViewNode);
                userViewTreeNodeLookupTable.Add(userView.Id, userViewNode);

                userView.Update();
                // This will set the correct image for the user view
                UserView_SeverityChanged(userView, EventArgs.Empty);
                if (!suppressSelect)
                {
                    userViewsTreeView.SelectedNode = userViewNode;
                    LoadSelectedUserView();
                    ShowSelectedUserView();
                }

                AdjustUserViewsPanelHeight();
            }
        }

        private void UserView_NameChanged(object sender, EventArgs e)
        {
            UserView userView = sender as UserView;

            if (userView != null)
            {
                TreeNode existingNode;

                if (userViewTreeNodeLookupTable.TryGetValue(userView.Id, out existingNode))
                {
                    string displayName = GetFormattedUserViewName(userView);
                    existingNode.Text = displayName;

                    if (userViewTreeView.Nodes[0].Tag == existingNode.Tag)
                    {
                        userViewTreeView.Nodes[0].Text = displayName;
                    }
                }
            }
        }

        private void UserView_SeverityChanged(object sender, EventArgs e)
        {
            UserView userView = sender as UserView;

            if (userView != null)
            {
                TreeNode existingNode;

                if (userViewTreeNodeLookupTable.TryGetValue(userView.Id, out existingNode))
                {
                    string imageKey = string.Empty;
                    if (userViewNodeIconDictionary.TryGetValue(userView.Name, out imageKey))
                    {
                        existingNode.ImageKey = imageKey;
                        existingNode.SelectedImageKey = imageKey;
                    }
                    else
                        UpdateUserViewTreeNode(existingNode, userView);
                    if (userViewTreeView.Nodes.Count > 0 &&
                        userViewTreeView.Nodes[0].Tag == existingNode.Tag)
                    {
                        userViewTreeView.Nodes[0].ImageKey = existingNode.ImageKey;
                        userViewTreeView.Nodes[0].SelectedImageKey = existingNode.SelectedImageKey;
                    }
                }
            }
        }

        private void UpdateUserViewTreeNode(TreeNode node, UserView userView)
        {
            string imageKey = "Servers";

            if (userView != null)
            {
                UserViewStatus severity = userView.Severity;
                if (severity != UserViewStatus.None)
                    imageKey += severity.ToString();
                else
                    imageKey += UserViewStatus.Critical.ToString();

                node.ImageKey = imageKey;
                node.SelectedImageKey = imageKey;
            }
        }

        private void RemoveUserView(UserView userView)
        {
            if (userView != null)
            {
                TreeNode existingNode;

                if (userViewTreeNodeLookupTable.TryGetValue(userView.Id, out existingNode))
                {
                    userView.NameChanged -= new EventHandler(UserView_NameChanged);
                    userView.SeverityChanged -= new EventHandler(UserView_SeverityChanged);
                    userView.InstancesChanged -= new EventHandler<UserViewInstancesChangedEventArgs>(UserViewInstances_Changed);
                    userViewsTreeView.SelectedNode = userViewsTreeView.Nodes[0];
                    LoadSelectedUserView();
                    ShowSelectedUserView();
                    existingNode.Remove();
                    userViewTreeNodeLookupTable.Remove(userView.Id);
                    AdjustUserViewsPanelHeight();
                }
            }
        }

        private void Settings_ActiveRepositoryConnectionChanging(object sender, EventArgs e)
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed -=
                    new EventHandler<UserViewCollectionChangedEventArgs>(UserViews_Changed);
            }
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed +=
                    new EventHandler<UserViewCollectionChangedEventArgs>(UserViews_Changed);
                InitializeUserViews();
                InitializeTagViews();
            }
        }

        private void AdjustUserViewsPanelHeight()
        {
            if (userViewsTreeView.Nodes.Count != 0)
            {
                int headerHeight = userViewsHeaderStrip.Height;
                int nodeHeight = userViewsTreeView.Nodes[0].Bounds.Height;
                int nodeCount = userViewsTreeView.Nodes.Count <= GroupPanelsMaxCount ? userViewsTreeView.Nodes.Count : GroupPanelsMaxCount;
                int padding = 3;
                userViewsPanel.Height = nodeHeight * nodeCount + headerHeight + padding;
            }
            else
            {
                userViewsPanel.Height = 50;
            }
        }

        private void ApplicationController_ActiveViewChanging(object sender, ActiveViewChangingEventArgs e)
        {
            //if (e.OldView is ServerView)
            //{
            //    ServerView serverView = e.OldView as ServerView;

            //    if (serverView != null)
            //    {
            //        serverView.SubViewChanged -= new EventHandler<SubViewChangedEventArgs>(serverView_SubViewChanged);
            //    }
            //}

            if (e.OldView is ServerViewContainer)
            {
                ServerViewContainer serverView = e.OldView as ServerViewContainer;

                if (serverView != null)
                {
                    serverView.SubViewChanged -= new EventHandler<SubViewChangedEventArgs>(serverView_SubViewChanged);
                }
            }
        }

        private void ApplicationController_ActiveViewChanged(object sender, EventArgs e)
        {
            if (inActiveViewChanged)
                return;

            if (ApplicationController.Default.ActiveView is ServerViewContainer)
            {
                var serverView = ApplicationController.Default.ActiveView as ServerViewContainer;

                if (serverView != null)
                {
                    TreeNode existingNode;

                    if (instanceTreeNodeLookupTable.TryGetValue(serverView.InstanceId, out existingNode))
                    {
                        //                        int tabIndex = (int) serverView.SelectedTab;
                        //
                        //                        if (tabIndex == 0)
                        //                        {
                        //                            userViewTreeView.SelectedNode = existingNode;
                        //                        }
                        //                        else
                        //                        {
                        //                            existingNode.Expand();
                        //                            userViewTreeView.SelectedNode = existingNode.Nodes[tabIndex - 1];
                        //                        }

                        serverView.SubViewChanged += new EventHandler<SubViewChangedEventArgs>(serverView_SubViewChanged);
                    }
                }
            }
            else if (ApplicationController.Default.ActiveView is ServerGroupView)
            {
                TreeNode existingNode = null;
                ServerGroupView groupView = (ServerGroupView)ApplicationController.Default.ActiveView;

                if (groupView.View == null)
                {
                    existingNode = userViewsTreeView.Nodes[0];
                }
                else if (groupView.View is UserView)
                {
                    UserView userView = (UserView)(groupView.View);
                    userViewTreeNodeLookupTable.TryGetValue(userView.Id, out existingNode);
                }
                else if (groupView.View is int && ApplicationModel.Default.Tags.Contains((int)(groupView.View)))
                {
                    int tagId = (int)(groupView.View);

                    if (ApplicationModel.Default.Tags.Contains(tagId))
                    {
                        Tag tag = ApplicationModel.Default.Tags[tagId];
                        tagTreeNodeLookupTable.TryGetValue(tag.Id, out existingNode);
                    }
                }

                if (existingNode != null && !existingNode.IsSelected)
                {
                    inActiveViewChanged = true;

                    try
                    {
                        existingNode.TreeView.SelectedNode = existingNode;
                    }
                    finally
                    {
                        inActiveViewChanged = false;
                    }
                }
            }
        }

        private void serverView_SubViewChanged(object sender, SubViewChangedEventArgs e)
        {
            try
            {
                if (inShowView)
                    return;
                currentServerSubTab = e.NewView;
                if (ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper && ApplicationController.Default.ActiveView is ServerViewContainer)
                {
                    MonitoredSqlServerWrapper instance = ApplicationModel.Default.FocusObject as MonitoredSqlServerWrapper;
                    var view = ApplicationController.Default.ActiveView as ServerViewContainer;

                    if (instance != null && view != null)
                    {
                        TreeNode existingNode;
                        int tabIndex = 0;
                        if (instanceTreeNodeLookupTable.TryGetValue(instance.InstanceId, out existingNode))
                        {
                            if (ApplicationModel.Default.AllInstances.ContainsKey(instance.Id) &&
                                ApplicationModel.Default.AllInstances[instance.Id].CloudProviderId ==
                                Common.Constants.MicrosoftAzureId)
                            {
                                if (((ServerViewContainer)sender).SelectedTab.ToString() == "Sessions")
                                    tabIndex = 1;
                                if (((ServerViewContainer)sender).SelectedTab.ToString() == "Resources")
                                    tabIndex = 2;
                                else if (((ServerViewContainer)sender).SelectedTab.ToString() == "Databases")
                                    tabIndex = 3;
                            }
                            else
                                tabIndex = (int)e.SelectedTab;

                            //SQLdm 10.0(Srishti Purohit)
                            //Defect fix for : SQLDM-25422
                            //Left side analyze tree node should be visible if Analyze tab is visible on top view


                            bool showAnalyzeNode = view.Analyze.Visibility == System.Windows.Visibility.Visible;
                            ConfigureAnalyseNode((TypedTreeNode)existingNode, showAnalyzeNode);
                            if (ApplicationModel.Default.AllInstances[instance.Id].CloudProviderId == Common.Constants.MicrosoftAzureId)
                            {
                                if (((ServerViewContainer)sender).SelectedTab.ToString() == "Analysis")
                                    tabIndex = 4;
                            }

                            if (tabIndex == 0)
                            {
                                if (view.ActiveView is ServerDetailsView && ((ServerDetailsView)view.ActiveView).Filter == ServerDetailsView.Selection.Custom)
                                {
                                    TypedTreeNode firstChildNode =
                                        existingNode.Nodes != null && existingNode.Nodes.Count > 0
                                            ? existingNode.Nodes[0] as TypedTreeNode
                                            : null;
                                    if (firstChildNode == null || firstChildNode.NodeType != TreeNodeType.CustomCounters || !firstChildNode.IsSelected)
                                    {
                                        userViewTreeView.SelectedNode = existingNode;
                                    }
                                }
                                else
                                    userViewTreeView.SelectedNode = existingNode;
                            }
                            else
                            {
                                if (!existingNode.IsExpanded)
                                    existingNode.Expand();

                                //10.6 removes nodes from instance nodes 
                                if (existingNode.Nodes != null && existingNode.Nodes.Count > 0)
                                {
                                    TypedTreeNode firstChildNode = existingNode.Nodes[0] as TypedTreeNode;
                                    if (firstChildNode != null && firstChildNode.NodeType == TreeNodeType.CustomCounters)
                                    {
                                        tabIndex++;
                                    }
                                }


                                if (ApplicationModel.Default.AllInstances[instance.Id].CloudProviderId == Common.Constants.AmazonRDSId)
                                {
                                    if (((ServerViewContainer)sender).SelectedTab.ToString() == "Analysis")
                                        tabIndex = 5;
                                }
                                //10.6 removes nodes from instance nodes 
                                if (existingNode.Nodes != null && existingNode.Nodes.Count > 0)
                                {
                                    TreeNode categoryNode = existingNode.Nodes[tabIndex - 1];
                                    userViewTreeView.SelectedNode = categoryNode;

                                    if (ServerViews.Databases != (ServerViews)categoryNode.Tag)
                                    {
                                        userViewTreeView.SelectedNode.Expand();
                                        foreach (TreeNode node in userViewTreeView.SelectedNode.Nodes)
                                        {
                                            if (node.Tag is ServerViews && ((ServerViews)node.Tag) == e.NewView)
                                            {
                                                userViewTreeView.SelectedNode = node;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("serverView_SubViewChanged: Handling Exception for the UI" + ex);
            }
        }

        private void AddInstanceNode(int instanceId, TreeNode userViewNode)
        {
            lock (userViewTreeViewLock)
            {   //SQLDM-31015
                if (ApplicationModel.Default.AllRepoInstances.ContainsKey(instanceId))
                {
                    //foreach (ServerTreeNode n in userViewNode.Nodes)
                    //    if (n.Tag != null && n.Tag is int && (int)n.Tag == instanceId)
                    //        return;
                    MonitoredSqlServer instance = ApplicationModel.Default.RepoActiveInstances[Settings.Default.RepoId][instanceId];
                    // ServerTreeNode instanceNode = new ServerTreeNode(instance.DisplayInstanceName, 0, instanceId);
                    ServerTreeNode instanceNode = new ServerTreeNode(instance.DisplayInstanceName, 0, instance.ClusterRepoId);
                    //SQLDM-31053
                    if (ApplicationModel.Default.AllRepoRegisteredInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllRepoRegisteredInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId || ApplicationModel.Default.AllRepoRegisteredInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllRepoRegisteredInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                    {
                        instanceNode.ImageKey = MonitoredSqlServerStatus.GetAzureImageKey(instanceId);
                    }
                    else
                    {
                        instanceNode.ImageKey = MonitoredSqlServerStatus.GetServerImageKey(instanceId);
                    }
                    instanceNode.SelectedImageKey = instanceNode.ImageKey;
                    instanceNode.Text = instance.DisplayInstanceName;
                    instanceNode.ForeColor = leftNavTextColor;
                    instanceNode.NodeFont = new Font("Tahoma", 11f, GraphicsUnit.Pixel);

                    //if (InstanceHasCustomCounters(instance))
                    //   instanceNode.Nodes.Add(new TypedTreeNode(TreeNodeType.CustomCounters, "Custom Counters", 48, ServerViews.OverviewDetails));

                    userViewNode.Nodes.Insert(GetInstanceInsertIndex(instance.DisplayInstanceName, userViewNode), instanceNode);
                    userViewNode.Expand();

                    if (instanceTreeNodeLookupTable.ContainsKey(instance.ClusterRepoId))
                    {
                        instanceTreeNodeLookupTable.Remove(instance.ClusterRepoId);
                    }

                    instanceTreeNodeLookupTable.Add(instance.ClusterRepoId, instanceNode);
                    //  var id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                    instanceNode.UpdateStatus(ApplicationModel.Default.GetInstanceStatus(instanceId, Settings.Default.RepoId));
                }
            }
        }

        private int GetInstanceInsertIndex(string instanceName, TreeNode userViewNode)
        {
            int index = 0;

            if (userViewNode != null)
            {
                foreach (TreeNode instanceNode in userViewNode.Nodes)
                {
                    if (string.Compare(instanceName, instanceNode.Text) > 0)
                    {
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return index;
        }

        private void SetUserViewTreeViewContextMenu(TreeNode selectedNode)
        {
            if (selectedNode != null)
            {
                if (selectedNode.Level == 0)
                {
                    toolbarsManager.Tools["refreshButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["addUserViewButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["moveUserViewUpInListButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["moveUserViewDownInListButton"].SharedProps.Visible = false;

                    if (selectedNode.Tag == AllServersUserView.Instance)
                    {
                        toolbarsManager.Tools["addServersButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["deleteButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["manageServersButton"].SharedProps.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                        toolbarsManager.Tools["viewAllServerProperties"].SharedProps.Visible = true;

                        toolbarsManager.Tools[SnoozeAllalertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools[ResumeAllAlertsServerButtonKey].SharedProps.Visible = true;

                        toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = false;
                        toolbarsManager.SetContextMenuUltra(this, "userViewPopupMenu");
                    }
                    else if (selectedNode.Tag is UserView)
                    {
                        UserView userView = userViewsTreeView.SelectedNode.Tag as UserView;
                        toolbarsManager.Tools["manageServersButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["viewAllServerProperties"].SharedProps.Visible = false;

                        toolbarsManager.Tools[SnoozeAllalertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools[ResumeAllAlertsServerButtonKey].SharedProps.Visible = true;

                        if (userView is StaticUserView)
                        {
                            toolbarsManager.Tools["deleteButton"].SharedProps.Caption = "Delete \"" + userView.Name +
                                                                                        "\"";
                            toolbarsManager.Tools["renameUserViewButton"].SharedProps.Caption = "Rename \"" +
                                                                                                userView.Name + "\"...";
                            toolbarsManager.Tools["addServersButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["deleteButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = true;
                        }
                        else if (userView is SearchUserView)
                        {
                            toolbarsManager.Tools["addServersButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["deleteButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = false;
                        }

                        toolbarsManager.SetContextMenuUltra(this, "userViewPopupMenu");
                    }
                    else if (selectedNode.Tag is Tag)
                    {
                        if (((Tag)selectedNode.Tag).IsGlobalTag) // SQLdm 10.1 - Praveen Suhalka - CWF3 Integration
                        {
                            toolbarsManager.Tools["deleteTagButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["tagPropertiesButton"].SharedProps.Visible = false;
                        }
                        else
                        {
                            toolbarsManager.Tools["deleteTagButton"].SharedProps.Visible =
                            toolbarsManager.Tools["tagPropertiesButton"].SharedProps.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                        }

                        toolbarsManager.Tools["snoozeServersAlertsButton"].SharedProps.Visible =
                        toolbarsManager.Tools["resumeServersAlertsButton"].SharedProps.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                        toolbarsManager.SetContextMenuUltra(this, "tagPopupMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(this, null);
                    }
                }
                else if (selectedNode.Level == 2)
                {
                    toolbarsManager.Tools["deleteButton"].SharedProps.Caption = "Delete";
                    toolbarsManager.Tools["deleteButton"].SharedProps.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                    toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeInstanceFromViewButton"].SharedProps.Visible = selectedNode.Parent.Tag is StaticUserView;
                    toolbarsManager.SetContextMenuUltra(this, "serverPopupMenu");
                }
                else if (selectedNode is LazyTreeNode)
                {
                    toolbarsManager.SetContextMenuUltra(this, "LazyLoadingNodePopupMenu");
                }
                else
                {
                    toolbarsManager.SetContextMenuUltra(this, null);
                }
            }
            else
            {
                toolbarsManager.SetContextMenuUltra(this, null);
            }
        }

        private void userViewsHeaderStrip_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolbarsManager.SetContextMenuUltra(this, null);

            if (e.Button == MouseButtons.Left)
            {
                ToggleUserViewsExpanded();
            }
        }

        private void ToggleUserViewsExpanded()
        {
            Settings.Default.NavigationPaneUserViewsExpanded = !Settings.Default.NavigationPaneUserViewsExpanded;
            UpdateUserViewsPanel();
        }

        private void UpdateUserViewsPanel()
        {
            if (Settings.Default.NavigationPaneUserViewsExpanded)
            {
                toggleUserViewsButton.Image = ImageHelper.GetBitmapFromSvgByteArray(downArrowSvg);
                userViewsTreeView.Visible = true;
                AdjustUserViewsPanelHeight();
            }
            else
            {
                toggleUserViewsButton.Image = ImageHelper.GetBitmapFromSvgByteArray(leftArrowSvg);
                userViewsTreeView.Visible = false;
                userViewsPanel.Height = 33;
            }
        }

        private void LoadUserView(UserView userView)
        {
            lock (userViewTreeViewLock)
            {
                userViewTreeView.BeginUpdate();
                instanceTreeNodeLookupTable.Clear();
                userViewTreeView.Nodes.Clear();

                TreeNode userViewNode = new TreeNode(userView.Name);
                userViewNode.Tag = userView;
                userViewNode.NodeFont = new Font("Tahoma", 11f, GraphicsUnit.Pixel);
                userViewNode.ForeColor = leftNavTextColor;
                string imageKey = string.Empty;
                if (userViewNodeIconDictionary.TryGetValue(userViewNode.Text, out imageKey))
                {
                    userViewNode.ImageKey = imageKey;
                    userViewNode.SelectedImageKey = imageKey;
                }
                else
                    UpdateUserViewTreeNode(userViewNode, userView);
                userViewTreeView.Nodes.Add(userViewNode);

                if (userView.Instances.Count > 0)
                {
                    if (ApplicationModel.Default.RepoActiveInstances.Values.Count > 0 && (userView.Name == AllServersUserViewName ||
                        userView.Name == "Critical" || userView.Name == "OK" || userView.Name == "Warning" || userView.Name == "Maintenance Mode"))
                    {
                        foreach (int instanceId in userView.Instances)
                        {
                            CreateHierarchyOfTreeView(instanceId);
                        }
                    }
                    else
                    {
                        foreach (int instanceId in userView.Instances)
                        {
                            var id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                            AddInstanceNode(id, userViewNode);
                        }
                    }
                }

                userViewTreeView.EndUpdate();
            }
        }

        private void LoadSelectedUserView()
        {
            if (userViewsTreeView.SelectedNode != null)
            {
                tagsTreeView.SelectedNode = null;
                LoadUserView(userViewsTreeView.SelectedNode.Tag as UserView);

                if (!maintainCurrentView && userViewTreeView.Nodes.Count > 0)
                {
                    userViewTreeView.SelectedNode = userViewTreeView.Nodes[0];
                }
            }
        }

        private void UserViewInstances_Changed(object sender, UserViewInstancesChangedEventArgs e)
        {
            if (userViewTreeView.Nodes.Count != 0 && sender == userViewTreeView.Nodes[0].Tag)
            {
                switch (e.ChangeType)
                {
                    case UserViewInstancesChangeType.Added:
                        CreateHierarchyOfTreeView(e.InstanceId);
                        //AddInstanceNode(e.InstanceId, userViewTreeView.Nodes[0]);
                        break;
                    case UserViewInstancesChangeType.Removed:
                        TreeNode existingNode;
                        if (instanceTreeNodeLookupTable.TryGetValue(e.InstanceId, out existingNode))
                        {
                            if (sender is SearchUserView && userViewTreeView.SelectedNode == existingNode)
                            {
                                // we need to follow this instance to its new search view but we
                                // can't determine which one that is yet.  Post an event to handle
                                // doing this.
                                BeginInvoke(new PostMethodObject(SelectInstanceInSearchView), e.InstanceId);
                            }
                            else
                            {
                                instanceTreeNodeLookupTable.Remove(e.InstanceId);
                                existingNode.Remove();
                            }
                        }
                        break;
                    case UserViewInstancesChangeType.Cleared:
                        userViewTreeView.Nodes[0].Nodes.Clear();
                        instanceTreeNodeLookupTable.Clear();
                        break;
                }
            }

            // update the count for the view
            UserView userView = sender as UserView;
            if (userView != null)
            {
                TreeNode existingNode;

                if (userViewTreeNodeLookupTable.TryGetValue(userView.Id, out existingNode))
                {
                    existingNode.Text = GetFormattedUserViewName(userView);
                }
            }
        }

        private void SelectInstanceInSearchView(object arg)
        {
            int instanceId = (int)arg;
            // try to find the search user view that this instance is now in, select that search view and focus on the instance.
            RepositoryConnection connection = Settings.Default.ActiveRepositoryConnection;
            if (connection == null)
                return;

            UserView userView = connection.UserViews[SearchUserView.CriticalUserViewID];
            if (!userView.Instances.Contains(instanceId))
            {
                userView = connection.UserViews[SearchUserView.MaintenanceModeUserViewID];
                if (!userView.Instances.Contains(instanceId))
                {
                    userView = connection.UserViews[SearchUserView.WarningUserViewID];
                    if (!userView.Instances.Contains(instanceId))
                    {
                        userView = connection.UserViews[SearchUserView.OKUserViewID];
                        if (!userView.Instances.Contains(instanceId))
                        {
                            return;
                        }
                    }
                }
            }

            TreeNode userViewsTreeNode = null;
            if (userViewTreeNodeLookupTable.TryGetValue(userView.Id, out userViewsTreeNode))
            {
                // turn off as much event logic as possible
                try
                {
                    userViewsTreeView.BeginUpdate();
                    userViewTreeView.BeginUpdate();
                    // flag to disable default node selection on view changes
                    maintainCurrentView = true;
                    // setting the selected node will still fire AfterSelect                
                    userViewsTreeView.SelectedNode = userViewsTreeNode;
                    LoadSelectedUserView();
                    maintainCurrentView = false;
                    ShowSelectedUserView();

                    // now we need to find the instance in the tree and select that node
                    TreeNode userViewTreeNode = null;
                    if (instanceTreeNodeLookupTable.TryGetValue(instanceId, out userViewTreeNode))
                        userViewTreeView.SelectedNode = userViewTreeNode;
                    else
                        userViewTreeView.SelectedNode = userViewTreeView.Nodes[0];

                }
                finally
                {
                    userViewTreeView.EndUpdate();
                    userViewsTreeView.EndUpdate();
                    maintainCurrentView = false;
                }
            }
        }


        #region Group View Tree Events

        private void userViewsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            UserView userView = e.Node.Tag as UserView;

            if (userView != null)
            {
                if (e.Label != null)
                {
                    string newViewName = e.Label.Trim();

                    if (newViewName.Length != 0)
                    {
                        UserView existingView;

                        if (string.Compare(newViewName, AllServersUserViewName, true) == 0 ||
                            Settings.Default.ActiveRepositoryConnection.UserViews.TryGetValue(newViewName,
                                                                                              out existingView))
                        {
                            e.Node.Text = GetFormattedUserViewName(userView);
                            ApplicationMessageBox.ShowInfo(ParentForm,
                                                           "A view with the same name already exists. Please specify a unique name.");
                        }
                        else
                        {
                            userView.Name = newViewName;
                        }
                    }
                }
                else
                {
                    e.Node.Text = GetFormattedUserViewName(userView);
                }
            }

            userViewsTreeView.LabelEdit = false;
            e.CancelEdit = true;
        }

        private void userViewsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (userViewsTreeView.SelectedNode != e.Node)
                {
                    treeViewReselectNode = userViewsTreeView.SelectedNode;
                }
            }
            else
            {
                if (e.Node.Level == 0 && ApplicationController.Default.ActiveView is SQLdmTodayView)
                {
                    ApplicationController.Default.ShowActiveServersView();
                    LoadSelectedUserView();
                    ShowSelectedUserView();
                }
            }
        }

        private void userViewsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                tagsTreeView.SelectedNode = null;
                LoadSelectedUserView();
                ShowSelectedUserView();
            }
        }

        private void ShowSelectedUserView()
        {
            if (inActiveViewChanged)
            {
                userViewsSelectedNode = userViewsTreeView.SelectedNode;
                return;
            }

            if (!maintainCurrentView)
            {
                if (userViewsTreeView.SelectedNode.Tag == AllServersUserView.Instance)
                {
                    ApplicationModel.Default.FocusObject = AllServersUserView.Instance;
                    ApplicationController.Default.ShowActiveServersView();
                }
                else if (userViewsTreeView.SelectedNode.Tag is UserView)
                {
                    UserView userView = (UserView)userViewsTreeView.SelectedNode.Tag;
                    ApplicationModel.Default.FocusObject = userView;
                    ApplicationController.Default.ShowUserView(userView.Id);
                }
            }

            userViewsSelectedNode = userViewsTreeView.SelectedNode;
        }

        private void userViewsTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            UserView userView = e.Node.Tag as UserView;

            if (userView == AllServersUserView.Instance || userView is SearchUserView)
            {
                e.CancelEdit = true;
            }
        }

        private void userViewsTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                userViewsTreeView.SelectedNode = userViewsTreeView.GetNodeAt(e.X, e.Y);

                if (userViewsTreeView.SelectedNode != null)
                {
                    toolbarsManager.Tools["refreshButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["addUserViewButton"].SharedProps.Visible = true;

                    if (userViewsTreeView.SelectedNode.Tag == AllServersUserView.Instance)
                    {
                        toolbarsManager.Tools["addServersButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["moveUserViewUpInListButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["moveUserViewDownInListButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["deleteButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["manageServersButton"].SharedProps.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                        toolbarsManager.Tools["viewAllServerProperties"].SharedProps.Visible = true;
                        toolbarsManager.Tools[SnoozeAllalertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools[ResumeAllAlertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["moveUserViewUpInListButton"].SharedProps.Enabled = false;
                        toolbarsManager.Tools["moveUserViewDownInListButton"].SharedProps.Enabled = false;
                        toolbarsManager.SetContextMenuUltra(this, "userViewPopupMenu");
                    }
                    else if (userViewsTreeView.SelectedNode.Tag is UserView)
                    {
                        UserView userView = userViewsTreeView.SelectedNode.Tag as UserView;
                        toolbarsManager.Tools["deleteButton"].SharedProps.Caption = "Delete \"" + userView.Name + "\"";
                        toolbarsManager.Tools["renameUserViewButton"].SharedProps.Caption = "Rename \"" + userView.Name +
                                                                                            "\"...";
                        toolbarsManager.Tools["manageServersButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["viewAllServerProperties"].SharedProps.Visible = false;
                        toolbarsManager.Tools[SnoozeAllalertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools[ResumeAllAlertsServerButtonKey].SharedProps.Visible = true;
                        toolbarsManager.Tools["moveUserViewUpInListButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["moveUserViewDownInListButton"].SharedProps.Visible = true;
                        toolbarsManager.Tools["moveUserViewUpInListButton"].SharedProps.Enabled =
                            userViewsTreeView.SelectedNode.Index > 1;
                        toolbarsManager.Tools["moveUserViewDownInListButton"].SharedProps.Enabled =
                            userViewsTreeView.SelectedNode.Index != userViewsTreeView.Nodes.Count - 1;

                        if (userView is StaticUserView)
                        {
                            toolbarsManager.Tools["addServersButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["deleteButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = true;
                            toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = true;
                        }
                        else if (userView is SearchUserView)
                        {
                            toolbarsManager.Tools["addServersButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["deleteButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["renameUserViewButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = false;
                        }

                        toolbarsManager.SetContextMenuUltra(this, "userViewPopupMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(this, null);
                    }
                }
            }
            else
            {
                TreeNode clickedNode = userViewsTreeView.GetNodeAt(e.X, e.Y);

                if (clickedNode != null && userViewsSelectedNode == clickedNode)
                {
                    if (!clickedNode.IsEditing)
                    {
                        BeginInvoke(new MethodInvoker(BeginEditingSelectedUserView));
                    }
                }
            }
        }

        private void userViewsTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowSelectedObjectProperties();
            }
        }

        #endregion

        private void ReselectNode(object arg)
        {
            // reselect either the arg if it is a tree node or select the saved node if it isn't null
            TreeNode treeNode = arg is TreeNode ? (TreeNode)arg : treeViewReselectNode;
            if (treeNode != null)
            {
                treeNode.TreeView.SelectedNode = treeNode;
            }

            treeViewReselectNode = null;
        }

        public void ShowViewForSelectedNode()
        {
            ShowViewForNode(userViewTreeView.SelectedNode, false, currentServerSubTab);
        }

        public void ShowViewForSelectedNode(bool properties)
        {
            ShowViewForNode(userViewTreeView.SelectedNode, properties);
        }

        private void ShowViewForNode(TreeNode node)
        {
            ShowViewForNode(node, false);
        }

        private void ShowViewForNode(TreeNode node, bool properties, ServerViews serverSubView = ServerViews.Overview)
        {
            try
            {
                inShowView = true;
                if (node == null)
                {
                    ApplicationController.Default.ShowActiveServersView(properties);
                    return;
                }
                else if (node.Level == 0)
                {
                    if (node.Tag == AllServersUserView.Instance)
                    {
                        ApplicationModel.Default.FocusObject = AllServersUserView.Instance;
                        ApplicationController.Default.ShowActiveServersView(properties);
                    }
                    else if (node.Tag is UserView)
                    {
                        UserView userView = (UserView)node.Tag;
                        ApplicationModel.Default.FocusObject = userView;
                        ApplicationController.Default.ShowUserView(userView.Id);
                    }
                    else if (node.Tag is Tag)
                    {
                        Tag tag = (Tag)node.Tag;
                        ApplicationModel.Default.FocusObject = tag;
                        ApplicationController.Default.ShowTagView(tag.Id);
                    }
                }
                // expected that all other nodes are TypedTreeNode objects
                if (!(node is TypedTreeNode))
                    return;

                TreeNode instanceNode = GetAncestor((TypedTreeNode)node, TreeNodeType.Instance);
                // everything should be or have an instance in its path
                if (instanceNode == null)
                    return;

                ApplicationModel.Default.SelectedInstanceId = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                Settings.Default.CurrentRepoId = Settings.Default.RepoId;
                Settings.Default.ActiveRepositoryConnection.RefreshRepositoryInfo();
                object argument = null;

                if (ApplicationModel.Default.ActiveInstances.Contains(ApplicationModel.Default.SelectedInstanceId))
                {
                    switch (((TypedTreeNode)node).NodeType)
                    {
                        case TreeNodeType.Instance:
                            ApplicationController.Default.ShowServerView(ApplicationModel.Default.SelectedInstanceId, ApplicationController.Default.GetCurrentServerViewForServer(ApplicationModel.Default.SelectedInstanceId));
                            // ApplicationController.Default.ShowServerView((int)instanceNode.Tag, ApplicationController.Default.GetCurrentServerViewForServer(ApplicationModel.Default.SelectedInstanceId));

                            return;
                        case TreeNodeType.LogsServer:
                            argument = LogsView.LogTreeRoot.SQLServer;
                            break;
                        case TreeNodeType.LogsAgent:
                            argument = LogsView.LogTreeRoot.Agent;
                            break;
                        case TreeNodeType.Database:
                            argument = new MonitoredObjectName(instanceNode.Text, node.Text);
                            break;
                        case TreeNodeType.DatabaseBackupRestore:
                        case TreeNodeType.DatabaseConfiguration:
                        case TreeNodeType.DatabaseFiles:
                        case TreeNodeType.Mirroring:
                        case TreeNodeType.SystemTables:
                        case TreeNodeType.Tables:
                            // fish out the database as the argument
                            TypedTreeNode dbNode = GetAncestor((TypedTreeNode)node, TreeNodeType.Database);
                            argument = new MonitoredObjectName(instanceNode.Text, dbNode.Text);
                            break;
                        case TreeNodeType.Table:
                            dbNode = GetAncestor((TypedTreeNode)node, TreeNodeType.Database);
                            argument = new MonitoredObjectName(instanceNode.Text, dbNode.Text, node.Text);
                            break;
                        case TreeNodeType.CustomCounters:
                            argument = ServerDetailsView.Selection.Custom;
                            break;
                    }
                }
                ServerViews view = (ServerViews)node.Tag;
                //  ApplicationController.Default.ShowServerView((int)instanceNode.Tag, view, argument);
                ApplicationController.Default.ShowServerView(ApplicationModel.Default.SelectedInstanceId, view, argument);
            }
            finally
            {
                inShowView = false;
            }
        }

        private void UpdateUserViewTreeViewToolTip(TreeNode mouseOverNode)
        {
            bool toolTipEnabled = false;
            string toolTipText = "";
            string toolTipHeading = "";
            Image toolTipHeadingImage = null;

            if (mouseOverNode != null && mouseOverNode.Level > 1)
            {
                TypedTreeNode instanceNode = null;
                // if (mouseOverNode.NextNode != null)
                {
                    instanceNode = GetAncestor((TypedTreeNode)mouseOverNode, TreeNodeType.Instance);
                }

                if (instanceNode != null)
                {
                    var Id = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                    switch (((TypedTreeNode)mouseOverNode).NodeType)
                    {
                        case TreeNodeType.Instance:
                            if (status != null)
                            {
                                toolTipEnabled = true;
                                if (status.IsSupportedVersion)
                                    toolTipText = status.ToolTip;
                                toolTipHeading = status.ToolTipHeading;
                                toolTipHeadingImage = status.ToolTipHeadingImage;
                            }
                            break;
                        case TreeNodeType.Sessions:
                        case TreeNodeType.Queries:
                        case TreeNodeType.QueriesSignatureMode:
                        case TreeNodeType.QueriesStatementMode:
                        case TreeNodeType.QueriesHistoryMode:
                        case TreeNodeType.Resources:
                        case TreeNodeType.Databases:
                        case TreeNodeType.Services:
                        case TreeNodeType.Logs:
                        case TreeNodeType.CustomCounters:
                            if (mouseOverNode is TypedTreeNode)
                            {
                                if (mouseOverNode is IToolTipProvider)
                                {
                                    UltraToolTipInfo tooltip = ((IToolTipProvider)mouseOverNode).GetToolTipInfo();
                                    if (tooltip != null)
                                    {
                                        userViewTreeViewToolTip.Enabled = DefaultableBoolean.True;
                                        toolTipManager.SetUltraToolTip(userViewTreeView, tooltip);
                                        return;
                                    }
                                }
                            }
                            if (status != null)
                            {
                                MetricCategory category = MetricCategory.All;
                                switch (((TypedTreeNode)mouseOverNode).NodeType)
                                {
                                    case TreeNodeType.Sessions:
                                        category = MetricCategory.Sessions;
                                        break;
                                    case TreeNodeType.Queries:
                                        category = MetricCategory.Queries;
                                        break;
                                    case TreeNodeType.Resources:
                                        category = MetricCategory.Resources;
                                        break;
                                    case TreeNodeType.Databases:
                                        category = MetricCategory.Databases;
                                        break;
                                    case TreeNodeType.Services:
                                        category = MetricCategory.Services;
                                        break;
                                    case TreeNodeType.Logs:
                                        category = MetricCategory.Logs;
                                        break;
                                    case TreeNodeType.CustomCounters:
                                        category = MetricCategory.Custom;
                                        break;
                                }
                                if (category != MetricCategory.All)
                                {
                                    ICollection<Issue> issues = status[category];
                                    if (issues != null && issues.Count > 0)
                                    {
                                        Issue[] issueArray = Algorithms.ToArray(issues);
                                        toolTipText = status.GetToolTip(category);
                                        toolTipHeading = String.Format("{0} are {1}", category, issueArray[0].Severity);
                                        toolTipHeadingImage =
                                            MonitoredSqlServerStatus.GetSeverityImage(issueArray[0].Severity);
                                        toolTipEnabled = true;
                                    }
                                }
                            }
                            break;
                        case TreeNodeType.Database:
                            if (status != null)
                            {
                                DatabaseStatus dbStatus;
                                if (status.DatabaseMap.TryGetValue(mouseOverNode.Text, out dbStatus))
                                {
                                    if (dbStatus.IssueCount > 0)
                                    {
                                        MonitoredState severity = dbStatus.Issues[0].Severity;
                                        toolTipText = FormatToolTipText(dbStatus.Issues, dbStatus.IssueCount);
                                        toolTipHeading = String.Format("{0} is {1}", dbStatus.DatabaseName, severity);
                                        toolTipHeadingImage = MonitoredSqlServerStatus.GetSeverityImage(severity);
                                        toolTipEnabled = true;
                                    }
                                }
                            }
                            break;
                        case TreeNodeType.Table:
                            if (status != null)
                            {
                                DatabaseStatus dbStatus;
                                TypedTreeNode dbNode = GetAncestor((TypedTreeNode)mouseOverNode, TreeNodeType.Database);
                                if (status.DatabaseMap.TryGetValue(dbNode.Text, out dbStatus))
                                {
                                    TableStatus tableStatus;
                                    if (dbStatus.TableStatus.TryGetValue(mouseOverNode.Text, out tableStatus))
                                    {
                                        if (tableStatus.IssueCount > 0)
                                        {
                                            MonitoredState severity = tableStatus.Issues[0].Severity;
                                            toolTipText = FormatToolTipText(tableStatus.Issues, tableStatus.IssueCount);
                                            toolTipHeading = String.Format("{0} is {1}", tableStatus.TableName, severity);
                                            toolTipHeadingImage = MonitoredSqlServerStatus.GetSeverityImage(severity);
                                            toolTipEnabled = true;
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            toolTipEnabled = false;
                            break;
                    }
                }
            }

            if (toolTipEnabled)
            {
                userViewTreeViewToolTip.ToolTipText = toolTipText;
                userViewTreeViewToolTip.ToolTipTitle = toolTipHeading;
                userViewTreeViewToolTip.ToolTipTitleAppearance.Image = toolTipHeadingImage;
                userViewTreeViewToolTip.Enabled = DefaultableBoolean.True;
                toolTipManager.SetUltraToolTip(userViewTreeView, userViewTreeViewToolTip);
            }
            else
            {
                userViewTreeViewToolTip.Enabled = DefaultableBoolean.False;
                toolTipManager.HideToolTip();
            }
        }

        public string FormatToolTipText(Issue[] issues, int count)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                if (buffer.Length > 0)
                    buffer.Append("\n");

                buffer.AppendFormat("{0}. {1}",
                    i + 1,
                    issues[i].Subject
                    );
            }
            return buffer.ToString();
        }

        #region Server Tree Events

        private void userViewTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (string.Compare(e.Node.Text, "System Databases") == 0)
            {
                e.Cancel = true;
            }
        }

        private void userViewTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // if the reason for the selection is anything but unknown switch to the view for the node
            if (e.Action != TreeViewAction.Unknown)
                ShowViewForNode(e.Node);
        }

        private void userViewTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // if the collapsing node is an ancestor of the currently selected node
            // show the view for the collapsing node
            TreeNode node = userViewTreeView.SelectedNode;
            if (node != null && IsAncestorOf(e.Node, node))
                ShowViewForNode(e.Node);
        }

        private bool IsAncestorOf(TreeNode ancestor, TreeNode node)
        {
            for (; ancestor.Level < node.Level; node = node.Parent)
            {
                if (node.Parent == ancestor)
                    return true;
            }
            return false;
        }

        private void userViewTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == 1 && e.Node.Tag is Int32)
            {
                // don't allow an instance node to be expanded if it is offline
                var Id = RepositoryHelper.GetSelectedInstanceId((int)e.Node.Tag);
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                if (status != null)
                {
                    if (status.IsSupportedVersion)
                    {
                        foreach (TreeNode node in e.Node.Nodes)
                            UpdateInstanceNode(node, status);
                    }
                    else
                        e.Cancel = true;
                }
            }
            else
            {
                if (e.Node is LazyTreeNode)
                {
                    ((LazyTreeNode)e.Node).ExpandNode();
                }
            }
        }

        private void userViewTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            // set the context menus
            TreeNode selectedNode = userViewTreeView.GetNodeAt(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                SetUserViewTreeViewContextMenu(selectedNode);
            }
            else
            {
                TypedTreeNode typedTreeNode = selectedNode as TypedTreeNode;
                if (typedTreeNode != null)
                {
                    if (typedTreeNode.NodeType == TreeNodeType.CustomCounters)
                    {
                        // get the active view and check the filter
                        var serverView = ApplicationController.Default.ActiveView as ServerViewContainer;
                        if (serverView != null)
                        {
                            ServerDetailsView detailsView = serverView.ActiveView as ServerDetailsView;
                            if (detailsView != null && detailsView.Filter != ServerDetailsView.Selection.Custom)
                                detailsView.Filter = ServerDetailsView.Selection.Custom;
                        }
                    }
                    else if (typedTreeNode.NodeType == TreeNodeType.QueriesSignatureMode)
                    {
                        // get the active view and check the filter
                        var serverView = ApplicationController.Default.ActiveView as ServerViewContainer;
                        if (serverView != null)
                        {
                            QueryMonitorView detailsView = serverView.ActiveView as QueryMonitorView;
                            if (detailsView != null && detailsView.ViewMode != QueryMonitorView.QueryMonitorViewMode.Signature)
                                detailsView.ShowSignatureMode();
                        }
                    }
                    else if (typedTreeNode.NodeType == TreeNodeType.QueriesStatementMode)
                    {
                        // get the active view and check the filter
                        var serverView = ApplicationController.Default.ActiveView as ServerViewContainer;
                        if (serverView != null)
                        {
                            QueryMonitorView detailsView = serverView.ActiveView as QueryMonitorView;
                            if (detailsView != null && detailsView.ViewMode != QueryMonitorView.QueryMonitorViewMode.Statement)
                                detailsView.ShowStatementMode();
                        }
                    }
                    else if (typedTreeNode.NodeType == TreeNodeType.QueriesHistoryMode)
                    {
                        // get the active view and check the filter
                        var serverView = ApplicationController.Default.ActiveView as ServerViewContainer;
                        if (serverView != null)
                        {
                            QueryMonitorView detailsView = serverView.ActiveView as QueryMonitorView;
                            if (detailsView != null && detailsView.ViewMode != QueryMonitorView.QueryMonitorViewMode.History)
                                detailsView.ShowHistoryMode();
                        }
                    }
                }
            }
        }

        private void userViewTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode mouseOverNode = userViewTreeView.GetNodeAt(e.Location);
            if (userViewTreeViewToolTipNode != mouseOverNode)
            {
                // hide the currently showing tooltip
                toolTipManager.HideToolTip();
                UpdateUserViewTreeViewToolTip(mouseOverNode);
            }
            userViewTreeViewToolTipNode = mouseOverNode;
        }

        private void userViewTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // right clicked on a different node
                if (userViewTreeView.SelectedNode != e.Node)
                {
                    // level 0 and 1 nodes are the only ones right now with context menus
                    // save the node that is currently selected and select the node that was clicked
                    if (e.Node.Level <= 1 || e.Node is LazyTreeNode)
                    {
                        treeViewReselectNode = userViewTreeView.SelectedNode;
                        userViewTreeView.SelectedNode = e.Node;
                    }
                }
            }
            else if (e.Node.Level == 0)
            {
                ShowViewForNode(e.Node);
            }
        }


        #endregion

        private static TypedTreeNode GetAncestor(TypedTreeNode startingNode, TreeNodeType nodeType)
        {
            for (TreeNode node = startingNode; node != null; node = node.Parent)
            {
                if (node is TypedTreeNode)
                {
                    if (((TypedTreeNode)node).NodeType == nodeType)
                        return (TypedTreeNode)node;
                }
            }
            return null;
        }

        private void databasesNode_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
                System.Threading.Thread.CurrentThread.Name = "databasesNode_DoWork";

            using (Log.VerboseCall("databasesNode_DoWork"))
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                LazyTreeNode databasesNode = e.Argument as LazyTreeNode;
                List<TreeNode> nodes = new List<TreeNode>();

                bool systems = databasesNode.NodeType == TreeNodeType.SystemDatabases;

                if (!systems)
                {
                    LazyTreeNode dbSystemNode =
                        new LazyTreeNode(TreeNodeType.SystemDatabases, "System Databases", 16,
                                         ServerViews.DatabasesSummary);
                    dbSystemNode.DoWork = databasesNode_DoWork;
                    dbSystemNode.RunWorkerCompleted = databasesNode_RunWorkerCompleted;
                    nodes.Add(dbSystemNode);
                }

                TreeNode instanceNode = GetAncestor(databasesNode, TreeNodeType.Instance);
                int instanceId = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                Settings.Default.CurrentRepoId = Settings.Default.RepoId;
                Settings.Default.ActiveRepositoryConnection.RefreshRepositoryInfo();
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                IDictionary<string, bool> databases = managementService.GetDatabases(instanceId, systems, !systems);
                foreach (string database in Algorithms.Sort<string>(databases.Keys))
                {
                    if (databases[database] != systems)
                        continue;

                    ServerViews viewNavigation = ServerViews.DatabasesSummary;
                    if (database == "tempdb")
                    {
                        MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId, Settings.Default.RepoId);
                        if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major > 8)
                        {
                            viewNavigation = ServerViews.DatabasesTempdbView;
                        }
                    }
                    LazyTreeNode databaseNode =
                        new LazyTreeNode(TreeNodeType.Database, database, 14, viewNavigation);
                    databaseNode.DoWork = databaseNode_DoWork;
                    databaseNode.RunWorkerCompleted = databaseNode_RunWorkerCompleted;

                    nodes.Add(databaseNode);
                }

                e.Result = new Pair<LazyTreeNode, List<TreeNode>>(databasesNode, nodes);
            }
        }

        private void databasesNode_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            using (Log.VerboseCall("databasesNode_RunWorkerCompleted"))
            {
                Pair<LazyTreeNode, List<TreeNode>> result = (Pair<LazyTreeNode, List<TreeNode>>)e.Result;
                TreeNode parentNode = result.First;
                if (parentNode.Nodes.Count == 0)
                {
                    Log.VerboseFormat("Adding {0} child nodes to databases node", parentNode.Nodes.Count);
                    result.First.Nodes.AddRange(result.Second.ToArray());
                }
                else
                {
                    MergeTreeNodes(parentNode, result.Second);
                }

                TreeNode instanceNode = GetAncestor(result.First, TreeNodeType.Instance);
                var Id = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                if (status != null)
                {
                    Log.Verbose("Updating child node status");
                    foreach (TreeNode node in result.Second)
                        UpdateInstanceNode(node, status);
                }
            }
        }

        private void databaseNode_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "databaseNode_DoWork";
            using (Log.VerboseCall("databaseNode_DoWork"))
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                LazyTreeNode databaseNode = e.Argument as LazyTreeNode;

                List<TreeNode> nodes = new List<TreeNode>();

                TreeNode dbConfigNode =
                    new TypedTreeNode(TreeNodeType.DatabaseConfiguration, "Configuration", 40,
                                      ServerViews.DatabasesConfiguration);
                TreeNode dbBackupNode =
                    new TypedTreeNode(TreeNodeType.DatabaseBackupRestore, "Backups & Restores", 41,
                                      ServerViews.DatabasesBackupRestoreHistory);
                TreeNode dbFilesNode =
                    new TypedTreeNode(TreeNodeType.DatabaseFiles, "Files", 42, ServerViews.DatabasesFiles);
                TreeNode tablesNode =
                    new TypedTreeNode(TreeNodeType.Tables, "Tables & Indexes", 33, ServerViews.DatabasesTablesIndexes);


                TreeNode instanceNode = GetAncestor(databaseNode, TreeNodeType.Instance);

                int instanceId = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                Settings.Default.CurrentRepoId = Settings.Default.RepoId;
                Settings.Default.ActiveRepositoryConnection.RefreshRepositoryInfo();
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
                {
                    nodes.AddRange(new TreeNode[]
                                      {
                                       dbConfigNode
                                      });
                }
                else
                {
                    nodes.AddRange(new TreeNode[]
                                      {
                                       dbConfigNode,
                                       dbFilesNode,
                                       dbBackupNode,
                                       tablesNode
                                      });
                }
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId, Settings.Default.CurrentRepoId);
                if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major > 8)
                {
                    TreeNode mirrorNode =
                        new TypedTreeNode(TreeNodeType.Mirroring, "Mirroring", 64, ServerViews.DatabasesMirroring);
                    nodes.Add(mirrorNode);
                }

                e.Result = new Pair<LazyTreeNode, List<TreeNode>>(databaseNode, nodes);
            }
        }

        private void databaseNode_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            using (Log.VerboseCall("databaseNode_RunWorkerCompleted"))
            {
                Pair<LazyTreeNode, List<TreeNode>> result = (Pair<LazyTreeNode, List<TreeNode>>)e.Result;
                TreeNode parentNode = result.First;
                try
                {
                    userViewTreeView.BeginUpdate();

                    if (parentNode.Nodes.Count == 0)
                    {
                        result.First.Nodes.AddRange(result.Second.ToArray());
                    }
                    else
                    {
                        MergeTreeNodes(parentNode, result.Second);
                    }
                    TreeNode instanceNode = GetAncestor(result.First, TreeNodeType.Instance);
                    int Id = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
                    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                    if (status != null)
                    {
                        foreach (TreeNode node in parentNode.Nodes)
                            UpdateInstanceNode(node, status);
                    }
                }
                finally
                {
                    userViewTreeView.EndUpdate();
                }
            }
        }

        private void tablesNode_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "tablesNode_DoWork";
            BackgroundWorker worker = sender as BackgroundWorker;
            LazyTreeNode tablesNode = e.Argument as LazyTreeNode;

            List<TreeNode> tables = new List<TreeNode>();

            TypedTreeNode databaseNode = GetAncestor(tablesNode, TreeNodeType.Database);
            bool system = tablesNode.NodeType == TreeNodeType.SystemTables;

            TreeNode instanceNode = GetAncestor(databaseNode, TreeNodeType.Instance);
            int instanceId = (int)instanceNode.Tag;
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (!system)
            {
                LazyTreeNode systemTablesNode = new LazyTreeNode(TreeNodeType.SystemTables, "System Tables", 16, ServerViews.DatabasesTablesIndexes);
                systemTablesNode.DoWork = tablesNode_DoWork;
                systemTablesNode.RunWorkerCompleted = tablesNode_RunWorkerCompleted;
                tables.Add(systemTablesNode);
            }

            List<Triple<string, string, bool>> triples = managementService.GetTables(instanceId, databaseNode.Text, system, !system);
            List<string> strings = new List<string>();
            foreach (Triple<string, string, bool> triple in triples)
            {
                strings.Add(triple.First + "." + triple.Second);
            }
            foreach (string table in Algorithms.Sort<string>(strings))
            {
                TypedTreeNode tableNode = new TypedTreeNode(TreeNodeType.Table, table, 15, ServerViews.DatabasesTablesIndexes);
                tables.Add(tableNode);
            }

            e.Result = new Pair<LazyTreeNode, List<TreeNode>>(tablesNode, tables);
        }

        private void tablesNode_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Pair<LazyTreeNode, List<TreeNode>> result = (Pair<LazyTreeNode, List<TreeNode>>)e.Result;
            TreeNode parentNode = result.First;
            if (parentNode.Nodes.Count == 0)
            {
                result.First.Nodes.AddRange(result.Second.ToArray());
            }
            else
            {
                MergeTreeNodes(parentNode, result.Second);
            }

            TreeNode instanceNode = GetAncestor(result.First, TreeNodeType.Instance);
            int Id = RepositoryHelper.GetSelectedInstanceId((int)instanceNode.Tag);
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
            if (status != null)
            {
                foreach (TreeNode node in parentNode.Nodes)
                    UpdateInstanceNode(node, status);
            }
        }

        private void MergeTreeNodes(TreeNode parentNode, List<TreeNode> newNodes)
        {
            using (Log.VerboseCall("MergeTreeNodes"))
            {
                List<TreeNode> existingChildNodes = new List<TreeNode>();
                foreach (TreeNode node in parentNode.Nodes)
                    existingChildNodes.Add(node);

                IEnumerator<TreeNode> existing = existingChildNodes.GetEnumerator();
                IEnumerator<TreeNode> merging = newNodes.GetEnumerator();

                TreeNode existingNode = null;
                TreeNode newNode = null;

                if (existing.MoveNext())
                    existingNode = existing.Current;

                if (merging.MoveNext())
                    newNode = merging.Current;

                int x = 0;
                while (existingNode != null && newNode != null)
                {
                    switch (existingNode.Text.CompareTo(newNode.Text))
                    {
                        case -1:
                            existingNode.Remove();
                            if (existing.MoveNext())
                                existingNode = existing.Current;
                            else
                                existingNode = null;
                            break;
                        case 1:
                            parentNode.Nodes.Insert(x++, newNode);
                            if (merging.MoveNext())
                                newNode = merging.Current;
                            else
                                newNode = null;
                            break;
                        case 0:
                            x++;
                            if (existingNode is LazyTreeNode)
                                ((LazyTreeNode)existingNode).Reset();
                            if (existing.MoveNext())
                                existingNode = existing.Current;
                            else
                                existingNode = null;
                            if (merging.MoveNext())
                                newNode = merging.Current;
                            else
                                newNode = null;
                            break;
                    }
                }

                // Remove any existing nodes that didn't exist in the new node collection
                while (existingNode != null)
                {
                    existingNode.Remove();

                    if (existing.MoveNext())
                        existingNode = existing.Current;
                    else
                        existingNode = null;
                }

                // Add any new nodes that didn't exist in the existing node collection
                while (newNode != null)
                {
                    parentNode.Nodes.Add(newNode);

                    if (merging.MoveNext())
                        newNode = merging.Current;
                    else
                        newNode = null;
                }
            }
        }

        internal enum TreeNodeType
        {
            Root,
            Instance,
            Queries,
            QueriesSignatureMode,
            QueriesStatementMode,
            QueriesHistoryMode,
            Databases,
            SystemDatabases,
            Database,
            DatabaseConfiguration,
            DatabaseBackupRestore,
            DatabaseFiles,
            SystemTables,
            Tables,
            Table,
            Logs,
            LogsServer,
            LogsAgent,
            Resources,
            ResourcesCPU,
            ResourcesMemory,
            ResourcesDisk,
            ResourcesProcedureCache,
            Services,
            ServicesAgentJobs,
            ServicesFullText,
            ServicesReplicaton,
            Sessions,
            SessionsDetails,
            SessionsLocks,
            SessionsBlocking,
            Analysis,
            //RunAnalysis,
            CustomCounters,
            Mirroring,
            ResourcesWaitStats,
            QueryWaitStatsActive,
            ResourcesFileActivity,
            ResourcesDiskSize      // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding the disk size view in Resources
        }


        internal interface IToolTipProvider
        {
            UltraToolTipInfo GetToolTipInfo();
        }

        internal class TypedTreeNode : TreeNode
        {
            public readonly TreeNodeType NodeType;
            protected string displayText;

            internal TypedTreeNode(TreeNodeType nodeType, string text, int imageIndex, object tag)
                : base(text, imageIndex, imageIndex)
            {
                this.NodeType = nodeType;
                Tag = tag;
                displayText = text;
            }

            public string DisplayText
            {
                get { return (displayText == null) ? Text : displayText; }
            }

            public IEnumerator<TreeNode> GetDepthFirstEnumerator()
            {
                yield return this;
                for (TreeNode child = FirstNode; child != null; child = child.NextNode)
                {
                    if (child is TypedTreeNode)
                    {
                        IEnumerator<TreeNode> childEnumerator = ((TypedTreeNode)child).GetDepthFirstEnumerator();
                        while (childEnumerator.MoveNext())
                            yield return childEnumerator.Current;
                    }
                    else
                        yield return child;
                }
            }

            public IEnumerator<TreeNode> GetBreadthFirstEnumerator()
            {
                Queue<TreeNode> queue = new Queue<TreeNode>();
                queue.Enqueue(this);
                while (queue.Count > 0)
                {
                    TreeNode node = queue.Dequeue();
                    for (TreeNode child = node.FirstNode; child != null; child = child.NextNode)
                    {
                        queue.Enqueue(child);
                    }
                    yield return node;
                }
            }

            public IEnumerator<TreeNode> GetBottomUpEnumerator()
            {
                Stack<TreeNode> queue = new Stack<TreeNode>();

                IEnumerator<TreeNode> enumerator = GetBreadthFirstEnumerator();

                while (enumerator.MoveNext())
                {
                    queue.Push(enumerator.Current);
                }
                while (queue.Count > 0)
                {
                    yield return queue.Pop();
                }
            }
        }

        internal interface StatusNode
        {
            void UpdateStatus(MonitoredSqlServerStatus status);
        }

        internal class ServerTreeNode : TypedTreeNode, StatusNode
        {
            internal ServerTreeNode(string text, int imageIndex, int instanceId)
                : base(TreeNodeType.Instance, text, imageIndex, instanceId)
            {
            }

            public void Refresh()
            {
                int instanceId = (int)Tag;
                int Id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                if (status != null)
                    UpdateStatus(status);
            }

            public void ForceScheduledRefresh()
            {
                int rinstanceId = Convert.ToInt32(Tag);
                int instanceId = RepositoryHelper.GetSelectedInstanceId(rinstanceId);
                Settings.Default.CurrentRepoId = Settings.Default.RepoId;
                Settings.Default.ActiveRepositoryConnection.RefreshRepositoryInfo();
                MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
                if (wrapper != null)
                    wrapper.ForceScheduledRefresh();

                foreach (TreeNode childNode in this.Nodes)
                {
                    if (childNode is LazyTreeNode)
                    {
                        ((LazyTreeNode)childNode).Reset();
                    }
                }
            }

            public void UpdateStatus(MonitoredSqlServerStatus status)
            {
                //using displayInstanceName instead of DisplayText of the current Instance for updating Text(reflected in UI)

                int rinstanceId = Convert.ToInt32(Tag);
                int instanceId = RepositoryHelper.GetSelectedInstanceId(rinstanceId);

                String displayInstanceName = ApplicationModel.Default.GetInstanceDisplayName(instanceId);

                if (status != null)
                { //Set TreeNode Images for Cloud Instances
                    if (ApplicationModel.Default.AllRepoInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllRepoInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllRepoInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId || ApplicationModel.Default.AllRepoInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                    {
                        ImageKey = status.AzureImageKey;
                    }
                    else
                    {
                        ImageKey = status.ServerImageKey;
                    }

                    SelectedImageKey = ImageKey;
                }
                if (IsExpanded)
                {
                    if (status != null && !status.IsSupportedVersion)
                        Collapse();
                }

                if (status != null && status.Instance != null && status.Instance.IsRefreshing)
                {
                    Text = displayInstanceName + Refreshing_Tag;
                }
                else
                    if (status == null || status.Severity == MonitoredState.OK && !status.IsRefreshAvailable)
                {
                    Text = displayInstanceName + Initializing_Tag;
                }
                else
                {
                    if (!Text.Equals(displayText))
                        Text = displayInstanceName;
                }
            }
        }

        internal class LazyTreeNode : TypedTreeNode, IToolTipProvider
        {
            private static readonly Logger Log = Logger.GetLogger("LazyTreeNode");

            private object sync = new object();
            private DoWorkEventHandler doWork;
            private RunWorkerCompletedEventHandler runWorkerCompleted;
            private BackgroundWorker worker;
            private bool expandWhenDone = false;
            protected bool hasErrorInformation = false;
            protected Exception lastException = null;

            internal LazyTreeNode(TreeNodeType nodeType, string text, int imageIndex, object tag)
                : base(nodeType, text, imageIndex, tag)
            {
                // add a dummy node so we get an expansion indicator
                TreeNode dummy = new TreeNode(DummyTag);
                dummy.Tag = DummyTag;
                Nodes.Add(dummy);
            }

            internal void BeginExpanding()
            {
                using (Log.VerboseCall("BeginExpanding " + displayText))
                {
                    Text = displayText + " (expanding...)";
                }
            }

            internal void EndExpanding(bool expand)
            {
                using (Log.VerboseCall("EndExpanding " + displayText))
                {
                    Text = displayText;
                    if (expand)
                    {
                        Log.Verbose("Expanding node to show children");
                        Expand();
                    }
                }
            }

            internal void ExpandNode()
            {
                using (Log.VerboseCall("ExpandNode " + displayText))
                {
                    lock (sync)
                    {
                        if (worker == null)
                        {
                            if (Nodes.Count == 1 && (Nodes[0].Tag is string && (string)Nodes[0].Tag == DummyTag))
                            {
                                Log.Verbose("Removing dummy child node");
                                Nodes.Remove(Nodes[0]);
                                expandWhenDone = true;
                                StartWorker();
                            }
                        }
                    }
                }
            }

            internal void Refresh()
            {
                using (Log.VerboseCall("Refresh " + displayText))
                    lock (sync)
                    {
                        if (worker == null)
                        {
                            expandWhenDone = IsExpanded;
                            if (Nodes.Count > 0)
                            {
                                Log.Verbose("Node {0} has {1} child nodes", displayText, Nodes.Count);
                                if (Nodes[0].Tag is string && (string)Nodes[0].Tag == DummyTag)
                                {
                                    Log.Verbose("Removing dummy child node");
                                    Nodes.Remove(Nodes[0]);
                                }
                            }
                            StartWorker();
                        }
                    }
            }

            internal void Reset()
            {
                using (Log.VerboseCall("Reset " + displayText))
                {
                    if (IsExpanded)
                        Collapse();

                    if (Nodes.Count == 1 && (Nodes[0].Tag is string && (string)Nodes[0].Tag == DummyTag))
                        return;
                    Log.VerboseFormat("Clearing {0} child nodes and adding a dummy", Nodes.Count);
                    Nodes.Clear();
                    TreeNode dummy = new TreeNode(DummyTag);
                    dummy.Tag = DummyTag;
                    Nodes.Add(dummy);
                }
            }

            private void StartWorker()
            {
                using (Log.VerboseCall("StartWorker " + displayText))
                {
                    worker = new BackgroundWorker();
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                    worker.RunWorkerAsync(this);
                }
            }

            public DoWorkEventHandler DoWork
            {
                get
                {
                    lock (sync)
                    {
                        return doWork;
                    }
                }
                set
                {
                    lock (sync)
                    {
                        doWork = value;
                    }
                }
            }

            public RunWorkerCompletedEventHandler RunWorkerCompleted
            {
                get
                {
                    lock (sync)
                    {
                        return runWorkerCompleted;
                    }
                }
                set
                {
                    lock (sync)
                    {
                        runWorkerCompleted = value;
                    }
                }
            }

            private void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
            {
                using (Log.VerboseCall("worker_DoWork " + displayText))
                {
                    TreeView.BeginInvoke(new MethodInvoker(BeginExpanding));
                    DoWorkEventHandler workDelegate = DoWork;
                    if (workDelegate != null)
                        workDelegate(sender, e);
                }
            }

            private void worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs args)
            {
                using (Log.VerboseCall("worker_RunWorkerCompleted " + displayText))
                {
                    try
                    {
                        /*  Commented to revert to prior error handling for dynamically created tree nodes.
                                            lastException = args.Error;
                                            hasErrorInformation = lastException != null;

                                            if (hasErrorInformation)
                                            {
                                                Collapse();
                                                EndExpanding(false);
                                                if (Nodes.Count == 0)
                                                {
                                                    TreeNode dummy = new TreeNode(DummyTag);
                                                    dummy.Tag = DummyTag;
                                                    Nodes.Add(dummy);
                                                }
                                                return;
                                             }
                        */
                        if (args.Error != null)
                            throw args.Error;

                        RunWorkerCompletedEventHandler runCompleteDelegate = RunWorkerCompleted;
                        if (runCompleteDelegate != null)
                            runCompleteDelegate(sender, args);
                        EndExpanding(expandWhenDone);
                    }
                    catch (Exception e)
                    {
                        this.Collapse();
                        EndExpanding(false);

                        string message =
                            String.Format("An error was detected while trying to expand '{0}' in the server tree.",
                                          DisplayText);

                        ApplicationMessageBox.ShowError(Program.MainWindow.GetWinformWindow(),
                                                        message,
                                                        e);

                        if (Nodes.Count == 0)
                        {
                            Log.Verbose("Adding dummy child node");
                            TreeNode dummy = new TreeNode(DummyTag);
                            dummy.Tag = DummyTag;
                            Nodes.Add(dummy);
                        }
                    }
                    finally
                    {
                        lock (sync)
                        {
                            worker.Dispose();
                            worker = null;
                        }
                    }
                }
            }

            #region IToolTipProvider Members

            public UltraToolTipInfo GetToolTipInfo()
            {
                UltraToolTipInfo result = null;
                if (hasErrorInformation)
                {
                    result = new UltraToolTipInfo();
                    result.ToolTipTitle = lastException.Message;
                    StringBuilder builder = new StringBuilder();
                    for (Exception inner = lastException.InnerException; inner != null; inner = inner.InnerException)
                    {
                        builder.Append(inner.Message + "\n");
                    }
                    result.ToolTipText = builder.ToString();
                    result.ToolTipImage = ToolTipImage.Error;
                }
                return result;
            }

            #endregion
        }

        private void toolbarsManager_BeforeShortcutKeyProcessed(object sender, BeforeShortcutKeyProcessedEventArgs e)
        {
            // Because the Delete key is captured by the toolbar manager as a shortcut, we must recognize it
            // in the case that it's being used to delete the text in a label edit.
            if (e.Tool.Key == "deleteButton" && userViewsTreeView.LabelEdit && userViewsSelectedNode != null)
            {
                userViewsSelectedNode.EndEdit(true);
                userViewsSelectedNode.Text = String.Empty;
                userViewsTreeView.LabelEdit = true;
                userViewsSelectedNode.BeginEdit();
            }
        }

        private void manageTagsLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                ManageTags();
            }
        }

        private void AddNewTag()
        {
            TagPropertiesDialog dialog = new TagPropertiesDialog();

            if (dialog.ShowDialog(ParentForm) == DialogResult.OK)
            {
                TreeNode existingNode;

                if (tagTreeNodeLookupTable.TryGetValue(dialog.TagId, out existingNode))
                {
                    userViewsTreeView.SelectedNode = null;
                    tagsTreeView.SelectedNode = existingNode;
                    LoadSelectedTagView();
                    ShowSelectedTagView();
                }
            }
        }

        private void tagsHeaderStrip_MouseClick(object sender, MouseEventArgs e)
        {
            toolbarsManager.SetContextMenuUltra(this, null);

            if (e.Button == MouseButtons.Left)
            {
                ToggleTagsPanel();
            }
        }

        private void ToggleTagsPanel()
        {
            Settings.Default.NavigationPaneTagsPanelExpanded = !Settings.Default.NavigationPaneTagsPanelExpanded;
            UpdateTagsPanel();
        }

        private void UpdateTagsPanel()
        {
            if (Settings.Default.NavigationPaneTagsPanelExpanded)
            {
                toggleTagsPanelButton.Image = ImageHelper.GetBitmapFromSvgByteArray(downArrowSvg);
                tagsTreeView.Visible = true;
                AdjustTagsPanelHeight();
            }
            else
            {
                toggleTagsPanelButton.Image = ImageHelper.GetBitmapFromSvgByteArray(leftArrowSvg);
                tagsTreeView.Visible = false;
                tagsPanel.Height = 33;
            }
        }

        private void AdjustTagsPanelHeight()
        {
            if (tagsTreeView.Nodes.Count != 0)
            {
                manageTagsLabel.Visible = false;
                int headerHeight = tagsHeaderStrip.Height;
                int nodeHeight = 16;
                int nodeCount = tagsTreeView.Nodes.Count <= GroupPanelsMaxCount ? tagsTreeView.Nodes.Count : GroupPanelsMaxCount;
                int padding = 3;
                tagsPanel.Height = nodeHeight * nodeCount + headerHeight + padding;
            }
            else
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                {
                    manageTagsLabel.Text = "< Click here to manage tags >";
                }
                else
                {
                    manageTagsLabel.Text = "< No viewable tags >";
                }

                manageTagsLabel.Visible = true;
                tagsPanel.Height = 58;
            }
        }

        private void tagsTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tagsTreeView.SelectedNode = tagsTreeView.GetNodeAt(e.X, e.Y);
                toolbarsManager.Tools["openButton"].SharedProps.Visible = tagsTreeView.SelectedNode != null;
                toolbarsManager.Tools["addTagButton"].SharedProps.Visible =
                    toolbarsManager.Tools["manageTagsButton"].SharedProps.Visible =
                    ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

                if (tagsTreeView.SelectedNode != null && ((Tag)tagsTreeView.SelectedNode.Tag).IsGlobalTag) //SQLdm 10.1 - Praveen Suhalka - CWF 3 Integration
                {
                    toolbarsManager.Tools["deleteTagButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["tagPropertiesButton"].SharedProps.Visible = false;
                }
                else
                {
                    toolbarsManager.Tools["deleteTagButton"].SharedProps.Visible =
                    toolbarsManager.Tools["tagPropertiesButton"].SharedProps.Visible =
                    ApplicationModel.Default.UserToken.IsSQLdmAdministrator && tagsTreeView.SelectedNode != null;
                }
                toolbarsManager.Tools["snoozeServersAlertsButton"].SharedProps.Visible =
                toolbarsManager.Tools["resumeServersAlertsButton"].SharedProps.Visible =
                    ApplicationModel.Default.UserToken.IsSQLdmAdministrator && tagsTreeView.SelectedNode != null;

                toolbarsManager.SetContextMenuUltra(this, "tagsPopupMenu");
            }
        }

        private void ManageTags()
        {
            ManageTagsDialog dialog = new ManageTagsDialog();
            dialog.ShowDialog(ParentForm);
        }

        private void tagsTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                ShowSelectedTagProperties();
            }
        }

        private void DeleteSelectedTag()
        {
            if (tagsTreeView.SelectedNode != null)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning(ParentForm,
                                                      "Deleting a tag will remove all links to associated SQLdm objects. If custom counters or permissions are associated with servers via tags, custom counters may no longer be collected for tagged servers and permissions may not longer apply. You can use the Manage Tags dialog to view all SQLdm objects associated with this tag. Would like to proceed with deleting this tag?",
                                                      ExceptionMessageBoxButtons.YesNo);

                if (dialogResult != DialogResult.No)
                {
                    try
                    {
                        Tag[] tag = new Tag[] { tagsTreeView.SelectedNode.Tag as Tag };
                        ApplicationModel.Default.RemoveTags(tag);
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(ParentForm,
                                                        "An error occurred while deleting the selected tag.",
                                                        e);
                    }
                }
            }
        }

        private void ShowSelectedTagProperties()
        {
            if (tagsTreeView.SelectedNode != null && ((Tag)tagsTreeView.SelectedNode.Tag).IsGlobalTag == false) //SQldm 10.1 -- Praveen Suhalka -- CWF 3 Integration
            {
                TagPropertiesDialog dialog = new TagPropertiesDialog(tagsTreeView.SelectedNode.Tag as Tag);
                dialog.ShowDialog(ParentForm);
            }
        }

        public bool InstanceHasCustomCounters(MonitoredSqlServer server)
        {
            if (server != null)
            {
                if (server.HasCustomCounters)
                    return true;

                if (server.HasTags)
                {
                    TagCollection tags = ApplicationModel.Default.Tags;
                    foreach (int tagId in server.Tags)
                    {
                        if (tags.Contains(tagId))
                        {
                            Tag tag = tags[tagId];

                            if (tag != null)
                            {
                                if (tag.CustomCounters.Count > 0)
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void tagsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                userViewsTreeView.SelectedNode = null;
                LoadSelectedTagView();
                ShowSelectedTagView();
            }
        }

        private void tagsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (tagsTreeView.SelectedNode != e.Node)
                {
                    treeViewReselectNode = tagsTreeView.SelectedNode;
                }
            }
        }

        private void UpdateTagStatus(int tagId)
        {
            lock (userViewTreeViewLock)
            {
                TreeNode tagNode;

                if (tagTreeNodeLookupTable.TryGetValue(tagId, out tagNode))
                {
                    Tag tag = (Tag)tagNode.Tag;
                    UserViewStatus tagStatus = UserViewStatus.None;

                    foreach (int instanceId in tag.Instances)
                    {
                        if (tagStatus != UserViewStatus.Critical)
                        {
                            int Id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);

                            if (status != null)
                            {
                                if (status.IsInMaintenanceMode)
                                {
                                    if (tagStatus < UserViewStatus.MaintenanceMode)
                                        tagStatus = UserViewStatus.MaintenanceMode;
                                }
                                else
                                {
                                    switch (status.Severity)
                                    {
                                        case MonitoredState.Critical:
                                            tagStatus = UserViewStatus.Critical;
                                            break;
                                        case MonitoredState.Warning:
                                            if (tagStatus < UserViewStatus.Warning)
                                                tagStatus = UserViewStatus.Warning;
                                            break;
                                        case MonitoredState.Informational:
                                        case MonitoredState.OK:
                                            if (tagStatus < UserViewStatus.OK)
                                                tagStatus = UserViewStatus.OK;
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    switch (tagStatus)
                    {
                        case UserViewStatus.Critical:
                            tagNode.ImageKey =
                                tagNode.SelectedImageKey = "TagCritical";
                            break;
                        case UserViewStatus.Warning:
                            tagNode.ImageKey =
                                tagNode.SelectedImageKey = "TagWarning";
                            break;
                        default:
                            tagNode.ImageKey =
                                tagNode.SelectedImageKey = "Tag";
                            break;
                    }

                    if (userViewTreeView.Nodes.Count > 0 && tagNode.Tag == userViewTreeView.Nodes[0].Tag)
                    {
                        userViewTreeView.Nodes[0].ImageKey =
                            userViewTreeView.Nodes[0].SelectedImageKey = tagNode.ImageKey;
                    }
                }
            }
        }

        public void ViewPulseProfile()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    int instanceId = (int)userViewTreeView.SelectedNode.Tag;
                    if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                    {
                        int pulseId;
                        if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].DisplayInstanceName, out pulseId))
                        {
                            ApplicationController.Default.ShowPulseProfileView(pulseId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error showing Newsfeed profile for server", ex);
            }
        }

        public void TogglePulseFollow()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    int instanceId = (int)userViewTreeView.SelectedNode.Tag;
                    if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                    {
                        int pulseId;
                        if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].DisplayInstanceName, out pulseId))
                        {
                            ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
                            bool followed = (button.CaptionResolved.StartsWith("Unfollow"));
                            PulseController.Default.SetFollowing(pulseId, !followed);
                            UpdatePulseFollowButton(pulseId);
                            bool followedNew = button.CaptionResolved.StartsWith("Unfollow");
                            if (followed != followedNew)
                            {
                                if (followedNew)
                                {
                                    ApplicationMessageBox.ShowInfo(ParentForm, string.Format("You are now following server {0} in the Newsfeed.", ApplicationModel.Default.ActiveInstances[instanceId].DisplayInstanceName));
                                }
                                else
                                {
                                    ApplicationMessageBox.ShowInfo(ParentForm, string.Format("You are no longer following server {0} in the Newsfeed.", ApplicationModel.Default.ActiveInstances[instanceId].DisplayInstanceName));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error following server in the Newsfeed.", ex);
            }
        }

        private void UpdatePulseFollowButton(int pulseId)
        {
            ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
            bool following = PulseController.Default.GetFollowing(pulseId);
            if (following)
            {
                button.SharedProps.Caption = "Unfollow in Newsfeed";
            }
            else
            {
                button.SharedProps.Caption = "Follow Updates in Newsfeed";
            }
        }

        private void CreateHierarchyOfTreeView(int instanceId)
        {
            try
            {
                MonitoredSqlServerCollection instanceDetail = null;
                for (int i = 1; i <= ApplicationModel.Default.RepoActiveInstances.Count; i++)
                {
                    if (ApplicationModel.Default.RepoActiveInstances.TryGetValue(i, out instanceDetail))
                    {
                        var instance = instanceDetail.Where(x => x.InstanceId == instanceId).FirstOrDefault();
                        if (instance != null)
                        {
                            bool obj = userViewTreeView.Nodes[0].Nodes.ContainsKey(i.ToString());
                            if (!obj)
                            {
                                UserViewTreeView.Nodes[0].Nodes.Add(i.ToString(), "Syn-Cluster" + i + "");
                            }
                            int nodeSeqId = UserViewTreeView.Nodes[0].Nodes.Count;
                            var id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                            AddInstanceNode(id, userViewTreeView.Nodes[0].Nodes[nodeSeqId - 1]);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
