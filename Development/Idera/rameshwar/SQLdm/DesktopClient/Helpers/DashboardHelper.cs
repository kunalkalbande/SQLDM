using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal class DashboardHelper
    {
        //6.2.4 
        private const string AzureDiskPanelDescription = "Use this panel to track performance of each IO used by your monitored SQL Server instance. Unexpected spikes in latency, throughput, or SQL Server IO, or chronically high disk metrics could be a sign of an insufficient IO subsystem, or excessive or inefficient IO activity on the server.\r\n\r\nAlerts related to the Disk panel include average time for read and write operations for data on the disk, and the number of disk reads and writes per second.";

        private const string CachePanelDescription = "Use this panel to track database and procedure processes using a buffer pool on your monitored SQL Server instance. Chronically high cache metrics may indicate the need for server maintenance, query tuning, or index updates to handle the ongoing workload. Charts offer page life expectancy, cache usage per area, and cache hit ratios for the Buffer and Procedure cache.\r\n\r\nAlerts related to the Cache panel include page life expectancy and the hit ratio for your Procedure Cache.";
        private const string CpuPanelDescription = "Use this panel to track processor performance for your instance. The CPU usage chart displays the  processing power % in use on the computer hosting this instance. Queue length determines the amount of work waiting to be done by this server. Call Rates provides a view of resource-intensive activities. \r\n\r\nRelated alerts include CPU usage for your ESX, SQL Server, and VM instances, and for your OS processor time and queue length.";
        private const string CustomCountersPanelDescription = "Use this panel to track selected custom counters for your instance. Use the selection dialog to choose the custom counters you want to view. You must have custom counters set up to populate the available list. You can create a custom counter by going to Administration > Custom Counters.\r\n\r\nRelated alerts depend on the selected custom counters and alert threshold settings.";
        private const string DatabasesPanelDescription = "Use this panel to track database performance on your monitored SQL Server instance. This panel offers a list of your top 100 databases sorted by current usage of that metric, and a bar representing the relative size of the associated database.\r\n\r\nViews available on the Database panel include per-second metrics of log flushes, transactions, reads, writes, and I/O stall milliseconds.";
        private const string DiskPanelDescription = "Use this panel to track performance of each disk used by your monitored SQL Server instance. Unexpected spikes in latency, throughput, or SQL Server IO, or chronically high disk metrics could be a sign of an insufficient IO subsystem, or excessive or inefficient IO activity on the server.\r\n\r\nAlerts related to the Disk panel include average time for read and write operations for data on the disk, and the number of disk reads and writes per second.";
        private const string EmptyPanelDescription = "This panel is a placeholder used when designing a dashboard and would not normally be used in a dashboard layout.";
        private const string FileActivityPanelDescription = "Use this panel to view the top five database files with the highest relative activity since the last refresh.\r\n\r\nYou can select from the available list of metrics to view a specific activity type, such as Transfers/sec, and select if you want all files or you want results limited to a specific database, disk, or file.";
        private const string LockWaitsPanelDescription = "Use this panel to track the number of waits caused when a task waits to acquire a lock on your monitored SQL Server instance. Chronically high lock waits may indicate the need for server maintenance, query tuning, or index updates to better handle the ongoing workload.\r\n\r\nThe Lock Wait chart displays the number of waits and the request type occurring, including database, extent, key, page, RID, and table locks.";
        private const string MemoryPanelDescription = "Use this panel to track memory usage and availability on your monitored SQL Server instance. Charts display metrics regarding the memory usage for your SQL Server, VM, and host servers. The paging chart provides the number of swapped pages per second over time.\r\n\r\nAlerts related to the Memory panel include memory usage for your ESX, OS, SQL Server, and VMs, plus an operating system paging alert for high OS memory paging, memory usage, or SQL Server usage.";
        private const string NetworkPanelDescription = "Use this panel to track network connection usage for your instance. Chronically high metrics may indicate excessive traffic or server workload, which may require you to free network resources and balance the workload. Charts display metrics regarding the throughput of your SQL Server, VM, and host networks.\r\n\r\nThe Network panel alert triggers on the SQL Server response time needed to send a simple SQL command to the SQL Server instance, have it processed, and receive the returned result set.";
        private const string ServerWaitsPanelDescription = "Use this panel to track the total number of waits and time spent on the waits affecting your monitored SQL Server instance. Chronically high server waits may indicate the need for server maintenance, query tuning, or index updates to handle the ongoing workload.\r\n\r\nThe Server Waits chart displays the overall wait time for key areas on your instance including I/O, locks, log, memory, signal, and other types of waits.";
        private const string SessionsPanelDescription = "Use this panel to track the performance of the active and blocked sessions running on this instance. The Activity & Blocking chart gives you a quick glimpse at the number of types of blocks affecting your instance while the Clients gauge displays the total number of unique client computers on this instance.\r\n\r\nAlerts related to the Sessions panel include deadlocks, the number of sessions blocked by other sessions holding requested locks, and the number of unique client computers connected to this instance.";
        private const string TempDBDescription = "Use this panel to  track the status of the tempdb database on your instance. The provided charts help you analyze how your database is used based on object type and show the current latch wait time for your tempdb allocation pages. The gauge lets you know whether data is filling up your tempdb.\r\n\r\nAlerts related to the Tempdb panel include tempdb contention, the version store generation ratio, and the version store size.";
        private const string VMDescription = "Use this panel to track different types of virtual memory usage and statistics on how your disks are used on your VM and host server. The CPU Ready gauge lets you know how long the vCPU is waiting for a physical CPU in its host to become available.\r\n\r\nAlerts related to the Virtualization panel include memory swap detection, the amount of ready wait time, and the amount of ballooned and reclaimed memory.";
        private const string SQLServerPhysicalIODescription = "Use this panel to track performance of each SQL Server IO used by your monitored SQL Server instance. Unexpected spikes in SQL Server IO, or chronically high insufficient IO subsystem, or excessive or inefficient IO activity on the server.\r\n\r\nAlerts related to the sqlServerReadsWritesChart panel include average time for read and write operations for data on the SQL Server IO.";



        /// <summary>
        /// Get the text for the dashboard details panel for the requested panel
        /// </summary>
        /// <param name="panel">The panel needing detail text</param>
        /// <returns>A string containing details on the panel</returns>
        internal static string GetDashboardDescription(DashboardPanel panel)
        {
            switch (panel)
            {
                //6.2.4
                case DashboardPanel.AzureDisk:
                    return AzureDiskPanelDescription;
                case DashboardPanel.Cache:
                    return CachePanelDescription;
                case DashboardPanel.Cpu:
                    return CpuPanelDescription;
                case DashboardPanel.CustomCounters:
                    return CustomCountersPanelDescription;
                case DashboardPanel.Databases:
                    return DatabasesPanelDescription;
                case DashboardPanel.Disk:
                    return DiskPanelDescription;
                case DashboardPanel.Empty:
                    return EmptyPanelDescription;
                case DashboardPanel.FileActivity:
                    return FileActivityPanelDescription;
                case DashboardPanel.LockWaits:
                    return LockWaitsPanelDescription;
                case DashboardPanel.Memory:
                    return MemoryPanelDescription;
                case DashboardPanel.Network:
                    return NetworkPanelDescription;
                case DashboardPanel.ServerWaits:
                    return ServerWaitsPanelDescription;
                case DashboardPanel.Sessions:
                    return SessionsPanelDescription;
                case DashboardPanel.TempDB:
                    return TempDBDescription;
                case DashboardPanel.VM:
                    return VMDescription;
                case DashboardPanel.SQLServerPhysicalIO:
                    return SQLServerPhysicalIODescription;
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Get the help topic associated with a dashboard panel
        /// </summary>
        /// <param name="panel">The panel requesting help</param>
        /// <returns>The name of the help topic page</returns>
        internal static string GetDashboardHelpTopic(DashboardPanel panel)
        {
            switch (panel)
            {
                //6.2.4
                case DashboardPanel.AzureDisk:
                    return HelpTopics.ServerDashboardViewAzureDiskPanel;
                case DashboardPanel.Cache:
                    return HelpTopics.ServerDashboardViewCachePanel;
                case DashboardPanel.Cpu:
                    return HelpTopics.ServerDashboardViewCpuPanel;
                case DashboardPanel.CustomCounters:
                    return HelpTopics.ServerDashboardViewCustomCountersPanel;
                case DashboardPanel.Databases:
                    return HelpTopics.ServerDashboardViewDatabasesPanel;
                case DashboardPanel.Disk:
                    return HelpTopics.ServerDashboardViewDiskPanel;
                case DashboardPanel.Empty:
                    return HelpTopics.ServerDashboardView;
                case DashboardPanel.FileActivity:
                    return HelpTopics.ServerDashboardViewFileActivityPanel;
                case DashboardPanel.LockWaits:
                    return HelpTopics.ServerDashboardViewLocksPanel;
                case DashboardPanel.Memory:
                    return HelpTopics.ServerDashboardViewMemoryPanel;
                case DashboardPanel.Network:
                    return HelpTopics.ServerDashboardViewNetworkPanel;
                case DashboardPanel.ServerWaits:
                    return HelpTopics.ServerDashboardViewServerWaitsPanel;
                case DashboardPanel.Sessions:
                    return HelpTopics.ServerDashboardViewSessionsPanel;
                case DashboardPanel.TempDB:
                    return HelpTopics.ServerDashboardViewTempDBPanel;
                case DashboardPanel.VM:
                    return HelpTopics.ServerDashboardViewVMPanel;
                case DashboardPanel.SQLServerPhysicalIO:
                    return HelpTopics.ServerDashboardViewDiskPanelPhysicalIo;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Generate a default dashboard configuration compatible with the 7.2 default dashboard
        /// This normally comes from the repository and is a fallback default to make sure one exists
        ///   and is used to generate layouts for the dashboard gallery model at design time
        /// </summary>
        /// <param name="serverVersion">The version of the server instance using the dashboard</param>
        /// <returns>A new DashboardConfiguration object with system default values</returns>
        internal static DashboardConfiguration GetDefaultConfig(ServerVersion serverVersion, int instanceId)
        {


            DashboardConfiguration config = new DashboardConfiguration();
            config.SetSize(2, 4);
            int? cloudID = ApplicationModel.Default.AllInstances[instanceId].CloudProviderId;

            //These panels apply to all instance types.
            config.Panels.Add(new DashboardPanelConfiguration(0, 0, DashboardPanel.Cpu));
            config.Panels.Add(new DashboardPanelConfiguration(0, 1, serverVersion.Major < 9 ? DashboardPanel.LockWaits : DashboardPanel.ServerWaits));
            config.Panels.Add(new DashboardPanelConfiguration(1, 0, DashboardPanel.Memory));
            config.Panels.Add(new DashboardPanelConfiguration(1, 1, DashboardPanel.Cache));
            config.Panels.Add(new DashboardPanelConfiguration(2, 0, DashboardPanel.Sessions));
            config.Panels.Add(new DashboardPanelConfiguration(2, 1, DashboardPanel.Databases));

            switch (cloudID)
            {
                case Common.Constants.MicrosoftAzureId:
                case Common.Constants.MicrosoftAzureManagedInstanceId:
                    {
                        config.Panels.Add(new DashboardPanelConfiguration(3, 0, DashboardPanel.SQLServerPhysicalIO));
                        config.Panels.Add(new DashboardPanelConfiguration(3, 1, DashboardPanel.AzureDisk));
                        break;
                    }
                case Common.Constants.AmazonRDSId:
                    {
                        config.Panels.Add(new DashboardPanelConfiguration(3, 0, DashboardPanel.SQLServerPhysicalIO));
                        break;
                    }
                default:
                    {
                        config.Panels.Add(new DashboardPanelConfiguration(3, 0, DashboardPanel.Network));
                        config.Panels.Add(new DashboardPanelConfiguration(3, 1, DashboardPanel.Disk));
                        break;
                    }
            }
            return config;
        }

        /// <summary>
        /// Create a Dashboard control for the specified panel
        /// </summary>
        /// <param name="panel">The type of control to create</param>
        /// <returns>A new instance of the DashboardControl object</returns>
        internal static DashboardControl GetNewDashboardControl(DashboardPanel panel, ViewContainer vHost)
        {
            switch (panel)
            {
                //6.2.4
                case DashboardPanel.AzureDisk:
                    return new AzureDiskControl();
                case DashboardPanel.Cache:
                    return new CacheControl();
                case DashboardPanel.Cpu:
                    return new CpuControl();
                case DashboardPanel.CustomCounters:
                    return new CustomCounterControl();
                case DashboardPanel.Databases:
                    return new DatabasesControl();
                case DashboardPanel.Disk:
                    return new DiskControl();
                case DashboardPanel.Empty:
                    return new EmptyControl();
                case DashboardPanel.FileActivity:
                    return new FileActivityControl();
                case DashboardPanel.LockWaits:
                    return new LockWaitsControl();
                case DashboardPanel.Memory:
                    return new MemoryControl();
                case DashboardPanel.Network:
                    return new NetworkControl();
                case DashboardPanel.ServerWaits:
                    return new ServerWaitsControl();
                case DashboardPanel.Sessions:
                    return new SessionsControl();
                case DashboardPanel.TempDB:
                    return new TempDBControl();
                case DashboardPanel.VM:
                    return new VirtualizationControl();
                case DashboardPanel.SQLServerPhysicalIO:
                    return new SQLServerPhysicalIO();
                default:
                    return null;
            }
        }

        public static Image GetPanelImage(DashboardPanel panel)
        {
            switch (panel)
            {
                //6.2.4
                case DashboardPanel.AzureDisk:
                    return Properties.Resources.PanelAzureDisk;
                case DashboardPanel.Cache:
                    return Properties.Resources.PanelCache;
                case DashboardPanel.Cpu:
                    return Properties.Resources.PanelCPU;
                case DashboardPanel.CustomCounters:
                    return Properties.Resources.PanelCustom;
                case DashboardPanel.Databases:
                    return Properties.Resources.PanelDatabases;
                case DashboardPanel.Disk:
                    return Properties.Resources.PanelDisk;
                case DashboardPanel.FileActivity:
                    return Properties.Resources.PanelFileActivity;
                case DashboardPanel.LockWaits:
                    return Properties.Resources.PanelLockWaits;
                case DashboardPanel.Memory:
                    return Properties.Resources.PanelMemory;
                case DashboardPanel.Network:
                    return Properties.Resources.PanelNetwork;
                case DashboardPanel.ServerWaits:
                    return Properties.Resources.PanelServerWaits;
                case DashboardPanel.Sessions:
                    return Properties.Resources.PanelSessions;
                case DashboardPanel.TempDB:
                    return Properties.Resources.PanelTempDB;
                case DashboardPanel.VM:
                    return Properties.Resources.PanelVirtual;
                case DashboardPanel.SQLServerPhysicalIO:
                    return Properties.Resources.SQLServerPhysicalIO;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get a list of all dashboard panels available for the user to place on the dashboard
        /// </summary>
        /// <returns>A List of new DashboardPanelSelector objects</returns>
        internal static List<DashboardPanelSelector> GetPanelList()
        {
            int idx = 0;
            List<DashboardPanelSelector> panels = new List<DashboardPanelSelector>();
            foreach (DashboardPanel panel in Enum.GetValues(typeof(DashboardPanel)))
            {
                if (panel != DashboardPanel.Empty)
                    panels.Add(new DashboardPanelSelector(idx++, panel));
            }
            return panels;
        }

        /// <summary>
        /// Determine if a dashboard control supports a specific version of SQL Server
        /// </summary>
        /// <param name="panel">The type of dashboard control to check</param>
        /// <param name="serverVersion">The SQL server version to check</param>
        /// <returns>True if supported, otherwise False</returns>
        internal static bool IsVersionSupported(DashboardPanel panel, ServerVersion serverVersion)
        {
            if (serverVersion == null)
                return true;

            switch (panel)
            {
                case DashboardPanel.ServerWaits:
                case DashboardPanel.TempDB:
                    // these panels require SQL version 2005 or greater
                    return serverVersion.Major > 8 ? true : false;
                default:
                    return true;
            }
        }
    }
}
