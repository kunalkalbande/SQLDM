//------------------------------------------------------------------------------
// <copyright file="ProviderDisplayNameAttribute.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProviderDisplayInfoAttribute : Attribute
    {
        #region fields

        private string name;
        private string description;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ProviderDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public ProviderDisplayInfoAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the name of the display.
        /// </summary>
        /// <value>The name of the display.</value>
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return description; }
            set { description = value; }
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
