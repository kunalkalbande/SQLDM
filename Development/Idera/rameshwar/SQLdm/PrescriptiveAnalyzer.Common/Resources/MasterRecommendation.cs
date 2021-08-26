using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources
{
    /// <summary>
    /// Stores properties for master recommendations which will be populated from DB.
    /// 7 aug 2015
    /// Srishti Purohit
    /// </summary>
    [Serializable]
    public class MasterRecommendation
    {
        private string recommendationID;

        public string RecommendationID
        {
            get { return recommendationID; }
            set { recommendationID = value; }
        }
        private string additional_Considerations;

        public string Additional_Considerations
        {
            get { return additional_Considerations; }
            set { additional_Considerations = value; }
        }

        private string bitly;

        public string Bitly
        {
            get { return bitly; }
            set { bitly = value; }
        }
        private string category;

        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        private int confidence_Factor;

        public int Confidence_Factor
        {
            get { return confidence_Factor; }
            set { confidence_Factor = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private string finding;

        public string Finding
        {
            get { return finding; }
            set { finding = value; }
        }
        private string impact_Explanation;

        public string Impact_Explanation
        {
            get { return impact_Explanation; }
            set { impact_Explanation = value; }
        }
        private int impact_Factor;

        public int Impact_Factor
        {
            get { return impact_Factor; }
            set { impact_Factor = value; }
        }
        private string infoLinks;

        public string InfoLinks
        {
            get { return infoLinks; }
            set { infoLinks = value; }
        }
        private string plural_Form_Finding;

        public string Plural_Form_Finding
        {
            get { return plural_Form_Finding; }
            set { plural_Form_Finding = value; }
        }
        private string plural_Form_Impact_Explanation;

        public string Plural_Form_Impact_Explanation
        {
            get { return plural_Form_Impact_Explanation; }
            set { plural_Form_Impact_Explanation = value; }
        }
        private string plural_Form_Recommendation;

        public string Plural_Form_Recommendation
        {
            get { return plural_Form_Recommendation; }
            set { plural_Form_Recommendation = value; }
        }
        private string problem_Explanation;

        public string Problem_Explanation
        {
            get { return problem_Explanation; }
            set { problem_Explanation = value; }
        }
        private string recommendation;

        public string Recommendation
        {
            get { return recommendation; }
            set { recommendation = value; }
        }
        private double relevance;

        public double Relevance
        {
            get { return relevance; }
            set { relevance = value; }
        }
        private IList<string> tags;

        public IList<string> Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        public MasterRecommendation(string id)
        {
            recommendationID = id;
        }
    }
}
