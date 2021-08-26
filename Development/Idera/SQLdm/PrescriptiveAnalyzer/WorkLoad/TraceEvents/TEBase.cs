using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.AdHoc;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Microsoft.Data.Schema.ScriptDom;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TEBase : IComparable
    {
        private static readonly bool _hashTSQL = Properties.Settings.Default.UseTSQLHashing;

        private readonly UInt64 _eventSequence = 0;
        private readonly UInt64 _duration = 0;
        private readonly UInt64 _reads = 0;
        private readonly UInt64 _writes = 0;
        private readonly UInt64 _cpu = 0;
        private readonly UInt32 _dbid = 0;
        private readonly string _ntUserName = string.Empty;
        private readonly string _hostName = string.Empty;
        private readonly string _loginName = string.Empty;
        private readonly string _applicationName = string.Empty;

        private string _objectName = string.Empty;

        private string _textData = string.Empty;
        private string _textNormalized = null;
        private DataBucket _parent = null;

        public UInt64 Duration { get { return (_duration); } }
        public UInt64 Reads { get { return (_reads); } }
        public UInt64 Writes { get { return (_writes); } }
        public UInt64 CPU { get { return (_cpu); } }
        public UInt32 DBID { get { return (_dbid); } }
        public string TextData { get { return ((null != _parent) ? _parent.TextData : _textData); } }
        public UInt64 EventSequence { get { return (_eventSequence); } }
        public string NTUserName { get { return (_ntUserName); } }
        public string HostName { get { return (_hostName); } }
        public string ApplicationName { get { return (_applicationName); } }
        public string LoginName { get { return (_loginName); } }
        public string ObjectName { get { return (_objectName); } protected set { _objectName = value; } }
        public string TextNormalized { get { return ((null != _parent) ? _parent.TextNormalized: _textNormalized); } }

        public TEBase(DataRow dr)
        {
            _eventSequence = DataHelper.ToUInt64(dr, "EventSequence");
            _duration = DataHelper.ToUInt64(dr, "Duration");
            _reads = DataHelper.ToUInt64(dr, "Reads");
            _writes = DataHelper.ToUInt64(dr, "Writes");
            _cpu = DataHelper.ToUInt64(dr, "CPU");
            _dbid = DataHelper.ToUInt32(dr, "DatabaseID");
            _ntUserName = DataHelper.ToString(dr, "NTUserName");
            _hostName = DataHelper.ToString(dr, "HostName");
            _applicationName = DataHelper.ToString(dr, "ApplicationName");
            _loginName = DataHelper.ToString(dr, "LoginName");
            _objectName = DataHelper.ToString(dr, "ObjectName");
            _textData = DataHelper.ToString(dr, "TextData");
        }

        public TEBase(SqlDataReader r)
        {
            _eventSequence = DataHelper.ToUInt64(r, "EventSequence");
            _duration = DataHelper.ToUInt64(r, "Duration");
            _reads = DataHelper.ToUInt64(r, "Reads");
            _writes = DataHelper.ToUInt64(r, "Writes");
            _cpu = DataHelper.ToUInt64(r, "CPU");
            _dbid = DataHelper.ToUInt32(r, "DatabaseID");
            _ntUserName = DataHelper.ToString(r, "NTUserName");
            _hostName = DataHelper.ToString(r, "HostName");
            _applicationName = DataHelper.ToString(r, "ApplicationName");
            _loginName = DataHelper.ToString(r, "LoginName");
            _objectName = DataHelper.ToString(r, "ObjectName");
            _textData = DataHelper.ToString(r, "TextData");
        }

        public TEBase(AdHocBatch b)
        {
            _eventSequence = 0;
            _duration = 0;
            _reads = 0;
            _writes = 0;
            _cpu = 0;
            _dbid = b.DBID;
            _ntUserName = string.Empty;
            _hostName = string.Empty;
            _applicationName = string.Empty;
            _loginName = string.Empty;
            _objectName = string.Empty;
            _textData = b.Batch;
        }

        public TEBase(TEWorstTSQL w, bool cloneMin)
        {
            _eventSequence = w.EventSequence;
            _duration = w.MinTime;
            _reads = w.MinReads;
            _writes = w.MinWrites;
            _cpu = w.MinCPU;
            _dbid = w.DBID;
            _textData = w.TextData;
            _textNormalized = w.TextNormalized;
            _objectName = w.ObjectName;
        }

        public void SetParentDataBucket(DataBucket parent)
        {
            if (null != parent)
            {
                _textData = string.Empty;
                _textNormalized = string.Empty;
                _parent = parent;
            }
        }

        private string GetHash(string s, SHA1 sha1)
        {
            if (!_hashTSQL) return (s);
            if (string.IsNullOrEmpty(s)) return (s);
            if (s.Length < 60) return (s);
            try
            {
                byte[] buffer = System.Text.Encoding.Unicode.GetBytes(s);
                return (BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", ""));
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                ExceptionLogger.Log(string.Format("GetHash({0}) failed!", s), ex);
                return (string.Empty);
            }
        }

        public string GetTextNormalized(TSql90Parser parser, SHA1 sha1)
        {
            if ((null == _textNormalized) && (null == _parent))
            {
                StringBuilder sb = new StringBuilder(_textData.Length);
                using (TextReader reader = new StringReader(_textData))
                {
                    IList<ParseError> errors = new List<ParseError>();
                    IList<TSqlParserToken> tokens = parser.GetTokenStream(reader, errors);
                    foreach (TSqlParserToken token in tokens)
                    {
                        if (TSqlTokenType.EndOfFile != token.TokenType)
                        {
                            if (KeepToken(token)) sb.Append(token.Text.ToUpper());
                        }
                    }
                }
                _textNormalized = GetHash(sb.ToString(), sha1);

            }
            return (TextNormalized);
        }

        internal virtual bool KeepToken(TSqlParserToken token)
        {
            switch (token.TokenType)
            {
                case (TSqlTokenType.MultilineComment):
                case (TSqlTokenType.SingleLineComment):
                case (TSqlTokenType.AsciiStringLiteral):
                case (TSqlTokenType.UnicodeStringLiteral):
                case (TSqlTokenType.WhiteSpace):
                case (TSqlTokenType.EndOfFile):
                case (TSqlTokenType.Integer):
                case (TSqlTokenType.Real):
                case (TSqlTokenType.Double):
                case (TSqlTokenType.Money):
                case (TSqlTokenType.HexLiteral):
                    {
                        return (false);
                    }
            }
            return (true);
        }
        public static bool operator <(TEBase x, TEBase y)
        {
            return (x.CompareTo(y) < 0);
        }
        public static bool operator >(TEBase x, TEBase y)
        {
            return (x.CompareTo(y) > 0);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            TEBase te = obj as TEBase;
            if (null == te) return (-1);
            if (_duration > te.Duration) 
            {
                return (1);
            }
            else if (_duration == te.Duration)
            {
                if (_cpu > te.CPU)
                {
                    return (1);
                }
                else if (_cpu == te.CPU)
                {
                    if (_writes > te.Writes)
                    {
                        return (1);
                    }
                    else if (_writes == te.Writes)
                    {
                        if (_reads > te.Reads)
                        {
                            return (1);
                        }
                        else if (_reads == te.Reads)
                        {
                            return (0);
                        }
                    }
                }
            }
            return (-1);
        }

        #endregion

        internal virtual void DumpData(BBS.TracerX.Logger _logX)
        {
            _logX.InfoFormat("Duration = {0}", Duration);
            _logX.InfoFormat("Reads = {0}", Reads);
            _logX.InfoFormat("Writes = {0}", Writes);
            _logX.InfoFormat("CPU = {0}", CPU);
            _logX.InfoFormat("DBID = {0}", DBID);
            _logX.InfoFormat("TextData = {0}", TextData);
            _logX.InfoFormat("EventSequence = {0}", EventSequence);
            _logX.InfoFormat("NTUserName = {0}", NTUserName);
            _logX.InfoFormat("HostName = {0}", HostName);
            _logX.InfoFormat("ApplicationName = {0}", ApplicationName);
            _logX.InfoFormat("LoginName = {0}", LoginName);
            _logX.InfoFormat("ObjectName = {0}", ObjectName);
            _logX.InfoFormat("TextNormalized = {0}", TextNormalized);
        }
    }
}
