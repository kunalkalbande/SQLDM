//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{

    /// <summary>
    /// Prepares the environment to log changes for Grooming Action
    /// </summary>
    class GroomingAction : IAuditAction
    {
        private GroomingConfiguration newValue = null;
        private GroomingConfiguration oldValue = null;

        public AuditableActionType Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="type"></param>
        public GroomingAction(GroomingConfiguration newValue, GroomingConfiguration oldValue, AuditableActionType type)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;

            this.Type = type;
        }

        public GroomingAction()
        {}

        /// <summary>
        /// Fills the entity with Grooming information
        /// </summary>
        /// <param name="entity"></param>
        public void FillEntity(ref MAuditableEntity entity)
        {
            if(newValue == null && oldValue == null)
            {
                GroomNowAction(ref entity);
            }
            else
            {
                entity = GetConfigurationEntity();   
            }
        }

        public void GroomNowAction(ref MAuditableEntity entity)
        {
            var contextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
            var groomEntity = MAuditingEngine.Instance.PopAuxiliarData("GroomEntity");

            if (groomEntity == null)
            {
                entity = null;
                return;
            }

            entity = new MAuditableEntity(groomEntity as AuditableEntity);
            entity.SetHeaderParam(contextData == null
                                      ? ManagementServiceConfiguration.RepositoryUser
                                      : contextData.SqlUser);

            Type = AuditableActionType.GroomNow;
        }

        /// <summary>
        /// Resturns an Auditable Entity with Grooming data
        /// </summary>
        /// <returns></returns>
        private MAuditableEntity GetConfigurationEntity()
        {
            MAuditableEntity entityResult = null;

            var comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, newValue);

            if (propertiesChanged.Count > 0)
            {
                entityResult = new MAuditableEntity();
                entityResult.Name = "Grooming configuration";
            }

            foreach (var property in propertiesChanged)
            {
                if ((newValue.ScheduleSubDayType == GroomingConfiguration.SubDayType.Hours && property.Name.Equals("Scheduled Grooming Time")) ||
                    (newValue.AggregationSubDayType == GroomingConfiguration.SubDayType.Hours && property.Name.Equals("Scheduled Aggregation Time")))
                {
                    entityResult.AddMetadataProperty(property.Name + " [Hours]", TimeSpan.Parse(property.Value).Seconds.ToString());
                }
                else
                {
                    entityResult.AddMetadataProperty(property.Name, property.Value);
                }
            }

            return entityResult;
        }
    }
}
