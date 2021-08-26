using BBS.TracerX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Management.Instrumentation;
using System.IO;
using System.Configuration;
using PluginAddInView;
using PluginCommon;
using Idera.SQLdm.Service;
using System.IdentityModel;
using System.Security.Principal;
using System.Security;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.ConstrainedExecution;
using System.Data;

namespace Idera.SQLdm.Service.Runner
{
    class Program
    {
        
        static void Main(string[] args)
        {

            string SQLDM_REPO_LOCATION = @"FREEDOM\FOR_IDERA;IderaDashboardRepository_2108_1;";
            string SQLDM_REPO_USER = @"FREEDOM\Administrator";
            string SQLDM_REPO_PASSWORD = "admin"; //encrypted password:"bRSpoMdSCQSVz2ytrCfi2Q==";
            string SQLDM_PRODUCT_ID = @"2";
            string SQLDM_HOSTED_URL = "http://localhost:9292/SQLdm/";



            for (int i=0;i < args.Length; i++)
            {
                if (string.Compare("-repolocation", args[i], true)==0) 
                {
                    SQLDM_REPO_LOCATION = (args.Length> i+1 && args[i + 1]!=null)? args[i+1]:null ;
                }

                if (string.Compare("-repouser", args[i], true) == 0)
                {
                    SQLDM_REPO_USER = (args.Length > i + 1 && args[i + 1] != null) ? args[i + 1] : null;
                }

                if (string.Compare("-repopwd", args[i], true) == 0)
                {
                    SQLDM_REPO_PASSWORD = (args.Length > i + 1 && args[i + 1] != null) ? args[i + 1] : null;
                }

                if (string.Compare("-pid", args[i], true) == 0)
                {
                    SQLDM_PRODUCT_ID = (args.Length > i + 1 && args[i + 1] != null) ? args[i + 1] : null;
                }

                if (string.Compare("-url", args[i], true) == 0)
                {
                    SQLDM_HOSTED_URL = (args.Length > i + 1 && args[i + 1] != null) ? args[i + 1] : null;
                }

                
            }
            
            
            //creating connection credentials
            ConnectionCredentials ccDefaultCredentials = new ConnectionCredentials();
            ccDefaultCredentials.Location = SQLDM_REPO_LOCATION; //
            ccDefaultCredentials.ConnectionUser = SQLDM_REPO_USER;
            //ccDefaultCredentials.ConnectionPassword = Idera.SQLdm.Common.Security.Encryption.EncryptionHelper.QuickEncryptForCWF(SQLDM_REPO_PASSWORD);
            ccDefaultCredentials.ConnectionPassword = SQLDM_REPO_PASSWORD;

            AutomationHost aHost = new AutomationHost(ccDefaultCredentials);
            SQLdmAddInHook sqldmService = new SQLdmAddInHook();
            sqldmService.Initialize(aHost);
            sqldmService.StartREST(SQLDM_PRODUCT_ID, SQLDM_HOSTED_URL + SQLDM_PRODUCT_ID);//the hardcoded product id is irrelevant in our context but just respecting the signature
            Console.WriteLine("services have been started");
            Console.ReadKey();
        }

        public class AutomationHost : HostObject
        {
            #region Data
            private static readonly Logger LogX = Logger.GetLogger("AutomationHost");
            private ConnectionCredentials _repoCredentials = null;
            private RepositoryHelper _ideraDashboardRepoHelper = null;
            #endregion

            #region Constructor
            public AutomationHost(ConnectionCredentials repoConnCredentials)
            {
                _repoCredentials = repoConnCredentials;
                _ideraDashboardRepoHelper = RepositoryHelper.GetHelper(repoConnCredentials);
            }
            #endregion

            #region Products

            public override CreateInstanceResults SynchronizeInstances(string productId, CreateInstances instances, IPrincipal principal)
            {
                //ToDo: Should not return null instead return an InvalidUserException
                if (principal == null)
                    return null;
                return new CreateInstanceResults();
            }

