using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.PredictiveAnalyticsService.Configuration
{
    static class PredictiveAnalyticsConfiguration
    {
        private const int DEFAULT_ForecastBlockSize = 120; // number of minutes per forecast block of time (e.g., 60: 6am-7am, 120: 6am-8am, 180: 6am-9am)
        private const int DEFAULT_ForecastNumBlocks = 6;   // how many forecast blocks to build models for
        public  const int DEFAULT_ForecastInterval  = 60;  // how many minutes between forecasts

        private static DateTime DEFAULT_TrainingDataReferenceDateTime = new DateTime(2000, 1, 1, 0, 0, 0);  // Jan 1, 2000
        private static DateTime DEFAULT_ModelRebuildDateTime          = new DateTime(1900, 1, 1, 2, 0, 0);  // 2:00 AM
        private static DateTime DEFAULT_BaselineAnalysisDateTime      = new DateTime(1900, 1, 1, 1, 0, 0);  // 1:00 AM

        public static int ForecastBlockSize
        {
            get 
            {
                return DEFAULT_ForecastBlockSize; // override from config
            }
        }

        public static int ForecastNumBlocks
        {
            get 
            {
                return DEFAULT_ForecastNumBlocks; // override from config
            }
        }

        public static int ForecastInteval
        {
            get
            {
                return DEFAULT_ForecastInterval; // can override
            }
        }

        // This is the time used to convert datetimes to doubles (subtract the this date from the data to get a double)
        public static DateTime TrainingDataReferenceDateTime
        {
            get 
            {
                return DEFAULT_TrainingDataReferenceDateTime; 
            }
        }

        // This is the time that is used as the time from which models will be built (data up to this time is used)
        public static DateTime TrainingDataCutoffDateTime
        {
            get 
            {
                return DateTime.Now.ToUniversalTime(); // can override
            }
        }

        public static DateTime ModelRebuildDateTime
        {
            get
            {
                return PredictiveAnalyticsServiceSection.GetSection().ModelRebuildDateTime; // can override
            }
        }

        public static string ManagementServiceAddress
        {
            get
            {
                return PredictiveAnalyticsServiceSection.GetSection().ManagementServiceAddress;
            }

            set
            {
                PredictiveAnalyticsServiceSection.GetSection().ManagementServiceAddress = value;
            }
        }

        public static int ManagementServicePort
        {
            get
            {
                return PredictiveAnalyticsServiceSection.GetSection().ManagementServicePort;
            }

            set
            {
                PredictiveAnalyticsServiceSection.GetSection().ManagementServicePort = value;
            }
        }

        public static DateTime BaselineAnalysisDateTime
        {
            get 
            {
                PredictiveAnalyticsServiceSection section = PredictiveAnalyticsServiceSection.GetSection();
                return section!=null? section.BaselineAnalysisDateTime:DateTime.MinValue;
            }// can override
        }

        public static int GetPrescriptiveAnalysisSnapshotsInSeconds
        {
            get { return PredictiveAnalyticsServiceSection.GetSection().GetPrescriptiveAnalysisSnapshotsInSeconds; }
        }

        //Start - SQLdm 10.0 -(Praveen Suhalka) - added new property
        public static string CollectionServiceAddress
        {
            get
            {
                PredictiveAnalyticsServiceSection section = PredictiveAnalyticsServiceSection.GetSection();
                return section!=null? section.CollectionServiceAddress:string.Empty;
            }

            set
            {
                PredictiveAnalyticsServiceSection.GetSection().CollectionServiceAddress = value;
            }
        }

        public static int CollectionServicePort
        {
            get
            {
                return PredictiveAnalyticsServiceSection.GetSection().CollectionServicePort;
            }

            set
            {
                PredictiveAnalyticsServiceSection.GetSection().CollectionServicePort = value;
            }
        }
        //End - SQLdm 10.0 -(Praveen Suhalka) - added new property
    }

    internal sealed class PredictiveAnalyticsServiceSection : ConfigurationSection
    {
        public const string SectionName = "Idera.SQLdm";
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("PredictiveServiceSection");
        private static object sync = new object();
        private const string GetPrescriptiveAnalysisSnapshotsInSecondsKey = "getPrescriptiveAnalysisSnapshotsInSeconds";

        [ConfigurationProperty("ManagementServiceAddress", DefaultValue = Program.OPTION_MANAGEMENT_HOST_DEFAULT, IsRequired = true)]
        public string ManagementServiceAddress
        {
            get { return (string)this["ManagementServiceAddress"]; }
            set { this["ManagementServiceAddress"] = value; Save(); }
        }

        [ConfigurationProperty("ManagementServicePort", DefaultValue = (int)Program.OPTION_MANAGEMENT_PORT_DEFAULT_INT, IsRequired = true)]
        public int ManagementServicePort
        {
            get { return (int)this["ManagementServicePort"]; }
            set { this["ManagementServicePort"] = value; Save(); }
        }

        // ModelRebuildDateTime
        [ConfigurationProperty("ModelRebuildDateTime", DefaultValue = "1900/1/1 02:00:00", IsRequired = false)]
        public DateTime ModelRebuildDateTime
        {
            get { return (DateTime)this["ModelRebuildDateTime"]; }
            set { this["ModelRebuildDateTime"] = value; Save(); }
        }

        // ForecastInteval
        [ConfigurationProperty("ForecastInteval", DefaultValue = (int)PredictiveAnalyticsConfiguration.DEFAULT_ForecastInterval, IsRequired = false)]
        public int ForecastInteval
        {
            get { return (int)this["ForecastInteval"]; }
            set { this["ForecastInteval"] = value; Save(); }
        }

        // BaselineAnalysisDateTime
        [ConfigurationProperty("BaselineAnalysisDateTime", DefaultValue = "1900/1/1 01:00:00", IsRequired = false)]
        public DateTime BaselineAnalysisDateTime
        {
            get { return (DateTime)this["BaselineAnalysisDateTime"]; }
            set { this["BaselineAnalysisDateTime"] = value; Save(); }
        }

        //Start - SQLdm 10.0 -(Praveen Suhalka) - added new property
        [ConfigurationProperty("CollectionServiceAddress", DefaultValue = Program.OPTION_COLLECTION_HOST_DEFAULT, IsRequired = false)]
        public string CollectionServiceAddress
        {
            get { return (string)this["CollectionServiceAddress"]; }
            set { this["CollectionServiceAddress"] = value; Save(); }
        }

        [ConfigurationProperty("CollectionServicePort", DefaultValue = (int)Program.OPTION_COLLECTION_PORT_DEFAULT_INT, IsRequired = false)]
        public int CollectionServicePort
        {
            get { return (int)this["CollectionServicePort"]; }
            set { this["CollectionServicePort"] = value; Save(); }
        }

        [ConfigurationProperty(GetPrescriptiveAnalysisSnapshotsInSecondsKey, DefaultValue = 500)]
        public int GetPrescriptiveAnalysisSnapshotsInSeconds
        {
            get { return (int)this[GetPrescriptiveAnalysisSnapshotsInSecondsKey]; }
            set { this[GetPrescriptiveAnalysisSnapshotsInSecondsKey] = value; }
        }
        //End - SQLdm 10.0 -(Praveen Suhalka) - added new property

        public static PredictiveAnalyticsServiceSection GetSection()
        {
            PredictiveAnalyticsServiceSection section = null;
            try
            {
                lock (sync)
                {
                    section = ConfigurationManager.GetSection(SectionName) as PredictiveAnalyticsServiceSection;
                }
                if (section == null)
                {
                    LOG.Error("Section '" + SectionName + "' not found in config file.  The config file (SQLdmPredictiveAnalyticsService.exe.config) does not exist or is corrupt.");
                }
                //Start : dm 10.0 de46203 (vineet) -- AddSection method is already handling if the section exists or not. So moving it outside to enforce the default properties of sections
                AddSection(SectionName);
                section = ConfigurationManager.GetSection(SectionName) as PredictiveAnalyticsServiceSection;
            }
            catch (ConfigurationErrorsException cee)
            {
                LOG.Error(cee);
            }

            return section;
        }

        private static void AddSection(string instanceName)
        {
            using (LOG.DebugCall("AddUpdateSection"))
            {
                PredictiveAnalyticsServiceSection section = null;
                System.Configuration.Configuration configuration = null;
                // update the local configuration file
                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Get/Create the Idera.SQLdm configuration section
                section = configuration.GetSection(SectionName) as PredictiveAnalyticsServiceSection;

                if (section != null)
                {
                    return; // The section already exists.
                }

                lock (sync)
                {
                    // Get/Create the Idera.SQLdm configuration section
                    section = configuration.GetSection(SectionName) as PredictiveAnalyticsServiceSection;

                    if (section == null)
                    {
                        section = new PredictiveAnalyticsServiceSection();
                        configuration.Sections.Add(SectionName, section);
                        LOG.DebugFormat("Added default section '{0}' to SQLdmPredictiveAnalyticsService.exe.config.", SectionName);//10.0 Moved it to correct place

                        //All the Properties should be initialized/invoked here.  Currently the below setters calls the Save() method
                        section.CollectionServiceAddress = section.CollectionServiceAddress;
                        section.CollectionServicePort = section.CollectionServicePort;

                        configuration.Save();
                        ConfigurationManager.RefreshSection(SectionName);
                    }
                }
            }
        }

        private static void Save()
        {
            lock (sync)
            {
                System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.Save();
                ConfigurationManager.RefreshSection(SectionName);
            }
        }
    }
}
