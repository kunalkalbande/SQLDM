using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class Partition
    {
        public int PartitionNumber;
        public string DataCompressionDesc;
        public Partition() { }
        public Partition(int partitionNumber, string dataCompressionDesc)
        {
            PartitionNumber = partitionNumber;
            DataCompressionDesc = dataCompressionDesc;
        }
    }
}
