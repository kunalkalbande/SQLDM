//------------------------------------------------------------------------------
// <copyright file="MetricDefinition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Globalization;

namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;
    using BBS.TracerX;
    using Messages;
    using Microsoft.ApplicationBlocks.Data;
    using Objects;
    using Snapshots;
    using Wintellect.PowerCollections;
    //using Idera.SQLsafe.Shared.Service.Backup;
    using Vim25Api;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Static class that holds a shared instance of the MetricDefinitions class.  This allows the MS to 
    /// load a copy of the metric definitions that can also be used by the notification classes that are 
    /// defined in the common assembly.
    /// </summary>
    public static class SharedMetricDefinitions
    {
        private static MetricDefinitions metricDefinitions = null;
        private static readonly object sync = new object();

        public static MetricDefinitions CreateSharedInstance(string connectionString, bool loadCounterDefinitions, bool loadMessageTemplates, bool loadMetricDescriptions, bool nonAzure, int? cloudProviderId)
        {
            lock (sync)
            {
                metricDefinitions = new MetricDefinitions(loadCounterDefinitions, loadMessageTemplates, loadMetricDescriptions, nonAzure, cloudProviderId);
                metricDefinitions.Load(connectionString, false);
                return metricDefinitions;
            }
        }

        public static MetricDefinitions Load(string connectionString)
        {
            lock (sync)
            {
                metricDefinitions.Load(connectionString,false);
                return metricDefinitions;
            }
        }

        public static MetricDefinitions MetricDefinitions
        {
            get { lock(sync) { return metricDefinitions; } }
            set { lock (sync) { metricDefinitions = value; } }
        }
    }

    public class MetricDefinitionChangedEventArgs
    {
        public readonly int MetricID;
        public readonly ChangeType Change;
        public readonly object ChangedObject;
    
        internal MetricDefinitionChangedEventArgs(ChangeType change, int metricID, object changedObject)
        {
            this.Change = change;
            this.ChangedObject = changedObject;
            this.MetricID = metricID;
        }
        public enum ChangeType
        {
            Added,
            Changed,
            Deleted,
            Cleared
        }
    }

    public delegate void MetricDefinitionChangedHandler(MetricDefinitionChangedEventArgs args);

    [Serializable]
    public class MetricDefinitions : ISerializable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MetricDefinitions");

        private const int FALSE = 0;
        private const int TRUE = 1;
        private readonly bool loadCounterDefinitions;
        private readonly bool loadMessageTemplates;
        private readonly bool loadMetricDescriptions;
        private readonly bool nonAzureOnly;
        private readonly int? cloudProviderId;
        private DateTime lastKnownChangeDateTimeUTC;

        private readonly Dictionary<int, MetricDefinition> metricDefinitions;
        private readonly Dictionary<int, CustomCounterDefinition> counterDefinitions;
        private readonly Dictionary<int, MessageMap> messageDefintions;
        private readonly Dictionary<int, MetricDescription> metricDescriptions;
        private string lastConnectionString;
        private event MetricDefinitionChangedHandler MetricDefinitionChanged;

        [NonSerialized]
        private int loading;
        [NonSerialized]
        private bool doNotifications;
        [NonSerialized]
        private readonly object sync;
        [NonSerialized]
        private ReaderWriterLock rwLock;

        private MetricDefinitions()
        {
            // initialize nonserialized fields
            loading = FALSE;
            doNotifications = false;
            sync = new object();
            rwLock = new ReaderWriterLock();
        }

        public MetricDefinitions(bool loadCounterDefinitions, bool loadMessageTemplates, bool loadMetricDescriptions, bool nonAzureOnly = false, int? cloudProviderId = null) : this()
        {
            lastKnownChangeDateTimeUTC = DateTime.MinValue;
            metricDefinitions = new Dictionary<int,MetricDefinition>();
            counterDefinitions = new Dictionary<int,CustomCounterDefinition>();
            messageDefintions = new Dictionary<int, MessageMap>();
            metricDescriptions = new Dictionary<int, MetricDescription>();
            lastConnectionString = String.Empty;
            
            this.cloudProviderId = cloudProviderId;
            this.loadCounterDefinitions = loadCounterDefinitions;
            this.loadMessageTemplates = loadMessageTemplates;
            this.loadMetricDescriptions = loadMetricDescriptions;
            this.nonAzureOnly = nonAzureOnly;
        }

        public MetricDefinitions(SerializationInfo info, StreamingContext context) : this()
        {
            loadCounterDefinitions = info.GetBoolean("loadCounterDefinitions");
            loadMessageTemplates = info.GetBoolean("loadMessageTemplates");
            loadMetricDescriptions = info.GetBoolean("loadMetricDescriptions");
            lastKnownChangeDateTimeUTC = info.GetDateTime("lastKnownChangeDateTimeUTC");
            lastConnectionString = info.GetString("lastConnectionString");
            metricDefinitions = (Dictionary<int, MetricDefinition>)info.GetValue("metricDefinitions", typeof(Dictionary<int, MetricDefinition>));
            counterDefinitions = (Dictionary<int, CustomCounterDefinition>)info.GetValue("counterDefinitions", typeof(Dictionary<int, CustomCounterDefinition>));
            messageDefintions = (Dictionary<int, MessageMap>)info.GetValue("messageDefintions", typeof(Dictionary<int, MessageMap>));
            metricDescriptions = (Dictionary<int, MetricDescription>)info.GetValue("metricDescriptions", typeof(Dictionary<int, MetricDescription>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("loadCounterDefinitions", loadCounterDefinitions);
            info.AddValue("loadMessageTemplates", loadMessageTemplates);
            info.AddValue("loadMetricDescriptions", loadMetricDescriptions);
            info.AddValue("lastKnownChangeDateTimeUTC", lastKnownChangeDateTimeUTC);
            info.AddValue("lastConnectionString", lastConnectionString);
            info.AddValue("metricDefinitions", metricDefinitions);
            info.AddValue("counterDefinitions", counterDefinitions);
            info.AddValue("messageDefintions", messageDefintions);
            info.AddValue("metricDescriptions", metricDescriptions);
        }

        protected void OnMetricDefinitionChanged(MetricDefinitionChangedEventArgs.ChangeType changeType, int metricID, object changedObject)
        {
            if (!doNotifications)
                return;

            if (MetricDefinitionChanged != null)
            {
                try
                {
                    MetricDefinitionChanged(new MetricDefinitionChangedEventArgs(changeType, metricID, changedObject));
                }
                catch
                {
                    /* */
                }
            }
        }

        public void Reload(string connectionString)
        {
            if (Interlocked.CompareExchange(ref loading, TRUE, FALSE) == FALSE)
            {
                try
                {
                    if (!connectionString.Equals(lastConnectionString))
                    {
                        Clear();
                        InternalLoad(connectionString, null, false);
                    }
                    else
                    {
                        object p = (lastKnownChangeDateTimeUTC > DateTime.MinValue)
                                       ? (object)lastKnownChangeDateTimeUTC
                                       : null;
                        using (
                            SqlDataReader reader =
                                SqlHelper.ExecuteReader(connectionString, "p_GetCounters", null, loadMetricDescriptions,
                                                        loadMessageTemplates, loadCounterDefinitions, false,
                                                        p, nonAzureOnly, cloudProviderId))
                        {
                            Load(reader);
                        }
                        lastConnectionString = connectionString;
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref loading, FALSE);
                }
            }
            else
                LOG.Info("Skipped reload");
        }

        public void Load(string connectionString) 
        {
            Load(connectionString, false);
        }

        public void Load(string connectionString, bool customCountersOnly)
        {
            if (Interlocked.CompareExchange(ref loading, TRUE, FALSE) == FALSE)
            {
                try
                {
                    InternalLoad(connectionString, null, customCountersOnly);
                    lastConnectionString = connectionString;
                } finally
                {
                    Interlocked.Exchange(ref loading, FALSE);
                }
            }
            else
                LOG.Info("Skipped load");
        }

        public void Load(string connectionString, int metricID)
        {
            if (Interlocked.CompareExchange(ref loading, TRUE, FALSE) == FALSE)
            {
                try
                {
                    InternalLoad(connectionString,metricID,false);
                    lastConnectionString = connectionString;
                }
                finally
                {
                    Interlocked.Exchange(ref loading, FALSE);
                }
            }
            else
                LOG.Info("Skipped load");
        }

        private void InternalLoad(string connectionString, int? metricID, bool customCountersOnly)
        {
            object metric = metricID.HasValue ? (object)metricID.Value : null;
            using (SqlDataReader reader =
                SqlHelper.ExecuteReader(connectionString, "p_GetCounters", metric, loadMetricDescriptions,
                                loadMessageTemplates, loadCounterDefinitions, customCountersOnly,
                                null, nonAzureOnly, cloudProviderId))
            {
                Load(reader);
            }
        }

        private void Load(SqlDataReader reader)
        {
            bool messagesLoaded = false;
            bool messageMapLoaded = false;
            bool countersLoaded = false;
           
            LoadMetaData(reader);
            while (reader.NextResult())
            {
                if (loadMessageTemplates && !messagesLoaded)
                {
                    LoadMessages(reader);
                    messagesLoaded = true;
                }
                else
                    if (loadMessageTemplates && !messageMapLoaded)
                    {
                        LoadMessageMap(reader);
                        messageMapLoaded = true;
                    }
                    else
                        if (loadCounterDefinitions && !countersLoaded)
                        {
                            LoadCustomCounters(reader);
                            countersLoaded = true;
                        }
            }
        }

        public void RemoveCounter(int metricID)
        {
            lock (sync)
            {
                if (counterDefinitions.ContainsKey(metricID))
                    counterDefinitions.Remove(metricID);
                if (metricDefinitions.ContainsKey(metricID))
                    metricDefinitions.Remove(metricID);
                if (metricDescriptions.ContainsKey(metricID))
                    metricDescriptions.Remove(metricID);
            }
        }

        private void LoadMetaData(SqlDataReader reader)
        {
            DateTime lastChangeDateTime;
            lock(sync)
            {
                lastChangeDateTime = lastKnownChangeDateTimeUTC;
            }

            while (reader.Read())
            {
                int metricID = reader.GetInt32(0);
                MetricDefinition definition = new MetricDefinition(reader);
                bool replaced = false;
                if (metricDefinitions.ContainsKey(metricID))
                {
                    metricDefinitions.Remove(metricID);
                    replaced = true;
                }
                metricDefinitions.Add(metricID, definition);
                //messageDefintions.Add(metricID, definition.Rank);

                if (definition.LastChanged > lastChangeDateTime)
                {
                    lastChangeDateTime = definition.LastChanged;
                }
                if (definition.LastChanged > lastKnownChangeDateTimeUTC)
                    lastKnownChangeDateTimeUTC = definition.LastChanged;

                if (loadMetricDescriptions)
                    LoadMetricDescriptions(reader);

                OnMetricDefinitionChanged(
                       replaced
                          ? MetricDefinitionChangedEventArgs.ChangeType.Changed
                          : MetricDefinitionChangedEventArgs.ChangeType.Added,
                       metricID,
                       definition);
            }

            lock (sync)
            {
                if (lastChangeDateTime > lastKnownChangeDateTimeUTC)
                    lastKnownChangeDateTimeUTC = lastChangeDateTime;
            }
        }

        private void LoadMetricDescriptions(IDataRecord record)
        {
            int metricID = (int)record[0];

            MetricDescription description = new MetricDescription(record);
            if (metricDescriptions.ContainsKey(metricID))
            {
                metricDescriptions.Remove(metricID);
            }

            metricDescriptions.Add(metricID, description);
        }

        private void LoadMessages(SqlDataReader reader)
        {
            int lastMetricID = -1;
            MessageMap messageMap = null;
            MetricDefinition definition = null;

            while (reader.Read())
            {
                int metricID = (int)reader[0];
                if (lastMetricID != metricID)
                {
                    lastMetricID = metricID;

                    // get the default message id from the metric definition 
                    int defaultMessageID = 0;
                    if (metricDefinitions.TryGetValue(metricID, out definition))
                    {
                        defaultMessageID = definition.DefaultMessageID;
                    }

                    messageMap = new MessageMap(defaultMessageID);
                    if (messageDefintions.ContainsKey(metricID))
                        messageDefintions.Remove(metricID);
                    messageDefintions.Add(metricID, messageMap);
                }
                if (messageMap != null)
                {
                    int messageID = (int) reader["MessageID"];
                    Message message = new Message(reader);
                    messageMap.AddMessage(messageID, message);
                }
            }
        }

        public void LoadMessageMap(SqlDataReader reader)
        {
            int loadedMetricID = -1;
            MessageMap messageMap = null;
            while (reader.Read())
            {
                int metricID = (int)reader["Metric"];
                if (loadedMetricID != metricID)
                {
                    loadedMetricID = metricID;
                    if (!messageDefintions.TryGetValue(metricID, out messageMap))
                        messageMap = null;
                }
                if (messageMap != null)
                    messageMap.MapMessage((int)reader["MessageID"], reader["Value"]);
            }
        }

        private void LoadCustomCounters(SqlDataReader reader)
        {
            DateTime lastChangeDateTime;
            lock (sync)
            {
                lastChangeDateTime = lastKnownChangeDateTimeUTC;
            }

            while (reader.Read())
            {
                int metricID = (int) reader[0];
                bool replaced = false;
                CustomCounterDefinition counter = new CustomCounterDefinition(reader);
                if (counterDefinitions.ContainsKey(metricID))
                {
                    counterDefinitions.Remove(metricID);
                    replaced = true;
                }
                counterDefinitions.Add(metricID, counter);

                OnMetricDefinitionChanged(
                    replaced
                    ? MetricDefinitionChangedEventArgs.ChangeType.Changed
                    : MetricDefinitionChangedEventArgs.ChangeType.Added,
                    metricID,
                    counter);

                if (counter.LastChanged > lastChangeDateTime)
                    lastChangeDateTime = counter.LastChanged;
            }

            lock (sync)
            {
                if (lastChangeDateTime > lastKnownChangeDateTimeUTC)
                    lastKnownChangeDateTimeUTC = lastChangeDateTime;
            }
        }

        public void SetNotificationsEnabled(bool enabled)
        {
            lock (sync)
            {
                doNotifications = enabled;
            }
        }

        public void Clear()
        {
            lock(sync)
            {
                lastKnownChangeDateTimeUTC = DateTime.MinValue;
                metricDefinitions.Clear();
                metricDescriptions.Clear();
                counterDefinitions.Clear();
            }
            OnMetricDefinitionChanged(MetricDefinitionChangedEventArgs.ChangeType.Cleared, -1, null);
        }

        public MetricDefinition GetMetricDefinition(int metricID)
        {
            MetricDefinition result = null;
            lock (sync)
            {
                metricDefinitions.TryGetValue(metricID, out result);
            }
            return result;
        }

        public int[] GetMetricDefinitionKeys()
        {
            lock (sync)
            {
                return Algorithms.ToArray<int>(metricDefinitions.Keys);
            }
        }

        public CustomCounterDefinition GetCounterDefinition(int metricID)
        {
            CustomCounterDefinition result = null;
            lock (sync)
            {
                counterDefinitions.TryGetValue(metricID, out result);
            }
            return result;
        }

        public int[] GetCounterDefinitionKeys()
        {
            lock (sync)
            {
                return Algorithms.ToArray<int>(counterDefinitions.Keys);
            }
        }

        public MessageMap GetMessages(int metricID)
        {
            // Custom counters have their own unique metricID but all have Metric.Custom as their metric.
            // The logic for finding messages is to first check for a messages object using metric id and 
            // if not found then check using the Metric.  This will allow for poking in a better message 
            // template for specific custom counters (not supported in the ui) but if not found 
            // we have a generic one.
            MessageMap result = null;
            MetricDefinition definition = null;
            lock (sync)
            {
                if (!messageDefintions.TryGetValue(metricID, out result))
                {
                    if (metricDefinitions.TryGetValue(metricID, out definition))
                    {
                        Metric metric = definition.Metric;
                        if (metricID != (int)metric)
                        {
                            messageDefintions.TryGetValue((int)metric, out result);
                        }
                    }   
                }
            }
            return result;
        }

        public int[] GetMessageDefinitionKeys()
        {
            lock (sync)
            {
                return Algorithms.ToArray<int>(messageDefintions.Keys);
            }
        }

        public MetricDescription? GetMetricDescription(int metricID)
        {
            lock (sync)
            {
                if (metricDescriptions.ContainsKey(metricID))
                    return metricDescriptions[metricID];
            }
            return null;
        }

        public int[] GetMetricDescriptionKeys()
        {
            lock (sync)
            {
                return Algorithms.ToArray<int>(metricDescriptions.Keys);
            }
        }

        public void SetMetricComments(int metricID, string comments,int rank)
        {
            lock (sync)
            {
                MetricDescription description;
                if (metricDescriptions.TryGetValue(metricID, out description))
                {
                    description.Comments = comments;
                    description.Rank = rank;
                    metricDescriptions.Remove(metricID);
                    metricDescriptions.Add(metricID, description);
                }
            }
        }

        public void AddCounter(MetricDefinition metricDefinition, MetricDescription? metricDescription, CustomCounterDefinition counterDefinition)
        {
            bool replaced = false;
            int metricID = metricDefinition.MetricID;
            lock (sync)
            {
                if (metricDefinitions.ContainsKey(metricID))
                {
                    metricDefinitions.Remove(metricID);
                    replaced = true;
                }
                metricDefinitions.Add(metricID, metricDefinition);

                if (metricDescription != null)
                {
                    if (metricDescriptions.ContainsKey(metricID))
                    {
                        metricDescriptions.Remove(metricID);
                        replaced = true;
                    }
                    metricDescriptions.Add(metricID, metricDescription.Value);
                }

                if (counterDefinitions.ContainsKey(metricID))
                {
                    counterDefinitions.Remove(metricID);
                    replaced = true;
                }
                counterDefinitions.Add(metricID, counterDefinition);
            }
            OnMetricDefinitionChanged(
                replaced 
                ? MetricDefinitionChangedEventArgs.ChangeType.Changed 
                : MetricDefinitionChangedEventArgs.ChangeType.Added, 
                metricID,
                new object[] { metricDefinition, metricDescription, counterDefinition });
        }

    }

    [Serializable]
    public class MetricDefinition
    {
        #region fields

        public static readonly Message DefaultMessageTemplate = new Message(0, "header", "body", "todo", "pulse", "header_baseline", "body_baseline", "todo_baseline", "pulse_baseline");//SQLdm 10.0 (Tarun Sapra)- Alert msg when baseline alerts are enabled
        public object sync = new object();
        
        private int metricID;
        private bool deleted;
        private DateTime lastChanged;
        private ThresholdOptions options;
        private MetricClass metricClass;
        private ComparisonType comparisonType;
        private int defaultMessageID;
        private int rank;
        private bool processNotifications;
        private bool alertEnabledByDefault;
        private uint eventCategory;
        private string name; //SQLDM 8.5 Mahesh: New field required for Rest Service
        private string description; //SQLDM 8.5 Mahesh: New field required for Rest Service
        private MetricCategory metricCategory; //SQLDM 8.5 Mahesh: New field required for Rest Service
        private int minValue;
        private long maxValue;
        private long maxValueBaseline;
        private long defaultInfoThreshold;
        private long defaultWarningThreshold;
        private long defaultCriticalThreshold;
        private long defaultInfoThresholdBaseline;
        private long defaultWarningThresholdBaseline;
        private long defaultCriticalThresholdBaseline;

        private Type valueType;

        #endregion

        public MetricDefinition(int metricID)
        {
            this.metricID = metricID;
            processNotifications = true;
            options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue;
            valueType = typeof (object);
            lastChanged = DateTime.MinValue;
        }

        internal MetricDefinition(IDataRecord record)
        {
            metricID = (int) record["Metric"];
            deleted = (bool) record["Deleted"];
            lastChanged = (DateTime) record["UTCLastChangeDateTime"];
            metricClass = (MetricClass)Enum.ToObject(typeof (MetricClass), (int)record["Class"]);
            options = (ThresholdOptions)Enum.ToObject(typeof(ThresholdOptions), (int)record["Flags"]);
            minValue = (int) record["MinValue"];
            maxValue = (long) record["MaxValue"];
            defaultInfoThreshold = (long)record["DefaultInfoValue"];
            defaultWarningThreshold = (long) record["DefaultWarningValue"];
            defaultCriticalThreshold = (long)record["DefaultCriticalValue"];
            processNotifications = (bool) record["DoNotifications"];
            eventCategory = Convert.ToUInt32(record["EventCategory"]);
            defaultMessageID = (int) record["DefaultMessageID"];
            alertEnabledByDefault = (bool) record["AlertEnabledDefault"];
            comparisonType = (ComparisonType) Enum.ToObject(typeof (ComparisonType), (int)record["ValueComparison"]);
            //10.0 SQLdm Srishti purohit
            //Data for baseline alert
            maxValueBaseline = (long)record["BaselineMaxValue"];
            defaultInfoThresholdBaseline = (long)record["BaselineDefaultInfoValue"];
            defaultWarningThresholdBaseline = (long)record["BaselineDefaultWarningValue"];
            defaultCriticalThresholdBaseline = (long)record["BaselineDefaultCriticalValue"];

            //before to parse we need to be sure the category is in the MetricCategory enum to avoid any exception 
            string categoyMetricName = record["Category"].ToString().Replace(' ', '_');
            if (Enum.IsDefined(typeof(MetricCategory), categoyMetricName))
                metricCategory = (MetricCategory)Enum.Parse(typeof(MetricCategory), categoyMetricName);
            else
                metricCategory = MetricCategory.All;

            name = (string)record["Name"];
            description = (string)record["Description"];
            valueType = Type.GetType(record["ValueType"].ToString());
            rank = (int)record["Rank"];
        }

        #region Properties

        public bool AlertEnabledByDefault
        {
            get { return alertEnabledByDefault; }
            set { alertEnabledByDefault = value; }
        }

        public ComparisonType ComparisonType
        {
            get { return comparisonType; }
            set { comparisonType = value; }
        }

        public int DefaultMessageID
        {
            get { return defaultMessageID; }
            set { defaultMessageID = value; }
        }

        public bool IsDeleted
        {
            get { return deleted; }
            set { deleted = value; }
        }

        //public static bool IsDatabaseAlert(Metric metric)
        //{
        //    switch (metric)
        //    {
        //        case Metric.DatabaseStatus:
        //        case Metric.DatabaseSizePct:
        //        case Metric.TransLogSize:
        //        case Metric.ReorganisationPct:
        //        case Metric.DataFileAutogrow:
        //        case Metric.LogFileAutogrow:
        //        case Metric.DatabaseSizeMb:
        //        case Metric.TransLogSizeMb:
        //            return true;
        //    }
        //        return false;
        //}

        public bool HasAdditionalData
        {
            get { return (Options & ThresholdOptions.AdditionalData) == ThresholdOptions.AdditionalData; }
        }

        public bool IsConfigurable
        {
            get
            {
                return (Options & ThresholdOptions.MutuallyExclusive) != ThresholdOptions.MutuallyExclusive;

//                if ((Options & ThresholdOptions.WarningDisabled) == ThresholdOptions.WarningDisabled)
//                {
//                    if ((Options & ThresholdOptions.CriticalDisabled) == ThresholdOptions.CriticalDisabled)
//                        return false;
//                }
//                return true;
            }
        }

        public bool IsCustom
        {
            get { return Metric == Metric.Custom; }
        }

        public Metric Metric
        {
            get
            {
                return GetMetric(metricID);
            }
        }

        public static Metric GetMetric(int metricID)
        {
            try
            {
                if (Enum.IsDefined(typeof(Metric), metricID))
                    return (Metric)Enum.ToObject(typeof(Metric), metricID);
            }
            catch
            {
                /* */
            }
            return Metric.Custom;
        }

        public DateTime LastChanged
        {
            get { return lastChanged; }
        }

        public int MetricID
        {
            get { return metricID;  }
            set { metricID = value; }
        }

        public MetricClass MetricClass
        {
            get { return metricClass;  }
            set { metricClass = value; }
        }

        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        
        public long MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        //10.0 SQLdm srishti purohit -- baseline support
        public long MaxValueBaseline
        {
            get { return maxValueBaseline; }
            set { maxValueBaseline = value; }
        }
        public long DefaultInfoThresholdValue
        {
            get { return defaultInfoThreshold; }
            set { defaultInfoThreshold = value; }
        }

        public long DefaultWarningThresholdValue
        {
            get { return defaultWarningThreshold; }
            set { defaultWarningThreshold = value; }
        }

        public long DefaultCriticalThresholdValue
        {
            get { return defaultCriticalThreshold; }
            set { defaultCriticalThreshold = value; }
        }
        //10.0 SQLdm srishti purohit -- baseline support
        public long DefaultInfoThresholdValueBaseline
        {
            get { return defaultInfoThresholdBaseline; }
            set { defaultInfoThresholdBaseline = value; }
        }

        public long DefaultWarningThresholdValueBaseline
        {
            get { return defaultWarningThresholdBaseline; }
            set { defaultWarningThresholdBaseline = value; }
        }

        public long DefaultCriticalThresholdValueBaseline
        {
            get { return defaultCriticalThresholdBaseline; }
            set { defaultCriticalThresholdBaseline = value; }
        }

        public ThresholdOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public bool ProcessNotifications
        {
            get { return processNotifications; }
            set { processNotifications = value; }
        }

        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        public Type ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public MetricCategory MetricCategory
        {
            get
            {
                if (Enum.IsDefined(typeof(MetricCategory), metricCategory))
                    return (MetricCategory)Enum.ToObject(typeof(MetricCategory), metricCategory);
                return MetricCategory.All;
            }
        }

        public Category EventCategory
        {
            get
            {
                if (Enum.IsDefined(typeof(Category), eventCategory))
                    return (Category)Enum.ToObject(typeof(Category), eventCategory);
                return Category.General;
            }
        }

        #endregion

        #region Methods

        public virtual decimal GetVisualUpperBound(decimal value)
        {
            if ((Options & ThresholdOptions.CalculateMaxValue) != ThresholdOptions.CalculateMaxValue)
                return MaxValue;

            decimal ub = 1.2m * value;

            if (ub < 20)
                ub = MinValue + 20;

            if (ub > MaxValue)
                return MaxValue;

            return Math.Floor(ub);
        }

        public void SetEventCategory(Category cat)
        {
            this.eventCategory = (uint) cat;
        }

        #endregion
    }

    #region Enums

    public enum MessageType
    {
        Header,
        Body,
        ToDo,
        Pulse
    }

    public enum MetricClass
    {
        ServerThreshold = 0,
        ScheduledJobs = 1,
        Processes = 2,
        Hardcoded = 3
    }

    public enum MetricCategory
    {
        All,
        Sessions,
        Queries,
        Resources,
        Databases,
        Services,
        Logs,
        Analyze,
        LaunchSWA,
        Operational,
        Replication,
        General,
        Custom,
        Virtualization,
        Neg_Custom,
        Backup //SQLdm 10.1 (GK): new category added in SQLdm 10.1 for last backup
    }

    public enum ComparisonType
    {
        [Description("Greater than or equal")]
        GE,
        [Description("Less than or equal")]
        LE
    }

    public enum CalculationType
    {
        Value,
        Delta
    }

    [Flags]
    public enum ThresholdOptions
    {
        NumericValue = 1,
        OKDisabled = 2,
        WarningDisabled = 4,
        CriticalDisabled = 8,
        ContainedValueList = 16,
        MutuallyExclusive = 32,
        AdditionalData = 64,
        CalculateMaxValue = 128,
        InfoDisabled = 256,
        MultiInstance = 512,
        SingleAlertConfig=1024 //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Added new threshol option for metrics which have only single alert enabled for each instance
    }

    [Serializable]
    public enum OptionStatus
    {
        Disabled = 0,
        Enabled = 1
    }

    [Serializable]
    public enum SQLdmServiceState
    {
        /// <summary>
        /// The service is running.
        /// </summary>
        Running,

        /// <summary>
        /// The service state is undetermined.
        /// </summary>
        Undetermined

    }

    /// <summary>
    /// Subset of the values collected for DatabaseStatus.  This is all that are exposed to the user.
    /// </summary>
    [Serializable]
    public enum DBStatus
    {
        /// <summary>
        /// Indicates that the database is ready for use.
        /// </summary>
        [Description("Normal")]
        Normal = 0,

        ///<summary>
        /// Indicates that the database status is undetermined
        /// Does not match sysdatabases
        /// </summary>
        [Description("Undetermined")]
        Undetermined = 4, // was -1

        ///<summary>
        /// Indicates that the database is a restoring mirror
        /// Does not match sysdatabases
        /// </summary>
        [Description("Restoring Mirror")]
        RestoringMirror = 8, // was 10

        ///<summary>
        /// Indicates that the database is in standby
        /// Does not match sysdatabases
        /// </summary>
        [Description("Standby")]
        Standby = 16, // was 5

        /// <summary>
        /// Indicates that the database is loading.
        /// </summary>
        [Description("Restoring")]
        Loading = 32,

        /// <summary>
        /// Indicates that the database is in a pre-recovery state.
        /// </summary>
        [Description("Recovery Pending")]
        PreRecovery = 64,

        /// <summary>
        /// Indicates that recovery is underway for the database.
        /// </summary>
        [Description("Recovering")]
        Recovering = 128,

        /// <summary>
        /// Indicates that the integrity of the database is suspect.
        /// </summary>
        [Description("Suspect")]
        Suspect = 256,

        /// <summary>
        /// Indicates that the database has been placed offline by a system or user action.
        /// </summary>
        [Description("Offline")]
        Offline = 512,

        ///<summary>    
        /// Indicates that the database is read-only
        /// </summary>
        [Description("Read Only")]
        ReadOnly = 1024,

        /// <summary>
        /// Indicates that the database is in DBO Use Only mode
        /// </summary>
        [Description("DBO Use Only")]
        DboUseOnly = 2048,

        /// <summary>
        /// Indicates that database is in single user mode
        /// </summary>
        [Description("Single User")]
        SingleUser = 4096,

        /// <summary>
        /// Indicates that the database is in a loading, offline, recovering,
        /// or suspect state.
        /// Does not match sysdatabases
        /// </summary>
        [Description("Inaccessible")]
        Inaccessible = 8192, // was 992

        /// <summary>
        /// Indicates that the secondary replica is 'Read-Intent only'
        /// </summary>
        [Description("Read-Intent only availability replica")]
        ReadOnlyAvailabilityReplica = 65536,

        /// <summary>
        /// Indicates the secondary replica is not readable.
        /// </summary>
        [Description("Unreadable availability replica")]
        UnreadableAvailabilityReplica = 131072,

        /// <summary>
        /// Indicates that emergency mode has been initiated on the database.
        /// </summary>
        [Description("Emergency Mode")]
        EmergencyMode = 32768,

        /// <summary>
        /// Indicates that the database was cleanly shutdown.
        /// </summary>
        [Description("Cleanly Shutdown")]
        CleanlyShutdown = 1073741824
    }

    [Serializable]
    public enum MetricType
    {
        [Description("Windows")]
        WMI,
        [Description("SQL Counter")]
        SQLCounter,
        [Description("SQL Script")]
        SQLStatement,
        [Description("VM Counter")]
        VMCounter,
        [Description("Azure Counter")]
        AzureCounter
    }

    #endregion

    #region MetricDescription

        [Serializable]
        public struct MetricDescription
        {
            public readonly string Name;
            public readonly string Category;
            public readonly string Description;
            private string comments;
            public int Rank; 

            internal MetricDescription(IDataRecord reader)
            {
                Name = reader["Name"] as string;
                if (Name != null)
                    Name = Name.TrimEnd();
                Category = reader["Category"] as string;
                if (Category != null)
                    Category = Category.TrimEnd();
                Description = reader["Description"] as string;
                if (Description != null)
                    Description = Description.TrimEnd();
                this.comments = reader["Comments"] as string;
                if (comments != null)
                    comments = comments.TrimEnd();
               
                this.Rank = Convert.ToInt32(reader["Rank"]);
                if(Rank!=null)
                Rank= Convert.ToInt32(reader["Rank"]);


        }

            public MetricDescription(string name, string category, string description, string comments,int rank)
            {
                Name = name;
                Category = category;
                Description = description;
              Rank = rank;
                this.comments = comments;
            }

            public string Comments
            {
                get { return comments;  }
                set { comments = value; }
            }
        
        }
        
    #endregion

    #region Messages

    [Serializable]
    public class MessageMap
    {
        private ListDictionary messages;
        private ListDictionary messageMap;
        private int defaultMessageID;

        static MessageMap()
        {
            
        }

        internal MessageMap() : this(0)
        {
        }

        internal MessageMap(int defaultMessageID)
        {
            messages = new ListDictionary();
            messageMap = null;
            this.defaultMessageID = defaultMessageID;
        }

        internal void AddMessage(int messageID, Message message)
        {
            if (messages.Contains(messageID))
                messages.Remove(messageID);
            messages.Add(messageID, message);
        }

        internal void MapMessage(int messageID, object value)
        {
            if (messageMap == null)
                messageMap = new ListDictionary();
            if (messageMap.Contains(value))
                messageMap.Remove(value);
            messageMap.Add(value, messageID);
        }

        public Message? GetMessage(int messageID)
        {
            if (messages.Contains(messageID))
                return (Message)messages[messageID];
            return null;
        }

        public Message? GetMessageForValue(object value)
        {
            if (value is Enum)
            {
                Type baseType = Enum.GetUnderlyingType(value.GetType());
                value = Convert.ChangeType(value, baseType);
            }
            // convert value to an int
            if (!(value is int))
            {
                try
                {
                    value = Convert.ChangeType(value, typeof (int));
                } catch (Exception)
                {
                    /* */
                }
            }
            if (messageMap == null || !messageMap.Contains(value))
                return null;

            int messageID = (int)messageMap[value];

            return GetMessage(messageID);
        }

        public string FormatMessage(Snapshot snapshot, IEvent baseEvent, MessageType messageType)//SQLdm 10.0 (Tarun Sapra)- Added a new param to highlight, which type of alert needs to be generated for the monitored instance
        {
            /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
            Message? message = GetMessageForValue(baseEvent.MetricValue);
            if (message == null)
            {
                object data = baseEvent.AdditionalData;
                if (data is Pair<CustomCounterSnapshot, MetricDescription?>)
                {
                    Pair<CustomCounterSnapshot, MetricDescription?> pcm = (Pair<CustomCounterSnapshot, MetricDescription?>)data;
                    if (pcm.First.CollectionFailed)
                    {
                        message = GetMessage(-1);
                        if (message == null)
                            message = GetMessage(defaultMessageID);
                    }
                    else
                        message = GetMessage(defaultMessageID);
                }
                else
                {
                    //SQLDM-21252 - Vamshi Krishna - Adding the condition to apply new message format for BlockingSession
                    if (data!= null && data is BlockingSession && (data as BlockingSession).Spid == 0)
                    {
                        message = GetMessage(1);
                        if (message == null)
                            message = GetMessage(defaultMessageID);
                    }
                    else
                    {
                        message = GetMessage(defaultMessageID);
                    }
                }
            }
            if (message.HasValue)
            {
                return message.Value.FormatMessage(snapshot, baseEvent, messageType);//SQLdm 10.0 (Tarun Sapra)- Baselined alerts
            }
            return null;
        }
    }

    [Serializable]
    public struct Message
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Message");

        public readonly long EventId;
        public readonly string HeaderTemplate;
        public readonly string BodyTemplate;
        public readonly string TodoTemplate;
        public readonly string PulseTemplate;
        //START:SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
        public readonly string HeaderTemplate_Baseline;
        public readonly string BodyTemplate_Baseline;
        public readonly string TodoTemplate_Baseline;
        public readonly string PulseTemplate_Baseline;
        //END:SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled

        internal Message(IDataRecord record)
        {
            EventId = (long) record["EventID"];
            HeaderTemplate = record["HeaderTemplate"] as string;
            BodyTemplate = record["BodyTemplate"] as string;
            TodoTemplate = record["TodoTemplate"] as string;
            PulseTemplate = record["PulseTemplate"] as string;
            //START: SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
            HeaderTemplate_Baseline = record["HeaderTemplate_Baseline"] as string;
            BodyTemplate_Baseline = record["BodyTemplate_Baseline"] as string;
            TodoTemplate_Baseline = record["TodoTemplate_Baseline"] as string;
            PulseTemplate_Baseline = record["PulseTemplate_Baseline"] as string;
            //END: SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
        }

        public Message(long eventId, string header, string body, string todo, string pulse, string header_baseline = null, string body_baseline = null, string todo_baseline = null, string pulse_baseline = null)//SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
        {
            EventId = eventId;
            HeaderTemplate = header;
            BodyTemplate = body;
            TodoTemplate = todo;
            PulseTemplate = pulse;
            //START: SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
            HeaderTemplate_Baseline = header_baseline;
            BodyTemplate_Baseline = body_baseline;
            TodoTemplate_Baseline = todo_baseline;
            PulseTemplate_Baseline = pulse_baseline;
            //END: SQLdm 10.0 (Tarun Sapra): Alert msg in case baseline alerts are enabled
        }

        public static string FormatMessage(Snapshot snapshot, IEvent baseEvent, string template)
        {
            try
            {
                // SQLDM-28167 Adjust $(AlertSummary) back to local time
                return LocalizeMessage(baseEvent, string.Format(template, GetMessageData(snapshot, baseEvent)));
            }
            catch (Exception)
            {
                LOG.ErrorFormat("Error formatting message metric={0} value={1} template={2}", baseEvent.MetricID, baseEvent.MetricValue, template ?? "(template is null)");
            }
            return template ?? "null template";
        }

        /// <summary>
        /// Localize the message by using the UTC time and replacing with the local time
        /// </summary>
        /// <param name="baseEvent"></param>
        /// <param name="formattedMessage"></param>
        /// <returns></returns>
        /// <remarks>
        /// SQLDM-28167 Adjust $(AlertSummary) back to local time
        /// </remarks>
        private static string LocalizeMessage(IEvent baseEvent, string formattedMessage)
        {
            var localizedFormattedMessage = formattedMessage;
            if ((Metric)baseEvent.MetricID == Metric.BlockingAlert)
            {
                var bs = baseEvent.AdditionalData as BlockingSession;

                //Convert the UTC date time to desktop client local time.
                var localDateTime = bs.BlockingStartTimeUTC.HasValue ? bs.BlockingStartTimeUTC.Value.ToLocalTime() : DateTime.Now;

                //Find all the occurrences of datetime text in messageBody of the alert and replace it with converted time in desktop client time zone.
                localizedFormattedMessage = Regex.Replace(localizedFormattedMessage, @"(?<=, since | issued at ).+?(?=\(UTC\))\(UTC\)", localDateTime.ToString("M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));

                //Find all the occurrences of datetime text in messageHeader of the alert and replace it with converted time in desktop client time zone.
                localizedFormattedMessage = Regex.Replace(localizedFormattedMessage, @"(?<=, since | issued at ).+?(?=\(UTC\))\(UTC\)", localDateTime.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));

            }

            return localizedFormattedMessage;
        }

        //START: SQLdm 10.0 (Tarun Sapra)- No msg change even if the alert is of type baseline
        public string FormatMessage(Snapshot snapshot, IEvent baseEvent, MessageType messageType)
        {
            string template = HeaderTemplate;
            switch (messageType)
            {
                case MessageType.Body:
                    template = BodyTemplate;                    
                    break;
                case MessageType.Pulse:
                    template = PulseTemplate;
                    break;
                case MessageType.ToDo:
                    template = TodoTemplate;
                    break;
            }
            return FormatMessage(snapshot, baseEvent, template);
        }
        

        public static object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
        {
            Metric metric = MetricDefinition.GetMetric(baseEvent.MetricID);
            return MessageFormatter.GetMessageData(metric, snapshot, baseEvent);
        }
    }


    #region Message Formatters

    public class MessageFormatter
    {
        protected static Logger LOG = Logger.GetLogger("MessageFormatter");
        private static Dictionary<string, MessageFormatter> messageFormatterMap = new Dictionary<string, MessageFormatter>();
        private static Type[] GetMessageData_Parms = new Type[] { typeof (Snapshot), typeof (BaseEvent) };
        private static MessageFormatter defaultFormatter = new MessageFormatter();
        static MessageFormatter()
        {
            foreach (Type type in typeof(MessageFormatter).GetNestedTypes())
            {
                if (type.IsAbstract)
                    continue;
                MessageFormatter formatter = (MessageFormatter)Activator.CreateInstance(type);
                messageFormatterMap.Add(type.Name, formatter);
            }
        }

        public static object[] DUMMY_SUBST_DATA = new object[]
                {"?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?"};
        public static object[] GetMessageData(Metric metric, Snapshot snapshot, IEvent baseEvent)
        {
            MessageFormatter formatter;
            if (!messageFormatterMap.TryGetValue(metric.ToString() + "MessageFormatter", out formatter))
                formatter = defaultFormatter;
            try
            {   // Log any exception chucked by getting message data
                return formatter.GetMessageData(snapshot, baseEvent);
            } catch (Exception e)
            {
                LOG.Error("Error getting message data for Metric{0}.  {1}", metric, e); 
            }
            return DUMMY_SUBST_DATA;
        }

        protected string GetMonitoredObjectName(IEvent baseEvent, string defaultName)
        {
            MonitoredObjectName mo = baseEvent.MonitoredObject;
            if (mo.IsServer)
                return mo.ServerName.Trim();
            if (mo.IsDatabase)
                return mo.DatabaseName;
            if (mo.IsTable)
                return mo.TableName;
            return defaultName;
        }

        /// <summary>
        /// Returns [ServerName, Sampled Value, MonitoredObject Name]
        /// </summary>
        /// <param name="baseEvent"></param>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public virtual object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
        {
            MonitoredObjectName mon = baseEvent.MonitoredObject;
            string serverName = mon != null
                                    ? mon.ServerName
                                    : snapshot != null
                                    ? snapshot.ServerName
                                    : "";

            /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
            object value = baseEvent.MetricValue;
            if (value is Enum)
            {
                value = Idera.SQLdm.Common.Configuration.EnumDescriptionConverter.GetFlagEnumDescription((Enum)value);
            }


            return new object[]
                {
                    serverName,
                    value,
                    GetMonitoredObjectName(baseEvent, serverName)
                };
        }

    public class CustomMessageFormatter : MessageFormatter
    {
        public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
        {
            object[] result = new object[4];
            result[0] = snapshot.ServerName;
            result[1] = baseEvent.Value;
            result[2] = baseEvent.MetricID;
            result[3] = "";

            object data = baseEvent.AdditionalData;
            if (data is Pair<CustomCounterSnapshot, MetricDescription?>)
            {
                Pair<CustomCounterSnapshot, MetricDescription?> pcm = (Pair<CustomCounterSnapshot, MetricDescription?>)data;
                if (pcm.First.Error != null)
                    result[3] = pcm.First.Error.Message;
                if (pcm.Second.HasValue)
                {
                    result[2] = pcm.Second.Value.Name;
                }
            }

            return result;
        }
    }

        public abstract class ServiceStateMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                string serverName = mon != null
                                        ? mon.ServerName
                                        : snapshot != null
                                        ? snapshot.ServerName
                                        : "";

                Exception e = snapshot == null ? null : snapshot.Error;
                string message = e != null ? e.Message : "";

                object value = baseEvent.Value;
                if (value is Enum)
                {
                    value = Idera.SQLdm.Common.Configuration.EnumDescriptionConverter.GetFlagEnumDescription((Enum)value);
                }

                return new object[]
                {
                    serverName,
                    value,
                    GetMonitoredObjectName(baseEvent, serverName),
                    message
                };
            }
        }

        public class AgentServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        public class SqlServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        public class DtcServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        public class FullTextServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --adding message formatter class for new monitored service
        public class SQLBrowserServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        public class SQLActiveDirectoryHelperServiceStatusMessageFormatter : ServiceStateMessageFormatter { }
        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --adding message formatter class for new monitored service

        public class SQLdmCollectionServiceStatusMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                if (baseEvent.Value is int && ((int)baseEvent.Value) >= 100)
                {
                    object[] addlData = baseEvent.AdditionalData as object[];
                    int n = (addlData != null) ? addlData.Length : 0;
                    object[] result = new object[n + 2];
                    result[0] = baseEvent.MonitoredObject.ServerName;
                    result[1] = baseEvent.Value;
                    for (int i = 0; i < n; i++)
                        result[i + 2] = addlData[i];
                    return result;
                }
                else
                    return base.GetMessageData(snapshot, baseEvent);
            }
        }

        public abstract class ConfigurationOptionMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                string serverName = mon != null
                                        ? mon.ServerName
                                        : snapshot != null
                                        ? snapshot.ServerName
                                        : "";

                return new object[]
                {
                    serverName,
                    baseEvent.Value.ToString().ToLower()
                };
            }
        }

        public class CLRStatusMessageFormatter : ConfigurationOptionMessageFormatter { }

        public class OLEAutomationStatusMessageFormatter : ConfigurationOptionMessageFormatter { }

        public class UserConnectionPctMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] additionalData = baseEvent.AdditionalData as object[];

                return new object[]
                {
                    snapshot.ServerName,
                    baseEvent.Value,
                    additionalData[0]
                };
            }
        }

        public class OldestOpenTransMinutesMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "", "", "", 0, "", ""};
                OpenTransaction xa = baseEvent.AdditionalData as OpenTransaction;

                if (xa != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = xa.Database;
                    result[3] = xa.Application;
                    result[4] = xa.Workstation;
                    result[5] = xa.UserName;
                    result[6] = xa.Spid;
                    result[7] = xa.LastCommand;
                    result[8] = xa.StartTime.Value.ToLocalTime();
                }
                return result;
            }
        }

        public class OSDiskFullMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, ""};

                
                result[0] = snapshot.ServerName;
                result[1] = baseEvent.Value;
                result[2] = baseEvent.MonitoredObject.HasAdditionalQualifiers ? 
                                    baseEvent.MonitoredObject.AdditionalQualifiers[0] : 
                                    baseEvent.MonitoredObject.ResourceName;
                
                return result;
            }
        }

        public class OSDiskPhysicalDiskTimePctPerDiskMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class OSDiskAverageDiskQueueLengthPerDiskMessageFormatter : OSDiskFullMessageFormatter
        {
        }
        
        public class MirroringSessionNonPreferredConfigMessageFormatter: MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                string serverName = mon != null
                                        ? mon.ServerName
                                        : snapshot != null
                                        ? snapshot.ServerName
                                        : "";
                object[] additionalData = baseEvent.AdditionalData as object[];
                //object[] additionalData = mon.AdditionalQualifiers;

                object value = baseEvent.Value;
                if (value is Enum)
                {
                    value = Idera.SQLdm.Common.Configuration.EnumDescriptionConverter.GetFlagEnumDescription((Enum)value);
                }


                return new object[]
                    {
                        serverName,
                        additionalData[1],
                        additionalData[0]
                    };            
            }
        }
        
        public class MirroringSessionRoleChangeMessageFormatter: MirroringSessionNonPreferredConfigMessageFormatter
        {
        
        }
        public class MirroringWitnessConnectionMessageFormatter : MirroringSessionNonPreferredConfigMessageFormatter
        {

        }
        
        /// <summary>
        /// Whatever is in additional data
        /// </summary>
        public class ErrorLogMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "", ""};

                var message = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).First;
                var fileSizeInMb = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).Second.Megabytes;
                var logSizeLimitInMB = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).Third.Megabytes;

                result[0] = snapshot.ServerName;
                result[1] = baseEvent.Value;
                result[2] = message;
                result[3] = "s";

                switch ((int)(baseEvent.Value))
                {
                    case -1: //file size exceeded
                        result[2] = string.Format(message, ((double)fileSizeInMb).ToString("###,###,##0.#"));
                        result[1] = ((double)fileSizeInMb).ToString("###,###,##0.#");
                        if (logSizeLimitInMB != null) result[4] = ((double)logSizeLimitInMB).ToString("###,###,##0.#");
                        break;
                    case 1: //There is just a single error
                        result[3] = "";
                        break;
                }


                return result;
            }
        }
        
        public class AgentLogMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "","" };
                var message = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).First;
                var fileSizeInMb = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).Second.Megabytes;
                var logSizeLimitInMB = ((Triple<string, FileSize, FileSize>)baseEvent.AdditionalData).Third.Megabytes;

                result[0] = snapshot.ServerName;
                result[1] = baseEvent.Value;
                result[2] = message;
                result[3] = "s";

                switch((int)(baseEvent.Value))
                {
                    case -1: //file size exceeded
                        result[2] = string.Format(message, ((double)fileSizeInMb).ToString("###,###,##0.#"));
                        result[1] = ((double)fileSizeInMb).ToString("###,###,##0.#");
                        if (logSizeLimitInMB != null) result[4] = ((double)logSizeLimitInMB).ToString("###,###,##0.#");
                        break;
                    case 1: //There is just a single error
                        result[3] = "";
                        break;
                }
                
                return result;
            }
        }

        public class ClusterFailoverMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", ""};

                Pair<string, string> additionalData = (Pair<string, string>)baseEvent.AdditionalData;
               
                result[0] = snapshot.ServerName;
                result[1] = additionalData.First;
                result[2] = additionalData.Second;
                // For when the batch returns 1900-1-1 0:00:00.000
                result[3] = (snapshot.ServerStartupTime.HasValue && snapshot.ServerStartupTime.Value.Year > 1900) ? 
                            snapshot.ServerStartupTime.Value.ToLocalTime() : 
                            snapshot.TimeStamp.HasValue ? snapshot.TimeStamp.Value.ToLocalTime() : (DateTime?) null;
                return result;
            }
        }

        public class ClusterActiveNodeMessageFormatter : ClusterFailoverMessageFormatter
        {
          
        }

        public class ReorganisationPctMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                return baseEvent.AdditionalData as object[];
            }
        }

        public class ResourceAlertMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                string serverName = string.Empty,
                       sessionID = "N/A",
                       loginTime = "N/A",
                       lastCommand = "N/A",
                       cpu = string.Empty;
                DateTime? ldt = null;

                if (baseEvent != null)
                {
                    MonitoredObjectName mon = baseEvent.MonitoredObject;
                    if (mon != null)
                    {
                        serverName = mon.ServerName;
                        sessionID = mon.AdditionalQualifiers[0];
                        lastCommand = mon.AdditionalQualifiers[2];

                        if (mon.AdditionalQualifiers[1] != String.Empty)
                        {
                            ldt = DateTime.Parse(mon.AdditionalQualifiers[1]);
                        }
                    }
                    else if (snapshot != null)
                    {
                        serverName = snapshot.ServerName;
                    }

                    if (ldt != null)
                    {
                        loginTime = ldt.Value.ToLocalTime().ToString();
                    }
                    cpu = string.Format("{0:F3}", baseEvent.Value);
                }

                return new object[]
                {
                    serverName,
                    cpu,
                    sessionID,
                    loginTime,
                    lastCommand
                };
            }
        }

        public class BlockingAlertMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", 0, "", "", "", "", DateTime.MinValue};
                BlockingSession bs = baseEvent.AdditionalData as BlockingSession;

                if (bs != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = bs.Databasename;
                    result[3] = bs.Spid;
                    result[4] = bs.Login;
                    result[5] = bs.Application;
                    result[6] = bs.Host;
                    result[7] = bs.InputBuffer;
                    result[8] = bs.BlockingStartTimeUTC.HasValue?bs.BlockingStartTimeUTC.Value:DateTime.MinValue;
                }
                return result;
            }
        }

        public class LongJobsMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", 0, 0, 0, null, "", "", "", "" };
                AgentJobRunning ajr = baseEvent.AdditionalData as AgentJobRunning;

                if (ajr != null)
                {
                    bool stepWise = baseEvent.MonitoredObject.AdditionalQualifiers[1] == "stepLevel";

                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = ajr.JobName;
                    result[3] = (int)((stepWise ? ajr.StepRunTimePercentOver : ajr.RunTimePercentOver) ?? 0);
                    result[4] = (int)((stepWise ? ajr.AverageStepDurationInSeconds : ajr.AverageDurationInSeconds) ?? 0);
                    result[5] = stepWise ? ajr.StepRunningTime.TotalSeconds : ajr.RunningTime.TotalSeconds;
                    result[6] = ajr.StartedAt.HasValue ? ajr.StartedAt.Value.ToLocalTime() : (DateTime?)null;
                    result[7] = ajr.StepName;
                    result[8] = ajr.JobDescription;
                    result[9] = stepWise ? ".  This step" : " and";
                    result[10] = stepWise ? "step '" + ajr.StepName.Trim() + "' " : "";

                }
                return result;
            }
        }

        public class LongJobsMinutesMessageFormatter : LongJobsMessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = base.GetMessageData(snapshot, baseEvent);

                AgentJobRunning ajr = baseEvent.AdditionalData as AgentJobRunning;
                if (ajr != null)
                {
                    bool stepWise = baseEvent.MonitoredObject.AdditionalQualifiers[1] == "stepLevel";

                    result[5] = Math.Floor(stepWise ? ajr.StepRunningTime.TotalMinutes : ajr.RunningTime.TotalMinutes);
                    result[9] = stepWise ? ".  This step" : " and";
                    result[10] = stepWise ? "step '" + ajr.StepName.Trim() + "' " : "";
                }

                return result;
            }

        }

        public class JobCompletionMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", "", null, null, "", null, "", "", "", "", "" };

                AgentJobCompletion job = baseEvent.AdditionalData as AgentJobCompletion;

                if (job != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = job.StepId == 0 ? "[" + job.JobName + "]" : "step [" + job.JobName.Trim() + "].[" + job.StepName.Trim() + "]";
                    result[3] = Enum.GetName(typeof(JobStepCompletionStatus), job.RunStatus);
                    result[4] = job.EndTime.HasValue ? job.EndTime.Value.ToLocalTime() : (DateTime?)null;
                    result[5] = job.StartTime.HasValue ? job.StartTime.Value.ToLocalTime() : (DateTime?)null;
                    result[6] = job.Message;
                    result[7] = job.CollectionSince.HasValue ? job.CollectionSince.Value.ToLocalTime() : (DateTime?)null;
                    result[8] = job.StepId == 0 ? "" : "step ";
                    result[9] = job.Successful;
                    result[10] = job.Retries;
                    result[11] = job.Failures;
                    result[12] = job.Canceled;
                }

                return result;
            }
        }

        public class BombedJobsMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result;
                if (baseEvent.AdditionalData is AgentJobFailure)
                {
                    result = new object[] { "", "", "", null, "", 0, 0, "", "", "", 0, 0, null };
                    AgentJobFailure ajf = baseEvent.AdditionalData as AgentJobFailure;

                    if (ajf != null)
                    {
                        result[0] = snapshot.ServerName;
                        result[1] = baseEvent.Value;
                        result[2] = (baseEvent.MonitoredObject.AdditionalQualifiers.Length > 1) ? 
                                        "step [" + ajf.JobName.Trim() + "].[" + ajf.StepName.Trim() + "]" : 
                                        "[" + ajf.JobName.Trim() + "]";
                        result[3] = ajf.RunTime.HasValue ? ajf.RunTime.Value.ToLocalTime() : (DateTime?) null;
                        result[4] = ajf.StepName;
                        result[5] = ajf.SqlSeverity;
                        result[6] = ajf.SqlMessageId;
                        result[7] = ajf.ErrorMessage;
                        result[8] = ajf.Command;
                        result[9] = ajf.JobDescription;
                        result[10] = ajf.Executions;
                        result[11] = ajf.FailedRuns;
                        result[12] = ajf.CollectionsSince.HasValue
                                         ? ajf.CollectionsSince.Value.ToLocalTime()
                                         : (DateTime?) null;
                    }
                }
                else
                {
                    result = new object[] { "","","",null,0};
                    AgentJobClear ajc = baseEvent.AdditionalData as AgentJobClear;
                    if (ajc != null)
                    {
                        result[0] = snapshot.ServerName;
                        result[1] = baseEvent.Value;
                        result[2] = ajc.JobName;
                        result[3] = ajc.LastRunStartTime.HasValue ? ajc.LastRunStartTime.Value.ToLocalTime() : (DateTime?)null;
                        result[4] = ajc.RunDuration.TotalMilliseconds;

                    }
                }
                return result;
            }
        }

        public class MaintenanceModeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                string serverName = mon != null ? mon.ServerName : snapshot != null ? snapshot.ServerName : "";

                MonitoredSqlServer server = baseEvent.AdditionalData as MonitoredSqlServer;
                string mmEndTime = "N/A";

                if (server != null)
                {
                    MaintenanceMode mmMode = server.MaintenanceMode;

                    if (mmMode.MaintenanceModeType == MaintenanceModeType.Recurring)
                    {
                        DateTime? mmTime = mmMode.MaintenanceModeRecurringStart + mmMode.MaintenanceModeDuration;
                        mmEndTime = String.Format("{0} {1}", DateTime.Now.Date.ToShortDateString(), mmTime.Value.ToShortTimeString());
                    }
                    else if (mmMode.MaintenanceModeType == MaintenanceModeType.Once)
                    {
                        mmEndTime = mmMode.MaintenanceModeStop.ToString();
                    }
                    else
                    {
                        mmEndTime = "further notice";
                    }
                }

                return new object[]
                {
                    serverName,
                    mmEndTime
                };

            }
        }


        public class DeadlockMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "", "", "", "", "", ""};
                DeadlockInfo dl = baseEvent.AdditionalData as DeadlockInfo;

                try
                {
                    ScheduledRefresh scheduledRefresh = (ScheduledRefresh) snapshot;

                    if(scheduledRefresh.MonitoredServer.ActivityMonitorConfiguration.TraceMonitoringEnabled)
                    {
                        result = dl.GetAlertMessageFields();//Tracefile XML file Format
                    }
                    else
                    {
                        result = dl.GetAlertEXFormatMessageFields(); //Extended Event XML file Format
                    }
                }
                catch(Exception e)
                {
                    result[2] = "unknown"; // appname 
                    result[3] = "unknown"; // user
                    result[4] = "unknown"; // host
                    result[5] = "unknown"; // last command
					result[8] = ""; // SPID unknown message
                    LOG.Error("Error reading deadlock fields for message formatter.",e);
                }

                result[0] = snapshot.ServerName;

                return result;
            }
        }
        public class NonSubscribedTransTimeMessageFormatter : NonDistributedTransTimeMessageFormatter
        {
        }
        public class NonDistributedTransTimeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                string serverName = mon != null
                                        ? mon.ServerName
                                        : snapshot != null
                                        ? snapshot.ServerName
                                        : "";

                object value = baseEvent.Value;
                if (value is Enum)
                {
                    value = Idera.SQLdm.Common.Configuration.EnumDescriptionConverter.GetFlagEnumDescription((Enum)value);
                }

                TimeSpan time = TimeSpan.FromSeconds((double)value);
                
                return new object[]
                {
                    serverName,
                    ((double) value).ToString("###,###,##0.##"),
                    GetMonitoredObjectName(baseEvent, serverName),
                    time.Hours,
                    time.Minutes,
                    time.Seconds,
                };
            }
        }

        public class AverageDiskMillisecondsPerReadMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class AverageDiskMillisecondsPerTransferMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class AverageDiskMillisecondsPerWriteMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class DiskReadsPerSecondMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class DiskTransfersPerSecondMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class DiskWritesPerSecondMessageFormatter : OSDiskFullMessageFormatter
        {
        }

        public class VersionStoreGenerationRatioMessageFormatter : MessageFormatter
        {

            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, 0, 0, 0, 0, 0};
                decimal value = Convert.ToDecimal(baseEvent.Value);
                object[] tempdbData = baseEvent.AdditionalData as object[];

                result[0] = snapshot.ServerName;

                try
                {
                    if (tempdbData != null)
                    {
                        result[1] = Math.Abs(value); // version store ratio
                        result[2] = tempdbData[0]; // current generation rate in kb/s
                        result[3] = tempdbData[1]; // current cleanup rate in kb/s
                        result[4] = tempdbData[2]; // version store size in kb
                        result[5] = tempdbData[3]; // temp size in kb
                        result[6] = value > 0 ? "greater" : "less";
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading version store ratio fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VersionStoreSizeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, 0, 0, 0, 0 };
                object value = baseEvent.Value;
                object[] tempdbData = baseEvent.AdditionalData as object[];

                result[0] = snapshot.ServerName;

                try
                {
                    if (tempdbData != null)
                    {
                        result[1] = baseEvent.Value; // version store size
                        result[2] = tempdbData[0]; // current generation rate in kb/s
                        result[3] = tempdbData[1]; // current cleanup rate in kb/s
                        result[4] = tempdbData[2]; // version store size in kb
                        result[5] = tempdbData[3]; // temp size in kb
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading version store size fields for message formatter.", e);
                }

                return result;
            }
        }

        public class LongRunningVersionStoreTransactionMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, 0, 0, 0, 0, "", "","","","","",0};
                object value = baseEvent.Value;
                Pair<Session, object[]> data = (Pair<Session, object[]>)baseEvent.AdditionalData;

                result[0] = snapshot.ServerName;

                try
                {
                    if (data != null)
                    {
                        result[1] = baseEvent.Value; // transaction length in minutes
                        result[2] = data.Second[0]; // current generation rate in kb/s
                        result[3] = data.Second[1]; // current cleanup rate in kb/s
                        result[4] = data.Second[2]; // version store size in kb
                        result[5] = data.Second[3]; // temp size in kb
                        result[6] = (int) baseEvent.Value > 1 ? "s" : "";
                        result[7] = data.First.Database;
                        result[8] = data.First.Application;
                        result[9] = data.First.UserName;
                        result[10] = data.First.Workstation;
                        result[11] = data.First.LastCommand;
                        result[12] = data.First.Spid;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading version store transaction fields for message formatter.", e);
                }

                return result;
            }
        }

        public class SessionTempdbSpaceUsageMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "","",0 };
                object value = baseEvent.Value;
                Session session = baseEvent.AdditionalData as Session;

                result[0] = snapshot.ServerName;

                try
                {
                    if (session != null && session.TotalTempdbSpaceUsed != null)
                    {
                        result[1] = session.TotalTempdbSpaceUsed.AsString(CultureInfo.CurrentCulture, "N1"); // tempdb space
                        result[2] = session.Database;
                        result[3] = session.Application;
                        result[4] = session.UserName;
                        result[5] = session.Workstation;
                        result[6] = session.LastCommand;
                        if (session.SessionUserSpaceUsed != null) result[7] = session.SessionUserSpaceUsed.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.SessionUserAllocatedTotal != null) result[8] = session.SessionUserAllocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.SessionUserDeallocatedTotal != null) result[9] = session.SessionUserDeallocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.SessionInternalSpaceUsed != null) result[10] = session.SessionInternalSpaceUsed.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.SessionInternalAllocatedTotal != null) result[11] = session.SessionInternalAllocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.SessionInternalDeallocatedTotal != null) result[12] = session.SessionInternalDeallocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskUserSpaceUsed != null) result[13] = session.TaskUserSpaceUsed.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskUserAllocatedTotal != null) result[14] = session.TaskUserAllocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskUserDeallocatedTotal != null) result[15] = session.TaskUserDeallocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskInternalSpaceUsed != null) result[16] = session.TaskInternalSpaceUsed.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskInternalAllocatedTotal != null) result[17] = session.TaskInternalAllocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        if (session.TaskInternalDeallocatedTotal != null) result[18] = session.TaskInternalDeallocatedTotal.AsString(CultureInfo.CurrentCulture, "N1");
                        result[19] = session.Spid;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading session tempdb space fields for message formatter.", e);
                }

                return result;
            }
        }

        public class TempdbContentionMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                var result = new object[] { "", 0, 0, 0, 0};
                var value = baseEvent.Value;
                var additionalData = baseEvent.AdditionalData as object[];

                try
                {
                    result[0] = snapshot.ServerName;
                    result[1] = value;

                    if (additionalData != null)
                    {
                        if (additionalData[0] != null) result[2] = additionalData[0];
                        if (additionalData[1] != null) result[3] = additionalData[1];
                        if (additionalData[2] != null) result[4] = additionalData[2];
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading version store transaction fields for message formatter.", e);
                }

                return result;
            }
        }

        public class LogFileAutogrowMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "" };
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                FileSize previousFileSize = baseEvent.AdditionalData as FileSize;
                FileSize currentFileSize = null;

                if (snapshot is ScheduledRefresh)
                {
                    if (((ScheduledRefresh) snapshot).DbStatistics.ContainsKey(mon.DatabaseName))
                    {
                        currentFileSize = ((ScheduledRefresh) snapshot).DbStatistics[mon.DatabaseName].LogFileSize;
                    }
                }
                if (snapshot is DatabaseSizeSnapshot)
                {
                    if (((DatabaseSizeSnapshot)snapshot).DbStatistics.ContainsKey(mon.DatabaseName))
                    {
                        currentFileSize = ((DatabaseSizeSnapshot)snapshot).DbStatistics[mon.DatabaseName].LogFileSize;
                    }
                }

                try
                {
                    result[0] = snapshot.ServerName;
                    result[1] = mon.DatabaseName;
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading autogrow fields for message formatter.", e);
                }

                return result;
            }
        }

        public class DataFileAutogrowMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", ""};
                MonitoredObjectName mon = baseEvent.MonitoredObject;
                FileSize previousFileSize = baseEvent.AdditionalData as FileSize;
                FileSize currentFileSize = null;

                if (snapshot is ScheduledRefresh)
                {
                    if (((ScheduledRefresh) snapshot).DbStatistics.ContainsKey(mon.DatabaseName))
                    {
                        currentFileSize = ((ScheduledRefresh) snapshot).DbStatistics[mon.DatabaseName].DataFileSize;
                    }
                }
                if(snapshot is DatabaseSizeSnapshot)
                {
                    if (((DatabaseSizeSnapshot)snapshot).DbStatistics.ContainsKey(mon.DatabaseName))
                    {
                        currentFileSize = ((DatabaseSizeSnapshot)snapshot).DbStatistics[mon.DatabaseName].DataFileSize;
                    }
                }

                try
                {
                    result[0] = snapshot.ServerName;
                    result[1] = mon.DatabaseName;                    
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading autogrow fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmConfigChangeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                result[0] = snapshot.ServerName;

                try
                {
                    var additionalData = (VMware.VMwareVirtualMachine[]) baseEvent.AdditionalData;

                    var prevConfig = additionalData[0];
                    var currentConfig = additionalData[1];

                    if (baseEvent.MonitoredObject.AdditionalQualifiers != null && currentConfig != null)
                    {
                        result[1] = prevConfig.NumCPUs;
                        result[2] = currentConfig.NumCPUs;
                        result[3] = prevConfig.CPULimit;
                        result[4] = currentConfig.CPULimit;
                        result[5] = prevConfig.CPUReserve;
                        result[6] = currentConfig.CPUReserve;
                        result[7] = prevConfig.MemSize.Megabytes;
                        result[8] = currentConfig.MemSize.Megabytes;
                        result[9] = prevConfig.MemLimit.Megabytes;
                        result[10] = currentConfig.MemLimit.Megabytes;
                        result[11] = prevConfig.MemReserve.Megabytes;
                        result[12] = currentConfig.MemReserve.Megabytes;
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmHostServerChangeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", 0, 0, 0, 0, 0 };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (baseEvent.MonitoredObject.AdditionalQualifiers != null && currentConfig != null)
                    {
                        result[1] = currentConfig.ESXHost.Name;
                        result[2] = baseEvent.MonitoredObject.AdditionalQualifiers[0];
                        result[3] = currentConfig.ESXHost.NumCPUCores;
                        result[4] = currentConfig.ESXHost.CPUMHz;
                        result[5] = currentConfig.ESXHost.MemSize.Gigabytes;
                        result[6] = currentConfig.ESXHost.PerfStats.CpuUsage;
                        result[7] = currentConfig.ESXHost.PerfStats.MemUsage;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmCPUUtilizationMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0.0 };
                object value = baseEvent.Value;

                try
                {
                    result[0] = snapshot.ServerName;
                    result[1] = value;
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmESXCPUUtilizationMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0.0, "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.ESXHost.PerfStats.CpuUsage;
                        result[2] = currentConfig.ESXHost.Name;
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmMemoryUtilizationMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0.0 };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.PerfStats.MemUsage;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmESXMemoryUsageMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0.0, "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.ESXHost.PerfStats.MemUsage;
                        result[2] = currentConfig.ESXHost.Name;
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmCPUReadyWaitTimeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.PerfStats.CpuReady;
                        result[2] = currentConfig.ESXHost.Name;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmReclaimedMemoryMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.PerfStats.MemBallooned.Kilobytes;
                        result[2] = currentConfig.ESXHost.Name;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmMemorySwapDelayDetectedMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "" };

                result[0] = snapshot.ServerName;

                return result;
            }
        }

        public class VmESXMemorySwapDetectedMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.ESXHost.Name;
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmResourceLimitsMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.CPULimit > 0 
                                        ? currentConfig.CPULimit.ToString() 
                                        : "Unlimited";
                        result[2] = currentConfig.MemLimit.Megabytes > 0
                                        ? currentConfig.MemLimit.Megabytes.ToString()
                                        : "Unlimited";
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading VM Config fields for message for matter in Resource Limits message formatter", e);
                }

                return result;
            }
        }

        public class VmPowerStateMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.Status.ToString();
                    }

                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }

        public class VmESXPowerStateMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "" };

                result[0] = snapshot.ServerName;

                try
                {
                    var currentConfig = baseEvent.AdditionalData as VMware.VMwareVirtualMachine;

                    if (currentConfig != null)
                    {
                        result[1] = currentConfig.ESXHost.Status.ToString();
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading vm config fields for message formatter.", e);
                }

                return result;
            }
        }


        public class DatabaseSizeMbMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] {"", "", "", ""};

                FileSize dbSize = new FileSize {Megabytes = (decimal) baseEvent.Value};
                string denomination;

                result[0] = snapshot.ServerName;
                result[1] = dbSize.BestDenomination(out denomination);
                result[2] = baseEvent.MonitoredObject.DatabaseName;
                result[3] = denomination;

                return result;
            }
        }

        public class TransLogSizeMbMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] {"","","",""};

                FileSize logSize = new FileSize { Megabytes = (decimal)baseEvent.Value };
                string denomination;

                result[0] = snapshot.ServerName;
                result[1] = logSize.BestDenomination(out denomination);
                result[2] = baseEvent.MonitoredObject.DatabaseName;
                result[3] = denomination;

                return result;
            }
        }

        public class OsDiskFreeMbMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] {"","","","","",""};

                FileSize osFreeSpace = new FileSize { Megabytes = (decimal)baseEvent.Value };
                string denomination;
                DiskDrive driveInfo = (DiskDrive)baseEvent.AdditionalData;

                result[0] = snapshot.ServerName;
                result[1] = osFreeSpace.BestDenomination(out denomination);
                result[2] = driveInfo.DriveLetter; // Drive Letter
                result[3] = denomination;
                result[4] = String.Format("{0:F2}",driveInfo.UsedSize.BestDenomination(out denomination)); // Used Space on the drive
                result[5] = denomination; // denomination of the used space (MB/GB)

                return result;
            }
        }

        public class AlwaysOnAvailabilityGroupRoleChangeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] {"", "", "", "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];
                result[3] = additionalData[3];

                return result;
            }
        }
        
        public class AlwaysOnEstimatedDataLossTimeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { 0, "" };
                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];

                return result;
            }
        }
        
        public class AlwaysOnSynchronizationHealthStateMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", "" };
                
                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];
                result[3] = additionalData[3];

                return result;
            }
        }
        
        public class AlwaysOnEstimatedRecoveryTimeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { 0, "" };
                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];

                return result;
            }
        }
        
        public class AlwaysOnSynchronizationPerformanceMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { 0, "", "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];

                return result;
            }
        }
        
        public class AlwaysOnLogSendQueueSizeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];

                return result;
            }
        }
        
        public class AlwaysOnRedoQueueSizeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];

                return result;
            }
        }
        
        public class AlwaysOnRedoRateMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];

                return result;
            }
        }

        public class PreferredNodeUnavailabilityMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "" , 0};

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = additionalData[1];
                result[2] = additionalData[2];
                result[3] = additionalData[3];

                return result;
            }
        }

        //Start -SQLdm 9.0 (Ankit Srivastava) added message formatter for the new metric grooming time out
        public class RepositoryGroomingTimedOutMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { String.Empty,0,String.Empty };

                string[] additionalData= (string[])baseEvent.AdditionalData;
                result[0] = additionalData[0];
                result[1] = (additionalData[1].Length == 0) ? 0 : 1;
                result[2] = additionalData[1];

                return result;
            }
        }
        //End -SQLdm 9.0 (Ankit Srivastava) added message formatter for the new metric grooming time out

        //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added message formatter for filegroup space full metrices
        public class FilegroupSpaceFullPctMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", "", "" };
 
                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = String.Format("{0:F2}", additionalData[1]);
                result[2] = additionalData[2];
                result[3] = additionalData[3];
                result[4] = additionalData[4];

                return result;
            }
        }

        public class FilegroupSpaceFullSizeMessageFormatter : MessageFormatter
        {
            public override object[] GetMessageData(Snapshot snapshot, IEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", "", "" };

                object[] additionalData = baseEvent.AdditionalData as object[];

                result[0] = additionalData[0];
                result[1] = String.Format("{0:F2}", additionalData[1]);
                result[2] = additionalData[2];
                result[3] = "MB";
                result[4] = additionalData[4];

                return result;
            }
        }
        //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added message formatter for filegroup space full metrices
    }
    

    #endregion

    #endregion

    #region Custom Counters

    [Serializable]
    public class CustomCounterDefinition : ICloneable
    {
        private int metricID;
        private bool enabled;
        private DateTime lastChanged = DateTime.MinValue;
        private MetricType metricType;
        private CalculationType calculationType;
        private double scale;
        private string objectName;
        private string counterName;
        private string instanceName;
        private string sqlStatement;
        private string serverType;
        private long profileId;

        public CustomCounterDefinition()
        {
            metricID = -1;
            MetricType = MetricType.SQLCounter;
            CalculationType = CalculationType.Value;
            enabled = true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public CustomCounterDefinition(int metricID, MetricType metricType, CalculationType calculationType, double scale, string objectName, string counterName, string instanceName, string sqlStatement, int profileId)
        {
            this.metricID = metricID;
            this.metricType = metricType;
            this.calculationType = calculationType;
            this.scale = scale != 0.0d ? scale : 1.0d;  // scale should never be 0
            this.objectName = objectName;
            this.counterName = counterName;
            this.instanceName = instanceName;
            this.sqlStatement = sqlStatement;
            this.ProfileId = profileId;
        }

        internal CustomCounterDefinition(IDataRecord record)
        {
            metricID = (int) record["Metric"];
            lastChanged = (DateTime) record["UTCLastChangeDateTime"];
            enabled = (bool)record["Enabled"];
            scale = (double) record["Scale"];
            metricType = (MetricType)Enum.ToObject(typeof(MetricType), (int)record["MetricType"]);
            calculationType = (CalculationType)Enum.ToObject(typeof(CalculationType), (int)record["CalculationType"]);
            int fieldID = record.GetOrdinal("Object");
            if (!record.IsDBNull(fieldID))
                objectName = record.GetString(fieldID).TrimEnd();
            fieldID = record.GetOrdinal("Counter");
            if (!record.IsDBNull(fieldID))
                counterName = record.GetString(fieldID).TrimEnd();
            fieldID = record.GetOrdinal("Instance");
            if (!record.IsDBNull(fieldID))
                instanceName = record.GetString(fieldID).TrimEnd();
            fieldID = record.GetOrdinal("Batch");
            if (!record.IsDBNull(fieldID))
                sqlStatement = record.GetString(fieldID).TrimEnd();
            fieldID = record.GetOrdinal("ServerType");
            if (!record.IsDBNull(fieldID))
                serverType = record.GetString(fieldID).TrimEnd();
            fieldID = record.GetOrdinal("AzureProfileId");
            if (!record.IsDBNull(fieldID))
                profileId = record.GetInt64(fieldID);
        }

        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }
        
        public bool IsEnabled
        {
            get { return enabled;  }
            set { enabled = value; }
        }

        public MetricType MetricType
        {
            get { return metricType; }
            set { metricType = value; }
        }

        public DateTime LastChanged
        {
            get { return lastChanged; }
        }

        public CalculationType CalculationType
        {
            get { return calculationType;  }
            set { calculationType = value; }
        }

        /// <summary>
        /// Resource Name
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        public string CounterName
        {
            get { return counterName; }
            set { counterName = value; }
        }

        public string InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }

        /// <summary>
        /// Stores the Resource Type for the Azure Monitor Custom Counter
        /// </summary>
        public string SqlStatement
        {
            get { return sqlStatement; }
            set { sqlStatement = value; }
        }

        /// <summary>
        /// Stores the Resource URI for the Azure Monitor Custom Counter
        /// </summary>
        public string ServerType
        {
            get { return serverType; }
            set { serverType = value; }
        }

        public long ProfileId
        {
            get { return profileId; }

            set {  profileId = value; }
        }

        /// <summary>
        /// Used to compare two configurations, excluding Enabled
        /// </summary>
        public bool EqualsMinusEnabled(object obj)
        {
            if (this == obj) return true;
            CustomCounterDefinition customCounterDefinition = obj as CustomCounterDefinition;
            if (customCounterDefinition == null) return false;
            if (metricID != customCounterDefinition.metricID) return false;
            if (!Equals(metricType, customCounterDefinition.metricType)) return false;
            if (!Equals(calculationType, customCounterDefinition.calculationType)) return false;
            if (!Equals(objectName, customCounterDefinition.objectName)) return false;
            if (!Equals(counterName, customCounterDefinition.counterName)) return false;
            if (!Equals(instanceName, customCounterDefinition.instanceName)) return false;
            if (!Equals(sqlStatement, customCounterDefinition.sqlStatement)) return false;
            if (!Equals(serverType, customCounterDefinition.serverType)) return false;
            return true;
        }

        public double Scale
        {
            get { return scale == 0.0d ? 1 : scale; }
            set
            {
                System.Diagnostics.Debug.Assert(value != 0.0d);
                scale = value;
            }
        }
    }

    #endregion

}
