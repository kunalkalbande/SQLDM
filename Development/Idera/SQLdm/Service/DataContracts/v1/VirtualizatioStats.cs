using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Virtualization Stats

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 13/12/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class VirtualizationStats
    {
        [DataMember]
        public decimal VMDiskRead { get; set; }
        [DataMember]
        public decimal VMDiskWrite { get; set; }
        [DataMember]
        public decimal ESXDiskRead { get; set; }
        [DataMember]
        public decimal ESXDiskWrite { get; set; }
        [DataMember]
        public decimal VMAvailableByte { get; set; }
        [DataMember]
        public decimal ESXAvailableMemBytes { get; set; }

        [DataMember]
        public decimal VMMemGrantedMB { get; set; }
        [DataMember]
        public decimal VMMemBaloonedMB { get; set; }
        [DataMember]
        public decimal VMMemActiveMB { get; set; }
        [DataMember]
        public decimal VMMemConsumedMB { get; set; }

        [DataMember]
        public decimal ESXMemGrantedMB { get; set; }
        [DataMember]
        public decimal ESXMemBaloonedMB { get; set; }
        [DataMember]
        public decimal ESXMemActiveMB { get; set; }
        [DataMember]
        public decimal ESXMemConsumedMB { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }
    }
}
