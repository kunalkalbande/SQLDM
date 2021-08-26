//------------------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Helpers
{
    /// <summary>
    /// Helper class for all math/date methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Gets integral minutes b/w start/end dates
        /// Author: Anshul Aggarwal
        /// </summary>
        public static int? GetMinutes(DateTime? startDateTime, DateTime? endDateTime)
        {
            return startDateTime == null || endDateTime == null ? (int?)null
               : (int)(endDateTime - startDateTime).Value.TotalMinutes;
        }

        /// <summary>
        /// Gets ceil of minutes b/w start/end dates
        /// </summary>
        public static int? GetCeilMinutes(DateTime? startDateTime, DateTime? endDateTime)
        {
            return startDateTime == null || endDateTime == null ? (int?)null
               : (int) Math.Ceiling((endDateTime - startDateTime).Value.TotalMinutes);
        }
    }
}
