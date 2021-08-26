
------------------------------------------------------------------------------
-- <copyright file="p_UpdateAlwaysOnAvailabilityReplicaTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_UpdateAlwaysOnAvailabilityReplicaTopologyXml') is not null)
begin
drop procedure p_UpdateAlwaysOnAvailabilityReplicaTopologyXml
end
go
create procedure [dbo].p_UpdateAlwaysOnAvailabilityReplicaTopologyXml(
    @AvailabilityReplicaXml xml
)
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    -- Declare dummy table.
    declare @temporalTopologyTable table (
        ReplicaId uniqueidentifier,
        GroupId uniqueidentifier,
        ServerSourceName sysname,
        SQLServerID int,
        ReplicaName nvarchar(256),
        ReplicaRole int,
        FailoverMode int,
        AvailabilityMode int,
        PrimaryConnectionMode tinyint,
        SecondaryConnectionMode tinyint
        )

    -- Get the XML in its data model representation.
    declare @status int
    declare @docpointer int
    exec @status = sp_xml_preparedocument @docpointer output, @AvailabilityReplicaXml

    -- @status 0 (success) or >0 (failure)
    if @status = 0
    begin
        -- If the XML could be converted in its data model representation.
        insert into @temporalTopologyTable
        select
                ReplicaId,
                GroupId,
                ServerSourceName,
                isnull([SQLServers].[SQLServerID], -1) as [SQLServerID],
                ReplicaName,
                ReplicaRole,
                FailoverMode,
                AvailabilityMode,
                PrimaryConnectionMode,
                SecondaryConnectionMode
            from openxml (@docpointer, '//ReplicaItems/AvailabilityReplica', 0)
            with(
                ReplicaId uniqueidentifier 'ReplicaId',
                GroupId uniqueidentifier 'GroupId',
                ServerSourceName sysname 'ServerSourceName',
                SQLServerID int 'SqlServerId',
                ReplicaName nvarchar(256) 'ReplicaName',
                ReplicaRole int 'ReplicaRoleIntValue',
                FailoverMode int 'FailoverModeIntValue',
                AvailabilityMode int 'AvailabilityModeIntValue',
                PrimaryConnectionMode tinyint 'PrimaryConnectionModeIntValue',
                SecondaryConnectionMode tinyint 'SecondaryConnectionModeIntValue'
                )
            left outer join [dbo].[MonitoredSQLServers] as [SQLServers] on UPPER([SQLServers].[RealServerName]) = UPPER(ReplicaName)
                and [SQLServers].[Deleted] = 0

        -- For all register to set as inactive.
        declare @nodesSubSet table (
            ServerSourceName sysname
        )

        insert into @nodesSubSet
        select distinct [@temporalTopologyTable].[ServerSourceName] from @temporalTopologyTable

        -- Process XML
        declare @registersToRemove table (
            ReplicaTopologyId bigint,
            GroupId uniqueidentifier,
            ReplicaId uniqueidentifier,
            ReplicaName nvarchar(256)
            )

        -- Get the registers to set inactive.
        insert into @registersToRemove
        select [Replicas].[ReplicaTopologyId], [Replicas].[GroupId], [Replicas].[ReplicaId], [Replicas].[ReplicaName]
            from [dbo].[AlwaysOnReplicas] as [Replicas]
            left outer join @temporalTopologyTable on [Replicas].[ReplicaId] = [@temporalTopologyTable].[ReplicaId] 
                and [Replicas].[ServerSourceName] = [@temporalTopologyTable].[ServerSourceName]
            left join @nodesSubSet on [Replicas].[ServerSourceName] = [@nodesSubSet].[ServerSourceName]
        where ([Replicas].[ReplicaId] <> [@temporalTopologyTable].[ReplicaId]
            or [@temporalTopologyTable].[ReplicaId] is null
            or [Replicas].[ReplicaId] is null)
            and (UPPER([Replicas].[ServerSourceName]) = UPPER([@nodesSubSet].[ServerSourceName]))
            and [Replicas].[Active] = 1

        -- Goes through Replicas and insert / update these.
        declare @replicaIdKey bigint
        set rowcount 0
        select 1 from @registersToRemove
        set rowcount 1

        -- Get the cursor
        select @replicaIdKey = [@registersToRemove].[ReplicaTopologyId] from @registersToRemove

        while @@rowcount <> 0
        begin
            set rowcount 0

            if exists(select 1 from [dbo].[AlwaysOnReplicas] as [Replicas]
                        where [Replicas].[ReplicaTopologyId] = @replicaIdKey)
            begin
                -- Set Active to false.
                update [dbo].[AlwaysOnReplicas]
                    set
                        [Active] = 0
                where 
                    ReplicaTopologyId = @replicaIdKey
            end

            delete @registersToRemove where ReplicaTopologyId = @replicaIdKey

            set rowcount 1
            select @replicaIdKey = [@registersToRemove].[ReplicaTopologyId] from @registersToRemove
        end

        set rowcount 0

        -- Goes through Replicas and insert / update these.
        declare @replicaIdCursor uniqueidentifier
        set rowcount 0
        select 1 from @temporalTopologyTable
        set rowcount 1

        -- Get the cursor
        select @replicaIdCursor = ReplicaId from @temporalTopologyTable

        while @@rowcount <> 0
        begin
            set rowcount 0
            -- Get the database id for a row.
            declare @replicaId uniqueidentifier
            select @replicaId = ReplicaId from @temporalTopologyTable
            
            -- Procede to update / insert the registers in the 'AlwaysOnReplicas'.
            -- Define scalar values
            declare @groupId uniqueidentifier
            declare @sqlServerId int
            declare @serverSourceName sysname
            declare @replicaName nvarchar(256)
            declare @replicaRole int
            declare @failoverMode int
            declare @availabilityMode int
            declare @primaryConnectionMode tinyint
            declare @secondaryConnectionMode tinyint
            declare @active bit
            declare @delete bit

            -- Get scalar values
            select @groupId = GroupId from @temporalTopologyTable where ReplicaId = @replicaId
            select @serverSourceName = ServerSourceName from @temporalTopologyTable where ReplicaId = @replicaId
            select @sqlServerId = SQLServerID from @temporalTopologyTable where ReplicaId = @replicaId
            select @replicaName = ReplicaName from @temporalTopologyTable where ReplicaId = @replicaId
            select @replicaRole = ReplicaRole from @temporalTopologyTable where ReplicaId = @replicaId
            select @failoverMode = FailoverMode from @temporalTopologyTable where ReplicaId = @replicaId
            select @availabilityMode = AvailabilityMode from @temporalTopologyTable where ReplicaId = @replicaId
            select @primaryConnectionMode = PrimaryConnectionMode from @temporalTopologyTable where ReplicaId = @replicaId
            select @secondaryConnectionMode = SecondaryConnectionMode from @temporalTopologyTable where ReplicaId = @replicaId
            select @active = 1
            select @delete = 0

			 -- SQLServerId considers left outer join [dbo].[MonitoredSQLServers] and defaults to -1
			-- Add condition to respect FOREIGN KEY constraint fk_AlwaysOnReplicasForServerID during inserts and updates
			if @sqlServerId <> -1
			begin
             if exists(select 1 from [dbo].[AlwaysOnReplicas]
                        where ReplicaId = @replicaId and UPPER(ServerSourceName) = UPPER(@serverSourceName))
             begin
                -- Update Availability Replicas
                update [dbo].[AlwaysOnReplicas]
                    set
                        [ReplicaId] = @replicaId,
                        [GroupId] = @groupId,
                        [ServerSourceName] = @serverSourceName,
                        [SQLServerID] = @sqlServerId,
                        [ReplicaName] = @replicaName,
                        [ReplicaRole] = @replicaRole,
                        [FailoverMode] = @failoverMode,
                        [AvailabilityMode] = @availabilityMode,
                        [PrimaryConnectionMode] = @primaryConnectionMode,
                        [SecondaryConnectionMode] = @secondaryConnectionMode,
                        [Active] = @active,
                        [Delete] = @delete
                where ReplicaId = @replicaId and UPPER(ServerSourceName) = UPPER(@serverSourceName)
             end
             else
             begin
                -- Insert Availability Replicas
                insert into [dbo].[AlwaysOnReplicas](
                    [ReplicaId],
                    [GroupId],
                    [ServerSourceName],
                    [SQLServerID],
                    [ReplicaName],
                    [ReplicaRole],
                    [FailoverMode],
                    [AvailabilityMode],
                    [PrimaryConnectionMode],
                    [SecondaryConnectionMode],
                    [Active],
                    [Delete]
                    )
                values(
                    @replicaId,
                    @groupId,
                    @serverSourceName,
                    @sqlServerId,
                    @replicaName,
                    @replicaRole,
                    @failoverMode,
                    @availabilityMode,
                    @primaryConnectionMode,
                    @secondaryConnectionMode,
                    @active,
                    @delete
                )
             end
			end

            delete @temporalTopologyTable where ReplicaId = @replicaId

            set rowcount 1
            select @replicaId = ReplicaId from @temporalTopologyTable
        end
        
        set rowcount 0

        -- Call SP in order to update the Databases topology.
        exec dbo.p_UpdateAlwaysOnDatabaseTopologyXml @DatabaseItemsXml = @AvailabilityReplicaXml
    end
    
    else
    begin
        -- An error occurred, the XML cannot be processed.
        select @AvailabilityReplicaXml
    end

    return @status
end