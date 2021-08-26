using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network recommendation report #75a
    /// </summary>
    [Serializable]
    public class NetCardErrorsRecommendation : NetworkRecommendation
    {
        public string Name { get; private set; }
        public UInt32 PacketsPerSec { get; private set; }
        public UInt32 PacketsOutboundErrors { get; private set; }
        public UInt32 PacketsReceivedErrors { get; private set; }
        public UInt32 Errors { get { return (PacketsOutboundErrors + PacketsReceivedErrors); } }

        public NetCardErrorsRecommendation() : base(RecommendationType.NetCardErrors) { }

        public NetCardErrorsRecommendation(string name, UInt32 totalPacketsPerSec, UInt32 totalPacketsOutboundErrorsPerSec, UInt32 totalPacketsReceivedErrorsPerSec)
            : this()
        {
            Name = name;
            PacketsPerSec = totalPacketsPerSec;
            PacketsOutboundErrors = totalPacketsOutboundErrorsPerSec;
            PacketsReceivedErrors = totalPacketsReceivedErrorsPerSec;
        }

        public NetCardErrorsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.NetCardErrors, recProp) 
        {
            Name = recProp.GetString("Name");
            PacketsPerSec = recProp.GetUInt32("PacketsPerSec");
            PacketsOutboundErrors = recProp.GetUInt32("PacketsOutboundErrors");
            PacketsReceivedErrors = recProp.GetUInt32("PacketsReceivedErrors");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Name", Name.ToString());
            prop.Add("PacketsPerSec", PacketsPerSec.ToString());
            prop.Add("PacketsOutboundErrors", PacketsOutboundErrors.ToString());
            prop.Add("PacketsReceivedErrors", PacketsReceivedErrors.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (Errors > 100) return (i + 1);
            return (base.AdjustImpactFactor(i));
        }
    }
}
