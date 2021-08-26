using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class ToggleButton : UserControl
    {
        private bool collapsed = true;

        public event EventHandler CollapsedChanged;
        
        public ToggleButton()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        public string ButtonText
        {
            get { return textLabel.Text; }
            set { textLabel.Text = value; }
        }

        [Category("Appearance")]
        public bool Collapsed
        {
            get { return collapsed; }
            set
            {
                if (collapsed != value)
                {
                    collapsed = value;
                    toggleImage.Image = collapsed ? Resources.DownArrow_Gray_7w_4h : Resources.UpArrow_Gray_7w_4h;

                    if (CollapsedChanged != null)
                    {
                        CollapsedChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            backgroundPanel.BorderColor = Color.FromArgb(136, 136, 136);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            backgroundPanel.BorderColor = Color.FromArgb(170, 170, 170);
            base.OnMouseLeave(e);
        }

        private void ChildControl_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void ChildControl_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void ChildControl_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }
    }
}
