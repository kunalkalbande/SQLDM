using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System.Runtime.Serialization;

    [Serializable]
    internal abstract class UserView : ISerializable
    {
        protected object syncRoot = new object();
        private Guid id = Guid.NewGuid();
        protected string name;

        [NonSerialized]
        public EventHandler NameChanged;
        
        [NonSerialized]
        public EventHandler<UserViewInstancesChangedEventArgs> InstancesChanged;

        [NonSerialized]
        public EventHandler SeverityChanged;


        public UserView()
        {
        }

        protected UserView(SerializationInfo info, StreamingContext context)
        {
            id = (Guid)info.GetValue("id", typeof (Guid));
            name = info.GetString("name");
        }

        public Guid Id
        {
            get { return id; }
            protected set { id = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    throw new ArgumentNullException("The view name cannot be null or empty.");
                }

                if (name != value)
                {
                    name = value;
                    OnNameChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Returns a read-only collection of instance id.
        /// </summary>
        public abstract ICollection<int> Instances
        {
            get;
        }

        public abstract UserViewStatus Severity
        {
            get;
            protected set;
        }

        public abstract void Update();

        private void OnNameChanged(EventArgs e)
        {
            if (NameChanged != null)
            {
                NameChanged(this, e);
            }
        }

        protected void OnInstancesChanged(UserViewInstancesChangedEventArgs e)
        {
            if (InstancesChanged != null)
            {
                InstancesChanged(this, e);
            }
        }

        protected void OnSeverityChanged()
        {
            if (SeverityChanged != null)
            {
                SeverityChanged(this, EventArgs.Empty);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("name", name);
        }
    }

    internal class UserViewInstancesChangedEventArgs : EventArgs
    {
        public readonly UserViewInstancesChangeType ChangeType;
        public readonly int InstanceId;

        public UserViewInstancesChangedEventArgs(
            UserViewInstancesChangeType change) : this(change, -1)
        {
        }

        public UserViewInstancesChangedEventArgs(
            UserViewInstancesChangeType change,
            int instanceId)
        {
            ChangeType = change;
            InstanceId = instanceId;
        }
    }

    [Serializable]
    internal enum UserViewStatus
    {
        None,
        MaintenanceMode,
        OK,
        Warning,
        Critical,
    }

    internal enum UserViewInstancesChangeType
    {
        Added,
        Removed,
        Replaced,
        Cleared
    }
}