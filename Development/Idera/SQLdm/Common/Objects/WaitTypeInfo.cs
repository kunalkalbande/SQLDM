using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Objects
{
    public class WaitTypeInfo
    {
        public string Name;
        public int    CategoryId;
        public string CategoryName;
        public string Description;
        public string HelpLink;

        public WaitTypeInfo(string name, int categoryId, string categoryName, string description, string helpLink)
        {
            this.Name         = name;
            this.CategoryId   = categoryId;
            this.CategoryName = categoryName;
            this.Description  = description;
            this.HelpLink     = helpLink;
        }
    }
}
