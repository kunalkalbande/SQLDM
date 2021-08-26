using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.DesktopClient.Properties;

    [Serializable]
    internal sealed class SearchUserView : UserView, ISerializable
    {
        public static Guid CriticalUserViewID = new Guid("94F7C05C-2B57-4f5f-99FD-4A9C01A47DDC");
        public static Guid WarningUserViewID = new Guid("{54B13511-FB79-46ff-88A4-C59CEE50D514}");
        public static Guid OKUserViewID = new Guid("58655D83-C3F9-4cf8-96D7-7A384A857FC3");
        public static Guid MaintenanceModeUserViewID = new Guid("A66D4FA6-7AA7-42f9-BB61-F7FA369D17D1");

        public delegate bool IsGroupMemberDelegate(MonitoredSqlServer instance, MonitoredSqlServerStatus instanceStatus);

        private readonly List<int> instances = new List<int>();
        private IsGroupMemberDelegate IsGroupMember;
        private UserViewStatus severity;

        public SearchUserView(string name, IsGroupMemberDelegate membershipDelegate, UserViewStatus severity)
        {
            Name = name;
            this.IsGroupMember = membershipDelegate;
            this.severity = severity;
            //ApplicationController.Default.BackgroundRefreshCompleted += new EventHandler<BackgroundRefreshCompleteEventArgs>(BackgroundRefreshCompleted);
        }

        public SearchUserView(string name, IsGroupMemberDelegate membershipDelegate, Guid id, UserViewStatus severity)
            : this(name, membershipDelegate, severity) 
        {
            Id = id;
        }

        public SearchUserView(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            IsGroupMember = (IsGroupMemberDelegate)info.GetValue("IsGroupMember", typeof(IsGroupMemberDelegate));
            severity = (UserViewStatus)info.GetValue("severity", typeof(UserViewStatus));
            //ApplicationController.Default.BackgroundRefreshCompleted += new EventHandler<BackgroundRefreshCompleteEventArgs>(BackgroundRefreshCompleted);
        }

        internal static SearchUserView GetUserView(Guid id)
        {
            SearchUserView result = null;

            RepositoryConnection activeConnection = Settings.Default.ActiveRepositoryConnection;
            if (activeConnection != null)
            {
                result = activeConnection.UserViews.Contains(id) ? activeConnection.UserViews[id] as SearchUserView : null;
            }
            return result;
        }

        internal static SearchUserView CreateView(Guid id)
        {
            if (id == CriticalUserViewID)
                return new SearchUserView("Critical", SearchUserView.IsCritical, id, UserViewStatus.Critical);
            if (id == WarningUserViewID)
                return new SearchUserView("Warning", SearchUserView.IsWarning, id, UserViewStatus.Warning);
            if (id == OKUserViewID)
                return new SearchUserView("OK", SearchUserView.IsOK, id, UserViewStatus.OK);
            if (id == MaintenanceModeUserViewID)
                return new SearchUserView("Maintenance Mode", SearchUserView.IsMaintenanceMode, id, UserViewStatus.MaintenanceMode);
            return null;
        }

        //void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        //{
        //    Update();
        //}

        public override ICollection<int> Instances
        {
            get
            {
                return new ReadOnlyCollection<int>(instances);
            }
        }

        public override UserViewStatus Severity
        {   // if there are instances in the view then return the status else return none
            get
            {
                lock (syncRoot)
                {
                    return instances.Count > 0 ? severity : UserViewStatus.None;
                }
            }
            protected set
            {
                lock (syncRoot)
                {
                    if ((instances.Count > 1 && severity != UserViewStatus.None) ||
                        (instances.Count == 0 && severity == UserViewStatus.None))
                        return;
                }
                OnSeverityChanged();
            }
        }

        /// <summary>
        /// Updates the view based on the current list of actively monitored sql servers.
        /// </summary>
        public override void Update()
        {
            List<int> copy;

            ApplicationModel model = ApplicationModel.Default;
            lock (syncRoot)
            {
                copy = new List<int>(instances);
                foreach (var repoactiveInstances in model.RepoActiveInstances.Values)
                {
                    IDictionary<int, MonitoredSqlServerWrapper> activeInstances = repoactiveInstances.GetDictionary();

                    if (activeInstances != null)
                    {
                        foreach (MonitoredSqlServerWrapper instance in activeInstances.Values)
                        {
                            int instanceId = instance.Id;
                            MonitoredSqlServerStatus status = model.GetInstanceStatus(instanceId, instance.RepoId);
                            if (status == null)
                                continue;

                            if (IsGroupMember(instance, status))
                            {
                                if (!instances.Contains(instance.InstanceId))
                                    AddInstance(instance.InstanceId);
                                else
                                    copy.Remove(instance.InstanceId);
                            }
                        }
                    }
                }
            }

            foreach (int id in copy)
            {
                RemoveInstance(id);
            }
        }

        public void AddInstance(int id)
        {
            if (instances.Contains(id))
            {
                throw new ArgumentException("The instance is already contained in the view.");
            }

            instances.Add(id);
            var instanceId=Helpers.RepositoryHelper.GetSelectedInstanceId(id);
            OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Added, instanceId));
            OnSeverityChanged();
        }

        public void RemoveInstance(int id)
        {
            if (instances.Contains(id))
            {
                instances.Remove(id);
                var instanceId=Helpers.RepositoryHelper.GetSelectedInstanceId(id);
                OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Removed, instanceId));
                OnSeverityChanged();
            }
        }

        public void ClearInstances()
        {
            instances.Clear();
            OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Cleared));
            OnSeverityChanged();
        }

        public static bool IsWarning(MonitoredSqlServer instance, MonitoredSqlServerStatus instanceStatus)
        {
            if (IsMaintenanceMode(instance, instanceStatus))
                return false;
            return (instanceStatus != null && instanceStatus.Severity == MonitoredState.Warning);
        }

        public static bool IsCritical(MonitoredSqlServer instance, MonitoredSqlServerStatus instanceStatus)
        {
            if (IsMaintenanceMode(instance, instanceStatus))
                return false;
            return (instanceStatus != null && instanceStatus.Severity == MonitoredState.Critical);
        }

        public static bool IsOK(MonitoredSqlServer instance, MonitoredSqlServerStatus instanceStatus)
        {
            if (IsMaintenanceMode(instance, instanceStatus))
                return false;
            return (instanceStatus == null || instanceStatus.Severity <= MonitoredState.OK);
        }

        public static bool IsMaintenanceMode(MonitoredSqlServer instance, MonitoredSqlServerStatus instanceStatus)
        {
            if (instanceStatus != null)
                return instanceStatus.IsInMaintenanceMode;

            return (instance != null && instance.MaintenanceModeEnabled);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IsGroupMember", IsGroupMember);
            info.AddValue("severity", severity);
        }
    }
}
