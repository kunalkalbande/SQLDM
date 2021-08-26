using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class BufferPoolExtIOMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("BufferPoolExtIOMetrics");
        private int state; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M33)

        private long averageValue;  // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M33)
        private const int IOSpikeLimit = 20;
        private const int IOSpikeTimeInterval = 30;
        private int Interval;
        private List<long> list = new List<long>();

        public BufferPoolExtIOMetrics()
        {
        }

        public int State
        {
            get { return state; }
            set { state = value; }
        }
        public long AverageValue
        {
            get { return averageValue; }
            set { averageValue = value; }
        }
        public bool IsBufferPoolExtIOSpikeExist
        {
            get { return FindBufferPoolSpike(); }
        }

        private bool FindBufferPoolSpike()
        {
            bool spikeExist = false;
            int ioSpikeNumer = Interval > 0 ? IOSpikeTimeInterval / Interval : 0;
            if (list.Count > ioSpikeNumer)
            {
                for (int i = 0; i < list.Count - ioSpikeNumer; i++)
                {
                    bool IOSpikeLimitSatisfied = true;
                    for (int j = 0; j <= ioSpikeNumer; j++)
                    {
                        if (list[i + j] >= IOSpikeLimit)
                        {
                            IOSpikeLimitSatisfied = true;
                        }
                        else
                        {
                            IOSpikeLimitSatisfied = false;
                            break;
                        }
                    }

                    if (IOSpikeLimitSatisfied == true) { spikeExist = true; break; }

                }
            }
            return spikeExist;
        }
        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.BufferPoolExtIOSnapshotValueInterval);
        }
        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.BufferPoolExtIOSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("BufferPoolExtIO not added : " + snapshot.Error); return; }


            state = snapshot.State;
            Interval = snapshot.Interval;
            if (snapshot.CurrentValue != null && snapshot.CurrentValue.Count > 0)
            {
                list = snapshot.CurrentValue;
                long toFindAverage = 0;
                for (int index = 0; index < snapshot.CurrentValue.Count; index++)
                {
                    try
                    {
                        toFindAverage += snapshot.CurrentValue[index];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                }
                averageValue = toFindAverage / snapshot.CurrentValue.Count;
            }
        }

    }

}
