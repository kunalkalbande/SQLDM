//------------------------------------------------------------------------------
// <copyright file="MonitoredObjectName.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Objects
{
    using System.Security.Cryptography;

    [Serializable]
    public class MonitoredObjectName : IComparable<MonitoredObjectName>, ICloneable
    {
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MonitoredObjectName");
        private static string[] EMPTY_STRING_ARRAY = { } ;

        private string serverName;
        private string databaseName;
        private string tableName;
        private string resourceName;
        private InstanceType resourceType = InstanceType.Unknown;

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --add filegroup property to generate unique hashcode for filegroup alerts
        private string filegroup;

        public string Filegroup
        {
            get { return filegroup; }
            set { filegroup = value; }
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --add filegroup property to generate unique hashcode for filegroup alerts
        private string[] additionalQualifiers;
        private string qualifiedValue;

        private string qualifierHash;

        public MonitoredObjectName(string serverName)
        {
            this.serverName = serverName.Trim();
        }

        public MonitoredObjectName(MonitoredSqlServer server) : this(server.ConnectionInfo.InstanceName)
        {    
        }

        public MonitoredObjectName(MonitoredObject monitoredObject) 
        {
            if (monitoredObject is MonitoredDatabase)
            {
                serverName = ((MonitoredDatabase)monitoredObject).Server.ConnectionInfo.InstanceName;
                DatabaseName = ((MonitoredDatabase)monitoredObject).Name;
            }
            else
                if (monitoredObject is MonitoredTable)
                {
                    serverName = ((MonitoredTable)monitoredObject).Database.Server.ConnectionInfo.InstanceName;
                    databaseName = ((MonitoredTable)monitoredObject).Database.Name;
                    tableName = ((MonitoredTable)monitoredObject).Name;
                }
                else
                    throw new ArgumentException("Monitored object must a database or a table");
        }

        public MonitoredObjectName(string serverName, string databaseName)
        {
            this.serverName = serverName;
            DatabaseName = databaseName;
        }

        public MonitoredObjectName(string serverName, string resourceName, InstanceType resourceType) 
            :this(serverName)
        {
            this.resourceName = resourceName;
            this.resourceType = resourceType;

            if (resourceType == InstanceType.Database)
                this.databaseName = resourceName;
        }
            

        public MonitoredObjectName(string serverName, string databaseName, string tableName)
        {
            this.serverName = serverName;
            DatabaseName = databaseName;
            this.tableName = tableName;
        }

        public MonitoredObjectName(string serverName, string databaseName, string tableName, string qualifierHash) 
            : this(serverName, databaseName, tableName)
        {
            this.qualifierHash = qualifierHash;
        }

        public bool IsServer
        {
            get { return databaseName == null;  }
        }

        public bool IsDatabase
        {
            get { return databaseName != null && tableName == null; }
        }

        public bool IsTable
        {
            get { return tableName != null; }
        }

        public bool IsMonitoredResource
        {
            get { return resourceType != InstanceType.Unknown && resourceName != null; }
        }

        public bool HasAdditionalQualifiers
        {
            get { return additionalQualifiers != null && additionalQualifiers.Length > 0; }
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public string ServerName
        {
            get { return serverName; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = resourceName = value; }
        }

        public string TableName
        {
            get { return tableName; }
        }

        public string ResourceName
        {
            get { return resourceName; }
        }

        public InstanceType ResourceType
        {
            get { return resourceType; }
        }

        public string[] AdditionalQualifiers
        {
            get 
            {
                if (HasAdditionalQualifiers)
                    return additionalQualifiers;
                return EMPTY_STRING_ARRAY;
            }
            set
            {
                qualifierHash = qualifiedValue = null;
                additionalQualifiers = value;
            }
        }

        public string GetQualifierHash()
        {
            if (qualifierHash == null)
            {
                string xml = AdditionalQualifiersXML;
                if (xml != null)
                {
                    byte[] hash = Idera.SQLdm.Common.Security.Encryption.Cipher.GetSHAHash(xml);
                    qualifierHash = Convert.ToBase64String(hash);
                }
            }
            return qualifierHash;
        }

        private string AdditionalQualifiersXML
        {
            get
            {
                if (!HasAdditionalQualifiers)
                    return null;

                XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(string[]));
                try
                {
                    XmlWriterSettings writerSettings = new XmlWriterSettings();
                    writerSettings.CheckCharacters = false;
                    StringBuilder buffer = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(buffer, writerSettings))
                    {
                        serializer.Serialize(writer, additionalQualifiers);
                        writer.Flush();
                    }
                    return buffer.ToString();
                } catch (Exception e)
                {
                    StringBuilder builder = new StringBuilder(serverName);
                    try
                    {
                        if (databaseName != null)
                        {
                            builder.Append(".").Append(databaseName);
                            if (tableName != null)
                            {
                                builder.Append(".").Append(tableName);
                            }
                        }
                        builder.Append("+");
                        int mark = builder.Length;
                        foreach (string qualifier in AdditionalQualifiers)
                        {
                            if (builder.Length > mark)
                                builder.Append(".");
                            builder.Append(qualifier);
                        }
                    } catch (Exception)
                    {
                    }
                    LOG.ErrorFormat("Failed to serialize additional qualifiers: {0} {1}", builder, e);
                    throw;
                }
            }
            set
            {
                if (value == null)
                    AdditionalQualifiers = null;
                else
                {
                    XmlSerializer serializer =
                        Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof (string[]));

                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    readerSettings.CheckCharacters = false;
                    StringReader stream = new StringReader(value);
                    using (XmlReader xmlReader = XmlReader.Create(stream, readerSettings))
                    {
                        AdditionalQualifiers = (string[]) serializer.Deserialize(xmlReader);
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            StringBuilder builder = new StringBuilder(serverName);
            if ((resourceType == InstanceType.Unknown)||(resourceType == InstanceType.Database))
            {
                if (databaseName != null)
                {
                    builder.Append(".").Append(databaseName);
                    if (tableName != null)
                    {
                        builder.Append(".").Append(tableName);
                    }
                    //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --add filegroup property to generate unique hashcode for filegroup alerts
                    if (!String.IsNullOrEmpty(filegroup))
                    {
                        builder.Append(".").Append(filegroup);
                    }
                }
            }
            else
            {
                builder.Append(".").Append(resourceName).Append(".").Append(resourceType.ToString());
            }


            string qualifierHash = GetQualifierHash();
            if (qualifierHash != null)
                builder.Append("~").Append(qualifierHash);

            return builder.ToString().GetHashCode();
        }

        public override string ToString()
        {
            if (qualifiedValue == null)
            {
                StringBuilder builder = new StringBuilder(serverName);
                if ((resourceType == InstanceType.Unknown) || (resourceType == InstanceType.Database))
                {
                    if (databaseName != null)
                    {
                        builder.Append(".").Append(databaseName);
                        if (tableName != null)
                        {
                            builder.Append(".").Append(tableName);
                        }
                        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --add filegroup property to generate unique hashcode for filegroup alerts
                        if (!String.IsNullOrEmpty(filegroup))
                        {
                            builder.Append(".").Append(filegroup);
                        }
                    }
                }
                else
                {
                    builder.Append(".").Append(resourceName).Append(".").Append(resourceType.ToString());
                }
                if (HasAdditionalQualifiers)
                {
                    builder.Append("+");
                    int mark = builder.Length;
                    foreach (string qualifier in AdditionalQualifiers)
                    {
                        if (builder.Length > mark)
                            builder.Append(".");
                        builder.Append(qualifier);
                    }
                } else
                if (qualifierHash != null)
                {
                    builder.Append("~").Append(qualifierHash);
                }

                qualifiedValue = builder.ToString();
            }
            return qualifiedValue;
        }

        #region IComparable<MonitoredObjectName> Members

        int IComparable<MonitoredObjectName>.CompareTo(MonitoredObjectName other)
        {
            int rc = 0;
            if (serverName == null)
            {
                if (other.serverName != null)
                    rc = -1;
            } else
            if (other.serverName == null)
                rc = 1;
            else
                rc = serverName.CompareTo(other.serverName);

            if (rc == 0)
            {
                if (databaseName == null)
                {
                    if (other.databaseName != null)
                        rc = -1;
                }
                else
                    if (other.databaseName == null)
                    rc = 1;
                else
                    rc = databaseName.CompareTo(other.databaseName);
            }
            if (rc == 0)
            {
                if (tableName == null)
                {
                    if (other.tableName != null)
                        rc = -1;
                }
                else
                if (other.tableName == null)
                    rc = 1;
                else
                    rc = tableName.CompareTo(other.tableName);
            }
            if (rc == 0)
            {
                if (resourceName == null)
                {
                    if (other.resourceName != null)
                        rc = -1;
                }
                else
                {
                    if (other.resourceName == null)
                        rc = 1;
                    else
                        rc = resourceName.CompareTo(other.ResourceName);
                }
            }
            if (rc == 0)
            {
                if (resourceType != other.ResourceType)
                {
                    if (resourceType < other.ResourceType)
                        rc = -1;
                    else
                        rc = 1;
                }
            }

            if (rc == 0)
            {
                string[] quals = AdditionalQualifiers;
                string[] oquals = other.AdditionalQualifiers;

                if (quals == null || quals.Length == 0)
                {
                    if (oquals != null && oquals.Length > 0)
                        rc = -1;
                }
                else
                if (oquals == null || oquals.Length == 0)
                   rc = 1;
                else
                {
                    int ui = Math.Max(quals.Length, oquals.Length);
                    for (int i = 0; rc == 0 && i < ui; i++)
                    {
                        string left =  i < quals.Length ? quals[i] : null;
                        string right = i < oquals.Length ? oquals[i] : null;
                        if (left == null)
                        {
                            if (right != null)
                                rc = -1;
                        }
                        else
                        if (right == null)
                            rc = 1;
                        else
                            rc = left.CompareTo(right);
                    }   
                }
            }
            return rc;
        }

        #endregion
        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
