using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.ImportWizard.Objects
{

    public class ImportContext
    {
        #region members

        private SqlConnectionInfo m_Repository;
        private DateTime m_ImportDate;
        private List<SQLdm5x.MonitoredSqlServer> m_Servers;

        #endregion

        #region ctors

        public ImportContext(
                SqlConnectionInfo repository,
                DateTime importDate,
                List<SQLdm5x.MonitoredSqlServer> servers
            )
        {
            Debug.Assert(servers != null && servers.Count != 0);

            m_Repository = repository;
            m_ImportDate = importDate;
            m_Servers = servers;
        }

        #endregion

        #region properties

        public SqlConnectionInfo Repository
        {
            get { return m_Repository; }
        }

        public DateTime ImportDate
        {
            get { return m_ImportDate; }
        }

        public List<SQLdm5x.MonitoredSqlServer> Servers
        {
            get { return m_Servers; }
        }

        #endregion
    }

    public class ImportStatus
    {
        #region types

        public enum ImportState { Start, InProgress, End, Error, Cancelled };

        #endregion

        #region members

        private ImportState m_State;
        private string m_Status;

        #endregion

        #region ctors

        public ImportStatus(ImportState state)
        {
            m_State = state;
            m_Status = string.Empty;
        }

        public ImportStatus(
                ImportState state,
                string status
            ) 
        {
            m_State = state;
            m_Status = status;
        }

        #endregion

        #region properties

        public ImportState State
        {
            get { return m_State; }
        }

        public string Status
        {
            get { return m_Status; }
        }

        #endregion
    }

    public class Import
    {
        #region members

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Import");

        private BackgroundWorker m_Bgw;
        private ImportContext m_ImportContext;
        private Stopwatch m_Timer;

        #endregion

        #region ctors

        public Import(
                BackgroundWorker backgroundWorker,
                ImportContext importContext
            )
        {
            Debug.Assert(backgroundWorker != null && importContext != null && importContext.Servers.Count > 0);

            m_Bgw = backgroundWorker;
            m_ImportContext = importContext;
        }

        #endregion

        #region properties

        private BackgroundWorker Bgw
        {
            get { return m_Bgw; }
        }

        private SqlConnectionInfo Repository
        {
            get { return m_ImportContext.Repository; }
        }

        private DateTime ImportDate
        {
            get { return m_ImportContext.ImportDate; }
        }

        private List<SQLdm5x.MonitoredSqlServer> Servers
        {
            get { return m_ImportContext.Servers; }
        }

        #endregion

        #region methods

        public void DoImport()
        {
            using (Log.DebugCall())
            {
                // Log import summary.
                logImportSummary();

                // We want to import from the most recent effective import date
                // for the selected servers back to the selected import date.  So
                // we start by getting the most recent effective import date.
                DateTime highestEffectiveImportDate = SQLdm5x.MonitoredSqlServer.GetHighestEffectiveImportDate(Servers);

                // Notify start of import.
                notifyStart(highestEffectiveImportDate);

                // Check if import was cancelled.
                if (isCancelled()) { return; }

                // If the highest effective import date is less then then the import
                // time stamp then no need to do any more processing.
                if (highestEffectiveImportDate < ImportDate)
                {
                    Log.Info("The import date exceeds the effective import time stamp for all servers, no import will be done");
                    notifyEnd();
                    return;
                }

                // Calculate the number of units of work for showing progress.  The total units
                // of work is number of days times number of servers.
                int totalUnitsToProcess = (highestEffectiveImportDate.Subtract(ImportDate).Days + 1) * Servers.Count;

                // Import data from the highest effective import date back to the
                // selected import date.
                DateTime currentDate = highestEffectiveImportDate;
                bool isCancelledFlag = false, isError = false;
                string errorStr = string.Empty;
                int numUnitsProcessed = 0;
                Stopwatch timer;
                DateTime errorDate;
                string serverWithWriteError = string.Empty;
                List<string> serversProcessedForTheDay = new List<string>();  // These lists are for tracking servers status for the day.
                List<string> serversNotProcessedForTheDay = new List<string>();
                while (currentDate >= ImportDate && !isCancelledFlag && !isError)
                {
                    using (Log.DebugCall("Date: " + currentDate.ToShortDateString()))
                    {
                        // Clear the lists used for tracking server status.
                        serverWithWriteError = string.Empty;
                        serversProcessedForTheDay.Clear();
                        serversNotProcessedForTheDay.Clear();

                        // Import data for each selected server.
                        foreach (SQLdm5x.MonitoredSqlServer s in Servers)
                        {
                            if (!isError && !isCancelledFlag)
                            {
                                bool isAnyReadError = false;
                                using (Log.DebugCall("Server: " + s.Instance))
                                {
                                    // Get the time stamp range for the current date from the server.
                                    // If there is no import data for this current date, then skip it.
                                    DateTime start, end;
                                    if (s.GetTimeStampRangeLocal(currentDate, out start, out end))
                                    {
                                        // Init timing stuff.
                                        long readMs = 0, writeMs = 0;

                                        // Read the 4.x statistics.
                                        timer = Stopwatch.StartNew();
                                        SQLdm4xData data = null;
                                        isAnyReadError = SQLdm4x.ReadData(s.RegKeyName, start.ToUniversalTime(), end.ToUniversalTime(), out data);
                                        timer.Stop();
                                        readMs = timer.ElapsedMilliseconds;

                                        // Write to DM 5.0 Repository.
                                        if (data != null)
                                        {
                                            timer = Stopwatch.StartNew();
                                            isError = SQLdm5x.WriteData(Repository, s.Id, data, out errorStr);
                                            timer.Stop();
                                            writeMs = timer.ElapsedMilliseconds;
                                        }
                                        else
                                        {
                                            Log.Warn("Skipping write as data was not read.");
                                        }

                                        Log.Info("Total- Read - " + readMs.ToString() + " ms, Write - " + writeMs.ToString() + " ms");
                                    }
                                    else
                                    {
                                        Log.Info("No data to import for this time period.");
                                    }

                                    // If there is any error terminate processing.
                                    // Else display progress and check for cancellation.
                                    if (isError)
                                    {
                                        Log.Error("Error was encountered, ending import");
                                        serverWithWriteError = s.Instance;
                                        errorDate = currentDate;
                                    }
                                    else
                                    {
                                        // Add server to processed list.
                                        serversProcessedForTheDay.Add(s.Instance);

                                        // Notify progress.
                                        ++numUnitsProcessed;
                                        notifyProgress(numUnitsProcessed, totalUnitsToProcess, currentDate.ToShortDateString(),
                                                s.Instance, isAnyReadError);

                                        // Check if cancellation was requested.
                                        if (isCancelled())
                                        {
                                            Log.Info("Import cancel was requested");
                                            isCancelledFlag = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Because of an erorr this server will not be
                                // processed, add to the list of servers not processed.
                                serversNotProcessedForTheDay.Add(s.Instance);
                            }
                        }
                    }

                    // Decrement by 1 day.
                    if (!isError && !isCancelledFlag)
                    {
                        currentDate = currentDate.Subtract(new TimeSpan(1, 0, 0, 0));
                    }
                }

                // If not cancelled, report end.
                if (!isCancelledFlag) 
                { 
                    notifyEnd(isError, currentDate, serverWithWriteError, errorStr, serversProcessedForTheDay, serversNotProcessedForTheDay); 
                }
            }
        }

        private void logImportSummary()
        {
            Log.Info("-------------------------------------------------");
            Log.Info("Starting import of historical data from SQLdm version " + SQLdm4x.VersionString);
            Log.Info("SQLdm 4.x computer : " + Environment.MachineName);
            Log.Info("SQLdm 5.0 repository : " + Repository.InstanceName + " (" + Repository.DatabaseName + ")");
            Log.Info("Import date : " + ImportDate.ToShortDateString());
            Log.Info("Servers : ");
            foreach (SQLdm5x.MonitoredSqlServer s in Servers)
            {
                Log.Info("      " + s.Instance + " (" + s.EffectiveImportDateLocalString + ")");
            }
            Log.Info("-------------------------------------------------");
        }

        private void notifyStart(DateTime start)
        {
            // Start the timer.
            m_Timer = Stopwatch.StartNew();

            // Construct the status string.
            string status = "Starting import of historical data from SQL diagnostic manager " + SQLdm4x.VersionString;
            status += " installed on " + Environment.MachineName;
            status += ", to the new SQL diagnostic manager Repository on " + Repository.InstanceName.ToUpper() + " (" + Repository.DatabaseName + ").";
            status += "  Data will be imported from " + start.ToShortDateString() + " back to " + ImportDate.ToShortDateString() + " for the following server(s):\n";
            foreach (SQLdm5x.MonitoredSqlServer s in Servers)
            {
                status += "            " + s.Instance + "\n";
            }
            status += "\n";

            // Report progress.
            Bgw.ReportProgress(0, new ImportStatus(ImportStatus.ImportState.Start,status));
        }

        private void notifyProgress(
                int numUnitsProcessed,
                int totalUnitsToProcess,
                string currentDate,
                string server,
                bool isAnyReadError
            )
        {
            // Calculate progress.
            double processed = numUnitsProcessed;
            double total = totalUnitsToProcess;
            double fc = (double)(processed / total) * 100.00;
            int progress = (int)fc;

            // Generate error/warning status string.
            string status = string.Empty;
            if (isAnyReadError)
            {
                status = "... all historical data for the server " + server.ToUpper() + " could not be read for " + currentDate + ".\n";
            }

            Bgw.ReportProgress(progress, new ImportStatus(ImportStatus.ImportState.InProgress, status));
        }

        private void notifyEnd()
        {
            notifyEnd(false, DateTime.MinValue, null, null, null, null);
        }

        private void notifyEnd(
                bool isError,
                DateTime errorDate,
                string serverWithError,
                string errorStr,
                List<string> serversProcessed,
                List<string> serversNotProcessed
            )
        {
            Debug.Assert(m_Timer != null);

            // Calculate elapsed time in seconds.
            m_Timer.Stop();
            long elapsed = m_Timer.ElapsedMilliseconds / 1000;

            // Construct status.
            string status = string.Empty;
            ImportStatus.ImportState state = ImportStatus.ImportState.End;
            if (isError)
            {
                state = ImportStatus.ImportState.Error;
                status = "Exception was encountered when writing " + serverWithError
                            + "data to the Repository for " + errorDate.ToShortDateString()
                            + ".  Import process has been terminated.\n\n";
                status += "Error : " + errorStr + "\n\n";
                status += "The import status for each server is shown below:\n";
                string dt = errorDate.AddDays(1).ToShortDateString();
                status += "            " + serverWithError + " - " + dt + "\n";
                foreach (string s in serversNotProcessed)
                {
                    status += "            " + s + " - " + dt + "\n";
                }
                dt = errorDate.ToShortDateString();
                foreach (string s in serversProcessed)
                {
                    status += "            " + s + " - " + dt + "\n";
                }
                status += "\nElapsed time: " + elapsed.ToString() + " seconds.";
            }
            else
            {
                status = "... completed in " + elapsed.ToString() + " seconds.";
            }

            // Report progress.
            Bgw.ReportProgress(100, new ImportStatus(state, status));
        }

        private bool isCancelled()
        {
            bool ret = Bgw.CancellationPending;
            if(ret)
            {
                // Calculate elapsed time in seconds.
                m_Timer.Stop();
                long elapsed = m_Timer.ElapsedMilliseconds / 1000;

                // Set the status string.
                string status = "... import was cancelled (" + elapsed.ToString() + " seconds)." ;

                // Report progress
                Bgw.ReportProgress(100, new ImportStatus(ImportStatus.ImportState.Cancelled, status));
            }
            return ret;
        }

        #endregion
    }
}
