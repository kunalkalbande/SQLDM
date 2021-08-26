//------------------------------------------------------------------------------
// <copyright file="ISnapshotSink.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Remoting;

    /// <summary>
    /// Provides access to registered remotable objects.
    /// </summary>
    public static class RemotingHelper
    {
        #region fields

        private static bool isInitialized;
        private static IDictionary<Type, WellKnownClientTypeEntry> wellKnownTypes;

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Builds the URI.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns></returns>
        public static string BuildUri(string baseUri, string relativeUri)
        {
            Uri uri = new Uri(new Uri(baseUri), relativeUri);
            return uri.ToString();
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns></returns>
        public static T GetObject<T>()
        {
            return (T)GetObject(typeof(T));
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetObject(Type type)
        {
            if (!isInitialized)
                InitTypeCache();

            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)wellKnownTypes[type];

            if (entry == null)
                throw new RemotingException("Unable to activate remotable object " + type.FullName);

            return GetObject(type, entry.ObjectUrl);
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static T GetObject<T>(string url)
        {
            return (T)GetObject(typeof(T), url);
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static object GetObject(Type type, string url)
        {
            return Activator.GetObject(type, url);
        }

        /// <summary>
        /// Inits the type cache.
        /// </summary>
        public static void InitTypeCache()
        {
            wellKnownTypes = new Dictionary<Type, WellKnownClientTypeEntry>();

            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            {
                if (entry.ObjectType == null)
                    throw new RemotingException("A configured type could not be found.");

                wellKnownTypes.Add(entry.ObjectType, entry);
            }
            isInitialized = true;
        }

        public static void RegisterWellKnownType(Type type, Uri uri)
        {
            if (wellKnownTypes == null)
                wellKnownTypes = new Dictionary<Type, WellKnownClientTypeEntry>();

            if (wellKnownTypes.ContainsKey(type))
            {
                wellKnownTypes.Remove(type);
            }
            
            WellKnownClientTypeEntry wkcte = new WellKnownClientTypeEntry(type, uri.ToString());
            wellKnownTypes.Add(type, wkcte);

            isInitialized = true;
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
