﻿using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLDm 10.0 Srishti Purohit- New Recommendations (SDR-M31) - adding new class
    /// </summary>
    [Serializable]
    public class LargeBufferPoolExtensionSizeRecommendation : Recommendation//, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string BufferPoolExtFilePath;
        public long BufferPoolExtSize;
        public LargeBufferPoolExtensionSizeRecommendation(string bufferPoolExtFilePath, long bufferPoolExtSize)
            : base(RecommendationType.LargeBufferPoolExtensionSize)
        {
            BufferPoolExtFilePath = bufferPoolExtFilePath;
            BufferPoolExtSize = bufferPoolExtSize;
        }
        public LargeBufferPoolExtensionSizeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.LargeBufferPoolExtensionSize,recProp)
        {
            BufferPoolExtFilePath = recProp.GetString("path");
            BufferPoolExtSize = recProp.GetInt("current_size_in_kb");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("path", BufferPoolExtFilePath.ToString());
            prop.Add("current_size_in_kb", BufferPoolExtSize.ToString());
            return prop;
        }
        // public IScriptGenerator GetScriptGenerator()
        //{
        //    return new LargeBufferPoolExtensionSizeScriptGenerator(BufferPoolExtFilePath, BufferPoolExtSize);
        //}

        //public bool IsScriptRunnable { get { return (true); } }

        //public IUndoScriptGenerator GetUndoScriptGenerator()
        //{
        //    return new LargeBufferPoolExtensionSizeScriptGenerator(BufferPoolExtFilePath, BufferPoolExtSize);
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
