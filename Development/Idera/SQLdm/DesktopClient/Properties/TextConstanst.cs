namespace Idera.SQLdm.DesktopClient.Properties
{
    public static class TextConstants
    {
        public const string MaintenanceModeEnableButtonKey = "maintenanceModeEnableButtonKey";
        public const string MaintenanceModeDisableButtonKey = "maintenanceModeDisablettonKey";
        public const string MaintenanceModeScheduleButtonKey = "maintenanceModeScheduleButtonKey";
        public const string MaintenanceModeButtonKey = "MaintenanceModeButtonKey";
        
        public const string SnoozeServersAlertsButtonButtonKey = "snoozeServersAlertsButton";
        public const string ResumeServersAlertsButtonButtonKey = "resumeServersAlertsButton";

        //For Alert Template dialog
        public const string ApplyAlertTemplateTagButtonKey = "applyAlertTemplateTagButtonKey";
        public const string ApplyAlertTemplateTitle = "Apply Alert Template - {0}";
        public const string ApplyAlertTemplateOverTagDescription = "Alert templates allow you to apply generic alert settings to a tag.  " +
                                                                   "Select the name of the alert template that you want to apply to the selected tag.  " +
                                                                   "Note that changes do not affect monitored servers settings until the template is applied to a tag.";
        public const string ApplyAlertTemplateOverSingleServerDescription = "Alert templates allow you to apply generic alert settings to a server.  " +
                                                                            "Select the name of the alert template that you want to apply to the selected server.  " +
                                                                            "Note that changes do not affect your monitored server settings until the template is applied to a server.";

        public const string StartUpTimeLogName = "StartUpTimeLog";//SqlDM10.2--Tushar--Added Start up time logger name in constants file.
    }
}