            public override void SynchronizeAlerts(string productId, Alerts alerts, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.AlertSynchronization(productId, alerts);
            }
            public override void SynchronizeSecurity(string productId, Configurations configurations, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.AddSecurityConfigurations(productId, configurations);
            }
            public override Product GetProduct(string productId, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new Product();
            }
            #endregion
            #region "Authentication and Repo Credentials"
            public override IPrincipal GetPrincipal(string header)
            {
                string userName,password;
                //CustomAuthenticationManager authenticationManager = new CustomAuthenticationManager();
                if (header!=null && header.StartsWith("BASIC ", StringComparison.InvariantCultureIgnoreCase)) 
                {
                    string authEncodedToken = System.Text.RegularExpressions.Regex.Replace(header, "Basic", string.Empty,System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    byte[] arrEncoded = Convert.FromBase64String(authEncodedToken);
                    string authDecodedToken = Encoding.UTF8.GetString(arrEncoded);
                    if (authDecodedToken != null && !String.IsNullOrWhiteSpace(authDecodedToken))
                    {
                        string[] splittedAuthToken = authDecodedToken.Split(':');
                        if (splittedAuthToken.Length >= 0) 
                        {
                            userName = splittedAuthToken[0];
                            password = splittedAuthToken[1];
                            return Validate(userName, password);
                        }
                    }
                    return null;
                }
                    
                //if (header.StartsWith("ICSToken ", StringComparison.InvariantCultureIgnoreCase))
                //    return authenticationManager.SwtAuthentication(header);
                return null;
            }

            public override IPrincipal GetPrincipalWithDomain(string header)
            {
                return null;
            }

            public IPrincipal Validate(string userName, string password)
            {
                string[] nativeGroups = null;
                IPrincipal principal = null;
                string domain = null;
                const char DOMAIN_SPLIT_CHARACTER = '\\';
                var parts = userName.Split(DOMAIN_SPLIT_CHARACTER);
                if (parts.Length != 1)
                {
                    if (parts.Length != 2) throw new ArgumentException("Incorrect user name format");
                    if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is not specified.");

                    userName = parts[1];
                    domain = parts[0];

                    using (var ctx = new ImpersonationContext(domain, userName, password))
                    {

                        // Validate password.
                        ctx.LogonUser();
                        // Get native windows groups by doing query through the repository SQL Server.
                        // It is quite likely that the user will not have permission to access the repository,
                        // in which case we need to get the native groups some other way.    For now we are
                        // saying there are no windows groups retrieved so ignoring them.
                        try
                        {

                            ctx.RunAs(() =>
                            {
                                nativeGroups = GetGroups(WindowsIdentity.GetCurrent().Token).ToArray();
                            });
                        }
                        catch (Exception ex)
                        {
                            // Ignore this exception, we don't have windows groups at this point.
                            LogX.ErrorCall("Something went wrong getting the groups for this user: " + ex.Message);
                        }

                        // Build the return prinicipal.
                        var wi = new GenericIdentity(userName, "Windows");
                        
                        LogX.InfoCall("\n\nGroups for this user: " + string.Join(",", nativeGroups));
                        ReadOnlyCollection<User> groupUsers = getGroupUsers(nativeGroups, domain + DOMAIN_SPLIT_CHARACTER + userName);
                        principal = new PluginCommon.Authentication.SimplePrincipal(wi, nativeGroups, groupUsers);
                        return principal;
                    }

                }
                else
                {
                    // We assume domain name will always be present, if the same is absent, we are considering this
                    // to be invalid user.
                    LogX.InfoCall("\n\n Attempt to login without passing the domain name.");
                    return null;
                }
            }

            // Get all groups.
            private List<string> GetGroups(IntPtr userToken)
            {
                List<string> result = new List<string>();
                try
                {
                    WindowsIdentity wi = new WindowsIdentity(userToken);
                    foreach (IdentityReference group in wi.Groups)
                    {
                        result.Add(group.Translate(typeof(NTAccount)).ToString());
                    }
                }
                catch (Exception ex)
                {
                    LogX.ErrorCall("Unable to get the groups for this user." + ex.Message);
                }

                result.Sort();
                return result;
            }

            private ReadOnlyCollection<User> getGroupUsers(string[] nativeGroups,string userName)
            {
                
                List<User> groupUsers = new List<User>();
                User _user = new User(userName,"Windows",true);
                groupUsers.Add(_user);
                //foreach (string group in nativeGroups)
                //{
                //    User _user = RepositoryHelper.GetUser(group);
                //    if (_user != null)
                //        groupUsers.Add(_user);
                //}
                return new ReadOnlyCollection<User>(groupUsers);
            }

            public override ConnectionCredentials GetConnectionCredentialsOfProductInstance(string productId, IPrincipal principal)
            {
                //ToDo: Should not return null instead return an InvalidUserException
                int numericProductId = 0;
                if (principal == null)
                    return null;
                
                if (int.TryParse(productId,out numericProductId))
                    return _ideraDashboardRepoHelper.GetConnectionCredentialsOfProductInstance(numericProductId);
                else return null;
                //return _service.GetConnectionCredentialsOfProductInstance(productId);
            }
            #endregion

            #region Alerts
            public override void CreateAlerts(string productId, Alerts alert, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.CreateAlerts(alert);
            }
            public override Alerts GetFilteredAlerts(int productId = 0, string instance = null, IPrincipal principal = null)
            {
                if (principal == null)
                    return null;
                return new Alerts();
            }
            #endregion

            #region Instances
            public override ProductInstances GetProductInstances(string name, string owner, string location, string op, string managed, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new ProductInstances();//_service.GetProductInstances(name, owner, location, op, managed);
            }
            public override CreateInstanceResults CreateInstances(CreateInstances instance, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new CreateInstanceResults();//_service.CreateInstances(instance);
            }
            public override ProductInstances RegisterProductInstances(string productid, RegisterProductInstances registerproductinstances, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new ProductInstances(); //_service.RegisterProductInstances(productid, registerproductinstances);
            }

            public override void CreateUpdateAlertsMetadata(AlertsMetadata metadata, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.CreateUpdateAlertsMetadata(metadata);
            }
            /// <summary>
            /// SQLDM 10.1 (Barkha Khatri)
            /// unregistering instance logic added
            /// </summary>
            /// <param name="productid"></param>
            /// <param name="instances"></param>
            /// <param name="principal"></param>
            public override void UnregisterProductInstances(string productid, Instances instances, IPrincipal principal)
            {
                if (principal == null)
                {
                    return;
                }
                
            }
            #endregion

            #region Permissions
            public override void AssignPermission(string productId, Permission permission, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.AssignPermission(permission);
            }
            public override void UpdatePermission(string productId, Permission permission, string permission_id, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.UpdatePermission(permission, permission_id);
            }
            public override void RevokePermission(string productId, string permission_id, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.RevokePermission(permission_id);
            }

            public override void AssignInstancePermissions(string productId, string permission_id, PluginCommon.Instance instance, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.AssignInstancePermissions(productId, permission_id, instance, principal);
            }
            public override void RevokeInstancePermissions(string productId, string permission_id, PluginCommon.Instance instance, IPrincipal principal)
            {
                if (principal == null)
                    return;

                //_service.RevokeInstancePermissions(productId, permission_id, instance, principal);
            }
            #endregion

            #region Roles
            public override Roles GetRoles(IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new Roles();//_service.GetRoles();
            }
            #endregion

            #region Users
            public override Users GetUsers(IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new Users();//_service.GetUsers();
            }
            public override User GetUserForName(string username, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new User();//_service.GetUserForName(username);
            }
            public override User GetUser(string userid, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new User();// _service.GetUser(userid);
            }
            public override User CreateUser(CreateUser user, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new User();//_service.CreateUser(user);
            }
            public override void DeleteUser(string userid, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.DeleteUser(userid);
            }
            public override void DeleteUsers(IntIds userids, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.DeleteUsers(userids);
            }
            public override void UpdateUser(string userid, string newname, string isactive, string email, string sid, string displayname, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.UpdateUser(userid, newname, isactive, email, sid);
            }
            public override void EditUser(EditUser user, string userid, IPrincipal principal)
            {
                if (principal == null)
                    return;
                //_service.EditPermissions(user, userid);
            }
            #endregion

            #region Widgets
            public override DashboardWidgets GetWidgets(IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new DashboardWidgets();// _dashboard.GetWidgets();
            }
            public override DashboardWidget GetWidget(string widgetid, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new DashboardWidget();// _dashboard.GetWidget(widgetid);
            }
            public override DashboardWidgets AddWidgets(string productID, Widgets widgets, IPrincipal principal)
            {
                //ToDo: Check If this Method call can be without Authentication
                //if (principal == null)
                //    return null;
                //ToDo: IDASH-212 Check if product can push widgets multiple times
                //Check if the Product is upgrading or Pushing widgets due to restart
                //DashboardWidgets existingwidgets = _dashboard.GetWidgetsByProduct(productID);
                //if (existingwidgets.Count >= widgets.Count)
                //{
                //    return null;
                //}
                return new DashboardWidgets();// _dashboard.AddWidgets(productID, widgets);
            }
            public override Widget UpdateWidgetConfiguration(string productID, string widgetID, UpdateWidget widget, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new Widget();//_dashboard.UpdateWidgetConfiguration(productID, widgetID, widget);
            }

            /// <summary>
            /// SQLDM 10.1 (Barkha Khatri)
            /// </summary>
            /// <param name="productId"></param>
            /// <param name="databases"></param>
            /// <param name="principal"></param>
            /// <returns></returns>
            public override CreateDatabaseResults AddDatabasesToProduct(string productId, CreateDatabases databases, IPrincipal principal)
            {
                if (principal == null)
                {
                    return null;
                }
                return new CreateDatabaseResults();
            }
            #endregion

            #region Databases
            public override ProductDatabases GetProductDatabases(string id, string name, string owner, string location, string op, string managed, IPrincipal principal)
            {
                throw new NotImplementedException();
            }
            public override CreateDatabaseResults CreateDatabases(string productId, CreateDatabases databases, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                return new CreateDatabaseResults();
            }
            public override ProductDatabases RegisterProductDatabases(string productId, RegisterProductDatabases registerproductdatabases, IPrincipal principal)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region Licenses
            public override PluginCommon.License SynchronizeLicense(string productId, CreateLicense license, IPrincipal principal)
            {
                if (principal == null)
                    return null;

                return new PluginCommon.License();
            }
            #endregion

            #region Tags
            public override Tags CreateTags(string productId, CreateTags tags, IPrincipal principal)
            {

                if (principal == null)
                    return null;
                if (GetProduct(productId, principal).IsTaggable)
                {
                    LogX.InfoFormat("Create Tags From Product:{0}", productId);
                    return new Tags();
                }
                else
                    return null;
            }

            public override void DeleteTags(string productId, Tags tags, IPrincipal principal)
            {
                if (principal == null)
                    return;
                if (!GetProduct(productId, principal).IsTaggable)
                    return;
                //Cannot delete other products tags 
                tags.ForEach(delegate(Tag tag)
                {
                    if (tag.Product.Id != Int32.Parse(productId))
                    {
                        return;
                    }
                });

                LogX.InfoFormat("Delete Tags From Product:{0}", productId);
                //RepositoryHelper.DeleteTags(tags);
            }
            public override void AddTagResources(string productId, TagResourcesList resourceList, IPrincipal principal)
            {
                if (principal == null)
                    return;

                if (!GetProduct(productId, principal).IsTaggable)
                    return;

                try
                {
                    foreach (var resources in resourceList)
                    {
                        LogX.InfoFormat("Add Resources to Tags From Product:{0} TagID:{1}", productId, resources.TagID);
                        //RepositoryHelper.AddResourcesToTag(resources, "add");
                    }
                }
                catch (Exception e)
                {
                    LogX.ErrorFormat("Error setting application resource tags {0}", e);
                    throw;
                }
            }
            public override void DeleteTagResources(string productId, TagResourcesList resourceList, IPrincipal principal)
            {
                if (principal == null)
                    return;

                if (!GetProduct(productId, principal).IsTaggable)
                    return;

                try
                {
                    foreach (var resources in resourceList)
                    {
                        LogX.InfoFormat("Delete Resources to Tags From Product:{0}  TagID:{1}", productId, resources.TagID);
                        //RepositoryHelper.AddResourcesToTag(resources, "delete");
                    }
                }
                catch (Exception e)
                {
                    LogX.ErrorFormat("Error deleting application resource tags {0}", e);
                    throw;
                }
            }
            public override Tags GetTags(string productId, IPrincipal principal)
            {
                if (principal == null)
                    return null;

                if (GetProduct(productId, principal).IsTaggable)
                    return new Tags();
                else
                    return new Tags();
            }

            public override Tag GetTag(string productId, string tagID, IPrincipal principal)
            {
                if (principal == null)
                    return null;

                if (GetProduct(productId, principal).IsTaggable)
                    return new Tag();
                else
                    return new Tag();
            }

            public override ProductDatabases GetTagDatabases(string productId, string tagId, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                if (GetProduct(productId, principal).IsTaggable)
                    return new ProductDatabases();// _service.GetTagDatabases(tagId);
                else
                    return new ProductDatabases();
            }

            public override ProductInstances GetTagInstances(string productId, string tagId, IPrincipal principal)
            {
                if (principal == null)
                    return null;
                if (GetProduct(productId, principal).IsTaggable)
                    return new ProductInstances();//_service.GetTagInstances(tagId);
                else
                    return new ProductInstances();
            }

            public override void UpdateAlerts(string productId, Alerts alert, IPrincipal principal)
            {
                
            }

            public override void DeleteAlerts(string productId, Alerts alert, IPrincipal principal)
            {
                
            }

            #endregion
        }

