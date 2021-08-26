//------------------------------------------------------------------------------
// <copyright file="ManagementServicesSection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.ManagementService.Configuration
{
    public class ManagementServicesSection : ConfigurationSection
    {
        public const string SectionName = "Idera.SQLdm";

        [ConfigurationProperty("Services", IsDefaultCollection = false),
         ConfigurationCollection(typeof(ManagementServiceCollection), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public ManagementServiceCollection ManagementServices
        {
            get { return this["Services"] as ManagementServiceCollection; }
        }

        private static void AddSection(string instanceName)
        {
            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get/Create the Idera.SQLdm configuration section
            ManagementServicesSection mss = configuration.GetSection(SectionName) as ManagementServicesSection;
            if (mss == null)
            {
                mss = new ManagementServicesSection();
                configuration.Sections.Add(SectionName, mss);
            }
            configuration.Save();

            Refresh();
        }

        /// <summary>
        /// Get the 'Idera.SQLdm' section from the configuration file.  If the section does not exist 
        /// it is created.
        /// </summary>
        /// <returns></returns>
        public static ManagementServicesSection GetSection()
        {
            ManagementServicesSection section = ConfigurationManager.GetSection(SectionName) as ManagementServicesSection;
            if (section == null)
            {
                AddSection(null);
                section = ConfigurationManager.GetSection(SectionName) as ManagementServicesSection;
            }
            return section;
        }

        public static void Refresh()
        {
            ConfigurationManager.RefreshSection(SectionName);
        }

    }
}
