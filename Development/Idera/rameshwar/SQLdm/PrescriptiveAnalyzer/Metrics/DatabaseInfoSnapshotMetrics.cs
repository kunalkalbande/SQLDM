using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class DatabaseInfoSnapshotMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("DatabaseInfoSnapshotMetrics");
        private Dictionary<int, DatabaseInfoSnapshot> _databases = new Dictionary<int, DatabaseInfoSnapshot>();

        //public DatabaseInfoSnapshot FindFirstDatabase(Func<DatabaseInfoSnapshot, bool> predicate)
        //{
        //    return _databases.Values.First(predicate);
        //}

        //public IEnumerable<DatabaseInfoSnapshot> Find(Func<DatabaseInfoSnapshot, bool> predicate)
        //{
        //    return _databases.Values.Where(predicate);
        //}

        public IEnumerable<DatabaseInfoSnapshot> FindAutoShrinkEnabledDb()
        {
            List<DatabaseInfoSnapshot> list = new List<DatabaseInfoSnapshot>();
            foreach (DatabaseInfoSnapshot snap in _databases.Values)
            {
                if (snap.IsAutoShrinkEnabled)
                {
                    list.Add(snap);
                }
            }
            return list;
        }

        public DatabaseInfoSnapshot FindFirstDatabaseById(int id)
        {
            foreach (DatabaseInfoSnapshot snap in _databases.Values)
            {
                if (snap.DatabaseId == id)
                {
                    return snap;
                }
            }
            return null;
        }

        public bool IsDatabaseHostedOnDrive(string lglDisk, IList<string> allLogicalDisks)
        {
            foreach (DatabaseInfoSnapshot snapshot in _databases.Values)
            {
                if (snapshot.HostedOnLogicalDisk(lglDisk, allLogicalDisks, null))
                    return true;
            }
            return false;
        }

        public bool IsDatabaseDataHostedOnDrive(string lglDisk, IList<string> allLogicalDisks)
        {
            foreach (DatabaseInfoSnapshot snapshot in _databases.Values)
            {
                if (snapshot.HostedOnLogicalDisk(lglDisk, allLogicalDisks, 0))
                    return true;
            }
            return false;
        }

        public bool IsDatabaseLogsHostedOnDrive(string lglDisk, IList<string> allLogicalDisks)
        {
            foreach (DatabaseInfoSnapshot snapshot in _databases.Values)
            {
                if (snapshot.HostedOnLogicalDisk(lglDisk, allLogicalDisks, 1))
                    return true;
            }
            return false;
        }


        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("DatabaseInfoSnapshotMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.GetMasterFilesSnapshotValueStartup == null) { return; }
            if (snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo != null && snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows.Count > 0)
            {
                DatabaseInfoSnapshot obj = null;
                for (int index = 0; index < snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows.Count; index++)
                {
                    try
                    {
                        int databaseId = (Int32)snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows[index]["database_id"];
                        if (obj == null || obj.DatabaseId != databaseId)
                        {
                            obj = new DatabaseInfoSnapshot(databaseId);
                            obj.DatabaseName = (string)snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows[index]["dbname"];
                            obj.IsAutoShrinkEnabled = (bool)snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows[index]["is_auto_shrink_on"];
                            _databases.Add(databaseId, obj);
                        }
                        obj.AddFile(snapshot.GetMasterFilesSnapshotValueStartup.DatabaseFileInfo.Rows[index]);
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                }
            }
        }
    }


    public class DatabaseInfoSnapshot
    {
        public int DatabaseId { get; private set; }
        public string DatabaseName { get; internal set; }
        public bool IsAutoShrinkEnabled { get; internal set; }
        public IList<DatabaseFileInfo> Files { get; private set; }

        internal DatabaseInfoSnapshot(int databaseId)
        {
            DatabaseId = databaseId;
            Files = new List<DatabaseFileInfo>();
        }

        internal void AddFile(DataRow row)
        {
            DatabaseFileInfo file = new DatabaseFileInfo(row);
            if (Files.Count == 0 && String.IsNullOrEmpty(DatabaseName))
                DatabaseName = file.Name;

            Files.Add(file);
        }

        //internal IEnumerable<DatabaseFileInfo> FindFiles(Func<DatabaseFileInfo,bool> predicate)
        //{
        //    return Files.Where(predicate);
        //}

        internal IEnumerable<DatabaseFileInfo> FindFilesByTypeId(int Id)
        {
            List<DatabaseFileInfo> lst = new List<DatabaseFileInfo>();
            foreach (DatabaseFileInfo f in Files)
            {
                if (f.TypeId == Id)
                {
                    lst.Add(f);
                }
            }
            return lst;
        }

        internal bool HostedOnLogicalDisk(string lglDisk, IList<string> allLogicalDisks, int? typeId)
        {
            foreach (DatabaseFileInfo file in Files)
            {
                if (file.PhysicalName.StartsWith(lglDisk))
                {
                    if (typeId == null || file.TypeId == typeId)
                    {
                        if (lglDisk.Equals(GetBestMatchDisk(file.PhysicalName, allLogicalDisks)))
                            return true;
                    }
                }
            }
            return false;
        }

        internal static string GetBestMatchDisk(string lglDisk, IList<string> allLogicalDisks)
        {
            List<string> matchList = new List<string>();
            foreach (string str in allLogicalDisks)
            {
                if(lglDisk.StartsWith(str))
                {
                    matchList.Add(str);
                }
            }
            return matchList[matchList.Count - 1];
            //return allLogicalDisks.Last(ald => lglDisk.StartsWith(ald));
        }
    }

    public class DatabaseFileInfo
    {
        public int FileId { get; private set; }
        public int TypeId { get; private set; }
        public string TypeDesc { get; private set; }
        public long Size { get; private set; }
        public long InitialSize { get; private set; }
        public int MaxSize { get; private set; }
        public string Name { get; private set; }
        public string PhysicalName { get; private set; }
        public long Growth { get; private set; }
        public bool IsPercentGrowth { get; private set; }

        internal DatabaseFileInfo(DataRow row)
        {
            FileId = DataHelper.ToInt32(row, "file_id");
            TypeId = DataHelper.ToTinyInt(row, "type");
            TypeDesc = DataHelper.ToString(row, "type_desc");
            Size = DataHelper.ToLong(row, "size");
            InitialSize = 8192L * DataHelper.ToInt32(row, "initial_size");    // size returned from database is in pages
            MaxSize = DataHelper.ToInt32(row, "max_size");
            Name = DataHelper.ToString(row, "file_name");
            PhysicalName = DataHelper.ToString(row, "physical_name");
            Growth = DataHelper.ToLong(row, "growth");
            IsPercentGrowth = DataHelper.ToBoolean(row, "is_percent_growth");
        }
    }
}
