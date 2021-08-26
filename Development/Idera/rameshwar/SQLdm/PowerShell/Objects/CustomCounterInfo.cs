//------------------------------------------------------------------------------
// <copyright file="CustomCounterInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using Idera.SQLdm.Common.Events;

    internal class CustomCounterInfo
    {
        private CustomCounterDefinition ccd;
        private MetricDefinition metricDef;
        private MetricDescription metricDesc;

        private string name;

        internal CustomCounterInfo(CustomCounterDefinition ccd, MetricDefinition def, MetricDescription desc) 
        {
            this.ccd = ccd;
            this.metricDef = def;
            this.metricDesc = desc;
        }

        public string Name
        {
            get { return metricDesc.Name; }
        }
        
        public string Category
        {
            get { return metricDesc.Category; }
        }
        
        public string Description
        {
            get { return metricDesc.Description; }
        }

        public string Comments
        {
            get { return metricDesc.Comments; }
        }

        public MetricType CounterType
        {
            get { return ccd.MetricType; }
        }

        public bool IsEnabled
        {
            get { return ccd.IsEnabled; }
        }

        public string Object
        {
            get { return ccd.ObjectName; }
        }
        
        public string Counter
        {
            get { return ccd.CounterName; }
        }
        
        public string Instnace
        {
            get { return ccd.InstanceName; }
        }

        public string Sql
        {
            get { return ccd.SqlStatement; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
