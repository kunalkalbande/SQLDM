// -----------------------------------------------------------------------
// <copyright file="SessionAction.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class SessionAction : IAuditAction
    {
        public AuditableActionType Type { get; set; }

        public SessionAction(AuditableActionType type)
        {
            Type = type;
        }

        public void FillEntity(ref MAuditableEntity entity)
        {
            var contextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
            object contextEntity = MAuditingEngine.Instance.PopAuxiliarData("KillSessionEntity");


            if (contextEntity == null)
            {
                entity = null;
                return;
            }

            
            string workstation = contextData == null ? string.Empty : contextData.Workstation;
            
            entity = new MAuditableEntity(contextEntity as AuditableEntity);
            entity.SetHeaderParam(entity.SqlUser, entity.GetPropertyValue("SPID"), workstation);
        }
    }
}
