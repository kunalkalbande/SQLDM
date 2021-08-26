using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using BBS.TracerX;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public enum SpecialValueMode
    {
        Expand,
        Translate,
        Ignore,
        Strip
    }

    public static class FormatHelper
    {
        private static Logger LOG = Logger.GetLogger("FormatHelper");
        private const string DEFAULT_FORMAT_SPEC = "{0}";
        public static string Format(string template, object arg, bool bullettedList)
        {
            if (bullettedList) return (FormatHelper.AdjustBullettedList(FormatHelper.Format(template, arg)));
            return (FormatHelper.Format(template, arg));
        }
        public static string Format(string template, object arg)
        {
            StringBuilder result = new StringBuilder(template);

            int varloc = template.LastIndexOf("$(");
            while (varloc >= 0)
            {
                int endloc = GetMatchingParen(template, varloc);
                if (endloc == -1)
                {
                    return result.ToString();
                }
                // get between $( and )
                string comparison = String.Empty;
                string comparand = String.Empty;
                string guts = template.Substring(varloc + 2, endloc - varloc - 2);
                string format = SplitFormat(ref guts, ref comparison, ref comparand);

                object value = IsSpecialValue(guts) ? GetSpecialValue(guts, SpecialValueMode.Expand) : GetProperty(arg, guts);

                result.Remove(varloc, endloc - varloc + 1);
                result.Insert(varloc, FormatValue(format, value, comparison, comparand));

                template = result.ToString();

                if (varloc > 0)
                    varloc = template.LastIndexOf("$(", varloc - 1);
                else 
                    varloc--;
            }
            return (result.ToString());
        }

        private static bool IsSpecialValue(string symbol)
        {
            switch (symbol)
            {
                case "br":
                case "li":
                    return true;
            }

            return false;
        }

        private static string GetSpecialValue(string symbol, SpecialValueMode mode)
        {
            switch (mode)
            {
                case SpecialValueMode.Expand:
                    return ExpandSpecialValue(symbol);
                case SpecialValueMode.Strip:
                    return String.Empty;
                case SpecialValueMode.Translate:
                    return TranslateSpecialValue(symbol);
            }

            return symbol;
        }

        static string BULLETT = "•";
        static string NL = Environment.NewLine;
        public static string LIST_ITEM = Environment.NewLine + "    " + BULLETT;
        private static string ExpandSpecialValue(string symbol)
        {
            int bcv = (int)BULLETT[0];
            if (symbol.Equals("li"))
                return LIST_ITEM;
            if (symbol.Equals("br"))
                return Environment.NewLine;

            return symbol;
        }
        public static string AdjustBullettedList(string list)
        {
            return (list.TrimStart(Environment.NewLine.ToCharArray()).Trim().Replace(LIST_ITEM, Environment.NewLine + BULLETT));
        }

        public static string RemoveBullets(string list) { return (list.Replace(BULLETT, "")); }

        private static string TranslateSpecialValue(string symbol)
        {
            return symbol;
        }

        private static string FormatValue(string format, object value, string comparison, string comparand)
        {
            bool positiveFormat = true;

            try
            {
                if (String.IsNullOrEmpty(format))
                    return String.Empty;

                string[] formats = format.Split(':');

                // null is a special case
                if ((value == null) ||
                    (value is string && String.IsNullOrEmpty((string)value)) ||
                    (value is ICollection && ((ICollection)value).Count == 0))
                {
                    if (formats.Length > 1)
                        return formats[1];
                    else
                        return String.Empty;
                }

                if (formats.Length > 1 && !String.IsNullOrEmpty(comparison) && !String.IsNullOrEmpty(comparand))
                {
                    positiveFormat = EvaluateExpression(value, comparison, comparand);
                }

                string fmt = formats[0];
                if (!positiveFormat && formats.Length > 1)
                {
                    fmt = formats[1];
                }

                if (value is Enum)
                    return String.Format(EnumFormatProvider.Instance, fmt, value);
                if (value is String)
                    return String.Format(fmt, value);
                if (value is Array || value is ICollection)
                    return String.Format(ArrayFormatProvider.Instance, fmt, value);

                return String.Format(fmt, value);
            }
            catch (Exception e)
            { 
                string f = format == null ? "<null>" : format;
                string v = value == null ? "<null>" : value.ToString();
                string cs = comparison == null ? "<null>" : comparison;
                string cd = comparand == null ? "<null>" : comparand;
                LOG.DebugFormat("Format error: format={0} value={1} comparison={2} comparand={3}: {4}", f, v, cs, cd, e);
            }

            return format;
        }

        public static string FormatMS(decimal d)
        {
            if (d > 1000) return (string.Format("{0:0.#}s", (d / 1000)));
            return (string.Format("{0:0.#}ms", d));
        }

        public static string FormatBytes(IntPtr bytes)
        {
            try { return (FormatBytes(Convert.ToUInt64(bytes))); }
            catch (Exception ex) { return (ex.Message); }
        }
        public static string FormatBytes(long bytes)
        {
            try { return (FormatBytes(Convert.ToUInt64(bytes))); }
            catch (Exception ex) { return (ex.Message); }
        }
        public static string FormatBytes(UInt64 bytes)
        {
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            UInt64 max = 1 << 30;

            foreach (string order in orders)
            {
                if (bytes >= max) return (string.Format("{0:0.#} {1}", bytes / (double)max, order));
                max = max >> 10;
            }
            return "0 Bytes";
        }

        public static string FormatBytesToMB(UInt64 bytes)
        {
            return (string.Format("{0:0.#} MB", bytes / (double)(1 << 20)));
        }

        public static string FormatTimeToString(TimeSpan ts)
        {
            StringBuilder sb = new StringBuilder(128);
            try
            {
                FormatTimeToString(sb, ts.Days, "day");
                FormatTimeToString(sb, ts.Hours, "hour");
                FormatTimeToString(sb, ts.Minutes, "minute");
                FormatTimeToString(sb, ts.Seconds, "second");
                if (0 == sb.Length)
                {
                    sb.Append(string.Format("{0,2} seconds", ts.TotalMilliseconds / 1000));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("FormatHelper.FormatTimeToString Exception: ", ex);
            }
            return (sb.ToString().Trim());
        }

        private static void FormatTimeToString(StringBuilder sb, int num, string lit)
        {
            if (num > 0)
            {
                sb.Append(string.Format("{0} {1}", num, lit));
                if (1 != num) sb.Append("s");
                sb.Append(" ");
            }
        }

        private static bool EvaluateExpression(object value, string comparison, string comparand)
        {
            if (value == null)
                value = (int)UInt32.MinValue;
            // convert comparand to same type as value
            object vcomparand = (int)UInt32.MinValue;
            try
            {
                vcomparand = Convert.ChangeType(comparand, value.GetType());
            }
            catch 
            {
                value = value.ToString();
                vcomparand = comparand;
            }

            int eval = ((IComparable)value).CompareTo(vcomparand);
            switch (comparison)
            {
                case ">":
                    return eval > 0;
                case ">=":
                    return eval >= 0;
                case "<":
                    return eval < 0;
                case "<=":
                    return eval <= 0;
                case "=":
                    return eval == 0;
                case "<>":
                    return eval != 0;
            }

            return false;
        }

        private static string SplitFormat(ref string guts, ref string comparator, ref string comparand)
        {
            int br = guts.IndexOf('?');
            if (br == -1)
                return DEFAULT_FORMAT_SPEC;

            string result = guts.Substring(br + 1);
            if (String.IsNullOrEmpty(result))
                result = DEFAULT_FORMAT_SPEC;

            // remove format leaving property name
            guts = guts.Substring(0, br);

            // check for comparand and split out the values
            int ix = guts.IndexOfAny(new char[] {'<', '>', '='});
            if (ix > 0)
            {
                int len = guts[ix + 1] == '=' ? 2 : 1;
                comparator = guts.Substring(ix, len);
                comparand = guts.Substring(ix + len).Trim();
                guts = guts.Substring(0, ix);
            }

            return result;
        }

        private static int GetMatchingParen(string template, int varloc)
        {
            int level = 0;
            for (int i = varloc + 1; i < template.Length; i++)
            {
                switch (template[i])
                {
                    case '(':
                        level++;
                        break;
                    case ')':
                        level--;
                        if (level == 0)
                            return i;
                        break;
                }
            }
            return -1;
        }

        public static object GetPropertyValue(object o, string property)
        {
            Type otype = o.GetType();
            PropertyInfo propInfo = otype.GetProperty(property);
            if (propInfo != null)
                return propInfo.GetValue(o, null);

            return null;
        }

        private const BindingFlags MemberAccess = BindingFlags.Public |
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.Static   |
                                                    BindingFlags.Instance |
                                                    BindingFlags.IgnoreCase;

        private const MemberTypes MemberType = MemberTypes.Method |
                                                MemberTypes.Property |
                                                MemberTypes.Field;

        public static object GetProperty(object obj, string property)
        {
            return (GetProperty(obj, property, true));
        }

        public static object GetProperty(object obj, string property, bool throwExceptions)
        {
            MemberInfo member = null;
            object o = obj;
            string[] parts = property.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                string propName = parts[i];
                if (propName.Equals("this", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (propName.Equals("me", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                int px = propName.IndexOf('(');
                if (px > 0)
                {
                    if (px + 1 >= propName.Length || propName[px + 1] != ')')
                    {
                        if (!throwExceptions) return (null);
                        throw new FormatException(String.Format("Invalid method name '{0}' in property path '{1}'", propName, property));
                    }

                    propName = propName.Substring(0, px);
                }

                try
                {
                    MemberInfo[] a = o.GetType().GetMember(propName, MemberType, MemberAccess);
                    if ((null == a) || (a.Length <= 0))
                    {
                        if (!throwExceptions) return (null);
                        throw new FormatException(String.Format("Property '{0}' in '{1}' not found", propName, property));
                    }
                    member = a[0];
                }
                catch (Exception e)
                {
                    if (!throwExceptions) return (null);
                    throw new FormatException(String.Format("Property '{0}' in '{1}' not found", propName, property), e);
                }

                if (member == null)
                {
                    if (!throwExceptions) return (null);
                    throw new FormatException(String.Format("Property '{0}' in '{1}' not found", propName, property));
                }

                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        o = ((PropertyInfo)member).GetValue(o, null);
                        break;
                    case MemberTypes.Method:
                        o = ((MethodInfo)member).Invoke(o, null);
                        break;
                    case MemberTypes.Field:
                        o = ((FieldInfo)member).GetValue(o);
                        break;
                    default:
                        {
                            if (!throwExceptions) return (null);
                            throw new FormatException(String.Format("Unable to find value for '{0}' in '{1}'", propName, property));
                        }
                }
            }
            return o;
        }

        public static object SetProperty(object parent, string property, object value)
        {
            Type Type = parent.GetType();
            MemberInfo Member = null;

            // *** no more .s - we got our final object
            int lnAt = property.IndexOf(".");
            if (lnAt < 0)
            {
                Console.WriteLine("Getting: " + property);
                foreach (MemberInfo member in Type.GetMembers())
                {
                    Console.WriteLine("Member: " + member);
                }
                Member = Type.GetMember(property, MemberAccess)[0];
                Console.WriteLine("Getting ok");

                if (Member.MemberType == MemberTypes.Property)
                {

                    ((PropertyInfo)Member).SetValue(parent, value, null);
                    return null;
                }
                else
                {
                    ((FieldInfo)Member).SetValue(parent, value);
                    return null;
                }
            }

            // *** Walk the . syntax
            string Main = property.Substring(0, lnAt);
            string Subs = property.Substring(lnAt + 1);
            Member = Type.GetMember(Main, MemberAccess)[0];

            object Sub;
            if (Member.MemberType == MemberTypes.Property)
                Sub = ((PropertyInfo)Member).GetValue(parent, null);
            else
                Sub = ((FieldInfo)Member).GetValue(parent);

            // *** Recurse until we get the lowest ref
            SetProperty(Sub, Subs, value);
            return null;
        }

        public static string Format(Enum e, string format)
        {
            return EnumFormatProvider.Instance.Format(format, e, EnumFormatProvider.Instance);
        }

        public static string Format(Array e, string format)
        {
            return ArrayFormatProvider.Instance.Format(format, e, ArrayFormatProvider.Instance);
        }

        //Start: DM 10.0 format converters
        public static string FormatBoolToString(bool val)
        {
            return val ? "true" : "false";
        }

        public static bool FormatStringToBool(string val)
        {
            return val.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
        }
               
        public static AffectedBatches FormatStringToAffectedBatches(string val)
        {
            //todo : deserialize to AffectedBatches
            return new AffectedBatches();
        }

        //End: DM 10.0 format converters

        public class EnumFormatProvider : IFormatProvider, ICustomFormatter
        {
            public static readonly EnumFormatProvider Instance = new EnumFormatProvider();
            
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                return null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                Type atype = arg.GetType();

                string formatCode = (String.IsNullOrEmpty(format)) ? "G" : format.Substring(0,1);
                switch (formatCode.ToUpperInvariant())
                {
                    case "N":
                    case "A":
                        break;
                    default:
                        return ((Enum)arg).ToString(format);
                }
                if (atype.IsEnum)
                {
                    bool flags = Attribute.IsDefined(atype, typeof(FlagsAttribute));
                    if (flags)
                        return FormatFlags(format, arg, formatProvider);

                    string name = Enum.GetName(atype, arg);
                    if (formatCode == "N")
                        return name;

                    FieldInfo field = atype.GetField(Enum.GetName(atype, arg));
                    if (field != null)
                    {
                        DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                        return (da != null) ? da.Description : name;
                    }
                }
                return arg.ToString();
            }

            private string FormatFlags(string format, object arg, IFormatProvider formatProvider)
            {
                string delimiter = ", ";
                if (format.Length > 1)
                    delimiter = format.Substring(1);

                string formatCode = format.Substring(0, 1).ToUpperInvariant();

                StringBuilder result = new StringBuilder();

                Type atype = arg.GetType();
                ulong avalue = Convert.ToUInt64(arg);
                foreach (object value in Enum.GetValues(atype))
                {
                    ulong v = Convert.ToUInt64(value);
                    if ((v & avalue) != 0)
                    {
                        if (result.Length > 0)
                            result.Append(delimiter);

                        string name = Enum.GetName(atype, value);
                        if (formatCode == "N")
                            result.Append(name);
                        else
                        {

                            FieldInfo field = atype.GetField(name);
                            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                            string desc = (da != null) ? da.Description : name;
                            result.Append(desc);
                        }
                    }
                }
                return result.ToString();
            }
        }


        public class ArrayFormatProvider : IFormatProvider, ICustomFormatter
        {
            public static readonly ArrayFormatProvider Instance = new ArrayFormatProvider();
            #region IFormatProvider Members

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;

                return null;
            }

            #endregion

            #region ICustomFormatter Members

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg is IEnumerable)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (object o in ((IEnumerable)arg))
                    {
                        string s = String.Empty;
                        if (o.GetType().IsEnum)
                            s = FormatHelper.Format((Enum)o,format);
                        else
                            s = o.ToString();
                        if (String.IsNullOrEmpty(s))
                            continue;
                        if (sb.Length > 0)
                            sb.Append(", ");

                        sb.Append(s);
                    }
                    return sb.ToString();
                }
                return arg.ToString();
            }

            #endregion
        }
    }
}