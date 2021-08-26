//------------------------------------------------------------------------------
// <copyright file="AllServersUserView.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class AllServersUserView : UserView
    {
        private const string AllServersUserViewName = "All Servers";
        public static readonly AllServersUserView Instance = new AllServersUserView();

        private UserViewStatus severity = UserViewStatus.None;

        private AllServersUserView()
        {
            Name = AllServersUserViewName;
            ApplicationController.Default.BackgroundRefreshCompleted += new EventHandler<BackgroundRefreshCompleteEventArgs>(BackgroundRefreshCompleted);
        }

        /// <summary>
        /// Returns a read-only collection of instance id's.
        /// </summary>
        public override ICollection<int> Instances
        {
            get
            {
                var i = new List<int>();
                foreach(var inst in ApplicationModel.Default.RepoActiveInstances)
                {
                    i.AddRange(inst.Value.Select(x => x.InstanceId).ToList());
                }
                return i;
            }
        }

        public override UserViewStatus Severity
        {
            get
            {
                return severity;
            }
            protected set
            {
                if (value != severity)
                {
                    severity = value;
                    OnSeverityChanged();
                }
            }
        }

        void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            Update();
        }

        public override void Update()
        {
            if (Instances != null)
            {
                ApplicationModel applicationModel = ApplicationModel.Default;
                UserViewStatus uvs = UserViewStatus.None;

                foreach (int id in Instances)
                {
                    if (uvs != UserViewStatus.Critical)
                    {
                        var instanceId = Helpers.RepositoryHelper.GetSelectedInstanceId(id);
                        MonitoredSqlServerStatus status = applicationModel.GetInstanceStatus(instanceId);

                        if (status != null)
                        {
                            if (status.IsInMaintenanceMode)
                            {
                                if (uvs < UserViewStatus.MaintenanceMode)
                                {
                                    uvs = UserViewStatus.MaintenanceMode;
                                }
                            }
                            else
                            {
                                switch (status.Severity)
                                {
                                    case Idera.SQLdm.Common.MonitoredState.Critical:
                                        uvs = UserViewStatus.Critical;
                                        break;
                                    case Idera.SQLdm.Common.MonitoredState.Warning:
                                        if (uvs < UserViewStatus.Warning)
                                            uvs = UserViewStatus.Warning;
                                        break;
                                    case Idera.SQLdm.Common.MonitoredState.OK:
                                        if (uvs < UserViewStatus.OK)
                                            uvs = UserViewStatus.OK;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                Severity = uvs;
            }
        }
    }
}
