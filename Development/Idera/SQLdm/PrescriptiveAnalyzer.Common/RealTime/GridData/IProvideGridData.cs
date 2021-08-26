using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.RealTime.GridData
{
    public interface IProvideGridData
    {
        Int64 GetInt64(string name);
        Int32 GetInt32(string name);
        UInt64 GetUInt64(string name);
        UInt32 GetUInt32(string name);
        bool GetBool(string name);
        string GetString(string name);
    }
}
