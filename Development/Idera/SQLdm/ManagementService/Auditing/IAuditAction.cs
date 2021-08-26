using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.ManagementService.Auditing
{
    interface IAuditAction
    {        
        void FillEntity(ref MAuditableEntity entity);
        AuditableActionType Type { get; set; }
    }
}
