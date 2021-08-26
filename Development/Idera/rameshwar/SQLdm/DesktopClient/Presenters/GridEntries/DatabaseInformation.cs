using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Presenters.GridEntries
{
    public class DatabaseInformation
    {

        public int DatabaseID { get; set; }

        public string DatabaseName { get; set; }

        public bool IsSystemDatabase { get; set; }

    }
}
