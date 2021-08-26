using System;
using System.Collections.Generic;
using System.Text;
using BBS.License;
using BBS.TracerX;

namespace Idera.SQLdm.Common {
    /// <summary>
    /// The summarized status of all license keys.
    /// </summary>
    public enum LicenseStatus { OK, Expired, CountExceeded, NoValidKeys };

    [Serializable]
    public class LicenseSummary {
        private static readonly Logger Log = Logger.GetLogger("LicenseSummary");

        // Private ctor forces use of SummarizeKeys to create instance.
        private LicenseSummary() { }

        // Static ctor
        static LicenseSummary() {
            // Try using the TracerX RendererMap for logging LicenseSummary objects.
            RendererMap.Put(typeof(LicenseSummary), new LicenseSummaryRenderer());
        }

        public string Repository = string.Empty;
        public LicenseStatus Status = LicenseStatus.NoValidKeys;
        public bool IsTrial = true; // Allows trial key to be added if not changed.
        public bool IsPermanent = false;
        public DateTime Expiration = DateTime.MinValue;
        public int LicensedServers;  // Servers allowed by the license(s).
        public int MonitoredServers; // Count of monitored servers.
        public List<CheckedKey> CheckedKeys = new List<CheckedKey>(); // Contains results of deserializing and checking each of the input keys.
        public CheckedKey BadKey;  // Last invalid key found when summarizing/checking keys.

        /// <summary>
        /// Returns true if we can add at least one more server without violating.
        /// </summary>
        public bool IsNotFull { get { return LicensedServers == BBSLic.Unlimited || LicensedServers > MonitoredServers; } }

        /// <summary>
        /// Returns true if the license count is unlimited.
        /// </summary>
        public bool IsUnlimited { get { return LicensedServers == BBSLic.Unlimited ; } }

        public override string ToString() {
            StringBuilder bldr = new StringBuilder();

            switch (Status) {
                case LicenseStatus.OK:
                    if (IsNotFull) bldr.AppendLine("License Status: OK");
                    else bldr.AppendLine("License Status: OK, but no more servers can be added.");
                    break;
                case LicenseStatus.Expired:
                    bldr.AppendLine("License Status: Expired");
                    break;
                case LicenseStatus.CountExceeded:
                    bldr.AppendLine("License Status: The number of monitored servers exceeds the maximum allowed.");
                    break;
                case LicenseStatus.NoValidKeys:
                    bldr.AppendLine("License Status: No valid license keys were found.");
                    break;
                default:
                    throw new ApplicationException("Unexpected license status in LicenseSummary.ToString().");
            }

            if (Status != LicenseStatus.NoValidKeys) {
                bldr.AppendLine("Repository: " + Repository);

                if (IsTrial) bldr.AppendLine("License Type: Trial");
                else bldr.AppendLine("License Type: Production");

                if (IsPermanent) bldr.AppendLine("Expiration Date: Never");
                else bldr.AppendLine("Expiration Date: " + Expiration.ToShortDateString());

                bldr.AppendLine("Maximum Monitored Servers: " + LicensedServers);
                bldr.AppendLine("Currently Monitored Servers: " + MonitoredServers);
            }

            return bldr.ToString();
        }

