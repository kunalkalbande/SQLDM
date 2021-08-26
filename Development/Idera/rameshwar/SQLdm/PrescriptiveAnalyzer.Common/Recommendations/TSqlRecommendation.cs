using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TSqlRecommendation : Recommendation, IProvideDatabase, IProvideApplicationName, IProvideUserName, IProvideHostName
    {
        private OffendingSql statement;
        private bool tempTable;
        private List<DatabaseObjectName> sourceObjects;

        public string ApplicationName { get; internal set; }
        public string Database { get; internal set; }
        public string UserName { get; internal set; }
        public string HostName { get; internal set; }

        public TSqlRecommendation(RecommendationType type) : base(type)
        {
        }

        public TSqlRecommendation(RecommendationType type, RecommendationProperties  recProp)
            : base(type, recProp)
        {
            ApplicationName = recProp.GetString("ApplicationName");
            Database = recProp.GetString("Database");
            UserName = recProp.GetString("UserName");
            HostName = recProp.GetString("HostName");
            tempTable = recProp.GetBool("tempTable");
            statement = recProp.GetOffendingSql("statement");
            sourceObjects = recProp.GetDatabaseObjectNameList("sourceObjects");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            if (!string.IsNullOrWhiteSpace(ApplicationName))
                prop.Add("ApplicationName", ApplicationName.ToString());
            if (!string.IsNullOrWhiteSpace(Database))
                prop.Add("Database", Database.ToString());
            if (!string.IsNullOrWhiteSpace(UserName))
                prop.Add("UserName", UserName.ToString());
            if (!string.IsNullOrWhiteSpace(HostName))
                prop.Add("HostName", HostName.ToString());
            prop.Add("tempTable", tempTable.ToString());
            prop.Add("statement", RecommendationProperties.GetXml<OffendingSql>(statement));
            prop.Add("sourceObjects", RecommendationProperties.GetXml<List<DatabaseObjectName>>(sourceObjects));
            return prop;
        }

        public TSqlRecommendation()
        {

        }

        public TSqlRecommendation(RecommendationType type, string database, string application, string user, string host) : this(type)
        {
            ApplicationName  = application;
            if (database != null)
            {
                Database = database;
            }
            else { Database = string.Empty; }
            UserName = user;
            HostName = host;
        }

        private TSqlRecommendation(RecommendationType type, OffendingSql statement) : this(type)
        {
            this.statement = statement;
        }

        public OffendingSql Sql
        {
            get { return statement;  }
            set { statement = value; }
        }

        public bool UsesTempTable
        {
            get { return tempTable;  }
            set { tempTable = value; }
        }

        public void AddSourceObject(DatabaseObjectName objectName)
        {
            if (sourceObjects == null)
                sourceObjects = new List<DatabaseObjectName>();

            sourceObjects.Add(objectName);
        }

        public override IEnumerable<DatabaseObjectName> SourceObjects
        {
            get
            {
                return sourceObjects;
            }
        }
    }

    [Serializable]
    public class TSqlHintRecommendation : TSqlRecommendation
    {
        private List<string> hints;

        public TSqlHintRecommendation(RecommendationType type, String database, String application, String user, String host) : base(type, database, application, user, host)
        {
            hints = new List<string>();
        }

        public TSqlHintRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            try
            {
                hints = recProp.GetListOfStrings("hints");
            }
            catch
            {
                hints = new List<string>();
            }
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("hints", RecommendationProperties.GetXml<List<string>>(hints));
            return prop;
        }

        public string[] Hints 
        { 
            get 
            {
                hints.Sort();
                return hints.ToArray(); 
            } 
        }
        
        public void AddHint(string hint)
        {
            if (!hints.Contains(hint))
                hints.Add(hint);
        }
    }


    [Serializable]
    public class TSqlCursorRecommendation : TSqlRecommendation
    {
        public TSqlCursorRecommendation(RecommendationType type, String database, String application, String user, String host)  : base(type, database, application, user, host)
        {
        }

        public TSqlCursorRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            CursorName = recProp.GetString("CursorName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("CursorName", CursorName.ToString());
            return prop;
        }

        public string CursorName { get; set; }
    }

    [Serializable]
    public class TSqlNestedMinMaxRecommendation : TSqlRecommendation
    {
        public TSqlNestedMinMaxRecommendation(RecommendationType type, String database, String application, String user, String host)
            : base(type, database, application, user, host)
        {
        }

        public TSqlNestedMinMaxRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            InnerFunctionName = recProp.GetString("InnerFunctionName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("InnerFunctionName", InnerFunctionName.ToString());
            return prop;
        }

        public string InnerFunctionName { get; set; }
    }

    [Serializable]
    public class TSqlRecommendationWithTable : TSqlRecommendation
    {
        public TSqlRecommendationWithTable(RecommendationType type, String database, String application, String user, String host) : base(type, database, application, user, host)
        {
        }

        public TSqlRecommendationWithTable(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            TableName = recProp.GetString("TableName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("TableName", TableName.ToString());
            return prop;
        }

        public string TableName { get; set; }
    }

    [Serializable]
    public class TSqlRecommendationWithColumn : TSqlRecommendation
    {
        public TSqlRecommendationWithColumn(RecommendationType type, String database, String application, String user, String host)
            : base(type, database, application, user, host)
        {
        }

        public TSqlRecommendationWithColumn(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            TableName = recProp.GetString("TableName");
            ColumnName = recProp.GetString("ColumnName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("TableName", TableName.ToString());
            prop.Add("ColumnName", ColumnName.ToString());
            return prop;
        }

        public string TableName { get; set; }
        public string ColumnName { get; set; }
    }
}
