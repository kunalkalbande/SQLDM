//------------------------------------------------------------------------------
// <copyright file="ServiceStateThreshold.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Thresholds
{
    using System;
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common.Snapshots;
    
    /// <summary>
    /// Threshold for determining ServiceState value status.
    /// </summary>
    [Serializable]
    public class ServiceStateThreshold : BaseThreshold
    {
        #region fields

        #endregion

        #region constructors

        public ServiceStateThreshold()
        {
        }

        protected ServiceStateThreshold(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStateThreshold"/> class.
        /// </summary>
        /// <param name="states">The states that cause a warning or critical alert if enabled.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public ServiceStateThreshold(IEnumerable<ServiceState> states, bool enabled)
            : base(states, enabled)
        {
            IComparable icomp = ServiceState.Running;
            ServiceState state1 = ServiceState.Running;
            ServiceState state2 = ServiceState.Paused;
            if (state2 < state1)
            {
                

            }
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Checks the specified value against the threshold
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// true if the target value is in the list of states for this threshold.
        /// </returns>
        public override bool IsInViolation(object target)
        {
            if (!(target is ServiceState))
                throw new InvalidOperationException("value passed is not a ServiceState");

            ServiceState targetState = (ServiceState)target;
            IEnumerable<ServiceState> value = Value as IEnumerable<ServiceState>;
            if (value == null)
            {
                foreach (ServiceState state in value)
                {
                    if (targetState == state)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
     
}
