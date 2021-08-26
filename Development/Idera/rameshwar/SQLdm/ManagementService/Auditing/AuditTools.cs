using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Idera.SQLdm.ManagementService.Auditing
{
    class AuditTools
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
    }
}
