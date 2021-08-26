// -----------------------------------------------------------------------
// <copyright file="MAuditableEntity.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.ManagementService.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class MAuditableEntity : AuditableEntity
    {
        private object[] _headerParamList;
        public object[] HeaderParamList
        {
            get { return _headerParamList; }
            protected set { _headerParamList = value; }
        }

        public MAuditableEntity()
        {
        }

        public MAuditableEntity(AuditableEntity auditableEntity)
        {
            if (auditableEntity != null)//to be removed (swati gogia)
            {
                Header = auditableEntity.Header;
                HeaderParamList = null;
                MetaData = auditableEntity.MetaData;
                MetadataProperties = auditableEntity.MetadataProperties;
                Name = auditableEntity.Name;
                SqlUser = auditableEntity.SqlUser;
                TimeStamp = auditableEntity.TimeStamp;
                Workstation = auditableEntity.Workstation;
                WorkstationUser = auditableEntity.WorkstationUser;
            }
        }

        public MAuditableEntity(AuditContextData auditableEntity)
        {
            SqlUser = auditableEntity.SqlUser;
            Workstation = auditableEntity.Workstation;
            WorkstationUser = auditableEntity.WorkstationUser;
        }

        public void SetHeaderParam(params object[] headerParamList)
        {
            this._headerParamList = headerParamList;
        }

        public static MAuditableEntity Clone(AuditableEntity auditableEntity)
        {
            MAuditableEntity mAuditableEntity = new MAuditableEntity
            {
                Header = auditableEntity.Header,
                HeaderParamList = null,
                MetaData = auditableEntity.MetaData,
                MetadataProperties = auditableEntity.MetadataProperties,
                Name = auditableEntity.Name,
                SqlUser = auditableEntity.SqlUser,
                TimeStamp = auditableEntity.TimeStamp,
                Workstation = auditableEntity.Workstation,
                WorkstationUser = auditableEntity.WorkstationUser
            };

            return mAuditableEntity;
        }
    }
}
