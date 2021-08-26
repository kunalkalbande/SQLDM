using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System.Threading;

    [Serializable]
    internal sealed class StaticUserView : UserView
    {
        private readonly List<int> instances = new List<int>();
        private UserViewStatus severity = UserViewStatus.None;

        public StaticUserView(string name)
        {
            Name = name;
        }

        public StaticUserView(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            List<int> temp = info.GetValue("instances", typeof(List<int>)) as List<int>;
            instances.AddRange(temp);      
        }

        public override ICollection<int> Instances
        {
            get { return new ReadOnlyCollection<int>(instances); }
        }

        public override UserViewStatus Severity
        {   // if there are instances in the view then return the status else return none
            get
            {
                lock (syncRoot)
                {
                    return severity;
                }
            }
            protected set
            {
                UserViewStatus sev = Severity;
                if (value != severity)
                {
                    lock (syncRoot)
                    {
                        if (severity != value)
                            severity = value;
                        else
                            return;
                    }
                    OnSeverityChanged();
                }
            }
        }

        /// <summary>
        /// Updates the static view based on the current list of actively monitored sql servers.
        /// </summary>
        public override void Update()
        {
            List<int> invalidInstances = new List<int>();
            ApplicationModel applicationModel = ApplicationModel.Default;
            
            if(!applicationModel.Initialized)
                return;

            UserViewStatus uvs = UserViewStatus.None;
            MonitoredSqlServerCollection activeInstances = applicationModel.ActiveInstances;

            lock(syncRoot) {
                foreach (int id in instances)
                {
                    // get the instance 
                    lock (activeInstances.SyncRoot)
                    {
                        if (!activeInstances.Contains(id))
                            invalidInstances.Add(id);
                    }

                    if (uvs != UserViewStatus.Critical)
                    {
                        MonitoredSqlServerStatus status = applicationModel.GetInstanceStatus(id);
                        if (status != null)
                        {
                            if (status.IsInMaintenanceMode)
                            {
                                if (uvs < UserViewStatus.MaintenanceMode)
                                    uvs = UserViewStatus.MaintenanceMode;
                            }
                            else
                            {
                                switch (status.Severity)
                                {
                                    case Idera.SQLdm.Common.MonitoredState.Critical:
                                        uvs = UserViewStatus.Critical;
                                        break;
                                    case Idera.SQLdm.Common.MonitoredState.Warning:
                                        if (uvs < UserViewStatus.Warning)
                                            uvs = UserViewStatus.Warning;
                                        break;
                                    case Idera.SQLdm.Common.MonitoredState.OK:
                                        if (uvs < UserViewStatus.OK)
                                            uvs = UserViewStatus.OK;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (int id in invalidInstances)
            {
                RemoveInstance(id);
            }

            Severity = uvs;
        }

        public void AddInstance(int id)
        {
            lock (syncRoot)
            {
                if (instances.Contains(id))
                {
                    throw new ArgumentException("The instance is already contained in the view.");
                }
                instances.Add(id);
            }
            var instanceId=Helpers.RepositoryHelper.GetSelectedInstanceId(id);
            OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Added, instanceId));
        }

        public void RemoveInstance(int id)
        {
            lock (syncRoot)
            {
                if (instances.Contains(id))
                    instances.Remove(id);
                else
                    return;
            }
            var instanceId=Helpers.RepositoryHelper.GetSelectedInstanceId(id);
            OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Removed, instanceId));
        }

        public void ClearInstances()
        {
            lock (syncRoot)
            {
                instances.Clear();
            }
            OnInstancesChanged(new UserViewInstancesChangedEventArgs(UserViewInstancesChangeType.Cleared));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("instances", instances);
        }
    }
}
