//------------------------------------------------------------------------------
// <copyright file="AvailabilityGroup.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Idera.SQLdm.Common.Snapshots.AlwaysOn
{
    /// <summary>
    /// The availability group topology information.
    /// </summary>
    [Serializable]
    public class AvailabilityGroup
    {

        private Guid groupId = Guid.Empty;

        private String groupName = String.Empty;

        /// <summary>
        /// The Cluster Server Name from which is collected the 'Availability Group' information.
        /// </summary>
        private String serverSourceName = String.Empty;

        private String listenerDnsName = String.Empty;

        private int listenerPort = 0;

        private String listenerIPAddress = String.Empty;

        private Dictionary<Guid, AvailabilityReplica> replicas = new Dictionary<Guid, AvailabilityReplica>();

        [XmlElement]
        public Guid GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        [XmlElement]
        public String GroupName
        {
            get { return groupName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    groupName = value;
                }
            }
        }

        [XmlElement]
        public String ListenerDnsName
        {
            get { return listenerDnsName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    listenerDnsName = value;
                }
            }
        }

        [XmlElement]
        public String ListenerIPAddress
        {
            get { return listenerIPAddress; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    listenerIPAddress = value;
                }
            }
        }

        [XmlArray(ElementName = "ReplicaItems")]
        [XmlArrayItem(ElementName = "AvailabilityReplica")]
        public List<AvailabilityReplica> ReplicaItems
        {
            get
            {
                return new List<AvailabilityReplica>(replicas.Values);
            }

            set
            {
                // No op. Defined in order to serialize this class.
            }
        }

        [XmlIgnore]
        public Dictionary<Guid, AvailabilityReplica> Replicas
        { 
            get { return new Dictionary<Guid, AvailabilityReplica>(replicas);}
        }

        [XmlElement]
        public int ListenerPort
        {
            get { return listenerPort; }
            set { listenerPort = value; }
        }

        /// <summary>
        /// The Cluster Server Name from which is collected the 'Availability Group' information.
        /// </summary>
        [XmlElement]
        public string ServerSourceName
        {
            get { return serverSourceName; }
            set { serverSourceName = value; }
        }

        public void AddReplica(AvailabilityReplica availabilityReplica)
        {
            if (availabilityReplica == null)
            {
                return;
            }

            Guid replicaId = availabilityReplica.ReplicaId;

            if (replicas.ContainsKey(replicaId))
            {
                replicas[replicaId] = availabilityReplica;
            }
            else
            {
                replicas.Add(replicaId, availabilityReplica);
            }
        }
    }
}
