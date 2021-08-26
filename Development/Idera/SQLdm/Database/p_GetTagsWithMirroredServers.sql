IF (object_id('p_GetTagsWithMirroredServers') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTagsWithMirroredServers
END
GO

CREATE PROCEDURE [dbo].p_GetTagsWithMirroredServers
	@addSelectRequest bit = 0
AS
BEGIN
	set nocount on
	
	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]
	
	create table #MirroredServers(Server nvarchar(256), id int)
		insert #MirroredServers
		select distinct smss.InstanceName, smss.SQLServerID 
		from MirroringParticipants mp (nolock)
		inner join SQLServerDatabaseNames dbs (nolock) on mp.DatabaseID = dbs.DatabaseID
		inner join #SecureMonitoredSQLServers smss (nolock) on smss.SQLServerID = dbs.SQLServerID

	if @addSelectRequest=1
	begin
		SELECT 0 as 'Id','< All >' as 'Name'
		union
		select t.Id, t.Name from Tags t where exists(select st.TagId as Id
		  from #MirroredServers ms
		inner join ServerTags st (nolock) on st.SQLServerId = ms.id
		where t.Id = st.TagId)
	end
	else
    begin
		select t.Id, t.Name from Tags t where exists(select st.TagId as Id
		  from #MirroredServers ms
		inner join ServerTags st (nolock) on st.SQLServerId = ms.id
		where t.Id = st.TagId)
	end
	drop table #MirroredServers
END