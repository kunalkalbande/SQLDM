using System;
using System.Collections.Generic;
using System.Reflection;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Auditing
{
    public class PropertiesComparer
    {
        public class PropertiesData
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string OldValue { get; set; }
        }

        /// <summary>
        /// Returns all properties that were tagged as "Auditable Attribute" in the Object.
        /// </summary>
        /// 
        /// <param name="auditableObject">The object from which get the "Auditable Attribute"s</param>
        /// 
        /// <returns>All properties that were tagged as "Auditable Attribute" in the Object.</returns>
        public static List<PropertiesData> GetAuditableAttributes(Object auditableObject)
        {
            var propertiesDatas = new List<PropertiesData>();

            Type type = auditableObject.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof (AuditableAttribute), true);
                String propertyName = property.Name;

                // Try to get the "AuditableAttribute".
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0] as AuditableAttribute;

                    if (attribute == null || !attribute.IsAuditable)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(attribute.NewPropertyName))
                    {
                        propertyName = attribute.NewPropertyName;
                    }

                    var propertyData = new PropertiesData();
                    propertyData.Name = propertyName;
                    propertyData.OldValue = String.Empty;
                    bool isSensitive = attribute.IsPropetySensitive;

                    // Read property name.
                    try
                    {
                        object auditableObjectValue = property.GetValue(auditableObject, null);

                        PushPropertyDataValue(auditableObjectValue, propertyData, isSensitive);

                        // No applicable old values.
                        propertyData.OldValue = String.Empty;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    // Fill the properties list.
                    propertiesDatas.Add(propertyData);
                }
            }

            return propertiesDatas;
        }

        public List<PropertiesData> GetNewProperties(object oldValue, object newValue)
        {
            List<PropertiesData> result = new List<PropertiesData>();

            Type type = oldValue.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(AuditableAttribute), true);
                string propertyName = property.Name;
                bool isSensitive = false;

                if (attributes.Length > 0)
                {
                    AuditableAttribute attribute = (AuditableAttribute)attributes[0];
                    isSensitive = attribute.IsPropetySensitive;

                    if (!attribute.IsAuditable)
                    {
                        continue;
                    }

                    if (attribute.NewPropertyName != null)
                    {
                        propertyName = attribute.NewPropertyName;
                    }
                }

                object value1 = null;
                object value2 = null;
                try
                {
                    value1 = property.GetValue(oldValue, null);
                    value2 = property.GetValue(newValue, null);
                }
                catch (Exception)
                {
                    continue;
                }

                if (value1 == null || value2 == null)
                {
                    continue;
                }

                if (IsSubProperty(property.PropertyType))
                {
                    foreach (var propertyInfo in GetNewProperties(value1, value2))
                    {
                        PropertiesData propertiesData = new PropertiesData();
                        //propertiesData.Name = property.Name + "." + propertyInfo.Name;
                        propertiesData.Name = propertyInfo.Name;
                        propertiesData.Value = propertyInfo.Value;
                        propertiesData.OldValue = propertyInfo.OldValue;
                        result.Add(propertiesData);
                    }
                    continue;
                }

                if (property.PropertyType.Namespace != null && property.PropertyType.Namespace.Equals(typeof(List<>).Namespace))
                {
                    //Handle a list 
                    continue;
                }

                if (!value1.Equals(value2))
                {
                    PropertiesData propertiesData = new PropertiesData();
                    propertiesData.Name = propertyName;

                    if (property.PropertyType.IsArray)
                    {
                        foreach (var value in (Array)value2)
                        {
                            propertiesData.Value += value.ToString() + ",";
                        }

                        if (propertiesData.Value == null)
                        {
                            continue;
                        }

                        propertiesData.Value = propertiesData.Value.Remove(propertiesData.Value.Length - 1, 1);
                    }
                    else
                    {
                        PushPropertyDataValue(value2, propertiesData, isSensitive);

                        propertiesData.OldValue = value1.ToString();
                    }

                    result.Add(propertiesData);
                }
            }

            return result;
        }

        private static void PushPropertyDataValue(object auditableObjectValue, PropertiesData propertyData, bool isSensitive)
        {
            if (isSensitive)
            {
                propertyData.Value = "";
                propertyData.Name = propertyData.Name + " changed";
            }
            else if (auditableObjectValue is Enum)
            {
                propertyData.Value = EnumDescriptionConverter.GetEnumDescription(auditableObjectValue as Enum);
            }
            else
            {
                propertyData.Value = auditableObjectValue.ToString();
            }
        }

        private bool IsSubProperty(Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            return !type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System.") && !type.IsArray;
        }
    }
}
