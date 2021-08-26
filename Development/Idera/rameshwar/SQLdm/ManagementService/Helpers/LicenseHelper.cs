using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BBS.License;
using Idera.SQLdm.ManagementService.Auditing;
using Idera.SQLdm.ManagementService.Auditing.Actions;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.ManagementService.Configuration;
using System.Data.SqlTypes;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Messages;
using System.Data;
using System.Diagnostics;
using Idera.SQLdm.Common;
using Microsoft.Win32;

namespace Idera.SQLdm.ManagementService.Helpers {
    internal static class LicenseHelper {

        private static LicenseSummary _currentLicense;
        private static DateTime _lastChecked = DateTime.MinValue;
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("LicenseHelper");
        private static object _lock = new object();

        /// <summary>
        /// Returns the cached license status determined by the last call to CheckLicense(true).
        /// If unknown (e.g. on the first reference), calls
        /// CheckLicense() and returns the result.
        /// </summary>
        public static LicenseSummary CurrentLicense {
            get {
                lock (_lock) {
                    if (_currentLicense == null) {
                        _currentLicense = CheckLicense(true);
                    }

                    return _currentLicense;
                }
            }
        }

        /// <summary>
        /// Returns the time when the license was last checked and enforced.
        /// </summary>
        public static DateTime LastChecked { 
            get { return _lastChecked; } 
        }

        /// <summary>
        /// Applies changes to license keys, calls CheckLicense and returns the new key list.
        /// </summary>
        public static LicenseSummary SetLicenseKeys(LicenseKeyOperation operation, IEnumerable<string> keyList) {
            using (LOG.DebugCall()) {
                LOG.Debug("operation = ", operation);
                using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection()) {
                    connection.Open();
                    lock (_lock) {
                        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try {
                            switch (operation) {
                                case LicenseKeyOperation.Add:
                                    LicenseHelper.AddLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Remove:
                                    LicenseHelper.RemoveLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Replace:
                                    LicenseHelper.ReplaceLicenseKeys(transaction, keyList);
                                    break;
                            }

                            #region Change Log

                            MAuditingEngine.Instance.LogAction(new LicenseAction());

                            #endregion Change Log

                            LicenseSummary newLicense = GetLicenseSummary(transaction, connection, null);
                            
                            transaction.Commit();
                            CheckLicense(true, newLicense);

                            if (!newLicense.IsTrial)
                            {
                                //We need to mark the desktop client machine as well as the management service machine
                                RegistryKey rk = null;
                                RegistryKey rks = null;

                                rk = Registry.LocalMachine;
                                rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                                rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                                if (rks != null)
                                    rks.Close();
                                rks = null;

                                if (rk != null)
                                    rk.Close();
                                rk = null;
                            }

                            return newLicense;
                        } catch (ServiceException e) {
                            LOG.Error("ServiceException caught in SetLicenseKeys: ", e);
                            transaction.Rollback();
                            throw; 
                        } catch (Exception e) {
                            LOG.Error("Exception caught in SetLicenseKeys: ", e);
                            transaction.Rollback();
                            throw new ServiceException(e, Status.ErrorUnknown);
                        }
                    }
                }
            }
        }

        public static void CheckLicenseIfIntervalExpired(TimeSpan interval) {
            if (DateTime.Now - LastChecked > interval) {
                LOG.DebugFormat("Checking license since the last check was at {0} and the interval {1} expired.", LastChecked, interval);
                try
                {
                    CheckLicense(true);
                } catch (Exception e)
                {
                    LOG.Error("Daily license check failed:", e);
                }
            }
        }

