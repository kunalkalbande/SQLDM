//------------------------------------------------------------------------------
// <copyright file="ICondition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Xml;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface that all conditions must implement.
    /// </summary>
    public interface ICondition : ISerializable
    {
        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        bool Matches(XmlElement element);

        #endregion
    }
}
