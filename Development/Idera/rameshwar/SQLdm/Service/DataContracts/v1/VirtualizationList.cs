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
    public class VirtualizationList
    {
        [DataMember]
        public String type { get; set; }
        [DataMember]
        public IList<VirtualizationStats> VirtualizationStats { get; set; }
        public VirtualizationList()
        {
            VirtualizationStats = new List<VirtualizationStats>();
        }
    }
}
