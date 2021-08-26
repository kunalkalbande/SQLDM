//------------------------------------------------------------------------------
// <copyright file="RemotingConfig.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Configuration
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Remoting.Lifetime;

    /// <summary>
    /// Conigure the remoting interfaces and register well-known types
    /// </summary>
    public class RemotingConfig
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RemotingConfig");
        private const string ServerChannelName = "tcp-server";

        private int port = 5166;

        static RemotingConfig()
        {
            ManagementServiceConfiguration.OnServicePortChanged += new ManagementServiceConfiguration.ConfigValueChangedDelegate<int>(ServicePortChanged);

            // following can only be set once
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            LifetimeServices.LeaseTime = TimeSpan.FromMinutes(5);
            LifetimeServices.SponsorshipTimeout = TimeSpan.FromMinutes(1);
            LifetimeServices.RenewOnCallTime = TimeSpan.FromMinutes(2);
            LifetimeServices.LeaseManagerPollTime = TimeSpan.FromSeconds(15);
        }

        public void Reconfigure()
        {
            using (LOG.InfoCall("Reconfigure"))
            {
                LOG.Debug("Reconfiguring server channel...");
                TcpServerChannel serverChannel = ChannelServices.GetChannel(ServerChannelName) as TcpServerChannel;
                if (serverChannel != null)
                {
                    try
                    {
                        ChannelServices.UnregisterChannel(serverChannel);
                        LOG.Debug("Server channel unregistered...");
                    }
                    catch (Exception ex)
                    {
                        LOG.DebugFormat("Error stopping current server channel: {0}", ex.Message);
                    }
                }
                try
                {
                    ConfigureServerChannel();
                }
                catch (Exception ex)
                {
                    LOG.DebugFormat("Error starting new server channel: {0}", ex.Message);
                }
                // register the server channel
                foreach (IChannel channel in ChannelServices.RegisteredChannels)
                {
                    LOG.Info("Registered channel: " + channel.ChannelName);
                }
            }
        }

        private void ConfigureServerChannel()
        {
            using (LOG.InfoCall("ConfigureServerChannel"))
            {
                // register a server channel
                int servicePort = ManagementServiceConfiguration.ServicePort;
                IDictionary properties = new System.Collections.Specialized.ListDictionary();
                properties["name"] = ServerChannelName;
                properties["port"] = servicePort.ToString();
//                properties["impersonationLevel"] = "None";
//                properties["impersonate"] = false;
                properties["secure"] = false;

                // serialization provider that supports full serialization
                BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
                serverSinkProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                // 
                TcpServerChannel tcpServerChannel = new TcpServerChannel(properties, serverSinkProvider);
                ChannelServices.RegisterChannel(tcpServerChannel, false);
                LOG.Info("Management service listening on port: " + servicePort);
            }
        }

        public void Configure()
        {
            using (LOG.InfoCall( "Configure"))
            {
                // read the port number from the database

                // register well known types
                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(typeof(ConfigurationService), "Configuration", WellKnownObjectMode.SingleCall);

                ConfigureServerChannel();

                // register a client channel
                IDictionary properties = new System.Collections.Specialized.ListDictionary();
                properties = new System.Collections.Specialized.ListDictionary();
                properties["name"] = "tcp-client";
                properties["impersonationLevel"] = "None";
                properties["impersonate"] = false;
                properties["secure"] = false;

                BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();

                TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
                ChannelServices.RegisterChannel(tcpClientChannel, false);

                // register the server channel
                foreach (IChannel channel in ChannelServices.RegisteredChannels)
                {
                    LOG.Info("Registered channel: " + channel.ChannelName);
                }
                LOG.Info("Management service listening on port: " + port);
            }
        }

        public static void ServicePortChanged(int oldPort, int newPort)
        {
            using (LOG.InfoCall("ServicePortChanged"))
            {
                LOG.DebugFormat("Service port changed old={0} new={1}", oldPort, newPort);
                new RemotingConfig().Reconfigure();
            }
        }

    }
}
