using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class PageId : ICloneable
    {
        public UInt32 DB { get; private set; }
        public UInt32 File { get; private set; }
        public UInt64 Page { get; private set; }
        public PageId(UInt32 db, UInt32 file, UInt64 page) { DB = db; File = file; Page = page; }

        public object Clone() { return (MemberwiseClone()); }
        public override string ToString() { return string.Format("{0}:{1}:{2}", DB, File, Page); }
    }
    public class PageInfo
    {
        private static Logger _logX = Logger.GetLogger("PageInfo");
        public PageId PageId { get; private set; }
        public UInt64 ObjectId { get; private set; }
        public UInt32 IndexId { get; private set; }
        public string ObjectName { get; private set; }
        public string SchemaName { get; private set; }
        public string IndexName { get; private set; }
        public bool IsTable { get; private set; }
        public bool IsMSShipped { get; private set; }

        public PageInfo(PageId pageId, SqlConnection conn)
        {
            try
            {
                PageId = pageId.Clone() as PageId;
                string sql = string.Format(Properties.Resources.GetPageInfo, PageId.DB, PageId.File, PageId.Page);
                string field;
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            field = DataHelper.ToString(reader, "Field");
                            switch (field)
                            {
                                case "Metadata: ObjectId": { ObjectId = DataHelper.ToUInt64(reader, "VALUE"); break; }
                                case "Metadata: IndexId": { IndexId = DataHelper.ToUInt32(reader, "VALUE"); break; }
                            }
                        }
                    }
                }
                sql = string.Format(Properties.Resources.GetIndexInfo, ObjectId, IndexId);
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ObjectName = DataHelper.ToString(reader, "ObjectName");
                            SchemaName = DataHelper.ToString(reader, "SchemaName");
                            IsTable = DataHelper.ToBoolean(reader, "IsTable");
                            IsMSShipped = DataHelper.ToBoolean(reader, "IsMSShipped");
                            IndexName = DataHelper.ToString(reader, "IndexName");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert(false, "Failed to create page info!");
                ExceptionLogger.Log(_logX, string.Format("PageInfo() Exception:", pageId), ex);
            }
        }
        public override string ToString() 
        {
            return (string.Format("Page({0} - {1}:{2}) [{3}].[{4}].[{5}] (IsTable:{6} IsMSShipped:{7})", PageId, ObjectId, IndexId, SchemaName, ObjectName, IndexName, IsTable, IsMSShipped)); 
        }
    }
    public static class PageHelper
    {
        private static Logger _logX = Logger.GetLogger("PageHelper");
        public static PageInfo GetPageInfo(PageId pageId, SqlConnection conn)
        {
            using (_logX.DebugCall(string.Format("GetPageInfo({0})", pageId)))
            {
                return (new PageInfo(pageId, conn));
            }
        }
    }
}
