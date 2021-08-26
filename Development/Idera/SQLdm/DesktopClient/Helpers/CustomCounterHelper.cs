using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Messages;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - Custom counter helper class for serialezing/deserializing custom counters and converting one object to serialisable object
    /// </summary>
    public static class CustomCounterHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Message");
        public static string CreateXML(CustomCounterSerializable customCounter)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CustomCounterSerializable));
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, customCounter);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while exporting custom counter {0}. Detailed error : {1}", customCounter.CounterDefinition.CounterName, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        static public string SerializeCustomCounter(Idera.SQLdm.DesktopClient.Views.Administration.CustomCounter counter)
        {
            try
            {
                CustomCounterSerializable count = GetSerializableCustomCounter(counter);
                return CreateXML(count);
            }
            catch
            {
                throw;
            }
        }

        static public CustomCounterSerializable DeserializeCustomCounter(string xmlPath)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CustomCounterSerializable));
                TextReader reader = new StreamReader(xmlPath);
                object obj = deserializer.Deserialize(reader);
                CustomCounterSerializable XmlData = (CustomCounterSerializable)obj;
                reader.Close();
                return XmlData;
            }
            catch(Exception ex)
            {
                LOG.ErrorFormat("Error occured in parsing custom counter xml ({0}) during import. Detailed error : {1}", xmlPath, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        public static CustomCounterSerializable GetSerializableCustomCounter(Idera.SQLdm.DesktopClient.Views.Administration.CustomCounter counter)
        {
            CustomCounterSerializable cc = new CustomCounterSerializable();
            cc.MetricDefinition = GetSerializableMetricDefinition(counter.GetMetricDefinition());
            cc.MetricDescription = GetSerializableMetricDescription(counter.GetMetricDescription());
            cc.CounterDefinition = GetSerializableCustomCounterDefinition(counter.GetCustomCounterDefinition());
            return cc;
        }

        public static Idera.SQLdm.DesktopClient.Views.Administration.CustomCounter GetCustomCounter(CustomCounterSerializable cc)
        {
            Idera.SQLdm.DesktopClient.Views.Administration.CustomCounter counter = new Views.Administration.CustomCounter(GetMetricDefinition(cc.MetricDefinition),
                GetMetricDescription(cc.MetricDescription), GetCustomCounterDefinition(cc.CounterDefinition));
            return counter;
        }

        public static MetricDefinitionSerializable GetSerializableMetricDefinition(Idera.SQLdm.Common.Events.MetricDefinition metricDefinition)
        {
            MetricDefinitionSerializable md = new MetricDefinitionSerializable();
            md.AlertEnabledByDefault = metricDefinition.AlertEnabledByDefault;
            md.ComparisonType = metricDefinition.ComparisonType;
            md.DefaultCriticalThresholdValue = metricDefinition.DefaultCriticalThresholdValue;
            md.DefaultInfoThresholdValue = metricDefinition.DefaultInfoThresholdValue;
            md.DefaultMessageID = metricDefinition.DefaultMessageID;
            md.DefaultWarningThresholdValue = metricDefinition.DefaultWarningThresholdValue;
            md.Description = metricDefinition.Description;
            md.EventCategoryInt = (uint)metricDefinition.EventCategory;
            md.IsDeleted = metricDefinition.IsDeleted;
            md.LastChanged = metricDefinition.LastChanged;
            md.MaxValue = metricDefinition.MaxValue;
            md.MetricCategory = metricDefinition.MetricCategory;
            md.MetricClass = metricDefinition.MetricClass;
            md.MetricID = metricDefinition.MetricID;
            md.MinValue = metricDefinition.MinValue;
            md.Name = metricDefinition.Name;
            md.Options = metricDefinition.Options;
            md.ProcessNotifications = metricDefinition.ProcessNotifications;
            md.Rank = metricDefinition.Rank;
            return md;
        }

        public static Idera.SQLdm.Common.Events.MetricDefinition GetMetricDefinition(MetricDefinitionSerializable md)
        {
            Idera.SQLdm.Common.Events.MetricDefinition metricDefinition = new Idera.SQLdm.Common.Events.MetricDefinition(md.MetricID);
            metricDefinition.AlertEnabledByDefault = md.AlertEnabledByDefault;
            metricDefinition.ComparisonType = md.ComparisonType;
            metricDefinition.DefaultCriticalThresholdValue = md.DefaultCriticalThresholdValue;
            metricDefinition.DefaultInfoThresholdValue = md.DefaultInfoThresholdValue;
            metricDefinition.DefaultMessageID = md.DefaultMessageID;
            metricDefinition.DefaultWarningThresholdValue = md.DefaultWarningThresholdValue;
            metricDefinition.Description = md.Description;
            metricDefinition.IsDeleted = md.IsDeleted;
            metricDefinition.MaxValue = md.MaxValue;
            metricDefinition.MetricClass = md.MetricClass;
            metricDefinition.MinValue = md.MinValue;
            metricDefinition.Name = md.Name;
            metricDefinition.Options = md.Options;
            metricDefinition.ProcessNotifications = md.ProcessNotifications;
            metricDefinition.Rank = md.Rank;
            metricDefinition.ValueType = md.ValueType;
            return metricDefinition;
        }

        public static MetricDescriptionSerializable GetSerializableMetricDescription(Idera.SQLdm.Common.Events.MetricDescription metricDescription)
        {
            MetricDescriptionSerializable md = new MetricDescriptionSerializable();
            md.Category = metricDescription.Category;
            md.Comments = metricDescription.Comments;
            md.Description = metricDescription.Description;
            md.Name = metricDescription.Name;
            md.Rank = metricDescription.Rank;
            return md;
        }

        public static Idera.SQLdm.Common.Events.MetricDescription GetMetricDescription(MetricDescriptionSerializable md)
        {
            Idera.SQLdm.Common.Events.MetricDescription metricDescription = new MetricDescription(md.Name,md.Category,md.Description,md.Comments,md.Rank);
            return metricDescription;
        }

        public static CustomCounterDefinitionSerializable GetSerializableCustomCounterDefinition(Idera.SQLdm.Common.Events.CustomCounterDefinition customCounterDefinition)
        {
            CustomCounterDefinitionSerializable ccd = new CustomCounterDefinitionSerializable();
            ccd.CalculationType = customCounterDefinition.CalculationType;
            ccd.CounterName = customCounterDefinition.CounterName;
            ccd.InstanceName = customCounterDefinition.InstanceName;
            ccd.IsEnabled = customCounterDefinition.IsEnabled;
            ccd.LastChanged = customCounterDefinition.LastChanged;
            ccd.MetricID = customCounterDefinition.MetricID;
            ccd.MetricType = customCounterDefinition.MetricType;
            ccd.ObjectName = customCounterDefinition.ObjectName;
            ccd.Scale = customCounterDefinition.Scale;
            ccd.ServerType = customCounterDefinition.ServerType;
            ccd.SqlStatement = customCounterDefinition.SqlStatement;
            return ccd;
        }

        public static Idera.SQLdm.Common.Events.CustomCounterDefinition GetCustomCounterDefinition(CustomCounterDefinitionSerializable ccd)
        {
            Idera.SQLdm.Common.Events.CustomCounterDefinition customCounterDefinition = new CustomCounterDefinition();
            customCounterDefinition.CalculationType = ccd.CalculationType;
            customCounterDefinition.CounterName = ccd.CounterName;
            customCounterDefinition.InstanceName = ccd.InstanceName;
            customCounterDefinition.IsEnabled = ccd.IsEnabled;
            customCounterDefinition.MetricType = ccd.MetricType;
            customCounterDefinition.ObjectName = ccd.ObjectName;
            customCounterDefinition.Scale = ccd.Scale;
            customCounterDefinition.ServerType = ccd.ServerType;
            customCounterDefinition.SqlStatement = ccd.SqlStatement;
            return customCounterDefinition;
        }
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represnets a custom counter which can be serialized
    /// </summary>
    public class CustomCounterSerializable
    {

        private MetricDefinitionSerializable metricDefinition;
        private MetricDescriptionSerializable metricDescription;
        private CustomCounterDefinitionSerializable counterDefinition;

        public MetricDefinitionSerializable MetricDefinition { get { return metricDefinition; } set { metricDefinition = value; } }
        public MetricDescriptionSerializable MetricDescription { get { return metricDescription; } set { metricDescription = value; } }
        public CustomCounterDefinitionSerializable CounterDefinition { get { return counterDefinition; } set { counterDefinition = value; } }
       
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents a MetriDefinition which can be serialised
    /// </summary>
    public class MetricDefinitionSerializable
    {
        #region fields

        // public static readonly Message DefaultMessageTemplate = new Message(0, "header", "body", "todo", "pulse");
        //public object sync = new object();

        private int metricID;
        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }
        private bool deleted;
        public bool IsDeleted
        {
            get { return deleted; }
            set { deleted = value; }
        }

        private DateTime lastChanged;
        public DateTime LastChanged
        {
            get { return lastChanged; }
            set { lastChanged = value; }
        }
        private ThresholdOptions options;
        public ThresholdOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        private MetricClass metricClass;
        public MetricClass MetricClass
        {
            get { return metricClass; }
            set { metricClass = value; }
        }
        private ComparisonType comparisonType;
        public ComparisonType ComparisonType
        {
            get { return comparisonType; }
            set { comparisonType = value; }
        }

        private int defaultMessageID;
        public int DefaultMessageID
        {
            get { return defaultMessageID; }
            set { defaultMessageID = value; }
        }
        private int rank;
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }
        private bool processNotifications;
        public bool ProcessNotifications
        {
            get { return processNotifications; }
            set { processNotifications = value; }
        }
        private bool alertEnabledByDefault;
        public bool AlertEnabledByDefault
        {
            get { return alertEnabledByDefault; }
            set { alertEnabledByDefault = value; }
        }

        private string name; //SQLDM 8.5 Mahesh: New field required for Rest Service
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string description; //SQLDM 8.5 Mahesh: New field required for Rest Service
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int minValue;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private long maxValue;
        public long MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        private long defaultInfoThreshold;
        public long DefaultInfoThresholdValue
        {
            get { return defaultInfoThreshold; }
            set { defaultInfoThreshold = value; }
        }
        private long defaultWarningThreshold;
        public long DefaultWarningThresholdValue
        {
            get { return defaultWarningThreshold; }
            set { defaultWarningThreshold = value; }
        }
        private long defaultCriticalThreshold;
        public long DefaultCriticalThresholdValue
        {
            get { return defaultCriticalThreshold; }
            set { defaultCriticalThreshold = value; }
        }

        private Type valueType;
        public Type ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        private MetricCategory metricCategory; //SQLDM 8.5 Mahesh: New field required for Rest Service
        public MetricCategory MetricCategory
        {
            get
            {
                if (Enum.IsDefined(typeof(MetricCategory), metricCategory))
                    return (MetricCategory)Enum.ToObject(typeof(MetricCategory), metricCategory);
                return MetricCategory.All;
            }
            set
            {
                metricCategory = value;
            }
        }
        private uint eventCategory;
        public uint EventCategoryInt
        {
            get { return eventCategory; }
            set { eventCategory = value; }
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
        public MetricDefinitionSerializable() { }
        /*
        public MetricDefinition(int metricID)
        {
            this.metricID = metricID;
            processNotifications = true;
            options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue;
            valueType = typeof(object);
            lastChanged = DateTime.MinValue;
        }

        internal MetricDefinition(IDataRecord record)
        {
            metricID = (int)record["Metric"];
            deleted = (bool)record["Deleted"];
            lastChanged = (DateTime)record["UTCLastChangeDateTime"];
            metricClass = (MetricClass)Enum.ToObject(typeof(MetricClass), (int)record["Class"]);
            options = (ThresholdOptions)Enum.ToObject(typeof(ThresholdOptions), (int)record["Flags"]);
            minValue = (int)record["MinValue"];
            maxValue = (long)record["MaxValue"];
            defaultInfoThreshold = (long)record["DefaultInfoValue"];
            defaultWarningThreshold = (long)record["DefaultWarningValue"];
            defaultCriticalThreshold = (long)record["DefaultCriticalValue"];
            processNotifications = (bool)record["DoNotifications"];
            eventCategory = Convert.ToUInt32(record["EventCategory"]);
            defaultMessageID = (int)record["DefaultMessageID"];
            alertEnabledByDefault = (bool)record["AlertEnabledDefault"];
            comparisonType = (ComparisonType)Enum.ToObject(typeof(ComparisonType), (int)record["ValueComparison"]);

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
        */
        #region Properties
        /*
        public bool HasAdditionalData
        {
            get { return (Options & ThresholdOptions.AdditionalData) == ThresholdOptions.AdditionalData; }
        }

        public bool IsConfigurable
        {
            get
            {
                return (Options & ThresholdOptions.MutuallyExclusive) != ThresholdOptions.MutuallyExclusive;
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
                
            }
            return Metric.Custom;
        }
*/
        #endregion

        #region Methods
        /*
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
            this.eventCategory = (uint)cat;
        }
        */
        #endregion
    }

    /// <summary>
    /// /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents a Metri description which can be serialised
    /// </summary>
    public struct MetricDescriptionSerializable
    {
        public string Name;
        public string Category;
        public string Description;
        private string comments;
        public int Rank;
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

    }

    /// <summary>
    /// /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents a custom counter definition which can be serialised
    /// </summary>
    public class CustomCounterDefinitionSerializable
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

        public CustomCounterDefinitionSerializable()
        {
            metricID = -1;
            MetricType = MetricType.SQLCounter;
            CalculationType = CalculationType.Value;
            enabled = true;
        }


        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }

        public bool IsEnabled
        {
            get { return enabled; }
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
            set { lastChanged = value; }
        }

        public CalculationType CalculationType
        {
            get { return calculationType; }
            set { calculationType = value; }
        }

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

        public string SqlStatement
        {
            get { return sqlStatement; }
            set { sqlStatement = value; }
        }

        public string ServerType
        {
            get { return serverType; }
            set { serverType = value; }
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
}
