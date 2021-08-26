//------------------------------------------------------------------------------
// <copyright file="IEventContainer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    public interface IEventContainer
    {
        #region properties

        IEnumerable<IEvent> Events { get; }

        int NumberOfEvents { get; }

        #endregion

        #region events

        #endregion

        #region methods

        void AddEvent(IEvent evnt);
        void AddEvent(IEnumerable<IEvent> evnts);
        
        #endregion
    }
}
