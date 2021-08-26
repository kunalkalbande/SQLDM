//------------------------------------------------------------------------------
// <copyright file="CWFRegisterConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CWFRegister.Configuration
{
    using BBS.TracerX;

    /// <summary>
    /// CWFRegisterConfiguration
    /// custom initial configurations for register tool
    /// </summary>
    static class CWFRegisterConfiguration
    {
        /// <summary>
        /// Initiation of logging configuration from xml app.config and setting of name for file log
        /// </summary>
        public static void InitLogging()
        {
            Logger.Xml.Configure();
            Logger.FileLogging.Name = "IderaDashboardRegister.tx1";
            Logger.FileLogging.Open();
        }
    }
}
