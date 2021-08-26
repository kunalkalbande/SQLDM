using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to previous Analysis Information

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 17/11/2016

// 
//--------------------------------------------------------------------

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class PreviousAnalysisInformation
    {
        [DataMember]
        public string AnalyisType { get; set; }
        [DataMember]
        public string Duration { get; set; }
        [DataMember]
        public DateTime StartedOn { get; set; }
    }
}