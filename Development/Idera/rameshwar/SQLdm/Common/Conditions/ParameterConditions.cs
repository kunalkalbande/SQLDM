//------------------------------------------------------------------------------
// <copyright file="ParameterConditions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.ComponentModel;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Navigation Condition that matches any parameter
    /// </summary>
    [Serializable]
    public class ParameterAnyCondition : BaseNavigationCondition
    {
        public ParameterAnyCondition(ICondition subCondition)
            : base("//parameter", subCondition)
        {
        }

        private ParameterAnyCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

    /// <summary>
    /// Navigation Condition that matches a parameter by name
    /// </summary>
    [Serializable]
    public class ParameterNameCondition : BaseNavigationCondition
    {
        public ParameterNameCondition(string name, ICondition subCondition)
            : base(subCondition)
        {
            Path = String.Format("//parameter[@name='{0}']", name);
        }

        private ParameterNameCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
