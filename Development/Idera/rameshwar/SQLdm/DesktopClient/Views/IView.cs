using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views
{
    public interface IView : IDisposable
    {
        bool IsDisposed { get; }

        Logger Log { get; }

        void RefreshView();

        void CancelRefresh();

        void SetArgument(object argument);

        void SaveSettings();

        void ShowHelp();

        void UpdateUserTokenAttributes();

        void UpdateTheme(ThemeName theme);
    }

    public interface IServerView : IView
    {
        int InstanceId { get; }

        DateTime? HistoricalStartDateTime { get; set; }

        DateTime? HistoricalSnapshotDateTime { get; set; }

        ServerViewMode ViewMode { get; set; } // SqlDM 10.2(Anshul Aggarwal) : New History Browser
    }

    public interface IServerDesignView
    {
        string DesignTab { get; set; }

        bool DesignModeEnabled { get; }

        void ToggleDesignMode();

        void ToggleDesignMode(bool enabled);

        void CheckIfSaveNeeded();

        void SaveDashboardDesign();
    }
}
