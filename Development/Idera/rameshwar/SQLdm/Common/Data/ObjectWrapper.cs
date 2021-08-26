//------------------------------------------------------------------------------
// <copyright file="ObjectWrapper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Data
{
    using System;

    [Serializable]
    public class ObjectWrapper
    {
        private object value;

        public ObjectWrapper()
        {
        }
        public ObjectWrapper(object value)
        {
            this.value = value;
        }
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
