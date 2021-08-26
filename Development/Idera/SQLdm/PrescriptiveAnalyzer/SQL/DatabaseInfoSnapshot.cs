using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    [Serializable]
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

        //internal bool HostedOnLogicalDisk(string lglDisk, IList<string> allLogicalDisks, Func<DatabaseFileInfo,bool> fileFilter)
        //{
        //    foreach (DatabaseFileInfo file in Files)
        //    {
        //        if (file.PhysicalName.StartsWith(lglDisk))
        //        {
        //            if (fileFilter == null || fileFilter.Invoke(file))
        //            {
        //                if (lglDisk.Equals(GetBestMatchDisk(file.PhysicalName, allLogicalDisks)))
        //                    return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //internal static string GetBestMatchDisk(string lglDisk, IList<string> allLogicalDisks)
        //{
        //    return allLogicalDisks.Last(ald => lglDisk.StartsWith(ald));
        //}
    }

    [Serializable]
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
