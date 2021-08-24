using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace SQLdmCWFInstaller.Helpers
{
    internal class Logger
    {
        public static Logger _logger = null;
        private string _filepath = "SQLdmCWFInstallerLog.log";

        public Logger(string loggerName) 
        {
           // File.Create(_filepath).Close();
        }
        public static Logger GetLogger(string loggerName) 
        {
            if (_logger == null) 
            {
                _logger =  new Logger(loggerName);
                return _logger;
            }
            else return _logger;
        }

        public void Error(string errorStringPrefix,Exception ex) 
        {
            StringBuilder sbContent = new StringBuilder();
            sbContent.Length = 0;
            sbContent.AppendLine("!ERROR!====================" + DateTime.Now.ToString() + "==================!ERROR!");
            sbContent.AppendLine(". Error Details:");
            sbContent.AppendLine(errorStringPrefix);//SQLdm 9.0 (Ankit Srivastava) -- Updated the Logging
            sbContent.AppendLine(ex.Message);
            sbContent.AppendLine("::");
            sbContent.AppendLine(ex.InnerException != null ? ex.InnerException.Message : string.Empty);
            File.AppendAllText(_filepath, sbContent.ToString());
            sbContent.Clear();
            sbContent = null;
        }

        public void Warn(string errorStringPrefix, Exception ex)
        {
            StringBuilder sbContent = new StringBuilder();
            sbContent.Length = 0;
            sbContent.AppendLine("!WARNING!====================" + DateTime.Now.ToString() + "==================!WARNING!");
            sbContent.AppendLine(". Warning Details:");
            sbContent.AppendLine(ex.Message);
            sbContent.AppendLine("::");
            sbContent.AppendLine(ex.InnerException != null ? ex.InnerException.Message : string.Empty);
            File.AppendAllText(_filepath, sbContent.ToString());
            sbContent.Clear();
            sbContent = null;
        }

        public void Info(string infoString)
        {
            StringBuilder sbContent = new StringBuilder();
            sbContent.Length = 0;
            sbContent.AppendLine("!INFO!====================" + DateTime.Now.ToString() + "==================!INFO!");
            sbContent.AppendLine(". Info Details:");
            sbContent.AppendLine(infoString);
            File.AppendAllText(_filepath, sbContent.ToString());
            sbContent.Clear();
            sbContent = null;
        }
    }
}
