using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Lock Waits Stats

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 6/12/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class LockWaitsStatistics
    {
        [DataMember]
        public DateTime? UTCDateTime { get; set; }
        [DataMember]
        public double AllocUnit  { get; set; }
        [DataMember]
        public double Applicataion { get; set; }
        [DataMember]
        public double Database { get; set; }
        [DataMember]
        public double Extent { get; set; }
        [DataMember]
        public double File { get; set; }
        [DataMember]
        public double HoBT { get; set; }
        [DataMember]
        public double Key { get; set; }

        [DataMember]
        public double Latch { get; set; }
        [DataMember]
        public double Metadata { get; set; }
        [DataMember]
        public double Object { get; set; }
        [DataMember]
        public double Page { get; set; }
        [DataMember]
        public double RID { get; set; }
        [DataMember]
        public double Table { get; set; }
    }
}
