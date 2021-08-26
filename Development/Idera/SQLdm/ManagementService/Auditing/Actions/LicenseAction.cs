// -----------------------------------------------------------------------
// <copyright file="LicenseAction.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class LicenseAction : IAuditAction
    {
        public Common.Auditing.AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            var contextObject = MAuditingEngine.Instance.PopAuxiliarData("LicenseEntity");
            if(contextObject == null)
            {
                entity = null;
                return;
            }

            entity = new MAuditableEntity(contextObject as AuditableEntity);
            Type = AuditableActionType.LicenseUpdated;
        }
    }
}
