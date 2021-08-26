//------------------------------------------------------------------------------
// <copyright file="CollectionServicesSection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Configuration
{
    using System;
    using System.Configuration;

    public class CollectionServicesSection : ConfigurationSection
    {
        public const string SectionName = "Idera.SQLdm";
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CollectionServicesSection");

        [ConfigurationProperty("Services", IsDefaultCollection = false),
         ConfigurationCollection(typeof(CollectionServiceCollection), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public CollectionServiceCollection CollectionServices
        {
            get { return this["Services"] as CollectionServiceCollection; }
        }

        private static void AddSection(string instanceName)
        {
            using (LOG.DebugCall("AddSection"))
            {
                // update the local configuration file
                System.Configuration.Configuration configuration =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Get/Create the Idera.SQLdm configuration section
                CollectionServicesSection mss = configuration.GetSection(SectionName) as CollectionServicesSection;
                if (mss == null)
                {
                    mss = new CollectionServicesSection();
                    configuration.Sections.Add(SectionName, mss);
                }
                configuration.Save();
                LOG.DebugFormat("Added default section '{0}' to SQLdmCollectionService.exe.config.");
                Refresh();
            }
        }

        /// <summary>
        /// Get the 'Idera.SQLdm' section from the configuration file.  If the section does not exist 
        /// it is created.
        /// </summary>
        /// <returns></returns>
        public static CollectionServicesSection GetSection()
        {
            using (LOG.DebugCall("GetSection"))
            {
                CollectionServicesSection section = null;
                try
                {
                    section = ConfigurationManager.GetSection(SectionName) as CollectionServicesSection;
                    if (section == null)
                    {
                        LOG.Error("Section '" + SectionName +
                                  "' not found in config file.  The config file (SQLdmCollectionService.exe.config) does not exist or is corrupt.");
                        AddSection(SectionName);
                        section = ConfigurationManager.GetSection(SectionName) as CollectionServicesSection;
                    }
                }
                catch (ConfigurationErrorsException cee)
                {
                    LOG.Error(cee);
                }
                return section;
            }
        }

        public static void Refresh()
        {
            using (LOG.DebugCall("Refresh"))
            {
                ConfigurationManager.RefreshSection(SectionName);
            }
        }

    }
}