        #region Helper Classes for Auth

        public class ImpersonationContext : IDisposable
        {
            public delegate void Run();

            private readonly string domain;
            private readonly string userName;
            private readonly string password;
            private WindowsIdentity identity;

            public ImpersonationContext(string domain, string userID, string password)
            {
                this.domain = domain;
                // 
                if (String.IsNullOrEmpty(domain) && !userID.Contains("@"))
                {
                    // default to domain of current user (only good for remote machine if remote machine trusts domain)
                    domain = Environment.UserDomainName;
                    if (String.IsNullOrEmpty(domain))
                        // default to local machine account (no good for access to a remote machine)
                        domain = Environment.MachineName;
                }

                this.userName = userID;
                this.password = password;
            }

            public WindowsIdentity Identity { get { return identity; } }

            public void LogonUser()
            {
                if (identity != null)
                {
                    identity.Dispose();
                    identity = null;
                }

                if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
                    return;

                IntPtr userPtr = IntPtr.Zero;
                IntPtr idPtr = IntPtr.Zero;

                try
                {
                    bool rc = Native.LogonUser(
                        userName,
                        domain,
                        password,
                        Native.LOGON32_LOGON_NETWORK_CLEARTEXT,
                        Native.LOGON32_PROVIDER_WINNT50,
                        out userPtr);

                    if (false == rc)
                    {
                        int lastError = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastError);
                    }

                    // duplicate token with impersonation enabled
                    rc = Native.DuplicateToken(userPtr, 2, out idPtr);


                    if (false == rc)
                    {
                        int lastError = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastError);
                    }

