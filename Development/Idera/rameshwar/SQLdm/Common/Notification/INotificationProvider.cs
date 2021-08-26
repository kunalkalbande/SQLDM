//------------------------------------------------------------------------------
// <copyright file="INotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System.Diagnostics;

    /// <summary>
    /// Interface implemented by notification providers.
    /// </summary>
    public interface INotificationProvider
    {
        NotificationProviderInfo NotificationProviderInfo { get; set; }

        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        bool Send(NotificationContext context);

        void SetEventLog(EventLog eventLog);
    }

    /// <summary>
    /// Implemented by notification providers that support handling multiple
    /// nutifications in a single call.
    /// </summary>
    public interface IBulkNotificationProvider : INotificationProvider
    {
        int Send(NotificationContext[] notifications);
    }
}