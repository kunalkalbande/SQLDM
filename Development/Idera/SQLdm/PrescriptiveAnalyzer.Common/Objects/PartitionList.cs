using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class PartitionList
    {
        public List<Partition> Partitions;

        public PartitionList()
        {
            Partitions = new List<Partition>();
        }

        public void Add(Partition x)
        {
            Partitions.Add(x);
        }

        public void Add(int partitionNumber, string dataCompressionDesc)
        {
            Partitions.Add(new Partition(partitionNumber, dataCompressionDesc));
        }

        public string GenerateIndexString()
        {
            StringBuilder builder = new StringBuilder();
            if (1 == Partitions.Count)
                builder.AppendLine(string.Format("      , DATA_COMPRESSION = {0}", Partitions[0].DataCompressionDesc));
            else
                foreach (Partition p in Partitions)
                    builder.AppendLine(string.Format("      , DATA_COMPRESSION = {0} ON PARTITIONS ({1})", p.DataCompressionDesc, p.PartitionNumber));
            return builder.ToString();    
        }
    }
}
