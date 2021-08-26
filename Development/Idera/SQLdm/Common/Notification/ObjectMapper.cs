//------------------------------------------------------------------------------
// <copyright file="ObjectMapper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Collections;

    /// <summary>
    /// Helper class for getting / setting object properties by reflection.
    /// </summary>
    public class ObjectMapper
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Objects to map.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static IDictionary<string, object> ObjectToMap(object target)
        {
            Type type = target.GetType();
            Dictionary<string, object> map = new Dictionary<string, object>();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                PropertyToMap(target, prop, map);
            }

            return map;
        }

        /// <summary>
        /// Properties to map.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="map">The map.</param>
        private static void PropertyToMap(object target, PropertyInfo prop, IDictionary<string, object> map)
        {
            if (prop.GetGetMethod(true) == null)
                return;

            if (IsSimple(prop))
            {
                map[prop.Name] = prop.GetValue(target, null);
            }
            /*
            if (IsCollection(prop))
            {
                //TODO: Tallyho!
            }
            */
        }

        /// <summary>
        /// Maps to object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public static bool MapToObject(object target, IDictionary<string, object> map)
        {
            Type type = target.GetType();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                MapToProperty(target, map, prop);
            }

            return true;
        }

        /// <summary>
        /// Maps to property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="map">The map.</param>
        /// <param name="prop">The prop.</param>
        private static void MapToProperty(object target, IDictionary<string, object> map, PropertyInfo prop)
        {
            if (prop.GetSetMethod(true) == null)
                return;

            if (IsSimple(prop))
            {
                object val = null;
                if (map.TryGetValue(prop.Name, out val))
                {
                    prop.SetValue(target, val, null);
                }
                else
                {
                    object[] attributes = prop.GetCustomAttributes(typeof(PropertyInfoAttribute), true);
                    if (attributes != null && attributes.Length == 1)
                    {
                        PropertyInfoAttribute pia = attributes[0] as PropertyInfoAttribute;
                        prop.SetValue(target, pia.DefaultValue, null);
                    }                        
                }
            }
            /*
            if (IsCollection(prop))
            {
                //TODO: Tallyho!
            }
            */
        }

        /// <summary>
        /// Determines whether the specified prop is simple.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        /// 	<c>true</c> if the specified prop is simple; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSimple(PropertyInfo prop)
        {
            //TODO: YUCK... fix me please
            return true;
        }

        /// <summary>
        /// Determines whether the specified prop is collection.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        /// 	<c>true</c> if the specified prop is collection; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCollection(PropertyInfo prop)
        {
            if (prop.PropertyType.GetInterface("System.Collections.ICollection") != null ||
                prop.PropertyType.GetInterface("System.Collections.Generic.ICollection") != null)
                return true;

            return false;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
