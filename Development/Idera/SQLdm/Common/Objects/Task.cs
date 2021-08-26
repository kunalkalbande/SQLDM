//------------------------------------------------------------------------------
// <copyright file="Task.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;
    using Idera.SQLdm.Common.Events;

    [Serializable]
    [Flags]
    public enum TaskStatus : byte
    {
        None = 0,
        NotStarted = 1,
        InProgress = 2,
        OnHold = 4,
        Completed = 16,    // The stored procs depend on Completed being equal to 16
        NotCompleted = NotStarted | InProgress | OnHold,
        AnyAll = 255
    }

    [Serializable]
    public class Task
    {
        public int Id;
        public string ServerName;
        public string Subject;
        public string Message;
        public string Comments;
        public string Owner;
        public DateTime CreatedOn;
        public DateTime? CompletedOn;
        public TaskStatus Status = TaskStatus.NotStarted;
        public Nullable<Metric> Metric;
        public Nullable<MonitoredState> Severity;
        public Nullable<float> Value; // Why isn't this double?
        public Nullable<int> EventId;
    }
}
