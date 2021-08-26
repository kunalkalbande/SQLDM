using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Services
{
    public interface ICommonAssemblyInfo
    {
        /// <summary>
        /// Returns the Version Attribute of the Common Assembly
        /// </summary>
        /// <returns></returns>
        string GetCommonAssemblyVersion();
        
        /// <summary>
        /// Returns the Information Version Attribute of the Common Assembly
        /// </summary>
        /// <returns></returns>
        string GetCommonAssemblyInformationVersion();
    }
}
