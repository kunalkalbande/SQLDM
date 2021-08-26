using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using BBS.TracerX;
using Idera.SQLdoctor.AnalysisEngine.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Batches
{
    static class BatchResourceReader
    {
        private static readonly string SwapBatchVersion = "1.1";
        private static readonly string SwapBatchHeader = "-- SQLdoctor";
        private static Logger _logX = Logger.GetLogger("BatchResourceReader");
        private static List<string> _batchNames = new List<string>();

        static BatchResourceReader()
        {
            using (_logX.DebugCall("BatchResourceReader()"))
            {
                Type oType = typeof(BatchResources);
                Type sType = typeof(string);
                if (null == oType) return;
                PropertyInfo[] props = oType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (null == props) return;
                foreach (PropertyInfo propInfo in props)
                {
                    if (null == propInfo) continue;
                    try
                    {
                        if (propInfo.CanRead) 
                        {
                            if (propInfo.PropertyType == sType)
                            {
                                _batchNames.Add(propInfo.Name);
                                _logX.Debug("Batch: " + propInfo.Name);
                            }
                        }
                    }
                    catch (Exception ex) { Idera.SQLdm.PrescriptiveAnalyzer.Common.ExceptionLogger.Log(_logX, "BatchResourceReader()", ex); }
                }
                _batchNames.Sort(StringComparer.InvariantCultureIgnoreCase);
            }
        }

        internal static bool BatchExists(string batchResourceName)
        {
            return (!string.IsNullOrEmpty(GetNormalizedBatchName(batchResourceName)));
        }

        internal static string GetNormalizedBatchName(string batchResourceName)
        {
            int n = _batchNames.BinarySearch(batchResourceName, StringComparer.InvariantCultureIgnoreCase);
            if (n >= 0) return (_batchNames[n]);
            return (string.Empty);
        }

		internal static string GetBatch(string batchResourceName)
		{
		    return GetBatch(batchResourceName, false);
		}

        internal static List<BatchNameValue> GetBatchNameValue()
        {
            List<BatchNameValue> bnv = new List<BatchNameValue>(_batchNames.Count);
            foreach (string n in _batchNames)
            {
                if (string.IsNullOrEmpty(n)) continue;
                bnv.Add(new BatchNameValue() { Name = n, Value = BatchResources.ResourceManager.GetString(n) });
            }
            return (bnv);
        }

        internal static string GetBatch(string batchResourceName, bool suppressCopyright)
        {
            using (_logX.DebugCall(string.Format("BatchResourceReader.GetBatch({0}, {1})", batchResourceName, suppressCopyright)))
            {
                Assembly assem = typeof(BatchResourceReader).Assembly;
                string dataPath = Path.Combine(Path.Combine(Path.GetDirectoryName(assem.Location), "Batches"), batchResourceName);
                string batch = "";
                bool fileExists = File.Exists(dataPath);
                if (!fileExists) { dataPath += ".sql"; fileExists = File.Exists(dataPath); }
                if (fileExists)
                {
                    FileInfo file = new FileInfo(dataPath);
                    string header = "";
                    string version = "";
                    using (StreamReader reader = new StreamReader(file.OpenRead()))
                    {
                        header = reader.ReadLine();
                        if (header.StartsWith(SwapBatchHeader))
                        {
                            Regex regex = new Regex(string.Format(@"(?<={0}\s*)[\d\.+]+", SwapBatchHeader));
                            Match versionMatch = regex.Match(header);

                            if ((versionMatch.Success))
                            {
                                version = versionMatch.ToString();

                                if (version.Trim() == SwapBatchVersion.Trim())
                                {
                                    batch = reader.ReadToEnd();
                                }
                                else
                                {
                                    _logX.Warn("Alternate batch " + dataPath +
                                                                                 " is version " + version +
                                                                                 " is not valid for version " +
                                                                                 SwapBatchVersion +
                                                                                 ".  Proceeding to use built-in batch.");
                                }
                            }
                        }
                    }
                    if (batch.Length > 0)
                    {
                        string salt = "FtjdHsdDeUjNDjcKBW6NAoi4Xlj7NeRyrfmiI3plpR4SD19NoND5jTR2nce3KIKE9ySttNjuR7y0gLNgNQyAVNR90diu7GtJ2d4x";

                        byte[] bytes = UnicodeEncoding.Unicode.GetBytes(batch + salt + version);
                        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                        {
                            byte[] hash = sha1.ComputeHash(bytes);
                            string batchHash = Convert.ToBase64String(hash).Trim();
                            string fileHash = header.Substring(header.IndexOf("Hashvalue:") > 0 ? header.IndexOf("Hashvalue:") + 10 : 0).Trim();

                            if (batchHash == fileHash)
                            {
                                Regex regex = new Regex(string.Format(@"(?<={0}\s*[\d\.+]+\s+).*(?=Hash)", SwapBatchHeader));
                                Match commentMatch = regex.Match(header);
                                string comment = commentMatch.Success ? commentMatch.ToString().Trim() : "";

                                _logX.Warn("Using alternate batch " + dataPath + " for batch creation."
                                    + (comment.Length > 0 ? "  Comment: " + comment : ""));

                                if (suppressCopyright)
                                {
                                    return BatchConstants.SwapBatchNotice + (comment.Length > 0 ? "-- Comment: " + comment + "\r\n" : "") + batch;
                                }
                                else
                                {
                                    return BatchConstants.SwapBatchNotice + (comment.Length > 0 ? "-- Comment: " + comment + "\r\n" : "") + BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + batch;
                                }
                            }
                            else
                            {
                                _logX.Warn("Alternate batch " + dataPath + " has an incorrect checksum.  Proceeding to use built-in batch.");
                            }
                        }
                    }
                    else
                    {
                        _logX.Warn("Alternate batch " + dataPath + " has an incorrect header format.  Proceeding to use built-in batch.");
                    }
                }

                batch = BatchResources.ResourceManager.GetString(batchResourceName);

                if (suppressCopyright)
                {
                    return batch;
                }
                else
                {
                    return BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + batch;
                }
            }
        }
    }
}
