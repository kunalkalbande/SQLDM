using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using System.ComponentModel;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources
{
    [Serializable]
    public class MasterRecommendations
    {
        static List<MasterRecommendation> masterRecommendationsInformation = new List<MasterRecommendation>();
        private static TypeConverter converter; 
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

        public static List<MasterRecommendation> MasterRecommendationsInformation
        {
            get { return masterRecommendationsInformation; }
            set { masterRecommendationsInformation = value; }
        }
        public static string GetCategory(string id)
        {
            try
            {
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Category;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Confidence_Factor;                
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Description;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Bitly;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Finding;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Plural_Form_Finding;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Impact_Explanation;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Plural_Form_Impact_Explanation;
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
              return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Relevance;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Impact_Factor;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Recommendation;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Plural_Form_Recommendation;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Tags;

                //if (!string.IsNullOrEmpty(concatenatedStrings))
                //{
                //    var tags = new List<string>();
                //    var tagArray = concatenatedStrings.Split(',');

                //    foreach (var tag in tagArray)
                //    {
                //        tags.Add(tag.Trim());
                //    }

                //    return tags;
                //}

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
                string xml = masterRecommendationsInformation.Find(r => r.RecommendationID == id).InfoLinks;
                if (!String.IsNullOrEmpty(xml))
                {
                    RecommendationLinks links = RecommendationLinksConverter.ConvertFromInvariantString(xml) as RecommendationLinks;
                    if (null != links) links.RemoveAll(delegate(RecommendationLink l)
                    {
                        if (null == l) return (true);
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Additional_Considerations;
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
                return masterRecommendationsInformation.Find(r => r.RecommendationID == id).Problem_Explanation;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static bool ContainsRecommendation(string id)
        {
            try
            {
                MasterRecommendation recommendationFound = masterRecommendationsInformation.Find(r => r.RecommendationID == id);
                if (recommendationFound.RecommendationID == id)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
