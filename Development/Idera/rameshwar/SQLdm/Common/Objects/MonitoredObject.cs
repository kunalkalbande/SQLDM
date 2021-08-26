//------------------------------------------------------------------------------
// <copyright file="NamedObject.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Runtime.Serialization;

    /// <summary>
    /// Abstract class that represents a monitored object.
    /// </summary>
    [Serializable]
    public abstract class MonitoredObject : IEquatable<MonitoredObject>, IComparable<MonitoredObject>, ISerializable
    {
        #region fields

        private int id;
        private string name;
        private bool enabled;

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        protected MonitoredObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredObject"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected MonitoredObject(int id, string name, bool enabled)
        {
            this.id = id;
            this.name = name;
            this.enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredObject"/> class.
        /// </summary>
        /// <param name="monitoredObject">The monitored object.</param>
        protected MonitoredObject(MonitoredObject monitoredObject)
        {
            id = monitoredObject.Id;
            name = monitoredObject.Name;
            enabled = monitoredObject.Enabled;
        }

        protected MonitoredObject(SerializationInfo info, StreamingContext context)
        {
            id = info.GetInt32("id");
            name = info.GetString("name");
            enabled = info.GetBoolean("enabled");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
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
        /// Gets or sets a value indicating whether this <see cref="T:MonitoredObject"/> is enabled.
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
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> returned from the <see cref="T:Idera.SQLdm.Objects.ObjectNaespaceUtility"></see> class.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Toes the URI.
        /// </summary>
        /// <returns></returns>
        public abstract Uri ToUri();

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as MonitoredObject);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #region IComparable<NamedObject> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(MonitoredObject other)
        {
            if (other == null)
                return 1;

            return ToUri().ToString().CompareTo(other.ToUri().ToString());
        }

        #endregion

        #region IEquatable<NamedObject> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(MonitoredObject other)
        {
            if (other == null)
                return false;

            return (Id.Equals(other.Id));
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("name", name);
            info.AddValue("enabled", enabled);
        }
    }
}
