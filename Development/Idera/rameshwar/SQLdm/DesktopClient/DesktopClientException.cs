using System;

namespace Idera.SQLdm.DesktopClient
{
    internal class DesktopClientException : ApplicationException
    {
        public DesktopClientException(string message) 
            : base(message)
        {
        }

        public DesktopClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
