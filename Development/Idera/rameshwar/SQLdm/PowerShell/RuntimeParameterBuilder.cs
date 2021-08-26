//------------------------------------------------------------------------------
// <copyright file="RuntimeParameterBuilder.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell
{
    using System.Management.Automation;
    using System.Reflection;

    internal class RuntimeParameterBuilder
    {
        private readonly RuntimeDefinedParameterDictionary parameters;

        public RuntimeParameterBuilder()
        {
            parameters = new RuntimeDefinedParameterDictionary();
        }

        public RuntimeParameterBuilder(object parameters)
        {
            if (parameters is RuntimeDefinedParameterDictionary)
                this.parameters = (RuntimeDefinedParameterDictionary)parameters;
            else
                this.parameters = new RuntimeDefinedParameterDictionary();
        }

        public RuntimeDefinedParameterDictionary Parameters
        {
            get { return parameters; }
        }

        public RuntimeDefinedParameter AddParameter<T>(string name, bool mandatory, params string[] parameterSetName)
        {
            return AddParameter<T>(name, mandatory, false, parameterSetName);
        }

        public RuntimeDefinedParameter AddParameter<T>(string name, bool mandatory, bool parameterSetRequired, params string[] parameterSetName)
        {
            RuntimeDefinedParameter parm = new RuntimeDefinedParameter();
            parm.Name = name;
            parm.ParameterType = typeof(T);

            ParameterAttribute attribute = new ParameterAttribute();
            attribute.Mandatory = mandatory;
            parm.Attributes.Add(attribute);

            if (name.Equals(Constants.CredentialParameter))
                parm.Attributes.Add(new CredentialAttribute());

            if (parameterSetName != null && parameterSetName.Length > 0)
                parm.Attributes.Add(new DynamicParameterAttribute(parameterSetRequired, parameterSetName));

            parameters.Add(name, parm);

            return parm;
        }
    }
}
