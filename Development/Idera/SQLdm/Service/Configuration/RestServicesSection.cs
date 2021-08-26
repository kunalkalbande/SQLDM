using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.Service.Configuration
{
    public class RestServicesSection : ConfigurationSection
    {
        public const string SectionName = "Idera.SQLdm";

        [ConfigurationProperty("Services", IsDefaultCollection = false),
         ConfigurationCollection(typeof(RestServiceCollection), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public RestServiceCollection RestServices
        {
            get { return this["Services"] as RestServiceCollection; }
        }

        private static void AddSection(string instanceName)
        {
            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get/Create the Idera.SQLdm configuration section
            RestServicesSection mss = configuration.GetSection(SectionName) as RestServicesSection;
            if (mss == null)
            {
                mss = new RestServicesSection();
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
        public static RestServicesSection GetSection()
        {
            RestServicesSection section = ConfigurationManager.GetSection(SectionName) as RestServicesSection;
            if (section == null)
            {
                AddSection(null);
                section = ConfigurationManager.GetSection(SectionName) as RestServicesSection;
            }
            return section;
        }

        public static void Refresh()
        {
            ConfigurationManager.RefreshSection(SectionName);
        }
    }    
}
