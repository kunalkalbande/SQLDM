using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public class Tag :IAuditable
    {
        private readonly int tagId;
        private readonly string tagName;
        private readonly List<int> instances = new List<int>();
        private readonly List<int> customCounters = new List<int>();
        private readonly List<int> permissions = new List<int>();

        private bool isGlobalTag = false; // SQLdm 10.1 - Praveen Suhalka - CWF3 Integration

        public Tag(int id, string name) : this(id, name, null, null, null)
        {
        }

        public Tag(int id, Tag tag)
        {
            tagId = id;
            tagName = tag.Name;
            instances.AddRange(tag.Instances);
            customCounters.AddRange(tag.CustomCounters);
            permissions.AddRange(tag.Permissions);
        }

        public Tag(int id, string name, IEnumerable<int> instances, IEnumerable<int> customCounters, IEnumerable<int> permissions)
        {
            tagId = id;
            tagName = name;

            if (instances != null)
            {
                this.instances.AddRange(instances);
            }

            if (customCounters != null)
            {
                this.customCounters.AddRange(customCounters);
            }

            if (permissions != null)
            {
                this.permissions.AddRange(permissions);
            }
        }

        /// <summary>
        /// //SQLdm 10.1 - Praveen Suhalka - CWF3 Integration
        /// </summary>
        public bool IsGlobalTag
        {
            get { return isGlobalTag; }
            set { isGlobalTag = value; }
        }

        public int Id
        {
            get { return tagId; }
        }

        public string Name
        {
            get { return tagName; }
        }

        public IList<int> Instances
        {
            get { return new ReadOnlyCollection<int>(instances); }
        }

        public IList<int> CustomCounters
        {
            get { return new ReadOnlyCollection<int>(customCounters); }
        }

        public IList<int> Permissions
        {
            get { return new ReadOnlyCollection<int>(permissions); }
        }

        public void AddInstance(int instanceId)
        {
            if (!instances.Contains(instanceId))
            {
                instances.Add(instanceId);
            }
        }

        public void FilterInstances(ICollection<int> activeInstances)
        {
            List<int> preFilterInstances = new List<int>(instances);

            foreach (int instanceId in preFilterInstances)
            {
                if (!activeInstances.Contains(instanceId))
                {
                    instances.Remove(instanceId);
                }
            }
        }

        public bool AddCustomCounter(int customCounterId)
        {
            if (!customCounters.Contains(customCounterId))
            {
                customCounters.Add(customCounterId);
                return true;
            }
            return false;
        }

        public bool RemoveCustomCounter(int customCounterId)
        {
            if (customCounters.Contains(customCounterId))
            {
                customCounters.Remove(customCounterId);
                return true;
            }
            return false;
        }

        public void AddPermission(int permissionId)
        {
            if (!permissions.Contains(permissionId))
            {
                permissions.Add(permissionId);
            }
        }


        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {

           AuditableEntity auditableEntity = new AuditableEntity();
           auditableEntity.AddMetadataProperty("Tag name", this.Name);
           auditableEntity.Name=this.Name;

           return auditableEntity;
        }

        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return GetAuditableEntity();
        }

        /// <summary>
        /// Compare the deference between permissions, and returns false in case in both permission lists
        /// has not the same elements.
        /// </summary>
        /// <param name="otherTag">The tag with which compare the permissions.</param>
        /// <param name="notEqualsPermissions">The permisions that are different in both lists.</param>
        /// <returns>False in case in both permission lists has not the same elements</returns>
        public bool ComparePermissions(Tag otherTag, out List<int> notEqualsPermissions)
        {
            notEqualsPermissions = new List<int>();
            if(otherTag == null || otherTag.Permissions == null) return false;

            IList<int> otherTagPermissions =  otherTag.Permissions;
            List<int> retainedPermissions = new List<int>(otherTagPermissions);

            foreach (int permissionId in Permissions)
            {
                if (otherTagPermissions.Contains(permissionId))
                {
                    retainedPermissions.Remove(permissionId);
                }
                else if (! notEqualsPermissions.Contains(permissionId))
                {
                    notEqualsPermissions.Add(permissionId);
                }
            }

            notEqualsPermissions.AddRange(retainedPermissions);

            return notEqualsPermissions.Count > 0;
        }
    }
}
