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
    /// Represents a SQL Server blockedsession.
    /// Data comes from trace session
    /// </summary>
    [Serializable]
    public sealed class BlockedSessionInfo : IEquatable<BlockedSessionInfo>
    {
        #region fields

        private string xdlString = "";
        private long _xActID = -1;
        private string _clientApp;
        private DateTime _transactionStartTime;
        private string _host;
        private string _inputBuffer;
        private string _login;
        private int _spid;
        private long _blockingTime;
        private string _lockMode;
        private string _resource = string.Empty;

        #endregion

        #region constructors

        internal BlockedSessionInfo(string xdlString)
        {
            XdlString = xdlString;
            GetBlockedXActAttributes();
        }

        #endregion

        #region properties
        public string WaitResource
        {
            get { return _resource; }
            set { _resource = value.TrimEnd(); }
        }

        public string RequestMode
        {
            get { return _lockMode; }
        }

        public DateTime TransactionStartTime
        {
            get { return _transactionStartTime; }
        }

        public long BlockingTimems
        {
            get { return _blockingTime; }
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

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the xact id of the blocked session</returns>
        private void GetBlockedXActAttributes()
        {
            Logger LOG = Logger.GetLogger("GetBlockedXActAttributes");
            
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.CheckCharacters = false;

                using (XmlReader reader = XmlReader.Create(new StringReader(xdlString), readerSettings))
                {
                    doc.Load(reader);
                    var node = doc.SelectSingleNode("blocked-process-report/blocked-process/process");

                    if (node != null)
                    {
                        XmlAttributeCollection attributeList = node.Attributes;
                        if (attributeList != null)
                        {
                            if (attributeList.Count >= 1)
                            {
                                if (attributeList["xactid"] != null)
                                    _xActID = long.Parse(attributeList["xactid"].Value);

                                if (attributeList["clientapp"] != null)
                                    _clientApp = attributeList["clientapp"].Value;

                                if (attributeList["hostname"] != null)
                                    _host = attributeList["hostname"].Value;

                                if (attributeList["loginname"] != null)
                                    _login = attributeList["loginname"].Value;

                                if (attributeList["lockMode"] != null)
                                    _lockMode = attributeList["lockMode"].Value;

                                if (attributeList["waitresource"] != null)
                                    _resource = attributeList["waitresource"].Value;

                                if (attributeList["spid"] != null)
                                    _spid = int.Parse(attributeList["spid"].Value);

                                if (attributeList["waittime"] != null)
                                    _blockingTime = int.Parse(attributeList["waittime"].Value);

                                if (attributeList["lasttranstarted"] != null)
                                    _transactionStartTime = DateTime.Parse(attributeList["lasttranstarted"].Value, null, DateTimeStyles.RoundtripKind);
                                
                                var ibnode = node.SelectSingleNode("inputbuf");

                                _inputBuffer = (ibnode != null) ? ibnode.InnerText : "";
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
                LOG.Error("Unable to get blocked session xact id for Blocking Session Info.", e);
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public bool Equals(BlockedSessionInfo other)
        {
            if (other == null) return false;
            if (other.XActId != this.XActId) return false;
            if (other.BlockingTimems != this.BlockingTimems) return false;
            //if (other.TransactionStartTime.Year != TransactionStartTime.Year ||
            //    other.TransactionStartTime.DayOfYear != TransactionStartTime.DayOfYear ||
            //    other.TransactionStartTime.Hour != TransactionStartTime.Hour ||
            //    other.TransactionStartTime.Minute != TransactionStartTime.Minute ||
            //    other.TransactionStartTime.Second != TransactionStartTime.Second)
            //{
            //    return false;
            //}

            return true;
        }
    }
}
