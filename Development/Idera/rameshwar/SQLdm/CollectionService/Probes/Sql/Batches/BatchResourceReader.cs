using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Configuration;

namespace Idera.SQLdm.CollectionService.Probes.Sql.Batches
{
    class BatchResourceReader
    {
        private const string AssemblyPath = "Idera.SQLdm.CollectionService.Probes.Sql.Batches.";
        private const string SwapBatchVersion = "11.1";
        private const string SwapBatchHeader = "-- SQLdm";
        private static readonly Assembly resourceAssembly;

        static BatchResourceReader()
        {
            resourceAssembly = Idera.SQLdm.CollectionService.Helpers.EncryptedAssemblyLoader.Load("Idera.SQLdm.CollectionService.", "SQLdmCollectionServiceResources.dll");
        }

        public BatchResourceReader()
		{
			
		}

		internal string GetBatch(string batchResourceName)
		{
		    return GetBatch(batchResourceName, false);
		}

        internal string GetBatch(string batchResourceName, bool suppressCopyright)
        {
            
            // Assembly assem = this.GetType().Assembly;


            string dataPath = Path.Combine(CollectionServiceConfiguration.DataPath, "Batches");
            dataPath = Path.Combine(dataPath, batchResourceName);
            if (File.Exists(dataPath))
            {
                FileInfo file = new FileInfo(dataPath);
                string header = "";
                string batch = "";
                string version = "";
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    header = reader.ReadLine();
                    if (header.StartsWith(SwapBatchHeader))
                    {
                        Regex regex = new Regex(@"(?<=-- SQLdm\s*)[\d\.+]+");
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
                                Logger.GetLogger("BatchResourceReader").Warn("Alternate batch " + dataPath +
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
                    string salt = "Kfuljsv9YcbSf9FouKTfpEqLGyqxfYdYdKW0y6ceeETcS5O5LwITeieBI5AoLcjpqbnYHDZaitxEWapZC6G6A9w9RnwCB4qwlpoL";

                    byte[] hash = Idera.SQLdm.Common.Security.Encryption.Cipher.GetSHAHash(batch + salt + version);

                    string batchHash = Convert.ToBase64String(hash).Trim();
                    string fileHash = header.Substring(header.IndexOf("Hashvalue:") > 0 ? header.IndexOf("Hashvalue:") + 10 : 0).Trim();

                    if (batchHash == fileHash)
                    {
                        Regex regex = new Regex(@"(?<=-- SQLdm\s*[\d\.+]+\s+).*(?=Hash)");
                        Match commentMatch = regex.Match(header);
                        string comment = commentMatch.Success ? commentMatch.ToString().Trim() : "";
                       
                        Logger.GetLogger("BatchResourceReader").Warn("Using alternate batch " + dataPath + " for batch creation." 
                            + (comment.Length > 0 ? "  Comment: " + comment : ""));

                        if (suppressCopyright)
                        {
                            return BatchConstants.SwapBatchNotice + (comment.Length > 0 ? "-- Comment: " + comment + "\r\n" : "") + batch;
                        }
                        else
                        {
                            return BatchConstants.SwapBatchNotice + (comment.Length > 0 ? "-- Comment: " + comment + "\r\n": "") + BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + batch;
                        }
                    }
                    else
                    {
                        Logger.GetLogger("BatchResourceReader").Warn("Alternate batch " + dataPath + " has an incorrect checksum.  Proceeding to use built-in batch.");
                    }
                }
                else
                {
                    Logger.GetLogger("BatchResourceReader").Warn("Alternate batch " + dataPath +
                                                                 " has an incorrect header format.  Proceeding to use built-in batch.");
                }
            }
            

            batchResourceName = AssemblyPath + batchResourceName;

            using (Stream stream = resourceAssembly.GetManifestResourceStream(batchResourceName))
            {
                if (stream == null)
                {
                    //If you're coming here because you are getting this exception, you probably need to change the build 
                    //action of your resource file to Embedded Resource under Properties.  
                    throw new Exception("The stream resource is null for batch " + batchResourceName);
                }
                try      
				{
                    
					using( StreamReader reader = new StreamReader(stream) )         
					{
                        if (suppressCopyright)
                        {
                            return reader.ReadToEnd();
                        }
                        else
                        {
                            return BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + reader.ReadToEnd();
                        }
					}
				}     
				catch(Exception e)      
				{
                    throw new Exception("Unable to read batch " + batchResourceName + "\r\n" + e.ToString());      
				}
			}
		}
    }
}