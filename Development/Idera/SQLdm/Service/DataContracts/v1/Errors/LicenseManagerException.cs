﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.DataContracts.v1.Errors
{
    internal class LicenseManagerException : ApplicationException
    {
        private string Error { get; set; }
        public bool IsAuthenticationFailed;
        public override string Message
        {
            get
            {
                return Error;
            }
        }

        public LicenseManagerException(string error)
        {
            Error = error;
        }

        public LicenseManagerException(string error, bool authError)
        {
            Error = error;
            IsAuthenticationFailed = authError;
        }
    }
}
