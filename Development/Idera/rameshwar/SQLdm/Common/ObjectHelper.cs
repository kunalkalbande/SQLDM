//------------------------------------------------------------------------------
// <copyright file="ObjectHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Idera.SQLdm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Collections;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// Helper class for getting / setting object properties by reflection.
    /// </summary>
    public class ObjectHelper
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Objects to map.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static IDictionary<string, object> ObjectToMap(object target)
        {
            Type type = target.GetType();
            Dictionary<string, object> map = new Dictionary<string, object>();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                PropertyToMap(target, prop, map);
            }

            return map;
        }

        /// <summary>
        /// Properties to map.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="map">The map.</param>
        private static void PropertyToMap(object target, PropertyInfo prop, IDictionary<string, object> map)
        {
            if (prop.GetGetMethod(true) == null)
                return;

            if (IsSimple(prop))
            {
                map[prop.Name] = prop.GetValue(target, null);
            }
            /*
            if (IsCollection(prop))
            {
                //TODO: Tallyho!
            }
            */
        }

        /// <summary>
        /// Maps to object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public static bool MapToObject(object target, IDictionary<string, object> map)
        {
            Type type = target.GetType();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                MapToProperty(target, map, prop);
            }

            return true;
        }

        /// <summary>
        /// Maps to property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="map">The map.</param>
        /// <param name="prop">The prop.</param>
        private static void MapToProperty(object target, IDictionary<string, object> map, PropertyInfo prop)
        {
            if (prop.GetSetMethod(true) == null)
                return;

            if (IsSimple(prop))
            {
                object val = null;
                if (map.TryGetValue(prop.Name, out val))
                {
                    prop.SetValue(target, val, null);
                }
                else
                {
                    object[] attributes = prop.GetCustomAttributes(typeof(ObjectPropertyInfoAttribute), true);
                    if (attributes != null && attributes.Length == 1)
                    {
                        ObjectPropertyInfoAttribute pia = attributes[0] as ObjectPropertyInfoAttribute;
                        prop.SetValue(target, pia.DefaultValue, null);
                    }                        
                }
            }
            /*
            if (IsCollection(prop))
            {
                //TODO: Tallyho!
            }
            */
        }

        /// <summary>
        /// Determines whether the specified prop is simple.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        /// 	<c>true</c> if the specified prop is simple; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSimple(PropertyInfo prop)
        {
            //TODO: YUCK... fix me please
            return true;
        }

        /// <summary>
        /// Determines whether the specified prop is collection.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        /// 	<c>true</c> if the specified prop is collection; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCollection(PropertyInfo prop)
        {
            if (prop.PropertyType.GetInterface("System.Collections.ICollection") != null ||
                prop.PropertyType.GetInterface("System.Collections.Generic.ICollection") != null)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a cloned object from source.
        /// </summary>
        /// <typeparam name="T">Type of the object that is being cloned..</typeparam>
        /// <param name="source">The object to clone.</param>
        /// <returns>A cloned object from source.</returns>
        public static T Clone<T>(T source)
        {
            T clonedObject = default(T);

            if (typeof(T).IsSerializable)
            {
                if (ReferenceEquals(source, null))
                {
                    // If is null reference.
                    clonedObject = default(T);
                }
                else
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new MemoryStream();

                    using (stream)
                    {
                        formatter.Serialize(stream, source);
                        stream.Seek(0, SeekOrigin.Begin);

                        // Get a clon of the source.
                        clonedObject = (T) formatter.Deserialize(stream);
                    }
                }
            }

            return clonedObject;
        }

        /// <summary>
        /// Compresses the string.//SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --  New method for compressing the XML string and the encoding it in base64
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                var strResult = Convert.ToBase64String(gZipBuffer);
                return strResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Decompresses the string.//SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --  New method for decoding the base64 encoding and then decopressin it to XML string
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            try
            {
                byte[] gZipBuffer = Convert.FromBase64String(compressedText);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                    var buffer = new byte[dataLength];

                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }

                    var strResult = Encoding.UTF8.GetString(buffer);
                    return strResult;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
