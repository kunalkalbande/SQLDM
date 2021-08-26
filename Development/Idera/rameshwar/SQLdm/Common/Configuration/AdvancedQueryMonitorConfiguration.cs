using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class AdvancedQueryMonitorConfiguration
    {
        [Serializable]
        private enum FilterType
        {
            ApplicationLike,
            ApplicationMatch,
            DatabaseLike,
            DatabaseMatch,
            SqlTextLike,
            SqlTextMatch
        }

        private readonly Dictionary<FilterType, string[]> filters = new Dictionary<FilterType, string[]>();

        public AdvancedQueryMonitorConfiguration() : this(false)
        {
            
        }

        public AdvancedQueryMonitorConfiguration(bool initializeDefaults)
        {
            if (initializeDefaults)
            {
                ApplicationExcludeLike = new string[] { "SQLAgent%", "SQLDMO%" };
                ApplicationExcludeMatch = new string[] { "SQLProfiler" };
            }
        }

        [XmlArrayItem("Like")]
        public string[] ApplicationExcludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.ApplicationLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.ApplicationLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] ApplicationExcludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.ApplicationMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.ApplicationMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] DatabaseExcludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.DatabaseLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] DatabaseExcludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.DatabaseMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] SqlTextExcludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.SqlTextLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.SqlTextLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] SqlTextExcludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.SqlTextMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.SqlTextMatch, value);
            }
        }

        public string ApplicationExcludeToString()
        {
            return CombineStringArrays(ApplicationExcludeLike, ApplicationExcludeMatch);
        }

        public string DatabaseExcludeToString()
        {
            return CombineStringArrays(DatabaseExcludeLike, DatabaseExcludeMatch);
        }

        public string SqlTextExcludeToString()
        {
            return CombineStringArrays(SqlTextExcludeLike, SqlTextExcludeMatch);
        }

        private void SetStringArrayValue(FilterType filterType, string[] value)
        {
            if (value == null || value.Length == 0)
            {
                if (filters.ContainsKey(filterType))
                    filters.Remove(filterType);
            }
            else
            {
                if (filters.ContainsKey(filterType))
                    filters.Remove(filterType);
                filters.Add(filterType, value);
            }
        }

        private static string CombineStringArrays(IEnumerable<string> set1, IEnumerable<string> set2)
        {
            OrderedSet<string> orderedSet = new OrderedSet<string>();

            if (set1 != null)
            {
                foreach (string exclusion in set1)
                {
                    if (!orderedSet.Contains(exclusion))
                    {
                        orderedSet.Add(exclusion);
                    }
                }
            }

            if (set2 != null)
            {
                foreach (string exclusion in set2)
                {
                    if (!orderedSet.Contains(exclusion))
                    {
                        orderedSet.Add(exclusion);
                    }
                }
            }

            StringBuilder combinedString = new StringBuilder();

            foreach (string exclusion in orderedSet)
            {
                if (combinedString.Length != 0)
                {
                    combinedString.Append("; ");
                }

                combinedString.Append("[");
                combinedString.Append(exclusion);
                combinedString.Append("]");
            }

            return combinedString.ToString();
        }

        private static bool SetChanged(string[] left, string[] right)
        {
            if (left == null || left.Length == 0)
            {
                return (right != null && right.Length > 0);
            }

            if (right == null || right.Length == 0 || left.Length != right.Length)
            {
                return true;
            }

            return !Algorithms.EqualSets(left, right);
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            AdvancedQueryMonitorConfiguration configuration = obj as AdvancedQueryMonitorConfiguration;
            if (configuration == null) return false;
            if (SetChanged(ApplicationExcludeLike, configuration.ApplicationExcludeLike)) return false;
            if (SetChanged(ApplicationExcludeMatch, configuration.ApplicationExcludeMatch)) return false;
            if (SetChanged(DatabaseExcludeLike, configuration.DatabaseExcludeLike)) return false;
            if (SetChanged(DatabaseExcludeMatch, configuration.DatabaseExcludeMatch)) return false;
            if (SetChanged(SqlTextExcludeLike, configuration.SqlTextExcludeLike)) return false;
            if (SetChanged(SqlTextExcludeMatch, configuration.SqlTextExcludeMatch)) return false;
            return true;
        }

        public static string SerializeToXml(AdvancedQueryMonitorConfiguration configuration)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedQueryMonitorConfiguration));
            StringBuilder buffer = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, configuration);
                writer.Flush();
            }

            return buffer.ToString();
        }

        public static AdvancedQueryMonitorConfiguration DeserializeFromXml(string xml)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedQueryMonitorConfiguration));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                return (AdvancedQueryMonitorConfiguration)serializer.Deserialize(xmlReader);
            }
        }
    }
}
