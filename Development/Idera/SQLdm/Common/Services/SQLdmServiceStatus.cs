//------------------------------------------------------------------------------
// <copyright file="SQLdmServiceStatus.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class InstanceName
    {
        public readonly string Machine;
        public readonly string Instance;
        public InstanceName(string machine, string instance)
        {
            Machine = machine;
            Instance = instance;
        }

        public override string ToString()
        {
            return String.Format("{0}\\{1}", Machine, Instance);
        }
    }

    [Serializable]
    public class ManagementServiceStatus
    {
        private Guid? managementServiceID;
        private InstanceName instanceName;
        private int? servicePort;
        private Exception connectionException;
        
        private string repositoryHost;
        private string repositoryDatabase;
        private TestResult? repositoryTestResult;
        private Exception repositoryTestException;
        

        private SQLdmServiceStatus status = SQLdmServiceStatus.Unknown;

//        private string assemblyVersion;
        private Guid? defaultCollectionServiceID;
        private List<CollectionServiceStatus> collectionServices;

        public Guid? ManagementServiceID
        {
            get { return managementServiceID;   }
            set { managementServiceID = value;  }
        }
        public InstanceName InstanceName 
        {
            get { return instanceName; }
            set { instanceName = value; }
        }
        public int? ServicePort
        {
            get { return servicePort; }
            set { servicePort = value; }
        }
        public Exception ConnectionException
        {
            get { return connectionException;  }
            set { connectionException = value; }
        }
        public Guid? DefaultCollectionServiceID
        {
            get { return defaultCollectionServiceID;  }
            set { defaultCollectionServiceID = value; }
        }
        public SQLdmServiceStatus Status
        {
            get { return status;  }
            set { status = value; }
        }
        public string RepositoryHost
        {
            get { return repositoryHost; }
            set { repositoryHost = value; }
        }
        public string RepositoryDatabase
        {
            get { return repositoryDatabase; }
            set { repositoryDatabase = value; }
        }
        public TestResult? RepositoryConnectionTestResult
        {
            get { return repositoryTestResult; }
            set { repositoryTestResult = value; }
        }
        public Exception RepositoryConnectionTestException
        {
            get { return repositoryTestException; }
            set { repositoryTestException = value; }
        }

        public List<CollectionServiceStatus> CollectionServices
        {
            get
            {
                if (collectionServices == null)
                    collectionServices = new List<CollectionServiceStatus>();
                return collectionServices;
            }
        }

        public List<NewsfeedServiceStatus> NewsfeedServices
        {
            get
            {
                return new List<NewsfeedServiceStatus>();
            }
        }
    }

    [Serializable]
    public class CollectionServiceStatus
    {
        private Guid? collectionServiceID;        
        private InstanceName instanceName;
        private int? servicePort;
        private SQLdmServiceStatus status = SQLdmServiceStatus.Unknown;
        private DateTime? lastHeartbeatReceived;
        private DateTime? nextHeartbeatExpected;
        private Exception collectionServiceConnectException;

//        private string assemblyVersion;
//        private Guid? managementServiceID;
        private string managementServiceAddress;
        private int?   managementServicePort;
        private TestResult? managementServiceTestResult;
        private Exception managementServiceTestException;

        public Guid? CollectionServiceID
        {
            get { return collectionServiceID; }
            set { collectionServiceID = value; }
        }
        public InstanceName InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }
        public int? ServicePort
        {
            get { return servicePort; }
            set { servicePort = value; }
        }
        public Exception CollectionServiceConnectionException
        {
            get { return collectionServiceConnectException; }
            set { collectionServiceConnectException = value; }
        }
        public SQLdmServiceStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        public DateTime? LastHeartbeatReceived
        {
            get { return lastHeartbeatReceived;  }
            set { lastHeartbeatReceived = value; }
        }
        public DateTime? NextHeartbeatExpected
        {
            get { return nextHeartbeatExpected; }
            set { nextHeartbeatExpected = value; }
        }
        public string ManagementServiceAddress
        {
            get { return managementServiceAddress; }
            set { managementServiceAddress = value; }
        }
        public int? ManagementServicePort
        {
            get { return managementServicePort;   }
            set { managementServicePort = value; }
        }
        public TestResult? ManagementServiceTestResult
        {
            get { return managementServiceTestResult; }
            set { managementServiceTestResult = value; }
        }
        public Exception ManagementServiceTestException
        {
            get { return managementServiceTestException; }
            set { managementServiceTestException = value; }
        }

    }

    [Serializable]
    public class NewsfeedServiceStatus
    {
        private string hostName;
        private Exception connectionException;

        private string repositoryHost;
        private string repositoryDatabase;
        private TestResult? repositoryTestResult;
        private Exception repositoryException;
    }

    public enum TestResult
    {
        Unknown,
        Failed,
        Passed,
        Info
    }

    [Serializable]
    public enum SQLdmServiceStatus
    {
        Running,
        Unknown
    }

}
