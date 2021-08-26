//------------------------------------------------------------------------------
// <copyright file="ServerConditions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Navigation Condition that matches any server
    /// </summary>
    [Serializable]
    public class ServerAnyCondition : BaseNavigationCondition
    {
        public ServerAnyCondition(ICondition subCondition)
            : base("//server", subCondition)
        {
        }

        protected ServerAnyCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

    /// <summary>
    /// Navigation Condition that matches a server by name
    /// </summary>
    [Serializable]
    public class ServerNameCondition : BaseNavigationCondition
    {
        public ServerNameCondition(string name, ICondition subCondition)
            : base(subCondition)
        {
            Path = String.Format("//server[@name='{0}']", name);
        }

        private ServerNameCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
