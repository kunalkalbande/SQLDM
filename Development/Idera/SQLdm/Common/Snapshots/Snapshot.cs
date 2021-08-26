//------------------------------------------------------------------------------
// <copyright file="Snapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for all SQLdm snapshots.  This class includes a GUID and timestamp to identify the snapshot.
    /// </summary>
    [Serializable]
    public abstract class Snapshot
    {
        #region fields

        //private Guid id = new Guid();
        private string serverName = null;
        private ServerVersion productVersion = null;
        private DateTime? timestamp = null;
        private DateTime? timeStampLocal = null;
        private Exception error;
        private ProbePermissionHelpers.ProbeError probeError;
        private DateTime? serverStartupTime = null;
        private string productEdition = null;
        private bool isFullTextInstalled = false;

        private Dictionary<string, double> awsCloudMetrics = new Dictionary<string, double>();
        private Dictionary<string, Dictionary<string, object>> azureCloudMetrics = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, List<string>> azureElasticPools = new Dictionary<string, List<string>>();


        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snapshot"/> class.
        /// </summary>
        public Snapshot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snapshot"/> class.
        /// </summary>
        public Snapshot(string serverName)
        {
            this.serverName = serverName;
        }


        #endregion

        #region properties

        /// <summary>
        /// Gets whether collection of the sample failed.
        /// </summary>
        public bool CollectionFailed
        {
            get { return (Error != null) ? true : false; }
        }

        /// <summary>
        /// Gets the name of the sampled SQL Server.
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
            internal set { serverName = value; }
        }

        /// <summary>
        /// Contains the first error that occurred during collection
        /// </summary>
        public Exception Error
        {
            get { return error; }
            protected set { error = value; }
        }

        /// <summary>
        /// Contains the Probe Permission error if occurred during collection
        /// </summary>
        public ProbePermissionHelpers.ProbeError ProbeError
        {
            get { return probeError; }
            set { probeError = value; }
        }
        /// <summary>
        /// Gets the SQL Server product version.
        /// </summary>
        public ServerVersion ProductVersion
        {
            get { return productVersion; }
            internal set { productVersion = value; }
        }

        /// <summary>
        /// Gets the SQL Server product edition
        /// </summary>
        public string ProductEdition
        {
            get { return productEdition; }
            internal set { productEdition = value; }
        }

        /// <summary>
        /// Gets the time stamp for the sample (UTC)
        /// </summary>
        public DateTime? TimeStamp
        {
            get { return timestamp; }
            internal set { timestamp = value; }
        }

        /// <summary>
        /// Gets the time stamp for the sample in local time
        /// </summary>
        public DateTime? TimeStampLocal
        {
            get { return timeStampLocal; }
            internal set { timeStampLocal = value; }
        }

        /// <summary>
        /// Returns the server startup time
        /// </summary>
        public DateTime? ServerStartupTime
        {
            get { return serverStartupTime; }
            internal set { serverStartupTime = value; }
        }

        /// <summary>
        /// Checks if fullText feature is installed on server
        /// </summary>
        public bool IsFullTextInstalled
        {
            get { return isFullTextInstalled; }
            internal set { isFullTextInstalled = value; }
        }

        /// <summary>
        /// Stores mostly all Permissions required for Collector known
        /// </summary>
        /// <remarks>
        /// It didn't include the permissions which are required at runtime in replication or mirroring
        /// </remarks>
        public CollectionPermissions CollectionPermissions
        {
            get; set;
        }

        /// <summary>
        /// Stores Permissions related with metadata visibility on servers
        /// </summary>
        public MetadataPermissions MetadataPermissions
        {
            get; set;
        }

        /// <summary>
        /// Stores Minimum Permissions required by the SQLdm to work on the servers
        /// </summary>
        public MinimumPermissions MinimumPermissions
        {
            get; set;
        }

        public Dictionary<string, Dictionary<string, object>> AzureCloudMetrics
        {
            get { return azureCloudMetrics; }
            internal set { azureCloudMetrics = value; }
        }

        public Dictionary<string, double> AWSCloudMetrics
        {
            get { return awsCloudMetrics; }
            internal set { awsCloudMetrics = value; }
        }
        public Dictionary<string, List<string>> AzureElasticPools
        {
            get { return azureElasticPools; }
            internal set { azureElasticPools = value; }
        }
        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Sets the error that caused collection to fail
        /// Only one exception is permitted as it is the one which initially caused the failure
        /// </summary>
        internal void SetError(string message, Exception e)
        {
            if (Error == null)
            {
                if (e == null)
                {
                    Error = new Exception(message);
                }
                else
                {
                    Error = new Exception(String.Format(message, e.Message), e);
                }
            }
        }

        /// <summary>
        /// Sets the error that caused collection to fail
        /// Only one exception is permitted as it is the one which initially caused the failure
        /// </summary>
        internal void SetPermissionError(string message, Exception e, ProbePermissionHelpers.ProbeError probeError)
        {
            this.probeError = probeError;
            SetError(message, e);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            if (CollectionFailed)
            {
                return String.Format("{0} snapshot from {1} failure: {2}",
                    GetType().Name, ServerName, Error.Message);
            }
            else
            {
                return String.Format("{0} snapshot from {1} success",
                    GetType().Name, ServerName);
            }
        }
        #endregion

        /// <summary>
        /// Method to allow subclassers to allow subclassers a chance to initialize the 
        /// base class member variables.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected void SetObjectData(SerializationInfo info, StreamingContext context)
        {
            serverName = info.GetString("serverName");
            productVersion = (ServerVersion)info.GetValue("productVersion", typeof(ServerVersion));
            try
            {
				// SQLdm Minimum Privileges - Varun Chopra - Include Server Permissions
                MinimumPermissions = (MinimumPermissions)info.GetValue("MinimumPermissions", typeof(MinimumPermissions));
                MetadataPermissions = (MetadataPermissions)info.GetValue("MetadataPermissions", typeof(MetadataPermissions));
                CollectionPermissions = (CollectionPermissions)info.GetValue("CollectionPermissions", typeof(CollectionPermissions));

                // For Probe Permission Errors
                probeError = (ProbePermissionHelpers.ProbeError)info.GetValue("probeError", typeof(ProbePermissionHelpers.ProbeError));

            }
            catch (Exception exception)
            {
                Console.WriteLine("SetObjectData : Exception parsing Permissions : " + exception);
            }
            timestamp = (DateTime?)info.GetValue("timestamp", typeof(DateTime?));
            timeStampLocal = (DateTime?)info.GetValue("timeStampLocal", typeof(DateTime?));
            error = (Exception)info.GetValue("error", typeof(Exception));
            serverStartupTime = (DateTime?)info.GetValue("serverStartupTime", typeof(DateTime?));
            productEdition = info.GetString("productEdition");
            isFullTextInstalled = info.GetBoolean("isFullTextInstalled");
        }

        /// <summary>
        /// Implement GetObjectData from ISerializable (even though Snapshot does not
        /// implement the ISerializable interface) so that subclasses can implement
        /// ISerializable and call this to add in variables from the base class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("serverName", serverName);
            info.AddValue("productVersion", productVersion);
            try
            {
			    // SQLdm Minimum Privileges - Varun Chopra - Include Server Permissions
                info.AddValue("MinimumPermissions", MinimumPermissions);
                info.AddValue("CollectionPermissions", CollectionPermissions);
                info.AddValue("MetadataPermissions", MetadataPermissions);

                info.AddValue("probeError", probeError);
            }
            catch (Exception exception)
            {
                Console.WriteLine("ISerializable_GetObjectData : Exception adding values for  Permissions : " + exception);
            }
            info.AddValue("timestamp", timestamp);
            info.AddValue("timeStampLocal", timeStampLocal);
            info.AddValue("error", error);
            info.AddValue("serverStartupTime", serverStartupTime);
            info.AddValue("productEdition", productEdition);
            info.AddValue("isFullTextInstalled", isFullTextInstalled);
        }

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
