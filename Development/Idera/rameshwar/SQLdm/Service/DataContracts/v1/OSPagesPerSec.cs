using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate OS Pages Per Second

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 22/11/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class OSPagesPerSec
    {
        [DataMember]
        public DateTime UTCDateTime { get; set; }
        [DataMember]
        public float PagesPerSec { get; set; }
    }
}
