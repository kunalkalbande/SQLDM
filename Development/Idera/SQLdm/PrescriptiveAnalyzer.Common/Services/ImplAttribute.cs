using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Services
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ImplAttribute : Attribute
    {
        public readonly string AssemblyName;
        public readonly string TypeName;
        public ImplAttribute(string assemblyName, string typeName)
        {
            AssemblyName = assemblyName;
            TypeName = typeName;
        }
    }
}
