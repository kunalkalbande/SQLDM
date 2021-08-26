using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using System.Data.SqlClient;
using System.Data;
using Idera.SQLdm.Common.Data;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.ManagementService.Helpers
{
    internal partial class RepositoryHelper
    {
        public static Dictionary<int, List<Common.Configuration.BaselineConfiguration> > GetBaselineConfigurations(string connString)
        {
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("repositoryConnectionString");

            Dictionary<int, List<BaselineConfiguration> > baselineConfigurations = new Dictionary<int, List<BaselineConfiguration> >();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                //SQLdm 10.0 (Tarun Sapra) : Get all the active templates related to a SQLServerID
                using (SqlCommand command = new SqlCommand("SELECT SQLServerID, Template, TemplateID FROM BaselineTemplates WHERE Active=1", connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return new Dictionary<int, List<BaselineConfiguration> >(); //SQLdm 10.0 (Tarun Sapra) : returning list of baselines configs

                        int serverid = 0;

                        while (reader.Read())
                        {
                            serverid = reader.GetInt32(0);

                            BaselineConfiguration config = new BaselineConfiguration(reader.GetSqlString(1).ToString());
                            config.TemplateID = reader.GetInt32(2);

                            //START SQLdm 10.0 (Tarun Sapra) : Form a list of all the active config related to every server
                            if (baselineConfigurations.ContainsKey(serverid))
                                baselineConfigurations[serverid].Add(config);
                            else
                            {
                                List<BaselineConfiguration> tempList = new List<BaselineConfiguration>();
                                tempList.Add(config);
                                baselineConfigurations.Add(serverid, tempList);
                            }
                            //END SQLdm 10.0 (Tarun Sapra) : Form a list of all the active config related to every server
                        }
                    }
                }

                return baselineConfigurations;
            }
        }

        public static void CalculateBaseline(string connString, int serverid, int templateid, BaselineTemplate template)
        {
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("repositoryConnectionString");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                // get the baseline metadata for this server
                Dictionary<int, BaselineMetaDataItem> metaDataMap   = new Dictionary<int, BaselineMetaDataItem>();
                List<BaselineMetaDataItem>            metaDataItems = BaselineHelpers.GetBaselineMetaData(connection);

                foreach (BaselineMetaDataItem bmdi in metaDataItems)
                    metaDataMap.Add(bmdi.ItemId, bmdi);

                // Get time in UTC
                DateTime now = DateTime.UtcNow;

                // calculate the baselines
                List<BaselineItemData> baselineData = BaselineHelpers.ExecuteBaselineQuery(connection, serverid, false, now, metaDataMap, template);                

                // save        
                foreach (BaselineItemData bid in baselineData)
                {
                    BaselineMetaDataItem bmdi = bid.GetMetaData();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddBaselineStatistics", "@UTCCalculation", "@SQLServerID", "@TemplateID", "@MetricID", "@Mean", "@StdDeviation", "@Min", "@Max", "@Count"))
                    {
                        if (bmdi.MetricId == null || bid.Average == null || bid.Deviation == null || bid.Minimum == null || bid.Maximum == null || bid.Count == null)
                            continue;         

                        SqlHelper.AssignParameterValues(command.Parameters, 
                            now, 
                            serverid, 
                            templateid, 
                            bmdi.MetricId, 
                            bid.GetAverageUnscaled().Value,
                            bid.GetDeviationUnscaled().Value,
                            bid.Minimum.Value,
                            bid.Maximum.Value,
                            bid.Count.Value);                        

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Caught exception saving baseline statistics.", e);
                        }
                    }
                }
            }
        }

        public static void SaveBaselineTemplate(string connString, int serverid, BaselineConfiguration config)
        {
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("repositoryConnectionString");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddBaselineTemplate"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, config.Serialize());

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
