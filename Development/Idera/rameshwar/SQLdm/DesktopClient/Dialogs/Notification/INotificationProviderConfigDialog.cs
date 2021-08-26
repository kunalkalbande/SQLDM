using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Services;

    interface INotificationProviderConfigDialog
    {
        NotificationProviderInfo NotificationProvider
        {
            get;
            set;
        }

        void SetManagementService(IManagementService managementService);

    }
}
