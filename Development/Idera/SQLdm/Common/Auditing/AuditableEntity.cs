// -----------------------------------------------------------------------
// <copyright file="AuditableEntity.cs" company="Idera">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Idera.SQLdm.Common.Auditing
{
    using System;
    using System.Collections.Generic;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Entity class
    /// </summary>
    [Serializable]
    public class AuditableEntity : ILogicalThreadAffinative
    {
        public const String Context = "AuditableEntity";

        public DateTime TimeStamp { get; set; }

        public String Workstation { get; set; }

        public String WorkstationUser { get; set; }

        public String SqlUser { get; set; }

        public String Name { get; set; }

        public String MetaData { get; set; }

        public String Header { get; set; }

        public List<Pair<String, String>> MetadataProperties { get; protected set; }

        public AuditableEntity()
        {
            TimeStamp = DateTime.Now;
            Workstation = String.Empty;
            WorkstationUser = String.Empty;
            SqlUser = String.Empty;
            Name = String.Empty;
            Header = string.Empty;
            MetadataProperties = new List<Pair<string, string>>();
        }

        /// <summary>
        /// Adds a change as a Property/New Value pair.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        public void AddMetadataProperty(string property, string newValue)
        {
            this.MetadataProperties.Add(new Pair<string, string>(property, newValue));
        }

        /// <summary>
        /// Returns the AuditableEntity values as a flat text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.AppendFormat("{0}{1}", this.TimeStamp.ToLocalTime(), Environment.NewLine);
            text.AppendFormat("{0}{1}", this.Workstation, Environment.NewLine);
            text.AppendFormat("{0}{1}", this.WorkstationUser, Environment.NewLine);
            text.AppendFormat("{0}{1}", this.SqlUser, Environment.NewLine);
            text.AppendFormat("{0}{1}", this.Name, Environment.NewLine);
            text.AppendFormat("{0}{1}", this.MetaData, Environment.NewLine);

            return text.ToString();
        }

        /// <summary>
        /// Indicates if the metadata is empty.
        /// </summary>
        /// <returns></returns>
        public bool HasMetadataProperties()
        {
            return MetadataProperties.Count > 0;
        }

        /// <summary>
        /// Get the first property value that match with the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The first property value that match with the property name.</returns>
        public String GetPropertyValue(String propertyName)
        {
            String propertyValue = String.Empty;

            foreach (var property in MetadataProperties)
            {
                if (property.First.Equals(propertyName))
                {
                    propertyValue = property.Second;
                    break;
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Removes the specified property given a key.
        /// </summary>
        /// <param name="propertyKey">The name of the property</param>
        public void RemovePropertyValueByKey(String propertyKey)
        {
            foreach (var property in MetadataProperties)
            {
                if (property.First.Equals(propertyKey))
                {
                    MetadataProperties.Remove(property);   
                    break;
                }
            }
        }
    }
}
