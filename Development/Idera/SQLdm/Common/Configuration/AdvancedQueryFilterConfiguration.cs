using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Idera.SQLdm.Common.Attributes;
using Wintellect.PowerCollections;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Common.Configuration
{

    // Supress the warning regarding overrides
#pragma warning disable 0659
    [Serializable]
    public class AdvancedQueryFilterConfiguration : ISerializable
    {
        [Serializable]
        private enum FilterType
        {
            ApplicationLike,
            ApplicationMatch,
            DatabaseLike,
            DatabaseMatch,
            SqlTextLike,
            SqlTextMatch,

            //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
            ApplicationLikeInclude,
            ApplicationMatchInclude,
            DatabaseLikeInclude,
            DatabaseMatchInclude,
            SqlTextLikeInclude,
            SqlTextMatchInclude
        }

        private readonly Dictionary<FilterType, string[]> filters;
        private int rowcount = 0;
        private bool excludeDM = true;

        public AdvancedQueryFilterConfiguration()
            : this(false)
        {

        }

        public AdvancedQueryFilterConfiguration(bool initializeDefaults)
        {
            filters = new Dictionary<FilterType, string[]>();

            if (initializeDefaults)
            {
                ApplicationExcludeLike = new string[] { "SQLAgent%", "SQLDMO%", "SQL Server Profiler%" };
                ApplicationExcludeMatch = new string[] { "SQLProfiler" };

                //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
            }
        }

        public AdvancedQueryFilterConfiguration(SerializationInfo info, StreamingContext context)
        {
            filters = new Dictionary<FilterType, string[]>();

            excludeDM = info.GetBoolean("excludeDM");
            rowcount = info.GetInt32("rowcount");
            ApplicationExcludeLike = (string[])info.GetValue("applike", typeof(string[]));
            ApplicationExcludeMatch = (string[])info.GetValue("appmatch", typeof(string[]));
            DatabaseExcludeLike = (string[])info.GetValue("dblike", typeof(string[]));
            DatabaseExcludeMatch = (string[])info.GetValue("dbmatch", typeof(string[]));
            SqlTextExcludeLike = (string[])info.GetValue("sqllike", typeof(string[]));
            SqlTextExcludeMatch = (string[])info.GetValue("sqlmatch", typeof(string[]));

            //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
            ApplicationIncludeLike = (string[])info.GetValue("applikeIncl", typeof(string[]));
            ApplicationIncludeMatch = (string[])info.GetValue("appmatchIncl", typeof(string[]));
            DatabaseIncludeLike = (string[])info.GetValue("dblikeIncl", typeof(string[]));
            DatabaseIncludeMatch = (string[])info.GetValue("dbmatchIncl", typeof(string[]));
            SqlTextIncludeLike = (string[])info.GetValue("sqllikeIncl", typeof(string[]));
            SqlTextIncludeMatch = (string[])info.GetValue("sqlmatchIncl", typeof(string[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("excludeDM", excludeDM);
            info.AddValue("rowcount", rowcount);

            info.AddValue("applike", ApplicationExcludeLike);
            info.AddValue("appmatch", ApplicationExcludeMatch);
            info.AddValue("dblike", DatabaseExcludeLike);
            info.AddValue("dbmatch", DatabaseExcludeMatch);
            info.AddValue("sqllike", SqlTextExcludeLike);
            info.AddValue("sqlmatch", SqlTextExcludeMatch);

            //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
            info.AddValue("applikeIncl", ApplicationIncludeLike);
            info.AddValue("appmatchIncl", ApplicationIncludeMatch);
            info.AddValue("dblikeIncl", DatabaseIncludeLike);
            info.AddValue("dbmatchIncl", DatabaseIncludeMatch);
            info.AddValue("sqllikeIncl", SqlTextIncludeLike);
            info.AddValue("sqlmatchIncl", SqlTextIncludeMatch);
        }

        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
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
        [AuditableAttribute(false)]
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

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
        public string[] ApplicationIncludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.ApplicationLikeInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.ApplicationLikeInclude, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Match")]
        [AuditableAttribute(false)]
        public string[] ApplicationIncludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.ApplicationMatchInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.ApplicationMatchInclude, value);
            }
        }


        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
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
        [AuditableAttribute(false)]
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

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
        public string[] DatabaseIncludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.DatabaseLikeInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseLikeInclude, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Match")]
        [AuditableAttribute(false)]
        public string[] DatabaseIncludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.DatabaseMatchInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseMatchInclude, value);
            }
        }

        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
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
        [AuditableAttribute(false)]
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

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Like")]
        [AuditableAttribute(false)]
        public string[] SqlTextIncludeLike
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.SqlTextLikeInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.SqlTextLikeInclude, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        [XmlArrayItem("Match")]
        [AuditableAttribute(false)]
        public string[] SqlTextIncludeMatch
        {
            get
            {
                string[] result;
                filters.TryGetValue(FilterType.SqlTextMatchInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.SqlTextMatchInclude, value);
            }
        }

        [Auditable("Excluded SQLdm queries from the results")]
        public bool ExcludeDM
        {
            get { return excludeDM; }
            set { excludeDM = value; }
        }

        /// <summary>
        /// Not presently used by Query Monitor
        /// </summary>
        [Auditable(false)]
        public int Rowcount
        {
            get { return rowcount; }
            set { rowcount = value; }
        }

        public string ApplicationExcludeToString()
        {
            return CombineStringArrays(ApplicationExcludeLike, ApplicationExcludeMatch);
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        public string ApplicationIncludeToString()
        {
            return CombineStringArrays(ApplicationIncludeLike, ApplicationIncludeMatch);
        }

        public string DatabaseExcludeToString()
        {
            return CombineStringArrays(DatabaseExcludeLike, DatabaseExcludeMatch);
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        public string DatabaseIncludeToString()
        {
            return CombineStringArrays(DatabaseIncludeLike, DatabaseIncludeMatch);
        }

        public string SqlTextExcludeToString()
        {
            return CombineStringArrays(SqlTextExcludeLike, SqlTextExcludeMatch);
        }

        //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
        public string SqlTextIncludeToString()
        {
            return CombineStringArrays(SqlTextIncludeLike, SqlTextIncludeMatch);
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
            try
            {
                if (this == obj) return true;
                AdvancedQueryFilterConfiguration configuration = obj as AdvancedQueryFilterConfiguration;
                if (configuration == null) return false;
                if (ExcludeDM != configuration.ExcludeDM) return false;
                if (SetChanged(ApplicationExcludeLike, configuration.ApplicationExcludeLike)) return false;
                if (SetChanged(ApplicationExcludeMatch, configuration.ApplicationExcludeMatch)) return false;
                if (SetChanged(DatabaseExcludeLike, configuration.DatabaseExcludeLike)) return false;
                if (SetChanged(DatabaseExcludeMatch, configuration.DatabaseExcludeMatch)) return false;
                if (SetChanged(SqlTextExcludeLike, configuration.SqlTextExcludeLike)) return false;
                if (SetChanged(SqlTextExcludeMatch, configuration.SqlTextExcludeMatch)) return false;

                //SQLdm 8.5 (Ankit Srivastava):  for Inclusion Filters
                if (SetChanged(ApplicationIncludeLike, configuration.ApplicationIncludeLike)) return false;
                if (SetChanged(ApplicationIncludeMatch, configuration.ApplicationIncludeMatch)) return false;
                if (SetChanged(DatabaseIncludeLike, configuration.DatabaseIncludeLike)) return false;
                if (SetChanged(DatabaseIncludeMatch, configuration.DatabaseIncludeMatch)) return false;
                if (SetChanged(SqlTextIncludeLike, configuration.SqlTextIncludeLike)) return false;
                if (SetChanged(SqlTextIncludeMatch, configuration.SqlTextIncludeMatch)) return false;

                if (!Equals(rowcount, configuration.Rowcount)) return false;
                return true;
            }
            // Exception may occur if the serialized copy of the object is from a previous version
            catch (Exception)
            {
                return false;
            }
        }

        public static string SerializeToXml(AdvancedQueryFilterConfiguration configuration)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedQueryFilterConfiguration));
            StringBuilder buffer = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, configuration);
                writer.Flush();
            }

            return buffer.ToString();
        }

        public static AdvancedQueryFilterConfiguration DeserializeFromXml(string xml)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedQueryFilterConfiguration));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                return (AdvancedQueryFilterConfiguration)serializer.Deserialize(xmlReader);
            }
        }
    }

    [Serializable]
    public class AdvancedQueryMonitorConfiguration : AdvancedQueryFilterConfiguration
    {
        public AdvancedQueryMonitorConfiguration()
            : this(false)
        {

        }

        public AdvancedQueryMonitorConfiguration(bool initializeDefaults)
            : base(initializeDefaults)
        {
        }

        public AdvancedQueryMonitorConfiguration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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

        new public static AdvancedQueryMonitorConfiguration DeserializeFromXml(string xml)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedQueryMonitorConfiguration));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                return (AdvancedQueryMonitorConfiguration)serializer.Deserialize(xmlReader);
            }
        }
    }
}
