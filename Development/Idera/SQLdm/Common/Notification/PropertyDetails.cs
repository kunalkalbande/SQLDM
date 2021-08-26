//------------------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Text;

    /// <summary>
    /// Class representing the information contained in a PropertyInfoAttribute attached to a property.
    /// </summary>
    [Serializable]
    public class PropertyDetails
    {
        #region fields

        private string name;
        private Type propertyType;
        private string description;
        private object defaultValue;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PropertyInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="attribute">The attribute.</param>
        public PropertyDetails(string name, Type propertyType, ObjectPropertyInfoAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            Name = name;
            PropertyType = propertyType;
            Description = attribute.Description;
            DefaultValue = attribute.DefaultValue;
        }

        #endregion

        #region properties

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
        /// Gets the name of the display.
        /// </summary>
        /// <value>The name of the display.</value>
        public string DisplayName
        {
            get { return GetHumanName(Name); }
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType
        {
            get { return propertyType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("propertyType");
                propertyType = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return description; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("description");
                description = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Gets the human friendl name for a CamelCase property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected string GetHumanName(string name)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0) // Don't insert space at first character
                {
                    if (char.IsUpper(name[i])) // If this character is upper case
                    {
                        if ((i < name.Length - 1 && !char.IsUpper(name[i + 1])) || i >= name.Length - 1) // We're not at the end of a string and next character is upper case or we're at the end
                        {
                            sb.Append(' ');
                        }
                    }
                }
                sb.Append(name[i]);
            }

            return sb.ToString().Trim();
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
