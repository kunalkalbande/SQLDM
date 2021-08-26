using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Linq;
using BBS.TracerX;
using Microsoft.Win32.SafeHandles;
using Idera.SQLdm.Service.Repository;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Idera.SQLdm.Service.Core.Security;
using Idera.SQLdm.Service.Helpers.Auth;
namespace Idera.SQLdm.Service.Core.Security
{
    public class CustomAuthenticationManager : ServiceAuthenticationManager
    {
        private readonly Logger LogX = Logger.GetLogger("CustomAuthorizationManager"); 
        private readonly AuthenticationHelper _authHelper = new AuthenticationHelper();
        private Dictionary<string,CachedCredential> cache = new Dictionary<string, CachedCredential>();

        public override ReadOnlyCollection<IAuthorizationPolicy> Authenticate(ReadOnlyCollection<IAuthorizationPolicy> authPolicy, Uri listenUri, ref Message message)
        {
            using (LogX.InfoCall("Authenticate"))
            {
                // suck out the Authorization header and validate it according to the requested type
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                if (header != null)
                {
                    if (header.StartsWith("BASIC ", StringComparison.InvariantCultureIgnoreCase))
                        return BasicAuthentication(header, authPolicy, listenUri, ref message);
                }

                return base.Authenticate(authPolicy, listenUri, ref message);
            }
        }

        private ReadOnlyCollection<IAuthorizationPolicy> BasicAuthentication(string header, ReadOnlyCollection<IAuthorizationPolicy> authPolicy, Uri listenUri, ref Message message)
        {            
            var splittedAuthHeader = header.Split(new char[] { ' ' }, 2);
            if (splittedAuthHeader.Length != 2) throw new SecurityTokenValidationException("User id or password not valid for basic authentication");
            var bytes = Convert.FromBase64String(splittedAuthHeader[1]);
            header = Encoding.ASCII.GetString(bytes);
            LogX.Info("The user name in the token that came in was: " + (header.IndexOf(":") > -1 ? header.Substring(0, header.IndexOf(":")) : String.Empty));//ensuring that the password is not logged in the logs
            splittedAuthHeader = header.Split(new char[] { ':' }, 2);
            if (splittedAuthHeader.Length != 2) throw new SecurityTokenValidationException("User id or password not valid for basic authentication");
            CachedCredential cachedCredential = null;
            var cacheKey = splittedAuthHeader[0].ToLower();

            cache.TryGetValue(cacheKey, out cachedCredential);

            if (cachedCredential == null || cachedCredential.Expires <= DateTime.UtcNow) //either not found or is expired 
            {//added this check before taking lock to ensure once cachedCredential is validated we dont need lock for all the threads. Minimize locking. Double check idiom.
                lock (cache)
                {
                    if (cachedCredential == null)
                    {// check again as a previous thread may have added by now if current thread was waiting for the lock.
                        cache.TryGetValue(cacheKey, out cachedCredential);
                    }
                    if (cachedCredential != null && cachedCredential.Expires <= DateTime.UtcNow)
                    {
                        cache.Remove(cacheKey);
                        cachedCredential = null;
                    }

                    //populate the credentials in cache if cache empty. At this point cache was either not contaning the key or the corresponding value was expired.
                    //so validate against db and if success, store the validated value.
                    if (cachedCredential == null)
                    {
                        //If credentials were invalid, below line will throw exception
                        var principal = _authHelper.Validate(splittedAuthHeader[0], splittedAuthHeader[1]);
                        //caching credentials for five minutes
                        cachedCredential = new CachedCredential() { Expires = DateTime.UtcNow + new TimeSpan(0, 5, 0), Principal = principal };
                        cache.Add(cacheKey, cachedCredential);
                    }
                }

            }
            //if auth fails, code ll never reach here. If it succeeds, cachedCredential will never be null
            message.Properties["Principal"] = cachedCredential.Principal;

            return authPolicy;
        }
       
    }

    class CachedCredential
    {
        public IPrincipal  Principal { get; set; }
        public DateTime    Expires { get; set; }
    }
 }
