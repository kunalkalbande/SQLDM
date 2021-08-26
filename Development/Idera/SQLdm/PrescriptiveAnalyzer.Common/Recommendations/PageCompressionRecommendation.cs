using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class PageCompressionRecommendation : Recommendation, IProvideTableName
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string IndexName { get; set; }
        public int CurrentSizeKB { get; set; }
        public int CompressedSizeKB { get; set; }

        public string Database { get { return DatabaseName; } }
        public string Schema { get { return SchemaName; } }
        public string Table { get { return TableName; } } 

        internal PageCompressionRecommendation() : base(RecommendationType.DiskEnableCompression)
        {
        }

        public override int AdjustImpactFactor(int i)
        {
            if (CompressionPct > 70 && SavingsMB > 200)
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }

        public int CompressionPct
        {
            get
            {
                return (int)((1.0 - ((double)CompressedSizeKB) / ((double)CurrentSizeKB)) * 100.0);
            }
        }

        public int SavingsMB
        {
            get
            {
                return (int)(CurrentSizeKB - CompressedSizeKB) / 1024;
            }
        }


        public string ObjectName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!String.IsNullOrEmpty(IndexName))
                {
                    sb.Append("Index ").Append(IndexName).Append(" on ");
                }
                sb.Append("Table ").Append(SQLHelper.CreateBracketedString(DatabaseName));                                      
                sb.Append(".");                    
                if (!string.IsNullOrEmpty(SchemaName))
                    sb.Append(SQLHelper.CreateBracketedString(SchemaName));
                sb.Append(".").Append(SQLHelper.CreateBracketedString(TableName));
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}% savings of {2}MB", ObjectName, CompressionPct, SavingsMB);
        }
    }
}
