using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Idera.SQLdm.DesktopClient.Controls.Designers
{
    public class PropertyPageDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            Office2007PropertyPage control = (Office2007PropertyPage)component;
            EnableDesignMode(control.ContentPanel, "ContentPanel");
        }
    }
}
