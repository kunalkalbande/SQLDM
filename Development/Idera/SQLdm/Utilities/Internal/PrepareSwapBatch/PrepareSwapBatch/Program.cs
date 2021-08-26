using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PrepareSwapBatch
{
    class Program
    {
        private static string headerFormat = "-- SQLdm {0} {1} Hashvalue: {2} \n{3}";
        private static string salt = "Kfuljsv9YcbSf9FouKTfpEqLGyqxfYdYdKW0y6ceeETcS5O5LwITeieBI5AoLcjpqbnYHDZaitxEWapZC6G6A9w9RnwCB4qwlpoL";

        private const string SwapBatchVersion = "11.1";

        private const string SwapBatchHeader = "-- SQLdm";
        
        static void Main(string[] args)
        {
            string dataPath = args[0];
            string version = args.Length > 1 ? args[1] : SwapBatchVersion;
            string comments = args.Length > 2 ? args[2] : "";
            string batchHash = "";
            string batch = "";
        
            if (File.Exists(dataPath))
            {
                FileInfo file = new FileInfo(dataPath);
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    batch = reader.ReadLine() + "\n";
                    if (batch.StartsWith(SwapBatchHeader))
                        batch = "";
                    batch += reader.ReadToEnd();

                    byte[] hash = BBS.License.BBSLic.GetHash(batch + salt + version);
                    batchHash = Convert.ToBase64String(hash).Trim();
                }

                using (StreamWriter writer = new StreamWriter(dataPath))
                {
                    batch = String.Format(headerFormat, version, comments, batchHash, batch);
                    writer.Write(batch);
                    writer.Flush();
                }
            }

        }
    }
}
