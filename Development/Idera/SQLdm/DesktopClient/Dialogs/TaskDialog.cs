using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.DesktopClient.Views;
    using Idera.SQLdm.DesktopClient.Views.Alerts;
using Infragistics.Win;
    using Idera.SQLdm.Common.Objects;

    public partial class TaskDialog : BaseDialog
    {
        private const string BAND = "Band 0";
        private DataSourceWithID<int> dataSource;

        public TaskDialog(DataSourceWithID<int> dataSource,
                          ValueList metricValueList,
                          ValueList severityValueList,
                          ValueList statusValueList)
        {
            this.DialogHeader = "To Do";
            this.dataSource = dataSource;
            InitializeComponent();
            InitializeBindings(metricValueList, severityValueList, statusValueList);
            AdaptFontSize();
        }

        private void InitializeBindings(ValueList metricValueList, ValueList severityValueList, ValueList statusValueList)
        {
            // simple bindings
            instance.DataBindings.Add("Text", dataSource, BAND + ".ServerName");
            subject.DataBindings.Add("Text", dataSource, BAND + ".Subject");
            message.DataBindings.Add("Text", dataSource, BAND + ".Message");
            ownerDetails.DataBindings.Add("Text", dataSource, BAND + ".Owner");
            commentsDetails.DataBindings.Add("Text", dataSource, BAND + ".Comments");
            value.DataBindings.Add("Text", dataSource, BAND + ".Value");

            // bind created on 
            Binding binding = created.DataBindings.Add("Text", dataSource, BAND + ".CreatedOn");
            binding.FormatString = "G";
            binding.FormattingEnabled = true;
            // bind completed on
            binding = completed.DataBindings.Add("Text", dataSource, BAND + ".CompletedOn");
            binding.FormatString = "G";
            binding.FormattingEnabled = true;

            // configure the bindings for the details pane severity image and label
            ValueListBinding severityBindingImage = new ValueListBinding("Image", dataSource, BAND + ".Severity");
            ValueListBinding severityBindingText = new ValueListBinding("Text", dataSource, BAND + ".Severity");
            severityBindingImage.ValueList = severityValueList;
            severityBindingText.ValueList = severityValueList;
            severityImage.DataBindings.Add(severityBindingImage);
            severity.DataBindings.Add(severityBindingText);

            // configure the bindings for the details pane metric label
            ValueListBinding metricBindingText = new ValueListBinding("Text", dataSource, BAND + ".Metric");
            metricBindingText.ValueList = metricValueList;
            metric.DataBindings.Add(metricBindingText);

            statusDetailCombo.Items.AddRange(MakeStatusValues(statusValueList));
            statusDetailCombo.DataBindings.Add("Value", dataSource, BAND + ".Status");
        }

        private ValueListItem[] MakeStatusValues(ValueList source)
        {
            ValueListItem[] values = new ValueListItem[source.ValueListItems.Count];
            int i = 0;
            foreach (ValueListItem item in source.ValueListItems)
            {
                ValueListItem newItem = new ValueListItem(item.DataValue, item.DisplayText);
                if (item.HasAppearance)
                    newItem.Appearance = item.Appearance.Clone() as Appearance;
                values[i++] = newItem;
            }
            return values;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!okButton.Enabled)
                okButton.Enabled = true;
        }

        private void statusDetailCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!okButton.Enabled)
                okButton.Enabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

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