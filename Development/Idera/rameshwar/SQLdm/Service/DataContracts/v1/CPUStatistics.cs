using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate CPU Statistics

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 24/11/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class CPUStatistics
    {
        [DataMember]
        public DateTime UTCDateTime { get; set; }

        [DataMember]
        public float SQLCompilationsPerSec { get; set; }

        [DataMember]
        public float SQLRecompilationsPerSec { get; set; }

        [DataMember]
        public float BatchesPerSec { get; set; }

        [DataMember]
        public float TransactionsPerSec { get; set; }
    }
}
