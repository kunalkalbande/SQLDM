//------------------------------------------------------------------------------
// <copyright file="NotificationDestinationInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.Reflection;

    /// <summary>
    /// Provides the necessary information needed to create an instance of a notification destination
    /// and set it's properties.
    /// </summary>
    [Serializable]
    public class NotificationDestinationInfo : ICloneable
    {
        #region fields

        private bool enabled;
        private Guid providerId;
        private NotificationProviderInfo provider;
        private List<NotificationDestinationProperty> properties;

        #endregion

        #region constructors

        public NotificationDestinationInfo()
        {
            enabled = true;
        }

        public NotificationDestinationInfo(Guid providerId) : this()
        {
            this.providerId = providerId;
        }

        #endregion

        #region properties        

        [XmlAttribute]
        public Guid ProviderID
        {
            get
            {
                if (provider != null)
                    return provider.Id;

                return providerId;
            }

            set { providerId = value;  }

        }

        [XmlAttribute]
        public bool Enabled
        {
            get { return enabled;  }
            set { enabled = value; }
        }

        [XmlIgnore]
        public NotificationProviderInfo Provider
        {
            get { return provider; }
            set { provider = value; }
        }

        /// <summary>
        /// Gets or sets the properties.  Marked as XmlIgnore so that simpler tags can be
        /// specified in subclasses.  If you are using the base types then you will need to 
        /// enable this property with an Xml Override.
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore]
        [XmlElement(ElementName="Properties", Type=typeof(NotificationDestinationProperty))]
        public List<NotificationDestinationProperty> Properties
        {
            get
            {
                if (properties == null)
                    properties = new List<NotificationDestinationProperty>();
                return properties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("properties");
                properties = value;
            }
        }

        public NotificationDestinationProperty GetProperty(Guid propertyId)
        {
            foreach (NotificationDestinationProperty property in Properties)
            {
                if (property.Id == propertyId)
                    return property;
            }
            return null;
        }

        public bool IsPropertySet(string propertyName)
        {
            NotificationDestinationProperty prop = GetProperty(propertyName);
            return prop != null;
        }

        #endregion

        #region events

        #endregion

        #region methods
        
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
//        public object CreateInstance()
//        {
//            Type destinationType = provider.DestinationType;
//            object destination = Activator.CreateInstance(destinationType);
//            if (destination == null)
//                throw new Exception(String.Format("Unable to create an instance of ({0})", destinationType.FullName));
//
//            Dictionary<string, object> valueMap = new Dictionary<string, object>();
//            foreach (NotificationDestinationProperty prop in Properties)
//            {
//                if (valueMap.ContainsKey(prop.Name))
//                    throw new Exception(String.Format("Destination property map for ({0}) has duplicate entry for ({1})", Name, prop.Name));
//                valueMap[prop.Name] = prop.Value;
//            }
//
//            ObjectHelper.MapToObject(destination, valueMap);
//            
//            return destination;
//        }

        /// <summary>
        /// Gets the available properties.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PropertyDetails> GetAvailableProperties()
        {
            List<PropertyDetails> ret = new List<PropertyDetails>();

            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object[] attributes = prop.GetCustomAttributes(typeof(ObjectPropertyInfoAttribute), true);
                if (attributes == null || attributes.Length != 1)
                    continue;

                ret.Add(new PropertyDetails(prop.Name, prop.PropertyType, attributes[0] as ObjectPropertyInfoAttribute));
            }

            return ret;
        }

        public NotificationDestinationProperty GetProperty(string propertyName)
        {
            foreach (NotificationDestinationProperty property in Properties)
            {
                if (property.Name == propertyName)
                    return property;
            }
            return null;
        }

        public void SetProperty(string name, object value)
        {
            NotificationDestinationProperty npp = GetProperty(name);
            if (npp == null)
            {
                npp = new NotificationDestinationProperty(name, value);
                Properties.Add(npp);
            }
            else
                npp.Value = value;
        }

        
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            NotificationDestinationProperty property = GetProperty("Name");
            if (property != null)
                return property.Value.ToString();

            return Provider == null ? providerId.ToString() : Provider.Name;
        }

        #endregion

        #region interface implementations
        
        public object Clone()
        {
            NotificationDestinationInfo clone = (NotificationDestinationInfo)this.MemberwiseClone();

            // go a little deeper on the copy of the properties
            clone.Properties = new List<NotificationDestinationProperty>();
            foreach (NotificationDestinationProperty property in Properties)
            {
                NotificationDestinationProperty newProperty = new NotificationDestinationProperty(property);
                clone.Properties.Add(newProperty);
            }

            return clone;
        }

        public virtual void Validate()
        {
            // do your rudimentary validations here
        }

        #endregion

        #region nested types

        #endregion
    }
}
