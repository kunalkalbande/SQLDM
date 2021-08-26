using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class TagSecurityAction:IAuditAction
    {
        private Dictionary<int, Tag> _oldTags;
        private Dictionary<int, Tag> _newTags;
        private PermissionDefinition _permission;

        public TagSecurityAction(Dictionary<int, Tag> oldTags, Dictionary<int, Tag> newTags, PermissionDefinition permission )
        {
            this._oldTags = oldTags;
            this._newTags = newTags;
            _permission = permission;
        }

        public void FillEntity(ref MAuditableEntity entity)
        {
            List<int> newTags = new List<int>(_newTags.Keys);
            List<int> oldTags = new List<int>(_oldTags.Keys);

            var tagsAdded = AuditTools.Except(newTags, oldTags);
            var tagsRemoved = AuditTools.Except(oldTags, newTags);

            TagAction action=new TagAction(null,null);
            List<int> permissions = new List<int>();
            permissions.Add(_permission.PermissionID);

            foreach (var tagId in tagsAdded)
            {
                action.AddPermisionData(permissions, AuditableActionType.ApplicationSecurityLinkedToTag,_newTags[tagId]);    
            }

            foreach (var tagId in tagsRemoved)
            {
                action.AddPermisionData(permissions, AuditableActionType.ApplicationSecurityUnlinkedFromTag, _oldTags[tagId]);
            }

            entity = null;
        }

        public AuditableActionType Type { get; set; }
    }
}