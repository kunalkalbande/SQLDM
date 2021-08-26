//------------------------------------------------------------------------------
// <copyright file="ProviderDisplayNameAttribute.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Information needed to display a provider to users.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProviderInfoAttribute : Attribute
    {
        #region fields

        private Type destinationType;
        private string destinationTypeLabel;

        #endregion

        #region constructors

        public ProviderInfoAttribute(Type destinationType)
        {
            DestinationType = destinationType;
        }

      

        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the type of the destination.
        /// </summary>
        /// <value>The type of the destination.</value>
        public Type DestinationType
        {
            get { return destinationType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("destinationType");
                destinationType = value;
            }
        }

        public string DestinationTypeLabel
        {
            get { 
                return (String.IsNullOrEmpty(destinationTypeLabel) ? 
                    destinationType.Name : destinationTypeLabel); 
            }
            set
            {
                destinationTypeLabel = value;
            }
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
