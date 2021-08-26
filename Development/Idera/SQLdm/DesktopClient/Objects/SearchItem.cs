using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Objects
{
    public class SearchItem
    {
        public string Name { get; set; }
        public object Id;
        public SearchItemType Type;
    }

    public enum SearchItemType
    {
        UserView,
        Tag,
        Server
    }
}
