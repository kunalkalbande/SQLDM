using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class FileActivityForInstance
    {
        [DataMember]
        public string Drive { get; set; }
        
        [DataMember]
        public string DatebaseName  { get; set; }
        
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string FileType { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataMember]
        public Statistics statistics  { get; set; }
        
        [DataContract]
        public class Statistics
        {
            [DataMember]
            public long DiskReadsPerSec { get; set; }

            [DataMember]
            public long DiskWritesPerSec { get; set; }

            [DataMember]
            public double FileReadsPerSec { get; set; }

            [DataMember]
            public double FileWritesPerSec { get; set; }            
        }
    }
}
