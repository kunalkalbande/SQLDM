using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Renderers
{
    public class FlatToolStripRenderer : ToolStripProfessionalRenderer
    {
        public FlatToolStripRenderer()
        {
            this.RoundedEdges = false;            
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            
        }
    }
}
