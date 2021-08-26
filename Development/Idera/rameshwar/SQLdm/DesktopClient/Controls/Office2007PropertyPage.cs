using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [Designer(typeof(PropertyPageDesigner))]
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

        [Category("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Image HeaderImage
        {
            get { return headerImagePanel.BackgroundImage; }
            set { headerImagePanel.BackgroundImage = value; }
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

    class PropertyPageDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            Office2007PropertyPage control = (Office2007PropertyPage)component;
            EnableDesignMode(control.ContentPanel, "ContentPanel");
        }
    }
}
