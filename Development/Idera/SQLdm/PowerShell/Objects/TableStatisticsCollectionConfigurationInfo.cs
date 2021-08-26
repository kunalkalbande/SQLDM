//------------------------------------------------------------------------------
// <copyright file="TableStatisticsCollectionConfigurationInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;

    public class TableStatisticsCollectionConfigurationInfo : ICloneable
    {
        private const int ONEK = 1024;
        private const int PAGE = ONEK*8;

        private Days tableCollectionDays;
        private DateTime tableCollectionStartTime;
        private int reorgMinimumTableSizeK;
        private List<string> excludedDatabases;

        public TableStatisticsCollectionConfigurationInfo()
        {
            tableCollectionDays = Days.Monday | Days.Tuesday | Days.Wednesday | Days.Thursday | Days.Friday;
            tableCollectionStartTime = new DateTime(1900, 1, 1, 3, 0, 0);
            reorgMinimumTableSizeK = 8000;
        }

        public TableStatisticsCollectionConfigurationInfo(TableStatisticsCollectionConfigurationInfo copy)
        {
            tableCollectionDays = copy.tableCollectionDays;
            tableCollectionStartTime = copy.tableCollectionStartTime;
            reorgMinimumTableSizeK = copy.reorgMinimumTableSizeK;
            ExcludedDatabases.AddRange(copy.ExcludedDatabases);
        }

        public TableStatisticsCollectionConfigurationInfo(MonitoredSqlServerConfiguration config)
        {
            tableCollectionDays = (Days)config.ReorgStatisticsDays;
            if (config.ReorgStatisticsStartTime.HasValue)
                tableCollectionStartTime = config.ReorgStatisticsStartTime.Value;
            if (config.ReorganizationMinimumTableSize.Kilobytes.HasValue)
                reorgMinimumTableSizeK = Convert.ToInt32(config.ReorganizationMinimumTableSize.Kilobytes.Value);
            if (config.TableStatisticsExcludedDatabases != null && config.TableStatisticsExcludedDatabases.Count > 0)
                ExcludedDatabases.AddRange(config.TableStatisticsExcludedDatabases);
        }
        public TableStatisticsCollectionConfigurationInfo(MonitoredSqlServer server)
        {
            tableCollectionDays = (Days)server.TableFragmentationConfiguration.FragmentationStatisticsDays;
            if (server.TableFragmentationConfiguration.FragmentationStatisticsStartTime.HasValue)
                tableCollectionStartTime = server.TableFragmentationConfiguration.FragmentationStatisticsStartTime.Value;
            if (server.ReorganizationMinimumTableSize.Kilobytes.HasValue)
                reorgMinimumTableSizeK = Convert.ToInt32(server.ReorganizationMinimumTableSize.Kilobytes.Value);
            if (server.TableGrowthConfiguration.TableStatisticsExcludedDatabases != null && server.TableGrowthConfiguration.TableStatisticsExcludedDatabases.Count > 0)
                ExcludedDatabases.AddRange(server.TableGrowthConfiguration.TableStatisticsExcludedDatabases);
        }

        public Days CollectionDays
        {
            get { return tableCollectionDays; }
            set { tableCollectionDays = value; }
        }

        public TimeSpan CollectionStartTime
        {
            get { return tableCollectionStartTime.TimeOfDay; }
            set { tableCollectionStartTime = SQLdmProvider.Jan_1_1900 + value; }
        }

        public int ReorgMinimumTableSizeK
        {
            get { return reorgMinimumTableSizeK; }
            set { reorgMinimumTableSizeK = value; }
        }

        public List<string> ExcludedDatabases
        {
            get
            {
                if (excludedDatabases == null)
                    excludedDatabases = new List<string>();

                return excludedDatabases;
            }
            internal set { excludedDatabases = value; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return new TableStatisticsCollectionConfigurationInfo(this);
        }

        #endregion
    }
}
