using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Helpers
{
    public interface ICredentialDialog
    {
        string Message { set; get; }
        string Caption { set; get; }
        string User { set; get; }
        string Password { set; get; }

        bool ShowDialog(IntPtr parent);
    }
}
