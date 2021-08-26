//------------------------------------------------------------------------------
// <copyright file="ManagementServiceElement.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Configuration element that specifies the location of the management service
    /// that this collection service will connect to.
    /// </summary>
    public class ManagementServiceElement : ConfigurationElement
    {
        #region fields

        private const string UrlKey = "url";

        #endregion

        #region constructors

        public ManagementServiceElement()
        {
        }

        public ManagementServiceElement(string url)
        {
            Url = url;
        }

        #endregion

        #region properties

        [ConfigurationProperty(UrlKey,
           DefaultValue = "tcp://localhost:5166/")]
        public string Url
        {
            get { return (string)this[UrlKey]; }
            set { this[UrlKey] = value; }
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
