//------------------------------------------------------------------------------
// <copyright file="XmlObjectSerializer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Idera.SQLdm.Common.Serialization
{
    /// <summary>
    /// This class is responsible to serialize an object in its XML representation.
    /// </summary>
    public class XmlObjectSerializer
    {
        /// <summary>
        /// Serialize an object in its XML representation, returns this in a MemoryStream. If the
        /// object to try to serialize is null, returns null.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The XML representation, returns this in a MemoryStream. If the object to try to
        /// serialize is null, returns null.</returns>
        public static MemoryStream Serialize<T>(T obj)
        {
            if (null == obj)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, obj);

            return stream;
        }

        /// <summary>
        /// Serialize an object in its XML representation, returns this in a String. If the
        /// object to try to serialize is null, returns null.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The XML representation, returns this in a String. If the object to try to
        /// serialize is null, returns Empty String.</returns>
        public static String SerializeToString<T>(T obj)
        {
            if (null == obj)
            {
                return String.Empty;
            }

            MemoryStream objectSerialized = Serialize(obj);
            objectSerialized.Seek(0, SeekOrigin.Begin);

            return Encoding.ASCII.GetString(objectSerialized.ToArray());
        }

        /// <summary>
        /// De-serialize an object based in its XML representation.
        /// </summary>
        /// <typeparam name="T">The type for the object to de-serialize.</typeparam>
        /// <param name="stream">The XML stream from which try to build the object.</param>
        /// <returns>An instance of 'T' based on XML stream.</returns>
        public static T Deserialize<T>(Stream stream)
        {
            T result;

            if (stream == null)
            {
                result = default(T);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                object deserializedObject = serializer.Deserialize(stream);

                result = (T)deserializedObject;
            }

            return result;
        }
    }
}
