namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    interface IDashboardProperties
    {
        bool HasConfiguration { get; }
        bool HasDashboardProperties { get; }
        bool IsDashboardPropertiesVisible { get; set; }
        void ShowConfigurationDialog();
    }
}
