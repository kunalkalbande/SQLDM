
------------------------------------------------------------------------------
-- <copyright file="p_InsertAlwaysOnStatistics.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_InsertAlwaysOnStatistics') is not null)
begin
drop procedure p_InsertAlwaysOnStatistics
end
go
create procedure [dbo].p_InsertAlwaysOnStatistics(
    @UTCCollectionDateTime datetime,
    @ReplicaId uniqueidentifier,
    @GroupId uniqueidentifier,
    @GroupDatabaseId uniqueidentifier,
    @DatabaseId int,
    @IsFailoverReady bit,
    @SynchronizationState tinyint,
    @SynchronizationHealth tinyint,
    @DatabaseState tinyint,
    @IsSuspended bit,
    @LastHardenedTime datetime,
    @LogSedQueueSize bigint,
    @LogSendRate bigint,
    @RedoQueueSize bigint,
    @RedoRate bigint,
    @ReplicaRole int,
    @OperationalState tinyint,
    @ConnectedState tinyint,
    @SynchronizationHealthAvailabilityReplica tinyint,
    @LastConnectErrorNumber int,
    @LastConnectErrorDescription nvarchar(1024),
    @LastConnectErrorTimestamp datetime,
    @EstimatedDataLossTime bigint,
    @SynchronizationPerformance int,
    @FilestreamSendRate bigint,
    @TimeDeltaInSeconds float,
    @EstimatedRecoveryTime int
)
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    DECLARE @SQLServerID int
    select @SQLServerID = isnull((select top 1 SQLServerID from AlwaysOnReplicas AOR where AOR.ReplicaId = @ReplicaId), -1)

    insert into [dbo].[AlwaysOnStatistics] (
        UTCCollectionDateTime,
        ReplicaId,
        GroupId,
        DatabaseId,
        SQLServerID,
        IsFailoverReady,
        SynchronizationState,
        SynchronizationHealth,
        DatabaseState,
        IsSuspended,
        LastHardenedTime,
        LogSedQueueSize,
        LogSendRate,
        RedoQueueSize,
        RedoRate,
        ReplicaRole,
        OperationalState,
        ConnectedState,
        SynchronizationHealthAvailabilityReplica,
        LastConnectErrorNumber,
        LastConnectErrorDescription,
        LastConnectErrorTimestamp,
        EstimatedDataLossTime,
        SynchronizationPerformance,
        FilestreamSendRate,
        TimeDeltaInSeconds,
        EstimatedRecoveryTime,
        GroupDatabaseId
    )
    values
    (
        @UTCCollectionDateTime,
        @ReplicaId,
        @GroupId,
        @DatabaseId,
        @SQLServerID,
        @IsFailoverReady,
        @SynchronizationState,
        @SynchronizationHealth,
        @DatabaseState,
        @IsSuspended,
        @LastHardenedTime,
        @LogSedQueueSize,
        @LogSendRate,
        @RedoQueueSize,
        @RedoRate,
        @ReplicaRole,
        @OperationalState,
        @ConnectedState,
        @SynchronizationHealthAvailabilityReplica,
        @LastConnectErrorNumber,
        @LastConnectErrorDescription,
        @LastConnectErrorTimestamp,
        @EstimatedDataLossTime,
        @SynchronizationPerformance,
        @FilestreamSendRate,
        @TimeDeltaInSeconds,
        @EstimatedRecoveryTime,
        @GroupDatabaseId
    )
end
