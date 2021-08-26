using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class Index
    {
        #region Properties
        [NonSerialized]private static Logger _logX = Logger.GetLogger("Index");
        private Exception _ex;

        public enum IndexTypeEnum { Heap, Clustered, Nonclustered, XML, Spatial }
        public enum DataSpaceTypeEnum { Filegroup, PartitionScheme, FilestreamDataFilegroup }
        public enum xmlSecondaryTypeEnum { Path, Value, Property, Null }
        public string SqlVersion;

        public string DatabaseName;
        public string SchemaName;
        public int ObjectID;
        public int IndexID;
        public bool IsUnique;
        public IndexTypeEnum IndexType;
        public string IndexName;
        public bool HasFilter;
        public string FilterDefinition;
        public bool IsPadded;
        private int _fillFactor;
        public bool IgnoreDupKey;
        public bool AllowRowLocks;
        public bool AllowPageLocks;
        public int DataSpaceID;
        public string ObjectName;
        public bool IsDefinedOnTable;
        public ColumnList IndexColumns;
        public bool StatsNoRecompute;
        public PartitionList IndexPartitions;
        public string DataSpaceName;
        public DataSpaceTypeEnum DataSpaceType;
        public string FileStreamDataSpaceName;
        public bool IsPrimaryKeyConstraint;
        public bool IsUniqueConstraint;
        public bool? OnlineRebuild = null;

        public int xmlParentIndexID;
        public xmlSecondaryTypeEnum xmlSecondaryType;
        public string xmlParentIndexName;

        public string TessellationScheme;
        public int CellsPerObject;
        public double BoundingBoxXMin;
        public double BoundingBoxXMax;
        public double BoundingBoxYMin;
        public double BoundingBoxYMax;
        public string Level1GridDesc;
        public string Level2GridDesc;
        public string Level3GridDesc;
        public string Level4GridDesc;
        #endregion
        #region Accessors
        public int FillFactor
        {
            get { return _fillFactor; }
            set { _fillFactor = (0 == value) ? 100 : value; }
        }
        private bool IsXMLPrimary
        {
            get { return (-1 == xmlParentIndexID);}
            set { }
        }
        private bool IsXMLSecondary
        {
            get { return (!IsXMLPrimary); }
            set { }
        }
        private bool IsGeometryTessellation
        {
            get { return ("GEOMETRY_GRID" == TessellationScheme.Trim().ToUpper()); }
            set { }
        }
        private bool IsGeographyTessellation
        {
            get { return ("GEOGRAPHY_GRID" == TessellationScheme.Trim().ToUpper()); }
            set { }
        }
        private string BoundingBoxString
        {
            get
            {
                return (String.Format("      BOUNDING_BOX = (XMIN = {0}, YMIN = {1}, XMAX = {2}, YMAX = {3})", 
                    BoundingBoxXMin, BoundingBoxYMin, BoundingBoxXMax, BoundingBoxYMax));
            }

            set { }
        }
        private string TessellationGridString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("      ");
                if (IsGeometryTessellation) builder.Append(", ");
                builder.Append(string.Format("GRIDS = ( LEVEL_1 = {0}, LEVEL_2 = {1}, LEVEL_3 = {2}, LEVEL_4 = {3})", 
                        Level1GridDesc, Level2GridDesc, Level3GridDesc, Level4GridDesc));
                return builder.ToString();
            }

            set { }
        }
        private bool IsConstraint
        {
            get { return (IsPrimaryKeyConstraint || IsUniqueConstraint); }
            set { }
        }        
        public bool HasErrors
        {
            get { return (null != _ex); }
            set { }
        }
        public bool IsSql2005
        {
            get { return SqlVersion.StartsWith("09."); }
            set { }
        }
        public bool IsSql2008
        {
            get { return SqlVersion.StartsWith("10."); }
            set { }
        }
        #endregion

        public Index() { }

        public Index(string database, string schema, string objectName, string indexName)
        {
            DatabaseName = database;
            SchemaName = schema;
            ObjectName = objectName;
            IndexName = indexName;
            IndexColumns = new ColumnList();
            IndexPartitions = new PartitionList();
        }

        #region Populate Class Properties
        public void GetIndexProperties(SqlConnection conn)
        {
            try
            {
                SQLHelper.CheckConnection(conn);
                SqlVersion = conn.ServerVersion;

                GetPropertiesFromSysObjects(conn);
                GetPropertiesFromSysIndexes(conn);
                GetPropertiesFromSysIndexColumns(conn);
                GetOnlineRebuild(conn);
                switch (IndexType)
                {
                    case IndexTypeEnum.Clustered:
                    case IndexTypeEnum.Nonclustered:
                        {
                            GetPropertiesFromSysStats(conn);
                            GetPropertiesFromSysPartitions(conn);
                            GetPropertiesFromSysDataSpaces(conn);
                            GetPropertiesFromSysTables(conn);
                            break;
                        }
                    case IndexTypeEnum.XML:
                        {
                            GetPropertiesFromSysXMLIndexes(conn);
                            break;
                        }
                    case IndexTypeEnum.Spatial:
                        {
                            GetPropertiesFromSysStats(conn);
                            GetPropertiesFromSysDataSpaces(conn);
                            GetPropertiesFromSysSpatialIndexTessellations(conn);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _ex = ex;
                ExceptionLogger.Log(_logX, "GetIndexProperties Failed: ", ex);
            }
        }

        private void GetPropertiesFromSysIndexes(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysIndexesQuery;
            if (IsSql2008)
            {
                sysIndexesQuery = string.Format(Properties.Resources.GetPropertiesFromSysIndexes2008
                                                , SQLHelper.CreateBracketedString(DatabaseName)
                                                , ObjectID
                                                , SQLHelper.CreateSafeString(IndexName));
            }
            else
            {
                sysIndexesQuery = string.Format(Properties.Resources.GetPropertiesFromSysIndexes2005
                                                , SQLHelper.CreateBracketedString(DatabaseName)
                                                , ObjectID
                                                , SQLHelper.CreateSafeString(IndexName));
            }
            _logX.Verbose(sysIndexesQuery.ToString());

            using (SqlCommand sysIndexesCommand = new SqlCommand(sysIndexesQuery.ToString(), conn))
            {
                sysIndexesCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysIndexesQueryResults = sysIndexesCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.indexes
                    if (sysIndexesQueryResults.HasRows)
                    {
                        sysIndexesQueryResults.Read();
                        if (IsSql2008)
                        {
                            IsUnique = sysIndexesQueryResults.GetBoolean(0);
                            IndexType = (IndexTypeEnum)sysIndexesQueryResults.GetByte(1);
                            IndexID = (int)sysIndexesQueryResults.GetInt32(2);
                            HasFilter = sysIndexesQueryResults.GetBoolean(3);
                            FilterDefinition = (HasFilter) ? sysIndexesQueryResults.GetString(4) : "";
                            IsPadded = sysIndexesQueryResults.GetBoolean(5);
                            FillFactor = (int)sysIndexesQueryResults.GetByte(6);
                            IgnoreDupKey = sysIndexesQueryResults.GetBoolean(7);
                            AllowRowLocks = sysIndexesQueryResults.GetBoolean(8);
                            AllowPageLocks = sysIndexesQueryResults.GetBoolean(9);
                            DataSpaceID = (int)sysIndexesQueryResults.GetInt32(10);
                            IsPrimaryKeyConstraint = sysIndexesQueryResults.GetBoolean(11);
                            IsUniqueConstraint = sysIndexesQueryResults.GetBoolean(12);
                        }
                        else
                        {
                            IsUnique = sysIndexesQueryResults.GetBoolean(0);
                            IndexType = (IndexTypeEnum)sysIndexesQueryResults.GetByte(1);
                            IndexID = (int)sysIndexesQueryResults.GetInt32(2);
                            HasFilter = false;
                            FilterDefinition = "";
                            IsPadded = sysIndexesQueryResults.GetBoolean(3);
                            FillFactor = (int)sysIndexesQueryResults.GetByte(4);
                            IgnoreDupKey = sysIndexesQueryResults.GetBoolean(5);
                            AllowRowLocks = sysIndexesQueryResults.GetBoolean(6);
                            AllowPageLocks = sysIndexesQueryResults.GetBoolean(7);
                            DataSpaceID = (int)sysIndexesQueryResults.GetInt32(8);
                            IsPrimaryKeyConstraint = sysIndexesQueryResults.GetBoolean(9);
                            IsUniqueConstraint = sysIndexesQueryResults.GetBoolean(10);
                        }
                    }
                    sysIndexesQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysObjects(SqlConnection conn)
        {// No longer queries SysObjects.  Converted to instead use OBJECT_ID and OBJECTPROPERTY functions
            SQLHelper.CheckConnection(conn);
            string sysObjectsQuery = string.Format(Properties.Resources.GetPropertiesFromSysObjects
                                                    , SQLHelper.CreateBracketedString(DatabaseName)
                                                    , SQLHelper.CreateSafeString(SQLHelper.Bracket(SchemaName, ObjectName)));
            _logX.Verbose(sysObjectsQuery);

            using (SqlCommand sysObjectsCommand = new SqlCommand(sysObjectsQuery, conn))
            {
                sysObjectsCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysObjectsQueryResults = sysObjectsCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.Objects
                    if (sysObjectsQueryResults.HasRows)
                    {
                        sysObjectsQueryResults.Read();
                        ObjectID = (int)sysObjectsQueryResults.GetInt32(0);
                        IsDefinedOnTable = (1 == sysObjectsQueryResults.GetInt32(1)) ? true : false;
                    }
                    sysObjectsQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysIndexColumns(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysIndexColumnsQuery = string.Format(Properties.Resources.GetPropertiesFromSysIndexColumns
                                                        , SQLHelper.CreateBracketedString(DatabaseName)
                                                        , ObjectID
                                                        , IndexID);
            _logX.Verbose(sysIndexColumnsQuery.ToString());

            using (SqlCommand sysIndexColumnsCommand = new SqlCommand(sysIndexColumnsQuery.ToString(), conn))
            {
                sysIndexColumnsCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysIndexColumnsQueryResults = sysIndexColumnsCommand.ExecuteReader())
                {
                    #region Populate Columns list
                    if (sysIndexColumnsQueryResults.HasRows)
                        while (sysIndexColumnsQueryResults.Read())
                        {
                            IndexColumns.Add(new Column(sysIndexColumnsQueryResults.GetString(4),
                                (int)sysIndexColumnsQueryResults.GetByte(3),
                                (int)sysIndexColumnsQueryResults.GetByte(2),
                                sysIndexColumnsQueryResults.GetBoolean(0),
                                sysIndexColumnsQueryResults.GetBoolean(1)));
                        }
                    sysIndexColumnsQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysStats(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysStatsQuery = string.Format(Properties.Resources.GetPropertiesFromSysStats
                                                , SQLHelper.CreateBracketedString(DatabaseName)
                                                , ObjectID
                                                , IndexID);
            _logX.Verbose(sysStatsQuery.ToString());

            using (SqlCommand sysStatsCommand = new SqlCommand(sysStatsQuery.ToString(), conn))
            {
                sysStatsCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysStatsQueryResults = sysStatsCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.stats
                    if (sysStatsQueryResults.HasRows)
                    {
                        sysStatsQueryResults.Read();
                        StatsNoRecompute = sysStatsQueryResults.GetBoolean(0);
                    }
                    sysStatsQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysPartitions(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysPartitionsQuery;
            if (IsSql2008)
            {
                sysPartitionsQuery = string.Format(Properties.Resources.GetPropertiesFromSysPartitions2008
                                                    , SQLHelper.CreateBracketedString(DatabaseName)
                                                    , ObjectID
                                                    , IndexID);
            }
            else
            {
                sysPartitionsQuery = string.Format(Properties.Resources.GetPropertiesFromSysPartitions2005
                                                    , SQLHelper.CreateBracketedString(DatabaseName)
                                                    , ObjectID
                                                    , IndexID);
            }
            _logX.Verbose(sysPartitionsQuery.ToString());

            using (SqlCommand sysPartitionsCommand = new SqlCommand(sysPartitionsQuery.ToString(), conn))
            {
                sysPartitionsCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysPartitionsQueryResults = sysPartitionsCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.partitions
                    if (sysPartitionsQueryResults.HasRows)
                        if (IsSql2008)
                        {
                            while (sysPartitionsQueryResults.Read())
                            {
                                IndexPartitions.Add(new Partition((int)sysPartitionsQueryResults.GetInt32(0), sysPartitionsQueryResults.GetString(1)));
                            }
                        }
                        else
                        {
                            while (sysPartitionsQueryResults.Read())
                            {
                                IndexPartitions.Add(new Partition((int)sysPartitionsQueryResults.GetInt32(0), "NONE"));
                            }
                        }
                    sysPartitionsQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysDataSpaces(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysDataSpacesQuery = string.Format(Properties.Resources.GetPropertiesFromSysDataSpaces
                                                        , SQLHelper.CreateBracketedString(DatabaseName)
                                                        , DataSpaceID);
            _logX.Verbose(sysDataSpacesQuery.ToString());

            using (SqlCommand sysDataSpacesCommand = new SqlCommand(sysDataSpacesQuery.ToString(), conn))
            {
                sysDataSpacesCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysDataSpacesQueryResults = sysDataSpacesCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.data_spaces
                    if (sysDataSpacesQueryResults.HasRows)
                    {
                        sysDataSpacesQueryResults.Read();
                        DataSpaceName = sysDataSpacesQueryResults.GetString(0);
                        switch (sysDataSpacesQueryResults.GetString(1))
                        {
                            case "FG": DataSpaceType = DataSpaceTypeEnum.Filegroup; break;
                            case "PS": DataSpaceType = DataSpaceTypeEnum.PartitionScheme; break;
                            case "FD": DataSpaceType = DataSpaceTypeEnum.FilestreamDataFilegroup; break;
                        }
                    }
                    sysDataSpacesQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetPropertiesFromSysTables(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            if ((IsDefinedOnTable) && (IsSql2008))
            {
                string sysTablesQuery = string.Format(Properties.Resources.GetPropertiesFromSysTables2008
                                                        , SQLHelper.CreateBracketedString(DatabaseName)
                                                        , ObjectID);
                _logX.Verbose(sysTablesQuery.ToString());

                using (SqlCommand sysTablesCommand = new SqlCommand(sysTablesQuery.ToString(), conn))
                {
                    sysTablesCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                    object tempResult = sysTablesCommand.ExecuteScalar();
                    FileStreamDataSpaceName = (null == tempResult) ? "" : tempResult.ToString();
                }
            }
            else
                FileStreamDataSpaceName = "";
        }

        private void GetPropertiesFromSysXMLIndexes(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysXMLIndexesQuery = string.Format(Properties.Resources.GetPropertiesFromSysXMLIndexes
                                                        , SQLHelper.CreateBracketedString(DatabaseName)
                                                        , ObjectID
                                                        , IndexID);
            _logX.Verbose(sysXMLIndexesQuery.ToString());

            using (SqlCommand sysXMLIndexesCommand = new SqlCommand(sysXMLIndexesQuery.ToString(), conn))
            {
                sysXMLIndexesCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sysXMLIndexesQueryResults = sysXMLIndexesCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.xml_indexes
                    if (sysXMLIndexesQueryResults.HasRows)
                    {
                        sysXMLIndexesQueryResults.Read();
                        if (sysXMLIndexesQueryResults.IsDBNull(0))
                        {
                            xmlParentIndexID = -1;
                            xmlSecondaryType = xmlSecondaryTypeEnum.Null;
                        }
                        else
                        {
                            xmlParentIndexID = (int)sysXMLIndexesQueryResults.GetInt32(0);
                            switch (sysXMLIndexesQueryResults.GetString(1).Trim().ToUpper())
                            {
                                case "P": xmlSecondaryType = xmlSecondaryTypeEnum.Path; break;
                                case "V": xmlSecondaryType = xmlSecondaryTypeEnum.Value; break;
                                case "R": xmlSecondaryType = xmlSecondaryTypeEnum.Property; break;
                            }
                        }
                    }
                    sysXMLIndexesQueryResults.Close();
                    #endregion
                }
            }
            #region Get primary if necessary
            if (IsXMLSecondary)
            {
                string sysXMLIndexesPrimaryQuery = string.Format(Properties.Resources.GetPropertiesFromSysXMLIndexesPrimary
                                                                , SQLHelper.CreateBracketedString(DatabaseName)
                                                                , xmlParentIndexID);
                _logX.Verbose(sysXMLIndexesPrimaryQuery);

                using (SqlCommand sysXMLIndexesPrimaryCommand = new SqlCommand(sysXMLIndexesPrimaryQuery, conn))
                {
                    sysXMLIndexesPrimaryCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                    object tempResult = sysXMLIndexesPrimaryCommand.ExecuteScalar();
                    xmlParentIndexName = (null == tempResult) ? "" : tempResult.ToString();
                }
            }
            #endregion
        }

        private void GetPropertiesFromSysSpatialIndexTessellations(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string sysSpatialIndexTessellationsQuery = string.Format(Properties.Resources.GetPropertiesFromSysSpatialIndexTessellations
                                                                    , SQLHelper.CreateBracketedString(DatabaseName)
                                                                    , ObjectID
                                                                    , IndexID);
            _logX.Verbose(sysSpatialIndexTessellationsQuery.ToString());

            using (SqlCommand sysSpatialIndexTessellationsCommand = new SqlCommand(sysSpatialIndexTessellationsQuery.ToString(), conn))
            {
                sysSpatialIndexTessellationsCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                using (SqlDataReader sySpatialIndexTessellationsQueryResults = sysSpatialIndexTessellationsCommand.ExecuteReader())
                {
                    #region Populate class properties from sys.SpatialIndexTessellations
                    if (sySpatialIndexTessellationsQueryResults.HasRows)
                    {
                        sySpatialIndexTessellationsQueryResults.Read();
                        TessellationScheme = sySpatialIndexTessellationsQueryResults.GetString(0);
                        BoundingBoxXMin = (sySpatialIndexTessellationsQueryResults.IsDBNull(1)) ? -1 : sySpatialIndexTessellationsQueryResults.GetDouble(1);
                        BoundingBoxYMin = (sySpatialIndexTessellationsQueryResults.IsDBNull(2)) ? -1 : sySpatialIndexTessellationsQueryResults.GetDouble(2);
                        BoundingBoxXMax = (sySpatialIndexTessellationsQueryResults.IsDBNull(3)) ? -1 : sySpatialIndexTessellationsQueryResults.GetDouble(3);
                        BoundingBoxYMax = (sySpatialIndexTessellationsQueryResults.IsDBNull(4)) ? -1 : sySpatialIndexTessellationsQueryResults.GetDouble(4);
                        Level1GridDesc = sySpatialIndexTessellationsQueryResults.GetString(5);
                        Level2GridDesc = sySpatialIndexTessellationsQueryResults.GetString(6);
                        Level3GridDesc = sySpatialIndexTessellationsQueryResults.GetString(7);
                        Level4GridDesc = sySpatialIndexTessellationsQueryResults.GetString(8);
                        CellsPerObject = (int)sySpatialIndexTessellationsQueryResults.GetInt32(9);
                    }
                    sySpatialIndexTessellationsQueryResults.Close();
                    #endregion
                }
            }
        }

        private void GetOnlineRebuild(SqlConnection conn)
        {
            SQLHelper.CheckConnection(conn);
            string script = Idera.SQLdm.PrescriptiveAnalyzer.Common.Properties.Resources.IsIndexRebuildableOnline;
            
            string previousDatabase = conn.Database;
            if (!String.IsNullOrEmpty(DatabaseName))
                conn.ChangeDatabase(DatabaseName);

            using (SqlCommand command = new SqlCommand(script, conn))
            {
                command.Parameters.AddWithValue("@ObjectName", SQLHelper.Bracket(SchemaName, ObjectName));
                command.Parameters.AddWithValue("@IndexName", IndexName);
                OnlineRebuild = (bool)command.ExecuteScalar();
            }
            conn.ChangeDatabase(previousDatabase);
        }
        #endregion

        #region BuildCreateScripts
        public string GetIndexCreateScript()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("USE {0}", SQLHelper.CreateBracketedString(DatabaseName)));
            builder.AppendLine("GO");

            switch (IndexType)
            {
                case IndexTypeEnum.Clustered:
                case IndexTypeEnum.Nonclustered:
                    builder.Append(GetRelationalIndexCreateScript()); 
                    break;
                case IndexTypeEnum.XML:
                    builder.Append(GetXMLIndexCreateScript());
                    break;
                case IndexTypeEnum.Spatial:
                    builder.Append(GetSpatialIndexCreateScript());
                    break;
            }
            return builder.ToString();
        }

        private string GetRelationalIndexCreateScript()
        {
            StringBuilder builder = new StringBuilder();

            if (IsConstraint)
            {
                builder.AppendLine(string.Format("ALTER TABLE {0} ", SQLHelper.Bracket(SchemaName, ObjectName)));
                builder.Append(string.Format("   ADD CONSTRAINT {0} ", SQLHelper.Bracket(IndexName)));
                builder.Append((IsPrimaryKeyConstraint) ? "PRIMARY KEY " : "UNIQUE ");
                builder.Append((IndexType == IndexTypeEnum.Clustered) ? "CLUSTERED " : "NONCLUSTERED ");
                builder.AppendLine(string.Format("( {0} )", IndexColumns.KeyColumnsString));
            }
            else
            {
                builder.Append("Create ");
                if (IsUnique) builder.Append("UNIQUE ");
                if (IndexType == IndexTypeEnum.Clustered) builder.Append("CLUSTERED ");
                builder.AppendLine(string.Format("INDEX {0}", SQLHelper.Bracket(IndexName)));
                builder.AppendLine(string.Format("   ON {0} ({1})", SQLHelper.Bracket(SchemaName, ObjectName), IndexColumns.KeyColumnsString));

                if (0 != IndexColumns.IncludeColumnsString.Length)
                    builder.AppendLine(string.Format("   INCLUDE ({0})", IndexColumns.IncludeColumnsString));

                if (HasFilter) builder.AppendLine(string.Format("   WHERE {0}", FilterDefinition));
            }
            #region Build Index Options
            builder.AppendLine("   WITH ");
            builder.AppendLine("   (");
            builder.AppendLine((IsPadded) ? "      PAD_INDEX = ON" : "      PAD_INDEX = OFF");
            builder.AppendLine(string.Format("      , FILLFACTOR = {0}", FillFactor));
            builder.AppendLine((IgnoreDupKey) ? "      , IGNORE_DUP_KEY = ON" : "      , IGNORE_DUP_KEY = OFF");
            builder.AppendLine((StatsNoRecompute) ? "      , STATISTICS_NORECOMPUTE = ON" : "      , STATISTICS_NORECOMPUTE = OFF");
            builder.AppendLine((AllowRowLocks) ? "      , ALLOW_ROW_LOCKS = ON" : "      , ALLOW_ROW_LOCKS = OFF");
            builder.AppendLine((AllowPageLocks) ? "      , ALLOW_PAGE_LOCKS = ON" : "      , ALLOW_PAGE_LOCKS = OFF");
            if (IsSql2008) builder.Append(IndexPartitions.GenerateIndexString());
            builder.AppendLine("   )");
            #endregion

            if (DataSpaceTypeEnum.PartitionScheme == DataSpaceType)
                builder.AppendLine(string.Format("ON {0} ({1})", SQLHelper.Bracket(DataSpaceName), SQLHelper.Bracket(IndexColumns.PartitionColumn.ColumnName)));
            else
                builder.AppendLine(string.Format("ON {0}", SQLHelper.Bracket(DataSpaceName)));

            if (("" != FileStreamDataSpaceName) && (IndexType == IndexTypeEnum.Clustered))
                builder.AppendLine(string.Format("FILESTREAM_ON {0}", SQLHelper.Bracket(FileStreamDataSpaceName)));

            return builder.ToString();
        }

        private string GetXMLIndexCreateScript()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Create ");
            if (IsXMLPrimary) builder.Append("PRIMARY ");
            builder.AppendLine(string.Format("XML INDEX {0}", SQLHelper.Bracket(IndexName)));
            builder.AppendLine(string.Format("   ON {0} ({2})", SQLHelper.Bracket(SchemaName, ObjectName), IndexColumns.KeyColumnsString));
            if (IsXMLSecondary)
            {
                builder.AppendLine(string.Format("   USING XML INDEX {0}", SQLHelper.Bracket(xmlParentIndexName)));
                switch (xmlSecondaryType)
                {
                    case xmlSecondaryTypeEnum.Path: builder.AppendLine("      FOR PATH"); break;
                    case xmlSecondaryTypeEnum.Property: builder.AppendLine("      FOR PROPERTY"); break;
                    case xmlSecondaryTypeEnum.Value: builder.AppendLine("      FOR VALUE"); break;
                }
            }
            #region Build Index Options
            builder.AppendLine("   WITH ");
            builder.AppendLine("   (");
            builder.AppendLine((IsPadded) ? "      PAD_INDEX = ON" : "      PAD_INDEX = OFF");
            builder.AppendLine(string.Format("      , FILLFACTOR = {0}", FillFactor));
            builder.AppendLine((IgnoreDupKey) ? "      , IGNORE_DUP_KEY = ON" : "      , IGNORE_DUP_KEY = OFF");
            builder.AppendLine((AllowRowLocks) ? "      , ALLOW_ROW_LOCKS = ON" : "      , ALLOW_ROW_LOCKS = OFF");
            builder.AppendLine((AllowPageLocks) ? "      , ALLOW_PAGE_LOCKS = ON" : "      , ALLOW_PAGE_LOCKS = OFF");
            if (IsSql2008) builder.Append(IndexPartitions.GenerateIndexString());
            builder.AppendLine("   )");
            #endregion

            return builder.ToString();
        }

        private string GetSpatialIndexCreateScript()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Format("CREATE SPATIAL INDEX {0}", SQLHelper.Bracket(IndexName)));
            builder.AppendLine(string.Format("   ON {0} ({1})", SQLHelper.Bracket(SchemaName, ObjectName), IndexColumns.KeyColumnsString));
            builder.AppendLine(string.Format("   USING {0}", TessellationScheme));
            #region Build Index Options
            builder.AppendLine("   WITH ");
            builder.AppendLine("   (");
            if (IsGeometryTessellation) builder.AppendLine(BoundingBoxString);
            builder.AppendLine(TessellationGridString);
            builder.AppendLine(string.Format("      , CELLS_PER_OBJECT = {0}", CellsPerObject));
            builder.AppendLine((IsPadded) ? "      , PAD_INDEX = ON" : "      , PAD_INDEX = OFF");
            builder.AppendLine(string.Format("      , FILLFACTOR = {0}", FillFactor));
            builder.AppendLine((IgnoreDupKey) ? "      , IGNORE_DUP_KEY = ON" : "      , IGNORE_DUP_KEY = OFF");
            builder.AppendLine((AllowRowLocks) ? "      , ALLOW_ROW_LOCKS = ON" : "      , ALLOW_ROW_LOCKS = OFF");
            builder.AppendLine((AllowPageLocks) ? "      , ALLOW_PAGE_LOCKS = ON" : "      , ALLOW_PAGE_LOCKS = OFF");
            if (IsSql2008) builder.Append(IndexPartitions.GenerateIndexString());
            builder.AppendLine("   )");
            #endregion
            builder.AppendLine(string.Format("ON {0}", SQLHelper.Bracket(DataSpaceName)));

            return builder.ToString();
        }
        #endregion

        public string GetIndexDisableScript()
        {
            return string.Format("ALTER INDEX {0} ON {1} DISABLE", SQLHelper.Bracket(IndexName), SQLHelper.Bracket(SchemaName, ObjectName));
        }
    }
}
