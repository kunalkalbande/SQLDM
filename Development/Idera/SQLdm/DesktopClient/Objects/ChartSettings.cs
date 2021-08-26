using System;
using System.Collections.Generic;
using System.ComponentModel;

using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [SerializableAttribute]
    public class ChartSettings
    {
        #region fields

        private ChartType chartType;
        private ChartView chartView;
        private int chartDisplayValues;

        #endregion

        #region constructors

        public ChartSettings()
        {
            chartType = ChartType.Unknown;
            chartView = ChartView.Unknown;
            chartDisplayValues = 0;
        }

        #endregion

        #region properties

        public ChartType ChartType
        {
            get { return chartType; }
            set { chartType = value; }
        }

        public ChartView ChartView
        {
            get { return chartView; }
            set { chartView = value; }
        }

        public int ChartDisplayValues
        {
            get { return chartDisplayValues; }
            set { chartDisplayValues = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compare to the passed ChartSettings object and make sure all internal values are equal
        /// </summary>
        /// <param name="obj">a ChartSettings object to compare against</param>
        /// <returns>true if a ChartSettings object is passed and all settings match otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj is ChartSettings)
            {
                ChartSettings settings = (ChartSettings)obj;
                if (chartType != settings.ChartType
                    || chartView != settings.chartView
                    || chartDisplayValues != settings.ChartDisplayValues)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Create a new ChartSettings object with settings from the passed ChartPanel
        /// </summary>
        /// <param name="chart">The ChartPanel from which the settings will be retrieved</param>
        /// <returns>A ChartSettings object with the current settings of the grid</returns>
        public static ChartSettings GetSettings(ChartPanel chart)
        {
            ChartSettings settings = new ChartSettings();

            settings.ChartType = chart.ChartType;
            settings.ChartView = chart.ChartView;
            settings.ChartDisplayValues = chart.ChartDisplayValues;

            return settings;
        }

        #endregion
    }
}
