using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Idera.SQLdm.Common.Attributes;
using Vim25Api;
using System.Net;


namespace Idera.SQLdm.Common.VMware
{
    public class ServiceConnection
    {

        private static readonly string SessionCookie = "%APPDATA%\\Idera\\vc.dat";

        public VimService vimService;
        protected ConnectState state;
        protected ServiceContent serviceContent;
        protected ManagedObjectReference serviceMoRef;

        private string vcAddress;
        private string vcUser;
        private string vcPassword;

        public string VCAddress
        {
            get
            {
                if (!vcAddress.ToLower().StartsWith("https://"))
                    vcAddress = "https://" + vcAddress.ToLower();
                if (!vcAddress.ToLower().EndsWith("/sdk"))
                    vcAddress = vcAddress.ToLower() + "/sdk";

                return vcAddress;
            }
            set { vcAddress = value; }
        }

        public string UserName
        {
            set { vcUser = value; }
        }

        [AuditableAttribute(true, true)]
        public string Password
        {
            set { vcPassword = value; }
        }

        public ServiceConnection(string url)
        {
            state = ConnectState.Disconnected;

            VCAddress = url;

            System.Net.ServicePointManager.CertificatePolicy = new CertPolicy();

            serviceMoRef = new ManagedObjectReference();
            serviceMoRef.type = "ServiceInstance";
            serviceMoRef.Value = "ServiceInstance";
        }

