using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Objects.Replication
{
    #region Replication Component Classes
    public enum ReplicationType:int
    {
        Transaction = 0,
        Snapshot,
        Merge
    }
    public enum ReplicationRole : int
    {
        Publisher = 0,
        Distributor,
        Subscriber
    }
    [Serializable]
    public class publishedDB
    {
        private string _instance;
        private string _dbname;
        private string _distributor;
        private string _distributorDB;
        private string _publicationName;
        private string _publicationDescription;
        private long _publishedArticles;
        /// <summary>
        /// Delivered to the distributor and subscribed
        /// </summary>
        private long? _subscribedTrans;
        /// <summary>
        /// Delivered to the distributor and not subscribed
        /// </summary>
        private long? _nonSubscribedTrans;

        private ReplicationType _replicationType = ReplicationType.Transaction;

        private List<subscribedDB> _subscriptions = new List<subscribedDB>();

        public long PublishedArticles
        {
            get { return _publishedArticles; }
            set { _publishedArticles = value; }
        }

        public ReplicationType ReplicationType
        {
            get { return _replicationType; }
            set { _replicationType = value; }
        }

        public string FullPublisherName
        {
            get { return Instance + "." + DBName; }
        }

        public string Distributor
        {
            get { return _distributor; }
            set { _distributor = value; }
        }

        public string DistributorDB
        {
            get { return _distributorDB; }
            set { _distributorDB = value; }
        }

        public string PublicationName
        {
            get { return _publicationName; }
            set { _publicationName = value; }
        }

        public string PublicationDescription
        {
            get { return _publicationDescription; }
            set { _publicationDescription = value; }
        }

        public long? SubscribedTrans
        {
            get { return _subscribedTrans; }
            set { _subscribedTrans = value; }
        }
        
        public long? NonSubscribedTrans
        {
            get { return _nonSubscribedTrans; }
            set { _nonSubscribedTrans = value; }
        }

        public string DBName
        {
            get { return _dbname; }
            internal set { _dbname = value; }
        }
        
        public string Instance
        {
            get { return _instance; }
            internal set { _instance = value; }
        }

        public List<subscribedDB> Subscriptions
        {
            get { return _subscriptions; }
            set { _subscriptions = value; }
        }

    }
    [Serializable]
    public class subscribedDB
    {
        private string _instance;
        private string _dbname;
        private string _publisherInstance;
        private string _publisherdb;
        private string _publicationName;
        private int _replicationType;
        /// <summary>
        /// status of the subscription
        /// </summary>
        private int _subscriptionStatus;
        private DateTime _lastUpdated;
        private int _lastSyncStatus;
        private string _lastSyncSummary;
        private DateTime _lastSyncTime;
        /// <summary>
        /// Number of subscribed articles
        /// </summary>
        private int _articles;
        /// <summary>
        /// Type of initial synchronization
        /// </summary>
        private int _syncType;
        /// <summary>
        /// The type of the susbscription
        /// </summary>
        private int _subscriptionType;
        public string FullPublisherName
        {
            get { return PublisherInstance + "." + Publisherdb; }
        }
        /// <summary>
        /// Type of initial synchronization
        /// </summary>
        public int SyncType
        {
            get { return _syncType; }
            set { _syncType = value; }
        }
        /// <summary>
        /// The type of the susbscription
        /// </summary>
        public int SubscriptionType
        {
            get { return _subscriptionType; }
            set { _subscriptionType = value; }
        }

        /// <summary>
        /// Number of subscribed articles
        /// </summary>
        public int Articles
        {
            get { return _articles; }
            set { _articles = value; }
        }

        public string Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public string Dbname
        {
            get { return _dbname; }
            set { _dbname = value; }
        }

        public string PublisherInstance
        {
            get { return _publisherInstance; }
            set { _publisherInstance = value; }
        }

        public string Publisherdb
        {
            get { return _publisherdb; }
            set { _publisherdb = value; }
        }

        public string PublicationName
        {
            get { return _publicationName; }
            set { _publicationName = value; }
        }

        public int ReplicationType
        {
            get { return _replicationType; }
            set { _replicationType = value; }
        }

       
        public int SubscriptionStatus
        {
            get { return _subscriptionStatus; }
            set { _subscriptionStatus = value; }
        }

        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { _lastUpdated = value; }
        }

        public int LastSyncStatus
        {
            get { return _lastSyncStatus; }
            set { _lastSyncStatus = value; }
        }
        public string LastSyncSummary
        {
            get { return _lastSyncSummary; }
            set { _lastSyncSummary = value; }
        }


        public DateTime LastSyncTime
        {
            get { return _lastSyncTime; }
            set { _lastSyncTime = value; }
        }
    }
    
    [Serializable]
    public class cDistributor
    {
        private string _instance;
        private string _dbname;
        private string _snapshotDirectory;
        private string _account;
        private int _minDistributionRetention;
        private int _maxDistributionRetention;
        private int _historyRetention;
        private int _maxSubscriptionLatency;
        //private Dictionary<int, cPublication> _publications = new Dictionary<int, cPublication>();
        private Dictionary<int, publishedDB> distributedPublications = new Dictionary<int, publishedDB>();
        
        /// <summary>
        /// Distributor instance name
        /// </summary>
        public string Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        /// <summary>
        /// distributor database name
        /// </summary>
        public string Dbname
        {
            get { return _dbname; }
            set { _dbname = value; }
        }
        
        /// <summary>
        /// unc snapshot name
        /// </summary>
        public string SnapshotDirectory
        {
            get { return _snapshotDirectory; }
            set { _snapshotDirectory = value; }
        }

        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public int MinDistributionRetention
        {
            get { return _minDistributionRetention; }
            set { _minDistributionRetention = value; }
        }

        public int MaxDistributionRetention
        {
            get { return _maxDistributionRetention; }
            set { _maxDistributionRetention = value; }
        }

        public int HistoryRetention
        {
            get { return _historyRetention; }
            set { _historyRetention = value; }
        }

        public int MaxSubscriptionLatency
        {
            get { return _maxSubscriptionLatency; }
            set { _maxSubscriptionLatency = value; }
        }

        //public Dictionary<int, cPublication> Publications
        //{
        //    get { return _publications; }
        //    set { _publications = value; }
        //}
        /// <summary>
        /// Contains all of the publications utilizing this distributor 
        /// keyed on publisher instance and db
        /// </summary>
        public Dictionary<int, publishedDB> DistributedPublications
        {
            get { return distributedPublications; }
            set { distributedPublications = value; }
        }

    }
    [Serializable]
    public class cPublisher
    {
        private string _instanceName;
        /// <summary>
        /// Number of transactions in the log awaiting delivery to the distribution database - Non distributed.
        /// </summary>
        private int _replicatedTrans;
        /// <summary>
        /// Average number of transactions per second delivered to the distribution database.
        /// </summary>
        private double _replicatedTranRate;
        /// <summary>
        /// Average time, in seconds, that transactions were in the log before being distributed. Distribution Latency
        /// </summary>
        private double _replicationLatency;

        private Dictionary<int, publishedDB> publishedDatabases = new Dictionary<int, publishedDB>();
        private cDistributor _distributor;

        /// <summary>
        /// Number of transactions in the log awaiting delivery to the distribution database - Non distributed.
        /// </summary>
        public int ReplicatedTrans
        {
            get { return _replicatedTrans; }
            set { _replicatedTrans = value; }
        }

        public double ReplicatedTranRate
        {
            get { return _replicatedTranRate; }
            set { _replicatedTranRate = value; }
        }

        public double ReplicationLatency
        {
            get { return _replicationLatency; }
            set { _replicationLatency = value; }
        }

        public Dictionary<int, publishedDB> PublishedDatabases
        {
            get { return publishedDatabases; }
            set { publishedDatabases = value; }
        }

        /// <summary>
        /// Distributor that is associated with this publisher
        /// </summary>
        public cDistributor Distributor
        {
            get { return _distributor; }
            set { _distributor = value; }
        }
        /// <summary>
        /// Name of the publisher instance
        /// </summary>
        public string InstanceName
        {
            get { return _instanceName; }
            set { _instanceName = value; }
        }
    }
    [Serializable]
    public class cPublication
    {
        private string _publisherInstance;
        private string _publisherDB;
        private string _publicationName;
        private Dictionary<int, publishedDB> distributedPublications = new Dictionary<int, publishedDB>();

        public string PublisherInstance
        {
            get { return _publisherInstance; }
            set { _publisherInstance = value; }
        }

        public string PublisherDatabase
        {
            get { return _publisherDB; }
            set { _publisherDB = value; }
        }

        public string PublicationName
        {
            get { return _publicationName; }
            set { _publicationName = value; }
        }
        
        /// <summary>
        /// Contains all of the publications utilizing this distributor 
        /// keyed on publisher instance and db
        /// </summary>
        public Dictionary<int, publishedDB> DistributedPublications
        {
            get { return distributedPublications; }
            set { distributedPublications = value; }
        }
    }

    [Serializable]
    public class ReplicationTrendDataSample
    {
        private DateTime? _sampleTime;
        private long? _nonDistributedCount;
        private double? _nonDistributedLatency;
        private long? _nonSubscribedCount;
        private long? _subscribedCount;
        private double? _nonSubscribedLatency;

        public ReplicationTrendDataSample(DateTime? sampleTime,
            double? nonSubscribedLatency,
            double? nonDistributedLatency,
            long? subscribedCount,
            long? nonSubscribedCount,
            long? nonDistributedCount)
        {
            _nonDistributedCount = nonDistributedCount;
            _nonDistributedLatency = nonDistributedLatency;
            _nonSubscribedCount = nonSubscribedCount;
            _subscribedCount = subscribedCount;
            _nonSubscribedLatency = nonSubscribedLatency;
            _sampleTime = sampleTime;
        }

        public DateTime? SampleTime
        {
          get { return _sampleTime; }
          set { _sampleTime = value; }
        }
                
        public long? NonDistributedCount
        {
          get { return _nonDistributedCount; }
          set { _nonDistributedCount = value; }
        }
                
        public double? NonDistributedLatency
        {
          get { return _nonDistributedLatency; }
          set { _nonDistributedLatency = value; }
        }
                
        public long? NonSubscribedCount
        {
          get { return _nonSubscribedCount; }
          set { _nonSubscribedCount = value; }
        }
                
        public long? SubscribedCount
        {
          get { return _subscribedCount; }
          set { _subscribedCount = value; }
        }
        

        public double? NonSubscribedLatency
        {
          get { return _nonSubscribedLatency; }
          set { _nonSubscribedLatency = value; }
        }
    }
    #endregion

    #region Session Data class for Desktop Client
    [Serializable]
    public class ReplicationSession
    {
        private int articleCount;
        private string publisherInstance;
        private string publisherDB;
        private string distributorInstance;
        private string distributorDB;
        private string subscriberInstance;
        private string subscriberDB;
	    private DateTime lastSnapshotDateTime;
        private int subscribedTransactions;
        private int nonSubscribedTransactions;
        private int nonDistributedTransactions; 
        private double replicationLatency;
        private long maxSubscriptionLatency; 
        private int replicationType; 
        private int subscriptionType; 
        private DateTime lastSubscriberUpdate;
        private int lastSyncStatus;
        private string lastSyncSummary;
        private DateTime lastSyncTime;
        private int subscriptionStatus; 
        private string publication;
        private string publicationDescription;
        private int? publisherSQLServerID;
        private int? distributorSQLServerID;
        private int? subscriberSQLServerID;

        public int? PublisherSQLServerID
        {
            get { return publisherSQLServerID; }
            set { publisherSQLServerID = value; }
        }
        public int? DistributorSQLServerID
        {
            get { return distributorSQLServerID; }
            set { distributorSQLServerID = value; }
        }
        public int? SubscriberSQLServerID
        {
            get { return subscriberSQLServerID; }
            set { subscriberSQLServerID = value; }
        }

        public int SubscribedTransactions
        {
          get { return subscribedTransactions; }
          set { subscribedTransactions = value; }
        }
                

        public int NonSubscribedTransactions
        {
          get { return nonSubscribedTransactions; }
          set { nonSubscribedTransactions = value; }
        }
        	    

        public int NonDistributedTransactions
        {
          get { return nonDistributedTransactions; }
          set { nonDistributedTransactions = value; }
        }
                

        public double ReplicationLatency
        {
          get { return replicationLatency; }
          set { replicationLatency = value; }
        }
                

        public long MaxSubscriptionLatency
        {
          get { return maxSubscriptionLatency; }
          set { maxSubscriptionLatency = value; }
        }

        public int ReplicationType
        {
          get { return replicationType; }
          set { replicationType = value; }
        }
                
        public int SubscriptionType
        {
          get { return subscriptionType; }
          set { subscriptionType = value; }
        }
        	    

        public DateTime LastSubscriberUpdate
        {
          get { return lastSubscriberUpdate; }
          set { lastSubscriberUpdate = value; }
        }
        
        public int LastSyncStatus
        {
          get { return lastSyncStatus; }
          set { lastSyncStatus = value; }
        }
                

        public string LastSyncSummary
        {
          get { return lastSyncSummary; }
          set { lastSyncSummary = value; }
        }


        public DateTime LastSyncTime
        {
          get { return lastSyncTime; }
          set { lastSyncTime = value; }
        }

        public int SubscriptionStatus
        {
          get { return subscriptionStatus; }
          set { subscriptionStatus = value; }
        }
                

        public string Publication
        {
          get { return publication; }
          set { publication = value; }
        }

        public string PublicationDescription
        {
          get { return publicationDescription; }
          set { publicationDescription = value; }
        }

        public int ArticleCount
        {
          get { return articleCount; }
          set { articleCount = value; }
        }
        /// <summary>
        /// instance.db
        /// </summary>
        public string PublisherInstanceAndDB
        {
            get
            {
                if (string.IsNullOrEmpty(publisherInstance) || string.IsNullOrEmpty(publisherDB))
                    return "N\\A";
                if(publisherInstance.Length > 0 && publisherDB.Length > 0)
                    return PublisherInstance + "." + PublisherDB;
                return "";
            }
        }
        public string PublisherInstance
        {
          get { return publisherInstance; }
          set { publisherInstance = value; }
        }

        public string PublisherDB
        {
          get { return publisherDB; }
          set { publisherDB = value; }
        }
        /// <summary>
        /// instance.db
        /// </summary>
        public string DistributorInstanceAndDB
        {
            get
            {
                if (string.IsNullOrEmpty(distributorInstance) || string.IsNullOrEmpty(distributorDB))
                    return "N\\A";
                if (distributorInstance.Length > 0 && distributorDB.Length > 0)
                    return distributorInstance + "." + distributorDB;
                return "";
            }
        }

        public string DistributorInstance
        {
          get { return distributorInstance; }
          set { distributorInstance = value; }
        }
        
        public string DistributorDB
        {
          get { return distributorDB; }
          set { distributorDB = value; }
        }
        /// <summary>
        /// instance.db
        /// </summary>
        public string SubscriberInstanceAndDB
        {
            get
            {
                if (subscriberInstance.Length > 0 && subscriberDB.Length > 0)
                    return subscriberInstance + "." + subscriberDB;
                return "N\\A";
            }
        }

        public string SubscriberInstance
        {
          get { return subscriberInstance; }
          set { subscriberInstance = value; }
        }
        
        public string SubscriberDB
        {
          get { return subscriberDB; }
          set { subscriberDB = value; }
        }

        public DateTime LastSnapshotDateTime
        {
          get { return lastSnapshotDateTime; }
          set { lastSnapshotDateTime = value; }
        }
    }
    #endregion
    #region fields
    #endregion

}
