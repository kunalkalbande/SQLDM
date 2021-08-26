using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Idera.SQLdm.DesktopClient.Objects
{
    public class ShowHideItem
    {
        public ICommand ShowHideCommand { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}
