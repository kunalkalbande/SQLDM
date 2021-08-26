using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
//-------------------------------------------------------------------

// 

// Copyright (c) Idera, Inc. All rights reserved.

// class added to give structure to encapsulate Consolidated overview

// Author : SQLdm 10.2 (Nishant Adhikari)

// Created on : 17/11/2016

// 
//--------------------------------------------------------------------
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ConsolidatedInstanceOverview
    {
        [DataMember]
        public PreviousAnalysisInformation PreviousAnalysisInformation { get; set; }

        [DataMember]
        public IList<RecommendationSummary> RecommendationSummary { get; set; }
        public ConsolidatedInstanceOverview()
        {
            RecommendationSummary = new List<RecommendationSummary>();
        }
    }
}
