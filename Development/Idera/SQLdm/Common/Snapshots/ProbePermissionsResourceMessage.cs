using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    public interface IProbePermissionsResourceMessage
    {
        string GetString(string key, CultureInfo cultureInfo = null);
    }

    /// <summary>
    /// Helps to read the probe permissions from resource file ProbePermissionsResourceyy
    /// </summary>
    public class ProbePermissionsResourceMessage : IProbePermissionsResourceMessage
    {
        /// <summary>
        /// Resource Manager to read ProbePermissionsResource File
        /// </summary>
        private ResourceManager _resourceManager = null;

        private ProbePermissionsResourceMessage()
        {
            _resourceManager = new ResourceManager("Idera.SQLdm.Common.ProbePermissionsResource",Assembly.GetExecutingAssembly());
        }

        private static readonly Lazy<ProbePermissionsResourceMessage> LazyResourceMessageReader =
            new Lazy<ProbePermissionsResourceMessage>(() => new ProbePermissionsResourceMessage());

        public static ProbePermissionsResourceMessage Instance
        {
            get
            {
                return LazyResourceMessageReader.Value;
            }
        }

        public string GetString(string key, CultureInfo culture = null)
        {
            return culture == null ? _resourceManager.GetString(key) : _resourceManager.GetString(key, culture);                
        }
    }
}
