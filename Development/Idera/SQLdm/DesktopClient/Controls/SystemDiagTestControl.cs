using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    using Helpers;
    using Properties;

    public partial class SystemDiagTestControl : UserControl
    {
        private bool selected;

        public SystemDiagTestControl()
        {
            InitializeComponent();
            //SQLDM - 30848 - UX - Modernization, PRD 4.2,
            if (AutoScaleSizeHelper.isScalingRequired)
                this.dividerLabel.Height = 1;
        }
        public void SetStatusImage(Image image)
        {
            this.pictureBox1.Image = image;
        }
        public void SetComponentName(string componentName)
        {
            componentLabel.Text = componentName;
        }
        public void SetInstanceName(string instanceName)
        {
            instanceLabel.Text = instanceName;
        }
        public void SetTestName(string testName)
        {
            testLabel.Text = testName;
        }
        public void SetMessage(string message)
        {
            messageLabel.Text = message;
        }
        public bool DividerVisible
        {
            get { return dividerLabel.Visible; }
            set { dividerLabel.Visible = value; }
        }
        public bool Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;
                SetBackground();
            }
        }

        private void SetBackground()
        {
            BackgroundImage = selected ? Resources.SelectedRowBackground : null;
        }

        private void mouse_Click(object sender, EventArgs e)
        {
            foreach (SystemDiagTestControl control in Parent.Controls)
            {
                if (control.Selected)
                    control.Selected = false;
            }
            Selected = true;
        }
    }
}
