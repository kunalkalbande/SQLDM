//------------------------------------------------------------------------------
// <copyright file="ManagementServiceException.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;
    using Idera.SQLdm.Common.Messages;

    [Serializable]
    public class ManagementServiceException : ServiceException
    {
        protected ManagementServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ManagementServiceException(string message)
            : base(message)
        {
        }

        public ManagementServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class CollectionServiceException : ServiceException
    {
        protected CollectionServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        public CollectionServiceException(string message)
            : base(message)
        {
        }

        public CollectionServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown when a request craps out.
    /// </summary>
    [Serializable]
    public class ServiceException : ApplicationException
    {
        private static int UNKNOWN_EXCEPTION = -2146232832;
        private static MessageDll MESSAGES = new MessageDll();

        private string[] messageData = new string[0];

        public ServiceException() : base() 
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ServiceException(Messages.Status status) : base("")
        {
            unchecked
            {
                HResult = (int) status;
            }
        }

        public ServiceException(uint hResult) : base("")
        {
            unchecked
            {
                HResult = (int) hResult;
            }
        }

        public ServiceException(Messages.Status status, params string[] vars)
            : base("")
        {
            unchecked
            {
                HResult = (int) status;
            }
            messageData = vars;
        }

        public ServiceException(uint hResult, params string[] vars)
            : base("")
        {
            unchecked
            {
                HResult = (int) hResult;
            }
            messageData = vars;
        }

        public ServiceException(Exception innerException, Messages.Status error)
            : base("", innerException)
        {
            unchecked
            {
                HResult = (int) error;
            }
        }

        public ServiceException(Exception innerException, uint hResult)
            : base("", innerException)
        {
            unchecked
            {
                HResult = (int) hResult;
            }
        }

        public ServiceException(Exception innerException, Messages.Status error, params string[] vars)
            : base("", innerException)
        {
            unchecked
            {
                HResult = (int) error;
            }
            messageData = vars;
        }

        public ServiceException(Exception innerException, uint hResult, params string[] vars)
            : base("", innerException)
        {
            unchecked
            {
                HResult = (int) hResult;
            }
            messageData = vars;
        }

        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            messageData = (string[]) info.GetValue("messageData", typeof (string[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("messageData", messageData);
        }

        public int HRESULT
        {
            get
            {
                return HResult;
            }
        }

        public static int StatusToInt(Status status)
        {
            unchecked
            {
                return (int)status;
            }
        }

        public override string Message
        {
            get
            {
                unchecked
                {
                    if (HResult == UNKNOWN_EXCEPTION)
                        return base.Message;
                }
                try
                {
                    return MESSAGES.Format((uint)HResult, messageData);
                }
                catch (Exception)
                {
                }
                if (!String.IsNullOrEmpty(base.Message))
                    return base.Message;

                StringBuilder s = new StringBuilder();
                s.AppendFormat("Unknown error.  ({0})", HResult).AppendLine();
                foreach (string data in messageData)
                {
                    s.AppendFormat("    data={0}", data).AppendLine();
                }
                return s.ToString();
            }
        }

        public static string GetExternalizedMessage(uint messageId, string[] messageData)
        {
            try
            {
                return MESSAGES.Format(messageId, messageData);
            }
            catch (Exception)
            {
            }
            StringBuilder s = new StringBuilder();
            s.AppendFormat("Unknown error.  ({0})", messageId).AppendLine();
            foreach (string data in messageData)
            {
                s.AppendFormat("    data={0}", data).AppendLine();
            }
            return s.ToString();
        }
    }
}