using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Server Waits for DashBoard

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 9/12/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServerWaitsDashboard
    {
        [DataMember]
        public DateTime? UTCDateTime { get; set; }

        [DataMember]
        public String Category { get; set; }

        [DataMember]
        public decimal? TotalWaitMillisecondsPerSecond { get; set; }
        [DataMember]
        public decimal? ResourceWaitMillisecondsPerSecond { get; set; }
        [DataMember]
        public decimal? SignalWaitMillisecondsPerSecond { get; set; }
    }

}
