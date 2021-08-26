using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLsafe.Shared;
using Idera.SQLsafe.Shared.Service.Backup;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace Idera.SQLdm.Common.SQLsafe
{
    [Serializable]
    public abstract class SQLsafeOperation
    {
        #region Properties

        public int OperationId { get; set; }
        public OperationType OperationType { get; set; }
        public OperationStatusCode OperationStatus { get; set; }
        public int InstanceId { get; set; }
        public string DatabaseName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int UtcOffSet { get; set; }

        #endregion

        #region Constructors

        protected SQLsafeOperation()
        {
            
        }

        protected SQLsafeOperation(int operId, int operType, int operStatus, int instance, string db, DateTime start, DateTime end, int offset)
        {
            OperationId = operId;
            OperationType = (OperationType) operType;
            OperationStatus = (OperationStatusCode) operStatus;
            InstanceId = instance;
            DatabaseName = db;
            StartTime = start;
            EndTime = end;
            UtcOffSet = offset;
        }

        #endregion


    }

    [Serializable]
    public class SQLsafeBackupRestoreOperation : SQLsafeOperation
    {
        public string ResultText { get; set; }

        public SQLsafeBackupRestoreOperation()
        {
        }

        public SQLsafeBackupRestoreOperation(int operId, int type, int status, int instance, string db, DateTime start, DateTime end, int offset, string result) 
            :base(operId, type, status, instance, db, start, end, offset)
        {
            ResultText = result;
        }

        public SQLsafeBackupRestoreOperation(SqlDataReader dataReader)
        {
            OperationId = dataReader["ActionId"] != DBNull.Value ? (int)dataReader["ActionId"] : -1;
            OperationType = dataReader["ActionType"] != DBNull.Value ? (OperationType)dataReader["ActionType"] : OperationType.Defragmentation;
            OperationStatus = dataReader["ActionStatus"] != DBNull.Value ? (OperationStatusCode)dataReader["ActionStatus"] : OperationStatusCode.None;
            InstanceId = dataReader["InstanceId"] != DBNull.Value ? (int)dataReader["InstanceId"] : -1;
            DatabaseName = dataReader["DatabaseName"] != DBNull.Value
                               ? (string)dataReader["DatabaseName"]
                               : string.Empty;
            StartTime = dataReader["StartTime"] != DBNull.Value
                            ? (DateTime)dataReader["StartTime"]
                            : (DateTime)SqlDateTime.MinValue;
            EndTime = dataReader["EndTime"] != DBNull.Value
                          ? (DateTime)dataReader["EndTime"]
                          : (DateTime)SqlDateTime.MaxValue;
            UtcOffSet = dataReader["UtcOffSet"] != DBNull.Value ? (int)dataReader["UtcOffSet"] : 0;
            
        }
    }

    [Serializable]
    public class SQLsafeDefragOperation : SQLsafeOperation
    {
        #region Properties

        public int AnalyzedCount { get; set; }
        public int RebuiltCount { get; set; }
        public int ReorganizedCount { get; set; }
        public int ErrorCount { get; set; }

        public List<DefragStats> DefragStats = new List<DefragStats>();

        #endregion

        #region Constructors

        public SQLsafeDefragOperation()
        {
            
        }

        public SQLsafeDefragOperation(int operId, int type, int status, int instance, string db, DateTime start, DateTime end, int offset, int analyzed, int rebuilt, int reorg, int error) 
            :base(operId, type, status, instance, db, start, end, offset)
        {
            AnalyzedCount = analyzed;
            RebuiltCount = rebuilt;
            ReorganizedCount = reorg;
            ErrorCount = error;
        }

        public SQLsafeDefragOperation(SqlDataReader dataReader)
        {
            OperationId = dataReader["ActionId"] != DBNull.Value ? (int)dataReader["ActionId"] : -1;
            OperationType = dataReader["ActionType"] != DBNull.Value ? (OperationType) dataReader["ActionType"] : OperationType.Defragmentation;
            OperationStatus = dataReader["ActionStatus"] != DBNull.Value ? (OperationStatusCode) dataReader["ActionStatus"] : OperationStatusCode.None;
            InstanceId = dataReader["InstanceId"] != DBNull.Value ? (int) dataReader["InstanceId"] : -1;
            DatabaseName = dataReader["DatabaseName"] != DBNull.Value
                               ? (string) dataReader["DatabaseName"]
                               : string.Empty;
            StartTime = dataReader["StartTime"] != DBNull.Value
                            ? (DateTime) dataReader["StartTime"]
                            : (DateTime) SqlDateTime.MinValue;
            EndTime = dataReader["EndTime"] != DBNull.Value
                          ? (DateTime)dataReader["EndTime"]
                          : (DateTime) SqlDateTime.MaxValue;
            UtcOffSet = dataReader["UtcOffSet"] != DBNull.Value ? (int) dataReader["UtcOffSet"] : 0;
            AnalyzedCount = dataReader["AnalyzedCount"] != DBNull.Value ? (int) dataReader["AnalyzedCount"] : -1;
            RebuiltCount = dataReader["RebuiltCount"] != DBNull.Value ? (int)dataReader["RebuiltCount"] : -1;
            ReorganizedCount = dataReader["ReorgCount"] != DBNull.Value ? (int)dataReader["ReorgCount"] : -1;
            ErrorCount = dataReader["ErrorCount"] != DBNull.Value ? (int)dataReader["ErrorCount"] : -1;
        }

        #endregion

    }

    [Serializable]
    public class DefragStats
    {
        #region Properties

        public int OperationId { get; set; }
        public int IndexId { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public long OldPageCount { get; set; }
        public long NewPageCount { get; set; }
        public short OldFragmentationLevel { get; set; }
        public short NewFragmentationLevel { get; set; }
        public short OldPageDensity { get; set; }
        public short NewPagedensity { get; set; }
        public short OldFillFactor { get; set; }
        public short NewFillFactor { get; set; }
        #endregion

        #region Constructors

        public DefragStats()
        {
            
        }

        public DefragStats(SqlDataReader dataReader )
        {
            OperationId = dataReader["ActionId"] != DBNull.Value ? (int) dataReader["ActionId"] : -1;
            IndexId = dataReader["IndexId"] != DBNull.Value ? (int) dataReader["IndexId"] : -1;
            TableName = dataReader["TableName"] != DBNull.Value ? (string) dataReader["TableName"] : string.Empty;
            IndexName = dataReader["IndexName"] != DBNull.Value ? (string) dataReader["IndexName"] : string.Empty;
            OldPageCount = dataReader["OldPageCount"] != DBNull.Value ? (long) dataReader["OldPageCount"] : -1;
            NewPageCount = dataReader["NewPageCount"] != DBNull.Value ? (long) dataReader["NewPageCount"] : -1;
            OldFragmentationLevel = dataReader["OldFragmentationLevel"] != DBNull.Value
                                        ? (short) dataReader["OldFragmentationLevel"]
                                        : (short)-1;
            NewFragmentationLevel = dataReader["NewFragmentationLevel"] != DBNull.Value
                                        ? (short) dataReader["NewFragmentationLevel"]
                                        : (short) -1;
            OldPageDensity = dataReader["OldPageDensity"] != DBNull.Value
                                 ? (short) dataReader["OldPageDensity"]
                                 : (short) -1;
            NewPagedensity = dataReader["NewPageDensity"] != DBNull.Value
                                 ? (short) dataReader["NewPageDensity"]
                                 : (short) -1;
            OldFillFactor = dataReader["OldFillFactor"] != DBNull.Value
                                ? (short) dataReader["OldFillFactor"]
                                : (short) -1;
            NewFillFactor = dataReader["NewFillFactor"] != DBNull.Value
                                ? (short) dataReader["NewFillFactor"]
                                : (short) -1;


        }

        #endregion
    }
}
