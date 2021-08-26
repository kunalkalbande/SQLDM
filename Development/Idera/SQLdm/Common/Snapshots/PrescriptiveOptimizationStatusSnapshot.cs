using System;
using System.Collections.Generic;
using System.Text;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// This class includes Recommendation details with Optimization Status and Error message details.
    /// </summary>
    [Serializable]
    public class PrescriptiveOptimizationStatusSnapshot : Snapshot
    {
        #region fields

        private List<IRecommendation> recommendations = null;
        
        #endregion
        #region constructors
       
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RecommendationSnapshotWithOptimizationStatus"/> class.
        /// </summary>
        public PrescriptiveOptimizationStatusSnapshot(List<IRecommendation> recommendationList)
        {
            this.recommendations = recommendationList;
        }
        #endregion
        #region properties

        /// <summary>
        /// Gets recommendation details.
        /// </summary>
        public List<IRecommendation> Recommendations
        {
            get { return recommendations; }
        }
        
        #endregion
    }
}
