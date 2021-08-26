//------------------------------------------------------------------------------
// <copyright file="SetSQLdmLoggingCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Text;

namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using BBS.TracerX;

    [Cmdlet("Set", "SQLdmLogging")]
    public class SetSQLdmLoggingCommand : Cmdlet
    {
        [Parameter()]
        public string[] Loggers
        {
            get
            {
                if (loggers == null)
                    return new string[] {"Root"};
                return loggers;
            }
            set { loggers = value; }
        }
        private string[] loggers;

        [Parameter()]
        public TraceLevel FileTraceLevel
        {
            get { return fileTraceLevel;  }
            set
            {
                fileTraceLevel = value;
                fileTraceLevelSet = true;
            }
        }
        private TraceLevel fileTraceLevel;
        private bool fileTraceLevelSet;

        [Parameter()]
        public TraceLevel ConsoleTraceLevel
        {
            get { return consoleTraceLevel; }
            set
            {
                consoleTraceLevel = value;
                consoleTraceLevelSet = true;
            }
        }
        private TraceLevel consoleTraceLevel;
        private bool consoleTraceLevelSet;


        protected override void ProcessRecord()
        {
            foreach (string loggerName in Loggers)
            {
                Logger logger = 
                    String.Equals(loggerName,"Root",StringComparison.CurrentCultureIgnoreCase) 
                        ? Logger.Root 
                        : Logger.GetLogger(loggerName);

                if (logger != null)
                {
                    if (consoleTraceLevelSet)
                        logger.ConsoleTraceLevel = consoleTraceLevel;
                    if (fileTraceLevelSet)
                        logger.FileTraceLevel = fileTraceLevel;
                }
            }
        }
    }
}
