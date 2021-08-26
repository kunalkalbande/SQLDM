using Idera.SQLdm.Common.Auditing;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Xml;
    using BBS.TracerX;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Properties;
    using Idera.SQLdm.Common.Services;

    public class MonitoredSqlServerWrapper : IAuditable
    {
        private static readonly Logger Log = Logger.GetLogger(typeof(MonitoredSqlServerWrapper));
        public static readonly object SyncRoot = new object();

        public event EventHandler<MonitoredSqlServerChangedEventArgs> Changed;
        private MonitoredSqlServer instance;
        private AlertConfiguration alertConfiguration;
        private string machineName;
        private decimal _baseHealthIndex;
        private BackgroundWorker refreshWorker;
        private bool refreshing = false;

        public MonitoredSqlServerWrapper(MonitoredSqlServer instance)
        {
            this.instance = instance;
        }

        public MonitoredSqlServer Instance
        {
            get
            {
                lock (SyncRoot)
                {
                    return instance;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    instance = value;
                }
                FireChanged();
            }
        }

        public int Id
        {
            get
            {
                lock (SyncRoot)
                {
                    return instance.Id;
                }
            }
        }

        public string InstanceName
        {
            get
            {
                lock (SyncRoot)
                {
                    return instance.InstanceName;
                }
            }
        }

        public string FriendlyServerName
        {
            get
            {
                lock (SyncRoot)
                {
                    return instance.FriendlyServerName;
                }
            }
        }

        public string DisplayInstanceName
        {
            get
            {
                lock (SyncRoot)
                {
                    return instance.DisplayInstanceName;
                }
            }
        }

        public string MachineName
        {
            get
            {
                return machineName;
            }
            set
            {
                machineName = value;
            }
        }

        public bool MaintenanceModeEnabled
        {
            get { return instance.MaintenanceModeEnabled; }
            set
            {
                instance.MaintenanceModeEnabled = value;
                FireChanged();
            }
        }

        public decimal BaseHealthIndex
        {
            get { return _baseHealthIndex; }
            set
            {
                _baseHealthIndex = value;
            }
        }

        public AlertConfiguration AlertConfiguration
        {
            get
            {
                if (alertConfiguration == null)
                {
                    alertConfiguration = new AlertConfiguration(Id);
                }
                return alertConfiguration;
            }
        }

        public override string ToString()
        {
            return instance.ToString();
        }

        public void FireChanged()
        {
            MonitoredSqlServerChangedEventArgs args = new MonitoredSqlServerChangedEventArgs(this);

            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                Program.MainWindow.Dispatcher.BeginInvoke(
                    (MethodInvoker)delegate()
                        {
                            if (Changed != null)
                                Changed(this, args);
                        });
            }
            else
            {
                if (Changed != null)
                {
                    Changed(this, args);
                }
            }
        }

        public static implicit operator MonitoredSqlServer(MonitoredSqlServerWrapper value)
        {
            return value.Instance;
        }

        public bool IsRefreshing
        {
            get
            {
                lock (SyncRoot)
                {
                    return refreshing;
                }
            }
        }

        private void SetRefreshing(bool newValue)
        {
            lock (SyncRoot)
            {
                if (refreshing != newValue)
                    refreshing = newValue;
            }

            FireChanged();
        }


        public void ForceScheduledRefresh()
        {
            if (refreshWorker == null)
            {
                refreshWorker = new BackgroundWorker();
                refreshWorker.DoWork += refreshWorker_DoWork;
                refreshWorker.RunWorkerCompleted += refreshWorker_RunWorkerCompleted;
            }
            if (!refreshWorker.IsBusy)
            {
                SetRefreshing(true);
                refreshWorker.RunWorkerAsync();
            }
        }

        void refreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    Log.Warn("Refresh failed: ", e);
                }
                else
                if (e.Cancelled)
                {
                    Log.Info("Refresh cancelled");
                }
            }
            finally
            {
                SetRefreshing(false);
            }
        }


        void refreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            string xml = managementService.ForceScheduledRefresh(Id);

            XmlDocument document = null;
            if (xml != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                document = doc;
            }

            if (document != null && document.DocumentElement != null)
            {
                if (document.DocumentElement.ChildNodes.Count > 0)
                {
                    XmlNode node = document.DocumentElement.FirstChild;
                    MonitoredSqlServerStatus currentStatus = ApplicationModel.Default.GetInstanceStatus(Id);
                    if (currentStatus != null)
                    {
                        currentStatus.Update(node);
                        e.Result = currentStatus;
                    }
                    else
                        e.Result = new MonitoredSqlServerStatus(node);
                }
            }
        }

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            return new AuditableEntity {Name = this.InstanceName};
        }

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return GetAuditableEntity();
        }
    }

    public sealed class MonitoredSqlServerChangedEventArgs : EventArgs
    {
        public readonly MonitoredSqlServerWrapper Instance;

        public MonitoredSqlServerChangedEventArgs(MonitoredSqlServerWrapper instance)
        {
            Instance = instance;
        }
    }

}