                    identity = new WindowsIdentity(idPtr);
                }
                finally
                {
                    if (userPtr != IntPtr.Zero)
                        Native.CloseHandle(userPtr);
                    if (idPtr != IntPtr.Zero)
                        Native.CloseHandle(idPtr);
                }
            }

            public void RunAs(Run runnable)
            {
                var id = identity;
                var current = WindowsIdentity.GetCurrent(false);
                if (id == null)
                    id = current;

                if (id.User.Equals(current.User))
                {
                    runnable.Invoke();
                    return;
                }

                using (var context = id.Impersonate())
                {
                    try
                    {
                        runnable.Invoke();
                    }
                    catch
                    {
                        context.Undo();
                        throw;
                    }
                    finally
                    {
                        context.Undo();
                    }
                }
            }

            public void Dispose()
            {
                if (identity != null)
                {
                    identity.Dispose();
                    identity = null;
                }
            }
        }

        internal class Native
        {
            public const int LOGON32_LOGON_INTERACTIVE = 2;
            public const int LOGON32_LOGON_NETWORK = 3;
            public const int LOGON32_LOGON_BATCH = 4;
            public const int LOGON32_LOGON_SERVICE = 5;
            public const int LOGON32_LOGON_UNLOCK = 7;
            public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
            public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

