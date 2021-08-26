using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

using Idera.SQLdm.Common.Extensions;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    class SessionsHelper
    {
        //public static void ApplyFiltersToGrid(SessionsConfiguration configuration, UltraGrid grid, bool isHistory)
        //{
        //    UltraGridBand band = grid.DisplayLayout.Bands[0];

        //    grid.SuspendLayout();

        //    band.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
        //    //grid.Rows.ColumnFilters.ClearAllFilters();

        //    if (band.Columns.Exists("Application"))
        //    {
        //        grid.Rows.ColumnFilters["Application"].FilterConditions.Clear();

        //        if (configuration.ExcludeDiagnosticManagerProcesses)
        //        {
        //            grid.Rows.ColumnFilters["Application"].FilterConditions.Add(FilterComparisionOperator.DoesNotStartWith, "SQL diagnostic manager");
        //        }
        //    }

        //    //only do these manual filters for history browser
        //    if (!isHistory)
        //        return;

        //    //user only
        //    if (band.Columns.Exists("Type"))
        //    {
        //        grid.Rows.ColumnFilters["Type"].FilterConditions.Clear();

        //        if (configuration.ExcludeSystemProcesses)
        //        {
        //            grid.Rows.ColumnFilters["Type"].FilterConditions.Add(FilterComparisionOperator.NotEquals, DBNull.Value);
        //            grid.Rows.ColumnFilters["Type"].FilterConditions.Add(FilterComparisionOperator.NotEquals, "System");
        //            grid.Rows.ColumnFilters["Type"].LogicalOperator = FilterLogicalOperator.And;
        //        }
        //    }

        //    if (band.Columns.Exists("CPU"))
        //    {
        //        grid.Rows.ColumnFilters["CPU"].FilterConditions.Clear();

        //        if (configuration.ConsumingCpu)
        //        {
        //            grid.Rows.ColumnFilters["CPU"].FilterConditions.Add(FilterComparisionOperator.GreaterThan, 0);
        //        }
        //    }

        //    if (band.Columns.Exists("Blocked By"))
        //    {
        //        grid.Rows.ColumnFilters["Blocked By"].FilterConditions.Clear();

        //        if (configuration.Blocked || configuration.BlockingOrBlocked)
        //        {
        //            grid.Rows.ColumnFilters["Blocked By"].FilterConditions.Add(FilterComparisionOperator.GreaterThan, 0);
        //        }

        //        if (band.Columns.Exists("Blocking"))
        //        {
        //            grid.Rows.ColumnFilters["Blocking"].FilterConditions.Clear();

        //            if (configuration.BlockingBlocked)
        //            {
        //                grid.Rows.ColumnFilters["Blocking"].FilterConditions.Add(FilterComparisionOperator.Equals, false);
        //                grid.Rows.ColumnFilters["Blocked By"].FilterConditions.Add(FilterComparisionOperator.GreaterThan, 0);
        //            }

        //            if (configuration.BlockingOrBlocked)
        //            {
        //                grid.Rows.ColumnFilters["Blocking"].FilterConditions.Add(FilterComparisionOperator.Equals, false);
        //            }
        //        }

        //        if (band.Columns.Exists("Blocking Count"))
        //        {
        //            grid.Rows.ColumnFilters["Blocking Count"].FilterConditions.Clear();

        //            if (configuration.LeadBlockers)
        //            {
        //                grid.Rows.ColumnFilters["Blocking Count"].FilterConditions.Add(FilterComparisionOperator.LessThanOrEqualTo, 0);
        //                grid.Rows.ColumnFilters["Blocking Count"].LogicalOperator = FilterLogicalOperator.Or;
        //                grid.Rows.ColumnFilters["Blocked By"].FilterConditions.Add(FilterComparisionOperator.LessThanOrEqualTo, 0);
        //            }
        //        }

        //    }

        //    if (band.Columns.Exists("Open Transactions"))
        //    {
        //        grid.Rows.ColumnFilters["Open Transactions"].FilterConditions.Clear();

        //        if (configuration.OpenTransactions)
        //        {
        //            grid.Rows.ColumnFilters["Open Transactions"].FilterConditions.Add(FilterComparisionOperator.GreaterThan, 0);
        //        }
        //    }
        //}

        public static bool RowShouldBeFiltered(SessionsConfiguration configuration, UltraDataRow row, bool isHistory)
        {
            string value = string.Empty;

            if (configuration.ExcludeDiagnosticManagerProcesses &&
                row.Band.Columns.Exists("Application"))
            {
                if (row["Application"] != DBNull.Value)
                {
                    if (((string)(row["Application"])).StartsWith("SQL diagnostic manager"))
                        return true;
                }
            }

            //only do these manual filters for history browser
            if (!isHistory)
                return false;

            //user only
            if (configuration.ExcludeSystemProcesses &&
                row.Band.Columns.Exists("Type"))
            {
                if (row["Type"] == DBNull.Value ||
                    (string)row["Type"] == "System")
                {
                    return true;
                }
            }

            if (configuration.ConsumingCpu &&
                row.Band.Columns.Exists("CPU"))
            {
                if (row["CPU"] != DBNull.Value)
                {
                    if ((double)(row["CPU"]) <= 0)
                        return true;
                }
            }

            if (configuration.Blocked &&
                row.Band.Columns.Exists("Blocked By"))
            {

                if (row["Blocked By"] == DBNull.Value ||
                    (int)(row["Blocked By"]) <= 0)
                {
                    return true;
                }
            }

            if (configuration.BlockingBlocked &&
                row.Band.Columns.Exists("Blocking") &&
                row.Band.Columns.Exists("Blocked By"))
            {
                if (row["Blocking"] != DBNull.Value &&
                    row["Blocked By"] != DBNull.Value)
                {
                    if ((bool)(row["Blocking"]) &&
                        (int)(row["Blocked By"]) <= 0)
                        return true;
                }
                else
                {
                    return true;
                }
            }

            if (configuration.BlockingOrBlocked &&
                row.Band.Columns.Exists("Blocking") &&
                row.Band.Columns.Exists("Blocked By"))
            {
                if (row["Blocking"] == DBNull.Value ||
                    (bool)row["Blocking"] == false)
                {
                    return true;
                }

                if (row["Blocked By"] == DBNull.Value ||
                    (int)row["Blocked By"] <= 0)
                {
                    return true;
                }
            }

            if (configuration.OpenTransactions &&
                row.Band.Columns.Exists("Open Transactions"))
            {
                if (row["Open Transactions"] == DBNull.Value ||
                    (Int64)row["Open Transactions"] <= 0)
                {
                    return true;
                }
            }

            if (configuration.LeadBlockers &&
                row.Band.Columns.Exists("Blocking Count") &&
                row.Band.Columns.Exists("Blocked By"))
            {
                if (row["Blocking Count"] == DBNull.Value ||
                    row["Blocked By"] == DBNull.Value)
                {
                    return true;
                }
                else
                {
                    if ((int)row["Blocking Count"] > 0 &&
                        (int)row["Blocked By"] > 0)
                        return true;
                }
            }

            if (!string.IsNullOrWhiteSpace(configuration.ApplicationIncludeFilter) && 
                row.Band.Columns.Exists("Application"))
            {
                if (row["Application"] != DBNull.Value)
                {
                    if (!((string)(row["Application"])).ToLower().Like(configuration.ApplicationIncludeFilter.ToLower()))
                        return true;
                }
            
            }

            if (!string.IsNullOrWhiteSpace(configuration.ApplicationExcludeFilter) &&
                row.Band.Columns.Exists("Application"))
            {
                if (row["Application"] != DBNull.Value)
                {
                    if (((string)(row["Application"])).ToLower().Like(configuration.ApplicationExcludeFilter.ToLower()))
                        return true;
                }

            }

            if (!string.IsNullOrWhiteSpace(configuration.HostIncludeFilter) &&
                row.Band.Columns.Exists("Host"))
            {
                if (row["Host"] != DBNull.Value)
                {
                    if (!((string)(row["Host"])).ToLower().Like(configuration.HostIncludeFilter.ToLower()))
                        return true;
                }

            }

            if (!string.IsNullOrWhiteSpace(configuration.HostExcludeFilter) &&
                row.Band.Columns.Exists("Host"))
            {
                if (row["Host"] != DBNull.Value)
                {
                    if (((string)(row["Host"])).ToLower().Like(configuration.HostExcludeFilter.ToLower()))
                        return true;
                }

            }
            if (configuration.TempdbAffecting &&
                row.Band.Columns.Exists("Version Store Elapsed Seconds") &&
                row.Band.Columns.Exists("Current Session User Space (KB)") &&
                row.Band.Columns.Exists("Session User Space Allocated (KB)") &&
                row.Band.Columns.Exists("Session User Space Deallocated (KB)") &&
                row.Band.Columns.Exists("Current Task User Space (KB)") &&
                row.Band.Columns.Exists("Task User Space Allocated (KB)") &&
                row.Band.Columns.Exists("Task User Space Deallocated (KB)") &&
                row.Band.Columns.Exists("Current Session Internal Space (KB)") &&
                row.Band.Columns.Exists("Session Internal Space Allocated (KB)") &&
                row.Band.Columns.Exists("Session Internal Space Deallocated (KB)") &&
                row.Band.Columns.Exists("Current Task Internal Space (KB)") &&
                row.Band.Columns.Exists("Task Internal Space Allocated (KB)") &&
                row.Band.Columns.Exists("Task Internal Space Deallocated (KB)"))
            {
                if ((row["Version Store Elapsed Seconds"] == DBNull.Value || (double)row["Version Store Elapsed Seconds"] <= 0) &&
                    (row["Current Session User Space (KB)"] == DBNull.Value || (decimal)row["Current Session User Space (KB)"] <= 0) &&
                    (row["Session User Space Allocated (KB)"] == DBNull.Value || (decimal)row["Session User Space Allocated (KB)"] <= 0) &&
                    (row["Session User Space Deallocated (KB)"] == DBNull.Value || (decimal)row["Session User Space Deallocated (KB)"] <= 0) &&
                    (row["Current Task User Space (KB)"] == DBNull.Value || (decimal)row["Current Task User Space (KB)"] <= 0) &&
                    (row["Task User Space Allocated (KB)"] == DBNull.Value || (decimal)row["Task User Space Allocated (KB)"] <= 0) &&
                    (row["Task User Space Deallocated (KB)"] == DBNull.Value || (decimal)row["Task User Space Deallocated (KB)"] <= 0) &&
                    (row["Current Session Internal Space (KB)"] == DBNull.Value || (decimal)row["Current Session Internal Space (KB)"] <= 0) &&
                    (row["Session Internal Space Allocated (KB)"] == DBNull.Value || (decimal)row["Session Internal Space Allocated (KB)"] <= 0) &&
                    (row["Session Internal Space Deallocated (KB)"] == DBNull.Value || (decimal)row["Session Internal Space Deallocated (KB)"] <= 0) &&
                    (row["Current Task Internal Space (KB)"] == DBNull.Value || (decimal)row["Current Task Internal Space (KB)"] <= 0) &&
                    (row["Task Internal Space Allocated (KB)"] == DBNull.Value || (decimal)row["Task Internal Space Allocated (KB)"] <= 0) &&
                    (row["Task Internal Space Deallocated (KB)"] == DBNull.Value || (decimal)row["Task Internal Space Deallocated (KB)"] <= 0) )
                {
                    return true;
                }
                
            }

            return false;
        }
    }
}
