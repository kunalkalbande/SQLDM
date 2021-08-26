using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class RecommendationLink
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string CondensedLink { get; set; }
        public string Condition { get; set; }
    }

    [Serializable]
    [TypeConverter(typeof(RecommendationLinksConverter))]
    public class RecommendationLinks : List<RecommendationLink>
    {
        private class RecommendationCondition 
        {
            private enum RecommendationConditionValue
            {
                Unknown = 0,
                // OS version values
                OSVersion,
                OSVersionMajor,
                OSVersionMinor,

                // SQL version values
                SQLVersion,
                SQLVersionMajor,
                SQLVersionMinor,

                SQL64Bit,
                SQL32Bit,
            }
            [Flags]
            private enum RecommendationConditionOperation
            {
                Unknown     = 0x00,
                Equal       = 0x01,
                LessThan    = 0x02,
                GreaterThan = 0x04,
                Not         = 0x08,
            }

            private RecommendationConditionValue _rcv = RecommendationConditionValue.Unknown;
            private RecommendationConditionOperation _op = RecommendationConditionOperation.Unknown;
            private string _value = string.Empty;

            public RecommendationCondition(string condition)
            {
                if (string.IsNullOrEmpty(condition)) return;
                string[] parts = condition.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (null == parts) return;
                if (parts.Length < 3) return;
                //------------------------------------------------------
                // try to parse out the value being compared against.
                //
                try { _rcv = (RecommendationConditionValue)Enum.Parse(typeof(RecommendationConditionValue), parts[0].Trim(), true); }
                catch (Exception ex) { ExceptionLogger.Log(string.Format("RecommendationCondition({0}) Exception:", condition), ex); }
                int index = 1;
                char[] operators = new char[] { '<', '>', '=', '!'};
                string op = string.Empty;
                for (; index < parts.Length; ++index)
                {
                    if (-1 == parts[index].IndexOfAny(operators)) break;
                    op = op + parts[index];
                }
                if (op.Contains("=")) _op |= RecommendationConditionOperation.Equal;
                if (op.Contains("<")) _op |= RecommendationConditionOperation.LessThan;
                if (op.Contains(">")) _op |= RecommendationConditionOperation.GreaterThan;
                if (op.Contains("!")) _op |= RecommendationConditionOperation.Not;
                StringBuilder sb = new StringBuilder();
                for (; index < parts.Length; ++index)
                {
                    if (sb.Length > 0) sb.Append(" ");
                    sb.Append(parts[index]);
                }
                _value = sb.ToString();
            }

            internal bool IsTrue(IRecommendation r, string productVersion, string windowsVersion)
            {
                switch (_rcv)
                {
                    //case (RecommendationConditionValue.OSVersion):
                    //    {
                    //        return (EvaluateCondition(GetMajorMinor(ar.AnalysisValues.SnapshotValues.WindowsVersion)));
                    //    }
                    //case (RecommendationConditionValue.OSVersionMajor):
                    //    {
                    //        return (EvaluateCondition(GetMajor(ar.AnalysisValues.SnapshotValues.WindowsVersion)));
                    //    }
                    //case (RecommendationConditionValue.OSVersionMinor):
                    //    {
                    //        return (EvaluateCondition(GetMinor(ar.AnalysisValues.SnapshotValues.WindowsVersion)));
                    //    }
                    case (RecommendationConditionValue.SQLVersion):
                        {
                            return (EvaluateCondition(GetMajorMinor(productVersion)));
                        }
                    case (RecommendationConditionValue.SQLVersionMajor):
                        {
                            return (EvaluateCondition(GetMajor(productVersion)));
                        }
                    case (RecommendationConditionValue.SQLVersionMinor):
                        {
                            return (EvaluateCondition(GetMinor(productVersion)));
                        }
                    case (RecommendationConditionValue.SQL64Bit):
                        {
                            return (EvaluateCondition(Is64BitVersionString(productVersion).ToString()));
                        }
                    case (RecommendationConditionValue.SQL32Bit):
                        {
                            return (EvaluateCondition((!Is64BitVersionString(productVersion)).ToString()));
                        }
                }
                return (true);
            }

            private bool Is64BitVersionString(string p)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    string version = p.ToUpper();
                    if (version.Contains("X64") || version.Contains("64-BIT") || version.Contains("64BIT") || version.Contains("64 BIT"))
                    {
                        return (true);
                    }
                }
                return (false);
            }

            private string GetMinor(string p){return (RightOf(GetMajorMinor(p), '.'));}
            private string GetMajor(string p){return (LeftOf(GetMajorMinor(p), '.'));}

            private string RightOf(string p, char c)
            {
                int index = p.IndexOf(c) + 1;
                if ((index >= 0) && (index < p.Length))
                {
                    return (p.Substring(index));
                }
                return (p);
            }


            private string LeftOf(string p, char c)
            {
                int index = p.IndexOf(c);
                if ((index >= 0) && (index < p.Length))
                {
                    return (p.Substring(0, index));
                }
                return (p);
            }

            private bool EvaluateCondition(string p)
            {
                switch (_rcv)
                {
                    case (RecommendationConditionValue.OSVersion):
                    case (RecommendationConditionValue.OSVersionMajor):
                    case (RecommendationConditionValue.OSVersionMinor):
                    case (RecommendationConditionValue.SQLVersion):
                    case (RecommendationConditionValue.SQLVersionMajor):
                    case (RecommendationConditionValue.SQLVersionMinor):
                        {
                            double d1 = 0;
                            double d2 = 0;
                            try { if (!string.IsNullOrEmpty(_value)) d1 = Convert.ToDouble(_value.Trim()); }
                            catch (Exception ex) { ExceptionLogger.Log(string.Format("RecommendationCondition.EvaluateCondition({0})", p), ex); }
                            try { if (!string.IsNullOrEmpty(p)) d2 = Convert.ToDouble(p.Trim()); }
                            catch (Exception ex) { ExceptionLogger.Log(string.Format("RecommendationCondition.EvaluateCondition({0})", p), ex); }
                            return(EvaluateCondition(d1, d2));
                        }
                    default:
                        {
                            return (EvaluateCondition(_value, p));
                        }
                }
            }

            private bool EvaluateCondition(double d1, double d2)
            {
                if (0 != (_op & RecommendationConditionOperation.Equal))
                {
                    if (d1 == d2) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                if (0 != (_op & RecommendationConditionOperation.GreaterThan))
                {
                    if (d1 < d2) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                if (0 != (_op & RecommendationConditionOperation.LessThan))
                {
                    if (d1 > d2) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                return (false);
            }

            private bool EvaluateCondition(string s1, string s2)
            {
                int result = string.Compare(s1, s2, true);
                if (0 != (_op & RecommendationConditionOperation.Equal))
                {
                    if (0 == result) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                if (0 != (_op & RecommendationConditionOperation.GreaterThan))
                {
                    if (0 < result) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                if (0 != (_op & RecommendationConditionOperation.LessThan))
                {
                    if (0 > result) return (0 == (_op & RecommendationConditionOperation.Not));
                }
                return (false);
            }

            private string GetMajorMinor(string p)
            {
                if (string.IsNullOrEmpty(p)) return string.Empty;
                string[] parts = p.Split(' ');
                if (null == parts) return string.Empty;
                if (parts.Length < 1) return string.Empty;
                parts = parts[0].Split('.');
                if (null == parts) return string.Empty;
                if (parts.Length < 1) return string.Empty;
                if (parts.Length > 1) return (parts[0] + "." + parts[1]);
                return (parts[0] + ".0");
            }
        }
        public string ID { get; set; }
        public RecommendationLinks() : base() { }
        public RecommendationLinks(RecommendationLinks rl) : base(rl)
        {
            ID = rl.ID;
        }
        public RecommendationLinks Filtered(IRecommendation r, string productVersion, string windowsVersion)
        {
            RecommendationLinks result = new RecommendationLinks(this);
            RecommendationLinks defaults = null;
            result.RemoveAll(delegate(RecommendationLink l)
            {
                if (string.IsNullOrEmpty(l.Condition)) return (false);
                if (0 == string.Compare("default", l.Condition, true))
                {
                    if (null == defaults) defaults = new RecommendationLinks();
                    defaults.Add(l);
                    return (true);
                }
                return (!EvaluateConditions(GetConditions(l.Condition), r, productVersion, windowsVersion));
            });
            return (result);
        }

        private bool EvaluateConditions(IEnumerable<RecommendationCondition> conditions, IRecommendation r, string productVersion, string windowsVersion)
        {
            if (null == conditions) return (false);
            foreach (RecommendationCondition c in conditions)
            {
                if (null == c) continue;
                if (!c.IsTrue(r, productVersion, windowsVersion)) return (false);
            }
            return (true);
        }

        private IEnumerable<RecommendationCondition> GetConditions(string condition)
        {
            string[] conditions = condition.Split(new string[] {" AND ", " and "}, StringSplitOptions.RemoveEmptyEntries);
            if (null != conditions)
            {
                foreach (string s in conditions)
                {
                    yield return (new RecommendationCondition(s));
                }
            }
        }
    }

    public class RecommendationLinksConverter : TypeConverter
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(RecommendationLinks));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                StringReader reader = new StringReader(value.ToString());
                XmlReader xmlReader = XmlReader.Create(reader);
                try
                {
                    return serializer.Deserialize(xmlReader);
                }
                catch 
                {
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is RecommendationLinks)
            {
                try
                {
                    StringBuilder dest = new StringBuilder();
                    XmlWriter xmlWriter = XmlWriter.Create(dest);
                    serializer.Serialize(xmlWriter, value);
                    xmlWriter.Close();
                    return dest.ToString();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
