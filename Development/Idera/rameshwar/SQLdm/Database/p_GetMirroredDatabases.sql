if (object_id('p_GetMirroredDatabases') is not null)
begin
drop procedure p_GetMirroredDatabases
end
go

Create PROCEDURE [dbo].[p_GetMirroredDatabases]
 @SQLServerIDs nvarchar(4000) = null,
 @addSelectRequest bit = 0
	-- Add the parameters for the stored procure here
AS
BEGIN
declare @xmlDoc int

if @SQLServerIDs is not null 
Begin
	declare @SQLServers table(
			SQLServerID int) 

	-- Prepare XML document if there is one
	exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

	insert into @SQLServers
	select
		ID 
	from openxml(@xmlDoc, '//Srvrs/Srvr', 1)
		with (ID int)
end
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]	

if @addSelectRequest=1
	begin
		declare @Databases table (DatabaseName nvarchar(510), DatabaseID int)
		Insert @Databases
		select distinct dbs.DatabaseName, dbs.DatabaseID 
				from MirroringParticipants mp (nolock)
				inner join SQLServerDatabaseNames dbs (nolock) on mp.DatabaseID = dbs.DatabaseID
				inner join #SecureMonitoredSQLServers mss (nolock) on mss.SQLServerID = dbs.SQLServerID 
		where (dbs.SQLServerID in (select SQLServerID from @SQLServers)
				or @SQLServerIDs is NULL and dbs.[SQLServerID] = dbs.[SQLServerID]) 	
		if @@rowcount = 0
			SELECT '< No Mirrored Databases >' as DatabaseName, 0 as DatabaseID
			union
			Select DatabaseName, DatabaseID from @Databases
		else
			SELECT '< All >' as DatabaseName, 0 as DatabaseID
			union
			Select DatabaseName, DatabaseID from @Databases
	end
	else
	begin
    -- Insert statements for procedure here
		select distinct dbs.DatabaseName, dbs.DatabaseID 
				from MirroringParticipants mp (nolock)
				inner join SQLServerDatabaseNames dbs (nolock) on mp.DatabaseID = dbs.DatabaseID
				inner join #SecureMonitoredSQLServers mss (nolock) on mss.SQLServerID = dbs.SQLServerID 
		where (dbs.SQLServerID in (select SQLServerID from @SQLServers)
				or @SQLServerIDs is NULL and dbs.[SQLServerID] = dbs.[SQLServerID])
	end
END

