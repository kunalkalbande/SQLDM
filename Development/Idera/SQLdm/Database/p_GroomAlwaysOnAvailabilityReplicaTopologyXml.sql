
------------------------------------------------------------------------------
-- <copyright file="p_GroomAlwaysOnAvailabilityReplicaTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_GroomAlwaysOnAvailabilityReplicaTopologyXml') is not null)
begin
    drop procedure p_GroomAlwaysOnAvailabilityReplicaTopologyXml
end
go
create procedure [dbo].p_GroomAlwaysOnAvailabilityReplicaTopologyXml
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    -- Indicate which registeers will be deleted.
    declare @registersToDelete table (
        [ReplicaId] uniqueidentifier,
        [ReplicaName] sysname,
        [IsActive] int,
        [IsDelete] int
    )

    -- Get summary for the registers to delete.
    insert into @registersToDelete
        select
            [Replicas].[ReplicaId],
            [Replicas].[ReplicaName],
            sum(convert(int, [Replicas].[Active])) as [IsActive],
            sum(convert(int, [Replicas].[Delete])) as [IsDelete]
        from [AlwaysOnReplicas] as [Replicas]
    group by 
        [Replicas].[ReplicaId],
        [Replicas].[ReplicaName]

    -- Iterate througth the table to set [Delete] = 1
    declare @replicaIdCursor uniqueidentifier
    declare @isActive int
    declare @isDelete int
    set rowcount 0
    select 1 from @registersToDelete
    set rowcount 1

    -- Get cursor
    select @replicaIdCursor = [ReplicaId] from @registersToDelete
    select @isActive = [IsActive] from @registersToDelete
    select @isDelete = [IsDelete] from @registersToDelete

    while @@rowcount <> 0
    begin
        set rowcount 0

        if @isActive = 0 and @isDelete = 0
        begin
            -- Update [Delete] column.
            update [AlwaysOnReplicas]
                set
                    [Delete] = 1
            where
                [ReplicaId] = @replicaIdCursor
        end

        delete from @registersToDelete where [ReplicaId] = @replicaIdCursor
        set rowcount 1
        select @replicaIdCursor = [ReplicaId] from @registersToDelete
        select @isActive = [IsActive] from @registersToDelete
        select @isDelete = [IsDelete] from @registersToDelete
    end

    set rowcount 0

    -- Goes through the table to get the inactive monitored SQL Servers for replicas.
    declare @inactiveMonitoredSQLServers table (
        [ReplicaTopologyId] bigint,
        [SQLServerID] int,
        [ServerSourceName] sysname,
        [ReplicaName] nvarchar(256)
    )

    -- Get all references for inactive monitored SQL Servers on [AlwaysOnReplicas].
    insert into @inactiveMonitoredSQLServers
    select
            [Replicas].[ReplicaTopologyId],
            [Replicas].[SQLServerID],
            [Replicas].[ServerSourceName],
            [Replicas].[ReplicaName]
        from [AlwaysOnReplicas] as [Replicas] 
            join [MonitoredSQLServers] as [SQLServers] on [Replicas].[SQLServerID] = [SQLServers].[SQLServerID]
            and [Replicas].[Active] = 1 and [SQLServers].[Deleted] = 1

    -- Get the cursor for [@inactiveMonitoredSQLServers]
    declare @replicaTopologyId bigint
    set rowcount 1
    select @replicaTopologyId = [ReplicaTopologyId] from @inactiveMonitoredSQLServers

    while @@rowcount <> 0
    begin
        set rowcount 0

        if @replicaTopologyId is not null
        begin
            -- Proced to update SQLServerId
            update [AlwaysOnReplicas]
                set
                    [SQLServerID] = -1
            where
                [ReplicaTopologyId] = @replicaTopologyId
        end

        delete @inactiveMonitoredSQLServers where [ReplicaTopologyId] = @replicaTopologyId
        set rowcount 1
        select @replicaTopologyId = [ReplicaTopologyId] from @inactiveMonitoredSQLServers
    end

    set rowcount 0

    return null
end
