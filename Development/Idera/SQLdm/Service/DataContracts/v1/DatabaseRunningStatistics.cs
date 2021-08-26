using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Database Running Statistics

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 24/11/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class DatabaseRunningStatistics
    {
        [DataMember]
        public DateTime UTCDateTime { get; set; }

        [DataMember]
        public float TransactionsPerSec { get; set; }

        [DataMember]
        public float LogflushesPerSec { get; set; }

        [DataMember]
        public float NumberReadsPerSec { get; set; }

        [DataMember]
        public float NumberWritesPerSec { get; set; }

        [DataMember]
        public float IOStallMSPerSec { get; set; }

        [DataMember]
        public int DatabaseID { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }
    }
}