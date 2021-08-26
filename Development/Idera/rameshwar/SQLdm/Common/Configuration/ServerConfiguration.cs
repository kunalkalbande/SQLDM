using System;
using System.Collections.Generic;
using System.Xml;
using BBS.TracerX;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;


namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class ServerConfiguration
    {
        private static readonly Logger Log = Logger.GetLogger("ServerConfiguration");

        private readonly Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo _connectionInfo;
        private bool _enableOleAutomation;
        private bool _isProductionServer = true;
        private bool _isTransactionServer = true;
        private List<string> _blockedRecommendations = new List<string>();
        private List<string> _blockedDatabases = new List<string>();
        private List<string> _filteredRecommendations = new List<string>();
        //private List<HealthMetric> _healthMetrics;
        private object _lockHealthMetrics = new object();
        //private CheckListThresholds _checkListThresholds;
        private object _lockCheckListThresholds = new object();

        public ServerConfiguration()
            : this(new Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo())
        {

        }

        public ServerConfiguration(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            _connectionInfo = connectionInfo;
        }

        public string InstanceName
        {
            get { return _connectionInfo.InstanceName; }
            set { _connectionInfo.InstanceName = value; }
        }

        public Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        public bool EnableOleAutomation
        {
            get { return _enableOleAutomation; }
            set { _enableOleAutomation = value; }
        }

        public bool IsProductionServer
        {
            get { return _isProductionServer; }
            set { _isProductionServer = value; }
        }

        public bool IsTransactionServer
        {
            get { return _isTransactionServer; }
            set { _isTransactionServer = value; }
        }

        public IEnumerable<string> BlockedRecommendations
        {
            get { return _blockedRecommendations; }
            set
            {
                _blockedRecommendations.Clear();
                _blockedRecommendations.AddRange(value);
            }
        }

        public IEnumerable<string> BlockedDatabases
        {
            get { return _blockedDatabases; }
            set
            {
                _blockedDatabases.Clear();
                _blockedDatabases.AddRange(value);
            }
        }

        public IEnumerable<string> FilteredRecommendations
        {
            get { return _filteredRecommendations; }
            set
            {
                _filteredRecommendations.Clear();
                _filteredRecommendations.AddRange(value);
            }
        }

        //public List<HealthMetric> HealthMetrics
        //{
        //    get { return _healthMetrics; }
        //    set { _healthMetrics = value; }
        //}

        //public CheckListThresholds CheckListThreshold
        //{
        //    get { return _checkListThresholds; }
        //    set { _checkListThresholds = value; }
        //}

        public void AddFilteredRecommendation(string recommendationKey)
        {
            if (_filteredRecommendations != null && !_filteredRecommendations.Contains(recommendationKey))
            {
                _filteredRecommendations.Add(recommendationKey);
            }
        }

        public void RemoveFilteredRecommendation(string recommendationKey)
        {
            if (_filteredRecommendations != null)
            {
                _filteredRecommendations.Remove(recommendationKey);
            }
        }

        public void AddBlockedRecommendation(string recommendationId)
        {
            if (_blockedRecommendations != null && !_blockedRecommendations.Contains(recommendationId))
            {
                _blockedRecommendations.Add(recommendationId);
            }
        }

        public void AddBlockedDatabase(string db)
        {
            if (_blockedDatabases != null)
            {
                if (!_blockedDatabases.Contains(db)) _blockedDatabases.Add(db);
            }
        }

        public override string ToString()
        {
            return InstanceName;
        }

        public List<RecommendationType> GetBlockedRecommendationTypes()
        {
            var blockedRecommendationTypes = new List<RecommendationType>();

            foreach (var recommendationId in _blockedRecommendations)
            {
                var recommendationsTypes = FindingIdAttribute.GetRecommendationTypes(recommendationId);

                if (recommendationsTypes != null)
                {
                    blockedRecommendationTypes.AddRange(recommendationsTypes);
                }
            }

            return blockedRecommendationTypes;
        }

        public static ServerConfiguration DeserializeXml(XmlNode element)
        {
            if (element == null) return null;

            ServerConfiguration serverConfiguration;

            try
            {
                var connectionInfoNode = element["ConnectionInfo"];
                serverConfiguration = new ServerConfiguration(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo.DeserializeXml(connectionInfoNode));
            }
            catch
            {
                serverConfiguration = new ServerConfiguration();
                Log.Error("Unable to find connection info node.");
            }

            try
            {
                var analysisConfigurationNode = element["AnalysisConfiguration"];
                
                var attribute = GetXmlAttributeValue(analysisConfigurationNode, "EnableOleAutomation");
                if (attribute != null) serverConfiguration.EnableOleAutomation = Convert.ToBoolean(attribute.Value);
                
                attribute = GetXmlAttributeValue(analysisConfigurationNode, "IsProductionServer");
                if (attribute != null) serverConfiguration.IsProductionServer = Convert.ToBoolean(attribute.Value);
                
                attribute = GetXmlAttributeValue(analysisConfigurationNode, "IsTransactionServer");
                if (attribute != null) serverConfiguration.IsTransactionServer = Convert.ToBoolean(attribute.Value);
            }
            catch (Exception)
            {
                Log.Error("Unable to find analysis configuration node.");
            }

            try
            {
                var blockedRecommendationsNode = element["BlockedRecommendations"];

                if (blockedRecommendationsNode != null)
                {
                    var blockedRecommendations = new List<string>();

                    foreach (XmlNode recommendationNode in blockedRecommendationsNode.ChildNodes)
                    {
                        if (recommendationNode.Name != "Recommendation") continue;
                        var attribute = GetXmlAttributeValue(recommendationNode, "Id");
                        if (attribute != null) blockedRecommendations.Add(attribute.Value);
                    }

                    serverConfiguration.BlockedRecommendations = blockedRecommendations;
                }
            }
            catch (Exception e1)
            {
                Log.Error("Error reading blocked recommendations node.", e1);
            }

            try
            {
                var blockedDatabasesNode = element["BlockedDatabases"];

                if (blockedDatabasesNode != null)
                {
                    var blockedDatabases = new List<string>();

                    foreach (XmlNode databaseNode in blockedDatabasesNode.ChildNodes)
                    {
                        if (databaseNode.Name != "Database") continue;
                        var attribute = GetXmlAttributeValue(databaseNode, "Name");
                        if (attribute != null) blockedDatabases.Add(attribute.Value);
                    }

                    serverConfiguration.BlockedDatabases = blockedDatabases;
                }
            }
            catch (Exception e1)
            {
                Log.Error("Error reading blocked databases node.", e1);
            }

            //try
            //{
            //    var filteredRecommendationsNode = element["FilteredRecommendations"];

            //    if (filteredRecommendationsNode != null)
            //    {
            //        var filteredRecommendations = new List<string>();

            //        foreach (XmlNode recommendationNode in filteredRecommendationsNode.ChildNodes)
            //        {
            //            if (recommendationNode.Name != "Recommendation") continue;
            //            var attribute = GetXmlAttributeValue(recommendationNode, "Key");
            //            if (attribute != null) filteredRecommendations.Add(attribute.Value);
            //        }

            //        serverConfiguration.FilteredRecommendations = filteredRecommendations;
            //    }
            //}
            //catch (Exception e1)
            //{
            //    Log.Error("Error reading filtered recommendations node.", e1);
            //}
            //serverConfiguration.LoadHealthMetrics();
            //serverConfiguration.LoadCheckListThresholds();
            return serverConfiguration;
        }

        private static XmlAttribute GetXmlAttributeValue(XmlNode element, string attributeName)
        {
            if (element == null || string.IsNullOrEmpty(attributeName)) return null;

            try
            {
                return element.Attributes[attributeName];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XmlElement SerializeXml(XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }

            var serverElement = xmlDocument.CreateElement("Server");
            
            serverElement.AppendChild(_connectionInfo.SerializeXml(xmlDocument));

            var analysisConfigurationElement = xmlDocument.CreateElement("AnalysisConfiguration");
            var attribute = xmlDocument.CreateAttribute("EnableOleAutomation");
            attribute.Value = _enableOleAutomation.ToString();
            analysisConfigurationElement.Attributes.Append(attribute);
            attribute = xmlDocument.CreateAttribute("IsProductionServer");
            attribute.Value = _isProductionServer.ToString();
            analysisConfigurationElement.Attributes.Append(attribute);
            attribute = xmlDocument.CreateAttribute("IsTransactionServer");
            attribute.Value = _isTransactionServer.ToString();
            analysisConfigurationElement.Attributes.Append(attribute);
            serverElement.AppendChild(analysisConfigurationElement);

            var blockedRecommendationsElement = xmlDocument.CreateElement("BlockedRecommendations");
            foreach (var recommendationId in _blockedRecommendations)
            {
                var recommendationElement = xmlDocument.CreateElement("Recommendation");
                attribute = xmlDocument.CreateAttribute("Id");
                attribute.Value = recommendationId;
                recommendationElement.Attributes.Append(attribute);
                blockedRecommendationsElement.AppendChild(recommendationElement);
            }
            serverElement.AppendChild(blockedRecommendationsElement);

            var blockedDatabasesElement = xmlDocument.CreateElement("BlockedDatabases");
            foreach (var database in _blockedDatabases)
            {
                var databaseElement = xmlDocument.CreateElement("Database");
                attribute = xmlDocument.CreateAttribute("Name");
                attribute.Value = database;
                databaseElement.Attributes.Append(attribute);
                blockedDatabasesElement.AppendChild(databaseElement);
            }
            serverElement.AppendChild(blockedDatabasesElement);

            //var filteredRecommendationsElement = xmlDocument.CreateElement("FilteredRecommendations");
            //foreach (var recommendationKey in _filteredRecommendations)
            //{
            //    var recommendationElement = xmlDocument.CreateElement("Recommendation");
            //    attribute = xmlDocument.CreateAttribute("Key");
            //    attribute.Value = recommendationKey;
            //    recommendationElement.Attributes.Append(attribute);
            //    filteredRecommendationsElement.AppendChild(recommendationElement);
            //}
            //serverElement.AppendChild(filteredRecommendationsElement);

            return serverElement;
        }

        //public void SaveHealthMetrics()
        //{
        //    string settingsPath = CommonSettings.Default.GetSettingsPath();
        //    if (!Directory.Exists(settingsPath))
        //    {
        //        Directory.CreateDirectory(settingsPath);
        //    }
        //    string fullPath = Path.Combine(settingsPath, string.Format("{0}_Baseline.xml", InstanceName.Replace("\\", "_")));

        //    lock (_lockHealthMetrics)
        //    {
        //        XmlSerializer mySerializer = new XmlSerializer(typeof(List<HealthMetric>));
        //        using (FileStream myFileStream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            mySerializer.Serialize(myFileStream, HealthMetrics);
        //            myFileStream.Close();
        //        }
        //    }
        //}

        //public void LoadHealthMetrics()
        //{
        //    string settingsPath = CommonSettings.Default.GetSettingsPath();
        //    string fullPath = Path.Combine(settingsPath, string.Format("{0}_Baseline.xml", InstanceName.Replace("\\", "_")));
        //    lock (_lockHealthMetrics)
        //    {
        //        if (File.Exists(fullPath))
        //        {
        //            XmlSerializer mySerializer = new XmlSerializer(typeof(List<HealthMetric>));
        //            using (FileStream myFileStream = new FileStream(fullPath, FileMode.Open))
        //            {
        //                HealthMetrics = (List<HealthMetric>)mySerializer.Deserialize(myFileStream);
        //                myFileStream.Close();
        //            }
        //        }
        //        else
        //        {
        //            HealthMetrics = new List<HealthMetric>();
        //        }
        //    }
        //}

        //public void SaveCheckListThresholds()
        //{
        //    string settingsPath = CommonSettings.Default.GetSettingsPath();
        //    if (!Directory.Exists(settingsPath))
        //    {
        //        Directory.CreateDirectory(settingsPath);
        //    }
        //    string fullPath = Path.Combine(settingsPath, string.Format("{0}_CheckListThresholds.xml", InstanceName.Replace("\\", "_")));

        //    lock (_lockCheckListThresholds)
        //    {
        //        XmlSerializer mySerializer = new XmlSerializer(typeof(CheckListThresholds));
        //        using (FileStream myFileStream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            mySerializer.Serialize(myFileStream, CheckListThreshold);
        //            myFileStream.Close();
        //        }
        //    }
        //}

        //public void LoadCheckListThresholds()
        //{
        //    string settingsPath = CommonSettings.Default.GetSettingsPath();
        //    string fullPath = Path.Combine(settingsPath, string.Format("{0}_CheckListThresholds.xml", InstanceName.Replace("\\", "_")));
        //    lock (_lockCheckListThresholds)
        //    {
        //        if (File.Exists(fullPath))
        //        {
        //            XmlSerializer mySerializer = new XmlSerializer(typeof(CheckListThresholds));
        //            using (FileStream myFileStream = new FileStream(fullPath, FileMode.Open))
        //            {
        //                CheckListThreshold = (CheckListThresholds)mySerializer.Deserialize(myFileStream);
        //                myFileStream.Close();
        //            }
        //        }
        //        else
        //        {
        //            CheckListThreshold = new CheckListThresholds();
        //        }
        //    }
        //}
    }
}
