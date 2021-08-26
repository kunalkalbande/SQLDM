using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using BBS.TracerX;
using System.Collections;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common
{
    public static class ExceptionLogger
    {
        private static Logger _logX = Logger.GetLogger("ExceptionLogger");

        public static Exception Log(Exception ex)
        {
            Log("Exception: ", ex);
            return (ex);
        }

        //public static void Log(string msg, Exception ex)
        //{
        //   if (null != ex)
        //   {
        //      string details = GetAdditionalDetails(ex);
        //      logX.Error(msg + string.Format(": {0}", details.Replace("\r\n", "\n").Replace("\t", "  ")));
        //   }
        //}

        public static void Log(string msg, Exception ex) { Log(_logX, msg, ex); }

        public static void Log(Logger logX, string msg, Exception ex)
        {
            if (null == logX) logX = _logX;
            if (null != ex)
            {
                using (logX.ErrorCall(msg))
                {
                    ProcessExceptionStack(logX, ex, 0);
                    if (ex is System.OutOfMemoryException)
                    {
                        ProcessInfoHelper.Log(_logX);
                    }
                }
            }
        }

        public static string GetAdditionalDetails(Exception ex)
        {
            try
            {
                return (GetAdditionalDetails(ex, 0));
            }
            catch (Exception) { }
            if (null != ex)
            {
                return (string.Format(": Type={0}\n{1}\n{2}\n{3}", ex.GetType().ToString(), ex.Message, ex.StackTrace, ex.Source));
            }
            return (string.Empty);
        }

        private static string GetAdditionalDetails(Exception ex, int depth)
        {
            if (depth > 50)
            {
                return (string.Empty);
            }
            if (null != ex)
            {
                StringBuilder sb = new StringBuilder(1024);
                object val;
                sb.AppendLine("ExceptionType:" + ex.GetType());
                Type exType = ex.GetType();
                if (null != exType)
                {
                    PropertyInfo[] props = exType.GetProperties();
                    if (null != props)
                    {
                        foreach (PropertyInfo propInfo in props)
                        {
                            if (null != propInfo)
                            {
                                if (propInfo.CanRead && (0 != string.Compare("InnerException", propInfo.Name, true)))
                                {
                                    try
                                    {
                                        val = propInfo.GetValue(ex, null);
                                        if ((val is IEnumerable) &&
                                            ((val is IList) || (val is IDictionary)))
                                        {
                                            sb.AppendLine(string.Format("    IEnumerable {0}:{1}", propInfo.Name, val));
                                            sb.AppendLine(EnumerateProperties((IEnumerable)val));
                                        }
                                        else
                                        {
                                            sb.AppendLine(string.Format("    {0}: {1}", propInfo.Name, val));
                                        }
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }
                    }
                }
                if (null != ex.InnerException)
                {
                    sb.AppendLine();
                    sb.AppendLine("InnerException:");
                    sb.AppendLine(GetAdditionalDetails(ex.InnerException, depth + 1));
                }
                return (sb.ToString());
            }
            return (string.Empty);
        }

        private static void ProcessExceptionStack(Logger logX, Exception ex, int depth)
        {
            if (depth > 50)
            {
                return;
            }
            if (null != ex)
            {
                using (logX.ErrorCall())
                {
                    StringBuilder sb = new StringBuilder(1024);
                    object val;
                    sb.AppendLine("ExceptionType:" + ex.GetType());
                    Type exType = ex.GetType();
                    if (null != exType)
                    {
                        PropertyInfo[] props = exType.GetProperties();
                        if (null != props)
                        {
                            foreach (PropertyInfo propInfo in props)
                            {
                                if (null != propInfo)
                                {
                                    if (propInfo.CanRead && (0 != string.Compare("InnerException", propInfo.Name, true)))
                                    {
                                        try
                                        {
                                            val = propInfo.GetValue(ex, null);
                                            if ((val is IEnumerable) &&
                                                ((val is IList) || (val is IDictionary)))
                                            {
                                                sb.AppendLine(string.Format("    IEnumerable {0}:{1}", propInfo.Name, val));
                                                sb.AppendLine(EnumerateProperties((IEnumerable)val));
                                            }
                                            else
                                            {
                                                sb.AppendLine(string.Format("    {0}: {1}", propInfo.Name, val));
                                            }
                                        }
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }
                    }
                    logX.Error(sb.Replace("\r\n", "\n").Replace("\t", "  "));
                    ProcessExceptionStack(logX, ex.InnerException, depth + 1);
                }
            }
        }

        private static string GetObjectProperties(object obj)
        {
            if (null == obj)
            {
                return (string.Empty);
            }
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendLine("        Type:" + obj.GetType());
            Type obType = obj.GetType();
            if (null != obType)
            {
                PropertyInfo[] props = obType.GetProperties();
                if (null != props)
                {
                    foreach (PropertyInfo propInfo in props)
                    {
                        if (null != propInfo)
                        {
                            if (propInfo.CanRead)
                            {
                                try
                                {
                                    sb.AppendLine(string.Format("        {0}: {1}", propInfo.Name, propInfo.GetValue(obj, null)));
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            }
            return (sb.ToString());
        }

        private static string EnumerateProperties(IEnumerable obEnum)
        {
            if (null != obEnum)
            {
                StringBuilder sb = new StringBuilder(1024);
                foreach (object obj in obEnum)
                {
                    sb.AppendLine(GetObjectProperties(obj));
                }
                return (sb.ToString());
            }
            return (string.Empty);
        }
    }
}
