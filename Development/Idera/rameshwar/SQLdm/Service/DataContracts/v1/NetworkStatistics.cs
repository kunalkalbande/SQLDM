using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Network Statistics

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 25/11/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class NetworkStatistics
    {
        [DataMember]
        public DateTime UTCDateTime { get; set; }

        [DataMember]
        public float PacketsSentPerSec { get; set; }

        [DataMember]
        public float PacketsRecievedPerSec { get; set; }

    }
}
