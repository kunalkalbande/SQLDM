using System;
using System.IO;
using System.Reflection;
using Idera.SQLdoctor.Common.Helpers;
using Microsoft.Win32.TaskScheduler;
using TracerX;
using Idera.SQLdoctor.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Helpers
{
    internal class TaskInfo
    {
        public TaskInfo()
        {
            Enabled = true;
            State = TaskState.Unknown;
            Hidden = true;
            TriggerType = TaskTriggerType.Daily;
            Sunday = Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = true;
            Start = DateTime.Today + TimeSpan.FromHours(DateTime.Now.Hour + 24);
        }

        public bool Enabled { get; set; }
        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public TaskState State { get; set; }
        public int LastTaskResult { get; set; }
        public bool Hidden { get; set; }
        public string LastChangedBy { get; set; }

        public string InstanceName  { get; set; }
        public String TargetApplication { get; set; }
        public String TargetDatabase { get; set; }
        public String TargetCategory { get; set; }
        public int? Duration { get; set; }
        public bool CheckForUpdates { get; set; }
        public bool EnableOleAutomation { get; set; }

        /// <summary>
        /// Type of trigger - only Once and Weekly supported for Dr.
        /// </summary>
        public TaskTriggerType TriggerType { get; set; }
        /// <summary>
        /// DateTime to run Once type trigger.  Time component used for start on recurring triggers.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Weekly triggers - run on Sunday
        /// </summary>
        public bool Sunday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Monday
        /// </summary>
        public bool Monday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Tuesday
        /// </summary>
        public bool Tuesday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Wednesday
        /// </summary>
        public bool Wednesday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Thursday
        /// </summary>
        public bool Thursday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Friday
        /// </summary>
        public bool Friday { get; set; }
        /// <summary>
        /// Weekly triggers - run on Saturday
        /// </summary>
        public bool Saturday { get; set; }
    }
    
    internal static class ScheduleHelper
    {
        private const string TaskNameFormat = "Idera SQL doctor - {0}";
        private const string TaskDescriptionFormat = "Idera SQL doctor Scheduled {0}";

        private static Logger _logX = Logger.GetLogger("ScheduleHelper");

        public static void DeleteTask(ScheduledTaskType taskType)
        {
            string taskName = String.Format(TaskNameFormat, taskType);
            using (TaskService taskService = new TaskService())
            {
                Task task = taskService.GetTask(taskName);
                if (task != null)
                    taskService.RootFolder.DeleteTask(taskName);
            }
        }

        internal static TaskInfo GetTask(ScheduledTaskType taskType)
        {
            TaskInfo result = null;
            string taskName = String.Format(TaskNameFormat, taskType);

            using (TaskService taskService = new TaskService())
            {
                TaskFolder folder = taskService.RootFolder;

                Task t = taskService.GetTask(taskName);
                if (t != null)
                {
                    result = new TaskInfo();
                    result.Enabled = t.Enabled;
                    result.LastRunTime = t.LastRunTime;
                    try
                    {
                        result.LastTaskResult = t.LastTaskResult;
                    }
                    catch (System.Runtime.InteropServices.COMException com)
                    {
                        _logX.Debug("COMException: Error chucked getting last task result: ", com.Message); 
                        result.LastTaskResult = com.ErrorCode;
                    }
                    catch (Exception e)
                    {
                        _logX.Debug("Error chucked getting last task result: ", e.Message, e.GetType().Name);
                        result.LastTaskResult = -1;
                    }
                    result.NextRunTime = t.NextRunTime;
                    result.State = t.State;

                    TaskDefinition def = t.Definition;
                    result.Hidden = def.Settings.Hidden;

                    if (def.Triggers.Count > 0)
                    {
                        Trigger trigger = def.Triggers[0];
                        result.TriggerType = trigger.TriggerType;
                        if (trigger is WeeklyTrigger)
                        {
                            WeeklyTrigger weekly = (WeeklyTrigger)trigger;
                            result.Start = weekly.StartBoundary;
                            DaysOfTheWeek dotw = weekly.DaysOfWeek;
                            result.Monday = (dotw & DaysOfTheWeek.Monday) == DaysOfTheWeek.Monday;
                            result.Tuesday = (dotw & DaysOfTheWeek.Tuesday) == DaysOfTheWeek.Tuesday;
                            result.Wednesday = (dotw & DaysOfTheWeek.Wednesday) == DaysOfTheWeek.Wednesday;
                            result.Thursday = (dotw & DaysOfTheWeek.Thursday) == DaysOfTheWeek.Thursday;
                            result.Friday = (dotw & DaysOfTheWeek.Friday) == DaysOfTheWeek.Friday;
                            result.Saturday = (dotw & DaysOfTheWeek.Saturday) == DaysOfTheWeek.Saturday;
                            result.Sunday = (dotw & DaysOfTheWeek.Sunday) == DaysOfTheWeek.Sunday;
                        } else
                        if (trigger is DailyTrigger)
                        {
                            DailyTrigger dt = (DailyTrigger)trigger;
                            result.Start = dt.StartBoundary;
                        }
                    }
                    if (def.Actions.Count > 0)
                    {
                        ExecAction theAction = def.Actions[0] as ExecAction;
                        string instance, targetApplication, targetDatabase, targetCategory;
                        int? duration;
                        bool check;
                        bool enableOleAutomation;
                        ParseArgs(theAction.Arguments, out instance, out targetApplication, out targetDatabase, out targetCategory, out duration, out check, out enableOleAutomation);
                        result.InstanceName = instance;
                        result.TargetApplication = targetApplication;
                        result.TargetDatabase = targetDatabase;
                        result.TargetCategory = targetCategory;
                        result.Duration = duration;
                        result.CheckForUpdates = check;
                        result.EnableOleAutomation = enableOleAutomation;
                    }
                }
            }

            return result;
        }

        private static void ParseArgs(string args, out string instanceName, out string targetApplication, out string targetDatabase, out string targetCategory, out int? duration, out bool checkUpdates, out bool enableOleAutomation)
        {
            string[] separators = new string[] { " /" };
            instanceName = String.Empty;
            targetApplication = null;
            targetDatabase = null;
            targetCategory = null;
            duration = null;
            checkUpdates = false;
            enableOleAutomation = false;

            if (!String.IsNullOrEmpty(args))
            {
                foreach (string s in args.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (s.StartsWith("Instance=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        instanceName = s.Substring(9);
                    }
                    else if (s.StartsWith("ApplicationName=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            targetApplication = s.Substring(16);
                        }
                        catch { }
                    }
                    else if (s.StartsWith("DatabaseName=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            targetDatabase = s.Substring(13);
                        }
                        catch { }
                    }
                    else if (s.StartsWith("Category=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            targetCategory = s.Substring(9);
                        }
                        catch { }
                    }
                    else if (s.StartsWith("Duration=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            duration = Convert.ToInt32(s.Substring(9));
                        }
                        catch { }
                    }
                    else if (s.Equals("EnableOA", StringComparison.InvariantCultureIgnoreCase))
                    {
                        enableOleAutomation = true;
                    }
                    else if (s.Equals("GetUpdates", StringComparison.InvariantCultureIgnoreCase))
                    {
                        checkUpdates = true;
                    }
                }
            }
        }

        internal static void UpdateTask(ScheduledTaskType taskType, TaskInfo taskInfo, string user, string password)
        {
            string taskName = String.Format(TaskNameFormat, taskType);

            using (TaskService taskService = new TaskService())
            {
                Version ver = taskService.HighestSupportedVersion;
                bool newVer = (ver >= new Version(1, 2));

                TaskDefinition taskDef = null;

                // get the current definition or create a new one
                Task task = taskService.GetTask(taskName);
                if (task != null)
                    taskDef = task.Definition;
                if (taskDef == null)
                    taskDef = taskService.NewTask();

                taskDef.Settings.Enabled = taskInfo.Enabled;
                if (newVer)
                    taskDef.RegistrationInfo.Source = ApplicationHelper.AssemblyProduct;
                taskDef.RegistrationInfo.Author = Environment.UserName;
                taskDef.RegistrationInfo.Description = String.Format(TaskDescriptionFormat, taskType);
                taskDef.Settings.Hidden = taskInfo.Hidden;

                taskDef.Triggers.Clear();
                switch (taskInfo.TriggerType)
                {
                    case TaskTriggerType.Time:
                        var timeTrigger = new TimeTrigger();
                        timeTrigger.StartBoundary = taskInfo.Start;
                        taskDef.Triggers.Add(timeTrigger);
                        break;
                    case TaskTriggerType.Daily:
                        DailyTrigger dt = new DailyTrigger();
                        dt.DaysInterval = 1;
                        dt.StartBoundary = taskInfo.Start;
                        taskDef.Triggers.Add(dt);
                        break;
                    case TaskTriggerType.Weekly:
                        WeeklyTrigger wt = new WeeklyTrigger();
                        wt.StartBoundary = taskInfo.Start;
                        wt.WeeksInterval = 1;
                        DaysOfTheWeek dotw = DaysOfTheWeek.AllDays;
                        if (!taskInfo.Monday)
                            dotw &= ~DaysOfTheWeek.Monday;
                        if (!taskInfo.Tuesday)
                            dotw &= ~DaysOfTheWeek.Tuesday;
                        if (!taskInfo.Wednesday)
                            dotw &= ~DaysOfTheWeek.Wednesday;
                        if (!taskInfo.Thursday)
                            dotw &= ~DaysOfTheWeek.Thursday;
                        if (!taskInfo.Friday)
                            dotw &= ~DaysOfTheWeek.Friday;
                        if (!taskInfo.Saturday)
                            dotw &= ~DaysOfTheWeek.Saturday;
                        if (!taskInfo.Sunday)
                            dotw &= ~DaysOfTheWeek.Sunday;
                        wt.DaysOfWeek = dotw;
                        taskDef.Triggers.Add(wt);
                        break;
                    default:
                        throw new ApplicationException(String.Format("Trigger type {0} is not supported", taskInfo.TriggerType));
                }
                
                if (newVer)
                    taskDef.Actions.Clear();

                string args = String.Format("{0} /Instance={1}", taskType, taskInfo.InstanceName);

                if (!string.IsNullOrEmpty(taskInfo.TargetApplication))
                {
                    args += string.Format(@" /TargetApplication=""{0}""", taskInfo.TargetApplication);
                }

                if (!string.IsNullOrEmpty(taskInfo.TargetDatabase))
                {
                    args += string.Format(@" /TargetDatabase=""{0}""", taskInfo.TargetDatabase);
                }

                if (!string.IsNullOrEmpty(taskInfo.TargetCategory))
                {
                    args += string.Format(@" /TargetCategory=""{0}""", taskInfo.TargetCategory);
                }

                if (taskInfo.Duration.HasValue)
                {
                    args += string.Format(" /Duration={0}", taskInfo.Duration.Value);
                }

                if (taskInfo.CheckForUpdates)
                {
                    args += " /GetUpdates";
                }

                if (taskInfo.EnableOleAutomation)
                {
                    args += " /EnableOA";
                }

                string startDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string path = Path.Combine(startDirectory, "SQLdoctorAnalysisService.exe");

                ExecAction action = new ExecAction(path, args, startDirectory);
                taskDef.Actions.Add(action);

                TaskCreation createType = TaskCreation.CreateOrUpdate;
                if (!taskInfo.Enabled)
                    createType = TaskCreation.Disable;

                TaskLogonType logonType = TaskLogonType.Password;
                if (!taskInfo.Enabled)
                    logonType |= TaskLogonType.InteractiveTokenOrPassword;

                taskService.RootFolder.RegisterTaskDefinition(taskName, taskDef, createType, user, password, logonType, null);
            }
        }

        internal static void DisableTask(ScheduledTaskType taskType)
        {
            string taskName = String.Format(TaskNameFormat, taskType);
            using (TaskService taskService = new TaskService())
            {
                TaskFolder folder = taskService.RootFolder;

                Task t = taskService.GetTask(taskName);
                if (t != null)
                {
                    TaskLogonType logonType = TaskLogonType.Password | TaskLogonType.InteractiveTokenOrPassword;
                    taskService.RootFolder.RegisterTaskDefinition(taskName, t.Definition, TaskCreation.Disable, null, null, logonType, null);
                }
            }
        }
    }
}
