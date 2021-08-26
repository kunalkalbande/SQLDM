using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class AffectedRulesWarningDialog : Form
    {
        public AffectedRulesWarningDialog()
        {
            InitializeComponent();
            rulesListView.DrawFilter = new HideFocusRectangleDrawFilter();
            AdaptFontSize();
        }

        public string InfoText
        {
            get { return informationBox.Text; }
            set { informationBox.Text = value; }
        }

        public void SetNotificationRules(IList<NotificationRule> rules) 
        {
            if (rules.Count == 0)
            {
                stackLayoutPanel1.ActiveControl = noneSelectedLabel;
            } 
            else
            {
                rulesBindingSource.SuspendBinding();
                rulesBindingSource.Clear();

                foreach (NotificationRule rule in rules)
                    rulesBindingSource.Add(rule);

                rulesBindingSource.ResumeBinding();
                stackLayoutPanel1.ActiveControl = rulesListView;
            }
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }


}