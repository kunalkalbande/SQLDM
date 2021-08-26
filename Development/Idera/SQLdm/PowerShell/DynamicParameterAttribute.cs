//------------------------------------------------------------------------------
// <copyright file="DynamicParameterAttribute.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell
{
    public sealed class DynamicParameterAttribute : Attribute
    {
        public readonly bool Required;
        public readonly string[] ParameterSetNames;

        public DynamicParameterAttribute(bool parameterSetRequired, params string[] parameterSetNames)
        {
            Required = parameterSetRequired;
            ParameterSetNames = parameterSetNames;
        }

        public DynamicParameterAttribute(params string[] parameterSetNames) : this(false, parameterSetNames)
        {
        }
    }
}
