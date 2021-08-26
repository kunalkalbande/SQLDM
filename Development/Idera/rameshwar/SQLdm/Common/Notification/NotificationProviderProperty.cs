//------------------------------------------------------------------------------
// <copyright file="NotificationProviderProperty.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;

    /// <summary>
    /// A property entry for a notification provider.
    /// </summary>
    [Serializable]
    public class NotificationProviderProperty : ObjectProperty
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationProviderProperty"/> class.
        /// </summary>
        public NotificationProviderProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationProviderProperty"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public NotificationProviderProperty(ObjectProperty property)
            : base(property.Name, property.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationProviderProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public NotificationProviderProperty(string name, object value)
            : base(name, value)
        {
        }

        #endregion

        #region properties

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
