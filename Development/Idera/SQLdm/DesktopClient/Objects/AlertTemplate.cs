using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.DesktopClient.Objects
{
    public class AlertTemplate: IAuditable 
    {
        private int templateID;
        private string name;
        private string description;
        private bool defaultIndicator = false;

        public AlertTemplate()
        {
        }

        public AlertTemplate(string templateName, string templateDescription, int templateID)
        {
            this.name = templateName;
            this.description = templateDescription;
            this.templateID = templateID;
        }

        public AlertTemplate(string templateName, string templateDescription, int templateID, bool defaultInd)
        {
            this.name = templateName;
            this.description = templateDescription;
            this.templateID = templateID;
            this.defaultIndicator = defaultInd;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [AuditableAttribute(false)]
        public int TemplateID
        {
            get { return templateID; }
            set { templateID = value; }
        }

        [AuditableAttribute("Template set as 'Default Template'")]
        public bool DefaultIndicator
        {
            get { return defaultIndicator; }
            set { defaultIndicator = value; }
        }


        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {

            AuditableEntity auditableEntity = new AuditableEntity();
            auditableEntity.Name = this.Name;
            auditableEntity.AddMetadataProperty("Template description", String.IsNullOrEmpty(this.Description) ? "None" : this.Description);
            if (this.DefaultIndicator)
            {
                auditableEntity.AddMetadataProperty("Template", "Alert template is now set as 'Default Template'"); 
            }
            
            return auditableEntity;
        }

        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = this.name;
            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);

            foreach (var property in propertiesChanged)
            {
                if (property.Name.Equals("Description") && String.IsNullOrEmpty(property.Value))
                {
                    auditable.AddMetadataProperty(property.Name,"None");
                }
                else
                {
                    auditable.AddMetadataProperty(property.Name, property.Value);
                }
            }

            if (auditable.MetadataProperties.Count == 0 )
            {
                auditable.AddMetadataProperty("Template changed", "No fields affected");
            }
            
            return auditable;
        }
    }
}
