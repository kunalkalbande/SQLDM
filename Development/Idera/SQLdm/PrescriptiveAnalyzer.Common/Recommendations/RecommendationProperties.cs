using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    public class RecommendationProperties : Dictionary<string, string>
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RepositoryHelper");

        public RecommendationProperties(Dictionary<string, string> lstProperties)
            : base(lstProperties)
        {

        }
        public string GetString(string key)
        {
            if (this.ContainsKey(key))
                return this[key];
            return string.Empty;
        }

        public bool GetBool(string key)
        {
            if (this.ContainsKey(key))
                return FormatHelper.FormatStringToBool(this[key]);
            return false;
        }

        public int GetInt(string key)
        {
            int output;
            if (this.ContainsKey(key))
                if (int.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public TimeSpan GetTimeSpan(string key)
        {
            TimeSpan output;
            if (this.ContainsKey(key))
                if (TimeSpan.TryParse(this[key], out output))
                    return output;
            return TimeSpan.MinValue;
        }

        public float GetFloat(string key)
        {
            float output;
            if (this.ContainsKey(key))
                if (float.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public List<string> GetListOfStrings(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<List<string>>(this[key]);
            }
            catch { }
            return null;
        }

        public BatchStatements GetBatchStatements(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<BatchStatements>(this[key]);
            }
            catch { }
            return null;
        }

        public List<BatchStatements> GetBatchStatementsList(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<List<BatchStatements>>(this[key]);
            }
            catch { }
            return null;
        }

        public Common.Objects.Index GetIndex(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<Common.Objects.Index>(this[key]);
            }
            catch { }
            return null;
        }

        public OffendingSql GetOffendingSql(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<OffendingSql>(this[key]);
            }
            catch { }
            return null;
        }

        public List<DatabaseObjectName> GetDatabaseObjectNameList(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                    return DeSerializeXml<List<DatabaseObjectName>>(this[key]);
            }
            catch { }
            return null;
        }

        private T DeSerializeXml<T>(string val)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(val))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static string GetXml<T>(T obj)
        {
            XmlWriterSettings wrtStg = new XmlWriterSettings();

            wrtStg.OmitXmlDeclaration = true;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter sww = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sww, wrtStg);
            xmlSerializer.Serialize(writer, obj);
            return sww.ToString();
        }

        public AffectedBatches GetAffectedBatches(string key)
        {
            if (this.ContainsKey(key))
                return DeSerializeXml<AffectedBatches>(this[key]);
            return null;
        }

        public List<MissingIndexCost> GetMissingIndexCost(string key)
        {
            if (this.ContainsKey(key))
                return DeSerializeXml<List<MissingIndexCost>>(this[key]);
            return null;
        }
        public List<DbWithCompatibility> GetDbWithCompatibility(string key)
        {
            try
            {
                if (this.ContainsKey(key))
                {
                    return DeSerializeXml<List<DbWithCompatibility>>(this[key]);
                }

                LOG.Info("Got Db Compatibility for Recommendaation Q37 or Q44. Key is " + key + ".");
                return null;
            }
            catch (Exception ex)
            {
                LOG.Error("Error while adding Db Compatibility for Recommendaation Q37 or Q44." + ex.Message);
                return null;
            }
        }

        public double GetDouble(string key)
        {
            double output;
            if (this.ContainsKey(key))
                if (double.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public decimal GetDecimal(string key)
        {
            decimal output;
            if (this.ContainsKey(key))
                if (decimal.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public ulong GetULong(string key)
        {
            ulong output;
            if (this.ContainsKey(key))
                if (ulong.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public UInt64 GetUInt64(string key)
        {
            UInt64 output;
            if (this.ContainsKey(key))
                if (UInt64.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public UInt32 GetUInt32(string key)
        {
            UInt32 output;
            if (this.ContainsKey(key))
                if (UInt32.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public long GetLong(string key)
        {
            long output;
            if (this.ContainsKey(key))
                if (long.TryParse(this[key], out output))
                    return output;
            return 0;
        }

        public DateTime GetDateTime(string key)
        {
            DateTime output;
            if (this.ContainsKey(key))
                if (DateTime.TryParse(this[key], out output))
                    return output;
            return DateTime.MinValue;
        }

        public ulong? GetNullableULong(string key)
        {
            ulong output;
            if (this.ContainsKey(key))
                if (ulong.TryParse(this[key], out output))
                    return output;
            return null;
        }
        
    }
}
