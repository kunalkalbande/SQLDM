//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Runtime.Remoting.Messaging;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Prepares the environment to log changes for Tag Action
    /// </summary>
    class TagAction : IAuditAction
    {
        private Tag newTag = null;
        private Tag oldTag = null;
        private IList<int> tagIds = null;
        private MonitoredSqlServerConfiguration newServer = null;
        private MonitoredSqlServerConfiguration oldServer = null;

        public Common.Auditing.AuditableActionType Type { get; set; }

        /// <summary>
        /// Recieves the new Tag configuration and the Old configuration
        /// </summary>
        /// <param name="newTag"></param>
        /// <param name="oldTag"></param>
        public TagAction(Tag newTag, Tag oldTag)
        {
            this.newTag = newTag;
            this.oldTag = oldTag;
        }

        /// <summary>
        /// Retrieves a list of TagIds
        /// </summary>
        public TagAction(IList<int> tagIds, AuditableActionType actionType)
        {
            Type = AuditableActionType.DeleteTag;
            this.tagIds = tagIds;
        }

        /// <summary>
        /// Retrieves the new server
        /// </summary>
        public TagAction(MonitoredSqlServerConfiguration server, AuditableActionType actionType)
        {
            Type = AuditableActionType.AddServerToTag;
            newServer = server;
        }

        /// <summary>
        /// Retrieves a change on the Monitored Server
        /// </summary>
        public TagAction(MonitoredSqlServerConfiguration server, MonitoredSqlServerConfiguration old, AuditableActionType actionType)
        {
            Type = AuditableActionType.AddServerToTag;
            newServer = server;
            oldServer = old;
        }

        /// <summary>
        /// Recieves an MAuditableEntity which already has the default values like SQLUser, and then adds more flavors to it according to the case.
        /// </summary>
        /// <param name="entity"></param>
        public void FillEntity(ref MAuditableEntity entity)
        {
            AddTagAction(ref entity);
            DeleteTagAction(ref entity);
            RemoveServerToTagsFromServerConfig(ref entity);
            RemoveServerToTagsFromTagConfig(ref entity);

            object context = MAuditingEngine.Instance.PopAuxiliarData("CustomCountersData");
            AddedCustomCounterToTagAction(ref entity, context);
            context = MAuditingEngine.Instance.PopAuxiliarData("CustomCountersDataRemovedTags");
            RemovedCustomCounterFromTagAction(ref entity, context);

            VerifySecurity(ref entity);

            bool serverAddedToTag = false;

            serverAddedToTag = AddServerToTag(ref entity);
            if(!serverAddedToTag)
            {
                serverAddedToTag = AddNewServerToTags(ref entity);
            }
                
            if(!serverAddedToTag)
            {
                AddExistingServerToTags(ref entity);
            }
        }

        private void VerifySecurity(ref MAuditableEntity entity)
        {
            bool logAction = (newTag != null && oldTag != null);
            if (!logAction)
            {
                entity = null;
                return;
            }

            List<int> newPermissions = new List<int>(newTag.Permissions);
            List<int> oldPermissions = new List<int>(oldTag.Permissions);

            var permisionsAdded = AuditTools.Except(newPermissions, oldPermissions);
            var permisionsRemoved = AuditTools.Except(oldPermissions, newPermissions);

            AddPermisionData(permisionsAdded, AuditableActionType.ApplicationSecurityLinkedToTag, newTag);
            AddPermisionData(permisionsRemoved, AuditableActionType.ApplicationSecurityUnlinkedFromTag, newTag);

            entity = null;
        }

        public void AddPermisionData(List<int> permisionsID, AuditableActionType actionType, Tag tag)
        {
            if (permisionsID.Count <= 0)
            {
                return;
            }

            foreach (var id in permisionsID)
            {
                var permission = RepositoryHelper.GetPermissionDefinitionById(id,
                                                                              ManagementServiceConfiguration.
                                                                                  ConnectionString);

                StringBuilder listItemName = new StringBuilder(permission.Login);
                listItemName.Append(" (");
                listItemName.Append(permission.PermissionType);
                listItemName.Append(")");
                if (!permission.Enabled)
                {
                    listItemName.Append(" (Disabled)");
                }

                MAuditableEntity entity = new MAuditableEntity();
                entity.Name = tag.Name;
                entity.SetHeaderParam(listItemName.ToString(), tag.Name);

                if (actionType == AuditableActionType.ApplicationSecurityLinkedToTag)
                {
                    entity.AddMetadataProperty("Linked Application Security User", listItemName.ToString());
                    entity.AddMetadataProperty("Linked Tag", tag.Name);
                }
                else if (actionType == AuditableActionType.ApplicationSecurityUnlinkedFromTag)
                {
                    entity.AddMetadataProperty("Unlinked Application Security User", listItemName.ToString());
                    entity.AddMetadataProperty("Unlinked Tag", tag.Name);
                }

                MAuditingEngine.Instance.LogAction(entity, actionType);
            }
        }

        /// <summary>
        /// Remove Server from Tag given an existing Tag Configuration
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveServerToTagsFromTagConfig(ref MAuditableEntity entity)
        {
            bool logAction = (newTag != null && oldTag != null);
            if (!logAction)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.DeleteServerToTag;

            var removedServers = new List<int>();
            foreach (int instance in oldTag.Instances)
            {
                if(!newTag.Instances.Contains(instance))
                    removedServers.Add(instance);
            }

            var instances = RepositoryHelper.GetMonitoredSqlServerNames(ManagementServiceConfiguration.GetRepositoryConnection(), null, true);

            foreach (int serverId in removedServers)
            {
                string serverName = GetInstance(serverId, instances).Second;
                entity = new MAuditableEntity();
                entity.Name = newTag.Name;
                entity.AddMetadataProperty("Deleted Server", serverName);
                entity.SetHeaderParam(serverName);
                MAuditingEngine.Instance.LogAction(entity, Type);
            }

            entity = null;
        }

        /// <summary>
        /// Removes a server from a Tag given a specific Server Configuration
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveServerToTagsFromServerConfig(ref MAuditableEntity entity)
        {
            bool logAction = (newServer != null && oldServer != null);
            if(!logAction)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.DeleteServerToTag;

            ICollection<Tag> tags = RepositoryHelper.GetTags(ManagementServiceConfiguration.ConnectionString);
            List<Tag> removedTags = new List<Tag>();

            foreach (int tagId in oldServer.Tags)
            {
                if (!newServer.Tags.Contains(tagId))
                    removedTags.Add(GetTagByID(tagId, tags));
            }

            foreach (Tag tag in removedTags)
            {
                entity = new MAuditableEntity();
                entity.Name = tag.Name;
                entity.AddMetadataProperty("Deleted Server", newServer.InstanceName);
                entity.SetHeaderParam(newServer.InstanceName);
                MAuditingEngine.Instance.LogAction(entity, Type);
            }

            entity = null;
        }

        /// <summary>
        /// Adds an existing Server's Tag
        /// </summary>
        /// <param name="entity"></param>
        private bool AddExistingServerToTags(ref MAuditableEntity entity)
        {
            bool logAction = (newServer != null && oldServer != null);
            if(!logAction)
            {
                entity = null;
                return false;
            }

            Type = AuditableActionType.AddServerToTag;

            ICollection<Tag> tags = RepositoryHelper.GetTags(ManagementServiceConfiguration.ConnectionString);
            List<Tag> newTags = new List<Tag>();

            foreach (int tagId in newServer.Tags)
            {
                if (!oldServer.Tags.Contains(tagId))
                    newTags.Add(GetTagByID(tagId, tags));
            }

            foreach (Tag tag in newTags)
            {
                entity = new MAuditableEntity();
                entity.Name = tag.Name;
                entity.AddMetadataProperty("Server added", newServer.InstanceName);
                entity.SetHeaderParam(newServer.InstanceName);
                MAuditingEngine.Instance.LogAction(entity, Type);
            }

            entity = null;
            return true;
        }

        /// <summary>
        /// Called whenever a server was created
        /// </summary>
        /// <param name="entity"></param>
        private bool AddNewServerToTags(ref MAuditableEntity entity)
        {
            bool logAction = (newServer != null && oldServer == null);
            if(!logAction)
            {
                entity = null;
                return false;
            }

            Type = AuditableActionType.AddServerToTag;

            var tags = RepositoryHelper.GetTags(ManagementServiceConfiguration.ConnectionString);
            List<Tag> newTags = new List<Tag>();

            foreach (Tag tag in tags)
            {
                if(newServer.Tags.Contains(tag.Id))
                    newTags.Add(tag);
            }

            foreach (Tag tag in newTags)
            {
                entity = new MAuditableEntity();
                entity.Name = tag.Name;
                entity.AddMetadataProperty("Server added", newServer.InstanceName);
                entity.SetHeaderParam(newServer.InstanceName);
                MAuditingEngine.Instance.LogAction(entity, Type);
            }

            entity = null;
            return true;
        }

        /// <summary>
        /// Logs the adition of servers to a Tag
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        private bool AddServerToTag(ref MAuditableEntity entity)
        {
            object context = MAuditingEngine.Instance.PopAuxiliarData("ServersData");
            bool logAction = (context != null && newTag!= null);
            if (!logAction)
            {
                entity = null;
                return false;
            }

            Type = AuditableActionType.AddServerToTag;
            var instances = RepositoryHelper.GetMonitoredSqlServerNames(ManagementServiceConfiguration.GetRepositoryConnection(), null, true);

            List<int> newServers = GetNewServers();

            foreach (int instance in newServers)
            {
                string instanceName = GetInstance(instance, instances).Second;

                entity = new MAuditableEntity();
                entity.Name = newTag.Name;
                entity.AddMetadataProperty("Server added", instanceName);
                entity.SetHeaderParam(instanceName);
                MAuditingEngine.Instance.LogAction(entity, Type);
            }
            
            entity = null;
            return true;
        }

        /// <summary>
        /// Returns a list of the New Servers
        /// </summary>
        /// <returns></returns>
        private List<int> GetNewServers()
        {
            List<int> newServers = new List<int>();

            foreach (int instance in newTag.Instances)
            {
                if(!oldTag.Instances.Contains(instance))
                {
                    newServers.Add(instance);
                }
            }

            return newServers;
        }

        /// <summary>
        /// Specific case for Adding Custom Counter to Tag
        /// </summary>
        /// <param name="entity"></param>
        private void AddedCustomCounterToTagAction(ref MAuditableEntity entity, object context)
        {
            var contextObject = context as AuditAuxiliar<List<KeyValuePair<string, string>>>;

            if (contextObject == null || contextObject.Data == null)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.CustomCounterAddedToTag;

            //CustomCountersData should contain a List<KeyValuePair<string, string>>
            //With a list of Tag, CounterName

            foreach (KeyValuePair<string, string> keyValuePair in contextObject.Data)
            {
                var auditableEntity = new MAuditableEntity();
                auditableEntity.Name = String.Format(keyValuePair.Value);
                auditableEntity.AddMetadataProperty("Linked Tag", keyValuePair.Key);
                auditableEntity.SetHeaderParam(keyValuePair.Value, keyValuePair.Key);
                MAuditingEngine.Instance.LogAction(auditableEntity, Type);
            }

            entity = null;  
        }

        /// <summary>
        /// Logs remove Custom Counter from Tag action
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        private void RemovedCustomCounterFromTagAction(ref MAuditableEntity entity, object context)
        {
            var contextObject = context as AuditAuxiliar<List<KeyValuePair<string, string>>>;

            if (contextObject == null || contextObject.Data == null)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.CustomCounterRemovedToTag;

            //CustomCountersData should contain a List<KeyValuePair<string, string>>
            //With a list of Tag, CounterName

            foreach (KeyValuePair<string, string> keyValuePair in contextObject.Data)
            {
                var auditableEntity = new MAuditableEntity();
                auditableEntity.Name = String.Format(keyValuePair.Value);
                auditableEntity.AddMetadataProperty("Unlinked Tag", keyValuePair.Key);
                auditableEntity.SetHeaderParam(keyValuePair.Value, keyValuePair.Key);
                MAuditingEngine.Instance.LogAction(auditableEntity, Type);
            }

            entity = null;
        }

        /// <summary>
        /// Specific case for Adding a Tag
        /// </summary>
        /// <param name="entity"></param>
        private void AddTagAction(ref MAuditableEntity entity)
        {
            bool logAction = (oldTag == null && newTag != null);
            if(!logAction)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.AddTag;

            entity.AddMetadataProperty("Tag name", newTag.Name);
            entity.Name = newTag.Name;

            var instances = RepositoryHelper.GetMonitoredSqlServerNames(ManagementServiceConfiguration.GetRepositoryConnection(), null, true);

            //get all ID server instance have a Tag  );
            foreach (int serverId in newTag.Instances)
            {
                string instanceName = GetInstance(serverId, instances).Second;
                entity.AddMetadataProperty("Server name", instanceName);
            }

            var auditable = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
            entity.SetHeaderParam(auditable.SqlUser);

            MAuditingEngine.Instance.LogAction(entity, Type);

            this.oldTag = new Tag(-1, "empty");
            this.AddServerToTag(ref entity);

            entity = null;
        }

        /// <summary>
        /// Specific case for Adding a Tag
        /// </summary>
        /// <param name="entity"></param>
        private void DeleteTagAction(ref MAuditableEntity entity)
        {
            if(tagIds == null)
            {
                entity = null;
                return;
            }

            Type = AuditableActionType.DeleteTag;

            var instances = RepositoryHelper.GetMonitoredSqlServerNames(ManagementServiceConfiguration.GetRepositoryConnection(), null, true);
            var contextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

            foreach (int tagId in tagIds)
            {
                var tag = RepositoryHelper.GetTagById(ManagementServiceConfiguration.ConnectionString, tagId);
                entity = new MAuditableEntity();

                entity.AddMetadataProperty("Tag name", tag.Name);
                entity.Name = tag.Name;
                entity.SetHeaderParam(contextData == null ? ManagementServiceConfiguration.RepositoryUser : contextData.SqlUser);

                foreach (int serverId in tag.Instances)
                {
                    string instanceName = GetInstance(serverId, instances).Second;
                    entity.AddMetadataProperty("Server name", instanceName);
                }

                MAuditingEngine.Instance.LogAction(entity, Type);
            }

            entity = null;  
        }

        /// <summary>
        /// Given a list of Triple, it will return the only one that has serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="instances"></param>
        /// <returns></returns>
        private Triple<int, string, bool> GetInstance(int serverId, List<Triple<int, string, bool>> instances)
        {
            var instance = new Triple<int, string, bool>();

            foreach (Triple<int, string, bool> triple in instances)
            {
                if (triple.First == serverId)
                {
                    instance = triple;
                    break;
                }   
            }

            return instance;
        }

        /// <summary>
        /// Given a list of Tags, returns an specific tag by its ID
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private Tag GetTagByID(int tagId, ICollection<Tag> tags)
        {
            Tag returnTag = null;

            foreach (Tag tag in tags)
            {
                if (tag.Id == tagId)
                {
                    returnTag = tag;
                    break;
                }
            }

            return returnTag;
        }
    }
}
