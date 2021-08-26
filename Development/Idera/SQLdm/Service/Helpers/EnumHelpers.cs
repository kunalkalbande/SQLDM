using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Service.Helpers
{   
    //SQLdm 8.5 (Gaurav): Added as helper class for enums
    /// <summary>
    /// Provides helpers to operate on enums
    /// </summary>
    public static class EnumHelpers
    {
        public static string GetDatabaseStatusNameFromValue(int value) 
        {
            DatabaseStatus targetEnum = (DatabaseStatus)value;
            return targetEnum.ToString();
        }

        public static string GetFileActivityFileTypeFromValue(int value)
        {
            FileActivityFileType targetEnum = (FileActivityFileType)value;
            return targetEnum.ToString();
        }

        public static string EnumToString(Type argEnum,string val) 
        {
            return Enum.Parse(argEnum, val).ToString();
        }
    }
}
