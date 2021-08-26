//------------------------------------------------------------------------------
// <copyright file="NotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using BBS.TracerX;
    using System.Runtime.InteropServices;

// Suppress warnings pertaining overrides
#pragma warning disable 0659

    /// <summary>
    /// Provides the necessary information needed to create an instance of a notification provider
    /// and set it's properties.
    /// </summary>
    [Serializable]
    public class NotificationProviderInfo : BaseNotificationInfo, IAuditable
    {
        private const String NameOfTheNotificationProvider = "Name of the notification provider";
        private const String ActionProviderTypeName = "Action Provider Type Name";
        private const string NotificationProviderType = "Notification provider type";

        #region fields

        private static readonly Logger LOG = Logger.GetLogger("NotificationProviderInfo");
        private static readonly Dictionary<String,Type> providerTypes = new Dictionary<String,Type>();
        private string providerTypeLabel;
        private string providerTypeName;
        private List<NotificationProviderProperty> properties;

        [NonSerialized]
        private Type providerType;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationProviderInfo"/> class.
        /// </summary>
        public NotificationProviderInfo()
        {
        }

        public static void RegisterNotificationProviderType(Type type)
        {
            using (LOG.InfoCall("RegisterNofiticationProviderType"))
            {
                if (!providerTypes.ContainsKey(type.FullName))
                {
                    providerTypes.Add(type.FullName, type);
                    LOG.InfoFormat("Registered notification provider: {0}", type.FullName);
                }
                else
                    LOG.InfoFormat("Notification provider already registered: {0}", type.FullName);
            }
        }

        public static Guid? GetDefaultId<T>() where T : NotificationProviderInfo
        {
            Guid? result = null;
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(T), typeof(GuidAttribute)) as GuidAttribute;
            
            if (attribute != null)
                result = new Guid(attribute.Value);

            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationProviderInfo"/> class.
        /// </summary>
        /// <param name="providerType">Type of the provider.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public NotificationProviderInfo(string providerTypeName, bool enabled)
            : base(enabled)
        {
            ProviderTypeName = providerTypeName;

            Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), true);
            if (attributes == null)
                throw new ArgumentException("Provider types must have a ProviderInfo attribute.");

            foreach(Attribute attribute in attributes)
            {
                if (attribute is DisplayNameAttribute)
                    Name = ((DisplayNameAttribute)attribute).DisplayName;
                else
                if (attribute is ProviderInfoAttribute)
                    ProviderTypeLabel = ((ProviderInfoAttribute)attribute).DestinationTypeLabel;
            }            
        }

        #endregion

        #region properties

        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        [XmlIgnore]
        [AuditableAttribute(false)]
        public virtual String Caption
        {
            get { return "Empty"; }
        }

        [XmlIgnore]
        [AuditableAttribute(false)]
        public string ProviderTypeLabel
        {
            get { return providerTypeLabel != null ? providerTypeLabel : ""; }
            set { providerTypeLabel = value; }
        }


        [XmlAttribute]
        [Description(NameOfTheNotificationProvider)]
        [AuditableAttribute(NameOfTheNotificationProvider)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }


        /// <summary>
        /// Gets or sets the type of the provider.
        /// </summary>
        /// <value>The type of the provider.</value>
        [XmlIgnore]
        [Browsable(false)]
        public Type ProviderType
        {
            get
            {
                using (LOG.InfoCall("ProviderType(get)"))
                {
                    if (providerType == null)
                    {
                        providerTypes.TryGetValue(providerTypeName, out providerType);
                    }

                    if (providerType == null)
                        LOG.InfoFormat("Unable to find registered provider type: {0}", providerTypeName);

                    return providerType;
                }                
            }
        }

        /// <summary>
        /// Gets or sets the name of the provider type.
        /// </summary>
        /// <value>The name of the provider type.</value>
        [Browsable(false)]
        [AuditableAttribute(false)]
        public string ProviderTypeName
        {
            get { return providerTypeName; }
            set { providerTypeName = value; }
        }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        /// <value>The type of the destination.</value>
        [XmlIgnore]
        [AuditableAttribute(false)]
        public Type DestinationType
        {
            get
            {
                Attribute attribute = Attribute.GetCustomAttribute(GetType(), typeof(ProviderInfoAttribute));
                if (attribute == null)
                    throw new Exception(String.Format("Unable to get destination type from provider type ({0})", ProviderType));

                ProviderInfoAttribute pia = attribute as ProviderInfoAttribute;
                return pia.DestinationType;
            }
        }

        /// <summary>
        /// Gets or sets the properties.  This property is tagged as XmlIgnore so that it 
        /// will not get serialized.  If you need these values to be serialized because you
        /// are not subclassing this object and providing your own properties then you will
        /// need to create to add code similar to this to create your serializer object:
        /// 
        ///     XmlAttributeOverrides overrides = new XmlAttributeOverrides();
        ///     XmlAttributes attributes = new XmlAttributes();
        ///     attributes.XmlIgnore = false;
        ///     overrides.Add(typeof(NotificationProviderInfo), "Properties", attributes);
        ///     XmlSerializer serializer = new XmlSerializer(typeof(SmtpNotificationProviderInfo), overrides);
        /// 
        /// </summary>
        /// <value>The properties.</value>
        [Browsable(false)]
        [XmlIgnore]
        [XmlElement(ElementName = "Properties", Type = typeof(NotificationProviderProperty))]
        [AuditableAttribute(false)]
        public virtual List<NotificationProviderProperty> Properties
        {
            get
            {
                if (properties == null)
                    properties = new List<NotificationProviderProperty>();
                return properties;
            }
            set { properties = value; }
        }

        public NotificationProviderProperty GetProperty(Guid propertyId)
        {
            foreach (NotificationProviderProperty property in Properties)
            {
                if (property.Id == propertyId)
                    return property;
            }
            return null;
        }

        public NotificationProviderProperty GetProperty(string propertyName)
        {
            foreach (NotificationProviderProperty property in Properties)
            {
                if (property.Name == propertyName)
                    return property;
            }
            return null;
        }
        
        public void SetProperty(string name, object value)
        {
            NotificationProviderProperty npp = GetProperty(name);
            if (npp == null)
            {
                npp = new NotificationProviderProperty(name, value);
                Properties.Add(npp);
            }
            else
                npp.Value = value;   
        }
        
        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        public INotificationProvider CreateInstance()
        {
            using (LOG.InfoCall("CreateInstance"))
            {
                INotificationProvider provider = null;
                
                Type type = ProviderType;
                if (type == null)
                {
                    String typeName = ProviderTypeName;
                    if (!String.IsNullOrEmpty(typeName))
                    {
                        int i = typeName.LastIndexOf('.');
                        if (i >= 0)
                            typeName = ProviderTypeName.Substring(i);
                        foreach (string providerTypeName in providerTypes.Keys) 
                        {
                            if (providerTypeName.EndsWith(typeName))
                            {
                                ProviderTypeName = providerTypeName;
                                type = providerTypes[providerTypeName];
                                break;
                            }
                        }
                    }
                }


                if (type != null)
                {
                    Type[] parms = new Type[] { typeof(NotificationProviderInfo) };
                    ConstructorInfo constructor = type.GetConstructor(parms);
                    provider = constructor.Invoke(new object[] { this }) as INotificationProvider;
                }

                if (provider == null)
                    throw new Exception(String.Format("Unable to create an instance of ({0})", ProviderType));

                return provider;
            }
        }

        /// <summary>
        /// Gets the available properties.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<PropertyDetails> GetAvailableProperties()
        {
            return GetPropertyDetails(ProviderType);
        }

        /// <summary>
        /// Gets the types for the available providers.
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAvailableProviderTypes()
        {
            List<Type> ret = new List<Type>();

            Type type;
            Type providerInterface = typeof(NotificationProviderInfo);

            Type[] types = Assembly.GetExecutingAssembly().GetExportedTypes();
            for (int i = 0; i < types.Length; i++)
            {
                type = types[i];
                if (!type.IsClass || type.IsAbstract)
                    continue;

                if (providerInterface.IsAssignableFrom(type))
                {
                    ProviderInfoAttribute pia =
                        (ProviderInfoAttribute)Attribute.GetCustomAttribute(type, typeof(ProviderInfoAttribute));

                    if (pia == null)
                        continue;

                    ret.Add(type);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the available providers.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<NotificationProviderInfo> GetAvailableProviders()
        {
            List<NotificationProviderInfo> ret = new List<NotificationProviderInfo>();

            foreach (Type type in GetAvailableProviderTypes()) 
            {
                NotificationProviderInfo instance = (NotificationProviderInfo)Activator.CreateInstance(type);
                ret.Add(instance);
            }

            return ret;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region interface implementations

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            NotificationProviderInfo npi = obj as NotificationProviderInfo;
            if (npi == null) return false;
            if (Name != npi.Name) return false;
            return true;
        }

        #endregion

        #region nested types

        #endregion

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entity = new AuditableEntity();

            entity.Name = Name;
            entity.AddMetadataProperty(NotificationProviderType, Caption);
            List<PropertiesComparer.PropertiesData> propertiesData = PropertiesComparer.GetAuditableAttributes(this);

            foreach (var property in propertiesData)
            {
                //to not log the password
                if (!property.Name.Equals("Password changed"))
                {
                    entity.AddMetadataProperty(property.Name, property.Value);
                }

            }

            return entity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            PropertiesComparer propertiesComparer = new PropertiesComparer();
            List<PropertiesComparer.PropertiesData> changedProperties = propertiesComparer.GetNewProperties(oldValue, this);
            AuditableEntity entity = new AuditableEntity();
            entity.Name = Name;

            foreach (var property in changedProperties)
            {
                entity.AddMetadataProperty(property.Name, property.Value);
            }

            if (entity.HasMetadataProperties())
            {
                entity.AddMetadataProperty(NotificationProviderType, Caption);
            }

            return entity;
        }
    }
}
