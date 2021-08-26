//------------------------------------------------------------------------------
// <copyright file="XmlSerializerFactory.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Data
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using BBS.TracerX;

    public static class XmlSerializerFactory
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("XmlSerializerFactory");

        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
        private static object sync = new object();

        public static XmlSerializer GetSerializer(Type type)
        {
            XmlSerializer serializer = null;

            lock (sync)
            {
                if (!serializers.TryGetValue(type, out serializer))
                {
                    try
                    {
                        serializer = new XmlSerializer(type);
                        serializer.UnknownElement += XmlUnknownElement;
                        serializers.Add(type, serializer);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Exception creating serializer", e);
                        throw;
                    }
                }
            }
            return serializer;
        }

        public static XmlSerializer GetSerializer(Type type, Type[] extraTypes)
        {
            XmlSerializer serializer = null;
            lock (sync)
            {
                if (!serializers.TryGetValue(type, out serializer))
                {
                    try
                    {
                        serializer = new XmlSerializer(type, extraTypes);
                        serializer.UnknownElement += XmlUnknownElement;
                        serializers.Add(type, serializer);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Exception creating serializer", e);
                        throw;
                    }
                }
            }
            return serializer;
        }

        static void XmlUnknownElement(object sender, XmlElementEventArgs e)
        {
            try
            {
                IUnknownElementHandler handler = e.ObjectBeingDeserialized as IUnknownElementHandler;
                if (handler != null)
                    handler.HandleUnknownElement(e.Element);
            }
            catch
            {
                LOG.ErrorFormat("Error handling unknown element ({0}) during deserialization of a {1} object",
                                e.Element.Name, e.ObjectBeingDeserialized.GetType().Name);
            }
        }

    }

    public interface IUnknownElementHandler
    {
        void HandleUnknownElement(XmlElement element);
    }

}
