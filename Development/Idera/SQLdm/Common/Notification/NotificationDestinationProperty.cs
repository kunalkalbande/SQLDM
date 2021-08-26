//------------------------------------------------------------------------------
// <copyright file="NotificationDestinationProperty.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;

    /// <summary>
    /// A property entry for a notification destination.
    /// </summary>
    [Serializable]
    public class NotificationDestinationProperty : ObjectProperty
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationDestinationProperty"/> class.
        /// </summary>
        public NotificationDestinationProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationDestinationProperty"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public NotificationDestinationProperty(ObjectProperty property)
            : base(property.Name, property.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationDestinationProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public NotificationDestinationProperty(string name, object value)
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
