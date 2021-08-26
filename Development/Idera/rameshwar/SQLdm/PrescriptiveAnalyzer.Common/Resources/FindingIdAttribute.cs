using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources
{
    public sealed class FindingIdAttribute : Attribute
    {
        private static Logger LOG = Logger.GetLogger("FindingIdAttribute");
        private static Dictionary<RecommendationType, string> recommendationToFindingMap = null;
        private static Dictionary<string, RecommendationType[]> findingToRecommendationMap = null;

        private readonly String ID;
        public FindingIdAttribute(string id)
        {
            ID = id;
        }

        public static string GetFindingId(RecommendationType type)
        {
            // this is hit too often and causing excessive logging.
            //using (LOG.VerboseCall("GetFindingId"))
            {
                lock (typeof(FindingIdAttribute))
                {
                    if (recommendationToFindingMap == null)
                    {
                        LoadRecommendationToFindingMap();
                    }
                }

                string result;
                if (!recommendationToFindingMap.TryGetValue(type, out result))
                {
                    LOG.Error("Finding ID not found for recommendation type: " + type.ToString());
                    result = String.Empty;
                }
                return result;
            }
        }

        public static RecommendationType[] GetRecommendationTypes(string findingId)
        {
            // this is hit too often and causing excessive logging.
            //using (LOG.VerboseCall("GetFindingId"))
            {
                lock (typeof(FindingIdAttribute))
                {
                    if (recommendationToFindingMap == null)
                    {
                        LoadRecommendationToFindingMap();
                    }
                }

                RecommendationType[] result;
                if (!findingToRecommendationMap.TryGetValue(findingId, out result))
                {
                    LOG.Error("Recommendation types not found for finding id: " + findingId);
                    result = null;
                }
                return result;
            }
        }


        private static void LoadRecommendationToFindingMap()
        {
            using (LOG.DebugCall("LoadRecommendationToFindingMap"))
            {
                lock (typeof(FindingIdAttribute))
                {
                    if (recommendationToFindingMap == null)
                    {
                        Dictionary<RecommendationType, string>  findingMap = new Dictionary<RecommendationType, string>();
                        Dictionary<string, RecommendationType[]> recommendationMap = new Dictionary<string, RecommendationType[]>();
                        
                        Type otype = typeof(RecommendationType);
                        Type ftype = typeof(FindingIdAttribute);

                        foreach (object value in Enum.GetValues(otype))
                        {
                            string name = Enum.GetName(otype, value);
                            FieldInfo field = otype.GetField(name);
                            if (field != null)
                            {
                                object[] attributes = field.GetCustomAttributes(ftype, true);
                                if (attributes.Length > 0)
                                {
                                    string id = ((FindingIdAttribute)attributes[0]).ID;
                                    findingMap.Add((RecommendationType)value, id ?? String.Empty);

                                    RecommendationType[] types;
                                    if (recommendationMap.TryGetValue(id, out types))
                                        recommendationMap.Remove(id);
                                    else
                                        types = new RecommendationType[0];

                                    RecommendationType[] newtypes = new RecommendationType[types.Length + 1];
                                    if (types.Length > 0)
                                        Array.Copy(types, newtypes, types.Length);

                                    newtypes[types.Length] = (RecommendationType)value;
                                    recommendationMap.Add(id, newtypes);
                                }
                                else
                                    LOG.Error("Finding ID not found for recommendation type: " + value.ToString());
                            }
                        }
                        recommendationToFindingMap = findingMap;
                        findingToRecommendationMap = recommendationMap;
                    }
                }
            }
        }
    }
}
