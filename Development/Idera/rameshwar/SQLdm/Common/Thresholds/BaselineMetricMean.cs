//------------------------------------------------------------------------------
// <copyright file="BaselineMetricMean.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//SQLdm 10.1 (Srishti Purohit)
//4.1.12 Revise Multiple Baseline for Independent Scheduling
//------------------------------------------------------------------------------
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Thresholds
{
    /// <summary>
    /// Class definition of Metric Mean respect to baseline tamplate for specific server.
    /// </summary>
    [Serializable]
    public class BaselineMetricMean
    {

        #region fields

        private static readonly Logger LOG = Logger.GetLogger(typeof(BaselineMetricMean));
        private BaselineConfiguration baselineConfig = new BaselineConfiguration();
        //private Dictionary<int, double?> baselineMetricMeanDic = new Dictionary<int, double?>();
        private int metric = -1;
        private double? mean = null;
        private DateTime utcCalculationTime;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaselineMetricMeanCollection"/> class.
        /// </summary>
        public BaselineMetricMean()
        {
        }

        #endregion

        #region properties

        public int Metric
        {
            get
            {
                return metric;
            }

            set
            {
                metric = value;
            }
        }
        
        public BaselineConfiguration BaselineConfig
        {
            get
            {
                return baselineConfig;
            }

            set
            {
                baselineConfig = value;
            }
        }

        public DateTime UTCCalculationTime
        {
            get
            {
                return utcCalculationTime;
            }

            set
            {
                utcCalculationTime = value;
            }
        }

        public double? Mean
        {
            get
            {
                return mean;
            }

            set
            {
                mean = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
