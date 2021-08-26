//------------------------------------------------------------------------------
// <copyright file="BaseNotificationInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    /// <summary>
    /// Abstract base class that <see cref="T:NotificationProviderInfo"/> and <see cref="T:NotificationDestinationInfo"/> derive from.
    /// </summary>
    [Serializable]
    public abstract class BaseNotificationInfo : ICloneable
    {
        #region fields

        private Guid id;
        private string name;
        private bool enabled;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseNotificationInfo"/> class.
        /// </summary>
        protected BaseNotificationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseNotificationInfo"/> class.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public BaseNotificationInfo(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseNotificationInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public BaseNotificationInfo(string name, bool enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [Browsable(false)]
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
        public virtual string Name
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
        /// Gets or sets a value indicating whether this <see cref="T:BaseNotificationInfo"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        [XmlAttribute]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void SetName(string name)
        {
            this.name = name;
        }
        
        /// <summary>
        /// Gets the available properties.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<PropertyDetails>  GetAvailableProperties();

        /// <summary>
        /// Gets the property details.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected IEnumerable<PropertyDetails> GetPropertyDetails(Type type)
        {
            List<PropertyDetails> ret = new List<PropertyDetails>();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object[] attributes = prop.GetCustomAttributes(typeof(ObjectPropertyInfoAttribute), true);
                if (attributes == null || attributes.Length != 1)
                    continue;

                ret.Add(new PropertyDetails(prop.Name, prop.PropertyType, attributes[0] as ObjectPropertyInfoAttribute));
            }

            return ret;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion


        public object Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}
