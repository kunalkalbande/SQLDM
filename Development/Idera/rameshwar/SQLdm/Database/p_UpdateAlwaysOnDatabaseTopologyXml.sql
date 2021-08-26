
------------------------------------------------------------------------------
-- <copyright file="p_UpdateAlwaysOnDatabaseTopologyXml.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
if (object_id('p_UpdateAlwaysOnDatabaseTopologyXml') is not null)
begin
drop procedure p_UpdateAlwaysOnDatabaseTopologyXml
end
go
create procedure [dbo].p_UpdateAlwaysOnDatabaseTopologyXml(
    @DatabaseItemsXml xml
)
as
begin
    -- Nolock transactions.
    set transaction isolation level read uncommitted

    -- Declare the table to wrap the XML.
    declare @temporalTopologyTable table (
        AlwaysOnDatabaseId int,
        ReplicaId uniqueidentifier,
        GroupId uniqueidentifier,
        GroupDatabaseId uniqueidentifier,
        DatabaseId int,
        DatabaseName nvarchar(128),
        ServerSourceName sysname
    )

    -- Get the XML in a data model representation.
    declare @status int
    declare @docpointer int
    exec @status = sp_xml_preparedocument @docpointer output, @DatabaseItemsXml

    -- @status 0 (success) or >0 (failure)
    if @status = 0
    begin
        -- If the XML could be converted in its data model representation.
        insert into @temporalTopologyTable
        select
            AlwaysOnDatabaseId,
            ReplicaId,
            GroupId,
            GroupDatabaseId,
            DatabaseId,
            DatabaseName,
            ServerSourceName
        from openxml (@docpointer, '//DatabaseItems/AlwaysOnDatabase', 0)
        with(
            AlwaysOnDatabaseId int 'NULL',
            ReplicaId uniqueidentifier 'ReplicaId',
            GroupId uniqueidentifier 'GroupId',
            GroupDatabaseId uniqueidentifier 'GroupDatabaseId',
            DatabaseId int 'DatabaseId',
            DatabaseName nvarchar(128) 'DatabaseName',
            ServerSourceName sysname 'ServerSourceName'
            )

        -- Goes through each row our temporary table.
        declare @alwaysOnDatabaseCursor int
        set rowcount 0
        select 1 from @temporalTopologyTable
        set rowcount 1
        --update #mytemp set mykey = 1
        update @temporalTopologyTable set AlwaysOnDatabaseId=1

        -- Get the cursor
        select @alwaysOnDatabaseCursor = AlwaysOnDatabaseId from @temporalTopologyTable

        while @@rowcount <> 0
        begin
            set rowcount 0
            -- Get the database id for a row.
            declare @alwaysOnDatabaseId int
            select @alwaysOnDatabaseId = 1

            -- Define scalar values
            declare @replicaId uniqueidentifier
            declare @groupId uniqueidentifier
            declare @groupDatabaseId uniqueidentifier
            declare @databaseId int
            declare @databaseName sysname
            declare @serverSourceName sysname
            declare @delete bit

            -- Get scalar values
            select @replicaId = ReplicaId from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @groupId = GroupId from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @groupDatabaseId = GroupDatabaseId from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @databaseId = DatabaseId from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @databaseName = DatabaseName from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @serverSourceName = ServerSourceName from @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId
            select @delete = 0

            if exists(select 1 from [dbo].[AlwaysOnDatabases]
                        where GroupId = @groupId and ReplicaId = @replicaId and DatabaseID = @databaseId and ServerSourceName = @serverSourceName)
            begin
                -- Update the row
                update [dbo].[AlwaysOnDatabases]
                    set
                    [ReplicaId] = @replicaId,
                    [GroupId] = @groupId,
                    [GroupDatabaseId] = @groupDatabaseId,
                    [DatabaseID] = @databaseId,
                    [DatabaseName] = @databaseName,
                    [Delete] = @delete
                where GroupId = @groupId and ReplicaId = @replicaId and DatabaseID = @databaseId and ServerSourceName = @serverSourceName
            end
            else
            begin
                -- Insert the row
                insert into [dbo].[AlwaysOnDatabases] (
                    [ReplicaId],
                    [GroupId],
                    [GroupDatabaseId],
                    [DatabaseID],
                    [DatabaseName],
                    [ServerSourceName],
                    [Delete]
                    )
                values (
                    @replicaId,
                    @groupId,
                    @groupDatabaseId,
                    @databaseId,
                    @databaseName,
                    @serverSourceName,
                    @delete
                    )
            end

            delete @temporalTopologyTable where AlwaysOnDatabaseId = @alwaysOnDatabaseId

            set rowcount 1
            update @temporalTopologyTable set AlwaysOnDatabaseId=1
        end

        set rowcount 0

        -- Call SP in order to update the Databases topology.
        exec dbo.p_GroomAlwaysOnDatabaseTopologyXml @DatabaseItemsXml = @DatabaseItemsXml
    end
    else
    begin
        -- Error occurs the xml cannot be retrieved
        select @DatabaseItemsXml
    end

    return @status
end
