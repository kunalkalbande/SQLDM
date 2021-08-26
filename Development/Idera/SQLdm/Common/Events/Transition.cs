//------------------------------------------------------------------------------
// <copyright file="Transition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Events
{
    [Flags,Serializable]
    public enum Transition : byte
    {
        OK_Warning = 0x12,
        OK_Info = 0x13,
        OK_Critical = 0x14,
        Warning_OK = 0x21,
        Warning_Warning = 0x22,
        Warning_Info = 0x23,
        Warning_Critical = 0x24,
        Critical_OK = 0x41,
        Critical_Warning = 0x42,
        Critical_Info = 0x43,
        Critical_Critical = 0x44,
        Info_OK = 0x51,
        Info_Info = 0x52,
        Info_Warning = 0x53,
        Info_Critical = 0x54
    }

}
