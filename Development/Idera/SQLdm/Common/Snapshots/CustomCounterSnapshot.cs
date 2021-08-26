//------------------------------------------------------------------------------
// <copyright file="CustomCounterSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Snapshot for custom counters
    /// </summary>
    [Serializable]
    public class CustomCounterSnapshot : Snapshot, ICloneable
    {
        #region fields

        private CustomCounterDefinition definition;
        private decimal? rawValue = null;
        private decimal? previousValue = null;
        private DateTime? previousTimestamp = null;
        private DateTime? previousServerStartTime = null;
        private TimeSpan? runTime = null;

        #endregion

        #region constructors

        //private CustomCounterSnapshot(string instanceName)
        //    : base(instanceName)
        //{

        //}

        internal CustomCounterSnapshot(SqlConnectionInfo info, CustomCounterDefinition definition)
            : base(info.InstanceName)
        {
            if (definition == null)
                throw new ArgumentNullException("Custom counter definition was null");
            this.definition = definition;
            //SQLDM-30100. Update timeStamp in Customecouters.
            TimeStamp = DateTime.Now;

        }

        internal CustomCounterSnapshot(SqlConnectionInfo info, CustomCounterSnapshot previousSnapshot)
            : base(info.InstanceName)
        {
            if (previousSnapshot == null)
                throw new ArgumentNullException("Previous counter passed to custom counter snapshot was null");
            definition = previousSnapshot.definition;
            previousValue = previousSnapshot.RawValue;
            previousTimestamp = previousSnapshot.TimeStamp;
            previousServerStartTime = previousSnapshot.ServerStartupTime;
            //SQLDM-30100. Update timeStamp in Customecouters.
            TimeStamp = DateTime.Now;
        }

        #endregion

        #region properties

        /// <summary>
        /// The counter definition used to collect this snapshot
        /// </summary>
        public CustomCounterDefinition Definition
        {
            get { return definition; }
            private set { definition = value; }
        }

        /// <summary>
        /// This is the raw value returned by the monitored SQL Server.  This may or may not be the display value 
        /// depending on the scale and calculation settings in the definition.
        /// </summary>
        public decimal? RawValue
        {
            get { return rawValue; }
            internal set { this.rawValue = value; }
        }

        public decimal? DeltaValue
        {
            get
            {
                if (previousValue != null && previousServerStartTime == ServerStartupTime)
                {
                    return rawValue - previousValue;
                }
                else
                {
                    return null;
                }
            }
        }

        public TimeSpan? TimeDelta
        {
            get
            {
                if (previousTimestamp != null && previousServerStartTime == ServerStartupTime)
                {
                    return TimeStamp.Value.Subtract(previousTimestamp.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? DisplayValue
        {
            get
            {
                if (definition.CalculationType == CalculationType.Delta)
                {
                    decimal? deltaValue = DeltaValue;
                    TimeSpan? deltaTime = TimeDelta;
                    if (deltaValue.HasValue && deltaTime.HasValue && deltaTime.Value.TotalMilliseconds > 0)
                        return deltaValue.Value / Convert.ToDecimal(deltaTime.Value.TotalMilliseconds) * 1000m;
                    else
                        return null;
                }
                else
                {
                    return rawValue;
                }
            }
        }

        public TimeSpan? RunTime
        {
            get { return runTime; }
            internal set { runTime = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public object Clone()
        {
            //CustomCounterSnapshot clone = new CustomCounterSnapshot(ServerName);
            //clone.Definition = (CustomCounterDefinition)this.Definition.Clone();
            //clone.Error = this.Error;
            //clone.previousServerStartTime = previousServerStartTime;
            //clone.previousTimestamp = previousTimestamp;
            //clone.previousValue = previousValue;
            //clone.ProductEdition = ProductEdition;
            //clone.ProductVersion = ProductVersion;
            //clone.RawValue = rawValue;
            //clone.ServerStartupTime = ServerStartupTime;
            //clone.TimeStamp = TimeStamp;
            //clone.TimeStampLocal = TimeStampLocal;
            //return clone;
            return this.MemberwiseClone();
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
