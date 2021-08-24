using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer_form_application
{
    public class SampleProductException : Exception
    {
        public virtual string ErrorCode { get; set; }
        public virtual string ErrorMessage { get; set; }
        public SampleProductException()
            : base()
        {
        }
        public SampleProductException(string message, string code) : base(message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
        public SampleProductException(string message, Exception inner, string code)
            : base(message, inner)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
    }

    public class VersionMismatchException : SampleProductException
    {
        public override string ErrorCode
        {
            get
            {
                return "Version_MisMatch";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Sample Product is already installed on the same Idera Dashboard.";
            }
        }
    }

    public class InstanceExistsException : SampleProductException
    {
        public override string ErrorCode
        {
            get
            {
                return "DisplayNameExist";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The display name you entered already exists.  Please enter a different display name for Idera Dashboard Sample Product installation.";
            }
        }
    }
}
