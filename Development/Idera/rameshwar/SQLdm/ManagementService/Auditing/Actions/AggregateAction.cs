// -----------------------------------------------------------------------
// <copyright file="AggregateACtion.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class AggregateAction : IAuditAction
    {
        public Common.Auditing.AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            var contextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
            var aggregateEntity = MAuditingEngine.Instance.PopAuxiliarData("AggregateEntity");

            if (aggregateEntity == null)
            {
                entity = null;
                return;
            }

            entity = new MAuditableEntity(aggregateEntity as AuditableEntity);
            entity.SetHeaderParam(contextData == null
                                      ? ManagementServiceConfiguration.RepositoryUser
                                      : contextData.SqlUser);

            Type = AuditableActionType.AggregateNow;
        }
    }
}
