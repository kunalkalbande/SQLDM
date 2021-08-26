using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using System.Globalization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.RealTime.GridData;
using TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    public class WMIObjectProperties : IProvideGridData
    {
        private static Logger _logX = Logger.GetLogger("WMIObjectProperties");

        Dictionary<string, string> _props = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        public WMIObjectProperties() { }
        public bool AddProp(string name, string value)
        {
            if (_props.ContainsKey(name)) return (false);
            _props.Add(name, value);
            return (true);
        }
        public string GetString(string name)
        {
            string value = string.Empty;
            _props.TryGetValue(name, out value);
            return (value);
        }

        private static string NegativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;
        public bool IsSigned(string value)
        {
            if (String.IsNullOrEmpty(value))
                return false;
            return (value.Contains(NegativeSign));
        }


        public UInt32 GetUInt32(string name)
        {
            string value = string.Empty;
            if (_props.TryGetValue(name, out value))
            {
                try
                {
                    if (IsSigned(value))
                        return (UInt32)(Convert.ToInt32(value));

                    if (!String.IsNullOrEmpty(value))
                    {
                        return (Convert.ToUInt32(value));
                    }
                    else _logX.InfoFormat("GetUInt32 - value for {0} is blank - returing 0", name);
                }
                catch (Exception ex) { ExceptionLogger.Log(string.Format("WMIObjectProperties.GetUInt32({0}) Value:{1}", name, value), ex); }
            }
            return (default(UInt32));
        }

        public UInt64 GetUInt64(string name)
        {
            string value = string.Empty;
            if (_props.TryGetValue(name, out value))
            {
                if (!String.IsNullOrEmpty(value))
                {
                    try
                    {
                        if (IsSigned(value))
                            return (UInt64)(Convert.ToInt64(value));
                        return (Convert.ToUInt64(value));
                    }
                    catch (Exception ex) { ExceptionLogger.Log(string.Format("WMIObjectProperties.GetUInt64({0}) Value:{1}", name, value), ex); }
                }
                else _logX.InfoFormat("GetUInt64 - value for {0} is blank - returing 0", name);
            }
            return (default(UInt64));
        }
        public Int32 GetInt32(string name)
        {
            string value = string.Empty;
            if (_props.TryGetValue(name, out value))
            {
                if (!String.IsNullOrEmpty(value))
                {
                    try { return (Convert.ToInt32(value)); }
                    catch (Exception ex) { ExceptionLogger.Log(string.Format("WMIObjectProperties.GetInt32({0}) Value:{1}", name, value), ex); }
                }
                else _logX.InfoFormat("GetInt32 - value for {0} is blank - returing 0", name);
            }
            return (default(Int32));
        }

        public Int64 GetInt64(string name)
        {
            string value = string.Empty;
            if (_props.TryGetValue(name, out value))
            {
                if (!String.IsNullOrEmpty(value))
                {
                    try { return (Convert.ToInt64(value)); }
                    catch (Exception ex) { ExceptionLogger.Log(string.Format("WMIObjectProperties.GetInt64({0}) Value:{1}", name, value), ex); }
                }
                else _logX.InfoFormat("GetInt64 - value for {0} is blank - returing 0", name);
            }
            return (default(Int64));
        }
        public bool GetBool(string name)
        {
            string value = string.Empty;
            if (_props.TryGetValue(name, out value))
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (0 == string.Compare(value, "true", true)) return (true);
                    if (0 == string.Compare(value, "false", true)) return (false);
                    if (0 == string.Compare(value, "1", true)) return (true);
                    if (0 == string.Compare(value, "0", true)) return (false);
                    try { return (Convert.ToBoolean(value)); }
                    catch (Exception ex) { ExceptionLogger.Log(string.Format("WMIObjectProperties.GetBool({0}) Value:{1}", name, value), ex); }
                }
                else _logX.InfoFormat("GetBool - value for {0} is blank - returing false", name);
            }
            return (default(bool));
        }
    }
}
