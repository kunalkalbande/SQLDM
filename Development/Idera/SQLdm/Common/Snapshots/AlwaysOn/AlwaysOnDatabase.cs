//------------------------------------------------------------------------------
// <copyright file="AlwaysOnDatabase.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace Idera.SQLdm.Common.Snapshots.AlwaysOn
{

    /// <summary>
    /// The AlwaysOn database topology.
    /// </summary>
    [Serializable]
    public class AlwaysOnDatabase
    {
        private long alwaysOnDatabaseId = -1;

        private Guid replicaId = Guid.Empty;

        private Guid groupId = Guid.Empty;

        private Guid groupDatabaseId = Guid.Empty;

        private long databaseId = -1;

        private String databaseName = String.Empty;

        /// <summary>
        /// The Cluster Server Name from which is collected the 'AlwaysOn Database' information.
        /// </summary>
        private String serverSourceName = String.Empty;

        public AlwaysOnDatabase(Guid replicaId, Guid groupId)
        {
            this.replicaId = replicaId;
            this.groupId = groupId;
        }

        public AlwaysOnDatabase()
        {
            // No op.
        }

        [XmlElement]
        public long AlwaysOnDatabaseId
        {
            get { return alwaysOnDatabaseId; }
            set { alwaysOnDatabaseId = value; }
        }

        [XmlElement]
        public Guid GroupDatabaseId
        {
            get { return groupDatabaseId; }
            set { groupDatabaseId = value; }
        }

        [XmlElement]
        public long DatabaseId
        {
            get { return databaseId; }
            set { databaseId = value; }
        }

        [XmlElement]
        public String DatabaseName
        {
            get { return databaseName; }
            set
            {
                if (! String.IsNullOrEmpty(value))
                {
                    databaseName = value;
                }
            }
        }

        [XmlElement]
        public Guid ReplicaId
        {
            get { return replicaId; }
            set { replicaId = value; }
        }

        [XmlElement]
        public Guid GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        /// <summary>
        /// The Cluster Server Name from which is collected the 'AlwaysOn Database' information.
        /// </summary>
        [XmlElement]
        public string ServerSourceName
        {
            get { return serverSourceName; }
            set { serverSourceName = value; }
        }
    }
}
