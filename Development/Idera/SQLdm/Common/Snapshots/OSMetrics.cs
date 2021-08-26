//------------------------------------------------------------------------------
// <copyright file="OSMetrics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents OS Metrics for a given server
    /// </summary>
    [Serializable]
    public sealed class OSMetrics
    {
        #region fields

        //These are the previous values from the SQL Server used for calculation
        private double? pagesPersec_Raw = null;
        private double? percentProcessorTime_Raw = null;
        private double? percentSQLProcessorTime_Raw = null;
        private double? percentPrivilegedTime_Raw = null;
        private double? percentUserTime_Raw = null;
        private double? percentDiskIdleTime_Raw = null;
        private double? avgDiskQueueLength_Raw = null;
        private double? timeStamp_Sys100NS_Raw = null;
        private double? timeStamp_PerfTime_Raw = null;
        private double? percentDiskIdleTimeBase_Raw = null;

        //These are the actual values for display and alerting

        private FileSize totalPhysicalMemory = new FileSize(); // This is written to the database non-calculated
        private FileSize availableBytes = new FileSize(); // This is written to the database non-calculated
        private double? pagesPersec = null;
        private double? percentProcessorTime = null;
        private double? percentSQLProcessorTime = null;
        private double? percentPrivilegedTime = null;
        private double? percentUserTime = null;
        //10.0 SQLdm Srishti Purohit -- baseline mean comparision change
        private double? percUserTimeBaseline;
        private double? userTimeBaselineDeviationPerc;

        private double? processorQueueLength = null; // This is written to the database non-calculated
        private double? percentDiskIdleTime = null;
        private double? avgDiskQueueLength = null;

        private double? timeStamp_Sys100NS_Delta = null;
        private double? timeStamp_PerfTime_Delta = null;
        private double? percentDiskIdleTimeBase_Delta = null;

        private DateTime? timestampUTC = null;

        //We also need to write this calculation constant to the database
        private double? frequency_PerfTime = null; // This is written to the database non-calculated

        ////Keep track of whether we have data available to cook with
        //private bool readyToCook = false;
        //And whether deltas are ready for the database
        private bool readyToWrite = false;

        private string osStatisticAvailability = null;

        private double? diffFromExpectedTimeStampError = 0;
        private string diffFromExpectedTimeStampName = "";

        //OsStatisticAvailability possible texts
        private const string ServiceAvailableText = "available";
        private const string ServiceTimedOutText = "service timedout";
        private const string ServiceUnavailableText = "service unavailable";
        private const string ProcedureUnavailableText = "procedure unavailable";
        private const string LightweightPoolingText = "lightweight pooling";
        private const string UnavailableDueToLWPText = "unavailable due to lightweight pooling"; // LWP = Lightweight pooling

        //START: SQLdm 10.0 (Tarun Sapra)- New fields for baseline mean and perc
        private double? oSMemoryUsagePctBaselineMean = null;
        private double? oSMemoryUsagePctAsBaselinePerc = null;

        private double? oSUserCPUUsagePctBaselineMean = null;
        private double? oSUserCPUUsagePctAsBaselinePerc = null;

        private double? oSMemoryPagesPerSecondBaselineMean = null;
        private double? oSMemoryPagesPerSecondAsBaselinePerc = null;

        private double? oSCPUUsagePctBaselineMean = null;
        private double? oSCPUUsagePctAsBaselinePerc = null;

        private double? oSCPUPrivilegedTimePctBaselineMean = null;
        private double? oSCPUPrivilegedTimePctAsBaselinePerc = null;

        private double? oSCPUProcessorQueueLengthBaselineMean = null;
        private double? oSCPUProcessorQueueLengthAsBaselinePerc = null;

        private double? oSDiskPhysicalDiskTimePctBaselineMean = null;
        private double? oSDiskPhysicalDiskTimePctAsBaselinePerc = null;

        private double? oSDiskAverageDiskQueueLengthBaselineMean = null;
        private double? oSDiskAverageDiskQueueLengthAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- New fields for baseline mean and perc

        #endregion

        #region constructors

        #endregion

        #region properties

        //START: SQLdm 10.0 (Tarun Sapra)- New fields for baseline mean and perc
        public double? OSMemoryUsagePctBaselineMean
        {
            get { return oSMemoryUsagePctBaselineMean; }
            set { oSMemoryUsagePctBaselineMean = value; }
        }
        public double? OSMemoryUsagePctAsBaselinePerc
        {
            get { return oSMemoryUsagePctAsBaselinePerc; }
            set { oSMemoryUsagePctAsBaselinePerc = value; }
        }

        public double? OSUserCPUUsagePctBaselineMean
        {
            get { return oSUserCPUUsagePctBaselineMean; }
            set { oSUserCPUUsagePctBaselineMean = value; }
        }
        public double? OSUserCPUUsagePctAsBaselinePerc
        {
            get { return oSUserCPUUsagePctAsBaselinePerc; }
            set { oSUserCPUUsagePctAsBaselinePerc = value; }
        }

        public double? OSMemoryPagesPerSecondBaselineMean
        {
            get { return oSMemoryPagesPerSecondBaselineMean; }
            set { oSMemoryPagesPerSecondBaselineMean = value; }
        }
        public double? OSMemoryPagesPerSecondAsBaselinePerc
        {
            get { return oSMemoryPagesPerSecondAsBaselinePerc; }
            set { oSMemoryPagesPerSecondAsBaselinePerc = value; }
        }

        public double? OSCPUUsagePctBaselineMean
        {
            get { return oSCPUUsagePctBaselineMean; }
            set { oSCPUUsagePctBaselineMean = value; }
        }
        public double? OSCPUUsagePctAsBaselinePerc
        {
            get { return oSCPUUsagePctAsBaselinePerc; }
            set { oSCPUUsagePctAsBaselinePerc = value; }
        }

        public double? OSCPUPrivilegedTimePctBaselineMean
        {
            get { return oSCPUPrivilegedTimePctBaselineMean; }
            set { oSCPUPrivilegedTimePctBaselineMean = value; }
        }
        public double? OSCPUPrivilegedTimePctAsBaselinePerc
        {
            get { return oSCPUPrivilegedTimePctAsBaselinePerc; }
            set { oSCPUPrivilegedTimePctAsBaselinePerc = value; }
        }

        public double? OSCPUProcessorQueueLengthBaselineMean
        {
            get { return oSCPUProcessorQueueLengthBaselineMean; }
            set { oSCPUProcessorQueueLengthBaselineMean = value; }
        }
        public double? OSCPUProcessorQueueLengthAsBaselinePerc
        {
            get { return oSCPUProcessorQueueLengthAsBaselinePerc; }
            set { oSCPUProcessorQueueLengthAsBaselinePerc = value; }
        }

        public double? OSDiskPhysicalDiskTimePctBaselineMean
        {
            get { return oSDiskPhysicalDiskTimePctBaselineMean; }
            set { oSDiskPhysicalDiskTimePctBaselineMean = value; }
        }
        public double? OSDiskPhysicalDiskTimePctAsBaselinePerc
        {
            get { return oSDiskPhysicalDiskTimePctAsBaselinePerc; }
            set { oSDiskPhysicalDiskTimePctAsBaselinePerc = value; }
        }

        public double? OSDiskAverageDiskQueueLengthBaselineMean
        {
            get { return oSDiskAverageDiskQueueLengthBaselineMean; }
            set { oSDiskAverageDiskQueueLengthBaselineMean = value; }
        }
        public double? OSDiskAverageDiskQueueLengthAsBaselinePerc
        {
            get { return oSDiskAverageDiskQueueLengthAsBaselinePerc; }
            set { oSDiskAverageDiskQueueLengthAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- New fields for baseline mean and perc

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfOS_Memory:PagesPersec - Not for display
        /// </summary>
        private double? PagesPersec_Raw
        {
            get { return pagesPersec_Raw; }
            set { pagesPersec_Raw = value; }
        }

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfOS_Processor:PercentProcessorTime - Not for display
        /// </summary>
        private double? PercentProcessorTime_Raw
        {
            get { return percentProcessorTime_Raw; }
            set { percentProcessorTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of win32_PerfRawData_PerfProc_Process:PercentProcessorTime - Not for display
        /// </summary>
        private double? PercentSQLProcessorTime_Raw
        {
            get { return percentSQLProcessorTime_Raw; }
            set { percentSQLProcessorTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfOS_Processor:PercentPrivilegedTime - Not for display
        /// </summary>
        private double? PercentPrivilegedTime_Raw
        {
            get { return percentPrivilegedTime_Raw; }
            set { percentPrivilegedTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfOS_Processor:PercentUserTime - Not for display
        /// </summary>
        private double? PercentUserTime_Raw
        {
            get { return percentUserTime_Raw; }
            set { percentUserTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfDisk_PhysicalDisk:PercentIdleTime - Not for display
        /// </summary>
        private double? PercentDiskIdleTime_Raw
        {
            get { return percentDiskIdleTime_Raw; }
            set { percentDiskIdleTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of Win32_PerfRawData_PerfDisk_PhysicalDisk:AvgDiskQueueLength - Not for display
        /// </summary>
        private double? AvgDiskQueueLength_Raw
        {
            get { return avgDiskQueueLength_Raw; }
            set { avgDiskQueueLength_Raw = value; }
        }

        /// <summary>
        /// Raw value of TimeStamp_Sys100NS - Not for display
        /// </summary>
        private double? TimeStamp_Sys100NS_Raw
        {
            get { return timeStamp_Sys100NS_Raw; }
            set { timeStamp_Sys100NS_Raw = value; }
        }

        /// <summary>
        /// Raw value of TimeStamp_Sys100NS - Not for display
        /// </summary>
        private double? TimeStamp_PerfTime_Raw
        {
            get { return timeStamp_PerfTime_Raw; }
            set { timeStamp_PerfTime_Raw = value; }
        }

        /// <summary>
        /// Raw value of PercentDiskIdleTime_Base - Not for display 
        /// </summary>
        private double? PercentDiskIdleTimeBase_Raw
        {
            get { return percentDiskIdleTimeBase_Raw; }
            set { percentDiskIdleTimeBase_Raw = value; }
        }

        /// <summary>
        /// Total physical memory on the target server
        /// </summary>
        public FileSize TotalPhysicalMemory
        {
            get { return totalPhysicalMemory; }
            internal set { totalPhysicalMemory = value; }
        }

        /// <summary>
        /// Total unused memory on the target server
        /// </summary>
        public FileSize AvailableBytes
        {
            get { return availableBytes; }
            internal set { availableBytes = value; }
        }

        /// <summary>
        /// Operating system pages per second
        /// </summary>
        public double? PagesPersec
        {
            get { return pagesPersec; }
            internal set { pagesPersec = value; }
        }

        /// <summary>
        /// Operating system percent processor time
        /// </summary>
        public double? PercentProcessorTime
        {
            get
            {
                if (!percentProcessorTime.HasValue)
                    return null;
                if (percentProcessorTime < 100)
                {
                    if (percentProcessorTime < 0)
                        return 0;
                    else
                        return percentProcessorTime;
                }
                else
                {
                    return 100;
                }
            }
            internal set { percentProcessorTime = value; }
        }

        /// <summary>
        /// SQL server percent processor time
        /// </summary>
        public double? PercentSQLProcessorTime
        {
            get
            {
                if (!percentSQLProcessorTime.HasValue)
                    return null;
                else
                    return percentSQLProcessorTime;
            }
            internal set { percentSQLProcessorTime = value; }
        }

        /// <summary>
        /// Operating system percent privileged time
        /// </summary>
        public double? PercentPrivilegedTime
        {
            get
            {
                if (!percentPrivilegedTime.HasValue)
                    return null;
                if (percentPrivilegedTime < 100)
                {
                    if (percentPrivilegedTime < 0)
                        return 0;
                    else
                        return percentPrivilegedTime;
                }
                else
                {
                    return 100;
                }
            }
            internal set { percentPrivilegedTime = value; }
        }

        /// <summary>
        /// Operating system percent user time
        /// </summary>
        public double? PercentUserTime
        {
            get
            {
                if (!percentUserTime.HasValue)
                    return null;
                if (percentUserTime < 100)
                {
                    if (percentUserTime < 0)
                        return 0;
                    else
                        return percentUserTime;
                }
                else
                {
                    return 100;
                }
            }
            internal set { percentUserTime = value; }
        }
        public double? OSMemoryPerUsageBaselineMean
        {
            get { return percUserTimeBaseline; }
            internal set { percUserTimeBaseline = value; }
        }

        public double? MemoryUsagePercDeviationBaseine
        {
            get { return userTimeBaselineDeviationPerc; }
            set { userTimeBaselineDeviationPerc = value; }
        }
        /// <summary>
        /// Operating system processor queue length
        /// </summary>
        public double? ProcessorQueueLength
        {
            get { return processorQueueLength; }
            internal set { processorQueueLength = value; }
        }

        /// <summary>
        /// Opeating system percent disk time
        /// </summary>
        public double? PercentDiskTime
        {
            get { return 100 - PercentDiskIdleTime; }
        }

        /// <summary>
        /// Opeating system percent disk idle time
        /// </summary>
        public double? PercentDiskIdleTime
        {
            get
            {
                if (!percentDiskIdleTime.HasValue)
                    return null;
                if (percentDiskIdleTime < 100)
                {
                    return percentDiskIdleTime;
                }
                else
                {
                    return 100;
                }
            }
            internal set { percentDiskIdleTime = value; }
        }

        /// <summary>
        /// Operating system average disk queue length
        /// </summary>
        public double? AvgDiskQueueLength
        {
            get { return avgDiskQueueLength; }
            internal set { avgDiskQueueLength = value; }
        }

        /// <summary>
        /// Delta value for calculation purposes
        /// </summary>
        public double? TimeStamp_Sys100NS_Delta
        {
            get { return timeStamp_Sys100NS_Delta; }
            internal set { timeStamp_Sys100NS_Delta = value; }
        }

        /// <summary>
        /// Delta value for calculation purposes
        /// </summary>
        public double? TimeStamp_PerfTime_Delta
        {
            get { return timeStamp_PerfTime_Delta; }
            internal set { timeStamp_PerfTime_Delta = value; }
        }

        /// <summary>
        /// Delta value for calculation purposes
        /// </summary>
        public double? PercentDiskIdleTimeBase_Delta
        {
            get { return percentDiskIdleTimeBase_Delta; }
            internal set { percentDiskIdleTimeBase_Delta = value; }
        }

        /// <summary>
        /// Raw value of Frequency_PerfTime - this is a calculation constant
        /// </summary>
        public double? Frequency_PerfTime
        {
            get { return frequency_PerfTime; }
            internal set { frequency_PerfTime = value; }
        }

        public bool ReadyToWrite
        {
            get { return readyToWrite; }
            internal set { readyToWrite = value; }
        }


        public string OsStatisticAvailability
        {
            get { return osStatisticAvailability; }
            internal set { osStatisticAvailability = value; }
        }

        public OSMetricsStatus OsMetricsStatus
        {
            get
            {
                switch (OsStatisticAvailability)
                {
                    case ServiceAvailableText:
                        return OSMetricsStatus.Available;
                    case ServiceTimedOutText:
                        return OSMetricsStatus.WMIServiceTimedOut;
                    case ServiceUnavailableText:
                        return OSMetricsStatus.WMIServiceUnreachable;
                    case ProcedureUnavailableText:
                        return OSMetricsStatus.OLEAutomationUnavailable;
                    case LightweightPoolingText:
                    case UnavailableDueToLWPText:
                        return OSMetricsStatus.UnavailableDueToLightweightPooling;
                    default:
                        return OSMetricsStatus.Disabled;
                }
            }
        }


        public DateTime? TimestampUTC
        {
            get { return timestampUTC; }
            internal set { timestampUTC = value; }
        }


        public double? DiffFromExpectedTimeStampError
        {
            get { return diffFromExpectedTimeStampError; }
            internal set { diffFromExpectedTimeStampError = value; }
        }


        public string DiffFromExpectedTimeStampName
        {
            get { return diffFromExpectedTimeStampName; }
            internal set { diffFromExpectedTimeStampName = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public static OSMetrics CookCounters(
            double? inputTotalPhysicalMemory,
            double? inputAvailableBytes,
            double? inputPagesPersec,
            double? inputPercentProcessorTime,
            double? inputPercentPrivilegedTime,
            double? inputPercentUserTime,
            double? inputProcessorQueueLength,
            double? inputPercentDiskIdleTime,
            double? inputAvgDiskQueueLength,
            double? inputTimeStamp_Sys100NS,
            double? inputTimeStamp_PerfTime,
            double? inputPercentDiskIdleTimeBase,
            double? inputFrequency_PerfTime,
            double? inputPercentSQLProcessorTime,
            DateTime? inputUTCTimestamp,
            OSMetrics previous)
        {
            OSMetrics outputOSMetrics = new OSMetrics();
            outputOSMetrics.OsStatisticAvailability = "available";
            outputOSMetrics.TimestampUTC = inputUTCTimestamp;

            TimeSpan? UTCdelta;

            // If we have the base values from a previous refresh, go ahead and calculate the values
            if (previous != null && previous.TimeStamp_Sys100NS_Raw.HasValue)
            {
                outputOSMetrics.TimeStamp_Sys100NS_Delta = Calculate_Timer_Delta(
                    previous.TimeStamp_Sys100NS_Raw,
                    inputTimeStamp_Sys100NS);

                UTCdelta = outputOSMetrics.TimestampUTC - previous.timestampUTC;


                bool breakOut = false;

                //If we detected a server restart while calculating the above counter
                //we need to break out 
                breakOut = !(outputOSMetrics.TimeStamp_Sys100NS_Delta.HasValue &&
                             outputOSMetrics.TimeStamp_Sys100NS_Delta.Value > 0);

                //If the Timestamp delta is highly inaccurate, break out
                if (!breakOut)
                {
                    if (!UTCdelta.HasValue)
                    {
                        breakOut = true;
                    }
                    else if ((outputOSMetrics.TimeStamp_Sys100NS_Delta / 10000000) >= UTCdelta.Value.TotalSeconds * 1.25)
                    {
                        breakOut = true;
                        outputOSMetrics.DiffFromExpectedTimeStampError = (outputOSMetrics.TimeStamp_Sys100NS_Delta /
                                                                          10000000) - UTCdelta.Value.TotalSeconds;
                        outputOSMetrics.DiffFromExpectedTimeStampName = "TimeStamp_Sys100NS";
                    }
                }

                if (!breakOut)
                {
                    outputOSMetrics.TimeStamp_PerfTime_Delta = Calculate_Timer_Delta(
                        previous.TimeStamp_PerfTime_Raw,
                        inputTimeStamp_PerfTime);

                    if (previous.PercentDiskIdleTimeBase_Raw.HasValue && previous.percentDiskIdleTime_Raw.HasValue)
                        if (previous.percentDiskIdleTime_Raw.Value < 0 || inputPercentDiskIdleTimeBase < 0)
                        {
                            outputOSMetrics.PercentDiskIdleTimeBase_Delta = -1;
                        }
                        else
                        {
                            outputOSMetrics.PercentDiskIdleTimeBase_Delta = Calculate_Timer_Delta(
                                previous.PercentDiskIdleTimeBase_Raw,
                                inputPercentDiskIdleTimeBase);
                        }


                    // Calculate values for display and alerting

                    double? delta;

                    outputOSMetrics.PagesPersec =
                        Calculate_PERF_COUNTER_COUNTER(
                            previous.PagesPersec_Raw,
                            inputPagesPersec,
                            out delta,
                            outputOSMetrics.TimeStamp_PerfTime_Delta,
                            previous.Frequency_PerfTime);

                    outputOSMetrics.PercentProcessorTime =
                                                           Calculate_PERF_100NSEC_TIMER_INV(
                                                               previous.PercentProcessorTime_Raw,
                                                               inputPercentProcessorTime,
                                                               out delta,
                                                               outputOSMetrics.TimeStamp_Sys100NS_Delta);
                    outputOSMetrics.PercentSQLProcessorTime =
                                                            Calculate_PERF_100NSEC_TIMER(
                                                            previous.PercentSQLProcessorTime_Raw,
                                                            inputPercentSQLProcessorTime,
                                                            out delta,
                                                            outputOSMetrics.TimeStamp_Sys100NS_Delta);

                    outputOSMetrics.PercentPrivilegedTime =
                                                            Calculate_PERF_100NSEC_TIMER(
                                                                previous.PercentPrivilegedTime_Raw,
                                                                inputPercentPrivilegedTime,
                                                                out delta,
                                                                outputOSMetrics.TimeStamp_Sys100NS_Delta);

                    outputOSMetrics.PercentUserTime =
                                                      Calculate_PERF_100NSEC_TIMER(
                                                          previous.PercentUserTime_Raw,
                                                          inputPercentUserTime,
                                                          out delta,
                                                          outputOSMetrics.TimeStamp_Sys100NS_Delta);

                    if (outputOSMetrics.PercentDiskIdleTimeBase_Delta < 0)
                    {
                        if (inputPercentDiskIdleTimeBase < 0)
                        {
                            outputOSMetrics.PercentDiskIdleTime = inputPercentDiskIdleTime;
                        }
                    }
                    else if ((outputOSMetrics.PercentDiskIdleTimeBase_Delta / 10000000) >= UTCdelta.Value.TotalSeconds * 1.25)
                    {
                        breakOut = true;
                        outputOSMetrics.DiffFromExpectedTimeStampError = (outputOSMetrics.PercentDiskIdleTimeBase_Delta /
                                                                          10000000) - UTCdelta.Value.TotalSeconds;
                        outputOSMetrics.DiffFromExpectedTimeStampName = "PercentDiskIdleTimeBase";
                        inputPercentDiskIdleTime = null;
                        inputPercentDiskIdleTimeBase = null;
                    }
                    else
                    {
                        // PercentIdleTime is a PERF_PRECISION_100NS_TIMER and has a separate time base
                        outputOSMetrics.PercentDiskIdleTime =
                            Calculate_PERF_100NSEC_TIMER(
                                previous.PercentDiskIdleTime_Raw,
                                inputPercentDiskIdleTime,
                                out delta,
                                outputOSMetrics.PercentDiskIdleTimeBase_Delta);
                    }

                    outputOSMetrics.AvgDiskQueueLength =
                        Calculate_PERF_COUNTER_100NS_QUEUELEN_TYPE(
                            previous.AvgDiskQueueLength_Raw,
                            inputAvgDiskQueueLength,
                            out delta,
                            outputOSMetrics.TimeStamp_Sys100NS_Delta);

                    //Deltas have now been calculated so we are ok to write to the database
                    outputOSMetrics.ReadyToWrite = true;
                }
            }

            //These are raw counters and do not need to be cooked
            if (inputTotalPhysicalMemory.HasValue)
            {
                //outputOSMetrics.TotalPhysicalMemory = new FileSize();
                outputOSMetrics.TotalPhysicalMemory.Bytes = (decimal)inputTotalPhysicalMemory;
            }

            if (inputAvailableBytes.HasValue)
            {
                //outputOSMetrics.AvailableBytes = new FileSize();
                outputOSMetrics.AvailableBytes.Bytes = (decimal)inputAvailableBytes;
            }

            outputOSMetrics.ProcessorQueueLength = inputProcessorQueueLength;

            //Store this value for file-writing only
            outputOSMetrics.Frequency_PerfTime = inputFrequency_PerfTime;

            // Store new values for next refresh
            outputOSMetrics.PagesPersec_Raw = inputPagesPersec;
            outputOSMetrics.PercentProcessorTime_Raw = inputPercentProcessorTime;
            outputOSMetrics.PercentSQLProcessorTime_Raw = inputPercentSQLProcessorTime;
            outputOSMetrics.PercentPrivilegedTime_Raw = inputPercentPrivilegedTime;
            outputOSMetrics.PercentUserTime_Raw = inputPercentUserTime;
            outputOSMetrics.PercentDiskIdleTime_Raw = inputPercentDiskIdleTime;
            outputOSMetrics.AvgDiskQueueLength_Raw = inputAvgDiskQueueLength;
            outputOSMetrics.TimeStamp_Sys100NS_Raw = inputTimeStamp_Sys100NS;
            outputOSMetrics.TimeStamp_PerfTime_Raw = inputTimeStamp_PerfTime;
            outputOSMetrics.PercentDiskIdleTimeBase_Raw = inputPercentDiskIdleTimeBase;

            if (previous != null && !previous.TimeStamp_Sys100NS_Raw.HasValue)
            {
                // Store new values for next refresh
                outputOSMetrics.pagesPersec = inputPagesPersec;
                outputOSMetrics.PercentProcessorTime = inputPercentProcessorTime;
                outputOSMetrics.PercentSQLProcessorTime = inputPercentSQLProcessorTime;
                outputOSMetrics.percentPrivilegedTime = inputPercentPrivilegedTime;
                outputOSMetrics.PercentUserTime = inputPercentUserTime;
                outputOSMetrics.percentDiskIdleTime = inputPercentDiskIdleTime;
                outputOSMetrics.AvgDiskQueueLength = inputAvgDiskQueueLength;
            }

            return outputOSMetrics;
        }

        private static double? Absolute(double? input)
        {
            if (input.HasValue)
                return Math.Abs(input.Value);
            else
                return null;
        }


        internal static double? Calculate_Timer_Delta(
            double? timeValue1,
            double? timeValue2)
        {
            try
            {
                double? timerDelta = (timeValue2 - timeValue1);
                return timerDelta;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // VIS 6/2006
        // Calculate_PERF_100NSEC_TIMER
        // Performs calculation for WMI counters of type PERF_100NSEC_TIMER
        //
        // Note:
        // Counters of type PERF_PRECISION_100NS_TIMER use this function with
        // a more precise timer value.
        //
        // Definitions:
        //	counterValue1 - first measured counter value
        //	counterValue2 - second measured counter value
        //  timeValue1 - first measured timer value (usually TimeStamp_Sys100NS)
        //  timeValue2 - second measured timer value (usually TimeStamp_Sys100NS)
        //
        // From TechNet:
        // This counter type shows the active time of a component as a 
        // percentage of the total elapsed time of the sample interval. 
        // It measures time in units of 100 nanoseconds. Counters of this 
        // type are designed to measure the activity of one component at 
        // a time.
        internal static double? Calculate_PERF_100NSEC_TIMER(
            double? counterValue1,
            double? counterValue2,
            out double? counterDelta,
            double? timerDelta
            )
        {
            try
            {
                if (timerDelta.HasValue && timerDelta != 0 && counterValue2 >= counterValue1)
                {
                    counterDelta = (counterValue2 - counterValue1);
                    return (100 * (Absolute(counterDelta) / (double)timerDelta));
                }
                else
                {
                    counterDelta = 0;
                    return null;
                }
            }
            catch (Exception)
            {
                counterDelta = 0;
                return 0;
            }
        }

        // VIS 6/2006
        // Calculate_PERF_100NSEC_TIMER_INV
        // Performs calculation for WMI counters of type PERF_100NSEC_TIMER_INV
        //	
        // Definitions:
        //	counterValue1 - first measured counter value
        //	counterValue2 - second measured counter value
        //  timeValue1 - first measured timer value (usually TimeStamp_Sys100NS)
        //  timeValue2 - second measured timer value (usually TimeStamp_Sys100NS)
        //	
        // From TechNet:
        // This counter type shows the average percentage of active time observed 
        // during the sample interval. This is an inverse counter. Inverse counters 
        // are calculated by monitoring the percentage of time that the service was 
        // inactive and then subtracting that value from 100 percent.
        internal static double? Calculate_PERF_100NSEC_TIMER_INV(
            double? counterValue1,
            double? counterValue2,
            out double? counterDelta,
            double? timerDelta)
        {
            try
            {
                if (timerDelta.HasValue && timerDelta != 0 && counterValue2 >= counterValue1)
                {
                    counterDelta = (counterValue2 - counterValue1);
                    return (100 * (1 - (Absolute(counterDelta) / (double)timerDelta)));
                }
                else
                {
                    counterDelta = 0;
                    return 0;
                }
            }
            catch (Exception)
            {
                counterDelta = 0;
                return 0;
            }
        }

        // VIS 6/2006
        // Calculate_PERF_COUNTER_100NS_QUEUELEN_TYPE
        // Performs calculation for WMI counters of type PERF_COUNTER_100NS_QUEUELEN_TYPE
        //	
        // Definitions:
        //	counterValue1 - first measured counter value
        //	counterValue2 - second measured counter value
        //  timeValue1 - first measured timer value (usually TimeStamp_Sys100NS)
        //  timeValue2 - second measured timer value (usually TimeStamp_Sys100NS)
        //	
        // From TechNet:
        // This counter type measures the queue-length space-time product using a 
        // 100-nanosecond time base.

        internal static double? Calculate_PERF_COUNTER_100NS_QUEUELEN_TYPE(
            double? counterValue1,
            double? counterValue2,
            out double? counterDelta,
            double? timerDelta)
        {
            try
            {
                if (timerDelta.HasValue && timerDelta != 0 && counterValue2 >= counterValue1)
                {
                    counterDelta = (counterValue2 - counterValue1);
                    double? calculationValue = (Absolute(counterDelta) / (double)timerDelta);
                    if (calculationValue.HasValue && calculationValue < 1000000000)
                        return calculationValue;
                    else
                        return 0;
                }
                else
                {
                    counterDelta = 0;
                    return 0;
                }
            }
            catch (Exception)
            {
                counterDelta = 0;
                return 0;
            }
        }

        // VIS 6/2006
        // Calculate_PERF_COUNTER_COUNTER
        // Performs calculation for WMI counters of type PERF_COUNTER_COUNTER
        //	
        // Definitions:
        //	counterValue1 - first measured counter value
        //	counterValue2 - second measured counter value
        //  timeValue1 - first measured timer value (usually Timestamp_PerfTime)
        //  timeValue2 - second measured timer value (usually Timestamp_PerfTime)
        //  timeBase - number of ticks per second (usually Frequency_PerfTime)
        //	
        // From TechNet:
        // This counter type shows the average number of operations completed during 
        // each second of the sample interval. Counters of this type measure time in 
        // ticks of the system clock. The F variable represents the number of ticks 
        // per second. The value of F is factored into the equation so that the result 
        // can be displayed in seconds.


        internal static double? Calculate_PERF_COUNTER_COUNTER(
            double? counterValue1,
            double? counterValue2,
            out double? counterDelta,
            double? timerDelta,
            double? timeBase)
        {
            try
            {
                if ((timerDelta.HasValue && timerDelta != 0) && (timeBase.HasValue && timeBase > 0) && counterValue2 >= counterValue1)
                {
                    counterDelta = (counterValue2 - counterValue1);
                    return (Absolute(counterDelta) / ((timerDelta / (double)timeBase)));
                }
                else
                {
                    counterDelta = 0;
                    return 0;
                }
            }
            catch (Exception)
            {
                counterDelta = 0;
                return 0;
            }
        }

        internal static double? Calculate_PERF_AVERAGE_TIMER(
            double? counterValue1,
            double? counterValue2,
            double? baseValue1,
            double? baseValue2,
            double? frequency)
        {
            if (counterValue1.HasValue && counterValue2.HasValue && baseValue1.HasValue && baseValue2.HasValue && frequency.HasValue)
            {
                double? counterDelta;
                double? baseDelta;
                counterDelta = counterValue2 - counterValue1;
                baseDelta = baseValue2 - baseValue1;
                if (baseDelta != 0 && frequency != 0)
                {
                    return ((Absolute(counterDelta) / frequency) / (Absolute(baseDelta)));
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return new double?();
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}