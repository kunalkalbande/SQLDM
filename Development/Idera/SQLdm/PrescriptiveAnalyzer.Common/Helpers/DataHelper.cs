using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class DataHelper
    {
        private DataHelper() { }

        public static bool IsNull(DataRow dr, int col)
        {
            if ((dr.Table.Columns.Count > col) && (col >= 0))
            {
                return (IsNull(dr[col]));
            }
            return (true);
        }
        public static bool IsNull(DataRow dr, string prop)
        {
            if (dr.Table.Columns.Contains(prop))
            {
                return (IsNull(dr[prop]));
            }
            return (true);
        }

        public static bool IsNull(SqlDataReader dr, string prop)
        {
            try
            {
                return (IsNull(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (true);
        }

        public static bool IsNull(DataTableReader dr, string prop)
        {
            try
            {
                return (IsNull(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (true);
        }

        public static bool IsNull(DataTableReader dr, int prop)
        {
            try
            {
                return (IsNull(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (true);
        }

        public static bool IsNull(SqlDataReader dr, int n)
        {
            try
            {
                return (IsNull(dr[n]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (true);
        }

        public static bool IsNull(object o)
        {
            if (null == o) return true;
            if (o is DBNull) return true;
            return (false);
        }

        public static DateTime ToDateTime(DataRow dr, int col, IFormatProvider format)
        {
            try
            {
                if (!IsNull(dr, col)) { return DateTime.Parse(dr[col].ToString(), format); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (DateTime.MinValue);
        }

        public static double ToDouble(DataRow dr, int col)
        {
            try
            {
                if (!IsNull(dr, col)) { return (Convert.ToDouble(dr[col])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static string ToString(SqlDataReader dr, int col)
        {
            try
            {
                if (dr.IsDBNull(col)) return (string.Empty);
                return (dr[col].ToString());
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        public static string ToString(SqlDataReader dr, string col)
        {
            try
            {
                if (IsNull(dr, col)) return (string.Empty);
                return (dr[col].ToString());
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        public static string ToString(DataTableReader dr, string col)
        {
            try
            {
                if (IsNull(dr, col)) return (string.Empty);
                return (dr[col].ToString());
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        public static string ToString(DataTableReader dr, int col)
        {
            try
            {
                if (IsNull(dr, col)) return (string.Empty);
                return (dr[col].ToString());
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        public static long ToLong(SqlDataReader dr, string col)
        {
            try
            {
                if (IsNull(dr, col)) return (0);
                return (Convert.ToInt64(dr[col]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
        public static long ToLong(DataTableReader dr, string col)
        {
            try
            {
                if (IsNull(dr, col)) return (0);
                return (Convert.ToInt64(dr[col]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static long ToLong(SqlDataReader dr, int col)
        {
            try
            {
                if (dr.IsDBNull(col)) return (0);
                return (Convert.ToInt64(dr[col]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static int ToInt32(SqlDataReader dr, int col)
        {
            try
            {
                if (dr.IsDBNull(col)) return (0);
                return (Convert.ToInt32(dr[col]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static int ToTinyInt(SqlDataReader dr, int col)
        {
            try
            {
                if (dr.IsDBNull(col)) return (0);
                return (Convert.ToInt16(dr.GetByte(col)));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static bool ToBoolean(SqlDataReader dr, int col)
        {
            try
            {
                if (!IsNull(dr, col)) 
                {
                    string v = dr[col].ToString();
                    if (0 == string.Compare(v, "true", true)) return (true);
                    if (0 == string.Compare(v, "false", true)) return (false);
                    if (0 == string.Compare(v, "1", true)) return (true);
                    if (0 == string.Compare(v, "0", true)) return (false);
                    return (Convert.ToBoolean(dr[col])); 
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (false);
        }

        public static bool ToBoolean(SqlDataReader dr, string col)
        {
            try
            {
                if (!IsNull(dr, col))
                {
                    string v = dr[col].ToString();
                    if (0 == string.Compare(v, "true", true)) return (true);
                    if (0 == string.Compare(v, "false", true)) return (false);
                    if (0 == string.Compare(v, "1", true)) return (true);
                    if (0 == string.Compare(v, "0", true)) return (false);
                    return (Convert.ToBoolean(dr[col]));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (false);
        }

        public static bool ToBoolean(DataTableReader dr, string col)
        {
            try
            {
                if (!IsNull(dr, col))
                {
                    string v = dr[col].ToString();
                    if (0 == string.Compare(v, "true", true)) return (true);
                    if (0 == string.Compare(v, "false", true)) return (false);
                    if (0 == string.Compare(v, "1", true)) return (true);
                    if (0 == string.Compare(v, "0", true)) return (false);
                    return (Convert.ToBoolean(dr[col]));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (false);
        }


        public static bool ToBoolean(DataRow dr, int col)
        {
            try
            {
                if (!IsNull(dr, col))
                {
                    string v = dr[col].ToString();
                    if (0 == string.Compare(v, "true", true)) return (true);
                    if (0 == string.Compare(v, "false", true)) return (false);
                    if (0 == string.Compare(v, "1", true)) return (true);
                    if (0 == string.Compare(v, "0", true)) return (false);
                    return (Convert.ToBoolean(dr[col]));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (false);
        }

        public static bool ToBoolean(DataRow dr, string col)
        {
            try
            {
                if (!IsNull(dr, col))
                {
                    string v = dr[col].ToString();
                    if (0 == string.Compare(v, "true", true)) return (true);
                    if (0 == string.Compare(v, "false", true)) return (false);
                    if (0 == string.Compare(v, "1", true)) return (true);
                    if (0 == string.Compare(v, "0", true)) return (false);
                    return (Convert.ToBoolean(dr[col]));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (false);
        }

        public static UInt64 ToUInt64(DataRow dr, int col)
        {
            try
            {
                if (!IsNull(dr, col)) 
                {
                    if (dr[col] is string)
                    {
                        string s = dr[col].ToString();
                        if (s.Contains("-"))
                        {
                            s = s.Replace("-", "");
                            return (Convert.ToUInt64(s) + Int64.MaxValue + 1);
                        }
                    }
                    return (Convert.ToUInt64(dr[col])); 
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static UInt32 ToUInt32(DataRow dr, int col)
        {
            try
            {
                if (!IsNull(dr, col)) 
                {
                    if (dr[col] is string)
                    {
                        string s = dr[col].ToString();
                        if (s.Contains("-"))
                        {
                            s = s.Replace("-", "");
                            return (Convert.ToUInt32(s) + Int32.MaxValue + 1);
                        }
                    }
                    return (Convert.ToUInt32(dr[col])); 
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static UInt64 ToUInt64(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToUInt64(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
        public static UInt64 ToUInt64(SqlDataReader dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToUInt64(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
        public static long ToLong(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToInt64(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
        public static UInt32 ToUInt32(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) 
                {
                    if (dr[prop] is string)
                    {
                        string s = dr[prop].ToString();
                        if (s.Contains("-"))
                        {
                            s = s.Replace("-", "");
                            return (Convert.ToUInt32(s) + Int32.MaxValue + 1);
                        }
                    }
                    return (Convert.ToUInt32(dr[prop])); 
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
        public static Int32 ToInt32(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToInt32(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static Int32 ToInt32(SqlDataReader dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToInt32(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static UInt32 ToUInt32(SqlDataReader dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToUInt32(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static int ToTinyInt(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(prop)) return (Convert.ToInt32(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static int ToTinyInt(SqlDataReader dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) return (Convert.ToInt32(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static double ToDouble(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) { return (Convert.ToDouble(dr[prop])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        public static DateTime ToDateTime(DataRow dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) return ((DateTime)(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (DateTime.MinValue);
        }

        public static DateTime ToDateTime(SqlDataReader dr, string prop)
        {
            try
            {
                if (!IsNull(dr, prop)) return ((DateTime)(dr[prop]));
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (DateTime.MinValue);
        }

        public static string ToString(DataRow dr, string prop)
        {
            try
            {
                return (IsNull(dr, prop) ? string.Empty : dr[prop].ToString());
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        public static byte[] ToByteArray(DataRow dr, string prop)
        {
            try
            {
                return (IsNull(dr, prop) ? null : dr[prop] as byte[]);
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (null);
        }

        public static string ToString(DataRow dr, int col)
        {

            try
            {
                if (!IsNull(dr, col)) { return dr[col].ToString(); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (string.Empty);
        }

        internal static DateTime ToDateTime(SqlDataReader r, int col, CultureInfo format)
        {
            try
            {
                if (!IsNull(r, col)) { return DateTime.Parse(r[col].ToString(), format); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (DateTime.MinValue);
        }

        internal static DateTime ToDateTime(SqlDataReader r, int col)
        {
            try
            {
                if (!IsNull(r, col)) { return r.GetDateTime(col); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (DateTime.MinValue);
        }

        internal static float ToFloat(SqlDataReader r, int col)
        {
            try
            {
                if (!IsNull(r, col)) { return (Convert.ToSingle(r[col])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }

        internal static float ToFloat(SqlDataReader r, string col)
        {
            try
            {
                if (!IsNull(r, col)) { return (Convert.ToSingle(r[col])); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.Assert(false, ex.Message); }
            return (0);
        }
    }
}
