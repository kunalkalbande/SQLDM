using System;
using System.Management;

namespace Idera.SQLdm.Common.Services
{
    [Serializable]
    public class WmiException : Exception
    {
        public ManagementStatus ErrorCode { get; set; }

        public WmiException(ManagementException exception) : this(exception.ErrorCode, exception.ErrorInformation)
        {
        }

        public WmiException(ManagementStatus errorCode, ManagementBaseObject statusObject)
        {
            ErrorCode = ManagementStatus.Failed;
            if (statusObject == null) return;
            try
            {
                foreach (var property in statusObject.Properties)
                {
                    Data.Add(property.Name, property.Value);
                }
            } catch (Exception e)
            {
                /* */
            }
        }

    }
}
