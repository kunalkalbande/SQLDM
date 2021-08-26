using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class CpuEncryptedConnectionsRecommendation : CpuRecommendation
    {
        public UInt32 EncryptedConnections { get; private set; }
        public UInt64 Connections { get; private set; }
        public UInt64 BatchesPerSec { get; private set; }

        public CpuEncryptedConnectionsRecommendation(UInt32 encryptedConnections, UInt64 connections, UInt64 batchesPerSec)
            : base(RecommendationType.CpuEncryptedConnections)
        {
            EncryptedConnections = encryptedConnections;
            Connections = connections;
            BatchesPerSec = batchesPerSec;
        }
        public override int AdjustImpactFactor(int i)
        {
            if ((Connections > 10000) || (BatchesPerSec > 1000))
            {
                return (LOW_IMPACT + 1);
            }
            return (base.AdjustImpactFactor(i));
        }
    }
}
