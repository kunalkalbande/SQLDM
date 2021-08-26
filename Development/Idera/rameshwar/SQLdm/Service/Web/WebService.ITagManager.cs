using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Net;
using PluginCommon;
using PluginAddInView;
using System.ServiceModel.Web;
using System.Security.Principal;
using Idera.SQLdm.Service.Helpers.CWF;
using Idera.SQLdm.Service.DataModels;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : ITagManager

    {
        /// <summary>
        /// SQldm 10.1 (Pulkit Puri) - For adding new class for tag implementation
        /// </summary>

        public ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> GetGlobalTags()
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            Principal userInfo = GetPrincipalFromCWFHost();
            _eventLog.WriteEntry("userInfoArg received null. " + userInfo.ToString());


            return GetGlobalTagsFromCWF(userInfo);

        }

        public void SynchronizeTags(CreateTags tags)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal
             * Calling CWF Api instead of Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //_myHost.CreateTags(GetProductIdFromRequest(), tags, userInfo);
            Principal userInfo = GetPrincipalFromCWFHost();
            CWFHelper.CreateTags(tags);
            
        }


        public void SynchronizeResources(TagResourcesList list)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal 
             * Calling CWF Api instead of Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //_myHost.AddTagResources(GetProductIdFromRequest(), list, userInfo);
            Principal userInfo = GetPrincipalFromCWFHost();
            CWFHelper.AddTagResources(list);
        }

        public void DeleteTags(Tags tags)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal 
             * Calling CWF Api instead of CWF Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //_myHost.DeleteTags(GetProductIdFromRequest(), tags, userInfo);
            Principal userInfo = GetPrincipalFromCWFHost();
            CWFHelper.DeleteTags(tags);
        }

        public Tag GetTag(string tagID)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal 
             * Calling CWF Api instead of CWF Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //return _myHost.GetTag(GetProductIdFromRequest(), tagID, userInfo);
            Principal userInfo = GetPrincipalFromCWFHost();
            return CWFHelper.GetTag(tagID);
        }

        public void DeleteTagResources(TagResourcesList resourceList)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal 
             * Calling CWF Api instead of CWF Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //_myHost.DeleteTagResources(GetProductIdFromRequest(), resourceList, userInfo);
            Principal userInfo = GetPrincipalFromCWFHost();
            CWFHelper.DeleteTagResources(resourceList);
        }

        public List<Idera.SQLdm.Common.CWFDataContracts.Instance> GetTagInstances(string tagId)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing Type from IPrincipal to Principal 
             * Calling CWF Api instead of CWF Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            //return CWFHelper.ObjectTranslator.TranslateToCWFContract(_myHost.GetTagInstances(GetProductIdFromRequest(), tagId, userInfo));
            Principal userInfo = GetPrincipalFromCWFHost();
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            return ObjectTranslator.TranslateToCWFContract(CWFHelper.GetTagInstances(tagId,header));
        }
    }
}