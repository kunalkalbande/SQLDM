//------------------------------------------------------------------------------
// <copyright file="ProbeReadyEventArgs.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    using System;

    using Idera.SQLdm.Common.Services;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    class CollectorCompleteEventArgs : EventArgs
    {
        #region fields

        private object value;
        private Result result;
        private Exception exception;
        private long? elapsedMilliseconds = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectorCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CollectorCompleteEventArgs(object value, Result result)
        {
            Value = value;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectorCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="elapsedMilliseconds">The elapsed time of the collector</param>
        public CollectorCompleteEventArgs(object value, long elapsedMilliseconds, Result result)
        {
            Value = value;
            Result = result;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectorCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="exception">The exception.</param>
        public CollectorCompleteEventArgs(object value, Exception exception)
        {
            Value = value;
            Result = Result.Failure;
            Exception = exception;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return this.value; }
            private set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public Result Result
        {
            get { return result; }
            private set { result = value; }
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return exception; }
            internal set { exception = value; }
        }

        /// <summary>
        /// Gets or sets the runtime of the collector
        /// </summary>
        public long? ElapsedMilliseconds
        {
            get { return elapsedMilliseconds; }
            private set { elapsedMilliseconds = value; }
        }

        public string Database { get; set; }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
