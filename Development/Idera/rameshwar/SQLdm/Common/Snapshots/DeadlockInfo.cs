//------------------------------------------------------------------------------
// <copyright file="Deadlock.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   13-Feb-2019
// Description          :   5.1.11 Deadlock Alert Message Enhancement.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BBS.TracerX;
using System.Web;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents a SQL Server deadlock
    /// </summary>
    [Serializable]
    public sealed class DeadlockInfo
    {
        #region fields

        // SQLDM-29758 - Added constant for unknown marks
        private const string UnknownMark = "unknown";
        private Guid id = Guid.NewGuid();
        private string xdlString = "";
        private DateTime? startTime = null;
        // SQLDM-10.2.2 (Varun Chopra) SQLDM-27814 Deadlock information not available although SQLdm is detecting a deadlock occurred
        private Dictionary<long, string> _databaseNames;

        #endregion

        #region constructors

        internal DeadlockInfo(string xdlString, Dictionary<long, string> databaseNames, DateTime? startTime)
        {
            //START SQLdm 9.1 (Ankit Srivastava) Activity Monitor with Extended events -- decoding the html tags &lt; &gt; to < > 
            //if (xdlString.Contains("&lt;") && xdlString.Contains("&gt;"))
            //    xdlString = HttpUtility.HtmlDecode(xdlString);
            //END SQLdm 9.1 (Ankit Srivastava) Activity Monitor with Extended events -- decoding the html tags &lt; &gt; to < > 
            XdlString = xdlString;
            StartTime = startTime;
            InsertDatabaseNames(databaseNames);
            // SQLDM-10.2.2 (Varun Chopra) SQLDM-27814 Deadlock information not available although SQLdm is detecting a deadlock occurred
            _databaseNames = databaseNames;
        }

        #endregion

        #region properties

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

        #endregion

        #region events

        #endregion

        #region methods

        public Guid GetId()
        {
            return id;
        }

        private void InsertDatabaseNames(Dictionary<long, string> databaseNames)
        {
            Logger LOG = Logger.GetLogger("DeadlockInfo");
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.CheckCharacters = false;
                using (XmlReader reader = XmlReader.Create(new StringReader(xdlString), readerSettings))
                {
                    doc.Load(reader);
                    XmlNodeList attributeList = doc.SelectNodes("//@dbid");
                    InsertDatabaseNamesInternal(attributeList, databaseNames);
                    attributeList = doc.SelectNodes("//@currentdb");
                    InsertDatabaseNamesInternal(attributeList, databaseNames);
                }
                xdlString = doc.InnerXml;
            }
            catch (Exception e)
            {
                LOG.Error("Unable to resolve database name for deadlock.", e);
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
        public object[] GetAlertEXFormatMessageFields()
        {
            object[] result = new object[] { "", 0, "", "", "", "", "", "", "", 0, "", "" };    //M1: Added new parameter for Winner SPID,Command of Winner and Database

            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xdlString));
            
            if (doc.SelectNodes("//deadlock/victim-list/victimProcess").Count > 1)
            {
                result[6] = "There were multiple victims of this deadlock.  ";
                result[7] = "first ";
            }
            else
            {
                result[6] = "";
                result[7] = "";
            }

            string deadlockVictim = null;            
            //SQLDM - 27814 - Code change to add log to understand what is deadlockVictim and handle other cases as well
            Logger LOG = Logger.GetLogger("DeadlockInfo");

            try
            {
                // SQLDM-10.2.2 (Varun Chopra) SQLDM-27814 Deadlock information not available although SQLdm is detecting a deadlock occurred
                deadlockVictim = doc.SelectSingleNode("//deadlock/victim-list/victimProcess").Attributes["id"].Value;
            }
            catch
            {
                LOG.Info("Getting fields for Intra-Query Parallel Thread Deadlock.");
                return GetIntraQueryParallelDeadlockExFields(doc);
            }

            LOG.Info("Deadlock Information Victim Id Value -", deadlockVictim);

            XmlNode victimNode = string.IsNullOrEmpty(deadlockVictim) || "unknown".Equals(deadlockVictim, StringComparison.InvariantCultureIgnoreCase) || "0".Equals(deadlockVictim, StringComparison.InvariantCultureIgnoreCase)
                ? null
                : doc.SelectSingleNode("//process-list/process[@id='" + deadlockVictim + "']");

            if (victimNode != null)
            {
                int spidNum = 0;
                string spid = null;
                try
                {
                    spid = victimNode.Attributes["spid"].Value;
                }
                catch (Exception e)
                {
                    spidNum = -1;
                    //Logger LOG = Logger.GetLogger("DeadlockInfo");
                    //LOG.ErrorFormat("GetAlertMessageFields Failed {0}, spid not found for deadlock. Process {1}", e.Message, deadlockVictim);
                }

                result[2] = "unknown"; // appname 
                result[3] = "unknown"; // user
                result[4] = "unknown"; // host
                result[5] = "unknown"; // last command
                result[8] = ""; // SPID unknown message
                // Modification Start ID: M1
                result[9] = "";                 // Winner SPID
                result[10] = "unknown";         // Command of Winner
                result[11] = "unknown";         // Database
                // Modification End ID: M1

                if (spid != null)
                {
                    spidNum = Convert.ToInt32(spid.Trim(new char[] { '*', 's' }));
                    result[1] = spidNum; //spid
                }
                if (spidNum > 49)
                {

                    try
                    {
                        result[2] = victimNode.Attributes["clientapp"].Value; //appname
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }

                    try
                    {
                        result[3] = victimNode.Attributes["loginname"].Value; // user
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }

                    try
                    {
                        result[4] = victimNode.Attributes["hostname"].Value; //host
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }
                }
                else if (spidNum < 0)
                {
                    result[1] = "unknown"; // SPID
                    result[2] = "unknown"; // appname 
                    result[3] = "unknown"; // user
                    result[4] = "unknown"; // host
                    result[5] = "unknown"; // last command
                    result[8] = "SPID not found. O/S thread may no longer be active. "; // SPID unknown message
                }
                else
                {
                    result[2] = "System"; //appname
                    result[4] = "System"; //host
                }
                result[5] = victimNode.SelectSingleNode("//inputbuf").InnerText; //last command

                //Modification Start ID: M1
                foreach (XmlNode node in doc.GetElementsByTagName("process"))
                {
                    if (deadlockVictim != node.Attributes["id"].Value)
                    {
                        if (node.Attributes["spid"] != null)
                            result[9] = node.Attributes["spid"].Value;          // Winner SPID
                        if (node.FirstChild["frame"] != null)
                            result[10] = node.FirstChild["frame"].InnerText;    // Command of Winner
                    }
                }
                result[11] = victimNode.Attributes["databaseName"].Value;   // Database
                //Modification End ID: M1

                if (spidNum == 0
                    && ("unknown".Equals((string)result[2]) || string.IsNullOrEmpty((string)result[2])) // appname case 
                    && ("unknown".Equals((string)result[3]) || string.IsNullOrEmpty((string)result[3])) // username case 
                    && ("unknown".Equals((string)result[4]) || string.IsNullOrEmpty((string)result[4])) // host case 
                    )
                {
                    result = GetIntraQueryParallelDeadlockExFields(doc);
                }
            }
            else
            {
                // SQLDM-10.2.2 (Varun Chopra) SQLDM-27814 Deadlock information not available although SQLdm is detecting a deadlock occurred
                // Get Alert Parameters for Intra Query Parallel Deadlocks
                result = GetIntraQueryParallelDeadlockExFields(doc);
            }
            return result;

        }

        /// <summary>
        /// Get Alert Parameters for Intra Query Parallel Deadlocks
        /// </summary>
        /// <param name="doc">Deadlock Report Document</param>
        /// <returns>Populated Result Parameters</returns>
        /// <remarks>
        /// SQLDM-10.2.2 (Varun Chopra) SQLDM-27814 Deadlock information not available although SQLdm is detecting a deadlock occurred
        /// </remarks>
        private object[] GetIntraQueryParallelDeadlockExFields(XmlDocument doc)
        {
            object[] result = { "", 0, "", "", "", "", "", "", "" };
            // Get List of Processes
            XmlNodeList deadlockProcessList = doc.SelectNodes("//deadlock/process-list/process");
            if (deadlockProcessList != null)
            {
                bool[] resultStatus = { false, false, false, false, false, false };
                int resultProcessed = 0;
                // Try to extract the required information from Processes and break out once completed
                foreach (XmlNode deadlockProcess in deadlockProcessList)
                {
                    // Database Name
                    if (!resultStatus[0])
                    {
                        var currentDb = GetAttributeValueFromNode(deadlockProcess, "currentdb");
                        if (!string.IsNullOrEmpty(currentDb))
                        {
                            int databaseId; //databaseName
                            if (int.TryParse(currentDb, out databaseId) &&
                                _databaseNames.ContainsKey(databaseId))
                            {
                                result[0] = _databaseNames[databaseId];
                                resultStatus[0] = true;
                                resultProcessed++;
                            }
                            else
                            {
                                result[0] = "#" + currentDb;
                            }
                        }
                    }

                    // SPID
                    if (!resultStatus[1])
                    {
                        var spidString = GetAttributeValueFromNode(deadlockProcess, "spid");
                        if (!string.IsNullOrEmpty(spidString))
                        {
                            int spid;
                            if (int.TryParse(spidString, out spid))
                            {
                                result[1] = spid;
                                resultStatus[1] = true;
                                resultProcessed++;
                            }
                        }
                    }

                    // Populate ClientApp, LoginName and HostName
                    string[] attributes = { "clientapp", "loginname", "hostname" };
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (!resultStatus[i + 2])
                        {
                            var attributeString = GetAttributeValueFromNode(deadlockProcess, attributes[i]);
                            if (!string.IsNullOrEmpty(attributeString))
                            {
                                result[i + 2] = attributeString;
                                resultStatus[i + 2] = true;
                                resultProcessed++;
                            }
                        }
                    }

                    // Populate Input Buffer
                    if (!resultStatus[5])
                    {
                        var lastCommand = deadlockProcess.SelectSingleNode("//inputbuf").InnerText; //last command
                        if (!string.IsNullOrEmpty(lastCommand))
                        {
                            result[5] = lastCommand;
                            resultStatus[5] = true;
                            resultProcessed++;
                        }
                    }

                    if (resultProcessed == resultStatus.Length)
                    {
                        break;
                    }
                }
                result[8] = "Intra-Query Parallel Thread Deadlock occured"; // Parallel Deadlocks message
            }
            return result;
        }

        /// <summary>
        /// Helps to Get the Value of an Attribute in a Node
        /// </summary>
        /// <param name="node">Deadlock Process Node</param>
        /// <param name="attribute">Attribute to get the Value</param>
        /// <returns>Value of the Attribute</returns>
        /// <remarks>
        /// SQLDM-10.3.1 - SQLDM-29376 The deadlock victim was spid 0 with application name 'unknown' by user 'unknown' on host 'unknown'
        /// </remarks>
        private string GetAttributeValueFromNode(XmlNode node, string attribute)
        {
            try
            {
                return node.Attributes[attribute].Value;
            }
            catch (Exception e)
            {
                Logger LOG = Logger.GetLogger("DeadlockInfo");
                LOG.VerboseFormat("Attribute - {0} - not found for node - {1}", attribute, node.Name);
                return UnknownMark;
            }
        }

        public object[] GetAlertMessageFields()
        {
            object[] result = new object[] { "", 0, "", "", "", "", "", "", "" };

            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xdlString));
            if (doc.SelectNodes("//deadlock-list/deadlock").Count > 1)
            {
                result[6] = "There were multiple victims of this deadlock.  ";
                result[7] = "first ";
            }
            else
            {
                result[6] = "";
                result[7] = "";
            }
            string deadlockVictim = doc.SelectSingleNode("//deadlock-list/deadlock").Attributes["victim"].Value;
            XmlNode victimNode = doc.SelectSingleNode("//process[@id='" + deadlockVictim + "']");
            if (victimNode != null)
            {
                int spidNum = 0;
                string spid = null;
                try
                {
                    spid = victimNode.Attributes["spid"].Value;
                }
                catch (Exception e)
                {
                    spidNum = -1;
                    Logger LOG = Logger.GetLogger("DeadlockInfo");
                    LOG.ErrorFormat("GetAlertMessageFields Failed {0}, spid not found for deadlock. Process {1}", e.Message, deadlockVictim);
                }                

                result[2] = "unknown"; // appname 
                result[3] = "unknown"; // user
                result[4] = "unknown"; // host
                result[5] = "unknown"; // last command
                result[8] = ""; // SPID unknown message

                if (spid != null)
                {
                    spidNum = Convert.ToInt32(spid.Trim(new char[] { '*', 's' }));
                    result[1] = spidNum; //spid
                }
                if (spidNum > 49)
                {
                   
                    try
                    {
                        result[2] = victimNode.Attributes["clientapp"].Value; //appname
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }

                    try
                    {
                        result[3] = victimNode.Attributes["loginname"].Value; // user
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }

                    try
                    {
                        result[4] = victimNode.Attributes["hostname"].Value; //host
                    }
                    catch (Exception)
                    {
                        // Leave as "unknown"
                    }
                }
                else if (spidNum < 0)
                {
                    result[1] = "unknown"; // SPID
                    result[2] = "unknown"; // appname 
                    result[3] = "unknown"; // user
                    result[4] = "unknown"; // host
                    result[5] = "unknown"; // last command
                    result[8] = "SPID not found. O/S thread may no longer be active. "; // SPID unknown message
                }
                else
                {
                    result[2] = "System"; //appname
                    result[4] = "System"; //host
                }
                result[5] = victimNode.SelectSingleNode("//inputbuf").InnerText; //last command
            }


            return result;
        }

        public List<object[]> GetInsertData()
        {
            List<object[]> returnData = new List<object[]>();
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xdlString));
            
            foreach (XmlNode node in doc.SelectNodes("//process"))
            {
                if (node != null)
                {
                    object[] result = new object[] { "", 0, "", "", "", "", "", "" };
                    string spid = null;
                    int spidNum = 0;
                    try
                    {
                        spid = node.Attributes["spid"].Value;
                    }
                    catch(Exception e)
                    {
                        spidNum = -1;
                        Logger LOG = Logger.GetLogger("DeadlockInfo");
                        LOG.Error("GetInsertData Failed, spid not found for deadlock.", e);
                    } 

                    result[2] = ""; // appname 
                    result[3] = ""; // user
                    result[4] = ""; // host
                    result[5] = ""; // last command

                    if (spid != null)
                    {
                        spidNum = Convert.ToInt32(spid.Trim(new char[] { '*', 's' }));
                        result[1] = spidNum; //spid
                    }
                    if (spidNum > 49)
                    {
                        try
                        {
                            result[0] = node.Attributes["databaseName"].Value; //databaseName
                        }
                        catch (Exception)
                        {
                            // Leave as null
                        }

                        try
                        {
                            result[2] = node.Attributes["clientapp"].Value; //appname
                        }
                        catch (Exception)
                        {
                            // Leave as null
                        }

                        try
                        {
                            result[3] = node.Attributes["loginname"].Value; // user
                        }
                        catch (Exception)
                        {
                            // Leave as null
                        }

                        try
                        {
                            result[4] = node.Attributes["hostname"].Value; //host
                        }
                        catch (Exception)
                        {
                            // Leave as null
                        }
                    }
                    else
                    {
                        result[2] = "System"; //appname
                        result[4] = "System"; //host
                    }
                    result[5] = node.SelectSingleNode("//inputbuf").InnerText; //last command
                    returnData.Add(result);
                }
            }

            return returnData;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
