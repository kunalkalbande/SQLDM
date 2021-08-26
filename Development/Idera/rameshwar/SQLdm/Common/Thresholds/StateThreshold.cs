using System.Collections.Generic;

namespace Idera.SQLdm.Common.Thresholds {
    using System;
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Threshold for determining state (e.g. ServiceState) value status.
    /// Type T must be bool or an enum type, thought the compiler will 
    /// not allow that constraint to be expressed.
    /// </summary>
    [Serializable]
    public class StateThreshold<T> : BaseThreshold where T:struct {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Protected constructor for Hibernate
        /// </summary>
        internal StateThreshold() {
        }

        protected StateThreshold(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateThreshold"/> class.
        /// </summary>
        /// <param name="states">The states that cause a warning or critical alert if enabled.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        //public StateThreshold(IList<T> states, bool enabled)
        //    : base(states, enabled) {
        //    if (!typeof(T).IsEnum && typeof(T) != typeof(bool)) {
        //        throw new InvalidOperationException("Type used is not a bool or enum: " + typeof(T));
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateThreshold"/> class.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="states">The states that cause a warning or critical alert if enabled.</param>
        public StateThreshold(bool enabled, params T[] states)
            : base(states, enabled) {
            if (!typeof(T).IsEnum && typeof(T) != typeof(bool)) {
                throw new InvalidOperationException("Type used is not a bool or enum: " + typeof(T));
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
        public override bool IsInViolation(object target) {
            if (!(target is T))
                throw new InvalidOperationException("value passed is not a " + typeof(T));
            
            if (target == null || Value == null) {
                return false;
            } else {
                foreach (T state in (T[])Value)
                {
                    if (state.Equals(target)) {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
     
}
