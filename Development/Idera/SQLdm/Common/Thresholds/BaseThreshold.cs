//------------------------------------------------------------------------------
// <copyright file="BaseThreshold.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Thresholds
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Abstract base class that contains common properties for thresholds.
    /// </summary>
    [Serializable]
    public abstract class BaseThreshold : IThreshold, ISerializable
    {
        #region fields

        private object value;
        private bool enabled;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseThreshold"/> class.
        /// </summary>
        protected BaseThreshold()
        {
        }

        protected BaseThreshold(SerializationInfo info, StreamingContext context)
        {
            value = info.GetValue("value", typeof(object));
            enabled = info.GetBoolean("enabled");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseThreshold&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        protected BaseThreshold(object value, bool enabled)
        {
            Value = value;
            Enabled = enabled;
        }

        #endregion

        #region properties


        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        public object Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.value = value;
            }
        }

//        /// <summary>
//        /// Gets or sets the threshold.
//        /// </summary>
//        /// <value>The threshold.</value>
//        internal byte[] SerializedValue
//        {
//            get
//            {
//                BinaryFormatter frmt = new BinaryFormatter();
//                using (MemoryStream ms = new MemoryStream())
//                {
//                    frmt.Serialize(ms, Value);
//                    ms.Close();
//                    return ms.GetBuffer();
//                }
//            }
//            set
//            {
//                BinaryFormatter frmt = new BinaryFormatter();
//                using (MemoryStream ms = new MemoryStream(value))
//                {
//                    Value = frmt.Deserialize(ms);
//                }
//            }
//        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:GreaterThanThreshold&lt;T&gt;"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Checks the specified value against the threshold
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// true if the value is in violation of this threshold
        /// </returns>
        abstract public bool IsInViolation(object target);

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", value);
            info.AddValue("enabled", enabled);
        }

        /// <summary>
        ///  Serializes a threshold as XML
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static void Serialize(IThreshold threshold, out string xml, out string typeName)
        {
            typeName = threshold.GetType().FullName;

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(threshold.GetType());
            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, threshold);
                writer.Flush();
            }

            xml = buffer.ToString();
        }

        /// <summary>
        /// Deserializes an IThreshold object from XML.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IThreshold Deserialize(string typeName, string xml)
        {
            Type type = Type.GetType(typeName, true, true);
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(type);

            IThreshold result = null;
            
            StringReader stream = new StringReader(xml);
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                result = (IThreshold)serializer.Deserialize(xmlReader);
            }

            return result;
        }

    }
}
