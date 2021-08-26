using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class UIRecommendation
    {
        private List<UILink> recommendationLinks = new List<UILink>();

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public float ComputedRankFactor { get; set; }

        [DataMember]
        public int AnalysisRecommendationID { get; set; }

        [DataMember]
        public bool IsFlagged { get; set; }

        [DataMember]
        public RecommendationOptimizationStatus OptimizationStatus { get; set; }

        [DataMember]
        public string OptimizationErrorMessage { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string DescriptionText { get; set; }

        [DataMember]
        public string FindingText { get; set; }

        [DataMember]
        public string ImpactExplanationText { get; set; }

        [DataMember]
        public int ImpactFactor { get; set; }

        [DataMember]
        public string RecommendationText { get; set; }

        [DataMember]
        public RecommendationType RecommendationType { get; set; }

        [DataMember]
        public double Relevance { get; set; }

        [DataMember]
        public RecommendationLinks Links { get; set; }

        [DataMember]
        public string ProblemExplanationText { get; set; }

        [DataMember]
        public string Database { get; set; }

        [DataMember]
        public string Schema { get; set; }

        [DataMember]
        public string Table { get; set; }

        [DataMember]
        public string DaysSinceLastCheck { get; set; }

        [DataMember]
        public Dictionary<string, string> Properties { get; set; }

        [DataMember]
        public List<UILink> RecommendationLinks
        {
            get
            {
                return recommendationLinks;
            }
            set
            {
                recommendationLinks = value;
            }
        }

        public UIRecommendation(IRecommendation rec, Dictionary<string, string> properties)
        {
            ID = rec.ID;
            ComputedRankFactor = rec.ComputedRankFactor;
            AnalysisRecommendationID = rec.AnalysisRecommendationID;
            IsFlagged = rec.IsFlagged;
            OptimizationStatus = rec.OptimizationStatus;
            OptimizationErrorMessage = rec.OptimizationErrorMessage;
            Category = rec.Category;
            DescriptionText = rec.DescriptionText;
            FindingText = rec.FindingText;
            ImpactExplanationText = rec.ImpactExplanationText;
            ImpactFactor = rec.ImpactFactor;
            RecommendationText = rec.RecommendationText;
            RecommendationType = rec.RecommendationType;
            Relevance = rec.Relevance;
            Links = rec.Links;
            ProblemExplanationText = rec.ProblemExplanationText;
            Properties = properties;

            if (rec.Links != null)
            {
                recommendationLinks = rec.Links.Select(link => new UILink()
                {
                    Link = link.Link,
                    Title = link.Title,
                    CondensedLink = link.CondensedLink,
                    Condition = link.Condition,
                }).ToList();
            }
        }
    }

    [DataContract]
    public class UILink
    {
        [DataMember]
        public string CondensedLink { get; set; }

        [DataMember]
        public string Condition { get; set; }

        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
