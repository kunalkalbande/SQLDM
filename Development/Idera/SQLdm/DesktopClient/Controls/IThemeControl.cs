using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public interface IThemeControl
    {
        void UpdateTheme(ThemeName themeName);
    }

    public enum ThemeName
    {
        Light,
        Dark
    }
}
