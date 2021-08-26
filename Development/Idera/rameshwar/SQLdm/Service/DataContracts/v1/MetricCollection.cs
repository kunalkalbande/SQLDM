using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [CollectionDataContract]
    public class MetricCollection : List<Metric>
    {
        public MetricCollection() : base() { }
        internal MetricCollection(IEnumerable<Metric> c)
            : base()
        {
            if (c == null) return;
            foreach (var i in c) this.Add(i);
        }
    }    
}
