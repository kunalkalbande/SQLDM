using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Configuration
{
    class PropertySorter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Return properties in sorted order based on the PropertyOrder attribute if present
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            List<PropertyOrder> properties = new List<PropertyOrder>();

            PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(value, attributes);
            foreach (PropertyDescriptor descriptor in descriptors)
            {
                Attribute attribute = descriptor.Attributes[typeof(DisplayOrderAttribute)];
                if (attribute != null)
                {
                    DisplayOrderAttribute orderAttribute = (DisplayOrderAttribute)attribute;
                    properties.Add(new PropertyOrder(descriptor.Name, orderAttribute.Order));
                }
                else
                {
                    // If no PropertyOrder attribute is specifed then given it an order of 0
                    properties.Add(new PropertyOrder(descriptor.Name, 0));
                }
            }

            // Sort uses the PropertyOrder CompareTo method of IComparable
            properties.Sort();

            List<string> sortedProperties = new List<string>();
            foreach (PropertyOrder property in properties)
            {
                sortedProperties.Add(property.Name);
            }

            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            return descriptors.Sort((string[])sortedProperties.ToArray());
        }
    }

    /// <summary>
    /// Attribute to allow specifying the order of the property for a PropertyGrid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayOrderAttribute : Attribute
    {
        private int order;

        public DisplayOrderAttribute(int order)
        {
            this.order = order;
        }

        public int Order
        {
            get { return order; }
        }
    }

    /// <summary>
    /// Class to store the Property Name and order and return them sorted correctly
    /// </summary>
    public class PropertyOrder : IComparable
    {
        private int order;
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        public PropertyOrder(string name, int order)
        {
            this.order = order;
            this.name = name;
        }

        public int CompareTo(object compareObject)
        {
            if (compareObject is PropertyOrder)
            {
                PropertyOrder item = (PropertyOrder)compareObject;

                // Sort using the specified order 
                int compareOrder = item.order;
                if (compareOrder == order)
                {
                    // If orders are equal, sort by name
                    return string.Compare(name, item.Name);
                }
                else if (compareOrder > order)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return string.Compare(name, compareObject.ToString());
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
