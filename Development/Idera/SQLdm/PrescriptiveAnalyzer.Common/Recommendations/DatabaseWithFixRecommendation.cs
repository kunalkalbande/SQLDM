using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseWithFixRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string Database { get; private set; }

        public DatabaseWithFixRecommendation(RecommendationType type, string database): base(type)
        {
            this.Database = database;
        }

        public DatabaseWithFixRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
           Database = recProp.GetString("Database");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            switch (RecommendationType)
            {
                case RecommendationType.GuestHasDatabaseAccess:
                    return new GuestHasDatabaseAccessScriptGenerator(Database);
                case RecommendationType.IsTrustworthyVulnerable:
                    return new DatabaseConfigurationScriptGenerator(Database, "trustworthy", "off");
                default:
                    return null;
            }
        }


        public bool IsScriptRunnable
        {
            get
            {
                switch (RecommendationType)
                {
                    case RecommendationType.GuestHasDatabaseAccess:
                    case RecommendationType.IsTrustworthyVulnerable:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            switch (RecommendationType)
            {
                case RecommendationType.GuestHasDatabaseAccess:
                    return new GuestHasDatabaseAccessScriptGenerator(Database);
                case RecommendationType.IsTrustworthyVulnerable:
                    return new DatabaseConfigurationScriptGenerator(Database, "trustworthy", "on");
                default:
                    return null;
            }
        }


        public bool IsUndoScriptRunnable
        {
            get
            {
                switch (RecommendationType)
                {
                    case RecommendationType.GuestHasDatabaseAccess:
                    case RecommendationType.IsTrustworthyVulnerable:
                        return true;
                    default:
                        return false;
                }
            }
        }


        /// <summary>
        /// implemented ITransactionLessScript to support opti/undo queries without transactions
        /// </summary>
        public bool IsScriptTransactionLess
        {
            get { return true; }
        }

        public bool IsUndoScriptTransactionLess
        {
            get { return true; }
        }
    }
}