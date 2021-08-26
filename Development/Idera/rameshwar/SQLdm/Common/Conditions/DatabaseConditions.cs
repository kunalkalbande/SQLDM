//------------------------------------------------------------------------------
// <copyright file="DatabaseConditions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
using System.Runtime.Serialization;

    /// <summary>
    /// Navigation Condition that matches any database
    /// </summary>
    [Serializable]
    public class DatabaseAnyCondition : BaseNavigationCondition
    {
        public DatabaseAnyCondition(ICondition subCondition)
            : base("//database", subCondition)
        {
        }

        private DatabaseAnyCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

    /// <summary>
    /// Navigation Condition that matches a database by name
    /// </summary>
    [Serializable]
    public class DatabaseNameCondition : BaseNavigationCondition
    {
        public DatabaseNameCondition(string name, ICondition subCondition)
            : base(subCondition)
        {
            Path = String.Format("//database[@name='{0}']", name);
        }

        private DatabaseNameCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
