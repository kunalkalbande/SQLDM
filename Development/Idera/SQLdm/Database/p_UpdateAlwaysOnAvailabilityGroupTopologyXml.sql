
------------------------------------------------------------------------------
-- <copyright file="p_UpdateAlwaysOnAvailabilityGroupTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_UpdateAlwaysOnAvailabilityGroupTopologyXml') is not null)
begin
drop procedure p_UpdateAlwaysOnAvailabilityGroupTopologyXml
end
go
create procedure [dbo].[p_UpdateAlwaysOnAvailabilityGroupTopologyXml](
    @AvailabilityGroupsXml xml
)
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    -- Declare dummy table.
    declare @temporalTopologyTable table (
        GroupId uniqueidentifier,
        GroupName sysname,
        ServerSourceName sysname,
        ListenerDnsName nvarchar(63),
        ListenerPort int,
        ListenerIpAddress nvarchar(48)
        )

    -- Get the XML in its data model representation.
    declare @status int
    declare @docpointer int
    exec @status = sp_xml_preparedocument @docpointer output, @AvailabilityGroupsXml

    -- @status 0 (success) or >0 (failure)
    if @status = 0
    begin
        -- If the XML could be converted in its data model representation.
        insert into @temporalTopologyTable
        -- TODO REview use explicit columns declarations.
        select * from openxml (@docpointer, '//AvailabilityGroup', 0)
            with(
                GroupId uniqueidentifier 'GroupId',
                GroupName sysname 'GroupName',
                ServerSourceName sysname 'ServerSourceName',
                ListenerDnsName nvarchar(63) 'ListenerDnsName',
                ListenerPort int 'ListenerPort',
                ListenerIpAddress nvarchar(48) 'ListenerIPAddress'
                )

        -- Get all nodes on which is perfomr the grooming.
        declare @nodesSubSet table (
            ServerSourceName sysname
        )

        insert into @nodesSubSet
        select distinct [@temporalTopologyTable].[ServerSourceName] from @temporalTopologyTable

        -- Get registers to set inactive.
        declare @inactiveRegisters table (
            GroupTopologyId bigint,
            GroupName sysname,
            GroupId uniqueidentifier,
            ServerSourceName sysname
            )

        insert into @inactiveRegisters
        select [Groups].[GroupTopologyId], [Groups].[GroupName], [Groups].[GroupId], [Groups].[ServerSourceName]
            from [dbo].[AlwaysOnAvailabilityGroups] as [Groups]
            left outer join @temporalTopologyTable on [Groups].[GroupId] = [@temporalTopologyTable].[GroupId]
                and [@temporalTopologyTable].[ServerSourceName] = [Groups].[ServerSourceName]
            left join @nodesSubSet on [@nodesSubSet].[ServerSourceName] = [Groups].[ServerSourceName]
        where ([Groups].[GroupId] <> [@temporalTopologyTable].[GroupId]
            or [@temporalTopologyTable].[GroupId] is null
            or [Groups].[GroupId] is null)
            and ([@nodesSubSet].[ServerSourceName] = [Groups].[ServerSourceName])
            and [Groups].[Active] = 1

        -- Goes through Groups to insert / update these.
        declare @inactiveGroupIdCursor bigint
        set rowcount 0
        select 1 from @inactiveRegisters
        set rowcount 1

        -- Get the cursor
        select @inactiveGroupIdCursor = [@inactiveRegisters].[GroupTopologyId] from @inactiveRegisters

        while @@rowcount <> 0
        begin
            set rowcount 0

            if exists(select 1 from [dbo].[AlwaysOnAvailabilityGroups] as [Groups]
                        where [Groups].[GroupTopologyId] = @inactiveGroupIdCursor)
            begin
                -- Set [Active] column to false.
                update [dbo].[AlwaysOnAvailabilityGroups]
                    set
                        [Active] = 0
                where [GroupTopologyId] = @inactiveGroupIdCursor
            end

            delete @inactiveRegisters where [GroupTopologyId] = @inactiveGroupIdCursor

            set rowcount 1
            select @inactiveGroupIdCursor = [@inactiveRegisters].[GroupTopologyId] from @inactiveRegisters
        end

        set rowcount 0

         -- Goes through Replicas and insert / update these.
        declare @groupId uniqueidentifier
        set rowcount 0
        select 1 from @temporalTopologyTable
        set rowcount 1

        -- Get the cursor
        select @groupId = GroupId from @temporalTopologyTable

        while @@rowcount <> 0
        begin
            set rowcount 0

            -- Procede to update / insert the registers in the 'AlwaysOnReplicas'.
            declare @groupName sysname
            declare @serverSourceName sysname
            declare @listenerDnsName nvarchar(63)
            declare @listenerPort int
            declare @listenerIpAddress nvarchar(48)
            declare @active bit
            declare @delete bit

            -- Get scalar values
            select @groupName = GroupName from @temporalTopologyTable where GroupId = @groupId
            -- In a single topology, just have one to match for ServerSourceName
            select @serverSourceName = ServerSourceName from @temporalTopologyTable where GroupId = @groupId
            select @listenerDnsName = ListenerDnsName from @temporalTopologyTable where GroupId = @groupId
            select @listenerPort = ListenerPort from @temporalTopologyTable where GroupId = @groupId
            select @listenerIpAddress = ListenerIpAddress from @temporalTopologyTable where GroupId = @groupId
            select @active = 1
            select @delete = 0

            if exists(select 1 from [dbo].[AlwaysOnAvailabilityGroups]
                        where GroupId = @groupId and ServerSourceName = @serverSourceName)
            begin
                -- Update Availability Replicas
                update [dbo].[AlwaysOnAvailabilityGroups]
                    set
                        [GroupName] = @groupName,
                        [ServerSourceName] = @serverSourceName,
                        [ListenerDnsName] = @listenerDnsName,
                        [ListenerPort] = @listenerPort,
                        [ListenerIpAddress] = @listenerIpAddress,
                        [Active] = @active,
                        [Delete] = @delete
                where GroupId = @groupId and ServerSourceName = @serverSourceName
            end
            else
            begin
                -- Insert Availability Replicas
                insert into [dbo].[AlwaysOnAvailabilityGroups](
                    [GroupId],
                    [GroupName],
                    [ServerSourceName],
                    [ListenerDnsName],
                    [ListenerPort],
                    [ListenerIpAddress],
                    [Active],
                    [Delete]
                    )
                values(
                    @groupId,
                    @groupName,
                    @serverSourceName,
                    @listenerDnsName,
                    @listenerPort,
                    @listenerIpAddress,
                    @active,
                    @delete
                    )
            end

            delete @temporalTopologyTable where GroupId = @groupId

            set rowcount 1
            select @groupId = GroupId from @temporalTopologyTable
        end
        
        set rowcount 0

        -- Call SP in order to update the Replicas topology.
        exec dbo.p_UpdateAlwaysOnAvailabilityReplicaTopologyXml @AvailabilityReplicaXml = @AvailabilityGroupsXml
    end
    else
    begin
        -- An error occurred, the XML cannot be processed.
        select @AvailabilityGroupsXml
    end

    return @status
end
