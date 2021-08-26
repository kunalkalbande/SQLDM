//------------------------------------------------------------------------------
// <copyright file="Serialized.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Data
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;


    public interface ISerialized
    {
        
    }

    /// <summary>
    /// Serializable wrapper for an object.  The main purpose for this object
    /// is to keep from deserializing the wrapped objects between remoting hops.
    /// For instance, without this object, an on-demand snapshot would be serialized
    /// by the Collection Service, deserialized on the Management Service, only to 
    /// serialize it again to pass it back to the client.  With the wrapper, the snapshot
    /// would be serialized to a byte array and in the Management Service only the wrapper 
    /// would be deserialized.  There are also static helper methods to serialize objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Serialized<T> : ISerialized
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Serialized<" + typeof(T).Name + ">");

        private byte[] serializedValue;
        private bool useCompression = false;

        [NonSerialized]
        private T theValue;


        public Serialized(T value) : this(value, false)
        {
        }
        
        public Serialized(T value, bool useCompression)
        {
                theValue = value;
                this.useCompression = useCompression;
        }

        public static byte[] GetSerializedBlob(Serialized<T> value)
        {
            if (value.serializedValue == null)
                value.Serialize(false);
            return value.serializedValue;
        }

        public static implicit operator Serialized<T>(T value)
        {
            return new Serialized<T>(value);
        }

        public static implicit operator T(Serialized<T> value)
        {
            if (value != null)
            {
                if (value.serializedValue == null)
                {
                    return value.theValue;
                }
                else
                {
                    value.theValue = value.Deserialize();
                    value.serializedValue = null;
                    return value.theValue;
                }
            }
            return default(T);
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            // needs to make sure that the wrapped object has been serialized to the byte array
            if (serializedValue == null)
            {
                serializedValue = Serialize(theValue, useCompression);
            }
        }

        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            // constructors don't get called - make sure the value gets a default assigned
            theValue = default(T);
        }

        public void Serialize(bool clearValue)
        {
            if (serializedValue == null)
            {
                serializedValue = Serialize(theValue, useCompression);                
            }

            if (clearValue)
                theValue = default(T);
        }

        public T Deserialize()
        {
            if (useCompression)
                return DeserializeCompressed();

            T result = default(T);

            if (serializedValue == null)
                return result;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(serializedValue))
            {
                result = (T) formatter.Deserialize(stream);
            }

            LOG.DebugFormat("Deserialized {0} bytes.", serializedValue.Length);

            return result;
        }

        public T DeserializeCompressed()
        {
            T result = default(T);

            if (serializedValue == null)
                return result;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(serializedValue))
            {
                using (DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    result = (T) formatter.Deserialize(inflater);
                }
            }
            LOG.DebugFormat("Deserialized {0} compressed bytes.", serializedValue.Length);

            return result;
        }

        public static byte[] Serialize<TYPE>(TYPE value, bool useCompression)
        {
            if (useCompression)
                return SerializeCompressed(value);

            byte[] bytes = null;
            
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(stream, value);

                }
                catch (Exception exception)
                {
                    LOG.Error("Serialize Retrying " + exception);
                    formatter.Serialize(stream, value);
                }
                bytes = stream.ToArray();
            }

            LOG.DebugFormat("Serialized {0} bytes.", bytes.Length);

            return bytes;
        }

        public static byte[] SerializeCompressed<TYPE>(TYPE value)
        {
            byte[] bytes = null;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                DeflateStream deflater = new DeflateStream(stream, CompressionMode.Compress, true);
                BufferedStream bufferedStream = new BufferedStream(deflater);
                try
                {

                    formatter.Serialize(bufferedStream, value);

                }
                catch (Exception exception)
                {
                    LOG.Error("SerializeCompressed Retrying for "+ exception);
                    formatter.Serialize(bufferedStream, value);
                }
                bufferedStream.Close();
                deflater.Close();

                bytes = stream.ToArray();

                LOG.DebugFormat("Serialized {0} compressed bytes.", bytes.Length);
            }
            return bytes;
        }
        
        public static TYPE Deserialize<TYPE>(byte[] serializedValue, bool useCompression)
        {
            using (LOG.VerboseCall("Deserialize<" + typeof(TYPE).Name + ">"))
            {
                if (useCompression)
                    return DeserializeCompressed<TYPE>(serializedValue);

                TYPE result = default(TYPE);

                if (serializedValue == null)
                    return result;

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(serializedValue))
                {
                    result = (TYPE) formatter.Deserialize(stream);
                }

                LOG.DebugFormat("Deserialized {0} bytes.", serializedValue.Length);

                return result;
            }
        }

        public static TYPE DeserializeCompressed<TYPE>(byte[] serializedValue)
        {
            using (LOG.VerboseCall("Deserialize<" + typeof(TYPE).Name + ">"))
            {
                TYPE result = default(TYPE);

                if (serializedValue == null)
                    return result;

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(serializedValue))
                {
                    using (DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        result = (TYPE) formatter.Deserialize(inflater);
                    }
                }
                LOG.DebugFormat("Deserialized {0} compressed bytes.", serializedValue.Length);

                return result;
            }
        }

  
        public static TYPE? TryGetSerializedStruct<TYPE>(SerializationInfo info, string key) where TYPE : struct
        {
            TYPE? result = null;
            try
            {
                var value = info.GetValue(key, typeof(TYPE));
                if (value != null) result = (TYPE)value;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static TYPE TryGetSerializedClass<TYPE>(SerializationInfo info, string key) where TYPE : class
        {
            TYPE result = null;
            try
            {
                result = (TYPE)info.GetValue(key, typeof(TYPE));
            }
            catch (Exception)
            {
            }
            return result;
        }


    }
}
