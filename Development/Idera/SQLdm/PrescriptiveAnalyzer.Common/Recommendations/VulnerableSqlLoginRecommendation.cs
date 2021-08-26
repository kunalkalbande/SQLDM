using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class VulnerableSqlLoginRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public readonly string Name;
        public string Policy { get; private set; }
        public string Expire { get; private set; }

        public VulnerableSqlLoginRecommendation(string Name, bool policy, bool expire)
            : base(RecommendationType.VulnerableSqlLogin)
        {
            this.Name = Name;
            Policy = (policy) ? "ON" : "OFF";
            Expire = (expire) ? "ON" : "OFF";
        }

        public VulnerableSqlLoginRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.VulnerableSqlLogin, recProp)
        {
            Name = recProp.GetString("Name");
            Policy = recProp.GetString("Policy");
            Expire = recProp.GetString("Expire");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Name", Name.ToString());
            prop.Add("Policy", Policy.ToString());
            prop.Add("Expire", Expire.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new VulnerableSqlLoginScriptGenerator(Name, Policy, Expire);
        }

        public bool IsScriptRunnable
        {
            get { return (true); }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new VulnerableSqlLoginScriptGenerator(Name, Policy , Expire);
        }

        public bool IsUndoScriptRunnable
        {
            get 
            {
                if (null == Policy) return false;
                if (null == Expire) return false;
                return (true); 
            }
        }
    }
}