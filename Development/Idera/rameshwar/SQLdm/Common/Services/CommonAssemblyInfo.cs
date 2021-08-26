//------------------------------------------------------------------------------
// <copyright file="CommonAssemblyInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Reflection;

    public sealed class CommonAssemblyInfo : ICommonAssemblyInfo
    {

        public string GetCommonAssemblyVersion()
        {
            //SQLdm 10.0 (vineet kumar) -- return hard coded value if entry assembly and executing assembly both are null to avoid unwanted exception
            if (Assembly.GetEntryAssembly() != null)
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            if (Assembly.GetExecutingAssembly() != null)
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            return "10.0.0";
        }

        public string GetCommonAssemblyInformationVersion()
        {
            AssemblyInformationalVersionAttribute aiva =
                GetAssemblyAttribute(typeof(AssemblyInformationalVersionAttribute)) 
                    as AssemblyInformationalVersionAttribute;

            return (aiva == null) ? "0.0.0.0" : aiva.InformationalVersion;
        }
        
        internal Attribute GetAssemblyAttribute(Type type)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            return Attribute.GetCustomAttribute(assembly, type);
        }

        public string GetCommonAssemblyCopyright()
        {
            //SQLdm 10.0 (vineet kumar) -- return hard coded value if entry assembly and executing assembly both are null to avoid unwanted exception
            if (Assembly.GetEntryAssembly() == null)
                return "Copyright Ignoring";
            object[] copyrightAttributes =
                Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute),false);
            if (copyrightAttributes.Length > 0 && copyrightAttributes[0].GetType() == typeof(AssemblyCopyrightAttribute))
            {
                return ((AssemblyCopyrightAttribute) copyrightAttributes[0]).Copyright;
            }
            return "";
        }
        
    }
}
