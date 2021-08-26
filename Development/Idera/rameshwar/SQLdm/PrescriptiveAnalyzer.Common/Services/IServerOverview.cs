using System.Collections.Generic;
using Idera.SQLdoctor.Common.Configuration;
using System;

namespace Idera.SQLdoctor.Common.Services
{
    public interface IServerOverview
    {
        ServerVersion ServerVersion { get; }
        bool OleEnabled { get; }
        bool IsSysAdmin { get; }
        DateTime StartupDate { get; }
        TimeSpan UpTime { get; }
        IList<string> Databases { get; }
    }
}
