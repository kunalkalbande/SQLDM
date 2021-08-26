//------------------------------------------------------------------------------
// <copyright file="Deadlock.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using BBS.TracerX;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents a SQL Server blocking session.
    /// Data comes from trace session
    /// </summary>
    [Serializable]
    public sealed class BlockingSessionInfo : IEquatable<BlockingSessionInfo>
    {
        #region fields

        private Guid id = Guid.NewGuid();
        private string xdlString = "";
        private DateTime? startTime = null;
        private DateTime? _blockingLastBatch = null;
        private long _xActID = -1;
        private string _clientApp;
        private DateTime _endTime;
        private string _dbName;
        private string _host;
        private string _inputBuffer;
        private string _login;
        private int _spid;
        private int? _objectID;
        private BlockedSessionInfo _blockedSession;
        private string _resource = string.Empty;
        #endregion

        #region constructors
        /// <summary>
        /// Blocking session returned by blocked process report
        /// </summary>
        /// <param name="xdlString"></param>
        /// <param name="databaseNames"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        internal BlockingSessionInfo(string xdlString, Dictionary<long, string> databaseNames, DateTime? startTime, DateTime endTime)
        {
            XdlString = xdlString;
            StartTime = startTime;
            EndTime = endTime;
            InsertDatabaseNames(databaseNames);

            GetBlockingXActAttributes();
            _blockedSession = new BlockedSessionInfo(xdlString);
        }

        #endregion

        #region properties

        public string WaitResource
        {
            get { return BlockedSession.WaitResource; }
            set { BlockedSession.WaitResource = value; }
        }

        public int? ObjectID
        {
            get { return _objectID; }
            set { _objectID = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public BlockedSessionInfo BlockedSession
        {
            get { return _blockedSession; }
        }

        public DateTime BlockingStartTime
        {
            get { return EndTime.Subtract(new TimeSpan(0,0,0,0,(int)BlockedSession.BlockingTimems)); }
        }

        public long BlockingTimems
        {
            get { return BlockedSession.BlockingTimems; }
        }

        public int SessionId
        {
            get { return _spid; }
        }

        public string Login
        {
            get { return _login; }
        }

        public string InputBuffer
        {
            get { return _inputBuffer; }
        }

        public string Host
        {
            get { return _host; }
        }

        public string ClientApp
        {
            get { return _clientApp; }
        }

        /// <summary>
        /// Transaction start time of the blocked session
        /// </summary>
        private DateTime TransactionStartTime
        {
            get { return this.BlockedSession.TransactionStartTime; }
        }

        public string DatabaseName
        {
            get { return _dbName; }
        }

        public long XActId
        {
            get { return _xActID; }
            set { _xActID = value; }
        }

        public string XdlString
        {
            get { return xdlString; }
            internal set { xdlString = value.Replace('\0', ' '); }
        }


        public DateTime? StartTime
        {
            get { return startTime; }
            internal set { startTime = value; }
        }
        
        public DateTime? LastBatchCompleted
        {
            get { return _blockingLastBatch; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public Guid GetId()
        {
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the xact id of the blocking session</returns>
        private void GetBlockingXActAttributes()
        {
            Logger LOG = Logger.GetLogger("GetBlockingXActAttributes");
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.CheckCharacters = false;
                using (XmlReader reader = XmlReader.Create(new StringReader(xdlString), readerSettings))
                {
                    doc.Load(reader);
                    var node = doc.SelectSingleNode("blocked-process-report/blocking-process/process");
                    if (node != null)
                    {
                        XmlAttributeCollection attributeList = node.Attributes;
                        //XmlNodeList attributeList = doc.SelectNodes("//<blocked-process-report>/blocking-process/process");
                        if (attributeList != null)
                        {
                            if (attributeList.Count >= 1)
                            {
                                if (attributeList["xactid"] != null)
                                {
                                    _xActID = long.Parse(attributeList["xactid"].Value);
                                }

                                if (attributeList["clientapp"] != null)
                                {
                                    _clientApp = attributeList["clientapp"].Value;
                                }

                                if (attributeList["hostname"] != null)
                                {
                                    _host = attributeList["hostname"].Value;
                                }

                                if (attributeList["lastbatchstarted"] != null)
                                {
                                    _blockingLastBatch = DateTime.Parse(attributeList["lastbatchstarted"].Value, null,
                                                                        DateTimeStyles.RoundtripKind);
                                }

                                if (attributeList["loginname"] != null)
                                {
                                    _login = attributeList["loginname"].Value;
                                }

                                if (attributeList["spid"] != null)
                                {
                                    _spid = int.Parse(attributeList["spid"].Value);
                                }
                                //_transactionStartTime = DateTime.Parse(attributeList["lastbatchstarted"].Value, null, DateTimeStyles.RoundtripKind);

                                var ibnode = node.SelectSingleNode("inputbuf");
                                _inputBuffer = (ibnode != null)? ibnode.InnerText:"";

                                try
                                {
                                    _dbName = attributeList["databaseName"].Value;
                                }
                                catch(NullReferenceException nullRef)
                                {
                                    LOG.Debug("GetBlockingXActAttributes: databaseName has not been successfully injected. " + nullRef.Message);
                                }
                            }
                        }
                    }
                    //InsertDatabaseNamesInternal(attributeList, databaseNames);
                    //attributeList = doc.SelectNodes("//@currentdb");
                    //InsertDatabaseNamesInternal(attributeList, databaseNames);
                }
               // xdlString = doc.InnerXml;
            }
            catch (Exception e)
            {
                LOG.Error("Unable to get blocking session xact id for Blocking Session Info.", e);
            }
        }

        private void InsertDatabaseNames(Dictionary<long, string> databaseNames)
        {
            Logger LOG = Logger.GetLogger("BlockingSessionInfo");
            try
            {
                var doc = new XmlDocument {PreserveWhitespace = true};

                var readerSettings = new XmlReaderSettings {CheckCharacters = false};

                using (XmlReader reader = XmlReader.Create(new StringReader(xdlString), readerSettings))
                {
                    string _byteOrderMarkUtf8 = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetPreamble());
                    if (xdlString.StartsWith(_byteOrderMarkUtf8))
                    {
                        var lastIndexOfUtf8 = _byteOrderMarkUtf8.Length - 1;
                        xdlString = xdlString.Remove(0, lastIndexOfUtf8);
                    }
                    doc.LoadXml(xdlString);

                    XmlNodeList attributeList = doc.SelectNodes("//@dbid");
                    InsertDatabaseNamesInternal(attributeList, databaseNames);
                    attributeList = doc.SelectNodes("//@currentdb");
                    InsertDatabaseNamesInternal(attributeList, databaseNames);
                }
                xdlString = doc.InnerXml;
            }
            catch (Exception e)
            {
                LOG.Error("Unable to resolve database name for Blocking Session Info. the blocking xml is: " + xdlString, e);
            }
        }

        private void InsertDatabaseNamesInternal(XmlNodeList attributeList, Dictionary<long, string> databaseNames)
        {
            foreach (XmlAttribute attr in attributeList)
            {
                XmlElement element = attr.OwnerElement;
                long databaseId = 0;
                long.TryParse(attr.Value, out databaseId);
                if (databaseId > 0 &&
                    databaseNames.ContainsKey(databaseId))
                {
                    element.SetAttribute("databaseName", databaseNames[databaseId]);
                }
            }
        }

        //public object[] GetAlertMessageFields()
        //{
        //    object[] result = new object[] { "", 0, "", "", "", "", "", "", "" };

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(new StringReader(xdlString));
        //    if (doc.SelectNodes("//deadlock-list/deadlock").Count > 1)
        //    {
        //        result[6] = "There were multiple victims of this deadlock.  ";
        //        result[7] = "first ";
        //    }
        //    else
        //    {
        //        result[6] = "";
        //        result[7] = "";
        //    }
        //    string deadlockVictim = doc.SelectSingleNode("//deadlock-list/deadlock").Attributes["victim"].Value;
        //    XmlNode victimNode = doc.SelectSingleNode("//process[@id='" + deadlockVictim + "']");
        //    if (victimNode != null)
        //    {
        //        int spidNum = 0;
        //        string spid = null;
        //        try
        //        {
        //            spid = victimNode.Attributes["spid"].Value;
        //        }
        //        catch (Exception e)
        //        {
        //            spidNum = -1;
        //            Logger LOG = Logger.GetLogger("DeadlockInfo");
        //            LOG.ErrorFormat("GetAlertMessageFields Failed {0}, spid not found for deadlock. Process {1}", e.Message, deadlockVictim);
        //        }

        //        result[2] = "unknown"; // appname 
        //        result[3] = "unknown"; // user
        //        result[4] = "unknown"; // host
        //        result[5] = "unknown"; // last command
        //        result[8] = ""; // SPID unknown message

        //        if (spid != null)
        //        {
        //            spidNum = Convert.ToInt32(spid.Trim(new char[] { '*', 's' }));
        //            result[1] = spidNum; //spid
        //        }
        //        if (spidNum > 49)
        //        {

        //            try
        //            {
        //                result[2] = victimNode.Attributes["clientapp"].Value; //appname
        //            }
        //            catch (Exception)
        //            {
        //                // Leave as "unknown"
        //            }

        //            try
        //            {
        //                result[3] = victimNode.Attributes["loginname"].Value; // user
        //            }
        //            catch (Exception)
        //            {
        //                // Leave as "unknown"
        //            }

        //            try
        //            {
        //                result[4] = victimNode.Attributes["hostname"].Value; //host
        //            }
        //            catch (Exception)
        //            {
        //                // Leave as "unknown"
        //            }
        //        }
        //        else if (spidNum < 0)
        //        {
        //            result[1] = "unknown"; // SPID
        //            result[2] = "unknown"; // appname 
        //            result[3] = "unknown"; // user
        //            result[4] = "unknown"; // host
        //            result[5] = "unknown"; // last command
        //            result[8] = "SPID not found. O/S thread may no longer be active. "; // SPID unknown message
        //        }
        //        else
        //        {
        //            result[2] = "System"; //appname
        //            result[4] = "System"; //host
        //        }
        //        result[5] = victimNode.SelectSingleNode("//inputbuf").InnerText; //last command
        //    }


        //    return result;
        //}

        //public List<object[]> GetInsertData()
        //{
        //    List<object[]> returnData = new List<object[]>();
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(new StringReader(xdlString));

        //    foreach (XmlNode node in doc.SelectNodes("//process"))
        //    {
        //        if (node != null)
        //        {
        //            object[] result = new object[] { "", 0, "", "", "", "", "", "" };
        //            string spid = null;
        //            int spidNum = 0;
        //            try
        //            {
        //                spid = node.Attributes["spid"].Value;
        //            }
        //            catch (Exception e)
        //            {
        //                spidNum = -1;
        //                Logger LOG = Logger.GetLogger("DeadlockInfo");
        //                LOG.Error("GetInsertData Failed, spid not found for deadlock.", e);
        //            }

        //            result[2] = ""; // appname 
        //            result[3] = ""; // user
        //            result[4] = ""; // host
        //            result[5] = ""; // last command

        //            if (spid != null)
        //            {
        //                spidNum = Convert.ToInt32(spid.Trim(new char[] { '*', 's' }));
        //                result[1] = spidNum; //spid
        //            }
        //            if (spidNum > 49)
        //            {
        //                try
        //                {
        //                    result[0] = node.Attributes["databaseName"].Value; //databaseName
        //                }
        //                catch (Exception)
        //                {
        //                    // Leave as null
        //                }

        //                try
        //                {
        //                    result[2] = node.Attributes["clientapp"].Value; //appname
        //                }
        //                catch (Exception)
        //                {
        //                    // Leave as null
        //                }

        //                try
        //                {
        //                    result[3] = node.Attributes["loginname"].Value; // user
        //                }
        //                catch (Exception)
        //                {
        //                    // Leave as null
        //                }

        //                try
        //                {
        //                    result[4] = node.Attributes["hostname"].Value; //host
        //                }
        //                catch (Exception)
        //                {
        //                    // Leave as null
        //                }
        //            }
        //            else
        //            {
        //                result[2] = "System"; //appname
        //                result[4] = "System"; //host
        //            }
        //            result[5] = node.SelectSingleNode("//inputbuf").InnerText; //last command
        //            returnData.Add(result);
        //        }
        //    }

        //    return returnData;
        //}

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public bool Equals(BlockingSessionInfo other)
        {
            if (other == null) return false;
            if (this.StartTime == null) return false;
            if (other.XActId != this.XActId) return false;
            //if (other.TransactionStartTime.Year != TransactionStartTime.Year ||
            //    other.TransactionStartTime.DayOfYear != TransactionStartTime.DayOfYear ||
            //    other.TransactionStartTime.Hour != TransactionStartTime.Hour ||
            //    other.TransactionStartTime.Minute != TransactionStartTime.Minute ||
            //    other.TransactionStartTime.Second != TransactionStartTime.Second)
            //{
            //    return false;
            //}
            if (other.BlockedSession.Equals(BlockedSession)) return false;
            return true;
        }

        public TimeSpan BlockingTime
        {
            get { return new TimeSpan(0,0,0,0,(int)_blockedSession.BlockingTimems); }
        }

        public DateTime BlockingStartTimeUtc
        {
            get { return _blockedSession.TransactionStartTime.ToUniversalTime(); }
        }
        public DateTime BlockingEndTimeUtc
        {
            get { return _endTime.ToUniversalTime(); }
        }
    }
}
