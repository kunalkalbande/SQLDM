using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Monitoring;
using Microsoft.Azure.Management.Monitor.Models;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Timers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.ManagementService.Helpers
{
    internal static class ApplicationHelper
    {
        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Enum Helpers

        internal static string GetEnumDescription(object o)
        {
            System.Type otype = o.GetType();
            if (otype.IsEnum)
            {
                FieldInfo field = otype.GetField(Enum.GetName(otype, o));
                if (field != null)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return o.ToString();
        }


		#endregion

		#region Recommendation helper
		  //SQLDM-30528
        internal static void GenerateRecommendationAlert(int interval)
        {


            Timer t = new Timer(TimeSpan.FromMinutes(interval).TotalMilliseconds); // Set the time (5 mins in this case)
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(AlertGenerator);
            t.Start();
        }

        internal static void AlertGenerator(object sender, ElapsedEventArgs e)
        {
            bool useDefaults;
            DateTime startDate;
            DateTime endDate;
            DateTime? earliestAvailableData;
            short days;

            Pair<Dictionary<int, BaselineItemData>, List<BaselineItemData>> result;

            List<BaselineItemData> newRecommendations = null;

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                connection.Open();

                Dictionary<int, BaselineItemData> newBaseline = new Dictionary<int, BaselineItemData>();
                List<BaselineItemData> recommendations = new List<BaselineItemData>();


                var monitoredServers = RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
                foreach (var monitoredServer in monitoredServers)
                {
                    var configuration = RepositoryHelper.GetAlertConfiguration(ManagementServiceConfiguration.GetRepositoryConnection(), monitoredServer);

                    foreach (BaselineItemData item in BaselineHelpers.GetBaseline(connection, monitoredServer.Id, true))
                    {
                        int? metricId = item.GetMetaData().MetricId;
                        if (metricId.HasValue)
                        {
                            if (!newBaseline.ContainsKey(metricId.Value))
                            {
                                newBaseline.Add(metricId.Value, item);
                            }
                        }
                    }

                    BaselineHelpers.GetBaselineParameters(connection,
                                                monitoredServer.Id,
                                                out useDefaults,
                                                out startDate,
                                                out endDate,
                                                out days,
                                                out earliestAvailableData);
                    // don't bother getting recommendations if we don't have 24 hours worth of data collected                
                    if (earliestAvailableData.HasValue && (DateTime.UtcNow - earliestAvailableData.Value) >= TimeSpan.FromHours(24))
                    {
                        newRecommendations = BaselineHelpers.FilterRecommendations(configuration, newBaseline.Values);
                    }

                    result = new Pair<Dictionary<int, BaselineItemData>, List<BaselineItemData>>(newBaseline, newRecommendations);

                    if (result.Second != null)
                        recommendations = result.Second;
                    else
                        recommendations.Clear();

                    if(recommendations.Count > 0)
                    {
                        AlertTableWriter.LogOperationalAlerts(Common.Events.Metric.Operational, new MonitoredObjectName(monitoredServer.DisplayInstanceName), MonitoredState.Warning, "Recommendations Are Available",
                            "Recommendations for alerts are available for SQLServer = " + monitoredServer.DisplayInstanceName + ", please review thresholds");
                    }

                }

            }  
        }
		
		internal static List<MasterRecommendation> GetMasterRecommondation()
		{
			var materRecomm = new List<MasterRecommendation>();

			var values = Enum.GetValues(typeof(PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.RecommendationType));
			foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.RecommendationType value in values)
			{
				var recommendationId = FindingIdAttribute.GetFindingId(value);

				if (string.IsNullOrEmpty(recommendationId)) continue;

				try
				{
					var recomm = new MasterRecommendation(recommendationId)
					{
						Description = SQLdoctorResources.GetDescription(recommendationId),
						Bitly = SQLdoctorResources.GetBitly(recommendationId),
						Additional_Considerations = SQLdoctorResources.GetAdditionalConsiderations(recommendationId),
						Category = SQLdoctorResources.GetCategory(recommendationId),
						Confidence_Factor = SQLdoctorResources.GetConfidenceFactor(recommendationId),
						Finding = SQLdoctorResources.GetFinding(recommendationId),
						Impact_Explanation = SQLdoctorResources.GetImpactExplanation(recommendationId),
						Impact_Factor = SQLdoctorResources.GetImpactFactor(recommendationId),
						Plural_Form_Finding = SQLdoctorResources.GetPluralFinding(recommendationId),
						Plural_Form_Impact_Explanation = SQLdoctorResources.GetPluralImpactExplanation(recommendationId),
						Plural_Form_Recommendation = SQLdoctorResources.GetPluralRecommendation(recommendationId),
						Problem_Explanation = SQLdoctorResources.GetProblemExplanation(recommendationId),
						Recommendation = SQLdoctorResources.GetRecommendation(recommendationId),
						RecommendationID = recommendationId,
						Relevance = SQLdoctorResources.GetRelevance(recommendationId),
						Tags = SQLdoctorResources.GetTags(recommendationId)
					};

					var links = SQLdoctorResources.GetInfoLinks(recommendationId);
					recomm.InfoLinks = string.Empty;

					if (links != null)
					{
						recomm.InfoLinks = new RecommendationLinksConverter().ConvertToInvariantString(links); //string.Join("\r\n", links.Select(x => x.Link));
					}
					materRecomm.Add(recomm);
				}
				catch (Exception ex)
				{

				}
			}
			return materRecomm;
		}
		#endregion
	}
}
