//------------------------------------------------------------------------------
// <copyright file="RegistrationServicesSection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.RegistrationService.Configuration
{
    public class RegistrationServicesSection : ConfigurationSection
    {
        public const string SectionName = "Idera.SQLdm";

        [ConfigurationProperty("Services", IsDefaultCollection = false),
         ConfigurationCollection(typeof(RegistrationServiceCollection), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public RegistrationServiceCollection RegistrationServices
        {
            get { return this["Services"] as RegistrationServiceCollection; }
        }

        private static void AddSection(string instanceName)
        {
            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get/Create the Idera.SQLdm configuration section
            RegistrationServicesSection mss = configuration.GetSection(SectionName) as RegistrationServicesSection;
            if (mss == null)
            {
                mss = new RegistrationServicesSection();
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
        public static RegistrationServicesSection GetSection()
        {
            RegistrationServicesSection section = ConfigurationManager.GetSection(SectionName) as RegistrationServicesSection;
            if (section == null)
            {
                AddSection(null);
                section = ConfigurationManager.GetSection(SectionName) as RegistrationServicesSection;
            }
            return section;
        }

        public static void Refresh()
        {
            ConfigurationManager.RefreshSection(SectionName);
        }

    }
}
