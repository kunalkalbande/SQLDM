using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuClockSpeedLostRecommendation : CpuRecommendation
    {
        public UInt32 CurrentCpuCount { get; private set; }
        public UInt32 PreviousCpuCount { get; private set; }
        public UInt64 PreviousSpeed { get; private set; }
        public UInt64 CurrentSpeed { get; private set; }
        public string LostPercent { get { return (string.Format("{0:0.#}%", SpeedLostPercent)); } }
        public double SpeedLostPercent { get { return (PreviousSpeed <= 0) ? 0 : 100.0 - ((CurrentSpeed * 100.0) / PreviousSpeed); } }

        public CpuClockSpeedLostRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuClockSpeedLost, recProp)
        {
            CurrentCpuCount = recProp.GetUInt32("CurrentCpuCount");
            PreviousCpuCount = recProp.GetUInt32("PreviousCpuCount");
            PreviousSpeed = recProp.GetUInt64("PreviousSpeed");
            CurrentSpeed = recProp.GetUInt64("CurrentSpeed");
        }

        public CpuClockSpeedLostRecommendation(UInt32 currentCpuCount, UInt32 previousCpuCount, UInt64 currentSpeed, UInt64 previousSpeed)
            : base(RecommendationType.CpuClockSpeedLost)
        {
            CurrentCpuCount = currentCpuCount;
            PreviousCpuCount = previousCpuCount;
            PreviousSpeed = previousSpeed;
            CurrentSpeed = currentSpeed;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("CurrentCpuCount", CurrentCpuCount.ToString());
            prop.Add("PreviousCpuCount", PreviousCpuCount.ToString());
            prop.Add("PreviousSpeed", PreviousSpeed.ToString());
            prop.Add("CurrentSpeed", CurrentSpeed.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // If sql server has lost 50% of its processing power, up the impact to high
            // 
            if (CurrentSpeed < (PreviousSpeed * .5))
            {
                return (HIGH_IMPACT);
            }
            return (base.AdjustImpactFactor(i));
        }
    }
}