        public void Connect(string user, string pswd)
        {
            if (vimService != null)
            {
                Disconnect();
            }

            try
            {
                UserName = user;
                Password = pswd;

                vimService = new VimService();
                vimService.Url = VCAddress;
                vimService.Timeout = 600000;
                vimService.CookieContainer = new System.Net.CookieContainer();

                serviceContent = vimService.RetrieveServiceContent(serviceMoRef);

                if (serviceContent.sessionManager != null)
                {
                    vimService.Login(serviceContent.sessionManager, vcUser, vcPassword, null);
                }

                state = ConnectState.Connected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Connect(Cookie cookie)
        {
            if (vimService != null)
            {
                Disconnect();
            }

            vimService = new VimService();
            vimService.Url = VCAddress;
            vimService.Timeout = 600000;
            vimService.CookieContainer = new System.Net.CookieContainer();
            vimService.CookieContainer.Add(cookie);

            serviceContent = vimService.RetrieveServiceContent(serviceMoRef);
            state = ConnectState.Connected;

        }

        public VimService Service
        {
            get { return vimService; }
        }

        public ServiceContent ServiceContent
        {
            get { return serviceContent; }
        }

        public ManagedObjectReference PropertyCollector
        {
            get { return serviceContent.propertyCollector; }
        }

        public ManagedObjectReference Root
        {
            get { return serviceContent.rootFolder; }
        }

        public ConnectState State
        {
            get { return state; }
        }

        public void Disconnect()
        {
            if (vimService != null)
            {
                vimService.Logout(serviceContent.sessionManager);
                vimService.Dispose();
                vimService = null;
                serviceContent = null;

                state = ConnectState.Disconnected;
            }
        }

        public void SaveSession(string url)
        {
            Cookie cookie = Service.CookieContainer.GetCookies(new Uri(url))[0];
            BinaryFormatter bf = new BinaryFormatter();
            Stream s = File.Open(SessionCookie, FileMode.Create);
            bf.Serialize(s, cookie);
            s.Close();
        }

        public void LoadSession(string url)
        {
            if (vimService != null)
                Disconnect();

            vimService = new VimService();
            vimService.Url = url;
            vimService.Timeout = 600000;
            vimService.CookieContainer = new System.Net.CookieContainer();

            BinaryFormatter bf = new BinaryFormatter();
            Stream s = File.Open(SessionCookie, FileMode.Open);
            Cookie c = bf.Deserialize(s) as Cookie;
            s.Close();
            vimService.CookieContainer.Add(c);
            serviceContent = vimService.RetrieveServiceContent(serviceMoRef);
            state = ConnectState.Connected;
        }

    }

    public class CertPolicy : ICertificatePolicy
    {

        private enum CertificateProblem : uint
        {
            CertEXPIRED = 0x800B0101,
            CertVALIDITYPERIODNESTING = 0x800B0102,
            CertROLE = 0x800B0103,
            CertPATHLENCONST = 0x800B0104,
            CertCRITICAL = 0x800B0105,
            CertPURPOSE = 0x800B0106,
            CertISSUERCHAINING = 0x800B0107,
            CertMALFORMED = 0x800B0108,
            CertUNTRUSTEDROOT = 0x800B0109,
            CertCHAINING = 0x800B010A,
            CertREVOKED = 0x800B010C,
            CertUNTRUSTEDTESTROOT = 0x800B010D,
            CertREVOCATION_FAILURE = 0x800B010E,
            CertCN_NO_MATCH = 0x800B010F,
            CertWRONG_USAGE = 0x800B0110,
            CertUNTRUSTEDCA = 0x800B0112
        }

        private static Hashtable problem2text;
        private readonly Hashtable request2problems;

        public CertPolicy()
        {
            if (problem2text == null)
            {
                problem2text = new Hashtable();

                problem2text.Add((uint)CertificateProblem.CertEXPIRED,
                   "A required certificate is not within its validity period.");
                problem2text.Add((uint)CertificateProblem.CertVALIDITYPERIODNESTING,
                   "The validity periods of the certification chain do not nest correctly.");
                problem2text.Add((uint)CertificateProblem.CertROLE,
                   "A certificate that can only be used as an end-entity is being used as a CA or visa versa.");
                problem2text.Add((uint)CertificateProblem.CertPATHLENCONST,
                   "A path length constraint in the certification chain has been violated.");
                problem2text.Add((uint)CertificateProblem.CertCRITICAL,
                   "An extension of unknown type that is labeled 'critical' is present in a certificate.");
                problem2text.Add((uint)CertificateProblem.CertPURPOSE,
                   "A certificate is being used for a purpose other than that for which it is permitted.");
                problem2text.Add((uint)CertificateProblem.CertISSUERCHAINING,
                   "A parent of a given certificate in fact did not issue that child certificate.");
                problem2text.Add((uint)CertificateProblem.CertMALFORMED,
                   "A certificate is missing or has an empty value for an important field, such as a subject or issuer name.");
                problem2text.Add((uint)CertificateProblem.CertUNTRUSTEDROOT,
                   "A certification chain processed correctly, but terminated in a root certificate which isn't trusted by the trust provider.");
                problem2text.Add((uint)CertificateProblem.CertCHAINING,
                   "A chain of certs didn't chain as they should in a certain application of chaining.");
                problem2text.Add((uint)CertificateProblem.CertREVOKED,
                   "A certificate was explicitly revoked by its issuer.");
                problem2text.Add((uint)CertificateProblem.CertUNTRUSTEDTESTROOT,
                   "The root certificate is a testing certificate and the policy settings disallow test certificates.");
                problem2text.Add((uint)CertificateProblem.CertREVOCATION_FAILURE,
                   "The revocation process could not continue - the certificate(s) could not be checked.");
                problem2text.Add((uint)CertificateProblem.CertCN_NO_MATCH,
                   "The certificate's CN name does not match the passed value.");
                problem2text.Add((uint)CertificateProblem.CertWRONG_USAGE,
                   "The certificate is not valid for the requested usage.");
                problem2text.Add((uint)CertificateProblem.CertUNTRUSTEDCA,
                   "Untrusted CA");
            }

            request2problems = new Hashtable();
        }

        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
        {
            try
            {
                string ignoreCert = Environment.GetEnvironmentVariable("VI_IGNORECERT");
                if (String.IsNullOrEmpty(ignoreCert) || ignoreCert.ToLower() == "true")
                    return true;
            }
            catch (Exception e)
            {
                return true;
            }

            if (problem == 0)
            {
                ArrayList problemArray = (ArrayList)request2problems[req];

                if (problemArray == null)
                    return true;

                string problemList = "";

                foreach (uint problemCode in problemArray)
                {
                    string problemText = (string)problem2text[problemCode];
                    if (problemText == null)
                        problemText = "Unknown Problem";

                    problemList += "* " + problemText + "\n\n";

                    //TODO: Figure out something to do with the problem text if the certificate is fucked up

                }

                request2problems.Remove(req);
                return true;
            }
            else
            {
                ArrayList problemArray = (ArrayList)request2problems[req];
                if (problemArray == null)
                {
                    problemArray = new ArrayList();
                    request2problems[req] = problemArray;
                }
                problemArray.Add((uint)problem);
                return true;
            }
        }
    }
}
