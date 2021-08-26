//------------------------------------------------------------------------------
// <copyright file="Counter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Counters
{
    using System;
    using System.Threading;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    public class Counter : ICounter
    {
        #region fields

        private string name;
        private long value;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Counter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Counter(string name)
            : this(name, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Counter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public Counter(string name, long value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public long Value
        {
            get
            {
                return Interlocked.Read(ref value);
            }
            private set
            {
                Interlocked.Exchange(ref this.value, value);
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}={1}", Name, Interlocked.Read(ref value));
        }

        /// <summary>
        /// Increments this instance.
        /// </summary>
        /// <returns></returns>
        public long Increment()
        {
            return Interlocked.Increment(ref value);
        }

        /// <summary>
        /// Decrements this instance.
        /// </summary>
        /// <returns></returns>
        public long Decrement()
        {
            return Interlocked.Decrement(ref value);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref value, 0);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
