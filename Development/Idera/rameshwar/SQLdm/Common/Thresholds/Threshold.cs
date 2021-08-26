//------------------------------------------------------------------------------
// <copyright file="Threshold.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Thresholds
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Snapshots;
    using System.Reflection;
    using Idera.SQLdm.Common.Objects;
    using Vim25Api;
    using System.Globalization;

    //using Idera.SQLsafe.Shared.Service.Backup;

    [Serializable]
    public class Threshold : ISerializable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Threshold");

        public static Type[] SUPPORTED_TYPES = {
            typeof(bool),
            typeof(string),
            typeof(Threshold.ComparableList),
            typeof(ServiceState),
            typeof(OptionStatus),
            typeof(DBStatus),
            typeof(OSMetricsStatus),
            typeof(SQLdmServiceState),
            typeof(FileSize),
            typeof(AdvancedAlertConfigurationSettings),
            typeof(SnoozeInfo),
            typeof(int),
            typeof(uint),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(MirroringMetrics.MirroringStateEnum),
            typeof(MirroringMetrics.MirroringRoleEnum),
            typeof(MirroringSession.MirroringPreferredConfig),
            typeof(MirroringMetrics.WitnessStatusEnum),
            typeof(JobStepCompletionStatus),
            typeof(VirtualMachinePowerState),
            typeof(HostSystemPowerState)
            //,typeof(OperationStatusCode)
        };

        private bool enabled;
        private Operator op;
        private IComparable value;

        // transient variable
        private Type valueType;

        public Threshold()
        {
            Op = Operator.GE;
            Enabled = true;
        }

        /// <summary>
        /// Constructs a new Threshold with Op = Operator.NE
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="value"></param>
        public Threshold(bool enabled, bool value)
        {
            Op = Operator.NE;
            Enabled = enabled;
            Value = value;
        }

        /// <summary>
        /// Constructs a new Threshold with Op = Operator.GE
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="value"></param>
        public Threshold(bool enabled, IComparable value)
        {
            Op = Operator.GE;
            Enabled = enabled;
            Value = value;
        }

        /// <summary>
        /// Constructs a new Threshold
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        public Threshold(bool enabled, Operator op, IComparable value)
        {
            Op = op;
            Enabled = enabled;
            Value = value;
        }

        protected Threshold(SerializationInfo info, StreamingContext context)
        {
            Op = (Operator)info.GetValue("op", typeof(Operator));
            Value = (IComparable)info.GetValue("value", typeof(IComparable));
            Enabled = info.GetBoolean("enabled");
        }

        public object Value
        {
            get { return value; }
            set
            {
                if (!(value is IComparable))
                    throw new ArgumentException("Values must implement IComparable");
                this.value = value as IComparable;
                valueType = value.GetType();
                Debug.Assert(IsSupportType(valueType), "Value set to type that is not supported");
            }
        }

        [XmlAttribute]
        public Operator Op
        {
            get { return op; }
            set { op = value; }
        }

        [XmlAttribute]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Validate the type to see if it is supported.  Unsupported types
        /// may work initially but will not serialize correctly and as a result
        /// will not deserialize back to the same value.  This is especially 
        /// true for enum values which will serialize/deserialize as an int.  
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSupportType(Type type)
        {
            foreach (Type supportedType in SUPPORTED_TYPES)
            {
                if (supportedType == type)
                    return true;
            }
            return false;
        }

        public bool IsInViolation(IComparable target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            Type targetType = target.GetType();
            if (!targetType.IsValueType && !(target is string))
                throw new ArgumentException("target is not a value type");

            IComparable compareValue = Value as IComparable;
            if (valueType != typeof(ComparableList) && valueType != target.GetType())
            {
                try
                {
                    Type comparisonType = null;
                    if (NeedsConversion(targetType, out comparisonType))
                    {
                        if (valueType != comparisonType)
                        {
                            compareValue = Convert(compareValue, comparisonType);
                        }
                        if (targetType != comparisonType)
                        {
                            target = Convert(target, comparisonType);
                        }
                    }
                }
                catch (Exception)
                {
                    LOG.DebugFormat("Exception converting to compatible type: target {0} - value {1}", targetType.Name, valueType.Name);
                    return false;
                }
            }
            try
            {
                int ic;

                if (target is Single)
                {
                    target = (Single)Math.Round((Single)target, 2);
                }

                if (valueType == typeof(ComparableList))
                    ic = compareValue.CompareTo(target);
                else
                    ic = target.CompareTo(compareValue);

                switch (op)
                {
                    case Operator.GE:
                        return ic >= 0;
                    case Operator.GT:
                        return ic > 0;
                    case Operator.EQ:
                        return ic == 0;
                    case Operator.NE:
                        return ic != 0;
                    case Operator.LE:
                        return ic <= 0;
                    case Operator.LT:
                        return ic < 0;
                    case Operator.NA:
                        return false;
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Exception comparing threshold values", e);
            }
            return false;
        }

        private IComparable Convert(IComparable compareValue, Type comparisonType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(compareValue);
            if (converter.CanConvertTo(comparisonType))
            {
                object result = converter.ConvertTo(compareValue, comparisonType);
                if (result is IConvertible)
                    return result as IComparable;
            }
            else
            if (comparisonType == typeof(decimal))
            {
                if (compareValue.GetType() == typeof(float))
                    return (decimal)((float)compareValue);
                if (compareValue.GetType() == typeof(double))
                    return (decimal)((double)compareValue);
            }
            return compareValue;
        }

        private bool NeedsConversion(Type targetType, out Type comparisonType)
        {
            Type lowerType;
            Type higherType;
            TypeCode higherTypeCode;

            TypeCode targetTypeCode = Type.GetTypeCode(targetType);
            TypeCode valueTypeCode = Type.GetTypeCode(valueType);
            if (targetTypeCode > valueTypeCode)
            {
                lowerType = valueType;
                higherType = targetType;
                higherTypeCode = targetTypeCode;
            }
            else
            {
                lowerType = targetType;
                higherType = valueType;
                higherTypeCode = valueTypeCode;
            }

            switch (higherTypeCode)
            {
                case TypeCode.String:   // if one of the types is a string convert to string
                    comparisonType = typeof(string);
                    return true;
                case TypeCode.Single:   // if one of the types have a fractional component
                case TypeCode.Double:   // convert to that type
                    comparisonType = higherType;
                    break;
                case TypeCode.Decimal:
                    comparisonType = lowerType;
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    comparisonType = typeof(double);
                    break;
                default:                // default to string comparison
                    comparisonType = typeof(string);
                    LOG.Debug("Unexpected typecode encountered while comparing metric threshold values.  The typecode is: " + higherTypeCode);
                    break;
            }
            // if the lower type is a bool then convert everything to bool
            if (lowerType == typeof(bool))
            {
                comparisonType = typeof(bool);
                return true;
            }
            // skip conversion
            return true;
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("op", op);
            info.AddValue("value", value);
            info.AddValue("enabled", enabled);
        }

        /// <summary>
        ///  Serializes a threshold as XML
        /// </summary>
        /// <param name="threshold">object to be serialized</param>
        /// <param name="xml">serialized object</param>
        /// <returns></returns>
        public static void Serialize(Threshold threshold, out string xml)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(Threshold), SUPPORTED_TYPES);
            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, threshold);
                writer.Flush();
            }

            xml = buffer.ToString();
        }

        /// <summary>
        /// Deserializes an IThreshold object from XML.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Threshold Deserialize(string xml)
        {
            Type type = typeof(Threshold);
            XmlSerializer serializer = Data.XmlSerializerFactory.GetSerializer(type, SUPPORTED_TYPES);

            Threshold result = null;

            StringReader stream = new StringReader(xml);
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                result = (Threshold)serializer.Deserialize(xmlReader);
                if (result != null)
                {
                    if (result.IsExponential)
                    {
                        // Parse a string in exponential notation with only the AllowExponent flag.               
                        result.value = string.Format("{0:0}", result.Value);
                    }
                }
            }

            return result;
        }

        public static void SerializeData(object data, out string xml)
        {
            XmlSerializer serializer = data is AdvancedAlertConfigurationSettings ?
                Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(AdvancedAlertConfigurationSettings), SUPPORTED_TYPES) :
                Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(ObjectWrapper), SUPPORTED_TYPES);

            if (!(data is AdvancedAlertConfigurationSettings))
                data = new ObjectWrapper(data);

            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, data);
                writer.Flush();
            }

            xml = buffer.ToString();
        }

        public static object DeserializeData(string xml)
        {
            Type type = xml.TrimEnd().EndsWith("</ObjectWrapper>") ? typeof(ObjectWrapper) : typeof(AdvancedAlertConfigurationSettings);
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(type, SUPPORTED_TYPES);

            object result = null;

            if (xml.Length == 0)
            {
                LOG.Debug("XMLErrorPrevented: Zero length string passed in for deserialization.");
                return null;
            }

            StringReader stream = new StringReader(xml);
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                result = serializer.Deserialize(xmlReader);
                if (result is ObjectWrapper)
                    result = ((ObjectWrapper)result).Value;
            }

            return result;
        }

        [Serializable]
        public enum Operator
        {
            GE,
            GT,
            EQ,
            NE,
            LE,
            LT,
            NA
        }

        [Serializable]
        public sealed class ComparableList : List<object>, IComparable
        {
            public ComparableList() { /* */ }

            public ComparableList(params IComparable[] values)
            {
                this.AddRange(values);
            }

            /// <summary>
            /// Return true if obj is contained in the list.  If obj
            /// is enumerable then return true if all are in the list.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                if (obj is IEnumerable)
                {
                    foreach (object o in obj as IEnumerable)
                    {
                        if (!this.Contains(o))
                            return 1;
                    }
                    return 0;
                }
                // Special handling for Database Status, where any match is a full match
                if (obj.GetType() == typeof(DatabaseStatus))
                {
                    foreach (object o in this)
                    {
                        if (Database.MatchStatus((DatabaseStatus)obj, (DatabaseStatus)o))
                            return 0;
                    }
                    return 1;
                }
                return (this.Contains(obj)) ? 0 : 1;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                foreach (object o in this)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append(GetEnumDescription(o));
                }
                return builder.ToString();
            }
        }
        private bool IsExponential
        {
            get
            {
                if (Value == null || !IsNumber) return false;
                string stringValue = Value.ToString();
                double number;
                return (stringValue.Contains("E") || stringValue.Contains("e")) && double.TryParse(stringValue, out number);
            }
        }
        private bool IsNumber
        {
            get
            {
                return valueType == typeof(int)
                        || valueType == typeof(uint)
                        || valueType == typeof(short)
                        || valueType == typeof(ushort)
                        || valueType == typeof(long)
                        || valueType == typeof(ulong)
                        || valueType == typeof(float)
                        || valueType == typeof(double)
                        || valueType == typeof(decimal);
            }
        }
        #region Enum Helpers
        public static string GetEnumDescription(object o)
        {
            System.Type otype = o.GetType();
            if (otype.IsEnum)
            {
                FieldInfo field = otype.GetField(Enum.GetName(otype, o));
                if (field != null)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return o.ToString();
        }
        #endregion
    }
}
