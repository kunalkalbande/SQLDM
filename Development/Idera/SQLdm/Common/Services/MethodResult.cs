//------------------------------------------------------------------------------
// <copyright file="MethodResult.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;

    public class MethodResult
    {
        private bool success;
        private Exception error;
        private object data;

        public MethodResult() : this(true, null, null)
        {
        }

        public MethodResult(Exception error) : this(false, null, error)
        {
        }

        public MethodResult(object data) : this(true, data, null)
        {
        }

        public MethodResult(bool success, object data, Exception error)
        {
            this.success = success;
            this.data = data;
            this.error = error;
        }

        public bool IsSuccess 
        {
            get { return success; }
        }

        public object Data
        {
            get { return data; }
        }

        public Exception Exception
        {
            get { return error; }
        }

    }
}
