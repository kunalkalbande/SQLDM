using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using System.Diagnostics;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan
{
    public static class ShowPlanUtils
    {
        private static XmlSerializer serializer;

        static ShowPlanUtils()
        {
            serializer = new XmlSerializer(typeof(ShowPlanXML));
        }

        public static ShowPlanXML GetPlan(string xml)
        {
            using (TextReader reader = new StringReader(xml))
            {
                using (XmlReader xmlreader = XmlTextReader.Create(reader))
                {
                    return (ShowPlanXML)serializer.Deserialize(xmlreader);
                }
            }
        }
       
        public static List<MissingIndexGroupType> GetRecommendedIndexes(ShowPlanXML plan)
        {
            List<MissingIndexGroupType> result = null;

            foreach (StmtBlockType[] blocks in plan.BatchSequence)
            {
                foreach (StmtBlockType block in blocks)
                {
                    foreach (BaseStmtInfoType bsit in block.Items)
                    {
                        if (bsit is StmtSimpleType)
                        {
                            StmtSimpleType sst = (StmtSimpleType)bsit;
                            if (sst.QueryPlan != null)
                            {
                                if (sst.QueryPlan.MissingIndexes != null && sst.QueryPlan.MissingIndexes.Length > 0)
                                {
                                    if (result == null)
                                        result = new List<MissingIndexGroupType>();
                                    result.AddRange(sst.QueryPlan.MissingIndexes);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
