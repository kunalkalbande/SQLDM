using System;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    public interface IRecommendation
    {
        IEnumerable<String> AffectedBatches { get; }
        String ID { get; }
        String Category { get; }
        int ConfidenceFactor { get; }
        String FindingText { get; }
        String DescriptionText { get; }
        String ImpactExplanationText { get; }
        String ProblemExplanationText { get; }
        int ImpactFactor { get; }
        bool IsScriptGeneratorProvider { get; }
        bool IsUndoScriptGeneratorProvider { get; }
        RecommendationLinks Links { get; }
        int RankID { get; }
        float ComputedRankFactor { get; }
        double Relevance { get; }
        String RecommendationText { get; }
        String AdditionalConsiderations { get; }
        RecommendationType RecommendationType { get; }
        IEnumerable<DatabaseObjectName> SourceObjects { get; }
        IList<string> Tags { get; }
        //10.0 SQLdm srishti purohit -- property to be populated from DB to get flag status
        bool IsFlagged { get; set; }
        //10.0 SQLdm srishti purohit -- property to be populated from DB to save unique recommendation ID
        int AnalysisRecommendationID { get; }

        //10.0 SQLdm srishti purohit -- property to be populated from DB to save status of optimization performed
        RecommendationOptimizationStatus OptimizationStatus { get; set; }

        string OptimizationErrorMessage { get; set; }

        void ComputeRankFactor(string filterDatabase, string filterApplication, RankingStats rankingStats);

        void SetProperties(Dictionary<string, string> lstProperties);

        Dictionary<string, string> GetProperties();
    }

    [Serializable]
    public class RecommendationCollection : List<IRecommendation>
    {
        new public void AddRange(IEnumerable<IRecommendation> collection)
        {
            if (null != collection) base.AddRange(collection);
        }
    }

    [Serializable]
    public class AffectedBatch
    {
        public string Name { get; set; }
        public string Batch { get; set; }
        public AffectedBatch(string name, string batch)
        {
            Name = name;
            Batch = batch;
        }
        /// <summary>
        /// vineet -- added parameterless constructor for serializing
        /// </summary>
        public AffectedBatch()
        { }
        public override string ToString()
        {
            return (Name);
        }
    }

    [Serializable]
    public class AffectedBatches : List<AffectedBatch>
    {
    }
    //SQLDm 10.0 Srishti Purohit - New Recommendations
    public interface IProvideQueryBatches : IProvideAffectedBatches
    { 
    }
    public interface IProvideAffectedBatches
    {
        AffectedBatches GetAffectedBatches();
    }

    public interface IProvideXmlPlan
    {
        String XmlPlan { get; }
    }

    public interface IProvideDatabase
    {
        String Database { get; }
    }

    public interface IProvideTableName : IProvideDatabase
    {
        String Schema { get; }
        String Table { get; }
    }

    public interface IProvideApplicationName
    {
        String ApplicationName { get; }
    }

    public interface IProvideUserName
    {
        String UserName { get; }
    }

    public interface IProvideHostName
    {
        String HostName { get; }
    }

    public interface IScriptGeneratorProvider
    {
        IScriptGenerator GetScriptGenerator();
        bool IsScriptRunnable { get; }
    }

    public interface IScriptGenerator
    {
        String GetTSqlFix(SqlConnectionInfo connectionInfo);
    }

    public interface IUndoScriptGeneratorProvider
    {
        IUndoScriptGenerator GetUndoScriptGenerator();
        bool IsUndoScriptRunnable { get; }
    }

    public interface IUndoScriptGenerator
    {
        String GetTSqlUndo(SqlConnectionInfo connectionInfo);
    }

    public interface IMessageGenerator
    {
        List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connectionInfo);
    }

    public interface IUndoMessageGenerator
    {
        List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connectionInfo);
    }

    /// <summary>
    /// vineet Kumar dm 10.0 -- Added to support opti/undo queries to run without transactions
    /// </summary>
    public interface ITransactionLessScript
    {
        bool IsScriptTransactionLess { get; }
        bool IsUndoScriptTransactionLess { get; }
    }
}
