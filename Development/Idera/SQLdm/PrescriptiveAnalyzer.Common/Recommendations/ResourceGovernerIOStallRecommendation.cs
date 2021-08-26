using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLdm10.0 -Srishti Purohit - New Recommendations - SDR-D23 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class ResourceGovernerIOStallRecommendation : Recommendation//, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public List<string> ResourcePoolNameList { get; private set; }

        public ResourceGovernerIOStallRecommendation(List<string> nameLsit)
            : base(RecommendationType.ResourceGovernerIOStall)
        {
            ResourcePoolNameList = nameLsit;
        }
        public ResourceGovernerIOStallRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.ResourceGovernerIOStall, recProp)
        {
            ResourcePoolNameList = recProp.GetListOfStrings("ResourcePoolNameList");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ResourcePoolNameList", RecommendationProperties.GetXml<List<string>>(ResourcePoolNameList));
            return prop;
        }
        //public IScriptGenerator GetScriptGenerator()
        //{
        //    return new ResourceGovernerIOStallScriptGenerator(ResourcePoolNameList);
        //}

        //public bool IsScriptRunnable { get { return (true); } }

        //public IUndoScriptGenerator GetUndoScriptGenerator()
        //{
        //    return new ResourceGovernerIOStallScriptGenerator(ResourcePoolNameList);
        //}

        //public bool IsUndoScriptRunnable { get { return (true); } }
        ///// <summary>
        ///// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        ///// </summary>
        //public bool IsScriptTransactionLess
        //{
        //    get { return true; }
        //}

        //public bool IsUndoScriptTransactionLess
        //{
        //    get { return true; }
        //}
    }
}