        /// <summary>
        /// Verify that all of the specified keys are valid, including checking that each key
        /// is parseable, has the right scope, is for the right product, is not expired,
        /// was not created in the future, is not a duplicate, and is compatible with
        /// the first valid or expired key (multiple keys must all be permanent production keys).
        /// Null keys are ignored.
        /// </summary>
        public static LicenseSummary SummarizeKeys(
            int monitoredServers,     // Number of monitored servers (pass 0 if not known).
            string scope,             // The license scope the keys are required to match (repository instance name).
            IEnumerable<string> keys  // List of keys to check/summarize.
        ) {
            using (Log.DebugCall("SummarizeKeys")) {
                LicenseSummary summary = new LicenseSummary();

                summary.MonitoredServers = monitoredServers;
                summary.Repository = scope;

                if (keys == null) {
                    Log.Error("Keys list is null in SummarizeKeys.");
                } else {
                    BBSLic firstLic = null;
                    int curIndex = 0;

                    foreach (string currentKey in keys) {
                        ++curIndex;

                        if (currentKey == null || currentKey == string.Empty) {
                            Log.Debug("Skipping a null or empty key.");
                        } else {
                            CheckedKey checkedKey = new CheckedKey(currentKey);
                            BBSLic lic = new BBSLic();

                            // First check if the current key is a duplicate of another key
                            // by comparing it to all the previous keys.
                            int prevIndex = 0;
                            foreach (string prevKey in keys) {
                                ++prevIndex;
                                if (prevIndex == curIndex) {
                                    // Did not find a duplicate.
                                    break;
                                } else if (KeysAreEqual(currentKey, prevKey)) {
                                    checkedKey.Comment = "Key is a duplicate.";
                                    break;
                                }
                            } // Loop for finding duplicates.

                            if (prevIndex == curIndex) {
                                // Did not find a duplicate.
                                // Attempt to load the key and check for other problems.
                                LicErr licErr = lic.LoadKeyString(currentKey);

                                if (licErr == LicErr.OK) {
                                    if (!IsLicenseReasonable(lic)) {
                                        checkedKey.Comment = "Key is invalid.";
                                    } else if (lic.ProductID != Constants.ProductId) {
                                        checkedKey.Comment = "Key is for another product.";
                                    } else if (!lic.CheckScopeHash(scope)) {
                                        checkedKey.Comment = string.Format("Key is not for repository '{0}'.", scope);
                                    }
                                    //Start : SQL DM 9.0 (Vineet Kumar) (License Changes) - If license is lesser than 9.0 then add key comment
                                    else if (!lic.IsTrial && lic.ProductVersion.Major < 9)
                                    {
                                        checkedKey.Comment = "The license key is for an older version of SQL Diagnostic Manager.  Please visit the customer portal to acquire a new license key at " + Common.Constants.CustomerPortalLink + ".";
                                    }
                                    //End : SQL DM 9.0 (Vineet Kumar) (License Changes) - If license is lesser than 9.0 then add key comment
                                    else
                                    {
                                        // Multiple keys are allowed only if all keys
                                        // are permanent production keys.
                                        // If the first valid key is a trial or non-permanent
                                        // key, all subsequent keys are invalid.
                                        if (firstLic == null)
                                        {
                                            // This will be the key that others must be compatible with,
                                            // even if it is expired.
                                            firstLic = lic;
                                            summary.Expiration = lic.ExpirationDate;
                                            summary.IsTrial = lic.IsTrial;
                                            summary.IsPermanent = lic.IsPermanent;

                                            //if (lic.IsEnterprise) summary.Repository = "any";
                                            if (lic.IsExpired)
                                            {
                                                summary.Status = LicenseStatus.Expired;
                                                checkedKey.Comment = "Key is expired.";
                                            }
                                            else
                                            {
                                                summary.Status = LicenseStatus.OK; // Even if the key is expired.
                                                checkedKey.IsValid = true;
                                            }
                                        }
                                        else if (firstLic.IsTrial)
                                        {
                                            checkedKey.Comment = "Key is incompatible with an existing trial key.";
                                        }
                                        else if (!firstLic.IsPermanent)
                                        {
                                            checkedKey.Comment = "Key is incompatible with an existing non-permanent key.";
                                        }
                                        else if (lic.IsTrial)
                                        {
                                            checkedKey.Comment = "Key is incompatible with an existing production key.";
                                        }
                                        else if (!lic.IsPermanent)
                                        {
                                            checkedKey.Comment = "Key is incompatible with an existing permanent key.";
                                        }
                                        else
                                        {
                                            // This key and the first key are both permanent production keys.
                                            checkedKey.IsValid = true;
                                        }
                                    }
                                } else if (licErr == LicErr.FutureKey) {
                                    checkedKey.Comment = "Key has future creation date.";
                                } else {
                                    lic = null;
                                    checkedKey.Comment = "Key is not parseable";
                                    Log.Warn("Could not parse key '", currentKey, "'. Error code = ", licErr);
                                }
                            } // if not duplicate

                            if (checkedKey.IsValid) {
                                // Add in the key's limit unless we're already at Unlimited.
                                if (lic.Limit1 == BBSLic.Unlimited) {
                                    summary.LicensedServers = BBSLic.Unlimited;
                                } else if (summary.LicensedServers != BBSLic.Unlimited) {
                                    //Start : SQL DM 9.0 (Vineet Kumar) (License Changes) - Instance count should be based on only >9.0 keys for production. Or for active trial keys
                                    if ((lic.IsTrial && !lic.IsExpired) || (lic.ProductVersion.Major >= 9))
                                        summary.LicensedServers += lic.Limit1;
                                    //Start : SQL DM 9.0 (Vineet Kumar) (License Changes) - Instance count should be based on only >9.0 keys for production. Or for active trial keys
                                    
                                }
                            } else {
                                summary.BadKey = checkedKey;
                            }

                            Log.Debug("key ", curIndex, " = ", checkedKey.KeyString);
                            Log.Debug("IsValid = ", checkedKey.IsValid);
                            Log.Debug("Comment = ", checkedKey.Comment);
                            if (lic != null) Log.Debug("Description = ", lic);

                            summary.CheckedKeys.Add(checkedKey);
                        }   // if key != null
                    }  // loop over all keys
                } // Key list is not null.

                if (summary.Status == LicenseStatus.OK) {
                    if (summary.MonitoredServers > summary.LicensedServers && summary.LicensedServers != BBSLic.Unlimited) 
                        summary.Status = LicenseStatus.CountExceeded;
                }

                Log.Debug("Returning LicenseSummary: ", summary);
                return summary;
            }
        }


