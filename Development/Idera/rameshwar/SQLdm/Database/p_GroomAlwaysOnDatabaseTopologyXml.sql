
------------------------------------------------------------------------------
-- <copyright file="p_GroomAlwaysOnDatabaseTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_GroomAlwaysOnDatabaseTopologyXml') is not null)
begin
    drop procedure p_GroomAlwaysOnDatabaseTopologyXml
end
go
create procedure [dbo].p_GroomAlwaysOnDatabaseTopologyXml(
    @DatabaseItemsXml xml
)
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    declare @temporalTopologyTable table (
        AlwaysOnDatabaseId int,
        ReplicaId uniqueidentifier,
        GroupId uniqueidentifier,
        GroupDatabaseId uniqueidentifier,
        DatabaseId int,
        DatabaseName nvarchar(128),
        ServerSourceName sysname
    )

    -- Convert the XML document in a data model representation.
    declare @status int
    declare @docpointer int
    exec @status = sp_xml_preparedocument @docpointer output, @DatabaseItemsXml

    -- @status 0 (success) or >0 (failure)
    if @status = 0
    begin
        -- Fill temporal table
        insert into @temporalTopologyTable
        select * from openxml (@docpointer, '//DatabaseItems/AlwaysOnDatabase', 0)
        with(
            AlwaysOnDatabaseId int 'AlwaysOnDatabaseId',
            ReplicaId uniqueidentifier 'ReplicaId',
            GroupId uniqueidentifier 'GroupId',
            GroupDatabaseId uniqueidentifier 'GroupDatabaseId',
            DatabaseId int 'DatabaseId',
            DatabaseName nvarchar(128) 'DatabaseName',
            ServerSourceName sysname 'ServerSourceName'
            )

        -- Get all nodes on which is perfomr the grooming.
        declare @nodesSubSet table (
            ServerSourceName sysname
        )

        insert into @nodesSubSet
        select distinct [@temporalTopologyTable].[ServerSourceName] from @temporalTopologyTable

        -- Process XML
        declare @registersToRemove table (
            AlwaysOnDatabasesID bigint,
            DatabaseName nvarchar(128),
            ServerSourceName sysname
            )

        -- Get the registers to set delete.
        insert into @registersToRemove
        select [Databases].[AlwaysOnDatabasesID], [Databases].[DatabaseName], [Databases].[ServerSourceName]
            from [dbo].[AlwaysOnDatabases] as [Databases]
            left outer join @temporalTopologyTable on [Databases].[GroupId] = [@temporalTopologyTable].[GroupId]
                and [Databases].[ReplicaId] = [@temporalTopologyTable].[ReplicaId]
                and [Databases].[DatabaseID] = [@temporalTopologyTable].[DatabaseId]
                and [Databases].[ServerSourceName] = [@temporalTopologyTable].[ServerSourceName]
            left join @nodesSubSet on [@nodesSubSet].[ServerSourceName] = [Databases].[ServerSourceName]
        where ([Databases].[DatabaseID] <> [@temporalTopologyTable].[DatabaseId]
            or [@temporalTopologyTable].[DatabaseId] is null or [Databases].[DatabaseID] is null)
            and ([@nodesSubSet].[ServerSourceName] = [Databases].[ServerSourceName])
            and [Databases].[Delete] = 0

        -- Declare the cursor.
        declare @alwaysOnDatabaseCursor int
        set rowcount 0
        select 1 from @registersToRemove
        set rowcount 1

        select @alwaysOnDatabaseCursor = AlwaysOnDatabasesID from @registersToRemove

        -- Goes through all registers to set inactive.
        while @@rowcount <> 0
        begin
            set rowcount 0
            -- Get the database id for a row.
            declare @alwaysOnDatabaseId int
            select @alwaysOnDatabaseId = AlwaysOnDatabasesID from @registersToRemove

            if exists(select 1 from [dbo].[AlwaysOnDatabases]
                        where AlwaysOnDatabasesID = @alwaysOnDatabaseId)
            begin
                -- Update [Delete] field.
                update [dbo].[AlwaysOnDatabases]
                    set
                        [Delete] = 1
                where AlwaysOnDatabasesID = @alwaysOnDatabaseId
            end

            delete @registersToRemove where AlwaysOnDatabasesID = @alwaysOnDatabaseId

            set rowcount 1
            select @alwaysOnDatabaseId = AlwaysOnDatabasesID from @registersToRemove
        end

        set rowcount 0

    end
    else
    begin
        -- An error occurred
        select @DatabaseItemsXml
    end

    return @status
end
