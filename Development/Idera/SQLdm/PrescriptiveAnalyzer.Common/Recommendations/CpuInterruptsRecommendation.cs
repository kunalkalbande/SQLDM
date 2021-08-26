using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuInterruptsRecommendation : CpuRecommendation
    {
        public double PercentInterruptTime { get; private set; }
        public double DeviceReqPerInterrupt { get; private set; }
        public CpuInterruptsRecommendation(RecommendationProperties recProp) : base(RecommendationType.CpuInterrupts, recProp) {
            PercentInterruptTime = recProp.GetDouble("PercentInterruptTime");
            DeviceReqPerInterrupt = recProp.GetDouble("DeviceReqPerInterrupt");
        }
        public CpuInterruptsRecommendation(UInt64 activity, UInt32 interruptsPerSec, double interruptsTime)
            : this(RecommendationType.CpuInterrupts, activity, interruptsPerSec, interruptsTime)
        {
        }
        public CpuInterruptsRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
        {
            PercentInterruptTime = recProp.GetDouble("PercentInterruptTime");
            DeviceReqPerInterrupt = recProp.GetDouble("DeviceReqPerInterrupt");
        
        }
        public CpuInterruptsRecommendation(RecommendationType rt, UInt64 activity, UInt32 interruptsPerSec, double interruptsTime)
            : base(rt)
        {
            PercentInterruptTime = Math.Round(interruptsTime, 1);
            DeviceReqPerInterrupt = (0 == interruptsPerSec) ? 0 : Math.Round(activity / (double)interruptsPerSec, 1);
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("PercentInterruptTime", PercentInterruptTime.ToString());
            prop.Add("DeviceReqPerInterrupt", DeviceReqPerInterrupt.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // If interrupt time os over 15%, this is high impact.
            // 
            if (PercentInterruptTime > 15)
            {
                return (HIGH_IMPACT);
            }
            return (base.AdjustImpactFactor(i));
        }
        public override int  AdjustConfidenceFactor(int i)
        {
            if (PercentInterruptTime > 1.5)
            {
                //----------------------------------------------------------------------------
                // Our confidence should increase by 2 for each 10% over 1.5% of interrupt time.
                // 
                double t = PercentInterruptTime - 1.5;
                int adjust = ((int)(t / 10)) * 2;
                i += adjust; 
            }
            return base.AdjustConfidenceFactor(i);
        }
    }
}