        /// <summary>
        /// See if two keys are equal after removing dashes, trimming
        /// off white space, and ignoring case.
        /// Returns false if either key is null.
        /// </summary>
        public static bool KeysAreEqual(string k1, string k2) {
            if (k1 == null || k2 == null) return false;

            if (k1 == k2) return true;

            k1 = k1.Replace("-", string.Empty).Trim();
            k2 = k2.Replace("-", string.Empty).Trim();
            return (string.Compare(k1, k2, true) == 0);
        }

        /// <summary>
        /// IsLicenseReasonable - Our license key checksum is not solid so
        /// you can change characters in the key and
        /// still have a valid license. This could allow
        /// a customer to bump up their license cound.
        /// However the changes always create unresonable
        /// licenses like 1000s of seats. To avoid
        /// problems of upgrading license DLL we just are
        /// putting in a reasonableness check in the products
        /// </summary>
        public static bool IsLicenseReasonable(BBSLic license)
        {
            // Trials only valid for 0-90 days
            if (license.IsTrial)
            {
                if (license.IsPermanent) return false;
                if (license.DaysToExpiration < 0) return false;
                if (license.DaysToExpiration > 90) return false;
            }
            else // Purchase license only valid for 0-400 days or unlimited
            {
                if (license.DaysToExpiration < 0) return false;
                if (license.DaysToExpiration > 400 && license.DaysToExpiration != 32767) return false;
            }

            // License only good for up to 2000 licenses
            if (license.Limit1 < -1) return false;
            if (license.Limit1 > 2000) return false;
            if (license.Limit2 < -2 || license.Limit2 > 1) return false; // some products code limit 2 as 1 instead of unlimited

            return true;
        }
        // The TracerX renderer for a LicenseSummary object.
        private class LicenseSummaryRenderer : BBS.TracerX.IObjectRenderer {
            public void RenderObject(object obj, System.IO.TextWriter writer) {
                LicenseSummary summary = obj as LicenseSummary;
                if (summary == null)
                    return;

                writer.WriteLine("Status = {0}", summary.Status);
                writer.WriteLine("Expiration = {0}", summary.Expiration);
                writer.WriteLine("IsTrial = {0}", summary.IsTrial);
                writer.WriteLine("LicensedServers = {0}", summary.LicensedServers);
                writer.WriteLine("MonitoredServers = {0}", summary.MonitoredServers);

                if (summary.CheckedKeys == null)
                    writer.WriteLine("CheckedKeys is null");
                else
                    writer.WriteLine("CheckedKeys.Count = {0}", summary.CheckedKeys.Count);

                if (summary.BadKey == null)
                    writer.WriteLine("BadKey is null");
                else {
                    if (summary.BadKey.KeyString == null)
                        writer.WriteLine("BadKey.KeyString is null");
                    else
                        writer.WriteLine("BadKey.KeyString = {0}", summary.BadKey.KeyString);
                }
            }
        }
    }

    /// <summary>
    /// Contains the result of checking a single license key.
    /// </summary>
    [Serializable]
    public class CheckedKey {
        public string KeyString;// Original key string passed to SummarizeKeys.

        // KeyObject is the esult of deserializing KeyString.  It returns
        // null if KeyString can't be deserialized.
        // We would store the KeyObject instance if BBSLic were serializable.
        public BBSLic KeyObject { 
            get {
                BBSLic temp = new BBSLic();
                LicErr licErr = temp.LoadKeyString(KeyString);
                if (licErr == LicErr.OK || licErr == LicErr.FutureKey) {
                    return temp;
                } else {
                    return null;
                }
            }
        }

        public bool IsValid;    // False if expired, etc.
        
        public string Comment;  // Reason why key is not valid.

        public CheckedKey(string key) { KeyString = key; }
    }
}
