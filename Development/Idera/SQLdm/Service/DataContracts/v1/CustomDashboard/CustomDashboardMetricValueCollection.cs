using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.CustomDashboard
{
    [CollectionDataContract]
    public class MetricValueCollectionForCustomDashboard : List<MetricValueforCustomDashboard>
    {
        public MetricValueCollectionForCustomDashboard() : base() { }
        internal MetricValueCollectionForCustomDashboard(IEnumerable<MetricValueforCustomDashboard> c)
            : base()
        {
            if (c == null) return;
            foreach (var i in c) this.Add(i);
        }
    }
}
