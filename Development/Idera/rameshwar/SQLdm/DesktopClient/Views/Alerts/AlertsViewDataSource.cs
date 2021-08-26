//------------------------------------------------------------------------------
// <copyright file="AlertsViewDataSource.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Views.Alerts
{
    using System.Collections.Generic;
    using System.Drawing;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Infragistics.Win.UltraWinDataSource;
    using Wintellect.PowerCollections;
    using System.ComponentModel;

    /*
     * Column 0 is a long containing the alert id.  For refreshes of the data using the same 
     * filter should only have to add items that have an alert id greater than highest alert id.
     * 
     */

    public class AlertsViewDataSource : DataSourceWithID<long>
    {
        public AlertsViewDataSource()
        {
            base.KeyIndex = 0;
        }

        public override void EndInit()
        {
            ConfigureColumns();
            base.EndInit();
        }

        public void ConfigureColumns()
        {
            // Convert UTCOccuranceDateTime to local time
            this.Band.Columns["UTCOccurrenceDateTime"].Tag = new DateConverter();
        }
    }

    public class AlertsViewDataSourceX : DataSource
    {
        private const int ID_COLUMN = 0;

        public long HighestAlertID
        {
            get
            {
                if (table.Count == 0)
                    return 0;

                return (long) table[table.Count - 1][0];
            }
        }

        internal override void AddRow(object[] values)
        {
            if (table.Count == 0)
            {
                table.Add(values);
                return;
            }
            // new row alert id
            long newAlertID = (long) values[ID_COLUMN];

            // start at the end to find the insertion point
            int i = table.Count - 1;
            object[] row = this.table[i];
            long lastAlertID = (long) row[0];

            if (newAlertID > lastAlertID)
            {
                base.AddRow(values);
            } 
            else 
            {   // the alert row needs to be inserted somewhere in the middle
                for ( ; i >= 0; i--)
                {
                    row = this.table[i];
                    long rowID = (long)row[0];
                    if (newAlertID > rowID)
                    {
                        table.Insert(i + 1, values);
                        return;
                    }
                    if (newAlertID == rowID)
                    {
                        table[i] = values;
                        return;
                    }
                }
            }
        }

        public void ConfigureColumns()
        {
            // Supply SeverityImage and SeverityLabel column from Severity
//            MonitoredStateImageConverter msic = new MonitoredStateImageConverter("Severity");
//            this.Band.Columns["SeverityImage"].Tag = msic;
//            this.Band.Columns["SeverityLabel"].Tag = msic;
//            // Supply TransitionImage and TransitionLabel column from Transition
//            TransitionImageConverter tic = new TransitionImageConverter("StateEvent");
//            this.Band.Columns["TransitionImage"].Tag = tic;
//            this.Band.Columns["TransitionLabel"].Tag = tic;
            // Convert UTCOccuranceDateTime to local time
            this.Band.Columns["UTCOccurrenceDateTime"].Tag = new DateConverter();
        }

        public override void EndInit()
        {
            ConfigureColumns();

            base.EndInit();
        }

        internal struct ImageAndLabel
        {
            internal ImageAndLabel(Bitmap image, string label)
            {
                this.Image = image;
                this.Label = label;
            }

            internal string Label;
            internal Bitmap Image;
        }

        public class TransitionImageConverter : IValueConverter
        {
            private const string LABEL_OK_Info          = "Raised OK to Informational";
            private const string LABEL_OK_Warning       = "Raised OK to Warning";
            private const string LABEL_OK_Critical      = "Raised OK to Critical";
            private const string LABEL_Info_OK          = "Lowered Informational to OK";
            private const string LABEL_Info_Info        = "Still Informational";
            private const string LABEL_Info_Warning     = "Raised Informational to Warning";
            private const string LABEL_Info_Critical    = "Raised Informational to Critical";
            private const string LABEL_Warning_OK       = "Lowered Warning to OK";
            private const string LABEL_Warning_Info     = "Lowered Warning to Informational";
            private const string LABEL_Warning_Warning  = "Still Warning";
            private const string LABEL_Warning_Critical = "Raised Warning to Critical";
            private const string LABEL_Critical_OK      = "Lowered Critical to OK";
            private const string LABEL_Critical_Info    = "Lowered Critical to Informational";
            private const string LABEL_Critical_Warning = "Lowered Critical to Warning";
            private const string LABEL_Critical_Critical= "Still Critical";

            private static Dictionary<byte, ImageAndLabel> map = new Dictionary<byte, ImageAndLabel>();

            private string basedOnColumn;

            static TransitionImageConverter()
            {
                map.Add((byte)Transition.OK_Info, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToInfo, LABEL_OK_Info));
                map.Add((byte)Transition.OK_Warning, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToWarning, LABEL_OK_Warning));
                map.Add((byte)Transition.OK_Critical, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical, LABEL_OK_Critical));
                map.Add((byte)Transition.Info_OK, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK, LABEL_Info_OK));
                map.Add((byte)Transition.Info_Info, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedInfo, LABEL_Info_Info));
                map.Add((byte)Transition.Info_Warning, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToWarning, LABEL_Info_Warning));
                map.Add((byte)Transition.Info_Critical, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical, LABEL_Info_Critical));
                map.Add((byte)Transition.Warning_OK, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK, LABEL_Warning_OK));
                map.Add((byte)Transition.Warning_Info, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToInfo, LABEL_Warning_Info));
                map.Add((byte)Transition.Warning_Warning, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedWarning, LABEL_Warning_Warning));
                map.Add((byte)Transition.Warning_Critical, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical, LABEL_Warning_Critical));
                map.Add((byte)Transition.Critical_OK, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK, LABEL_Critical_OK));
                map.Add((byte)Transition.Critical_Info, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToInfo, LABEL_Critical_Info));
                map.Add((byte)Transition.Critical_Warning, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToWarning, LABEL_Critical_Warning));
                map.Add((byte)Transition.Critical_Critical, new ImageAndLabel(global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedCritical, LABEL_Critical_Critical));
            }

            public TransitionImageConverter(string basedOnColumn)
            {
                this.basedOnColumn = basedOnColumn;
            }

            public object ConvertData(UltraDataColumn column, UltraDataRow row)
            {
                UltraDataColumn sourceColumn = column.ParentCollection[basedOnColumn];
                if (sourceColumn != null)
                {
                    object value = ((DataSource)row.DataSource).GetRawCellValue(row.Index, sourceColumn.Index);
                    if (column.DataType == typeof(string))
                    {
                        return map[(byte)value].Label;
                    }
                    if (column.DataType == typeof(Bitmap))
                    {
                        return map[(byte)value].Image;
                    }
                    return value;
                }
                return null;
            }
        }


    }
}
