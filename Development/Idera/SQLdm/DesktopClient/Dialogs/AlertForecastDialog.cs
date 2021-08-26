using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common;
using Wintellect.PowerCollections;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AlertForecastDialog : BaseDialog
    {
        private DataTable data;
        private DateTime  starttime;
        private DateTime  endtime;
        private bool      sortbyCritical;

        public AlertForecastDialog()
        {
            this.DialogHeader = "Alert Forecast Details";
            InitializeComponent();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.grid);
        }

        public DataTable Data
        {
            set { data = value; }
        }

        public DateTime StartTime
        {
            set { starttime = value; }
        }

        public DateTime EndTime
        {
            set { endtime = value; }
        }

        public bool SortByCritical
        {
            set { sortbyCritical = value; }
        }

        private void AlertForecastDialog_Load(object sender, EventArgs e)
        {
            if(data == null || data.Rows.Count == 0)
                return;

            label1.Text = string.Format(label1.Text, starttime.ToShortTimeString(), endtime.ToShortTimeString());

            DataTable table = new DataTable();

            table.Columns.Add("Server",     typeof(string));
            table.Columns.Add("Metric",     typeof(string));
            table.Columns.Add("Warning Likelihood",  typeof(double));
            table.Columns.Add("Critical Likelihood", typeof(double));

            // since the warning and critical values come across as two separate rows, we'll collapse them into a single record here
            Dictionary<Pair<string, string>, double[]> d = new Dictionary<Pair<string, string>, double[]>();

            foreach (DataRow row in data.Rows)
            {
                Pair<string, string> key = new Pair<string, string>((string)row["servername"], (string)row["metricname"]);

                if (!d.ContainsKey(key))
                    d.Add(key, new double[2]);

                int severity = (int)row["severity"];

                if (severity == (int)MonitoredState.Warning)
                    d[key][0] = (double)row["accuracy"]; // warning
                else
                    d[key][1] = (double)row["accuracy"]; // critical
            }

            foreach (Pair<string,string> key in d.Keys)
            {
                DataRow newRow = table.NewRow();

                newRow["Server"] = key.First;
                newRow["Metric"] = key.Second;
                newRow["Warning Likelihood"]  = d[key][0];
                newRow["Critical Likelihood"] = d[key][1];

                table.Rows.Add(newRow);
            }

            grid.DataSource = table;            
            grid.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.True;
            grid.DisplayLayout.Bands[0].Columns[2].Format = "#0.##%";
            grid.DisplayLayout.Bands[0].Columns[3].Format = "#0.##%";
            grid.DisplayLayout.Bands[0].SortedColumns.Add(sortbyCritical ? "Critical Likelihood" : "Warning Likelihood", true);

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if ((double)grid.Rows[i].Cells[2].Value >= 0.5)
                {
                    grid.Rows[i].Cells[2].Appearance.BackColor = Color.FromArgb(125, Color.Yellow);
                    grid.Rows[i].Cells[2].Appearance.ForeColor = Color.Black;
                }
                else if((double)grid.Rows[i].Cells[2].Value < 0.5)
                {
                    grid.Rows[i].Cells[2].Appearance.BackColor = Color.White;
                    grid.Rows[i].Cells[2].Appearance.ForeColor = Color.Black;
                }

                if ((double)grid.Rows[i].Cells[3].Value >= 0.5)
                {
                    grid.Rows[i].Cells[3].Appearance.BackColor = Color.FromArgb(125, Color.Red);
                    grid.Rows[i].Cells[3].Appearance.ForeColor = Color.Black;
                }
                else if ((double)grid.Rows[i].Cells[3].Value < 0.5)
                {
                    grid.Rows[i].Cells[3].Appearance.BackColor = Color.White;
                    grid.Rows[i].Cells[3].Appearance.ForeColor = Color.Black;
                }

                // A 0% means we didn't have data for that severity
                if ((double)grid.Rows[i].Cells[2].Value == 0.0)
                {
                    grid.Rows[i].Cells[2].Appearance.BackColor = Color.White;
                    grid.Rows[i].Cells[2].Appearance.ForeColor = Color.LightGray;
                }

                if ((double)grid.Rows[i].Cells[3].Value == 0.0)
                {
                    grid.Rows[i].Cells[3].Appearance.BackColor = Color.White;
                    grid.Rows[i].Cells[3].Appearance.ForeColor = Color.LightGray;
                }
            }
        }

        private void enableGroupByToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grid.DisplayLayout.GroupByBox.Hidden = !grid.DisplayLayout.GroupByBox.Hidden;
            enableGroupByToolStripMenuItem.Checked = !grid.DisplayLayout.GroupByBox.Hidden;
        }

        private void AlertForecastDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertForecast);
        }

        private void AlertForecastDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertForecast);

        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}