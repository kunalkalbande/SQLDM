using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.State;
using Idera.SQLdoctor.Common.Values;
using Idera.SQLdoctor.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Services
{
    public interface IRecommendationEngine
    {
        Guid ConfigurationId { get; }
        void Cancel(bool async);
        IList<IRecommendation> GetRecommendations();
        IList<Exception> GetExceptions();
        AnalysisValues GetAnalysisValues();
        AnalysisStateInfoCollection GetAnalysisState();
    }
}