            public const int LOGON32_PROVIDER_DEFAULT = 0;
            public const int LOGON32_PROVIDER_WINNT50 = 3;
            public const int LOGON32_PROVIDER_WINNT40 = 2;

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, out IntPtr DuplicateTokenHandle);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool RevertToSelf();

            [DllImport("advapi32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

            public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
            public const uint STANDARD_RIGHTS_READ = 0x00020000;
            public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
            public const uint TOKEN_DUPLICATE = 0x0002;
            public const uint TOKEN_IMPERSONATE = 0x0004;
            public const uint TOKEN_QUERY = 0x0008;
            public const uint TOKEN_QUERY_SOURCE = 0x0010;
            public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
            public const uint TOKEN_ADJUST_GROUPS = 0x0040;
            public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
            public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
            public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
            public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                TOKEN_ADJUST_SESSIONID);

            [DllImport("Kernel32.dll", SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            internal static extern bool CloseHandle(IntPtr handle);

            [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            internal static extern bool AdjustTokenPrivileges(
                [In]     IntPtr TokenHandle,
                [In]     bool DisableAllPrivileges,
                [In]     ref TOKEN_PRIVILEGE NewState,
                [In]     uint BufferLength,
                [In, Out] ref TOKEN_PRIVILEGE PreviousState,
                [In, Out] ref uint ReturnLength);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref Int64 lpLuid);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct LUID
            {
                internal long luid;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct LUID_AND_ATTRIBUTES
            {
                internal LUID Luid;
                internal uint Attributes;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct TOKEN_PRIVILEGE
            {
                internal uint PrivilegeCount;
                internal LUID_AND_ATTRIBUTES Privilege;
            }

            public const long SE_PRIVILEGE_ENABLED_BY_DEFAULT = (0x00000001L);
            public const long SE_PRIVILEGE_ENABLED = (0x00000002L);
            public const long SE_PRIVILEGE_REMOVED = (0X00000004L);
            public const long SE_PRIVILEGE_USED_FOR_ACCESS = (0x80000000L);

            public const string SE_CREATE_TOKEN_NAME = "SeCreateTokenPrivilege";
            public const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
            public const string SE_LOCK_MEMORY_NAME = "SeLockMemoryPrivilege";
            public const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
            public const string SE_UNSOLICITED_INPUT_NAME = "SeUnsolicitedInputPrivilege";
            public const string SE_MACHINE_ACCOUNT_NAME = "SeMachineAccountPrivilege";
            public const string SE_TCB_NAME = "SeTcbPrivilege";
            public const string SE_SECURITY_NAME = "SeSecurityPrivilege";
            public const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
            public const string SE_LOAD_DRIVER_NAME = "SeLoadDriverPrivilege";
            public const string SE_SYSTEM_PROFILE_NAME = "SeSystemProfilePrivilege";
            public const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
            public const string SE_PROF_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
            public const string SE_INC_BASE_PRIORITY_NAME = "SeIncreaseBasePriorityPrivilege";
            public const string SE_CREATE_PAGEFILE_NAME = "SeCreatePagefilePrivilege";
            public const string SE_CREATE_PERMANENT_NAME = "SeCreatePermanentPrivilege";
            public const string SE_BACKUP_NAME = "SeBackupPrivilege";
            public const string SE_RESTORE_NAME = "SeRestorePrivilege";
            public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
            public const string SE_DEBUG_NAME = "SeDebugPrivilege";
            public const string SE_AUDIT_NAME = "SeAuditPrivilege";
            public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
            public const string SE_CHANGE_NOTIFY_NAME = "SeChangeNotifyPrivilege";
            public const string SE_REMOTE_SHUTDOWN_NAME = "SeRemoteShutdownPrivilege";
            public const string SE_UNDOCK_NAME = "SeUndockPrivilege";
            public const string SE_SYNC_AGENT_NAME = "SeSyncAgentPrivilege";
            public const string SE_ENABLE_DELEGATION_NAME = "SeEnableDelegationPrivilege";
            public const string SE_MANAGE_VOLUME_NAME = "SeManageVolumePrivilege";
            public const string SE_IMPERSONATE_NAME = "SeImpersonatePrivilege";
            public const string SE_CREATE_GLOBAL_NAME = "SeCreateGlobalPrivilege";
        }
        #endregion
    }
}