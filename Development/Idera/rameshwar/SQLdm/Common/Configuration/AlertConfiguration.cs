//------------------------------------------------------------------------------
// <copyright file="AlertConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Snapshots.State;
    using Idera.SQLdm.Common.Thresholds;

    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Auditing;
    using Idera.SQLdm.Common.Attributes;
    using System.Reflection;
    using BBS.TracerX;


    [Serializable]
    public class AlertConfiguration : ISerializable
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        [NonSerialized]
        private PropertyChangedEventHandler itemPropertyChangedEventHandler;
        [NonSerialized]
        private Dictionary<Pair<int, string>, AlertConfigurationItem> data;

        private MetricDefinitions metricDefinitions;

        private int instanceID;
        private ArrayList changedItems;
        private bool changed;

        public AlertConfiguration(int instanceID)
        {
            this.instanceID = instanceID;
            data = new Dictionary<Pair<int, string>, AlertConfigurationItem>();
            itemPropertyChangedEventHandler = new PropertyChangedEventHandler(item_PropertyChanged);
        }

        #region Custom Serialization

        public AlertConfiguration(SerializationInfo info, StreamingContext context)
        {
            data = new Dictionary<Pair<int, string>, AlertConfigurationItem>();
            itemPropertyChangedEventHandler = new PropertyChangedEventHandler(item_PropertyChanged);

            instanceID = info.GetInt32("instanceID");
            changed = info.GetBoolean("changed");
            changedItems = (ArrayList)info.GetValue("changedItems", typeof(ArrayList));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("instanceID", instanceID);
            info.AddValue("changed", changed);
            info.AddValue("changedItems", changedItems);
        }

        #endregion

        /// <summary>
        /// Returns the Alert Configuration Item for the specified Metric and Instance Name.  Instance Name should be an empty string to get the 
        ///    default instance.
        /// </summary>
        /// <param name="metricID"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public AlertConfigurationItem this[int metricID, string instanceName]
        {
            get
            {
                AlertConfigurationItem item = null;
                Pair<int, string> defaultInstance = new Pair<int, string>(metricID, instanceName);

                if (!data.ContainsKey(defaultInstance))
                    defaultInstance.Second = String.Empty;

                data.TryGetValue(defaultInstance, out item);

                return item;
            }
        }

        public AlertConfigurationItem this[Metric metric, string instanceName]
        {
            get { return this[(int)metric, instanceName]; }
        }

        public MetricDefinitions MetricDefinitions
        {
            get { return metricDefinitions; }
            set { metricDefinitions = value; }
        }

        public int InstanceID
        {
            get { return instanceID; }
        }

        public bool IsChanged
        {
            get { return changed; }
            private set
            {
                bool old = changed;
                changed = value;
                if (old != changed)
                    OnPropertyChanged("Enabled");
            }
        }

        public AlertConfigurationItem[] ItemList
        {
            get
            {
                if (data == null)
                    data = new Dictionary<Pair<int, string>, AlertConfigurationItem>();

                return Collections.ToArray(data.Values);
            }
        }

        public ArrayList ChangeItems
        {
            get
            {
                if (changedItems == null)
                    PrepareChangedItems();
                return changedItems;
            }
        }

        /// <summary>
        /// Delete a threshold instance from the configuration
        /// </summary>
        /// <param name="oldItem"></param>
        public void RemoveEntry(AlertConfigurationItem oldItem)
        {
            if (oldItem.IsDefaultThreshold)
                return;
            RemoveEntry(oldItem.MetricID, oldItem.MetricInstance);
        }

        public void RemoveEntry(Metric metric, string metricInstance)
        {
            RemoveEntry((int)metric, metricInstance);
        }

        public void RemoveEntry(int metricId, string metricInstance)
        {
            Pair<int, string> aciKey = new Pair<int, string>(metricId, metricInstance);

            if (data.ContainsKey(aciKey))
            {
                data[aciKey].IsThresholdDeleted = true;
            }

            IsChanged = true;
        }

        /// <summary>
        ///  Add Threshold Instances to the configuration
        /// </summary>
        /// <param name="newItem"></param>
        public void AddEntry(AlertConfigurationItem newItem)
        {
            Pair<int, string> aciKey = new Pair<int, string>(newItem.MetricID, newItem.MetricInstance);

            if (data.ContainsKey(aciKey))
            {
                AlertConfigurationItem oldItem = data[aciKey];
                oldItem.PropertyChanged -= itemPropertyChangedEventHandler;
                data.Remove(aciKey);
            }
            newItem.IsThresholdNew = true;
            newItem.PropertyChanged += itemPropertyChangedEventHandler;
            data.Add(aciKey, newItem);

            IsChanged = true;
        }


        /// <summary>
        /// Add new threshold instances from the repository...
        /// </summary>
        /// <param name="metricID"></param>
        /// <param name="metricDescription"></param>
        /// <param name="thresholdEntry"></param>
        public void AddEntry(int metricID, MetricDescription metricDescription, MetricThresholdEntry thresholdEntry)
        {

            Pair<int, string> aciKey = new Pair<int, string>(metricID, thresholdEntry.IsDefaultThreshold ? String.Empty : thresholdEntry.MetricInstanceName);

            if (data.ContainsKey(aciKey))
            {
                AlertConfigurationItem olditem = data[aciKey];
                olditem.PropertyChanged -= itemPropertyChangedEventHandler;
                data.Remove(aciKey);
            }
            MetricDefinition metricDefinition = metricDefinitions.GetMetricDefinition(metricID);

            AlertConfigurationItem newitem = new AlertConfigurationItem(metricID, metricDefinition, metricDescription, thresholdEntry);
            newitem.PropertyChanged += itemPropertyChangedEventHandler;
            data.Add(aciKey, newitem);

        }

        public Dictionary<string, AlertConfigurationItem> GetThresholdsForMetric(Metric metric)
        {
            return GetThresholdsForMetric((int)metric);
        }

        public Dictionary<string, AlertConfigurationItem> GetThresholdsForMetric(int metric)
        {
            Dictionary<string, AlertConfigurationItem> result = new Dictionary<string, AlertConfigurationItem>();
            foreach (Pair<int, string> aciKey in data.Keys)
            {
                if (aciKey.First == metric)
                {
                    if (!result.ContainsKey(aciKey.Second))
                    {
                        result.Add(aciKey.Second, data[aciKey]);
                    }
                }
            }

            return result;
        }

        public bool ItemExists(AlertConfigurationItem item)
        {
            Pair<int, string> aciKey = new Pair<int, string>(item.MetricID, item.MetricInstance);

            return data.ContainsKey(aciKey);
        }

        /// <summary>
        /// Finds all the changed stuff and adds it to the changed items list.
        /// </summary>
        public void PrepareChangedItems()
        {
            if (changedItems == null)
                changedItems = new ArrayList();
            else
                changedItems.Clear();

            if (data != null)
            {
                foreach (Pair<int, string> keyPair in Collections.ToArray(data.Keys))
                {
                    AlertConfigurationItem item = data[keyPair];
                    if (item.IsMetricInfoChanged)
                        changedItems.Add(new Pair<int, MetricDescription>(keyPair.First, item.MetricDescription));

                    if (item.IsChanged)
                        changedItems.Add(item.ThresholdEntry);

                    //Pair<InstanceAction, MetricThresholdEntry> mteAction = new Pair<InstanceAction, MetricThresholdEntry>(InstanceAction.Edit, item.ThresholdEntry);
                    //if (item.IsThresholdDeleted)
                    //{
                    //    mteAction.First = InstanceAction.Delete;
                    //    changedItems.Add(mteAction);
                    //}
                    //else if (item.IsThresholdNew)
                    //{
                    //    mteAction.First = InstanceAction.Add;
                    //    changedItems.Add(mteAction);
                    //}
                    //else if (item.IsThresholdChanged)
                    //    changedItems.Add(mteAction);
                }
            }
        }

        /// <summary>
        /// Clone the all metric configuration changed to be copy to any alert configuration. 
        /// </summary>
        /// <returns>The array list with the clone metrics if the metric list return a empty list.</returns>
        public ArrayList GetAllAlertMetricChanged()
        {
            ArrayList metricsConfigurationList = new ArrayList();

            if (data != null)
            {
                foreach (Pair<int, string> keyPair in Collections.ToArray(data.Keys))
                {
                    AlertConfigurationItem item = ObjectHelper.Clone(data[keyPair]);
                    metricsConfigurationList.Add(item.ThresholdEntry);
                }
            }

            return metricsConfigurationList;
        }

        /// <summary>
        /// Reset the change flags.
        /// </summary>
        public void ChangedApplied()
        {
            if (data != null)
            {
                foreach (Pair<int, string> keyPair in Collections.ToArray(data.Keys))
                {
                    AlertConfigurationItem item = data[keyPair];
                    if (item.IsThresholdDeleted)
                        data.Remove(keyPair);
                    else if (item.IsChanged)
                        item.ChangesApplied();
                }
            }
            if (changedItems != null)
            {
                changedItems.Clear();
                changedItems = null;
            }
            changed = false;
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs pcea = new PropertyChangedEventArgs(name);
                PropertyChanged(this, pcea);
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ItemPropertyChanged != null)
            {
                // rebroadcast the event
                ItemPropertyChanged(sender, e);
            }

            IsChanged = true;
        }
    }

    [Serializable]
    public class AlertConfigurationItem : IAuditable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int metricID;
        private bool commentsChanged;
        private bool rankchanged;
        private bool multiInstance = false;
        private MetricDescription metricDescription;
        private MetricThresholdEntry thresholdEntry;
        private Threshold okThreshold;
        //10.0 SQLdm srishti purohit -- baseline support
        private Threshold okThresholdBaseline;
        private string configuredAlertValue; //SQLdm 8.6 -(Ankit Srivastava) : Preferred field Feature -- New property for the Configured Alert Value(Info/critical/warning)

        [NonSerialized]
        private object suggestedWarningThreshold;
        [NonSerialized]
        private object suggestedCriticalThreshold;

        [NonSerialized]
        private MetricDefinition metaData;
        [NonSerialized]
        private FlattenedThreshold[] flattenedThresholds;
        [NonSerialized]
        private FlattenedThreshold[] flattenedThresholdsBaseline;
        //Logging msg
        private static readonly Logger LOG = Logger.GetLogger(typeof(AlertConfigurationItem));

        /// <summary>
        /// Indicates that the change on the 'threshold item' is not a fact.
        /// </summary>
        [NonSerialized]
        private bool isNotFakeThresholdChanged = true;

        public AlertConfigurationItem(AlertConfigurationItem oldItem)
        {
            this.metricID = oldItem.MetricID;

            // Metric Description
            MetricDescription oldDescription = oldItem.MetricDescription;
            metricDescription = new MetricDescription(oldDescription.Name, oldDescription.Category,
                                                            oldDescription.Description, oldDescription.Comments, oldDescription.Rank);

            // Threshold Entry
            MetricThresholdEntry oldEntry = oldItem.ThresholdEntry;
            Threshold infoThreshold = new Threshold(oldEntry.InfoThreshold.Enabled, oldEntry.InfoThreshold.Op, (IComparable)oldEntry.InfoThreshold.Value);
            Threshold warningThreshold = new Threshold(oldEntry.WarningThreshold.Enabled, oldEntry.WarningThreshold.Op, (IComparable)oldEntry.WarningThreshold.Value);
            Threshold criticalThreshold = new Threshold(oldEntry.CriticalThreshold.Enabled, oldEntry.CriticalThreshold.Op, (IComparable)oldEntry.CriticalThreshold.Value);
            Threshold infoThresholdBaseline = new Threshold(oldEntry.BaselineInfoThreshold.Enabled, oldEntry.BaselineInfoThreshold.Op, (IComparable)oldEntry.BaselineInfoThreshold.Value);
            Threshold warningThresholdBaseline = new Threshold(oldEntry.BaselineWarningThreshold.Enabled, oldEntry.BaselineWarningThreshold.Op, (IComparable)oldEntry.BaselineWarningThreshold.Value);
            Threshold criticalThresholdBaseline = new Threshold(oldEntry.BaselineCriticalThreshold.Enabled, oldEntry.BaselineCriticalThreshold.Op, (IComparable)oldEntry.BaselineCriticalThreshold.Value);
            thresholdEntry = new MetricThresholdEntry(oldEntry.MonitoredServerID, this.metricID, warningThreshold, criticalThreshold, infoThreshold, warningThresholdBaseline, criticalThresholdBaseline, infoThresholdBaseline);

            // Metric Definition (metaData)
            MetricDefinition oldMetaData = oldItem.GetMetaData();
            metaData = new MetricDefinition(this.metricID);
            metaData.IsDeleted = false;
            metaData.MetricClass = oldMetaData.MetricClass;
            metaData.Options = oldMetaData.Options;
            metaData.MinValue = oldMetaData.MinValue;
            metaData.MaxValue = oldMetaData.MaxValue;
            metaData.DefaultInfoThresholdValue = oldMetaData.DefaultInfoThresholdValue;
            metaData.DefaultWarningThresholdValue = oldMetaData.DefaultWarningThresholdValue;
            metaData.DefaultCriticalThresholdValue = oldMetaData.DefaultCriticalThresholdValue;
            metaData.SetEventCategory(oldMetaData.EventCategory);
            metaData.DefaultMessageID = oldMetaData.DefaultMessageID;
            metaData.AlertEnabledByDefault = oldMetaData.AlertEnabledByDefault;
            metaData.ComparisonType = oldMetaData.ComparisonType;
            metaData.ValueType = oldMetaData.ValueType;
            metaData.Rank = oldMetaData.Rank;
            metaData.MaxValueBaseline = oldMetaData.MaxValueBaseline;
            metaData.DefaultInfoThresholdValueBaseline = oldMetaData.DefaultInfoThresholdValueBaseline;
            metaData.DefaultWarningThresholdValueBaseline = oldMetaData.DefaultWarningThresholdValueBaseline;
            metaData.DefaultCriticalThresholdValueBaseline = oldMetaData.DefaultCriticalThresholdValueBaseline;

            //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- New field for the Configured Alert Value(Info/critical/warning)
            this.configuredAlertValue = thresholdEntry.WarningThreshold.Enabled ? "Warning" : (thresholdEntry.InfoThreshold.Enabled ? "Informational" : "Critical");


        }

        public AlertConfigurationItem(int metricID, MetricDefinition metaData, MetricDescription metricDescription, MetricThresholdEntry thresholdEntry)
        {
            this.metricID = metricID;
            this.metaData = metaData;
            this.metricDescription = metricDescription;
            this.thresholdEntry = thresholdEntry;

            //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- New field for the Configured Alert Value(Info/critical/warning)
            this.configuredAlertValue = thresholdEntry.WarningThreshold.Enabled ? "Warning" : (thresholdEntry.InfoThreshold.Enabled ? "Informational" : "Critical");

        }

        public AlertConfigurationItem(int metricID, MetricDefinition metaData, MetricDescription metricDescription, IEnumerable<MetricThresholdEntry> thresholdEntries)
        {
            this.metricID = metricID;
            this.metaData = metaData;
            this.metricDescription = metricDescription;
        }

        //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- New property for the Configured Alert Value(Info/critical/warning)
        public string ConfiguredAlertValue
        {
            get { return configuredAlertValue; }
            set { configuredAlertValue = value; }
        }

        public MetricDescription MetricDescription
        {
            get { return metricDescription; }
        }

        public MetricThresholdEntry ThresholdEntry
        {
            get { return thresholdEntry; }
        }

        public void SetThresholdEntry(MetricThresholdEntry newEntry)
        {

            // UNUSED????
            MetricThresholdEntry oldEntry = thresholdEntry;

            thresholdEntry = newEntry;
            IsThresholdChanged = false;
            commentsChanged = false;
            rankchanged = false;
            IsThresholdNew = true;
            IsThresholdDeleted = false;
            okThreshold = null;
            flattenedThresholds = null;

            if (thresholdEntry.IsEnabled != newEntry.IsEnabled)
            {
                OnPropertyChanged("Enabled");
            }
        }

        public int MetricID
        {
            get { return metricID; }
        }

        public string MetricInstance
        {
            get { return ThresholdEntry.MetricInstanceName; }
            set { ThresholdEntry.MetricInstanceName = value; }
        }

        public InstanceType InstanceType
        {
            get { return ThresholdEntry.MetricInstanceType; }
        }

        public bool IsDefaultThreshold
        {
            get { return ThresholdEntry.IsDefaultThreshold; }
        }

        public bool IsInfoEnabled
        {
            get { return ThresholdEntry.InfoThreshold.Enabled; }
        }

        public bool IsWarningEnabled
        {
            get { return ThresholdEntry.WarningThreshold.Enabled; }
        }

        public bool IsCriticalEnabled
        {
            get { return ThresholdEntry.CriticalThreshold.Enabled; }
        }

        public bool IsMultiInstance
        {
            get { return ((metaData.Options & ThresholdOptions.MultiInstance) == ThresholdOptions.MultiInstance); }
        }

        public int Rank
        {
            get { return metricDescription.Rank; }
            set {
                if (metricDescription.Rank != value)
                {
                    metricDescription.Rank = value;
                    OnPropertyChanged("Rank");
                }
            }
        }
        public string Name
        {
            get { return metricDescription.Name; }
        }
        public string Category
        {
            get { return metricDescription.Category; }
        }
        public string Description
        {
            get { return metricDescription.Description; }
        }

        public bool Enabled
        {
            get
            {
                return thresholdEntry.IsEnabled;
            }
            set
            {
                // Doesn't metric doesn't support multi-instance keep the IsEnabled and IsThresholdEnabled in sync
                if ((metaData.Options & ThresholdOptions.MultiInstance) != ThresholdOptions.MultiInstance)
                    thresholdEntry.IsThresholdEnabled = thresholdEntry.IsEnabled = value;
                else
                    thresholdEntry.IsEnabled = value;

                OnPropertyChanged("Enabled");
            }
        }

        public bool ThresholdEnabled
        {
            get { return thresholdEntry.IsThresholdEnabled; }
            set
            {
                // Doesn't metric doesn't support multi-instance keep the IsEnabled and IsThresholdEnabled in sync
                if (((metaData.Options & ThresholdOptions.MultiInstance) == ThresholdOptions.MultiInstance) && IsDefaultThreshold)
                    thresholdEntry.IsThresholdEnabled = value;
                else
                    thresholdEntry.IsEnabled = thresholdEntry.IsThresholdEnabled = value;
                OnPropertyChanged("ThresholdEnabled");
            }
        }

        public string Comments
        {
            get { return metricDescription.Comments; }
            set
            {
                if (string.IsNullOrEmpty(metricDescription.Comments) && string.IsNullOrEmpty(value))
                {
                    return;
                }
                else if (metricDescription.Comments != value)
                {
                    metricDescription.Comments = value;
                    OnPropertyChanged("Comments");
                }
            }
        }

        public MonitoredState GetSeverity(IComparable value)
        {
            if (metaData != null)
            {
                if (value.GetType() != metaData.ValueType)
                {
                    // see if this is an enumerated value
                    if (metaData.ValueType.IsEnum)
                    {
                        // get the underlying type and convert value to that type
                        object evalue = Convert.ChangeType(value, Enum.GetUnderlyingType(metaData.ValueType));
                        if (Enum.IsDefined(metaData.ValueType, evalue))
                        {
                            value = (IComparable)Enum.ToObject(metaData.ValueType, evalue);
                        }
                    }
                }
            }
            return SnapshotState.GetState(value, ThresholdEntry);
        }

        public void SetData(object data)
        {
            thresholdEntry.Data = data;
            OnPropertyChanged("Data");
        }

        public bool IsChanged
        {
            get { return IsMetricInfoChanged || thresholdEntry.State != ThresholdState.Unchanged; }
        }
        
        public bool IsMetricInfoChanged
        {
            get { return commentsChanged||rankchanged ; }
        }
        //10.0 srishti purohit -- for baseline alert modifications

        public bool IsBaselineEnabled
        {
            get { return thresholdEntry.IsBaselineEnabled; }
            set
            {
                thresholdEntry.IsBaselineEnabled = value;
                //OnPropertyChanged("IsBaselineEnabled");       // Pruthviraj Nikam:    Done changes for Baseline Alerts
            }
        }
        /// <summary>
        /// True if the change in the 'AlertConfigurationItem' is not a fake change.
        /// </summary>
        public bool IsNotFakeThresholdChanged()
        {
            return isNotFakeThresholdChanged;
        }

        /// <summary>
        /// Use to indicates taht the change in the 'AlertConfigurationItem' is a fake change.
        /// </summary>
        public void EnableFakeThresholdChanged()
        {
            isNotFakeThresholdChanged = false;
        }

        public bool IsThresholdChanged
        {
            get { return thresholdEntry.State == ThresholdState.Changed; }
            set
            {
                bool changed = value;
                if (changed && !IsThresholdNew)
                    thresholdEntry.State = ThresholdState.Changed;
            }
        }

        public bool IsThresholdNew
        {
            get { return thresholdEntry.State == ThresholdState.Added; }
            set
            {
                bool added = value;
                if (added)
                    thresholdEntry.State = ThresholdState.Added;
            }
        }

        public bool IsThresholdDeleted
        {
            get { return thresholdEntry.State == ThresholdState.Deleted; }
            set
            {
                bool deleted = value;
                if (deleted)
                    thresholdEntry.State = ThresholdState.Deleted;
            }
        }

        public object RangeStart1
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholds[1];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = FlattenedThresholds[1];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }

        public object RangeStart2
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholds[2];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = FlattenedThresholds[2];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }

        public object RangeStart3
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholds[3];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = FlattenedThresholds[3];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }
        //10.0 Srishti purohit // For baseline alert
        //Making copy of State grid to give baseline alert functioanlity
        public object RangeStartBaselineInfo1
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholdsBaseline[1];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = FlattenedThresholdsBaseline[1];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }

        public object RangeStartBaselineWarning2
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholdsBaseline[2];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = FlattenedThresholdsBaseline[2];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }

        public object RangeStartBaselineCritical3
        {
            get
            {
                FlattenedThreshold threshold = FlattenedThresholdsBaseline[3];
                if (threshold.IsMultiValued || !threshold.IsNumeric)
                    return 0;

                return threshold.RangeStart;
            }
            set
            {
                FlattenedThreshold threshold = flattenedThresholdsBaseline[3];
                if (!threshold.IsMultiValued && threshold.IsNumeric)
                    threshold.RangeStart = value;
            }
        }

        public bool GetAllValuesAssigned()
        {
            if ((metaData.Options & ThresholdOptions.ContainedValueList) == ThresholdOptions.ContainedValueList)
            {
                List<object> possibleValues = GetPossibleValues<object>();

                foreach (FlattenedThreshold threshold in FlattenedThresholds)
                {
                    Threshold.ComparableList list = threshold.Value as Threshold.ComparableList;
                    foreach (object o in list)
                    {
                        possibleValues.Remove(o);
                    }
                }
                return possibleValues.Count == 0;
            }
            return true;
        }

        public FlattenedThreshold[] FlattenedThresholds
        {
            get
            {
                if (flattenedThresholds == null)
                    BuildFlattenedThresholds(false);
                return flattenedThresholds;
            }
        }
        public FlattenedThreshold[] FlattenedThresholdsBaseline
        {
            get
            {
                if (flattenedThresholdsBaseline == null)
                    BuildFlattenedThresholds(true);
                return flattenedThresholdsBaseline;
            }
        }

        private void BuildFlattenedThresholds(bool baselineCheckTrue)
        {
            try
            {
                if (!baselineCheckTrue)
                {
                    LOG.Info("Building Flattened array for normal alert threshold.");
                    Threshold warningThreshold = GetThreshold(ThresholdItemType.Warning, baselineCheckTrue);
                    object value = warningThreshold.Value;
                    if (value is Threshold.ComparableList)
                    {
                        Threshold.ComparableList list = (Threshold.ComparableList)value;
                        IEnumerable<object> temp = Algorithms.SetDifference(GetPossibleValues<object>(), list);
                        list =
                            GetThreshold(ThresholdItemType.Critical, baselineCheckTrue).Value as Threshold.ComparableList;
                        temp = Algorithms.SetDifference(temp, list);
                        list =
                            GetThreshold(ThresholdItemType.Informational, baselineCheckTrue).Value as Threshold.ComparableList;
                        temp = Algorithms.SetDifference(temp, list);
                        list = new Threshold.ComparableList();
                        list.AddRange(temp);
                        okThreshold = new Threshold(true, warningThreshold.Op, list);
                    }
                    else
                    {
                        try
                        {
                            value = Convert.ChangeType(0, metaData.ValueType);
                            if (!(value is IComparable))
                                value = 0;
                        }
                        catch (Exception)
                        {
                            /* the value for the ok threshold is irrelevant in most cases so just use the warning value */
                        }
                        okThreshold = new Threshold(true, warningThreshold.Op, (IComparable)value);
                    }

                    FlattenedThreshold ok = new FlattenedThreshold(ThresholdItemType.OK, this, baselineCheckTrue);
                    FlattenedThreshold info = new FlattenedThreshold(ThresholdItemType.Informational, this, baselineCheckTrue);
                    FlattenedThreshold warning = new FlattenedThreshold(ThresholdItemType.Warning, this, baselineCheckTrue);
                    FlattenedThreshold critical = new FlattenedThreshold(ThresholdItemType.Critical, this, baselineCheckTrue);
                    flattenedThresholds = new FlattenedThreshold[] { ok, info, warning, critical };
                }
                else
                {
                    LOG.Info("Building Flattened array for baseline alert threshold.");
                    Threshold warningBaselineThreshold = GetThreshold(ThresholdItemType.Warning, baselineCheckTrue);
                    if (warningBaselineThreshold != null)
                    {
                        object value = warningBaselineThreshold.Value;
                        if (value is Threshold.ComparableList)
                        {
                            Threshold.ComparableList list = (Threshold.ComparableList)value;
                            IEnumerable<object> temp = Algorithms.SetDifference(GetPossibleValues<object>(), list);
                            list =
                                GetThreshold(ThresholdItemType.Critical, baselineCheckTrue).Value as Threshold.ComparableList;
                            temp = Algorithms.SetDifference(temp, list);
                            list =
                                GetThreshold(ThresholdItemType.Informational, baselineCheckTrue).Value as Threshold.ComparableList;
                            temp = Algorithms.SetDifference(temp, list);
                            list = new Threshold.ComparableList();
                            list.AddRange(temp);
                            okThresholdBaseline = new Threshold(true, warningBaselineThreshold.Op, list);
                        }
                        else
                        {
                            try
                            {
                                value = Convert.ChangeType(0, metaData.ValueType);
                                if (!(value is IComparable))
                                    value = 0;
                            }
                            catch (Exception)
                            {
                                /* the value for the ok threshold is irrelevant in most cases so just use the warning value */
                            }
                            okThresholdBaseline = new Threshold(true, warningBaselineThreshold.Op, (IComparable)value);
                        }
                    }
                    FlattenedThreshold okBaseline = new FlattenedThreshold(ThresholdItemType.OK, this, baselineCheckTrue);
                    FlattenedThreshold infoBaseline = new FlattenedThreshold(ThresholdItemType.Informational, this, baselineCheckTrue);
                    FlattenedThreshold warningBaseline = new FlattenedThreshold(ThresholdItemType.Warning, this, baselineCheckTrue);
                    FlattenedThreshold criticalBaseline = new FlattenedThreshold(ThresholdItemType.Critical, this, baselineCheckTrue);
                    flattenedThresholdsBaseline = new FlattenedThreshold[] { okBaseline, infoBaseline, warningBaseline, criticalBaseline };
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error in BuildFlattenedThrsholds method. " + ex);
                throw;
            }
        }

        public Threshold GetThreshold(ThresholdItemType itemType, bool isbaselineObject)
        {
            if (isbaselineObject)
            {
                switch (itemType)
                {
                    case ThresholdItemType.OK:
                        return okThresholdBaseline;
                    case ThresholdItemType.Informational:
                        return thresholdEntry.BaselineInfoThreshold;
                    case ThresholdItemType.Warning:
                        return thresholdEntry.BaselineWarningThreshold;
                    case ThresholdItemType.Critical:
                        return thresholdEntry.BaselineCriticalThreshold;
                }
            }
            else
            {
                switch (itemType)
                {
                    case ThresholdItemType.OK:
                        return okThreshold;
                    case ThresholdItemType.Informational:
                        return thresholdEntry.InfoThreshold;
                    case ThresholdItemType.Warning:
                        return thresholdEntry.WarningThreshold;
                    case ThresholdItemType.Critical:
                        return thresholdEntry.CriticalThreshold;
                }
            }
            return null;
        }

        public MetricDefinition GetMetaData()
        {
            return metaData;
        }

        public void RaisePropertyChanged(string name)
        {
            OnPropertyChanged(name);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (name == "Comments")
                commentsChanged = true;

            else
                IsThresholdChanged = true;
            if (name == "Rank")
                rankchanged = true;
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs pcea = new PropertyChangedEventArgs(name);
                PropertyChanged(this, pcea);
            }
        }

        public List<ItemType> GetPossibleValues<ItemType>()
        {
            List<ItemType> result = new List<ItemType>();

            MetricDefinition metaData = GetMetaData();

            if (metaData.ValueType.IsEnum)
            {
                foreach (object o in Enum.GetValues(metaData.ValueType))
                {
                    if (metaData.ValueType == typeof(ServiceState))
                    {
                        if (metricID != (int)Metric.SqlServiceStatus)
                        {
                            if (((ServiceState)o) == ServiceState.Paused)
                                continue;
                        }
                    }
                    result.Add((ItemType)o);
                }
            }

            return result;
        }

        /// <summary>
        /// Reset the change flags.
        /// </summary>
        internal void ChangesApplied()
        {
            commentsChanged = false;
            rankchanged = false;
            IsThresholdChanged = false;
            IsThresholdNew = false;
            //IsThresholdDeleted = false;
        }

        public object GetSuggestedValue(ThresholdItemType itemType)
        {
            switch (itemType)
            {
                case ThresholdItemType.OK:
                    return null;
                case ThresholdItemType.Warning:
                    return suggestedWarningThreshold;
                case ThresholdItemType.Critical:
                    return this.suggestedCriticalThreshold;
            }
            return null;
        }

        public void SetSuggestedWarningThreshold(object value)
        {
            suggestedWarningThreshold = value;
        }

        public void SetSuggestedCriticalThreshold(object value)
        {
            suggestedCriticalThreshold = value;
        }

        /// <summary>
        /// Creates a new AuditableEntity object that contains the name of the alert
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = this.Name;
            return auditable;
        }

        /// <summary>
        /// Creates a new AuditableEntity comparing the current AlertConfigurationItem with one that has the values before the user change them.
        /// </summary>
        /// <param name="oldValue">An AlertConfigurationItem that let this Alert to compare and create a new AuditableEntity</param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity auditable = GetAuditableEntity();

            if (this.Enabled)
            {
                if (!((AlertConfigurationItem)oldValue).Enabled)
                {
                    auditable.AddMetadataProperty("Alert status", "Enabled");
                }

                List<Pair<string, string>> allChangedProperties = GetDetailProperties(oldValue);
                foreach (var property in allChangedProperties)
                {
                    auditable.AddMetadataProperty(property.First, property.Second);
                }
            }
            else
            {
                if (((AlertConfigurationItem)oldValue).Enabled)
                {
                    auditable.AddMetadataProperty("Alert status", "Disabled");
                }
            }

            return auditable;
        }

        public List<Pair<string, string>> GetDetailProperties(IAuditable oldValue)
        {
            List<Pair<string, string>> allPropertiesChanged = new List<Pair<string, string>>();
            flattenedThresholds = flattenedThresholds ?? this.FlattenedThresholds; //SQLdm 8.6 (Ankit Srivastava) -- Resolved defect DE43787
            foreach (var threshold in flattenedThresholds)
            {
                allPropertiesChanged.Add(
                    new Pair<string, string>(string.Format("    {0}:", threshold.ThresholdItemType.ToString()),
                                             String.Empty));
                allPropertiesChanged.Add(
                    new Pair<string, string>("        Enabled",
                                             threshold.Enabled.ToString()));
                if (threshold.Enabled)
                {
                    if (threshold.IsMutuallyExclusive)
                    {
                        // If the threshold is mutually exclusive then we should not logg any aditional
                        // data
                        continue;
                    }

                    if (threshold.IsNumeric)
                    {
                        allPropertiesChanged.Add(
                            new Pair<string, string>("        Range Start", threshold.RangeStart.ToString()));
                        allPropertiesChanged.Add(
                            new Pair<string, string>("        Range End", threshold.RangeEnd.ToString()));
                    }
                    else if (threshold.IsMultiValued)
                    {
                        string parameterTextStart = "";
                        GetPropertyValue(threshold.RangeStart.ToString(), out parameterTextStart);
                        allPropertiesChanged.Add(new Pair<string, string>("        Values", parameterTextStart));
                    }
                }
            }
            return allPropertiesChanged;
        }

        /// <summary>
        /// Verify if the range is something different of an Int or an empty string
        /// </summary>
        /// <param name="range">The RangeStart or RangEnd.ToString()</param>
        /// <param name="parameterText">Fill with the value of the Range if accomplish with the conditions</param>
        /// <returns></returns>
        private static bool GetPropertyValue(string range, out string parameterText)
        {
            int possibleValue;
            if (!int.TryParse(range, out possibleValue))
            {
                if (!range.Equals(string.Empty))
                {
                    parameterText = range;
                    return true;
                }
            }
            parameterText = "None";
            return false;
        }
    }

    [Serializable]
    public class FlattenedThreshold
    {
        private const char INFINITY_CHAR = (char)0x221E;

        private ThresholdItemType itemType;
        private AlertConfigurationItem configItem;

        //10.0 SQLdm Srishti Purohit
        private bool isBaselineThreshold = false;

        public FlattenedThreshold(ThresholdItemType itemType, AlertConfigurationItem configItem, bool isBaselineThreshold)
        {
            this.itemType = itemType;
            this.configItem = configItem;
            this.isBaselineThreshold = isBaselineThreshold;
        }

        public ThresholdItemType ThresholdItemType
        {
            get { return itemType; }
        }

        public bool IsBaselineTypeAlert
        {
            get { return isBaselineThreshold; }
        }

        public AlertConfigurationItem GetConfigurationItem()
        {
            return configItem;
        }

        public bool IsEditable
        {
            get
            {
                //5.4.2
                ThresholdOptions options = configItem.GetMetaData().Options;
                if (itemType == ThresholdItemType.Informational) {
                    switch (configItem.MetricID)
                    {
                        case (int)Metric.AverageDataIOPercent:

                        case (int)Metric.AverageLogWritePercent:
                        case (int)Metric.MaxWorkerPercent:
                        case (int)Metric.MaxSessionPercent:
                        case (int)Metric.DatabaseAverageMemoryUsagePercent:
                        case (int)Metric.InMemoryStorageUsagePercent:
                        //case (int)Metric.CPUCreditBalanceHigh:
                            return false;
                    }
                        
                }

                if (itemType == ThresholdItemType.Critical || itemType == ThresholdItemType.Warning)
                {
                    switch (configItem.MetricID)
                    {
                        case (int)Metric.AverageDataIOPercentLow:

                        case (int)Metric.AverageLogWritePercentLow:
                        case (int)Metric.MaxWorkerPercentLow:
                        case (int)Metric.MaxSessionPercentLow:
                        case (int)Metric.DatabaseAverageMemoryUsagePercentLow:
                        case (int)Metric.InMemoryStorageUsagePercentLow:                        
                        case (int)Metric.ReadLatencyLow:
                        case (int)Metric.WriteLatencyLow:
                            return false;
                    }

                }

                switch (itemType)
                {
                    case ThresholdItemType.OK:
                        return (options & ThresholdOptions.OKDisabled) != ThresholdOptions.OKDisabled;
                    case ThresholdItemType.Informational:
                        return (options & ThresholdOptions.InfoDisabled) != ThresholdOptions.InfoDisabled;
                    case ThresholdItemType.Warning:
                        return (options & ThresholdOptions.WarningDisabled) != ThresholdOptions.WarningDisabled;
                    case ThresholdItemType.Critical:
                        return (options & ThresholdOptions.CriticalDisabled) != ThresholdOptions.CriticalDisabled;
                    //case ThresholdItemType.OKBaseline:
                    //    return (options & ThresholdOptions.OKDisabled) != ThresholdOptions.OKDisabled;
                    //case ThresholdItemType.InformationalBaseline:
                    //    return (options & ThresholdOptions.InfoDisabled) != ThresholdOptions.InfoDisabled;
                    //case ThresholdItemType.WarningBaseline:
                    //    return (options & ThresholdOptions.WarningDisabled) != ThresholdOptions.WarningDisabled;
                    //case ThresholdItemType.CriticalBaseline:
                    //    return (options & ThresholdOptions.CriticalDisabled) != ThresholdOptions.CriticalDisabled;
                }
                return false;
            }
        }

        public bool IsNumeric
        {
            get
            {
                return
                    (configItem.GetMetaData().Options & ThresholdOptions.NumericValue) == ThresholdOptions.NumericValue;
            }
        }

        public bool IsMultiValued
        {
            get
            {
                return (configItem.GetMetaData().Options & ThresholdOptions.ContainedValueList) == ThresholdOptions.ContainedValueList;
            }
        }

        public bool IsMutuallyExclusive
        {
            get
            {
                return
                    (configItem.GetMetaData().Options & ThresholdOptions.MutuallyExclusive) == ThresholdOptions.MutuallyExclusive;
            }
        }

        public bool IsContained
        {
            get
            {
                return
                    (configItem.GetMetaData().Options & ThresholdOptions.ContainedValueList) == ThresholdOptions.ContainedValueList;
            }
        }

        public bool Enabled
        {
            get
            {
                bool result = false;
                if (itemType == ThresholdItemType.OK)
                    result = true;
                else
                    result = configItem.GetThreshold(itemType, isBaselineThreshold) != null ? configItem.GetThreshold(itemType, isBaselineThreshold).Enabled : false;

                if (!IsMutuallyExclusive || itemType != ThresholdItemType.OK)
                    return result;

                // handle mutually exclusive for OK
                result = configItem.GetThreshold(ThresholdItemType.Warning, isBaselineThreshold).Enabled;
                if (!result)
                    result = configItem.GetThreshold(ThresholdItemType.Critical, isBaselineThreshold).Enabled;

                return !result;
            }
            set
            {
                if (itemType != ThresholdItemType.OK)
                {
                    bool old = configItem.GetThreshold(itemType, isBaselineThreshold).Enabled;
                    if (old != value)
                    {
                        if (IsMutuallyExclusive)
                        {
                            if ((bool)value == true) // If enabling a mutually exclusive threshold, disable all others
                                switch (itemType)
                                {
                                    case ThresholdItemType.Informational:
                                        configItem.GetThreshold(ThresholdItemType.Warning, isBaselineThreshold).Enabled = !value;
                                        configItem.GetThreshold(ThresholdItemType.Critical, isBaselineThreshold).Enabled = !value;
                                        break;
                                    case ThresholdItemType.Warning:
                                        configItem.GetThreshold(ThresholdItemType.Informational, isBaselineThreshold).Enabled = !value;
                                        configItem.GetThreshold(ThresholdItemType.Critical, isBaselineThreshold).Enabled = !value;
                                        break;
                                    case ThresholdItemType.Critical:
                                        configItem.GetThreshold(ThresholdItemType.Informational, isBaselineThreshold).Enabled = !value;
                                        configItem.GetThreshold(ThresholdItemType.Warning, isBaselineThreshold).Enabled = !value;
                                        break;
                                    //case ThresholdItemType.InformationalBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.WarningBaseline, isBaselineThreshold).Enabled = !value;
                                    //    configItem.GetThreshold(ThresholdItemType.CriticalBaseline, isBaselineThreshold).Enabled = !value;
                                    //    break;
                                    //case ThresholdItemType.WarningBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.InformationalBaseline).Enabled = !value;
                                    //    configItem.GetThreshold(ThresholdItemType.CriticalBaseline).Enabled = !value;
                                    //    break;
                                    //case ThresholdItemType.CriticalBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.InformationalBaseline).Enabled = !value;
                                    //    configItem.GetThreshold(ThresholdItemType.WarningBaseline).Enabled = !value;
                                    //    break;
                                }
                            else  // In the unlikely event a mutually exclusive threshold is getting disabled, make sure something is selected
                            {
                                switch (itemType)
                                {
                                    case ThresholdItemType.Informational:
                                        configItem.GetThreshold(ThresholdItemType.Warning, isBaselineThreshold).Enabled = !value;
                                        break;
                                    case ThresholdItemType.Warning:
                                        configItem.GetThreshold(ThresholdItemType.Informational, isBaselineThreshold).Enabled = !value;
                                        break;
                                    case ThresholdItemType.Critical:
                                        configItem.GetThreshold(ThresholdItemType.Informational, isBaselineThreshold).Enabled = !value;
                                        break;
                                    //case ThresholdItemType.InformationalBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.WarningBaseline).Enabled = !value;
                                    //    break;
                                    //case ThresholdItemType.WarningBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.InformationalBaseline).Enabled = !value;
                                    //    break;
                                    //case ThresholdItemType.CriticalBaseline:
                                    //    configItem.GetThreshold(ThresholdItemType.InformationalBaseline).Enabled = !value;
                                    //    break;
                                }
                            }
                        }
                        configItem.GetThreshold(itemType, isBaselineThreshold).Enabled = value;
                        configItem.RaisePropertyChanged("");
                    }
                }
            }
        }

        public object RangeStart
        {
            get
            {
                if (configItem.GetMetaData().ComparisonType == ComparisonType.LE)
                {
                    if (itemType == ThresholdItemType.OK)
                    {
                        if (isBaselineThreshold)
                            return GetConfigurationItem().GetMetaData().MaxValueBaseline;
                        else
                            return GetConfigurationItem().GetMetaData().MaxValue;
                    }
                }
                return Value;
            }
            set
            {
                Value = value;
            }
        }

        public object RangeEnd
        {
            get
            {
                MetricThresholdEntry entry = GetConfigurationItem().ThresholdEntry;
                ComparisonType Op = configItem.GetMetaData().ComparisonType;
                if (!isBaselineThreshold)
                {
                    switch (itemType)
                    {
                        case ThresholdItemType.OK:
                            if (entry.InfoThreshold.Enabled)
                                return entry.InfoThreshold.Value;
                            if (entry.WarningThreshold.Enabled)
                                return entry.WarningThreshold.Value;
                            if (entry.CriticalThreshold.Enabled)
                                return entry.CriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValue :
                                        GetConfigurationItem().GetMetaData().MinValue;
                        case ThresholdItemType.Informational:
                            if (entry.WarningThreshold.Enabled)
                                return entry.WarningThreshold.Value;
                            if (entry.CriticalThreshold.Enabled)
                                return entry.CriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValue :
                                        GetConfigurationItem().GetMetaData().MinValue;
                        case ThresholdItemType.Warning:
                            if (entry.CriticalThreshold.Enabled)
                                return entry.CriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValue :
                                        GetConfigurationItem().GetMetaData().MinValue;
                    }
                }
                else
                {
                    switch (itemType)
                    {

                        case ThresholdItemType.OK:
                            if (entry.BaselineInfoThreshold.Enabled)
                                return entry.BaselineInfoThreshold.Value;
                            if (entry.BaselineWarningThreshold.Enabled)
                                return entry.BaselineWarningThreshold.Value;
                            if (entry.BaselineCriticalThreshold.Enabled)
                                return entry.BaselineCriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValueBaseline :
                                        GetConfigurationItem().GetMetaData().MinValue;
                        case ThresholdItemType.Informational:
                            if (entry.BaselineWarningThreshold.Enabled)
                                return entry.BaselineWarningThreshold.Value;
                            if (entry.BaselineCriticalThreshold.Enabled)
                                return entry.BaselineCriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValueBaseline :
                                        GetConfigurationItem().GetMetaData().MinValue;
                        case ThresholdItemType.Warning:
                            if (entry.BaselineCriticalThreshold.Enabled)
                                return entry.BaselineCriticalThreshold.Value;
                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValueBaseline :
                                        GetConfigurationItem().GetMetaData().MinValue;
                        case ThresholdItemType.Critical:

                            return (Op == ComparisonType.GE) ?
                                        GetConfigurationItem().GetMetaData().MaxValueBaseline :
                                        GetConfigurationItem().GetMetaData().MinValue;
                    }
                }
                return (Op == ComparisonType.GE) ?
                            GetConfigurationItem().GetMetaData().MaxValue :
                            GetConfigurationItem().GetMetaData().MinValue;
                //if (configItem.GetMetaData().ComparisonType == ComparisonType.LE)
                //    return 0;

                //return GetConfigurationItem().GetMetaData().MaxValue;
            }
        }

        [AuditableAttribute(false)]
        public object Value
        {
            get
            {
                return configItem.GetThreshold(itemType, isBaselineThreshold).Value;
            }
            set
            {
                MetricDefinition metaData = configItem.GetMetaData();
                Type valueType = metaData.ValueType;
                if (valueType == typeof(object) && (metaData.Options & ThresholdOptions.NumericValue) == ThresholdOptions.NumericValue)
                    valueType = typeof(long);

                if (!valueType.IsAssignableFrom(value.GetType()))
                {
                    if (value is double)
                        value = Math.Floor((double)value);

                    value = Convert.ChangeType(value, valueType);
                }
                if (value is IComparable)
                {
                    if (isBaselineThreshold)
                    {
                        if (metaData.MaxValueBaseline.CompareTo(Convert.ToInt64(value)) < 0)
                            value = Convert.ChangeType(metaData.MaxValueBaseline, valueType);
                    }
                    else
                        if (metaData.MaxValue.CompareTo(Convert.ToInt64(value)) < 0)
                            value = Convert.ChangeType(metaData.MaxValue, valueType);
                }

                configItem.GetThreshold(itemType, isBaselineThreshold).Value = value;
                configItem.RaisePropertyChanged("Value");
            }
        }
        [AuditableAttribute(false)]
        public object SuggestedValue
        {
            get
            {
                if (itemType == ThresholdItemType.OK)
                    return null;

                MetricDefinition mmd = configItem.GetMetaData();
                if (itemType == ThresholdItemType.Warning)
                {
                    if ((mmd.Options & ThresholdOptions.WarningDisabled) == ThresholdOptions.WarningDisabled)
                        return null;
                }
                else
                    if (itemType == ThresholdItemType.Critical)
                    {
                        if ((mmd.Options & ThresholdOptions.CriticalDisabled) == ThresholdOptions.CriticalDisabled)
                            return null;
                    }

                if ((mmd.Options & ThresholdOptions.NumericValue) == ThresholdOptions.NumericValue)
                    return configItem.GetSuggestedValue(itemType);

                return null;
            }
        }

        public List<ItemType> GetPossibleValues<ItemType>()
        {
            List<ItemType> result = new List<ItemType>();

            if (configItem.GetMetaData().ValueType.IsEnum)
            {
                foreach (object o in Enum.GetValues(configItem.GetMetaData().ValueType))
                {
                    result.Add((ItemType)o);
                }
            }

            return result;
        }

        public void SetComparableList(IList<object> newList, bool contained)
        {
            Threshold.ComparableList comparableList = Value as Threshold.ComparableList;

            ArrayList deselected = new ArrayList(comparableList);

            // update the list with new items and determine items to be removed
            foreach (object o in newList)
            {
                deselected.Remove(o);
                if (!comparableList.Contains(o))
                    comparableList.Add(o);
            }
            // remove deselecte items from the list
            foreach (object o in deselected)
            {
                comparableList.Remove(o);
            }
            if (contained)
            {
                // deselected items from warning or critical move to ok
                if (itemType != ThresholdItemType.OK)
                {
                    Threshold okThreshold = configItem.GetThreshold(ThresholdItemType.OK, isBaselineThreshold);
                    Threshold.ComparableList okList = okThreshold.Value as Threshold.ComparableList;
                    if (okList != null)
                    {
                        foreach (object o in deselected)
                        {
                            okList.Add(o);
                        }
                    }
                }

                // fixup the values in the other 2 thresholds - an item can only be contained by one threshold at a time
                foreach (ThresholdItemType tit in Enum.GetValues(typeof(ThresholdItemType)))
                {
                    if (tit != itemType)
                    {
                        Threshold otherThreshold = configItem.GetThreshold(tit, isBaselineThreshold);
                        IList otherList = otherThreshold.Value as IList;
                        if (otherList != null)
                        {
                            foreach (object o in comparableList)
                            {
                                if (otherList.Contains(o))
                                    otherList.Remove(o);
                            }
                        }
                    }
                }
            }
            configItem.RaisePropertyChanged("Value");
        }
    }

    public enum ThresholdItemType
    {
        OK, Informational, Warning, Critical
            //, OKBaseline, InformationalBaseline, WarningBaseline, CriticalBaseline
    }

    public enum InstanceAction
    {
        Add,
        Edit,
        Delete
    }
    // Enum for Instance based Alerting
    public enum InstanceType
    {
        Disk,
        Database,
        Job,
        Unknown
    }
    //SQLDM 10.1 (Barkha Khatri)
    //SQLCORE-2462 fix
    public enum AlertSeverityForCWF
    {
        OK=1,
        Info = 2,
        Warning = 4,
        Critical = 8

    }

}
