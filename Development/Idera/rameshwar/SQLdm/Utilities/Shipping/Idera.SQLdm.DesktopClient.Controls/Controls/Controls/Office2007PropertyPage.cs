using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [Designer("Idera.SQLdm.DesktopClient.Controls.Designers.PropertyPageDesigner, Idera.SQLdm.DesktopClient.Designers")]
    public partial class Office2007PropertyPage : UserControl
    {
        public Office2007PropertyPage()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientPanel ContentPanel
        {
            get { return contentPanel; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderWidth
        {
            get { return backgroundPanel.BorderWidth; }
            set { backgroundPanel.BorderWidth = value; }
        }


        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return descriptionLabel.Text; }
            set { descriptionLabel.Text = value; }
        }

        /// <summary>
        /// Property to put or get the header image for the 'properties' page. The value to put to
        /// this property cannot be null.
        /// </summary>
        [Category("Appearance")]
        public Image HeaderImage
        {
            get
            { 
                return _picBoxImageHeader.Image;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("'HeaderImage' cannot be .null'.");
                }

                _picBoxImageHeader.Image = value;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public enum PropertyPageType
    {
        Page,
        Divider
    }

    /*
    class PropertyPageDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            Office2007PropertyPage control = (Office2007PropertyPage)component;
            EnableDesignMode(control.ContentPanel, "ContentPanel");
        }
    }
     */
}
