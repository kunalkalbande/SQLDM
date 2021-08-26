using System;
using Idera.SQLdm.Common.Data;

namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public class ServerSummarySnapshots
    {
        private readonly Serialized<ServerOverview> serverOverviewSnapshot;
        private readonly Serialized<CustomCounterCollectionSnapshot> customCounterCollectionSnapshot;

        public ServerSummarySnapshots(
            Serialized<ServerOverview> serverOverviewSnapshot
            )
        {
            this.serverOverviewSnapshot = serverOverviewSnapshot;
            this.customCounterCollectionSnapshot = new Serialized<CustomCounterCollectionSnapshot>(new CustomCounterCollectionSnapshot());
        }

        public ServerSummarySnapshots(
            Serialized<ServerOverview> serverOverviewSnapshot,
            Serialized<CustomCounterCollectionSnapshot> customCounterCollectionSnapshot
            )
        {
            this.serverOverviewSnapshot = serverOverviewSnapshot;
            this.customCounterCollectionSnapshot = customCounterCollectionSnapshot;
        }

        public ServerOverview ServerOverviewSnapshot
        {
            get { return serverOverviewSnapshot; }
        }

        public CustomCounterCollectionSnapshot CustomCounterCollectionSnapshot
        {
            get { return customCounterCollectionSnapshot; }
        }
    }
}
