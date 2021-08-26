//------------------------------------------------------------------------------
// <copyright file="IXmlWritable.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Serialization
{
    using System;
using System.Xml;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    public interface IXmlWritable
    {
        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        bool WriteTo(XmlWriter wr);
        
        #endregion
    }
}
