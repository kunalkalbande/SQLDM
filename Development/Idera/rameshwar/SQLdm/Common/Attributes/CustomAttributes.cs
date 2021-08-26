//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Attributes
{
    /// <summary>
    /// Used to bind a column returned from a query to an attribute in an object.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ColumnOrdinalBindingAttribute : Attribute
    {
        private int columnIndex;

        public ColumnOrdinalBindingAttribute(int columnIndex)
        {
            this.columnIndex = columnIndex;
        }

        public int ColumnIndex
        {
            get { return columnIndex; }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ReferencedObjectBindingAttribute : ColumnOrdinalBindingAttribute
    {
        private Type cachedObjectType;
        private bool required;

        public ReferencedObjectBindingAttribute(int columnIndex) : base(columnIndex)
        {
        }

        public Type CachedObjectType
        {
            get { return cachedObjectType; }
            set { cachedObjectType = value; }
        }

        public bool IsRequired
        {
            get { return required; }
            set { required = value; }
        }

    }


    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class StoredProcedureBindingAttribute : Attribute
    {
        private string paramName;

        public StoredProcedureBindingAttribute(string paramName)
        {
            this.paramName = paramName;
        }

        public string ParameterName
        {
            get { return paramName; }
        }
    }

    /// <summary>
    /// Used to specify the table name for the attributed object.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TableBindingAttribute : Attribute
    {
        private string tableName;

        public TableBindingAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        public string TableName
        {
            get { return tableName; }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ObjectCachePolicyAttribute : Attribute
    {
        private static TimeSpan DEFAULT_TTL = TimeSpan.FromMinutes(5);
        private string key = "Id";
        private bool weakReference = true;
        private bool permanent = false;
        private bool resetTTLOnAccess = false;
        private TimeSpan ttl;

        public ObjectCachePolicyAttribute()
        {
            ttl = DEFAULT_TTL;
        }

        public ObjectCachePolicyAttribute(double ttlSeconds)
        {
            ttl = TimeSpan.FromSeconds(ttlSeconds);
        }

        public TimeSpan TimeToLive
        {
            get { return ttl; }
        }

        public DateTime GetExpirationTime()
        {
            return (DateTime.Now + ttl);
        }

        public string Key
        {
            get { return Key; }
            set { key = value; }
        }

        public bool IsWeakReference
        {
            get { return weakReference; }
            set { weakReference = value; }
        }

        public bool IsPermanent
        {
            get { return permanent; }
            set { permanent = value; }
        }

        public bool IsResetTtlOnAccess
        {
            get { return resetTTLOnAccess; }
            set { resetTTLOnAccess = value; }
        }

    }



    /// <summary>
    /// Custom Attribute indicate which porperty needs a new name and indicate if this porperty is auditable
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class AuditableAttribute : Attribute
    {

        private bool propetySensitive = false;

        /// <summary>
        /// Create a Auditable Attribute to rename the porperties and set IsAuditable true
        /// </summary>
        /// <param name="reName">New name for the porperty</param>
        public AuditableAttribute(string reName)
        {
            this.NewPropertyName = reName;
            this.IsAuditable = true;
            this.propetySensitive = false;
        }

        /// <summary>
        /// Create a Auditable Attribute to rename the porperty and set IsAuditable 
        /// </summary>
        /// <param name="reName">New name for the properties</param>
        /// <param name="propertySensitive">Indicate if the porperty is Insensitive</param>
        public AuditableAttribute(string reName, bool propertySensitive)
        {
            this.NewPropertyName = reName;
            this.IsAuditable = true;
            this.propetySensitive = propertySensitive;
        }

        /// <summary>
        /// Create a Auditable Attribute indicated if IsAuditable and set the null the new name
        /// </summary>
        /// <param name="isAuditable">Indicate if property is auditable</param>
        public AuditableAttribute(bool isAuditable)
        {
            this.NewPropertyName = null;
            this.IsAuditable = isAuditable;
            this.propetySensitive = false;
        }

       
        /// <summary>
        /// Create a Auditable Attribute indicated if IsAuditable and is sensitive also set the null the new name
        /// </summary>
        /// <param name="isAuditable">Indicate if property is auditable</param>
        /// <param name="propertySensitive">Indicate if property is Sensitive to show in log</param>
        public AuditableAttribute(bool isAuditable, bool propertySensitive)
        {
            this.NewPropertyName = null;
            this.IsAuditable = isAuditable;
            this.propetySensitive = propertySensitive;
        }

        /// <summary>
        /// The new property name
        /// </summary>
        public string NewPropertyName { get; set; }

        /// <summary>
        /// True to find the diference in PropertiesComparer and false to ignore the property
        /// </summary>
        public bool IsAuditable { get; set; }

        /// <summary>
        /// True to find if sensitive in PropertiesComparer and false to not log
        /// </summary>
        public bool IsPropetySensitive
        {
            get { return propetySensitive; }
        }

    }
}
