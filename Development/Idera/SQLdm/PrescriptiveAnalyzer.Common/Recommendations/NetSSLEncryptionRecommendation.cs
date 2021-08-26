using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Recommendations
{
    /// <summary>
    /// Network recommendation report #79a
    /// </summary>
    [Serializable]
    public class NetSSLEncryptionRecommendation : NetworkRecommendation
    {
        public UInt64 Connections { get; set; }
        public UInt64 BatchReqSec { get; set; }

        public NetSSLEncryptionRecommendation() : base(RecommendationType.NetSSLEncryption) { }

        public NetSSLEncryptionRecommendation(UInt64 connections, UInt64 batchReqSec)
            : this()
        {
            Connections = connections;
            BatchReqSec = batchReqSec;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (Connections > 10000) return (HIGH_IMPACT);
            if (BatchReqSec > 1000) return (HIGH_IMPACT);
            return (base.AdjustImpactFactor(i));
        }
    }
}
