using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Presenters
{
    interface IValidate
    {        
        bool Validate(object data, out string description);
    }
}
