using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class QueryStatisticsForInstance
    {        
        [DataMember]
        public int QueryId{get;set;}
       [DataMember]
        public string QueryName{get;set;}
       [DataMember]
        public string SqlText{get;set;}
       [DataMember]
        public int Occurences{get;set;}
       [DataMember]
        public string EventType{get;set;} 
       [DataMember]
        public string ApplicationName{get;set;}
       [DataMember]
        public string DatabaseName{get;set;}
       [DataMember]
        public string User {get;set;}
       [DataMember]
        public string client {get;set;} 
        [DataMember]
        public double AverageDuration;
        [DataMember]
        public double Cpuaverage;
        [DataMember]
        public double Cputotal;        
        [DataMember]
        public double AverageReads;
        [DataMember]
        public double TotalReads;
        [DataMember]
        public double AverageWrites;
        [DataMember]
        public long TotalWrites;
        [DataMember]
        public DateTime StartTime;
        [DataMember]
        public double AvgCPUPerSecond;
        [DataMember]
        public string QueryNum { get; set; }
        [DataMember]
        public long SignatureId { get; set; }
        [DataMember]
        public int SpId { get; set; }

    }
}
