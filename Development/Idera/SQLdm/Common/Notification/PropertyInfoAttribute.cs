//------------------------------------------------------------------------------
// <copyright file="DetinationPropertyInfoAttribute.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;

    /// <summary>
    /// Information needed for prompting a user for a property's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyInfoAttribute : Attribute
    {
        #region fields

        private string description;
        private object defaultValue;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DestinationPropertyInfoAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="defaultValue">The default value.</param>
        public PropertyInfoAttribute(string description, object defaultValue)
        {
            Description = description;
            DefaultValue = defaultValue;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return description; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("description");
                description = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

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
