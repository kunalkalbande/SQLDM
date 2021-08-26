using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using System.Globalization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Batches
{
    [Serializable]
    public class GetBatches : IGetBatches
    {
        List<BatchNameValue> _batches = null;

        public GetBatches()
        {
            _batches = BatchResourceReader.GetBatchNameValue();
        }

        public IList<BatchNameValue> Batches
        {
            get
            {
                return (_batches);
            }
        }
    }
}
