using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Services
{
    [Serializable]
    public class BatchNameValue 
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public interface IGetBatches
    {
        IList<BatchNameValue> Batches { get; }
    }
}
