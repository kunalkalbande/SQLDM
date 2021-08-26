using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Properties
{
    internal static class Constants
    {
        public const int MinScheduledRefreshIntervalSeconds = 30;
        public const int MaxScheduledRefreshIntervalSeconds = 1800;
        public const int MaxScheduledRefreshIntervalMinutes = MaxScheduledRefreshIntervalSeconds/60;
        public const int DefaultScheduledRefreshIntervalSeconds = 60;
        public const int MinServerPingIntervalSeconds = 30;
        public const int MaxServerPingIntervalSeconds = 600;
        public const int DefaultServerPingIntervalSeconds = 30;
        public const int MinDatabaseStatisticsIntervalSeconds = 60;
        public const int MaxDatabaseStatisticsIntervalSeconds = 86400;
        public const int DefaultDatabaseStatisticsIntervalSeconds = 3600;
    }
}
