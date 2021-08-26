using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Configuration
{
    public class CheckListThresholds
    {
        private const int InvalidThreshold = -1;

        public int MostRecentAnalysisThreshold = InvalidThreshold;
        public int RecommendationThreshold = InvalidThreshold;
        public string PriorityThreshold = "";

        public int CPUUsageThreshold = InvalidThreshold;
        public int MemoryThreshold = InvalidThreshold;
        public int BlockingThreshold = InvalidThreshold;
        public int NetworkThreshold = InvalidThreshold;

        public int FullBackupThreshold = InvalidThreshold;
        public int LogBackupThreshold = InvalidThreshold;

        public int DiskSpaceThreshold = InvalidThreshold;
        public int DatabaseSpaceThreshold = InvalidThreshold;
        public int LogSpaceThreshold = InvalidThreshold;

        public int LongJobsThreshold = InvalidThreshold;
        public int FailedJobsThreshold = InvalidThreshold;
    }
}
