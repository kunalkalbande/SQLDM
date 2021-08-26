IF (object_id('p_GetMonitoredMirroredServers') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMonitoredMirroredServers
END
GO

CREATE PROCEDURE [dbo].[p_GetMonitoredMirroredServers] 
	-- Add the parameters for the stored procedure here
	 @addSelectRequest bit = 0,
	 @tagID int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]

	if @addSelectRequest = 1
	begin
		declare @Servers table (InstanceName nvarchar(512), SQLServerID int)

		Insert @Servers
		select distinct smss.InstanceName, smss.SQLServerID 
			from MirroringParticipants mp (nolock)
			inner join SQLServerDatabaseNames dbs (nolock) on mp.DatabaseID = dbs.DatabaseID
			inner join #SecureMonitoredSQLServers smss (nolock) on smss.SQLServerID = dbs.SQLServerID
			left outer join ServerTags st (nolock) on st.SQLServerId = smss.SQLServerID
			where (st.TagId = isnull(@tagID, st.TagId) or (@tagID is null and st.TagId is null))
		if @@rowcount = 0
			SELECT '< No Mirrored Servers >' as InstanceName, 0 as SQLServerID
			union
			Select InstanceName, SQLServerID from @Servers
		else
			SELECT '< All >' as InstanceName, 0 as SQLServerID
			union
			Select InstanceName, SQLServerID from @Servers
	end
	else
	begin
    -- Insert statements for procedure here
		select distinct smss.InstanceName, smss.SQLServerID 
			from MirroringParticipants mp (nolock)
			inner join SQLServerDatabaseNames dbs (nolock) on mp.DatabaseID = dbs.DatabaseID
			inner join #SecureMonitoredSQLServers smss (nolock) on smss.SQLServerID = dbs.SQLServerID
			left outer join ServerTags st (nolock) on st.SQLServerId = smss.SQLServerID
			where (st.TagId = isnull(@tagID, st.TagId) or (@tagID is null and st.TagId is null))
	end
END
