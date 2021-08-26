using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Import {
    
    /// <summary>
    /// This class contains everything sent to the server when the user clicks OK in the import wizard.
    /// </summary>
    public class ImportRequest {
        /// <summary>
        /// "Max login time" field from the 4.x Global Settings dialog.  Seconds.
        /// </summary>
        public int MaxLogonTime {
            get { return _maxLogonTime; }
            set { _maxLogonTime = value; }
        }
        private int _maxLogonTime = 1;

        /// <summary>
        /// "Server command timeout" field from the 4.x Global Settings dialog.  Seconds.
        /// </summary>
        public int CommandTimeout {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }
        private int _commandTimeout = 180;

        /// <summary>
        /// "Server list refresh" field from the 4.x Global Settings dialog.  Seconds.
        /// </summary>
        public int RefreshInterval {
            get { return _refreshInterval; }
            set { _refreshInterval = value; }
        }
        private int _refreshInterval = 360;

        /// <summary>
        /// A list of objects containing the imported settings for each server
        /// selected by the user.
        /// </summary>
        public IList<ServerImport> Servers {
            get {
                if (_servers == null) {
                    _servers = new List<ServerImport>();
                }
                
                return _servers;
            }
        }
        private List<ServerImport> _servers;
    }
}
