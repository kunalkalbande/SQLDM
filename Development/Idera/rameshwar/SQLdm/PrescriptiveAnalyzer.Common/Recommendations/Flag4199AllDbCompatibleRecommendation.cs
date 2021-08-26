using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLdm10.0 -Srishti Purohit -  New Recommendations
    /// Trace flag 4199 is enabled globally for SQL Server 2016 and all databases are running in compatibility level 130
    /// </summary>
    [Serializable]
    public class Flag4199AllDbCompatibleRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {

        public Flag4199AllDbCompatibleRecommendation()
            : base(RecommendationType.Flag4199AllDbCompatible)
        {
        }
        #region UndoOptimize
        public IScriptGenerator GetScriptGenerator()
        {
            return new Flag4199AllDbCompatibleScriptGenerator();
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new Flag4199AllDbCompatibleScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

        /// <summary>
        /// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        /// </summary>
        public bool IsScriptTransactionLess
        {
            get { return true; }
        }

        public bool IsUndoScriptTransactionLess
        {
            get { return true; }
        }
        #endregion

    }
}
