using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Idera.SQLdm.Common.Auditing
{
    public class AuditTools
    {
        public static string GetEnumDescription(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            if (attributes != null &&
                attributes.Length > 0)
                return (T)attributes[0];
            else
                return null;
        }

        public static List<T> Except<T>(List<T> source, List<T> exceptList)
        {
            List<T> result = new List<T>();

            foreach (var value in source)
            {
                if (!exceptList.Contains(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        public static bool CompareByItem<T>(List<T> source, List<T> exceptList)
        {
            bool result = !(source.Count <= 0 && exceptList.Count > 0);

            List<T> maxList = source.Count >= exceptList.Count ? source : exceptList;
            List<T> minList = source.Count >= exceptList.Count ? exceptList : source;

            foreach (var value in maxList)
            {
                if (!minList.Contains(value))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Return the opposite of the short value, the bits are changed to opposite
        /// Example: 10101010 result 01010101
        /// </summary>
        /// <param name="MaintenanceModeDays"></param>
        /// <returns></returns>
        internal static short? Oposite(short? MaintenanceModeDays)
        {
            short[] index = new short[] {0, 1, 4, 8, 16, 32, 64, 128};

            short result = 0;

            foreach (var i in index)
            {
                if ((MaintenanceModeDays & i) != i)
                {
                    result |= i;
                }
            }

            return result;
        }

        /// <summary>
        /// Validate if one of the data is null return false and the ignore = true return false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        internal static bool Equals<T>(T newValue, T oldValue,bool ignore)
        {
            if (newValue == null || oldValue == null || ignore)
            {
                return false;
            }          
            
            return newValue.Equals(oldValue);
        }
    }
}
