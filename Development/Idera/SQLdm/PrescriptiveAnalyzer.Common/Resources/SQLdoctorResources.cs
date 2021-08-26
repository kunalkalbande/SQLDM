using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using System.ComponentModel;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources
{
    public static class SQLdoctorResources
    {
        private static ResourceManager resourceMan;
        private static TypeConverter converter; 

        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources.SQLdoctor Recommendations", typeof(SQLdoctorResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        internal static TypeConverter RecommendationLinksConverter
        {
            get
            {
                if (object.ReferenceEquals(converter, null))
                {
                    converter = new RecommendationLinksConverter();
                }
                return converter;
            }
        }

        public static string GetCategory(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Category");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static int GetConfidenceFactor(string id)
        {
            try
            {
                string value = ResourceManager.GetString(id + "_Confidence_Factor");
                if (!String.IsNullOrEmpty(value))
                    return Convert.ToInt32(value);
            }
            catch (Exception)
            {
            }
            return 1;
        }

        public static string GetDescription(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Description");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string GetBitly(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_bitly");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string GetFinding(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Finding");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
        
        public static string GetPluralFinding(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Plural_Form_Finding");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string GetImpactExplanation(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Impact_Explanation");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string GetPluralImpactExplanation(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Plural_Form_Impact_Explanation");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static double GetRelevance(string id)
        {
            try
            {
                string value = ResourceManager.GetString(id + "_Relevance");
                if (!String.IsNullOrEmpty(value))
                    return Convert.ToDouble(value);
            }
            catch (Exception)
            {
            }
            return 0.0;
        }

        public static int GetImpactFactor(string id)
        {
            try
            {
                string value = ResourceManager.GetString(id + "_Impact_Factor");
                if (!String.IsNullOrEmpty(value))
                    return Convert.ToInt32(value);
            }
            catch (Exception)
            {
            }
            return 1;
        }

        public static string GetRecommendation(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Recommendation");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string GetPluralRecommendation(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Plural_Form_Recommendation");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static IList<string> GetTags(string id)
        {
            try
            {
                var concatenatedStrings = ResourceManager.GetString(id + "_Tags");

                if (!string.IsNullOrEmpty(concatenatedStrings))
                {
                    var tags = new List<string>();
                    var tagArray = concatenatedStrings.Split(',');

                    foreach (var tag in tagArray)
                    {
                        tags.Add(tag.Trim());
                    }

                    return tags;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static RecommendationLinks GetInfoLinks(string id)
        {
            try
            {
                string xml =  ResourceManager.GetString(id + "_InfoLinks");
                if (!String.IsNullOrEmpty(xml))
                {
                    RecommendationLinks links = RecommendationLinksConverter.ConvertFromInvariantString(xml) as RecommendationLinks;
                    if (null != links) links.RemoveAll(delegate(RecommendationLink l)
                    {
                        if (null == l) return(true);
                        if (string.IsNullOrEmpty(l.Link)) return (true);
                        //------------------------------------------------------
                        // remove links that have been commented out by placing an
                        // asterisk at the start of the link.
                        //
                        if (l.Link.StartsWith("*")) return (true);
                        return (false);
                    });
                    return (links);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        internal static string GetAdditionalConsiderations(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Additional_Considerations");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        internal static string GetProblemExplanation(string id)
        {
            try
            {
                return ResourceManager.GetString(id + "_Problem_Explanation");
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}
