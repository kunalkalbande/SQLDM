using System;
using System.Drawing;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinTabs;
using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// This class is used to hide the tab headers of a infragistics wizard.
    /// </summary>
    public class HideTabHeaders : IUIElementCreationFilter
    {
        public HideTabHeaders()
        {

        }
        #region IUIElementCreationFilter Members

        public void AfterCreateChildElements(UIElement parent)
        {
            
        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            if (parent is TabHeaderAreaUIElement)
            {
               TabHeaderAreaUIElement headerArea = parent as TabHeaderAreaUIElement;
                TabLineUIElement line = new TabLineUIElement(parent, headerArea.TabManager);
                Rectangle rect = new Rectangle(headerArea.Rect.X, headerArea.Rect.Y + headerArea.Rect.Height - 2, headerArea.Rect.Width, 2);
                line.Rect = rect;
                headerArea.ChildElements.Add(line);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
    
}
