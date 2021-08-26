using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Idera.SQLdm.Service.Core.Enums
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            try
            {
                FieldInfo field = value.GetType().GetField(value.ToString());

                DescriptionAttribute attribute
                        = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                            as DescriptionAttribute;

                return attribute == null ? value.ToString() : attribute.Description;
            }
            catch { return (value.ToString()); }
        }
    }
}
