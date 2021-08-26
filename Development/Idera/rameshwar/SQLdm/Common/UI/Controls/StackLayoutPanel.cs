namespace Idera.SQLdm.Common.UI.Controls
{
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(resfinder), "Idera.SQLdm.Common.UI.Resources.StackLayoutPanel.bmp")]
    public partial class StackLayoutPanel : Panel
    {
        private Control activeControl;

        public StackLayoutPanel()
        {
            InitializeComponent();
        }

        public Control ActiveControl
        {
            get { return GetActiveControl(); }
            set { SetActiveControl(value);   }
        }

        [Description("Sets the child control that will be visible.  All other children are hidden.")]
        protected Control GetActiveControl()
        {
            return activeControl;
        }

        protected void SetActiveControl(Control value)
        {

            SuspendLayout();

            // hide all but the control we are about to show
            foreach (Control control in Controls)
            {
                if (control != value)
                {
                    control.Hide();
                } 
            }

            activeControl = value;

            // if activeControl is a child control make sure it is shown
            if (activeControl != null && Controls.Contains(activeControl))
            {
                value.Show();
                value.BringToFront();
                value.Select();
            }

            ResumeLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            Control control = ActiveControl;
            if (control != null && Controls.Contains(control))
            {
                Rectangle bounds = ClientRectangle;

                // adjust the bounds
                bounds.X = Padding.Left;
                bounds.Y = Padding.Top;
                bounds.Width = bounds.Width - Padding.Left - Padding.Right;
                bounds.Height = bounds.Height - Padding.Top - Padding.Bottom;
                
                activeControl.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
            base.OnLayout(levent);
        }
    }
}
internal class resfinder { }
