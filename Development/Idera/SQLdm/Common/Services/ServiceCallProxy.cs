//------------------------------------------------------------------------------
// <copyright file="ManagementServiceWrapper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Serialization;

    public class ServiceCallProxy : RealProxy
    {
        private const string ERROR_NO_SERVICE = "Unable to connect to the {0}. Please confirm that the service is running and the configured communication ports (5166 and 5167) are not blocked.  ";
        private const string NEW_ERROR_NO_SERVICE = "Unable to connect to the {0}.\r\nTo trouble shoot:\r\n1.)Check to make sure the {0} is running.\r\n2.)Confirm that the configured communcation ports (5166 and 5167) are open in the firewall.\r\n3.)Finally, run the Management Service Configuration Wizard to confirm that the service is connected to the proper repository.";

        [Serializable]
        public class ServiceCallException : ApplicationException {
            protected ServiceCallException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public ServiceCallException(string message) : base(message) { }
            public ServiceCallException(string message, Exception inner) : base(message, inner) { }
        }

        private IMessageSink sinkChain;
        private string objectURI;

        public ServiceCallProxy(Type type, string url) : base(type)
        {
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                if (channel is IChannelSender)
                {
                    sinkChain = ((IChannelSender)channel).CreateMessageSink(url, null, out objectURI);
                    if (sinkChain != null)
                        break;
                }
            }
            if (sinkChain == null)
                throw new Exception("No channel found capable of connecting to " + url);
        }

        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg)
        {
            msg.Properties["__Uri"] = objectURI;
            try
            {
                IMessage result = sinkChain.SyncProcessMessage(msg);
                if (result is IMethodReturnMessage && ((IMethodReturnMessage)result).Exception != null)
                {
                    IMethodReturnMessage returnMessage = (IMethodReturnMessage)result;
                    Exception exception = EnhanceException(returnMessage.Exception);
                    if (exception != returnMessage.Exception)
                        result = new ReturnMessage(exception, (IMethodCallMessage)msg);
                }
                return result;
            } 
            catch (Exception e)
            {
                throw EnhanceException(e);
            }
        }

        private Exception EnhanceException(Exception source)
        {
            if (source is ManagementServiceException)
                return EnhanceManagementServiceException((ManagementServiceException)source);
            if (source is CollectionServiceException)
                return EnhanceCollectionServiceException((CollectionServiceException)source);
            if (source is RemotingException)
                return EnhanceRemotingException((RemotingException)source);
            if (source is Win32Exception)
                return EnhanceWin32Exception((Win32Exception)source);

            return source;
        }

        private string GetServiceName()
        {
            object intrface = GetTransparentProxy();

            if (intrface is IManagementService || 
                intrface is IManagementService2 || 
                intrface is IManagementServiceConfiguration) 
                    return "SQLdm Management Service";

            if (intrface is ICollectionService ||
                intrface is ICollectionServiceConfiguration)
                    return "SQLdm Collection Service";

            return "SQLdm service";
        }

        private Exception EnhanceWin32Exception(Win32Exception win32Exception)
        {
            Exception result = win32Exception;
            switch (win32Exception.NativeErrorCode)
            {
                case 10061: // Socket connection failure
                    result =
                        new ServiceCallException(
                            String.Format(
                                NEW_ERROR_NO_SERVICE,
                                GetServiceName()));
                    break;
            }
            return result;
        }

        private Exception EnhanceRemotingException(RemotingException remotingException)
        {
            Exception result = remotingException;
    
            result =
                    new ServiceCallException(
                        String.Format(
                            NEW_ERROR_NO_SERVICE,
                            GetServiceName()), 
                            remotingException);

            return result;
        }

        private Exception EnhanceCollectionServiceException(CollectionServiceException collectionServiceException)
        {
            return collectionServiceException;
        }

        private Exception EnhanceManagementServiceException(ManagementServiceException managementServiceException)
        {
            return managementServiceException;
        }

    }

}
