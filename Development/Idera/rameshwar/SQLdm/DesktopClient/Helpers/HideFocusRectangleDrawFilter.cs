using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    using System.Diagnostics;

    internal sealed class HideFocusRectangleDrawFilter : IUIElementDrawFilter
    {
        private readonly IUIElementDrawFilter previousFilter;

        internal HideFocusRectangleDrawFilter()
        {
            this.previousFilter = null;
        }

        internal HideFocusRectangleDrawFilter(IUIElementDrawFilter previousFilter)
        {
            this.previousFilter = previousFilter;    
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
//            Debug.Print("HideFocusRectangleDrawFilter phase=" + drawPhase.ToString());
            if (drawPhase == DrawPhase.BeforeDrawFocus)
            {
//                Debug.Print("HideFocusRectangleDrawFilter.DrawElement returning true");
                return true;
            }

            if (previousFilter != null)
                return previousFilter.DrawElement(drawPhase, ref drawParams);

            return false;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            DrawPhase result = DrawPhase.BeforeDrawFocus;
            if (previousFilter != null)
                result |= previousFilter.GetPhasesToFilter(ref drawParams);
            
            return result;
        }
    }
}