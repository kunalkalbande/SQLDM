using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Policy;

namespace Idera.SQLdoctor.Common.Services
{
    public static class RecommendationServicesFactory
    {
        private const string DEFAULT_SERVICE_DOMAIN = "SQLdoctor";
        private static Dictionary<string, AppDomain> domains = new Dictionary<string, AppDomain>();

        public static void InitDefaultService()
        {
            IRecommendationService irs = RecommendationServicesFactory.GetService<IRecommendationService>();
            irs.Init();
        }

        public static AppDomain GetDomain(string name)
        {
            return GetDomain(name, "AnalysisEngine", false);
        }

        public static AppDomain GetDomain(string name, string applicationName, bool shadowCopyFiles)
        {
            AppDomain result = null;

            lock (domains)
            {
                if (!domains.TryGetValue(name, out result))
                {
                    AppDomainSetup domainSetup = new AppDomainSetup();

                    string appbase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

                    domainSetup.ApplicationBase = appbase;
                    domainSetup.ConfigurationFile = Path.Combine(appbase, "Idera.SQLdoctor.AnanlsysEngine.dll.config");
                    domainSetup.ApplicationName = applicationName;
                    if (shadowCopyFiles)
                        domainSetup.ShadowCopyFiles = "true";
                   
                    result = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, domainSetup);
                    domainSetup = result.SetupInformation;
                    result.DomainUnload += result_DomainUnload;
                    domains.Add(name, result);
                }
            }
            return result;
        }

        static void result_DomainUnload(object sender, EventArgs e)
        {
            lock (domains)
            {
                AppDomain domain = sender as AppDomain;
                if (domain != null)
                    domains.Remove(domain.FriendlyName);
            }
        }

        public static T GetService<T>()
        {
            return GetService<T>(DEFAULT_SERVICE_DOMAIN);
        }

        public static T GetService<T>(string domainName)
        {
            Type t = typeof(T);
            ImplAttribute implInfo = Attribute.GetCustomAttribute(t, typeof(ImplAttribute)) as ImplAttribute;

            if (implInfo != null)
                return (T)GetService(domainName, implInfo.AssemblyName, implInfo.TypeName);

            return default(T);
        }

        public static object GetService(string domainName, string assemblyName, string typeName)
        {
            AppDomain domain = GetDomain(domainName);
            return domain.CreateInstanceAndUnwrap(assemblyName, typeName);
        }
    }
}
