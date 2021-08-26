using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Idera.SQLdm.Common.Configuration
{
    using System.Text;

    public class EnumDescriptionConverter : System.ComponentModel.EnumConverter
    {
        protected System.Type myVal;
 
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        public static string GetFlagEnumDescription(Enum value)
        {
            Type type = value.GetType();
            object[] flagAtts = type.GetCustomAttributes(typeof (FlagsAttribute), false);
            if (flagAtts == null || flagAtts.Length == 0)
                return GetEnumDescription(value);

            try
            {
                StringBuilder builder = new StringBuilder();

                Type underlyingType = Enum.GetUnderlyingType(type);
                long lvalue = ConvertToLong(value, underlyingType);

                // have to iterate all the enum items and see if they are included in value
                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    long lv = ConvertToLong(fieldInfo.GetValue(null), underlyingType);

                    if (lv == 0 && lvalue != 0)
                        continue;

                    if ((lv & lvalue) == lv)
                    {
                        DescriptionAttribute[] attributes =
                            (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);

                        string description = (attributes.Length > 0)
                                                 ? attributes[0].Description
                                                 : fieldInfo.GetValue(null).ToString();
                        if (builder.Length > 0)
                            builder.Append(", ");
                        builder.Append(description);
                    }
                }

                return builder.Length == 0 ? value.ToString() : builder.ToString();
            } catch (Exception)
            {
                return value.ToString();
            }
        }

        private static long ConvertToLong(object value, Type underlyingType)
        {
            object underlyingValue = Convert.ChangeType(value, underlyingType);

            if (underlyingType == typeof(long))
                return (long)underlyingValue;

            return (long)Convert.ChangeType(underlyingValue, typeof(long));
        }

        public static string GetEnumDescription(System.Type value, string name)
        {
            FieldInfo fieldInfo = value.GetField(name);
            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : name;
        }

        public static object GetEnumValue(System.Type value, string description)
        {
            object returnValue = description;

            FieldInfo[] fieldInfos = value.GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0
                    && attributes[0].Description == description)
                {
                    returnValue = fieldInfo.GetValue(fieldInfo.Name);
                }
                else if (fieldInfo.Name == description)
                {
                    returnValue = fieldInfo.GetValue(fieldInfo.Name);
                }
            }

            return returnValue;
        }

        public EnumDescriptionConverter(System.Type type) : base(type.GetType())
        {
            myVal = type;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object returnValue;

            if (value is Enum && destinationType == typeof(string))
            {
                returnValue = GetEnumDescription((Enum)value);
            }
            else if (value is string && destinationType == typeof(string))
            {
                returnValue = GetEnumDescription(myVal, (string)value);
            }
            else
            {
                returnValue = base.ConvertTo(context, culture, value, destinationType);
            }

            return returnValue;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            object returnValue;

            if (value is string)
            {
                returnValue = GetEnumValue(myVal, (string)value);
            }
            else if (value is Enum)
            {
                returnValue = GetEnumDescription((Enum)value);
            }
            else
            {
                returnValue = base.ConvertFrom(context, culture, value);
            }

            return returnValue;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetValues(myVal));
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
}
}