        /// <summary>
        /// CheckLicense reads the license keys from the database and summarizes them.
        /// It also refreshes CurrentLicense;
        /// It will create a trial license if no keys are found.
        /// If the enforce parameter is true, it will also call PauseCollection or ResumeCollection.
        /// </summary>
        public static LicenseSummary CheckLicense(bool enforce) {
            using (LOG.DebugCall()) {
                lock (_lock) {
                    LicenseSummary license;

                    using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection()) {
                        connection.Open();

                        // Load the keys from the repository.
                        license = GetLicenseSummary(null, connection, null);
                    }

                    CheckLicense(enforce, license);
                    return license;
                } // lock
            } // using logger
        }


        /// <summary>
        /// SQL DM 9.0 (Vineet Kumar) (License Changes) -- Added this method for replacing existing old keys with the new key provided during upgrade process.
        /// </summary>
        /// <param name="enforce"></param>
        /// <param name="installInfo"></param>
        /// <returns></returns>
        public static LicenseSummary CheckLicense(bool enforce, InstallInfo installInfo)
        {
            using (LOG.DebugCall())
            {
                try
                {
                    LicenseSummary license;
                    List<string> keyList = new List<string>();
                    List<string> newKeyList = new List<string>();
                    int registeredServers = -1;
                    string instance = null;
                    if (installInfo != null)
                    {
                        if (!string.IsNullOrEmpty(installInfo.LicenseKey))
                        {
                            LOG.Info("New license key(s) found in installinfo file : " + installInfo.LicenseKey.Trim());
                            string[] delimiter = new string[] { "," };
                            string[] newLicenseKeys = installInfo.LicenseKey.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string key in newLicenseKeys)
                            {
                                newKeyList.Add(key);
                            }
                        }
                        else 
                        {
                            LOG.Info("No new license key found in installinfo file");
                        }
                    }
                    lock (_lock)
                    {
                        using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
                        {
                            connection.Open();
                            if (newKeyList.Count > 0)
                            {
                                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GetLicenseKeysAndListener"))
                                    {
                                        using (SqlDataReader reader = command.ExecuteReader())
                                        {
                                            int fieldId = reader.GetOrdinal("LicenseKey");
                                            while (reader.Read())
                                            {
                                                string key = reader.GetString(fieldId);
                                                keyList.Add(key);
                                            }
                                        }
                                        SqlParameter rsc = command.Parameters["@ReturnServerCount"];
                                        SqlInt32 sqlValue = (SqlInt32)rsc.SqlValue;
                                        if (!sqlValue.IsNull)
                                            registeredServers = sqlValue.Value;

                                        SqlParameter repository = command.Parameters["@ReturnInstanceName"];
                                        SqlString strValue = (SqlString)repository.SqlValue;
                                        instance = strValue.Value;

                                    }
                                
                                //Summarise existing keys
                                license = LicenseSummary.SummarizeKeys(
                              registeredServers,
                              instance,
                              keyList);

                                bool newKeyRequired = true;
                                foreach (var key in license.CheckedKeys)
                                {
                                    if (key.KeyObject != null)
                                    {
                                        var keyObj = key.KeyObject;
                                        if (!keyObj.IsTrial && keyObj.ProductVersion.Major < 9)
                                        {
                                            LOG.Info("Deleting old version (<9.0) license keys : " + key.KeyString + " on management service start");
                                            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_DeleteLicenseKey"))
                                            {
                                                SqlHelper.AssignParameterValues(command.Parameters, key.KeyString);
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                        if (!keyObj.IsTrial && keyObj.ProductVersion.Major >= 9)
                                        {
                                            newKeyRequired = false;
                                        }
                                    }
                                }

                                if (newKeyList.Count > 0 && newKeyRequired)
                                {
                                    LOG.Info("Registering 9.0 license key(s) obtained from installer while upgrading SQLdm.");
                                    LicenseHelper.AddKeysUnchecked(null, connection, newKeyList);

                                }

                            }
                            // Load the keys from the repository.
                            license = GetLicenseSummary(null, connection, null);
                        }

                        CheckLicense(enforce, license);
                        return license;
                    } // lock
                }
                catch (Exception ex)
                {
                    LOG.Error("Exception Occurred while updating 9.0 key obtaioned from installer while upgrading SQLdm : " + ex.Message + ":" + ex.StackTrace);
                    throw ex;
                }
            } // using logger
        }


        private static void CheckLicense(bool enforce, LicenseSummary license) 
        {
            using (LOG.DebugCall()) 
            {
                LOG.Debug("enforce = ", enforce);
               _currentLicense = license;

                if (_currentLicense.Status == LicenseStatus.OK) 
                {
                    if (enforce) Management.CollectionServices.ResumeAllCollection();

                    // Log an event if the license is near expiration.
                    TimeSpan remaining = _currentLicense.Expiration - DateTime.Now;
                    LOG.Debug("Remaining time = ", remaining);
                    
                    if (remaining < TimeSpan.FromDays(10)) 
                    {
                        Management.WriteEvent((int)EventLogEntryType.Warning, Status.LicenseExpirationWarningId, Category.General, _currentLicense.ToString());
                    }

                    if (!_currentLicense.IsTrial)
                    {
                        //We need to mark the desktop client machine as well as the management service machine
                        RegistryKey rk = null;
                        RegistryKey rks = null;

                        rk = Registry.LocalMachine;
                        rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                        rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                        if (rks != null)
                            rks.Close();
                        rks = null;

                        if (rk != null)
                            rk.Close();
                        rk = null;
                    }
                } 
                else 
                {
                    Management.WriteEvent((int)EventLogEntryType.Error, Status.ErrorLicenseViolationId, Category.General, _currentLicense.ToString());
                    if (enforce) Management.CollectionServices.PauseAllCollection(false);
                }

                if (enforce) _lastChecked = DateTime.Now;
            }
        } // CheckLicense
            
        // Summarizes the list of keys.  
        // If keyList is null, this gets the keys from the repository and may create the trial key.
        // If keyList.Count == 0, this gets the keys from the repository, may create the trial key, and adds them to keyList.
        // if keyList.Count != 0, this summarizes the specified keys.
        private static LicenseSummary GetLicenseSummary(SqlTransaction transaction, SqlConnection connection, List<string> keyList) {
            using (LOG.DebugCall()) {
                int registeredServers = -1;
                string instance = null;
                bool maybeCreateTrial = true;

                if (keyList == null) {
                    LOG.Debug("keyList is null, will get keys from repository.");
                    keyList = new List<string>();
                } else {
                    LOG.Debug("keyList.Count = ", keyList.Count);
                }

                try {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GetLicenseKeysAndListener"))
                    {
                        command.Transaction = transaction;
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            if (keyList.Count == 0) {
                                // Summarize the keys returned by th stored proc.
                                // If none were returned, we'll create a trial key a little later.
                                int fieldId = reader.GetOrdinal("LicenseKey");
                                while (reader.Read()) {
                                    string key = reader.GetString(fieldId);
                                    keyList.Add(key);
                                }
                            } else {
                                // The user passed in the keys to summarize.
                                // Don't create a trial even if the list is empty.
                                maybeCreateTrial = false;
                            }
                        } // using reader

                        SqlParameter rsc = command.Parameters["@ReturnServerCount"];
                        SqlInt32 sqlValue = (SqlInt32)rsc.SqlValue;
                        if (!sqlValue.IsNull)
                            registeredServers = sqlValue.Value;

                        SqlParameter repository = command.Parameters["@ReturnInstanceName"];
                        SqlString strValue = (SqlString)repository.SqlValue;
                        instance = strValue.Value;

                        if (maybeCreateTrial && keyList.Count == 0) {
                            // No licenses in the repository.  Create a trial.
                            keyList.Add(CreateTrial(transaction, connection, instance));
                        }

                        LicenseSummary license = LicenseSummary.SummarizeKeys(
                            registeredServers,
                            instance,
                            keyList);

                        LOG.Info("Returning license: ", license);
                        return license;
                    } // using command
                } catch (Exception exception) {
                    LOG.Fatal(exception);
                    throw new ServiceException(exception, Status.ErrorUnknown);
                }
            }
        }

        // Initialize the trial license key,
        // insert it into the database as the only key, and
        // return the trial key.
        private static string CreateTrial(SqlTransaction transaction, SqlConnection connection, string instance) {
            using (LOG.DebugCall()) {
                // We need the repository's creation date to determine the trial's expiration date.
                DateTime dbCreation = transaction != null ?
                    (DateTime)SqlHelper.ExecuteScalar(transaction, CommandType.Text, "select dbo.fn_GetDatabaseCreationDate()") :
                    (DateTime)SqlHelper.ExecuteScalar(connection, CommandType.Text, "select dbo.fn_GetDatabaseCreationDate()");

                BBSLic lic = new BBSLic();

                lic.ExpirationDate = dbCreation.AddDays(14); // Could already be expired.
                lic.IsTrial = true;
                lic.Limit1 = 15;
                lic.ProductID = Constants.ProductId;
                lic.ProductVersion = new Version(9, 0);//SQL DM 9.0 (Vineet Kumar) (License Changes)-Incremented the version no for trial license key generation.
                lic.SetScopeHash(instance);

                string trialKey = lic.GetKeyString(LicensePW());
                string[] keyList = new string[] { trialKey };
                AddKeysUnchecked(transaction, connection, keyList);
                return trialKey;
            }
        }

        private static void ReplaceLicenseKeys(SqlTransaction transaction, IEnumerable<string> keyList) {
            using (LOG.DebugCall()) {
                if (keyList == null)
                    throw new ServiceException("The list of keys is null.");

                // We really need the keys in a List<string>.
                List<string> newKeys = keyList as List<string>;
                if (newKeys == null) newKeys = new List<string>(keyList);

                if (newKeys.Count == 0)
                    throw new ServiceException("The list of keys is empty.");

                LicenseSummary summary = GetLicenseSummary(transaction, transaction.Connection, newKeys);

                if (summary.BadKey != null) {
                    // All new keys must be completely valid.
                    throw new ServiceException("Can't add key '" + summary.BadKey.KeyString + "'.  " + summary.BadKey.Comment);
                } else {
                    switch (summary.Status) {
                        case LicenseStatus.OK:
                            // delete all the license keys
                            SqlHelper.ExecuteNonQuery(transaction, "p_DeleteLicenseKey", DBNull.Value);
                            // add all keys in the newKeys to the database
                            AddKeysUnchecked(transaction, transaction.Connection, newKeys);
                            break;
                        case LicenseStatus.CountExceeded:
                            throw new ServiceException("The number of currently monitored servers is greater than the number allowed by the specified key(s).");
                        default:
                            // Other conditions should be impossible due to earlier check of summary.BadKey.
                            throw new ServiceException("Internal error in license check.");
                    }
                }
            }
        }

        // Remove the specified keys only if the resulting set of keys doesn't
        // cause problems we don't already have.
        private static void RemoveLicenseKeys(SqlTransaction transaction, IEnumerable<string> removeKeys) {
            using (LOG.DebugCall()) {
                List<string> currentKeys = new List<string>();
                LicenseSummary summary = GetLicenseSummary(transaction, transaction.Connection, currentKeys);

                // If we already have a violation, skip checking.
                if (summary.Status == LicenseStatus.OK) {
                    LOG.Debug("No violation with current keys.");
                    // There is currently no violation.
                    // See if removing the specified keys will cause a violation.
                    // First determine what the new list will be after removing
                    // the specified keys.  We use exact string matching here
                    // because the stored proc that deletes keys also works that way.
                    foreach (string toRemove in removeKeys) currentKeys.Remove(toRemove);

                    summary = LicenseSummary.SummarizeKeys(
                        summary.MonitoredServers,
                        summary.Repository,
                        currentKeys);

                    switch (summary.Status) {
                        case LicenseStatus.CountExceeded:
                            throw new ServiceException("Removing the specified key(s) would make the number of licensed servers less than the number of monitored servers.");
                        case LicenseStatus.Expired:
                            throw new ServiceException("Removing the specified key(s) would result in an expired license.");
                        case LicenseStatus.NoValidKeys:
                            throw new ServiceException("Removing the specified key(s) would leave no valid keys.");
                    }
                }

                // If we get this far, remove the keys.
                using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, "p_DeleteLicenseKey")) {
                    command.Transaction = transaction;
                    foreach (string key in removeKeys) {
                        SqlHelper.AssignParameterValues(command.Parameters, key);
                        command.ExecuteNonQuery();
                        // don't really care about returned id
                    }
                }
            }
        }

        // Try to add the specified keys to the table.
        // Caller locks the lock.
        private static void AddLicenseKeys(SqlTransaction transaction, IEnumerable<string> addList) {
            using (LOG.DebugCall()) {
                // See what errors exist before and after adding the new keys.
                // We will add the new keys if doing so doesn't add a new error.

                List<string> keys = new List<string>();
                LicenseSummary summary1 = GetLicenseSummary(transaction, transaction.Connection, keys);

                keys.AddRange(addList);
                LicenseSummary summary2 = LicenseSummary.SummarizeKeys(
                    summary1.MonitoredServers,
                    summary1.Repository,
                    keys);
                
                // See if we found a different bad key string.
                if (summary2.BadKey != null && 
                    (summary1.BadKey == null || !object.ReferenceEquals(summary1.BadKey.KeyString, summary2.BadKey.KeyString))) //
                {
                    throw new ServiceException("Can't add key '" + summary2.BadKey.KeyString + "'.  " + summary2.BadKey.Comment);
                } else {
                    AddKeysUnchecked(transaction, transaction.Connection, addList);
                }
            }
        }
        
        // Adds keys to the table without any checks.
        // Pass null for the transaction if you don't have one.
        private static void AddKeysUnchecked(SqlTransaction transaction, SqlConnection connection, IEnumerable<string> keyList) {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddLicenseKey")) {
                command.Transaction = transaction;
                foreach (string key in keyList) {
                    if (key != null) {
                        SqlHelper.AssignParameterValues(command.Parameters, key, DBNull.Value);
                        command.ExecuteNonQuery();
                        // don't really care about returned id
                    }
                }
            }
        }

        private static byte[] LicensePW() {
            Process currentProcess = Process.GetCurrentProcess();
            string data = currentProcess.MachineName + currentProcess.Id;
            return BBSLic.GetHash(data);
        }
    }

}
