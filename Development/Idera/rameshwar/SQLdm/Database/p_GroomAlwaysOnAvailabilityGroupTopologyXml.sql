
------------------------------------------------------------------------------
-- <copyright file="p_GroomAlwaysOnAvailabilityGroupTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_GroomAlwaysOnAvailabilityGroupTopologyXml') is not null)
begin
    drop procedure p_GroomAlwaysOnAvailabilityGroupTopologyXml
end
go
create procedure [dbo].p_GroomAlwaysOnAvailabilityGroupTopologyXml
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    -- Indicate which registeers will be deleted.
    declare @registersToDelete table (
        [GroupId] uniqueidentifier,
        [GroupName] sysname,
        [IsActive] int,
        [IsDelete] int
    )

    -- Get summary for the registers to delete.
    insert into @registersToDelete
    select 
            [Groups].[GroupId],
            [Groups].[GroupName],
            sum(convert(int, [Groups].[Active])) as [IsActive],
            sum(convert(int, [Groups].[Delete])) as [IsDelete]
        from [AlwaysOnAvailabilityGroups] as [Groups]
    group by 
        [Groups].[GroupId],
        [Groups].[GroupName]

    -- Iterate througth the table to set [Delete] = 0
    declare @groupIdCursor uniqueidentifier
    declare @isActive int
    declare @isDelete int
    set rowcount 0
    select 1 from @registersToDelete
    set rowcount 1

    -- Get cursor
    select @groupIdCursor = [GroupId] from @registersToDelete
    select @isActive = [IsActive] from @registersToDelete
    select @isDelete = [IsDelete] from @registersToDelete

    while @@rowcount <> 0
    begin
        set rowcount 0

        if @isActive = 0 and @isDelete = 0
        begin
            -- Update Delete column.
            update [AlwaysOnAvailabilityGroups]
                set
                    [Delete] = 1
            where
                [GroupId] = @groupIdCursor
        end

        delete from @registersToDelete where [GroupId] = @groupIdCursor
        set rowcount 1
        select @groupIdCursor = [GroupId] from @registersToDelete
        select @isActive = [IsActive] from @registersToDelete
        select @isDelete = [IsDelete] from @registersToDelete
    end

    set rowcount 0

    -- Call SP in order to update the Replicas topology.
    exec dbo.p_GroomAlwaysOnAvailabilityReplicaTopologyXml

    return null
end
