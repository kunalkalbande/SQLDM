using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Vim25Api;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - Custom counter helper class for serialezing/deserializing custom counters and converting one object to serialisable object
    /// </summary>
    public class AlertTemplateHelper
    {
       
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Message");
        public static string CreateXML(AlertTemplateSerializable alertTemplate)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AlertTemplateSerializable), Threshold.SUPPORTED_TYPES);
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, alertTemplate);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while exporting custom counter {0}. Detailed error : {1}", alertTemplate.Name, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        static public AlertTemplateSerializable DeserializeAlertTemplate(string xmlPath)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(AlertTemplateSerializable));
                TextReader reader = new StreamReader(xmlPath);
                object obj = deserializer.Deserialize(reader);
                AlertTemplateSerializable XmlData = (AlertTemplateSerializable)obj;
                reader.Close();
                return XmlData;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in parsing alert template xml ({0}) during import. Detailed error : {1}", xmlPath, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }


        public static AlertConfigurationItemSerializable GetAlertConfigurationItemSerializable(Idera.SQLdm.Common.Configuration.AlertConfigurationItem configItem)
        {
            AlertConfigurationItemSerializable configItemSerializable = new AlertConfigurationItemSerializable();
            configItemSerializable.Category=configItem.Category;
            if (configItem.ThresholdEntry.Data != null)
            {
                string data;
                Threshold.SerializeData(configItem.ThresholdEntry.Data, out data);
                configItemSerializable.Data = data;
            }
            string criticalThreshold, warningThreshold, infoThreshold;
            Threshold.Serialize(configItem.ThresholdEntry.CriticalThreshold, out criticalThreshold);
            Threshold.Serialize(configItem.ThresholdEntry.WarningThreshold, out warningThreshold);
            Threshold.Serialize(configItem.ThresholdEntry.InfoThreshold, out infoThreshold);
            configItemSerializable.CriticalThreshold = criticalThreshold;
            configItemSerializable.WarningThreshold = warningThreshold;
            configItemSerializable.InfoThreshold = infoThreshold;
            
            //configItemSerializable.CriticalThreshold = GetThresholdSerializable(configItem.ThresholdEntry.CriticalThreshold);
            //configItemSerializable.WarningThreshold = GetThresholdSerializable(configItem.ThresholdEntry.WarningThreshold);
            //configItemSerializable.InfoThreshold = GetThresholdSerializable(configItem.ThresholdEntry.InfoThreshold);
            configItemSerializable.MetricName=configItem.Name;
            configItemSerializable.Enabled = configItem.Enabled;
            configItemSerializable.MetricId = configItem.MetricID;
            configItemSerializable.ConfiguredAlertValue = configItem.ConfiguredAlertValue;
            return configItemSerializable;
        }

      
        public static AlertTemplateSerializable GetAlertTemplateSerializable(string name,string description,Idera.SQLdm.Common.Configuration.AlertConfiguration config)
        {
            AlertTemplateSerializable alertTemplate = new AlertTemplateSerializable();
            alertTemplate.Name = name;
            alertTemplate.Description = description;
            foreach (var item in config.ItemList)
            {
                if (item.MetricID < 1000)
                {
                    AlertConfigurationItemSerializable configItem = GetAlertConfigurationItemSerializable(item);
                    alertTemplate.AlertConfugurationItems.Add(configItem);
                }
            }
            return alertTemplate;
        }

        public static ThresholdSerializable GetThresholdSerializable(Threshold th)
        {
            ThresholdSerializable thsr = new ThresholdSerializable();
            thsr.enabled = th.Enabled;
            thsr.op = th.Op.ToString();
            thsr.value = (object)th.Value;
            return thsr;
        }

        public static Threshold GetThreshold(ThresholdSerializable thsr)
        {
            Threshold th = new Threshold();
            th.Enabled = thsr.enabled;
            th.Op = Threshold.Operator.GE;
            if (thsr.value is IComparable)
            {
                th.Value = thsr.value;
            } 
            return th;
        }
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represnets an alert configuration Item which can be serialized
    /// </summary>
    public class AlertConfigurationItemSerializable
    {
        public int MetricId;
        public string MetricName;
        public string Category;
        public string ConfiguredAlertValue;
        public string OkThreshold;
        public string WarningThreshold;
        public string CriticalThreshold;
        public string InfoThreshold;
        public string Data;
        public Boolean Enabled;
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represnets a Threshols which can be serialized
    /// </summary>
    public class ThresholdSerializable
    {
        public ThresholdSerializable()
        {
        }
        public bool enabled;
        public string op;
        public object value;
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represnets an alert template which can be serialized
    /// </summary>
    public class AlertTemplateSerializable
    {
        public string Name;
        public string Description;
        public List<AlertConfigurationItemSerializable> AlertConfugurationItems;
        public AlertTemplateSerializable()
        {
            AlertConfugurationItems = new List<AlertConfigurationItemSerializable>();
        }
    }

}
