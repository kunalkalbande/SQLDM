//------------------------------------------------------------------------------
// <copyright file="ObjectProperty.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    /// <summary>
    /// base class that represents a notification property.
    /// </summary>
    [Serializable]
    public class ObjectProperty
    {
        #region fields

        private Guid id;
        private string name;
        private object value;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ObjectProperty"/> class.
        /// </summary>
        protected ObjectProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ObjectProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public ObjectProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute]
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("name"); 
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
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

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        [XmlIgnore]
        public byte[] SerializedValue
        {
            get
            {
                BinaryFormatter frmt = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    frmt.Serialize(ms, Value);
                    ms.Close();
                    return ms.GetBuffer();
                }
            }
            set
            {
                BinaryFormatter frmt = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(value))
                {
                    Value = frmt.Deserialize(ms);
                }
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
