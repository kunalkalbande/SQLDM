IF (object_id('p_GetTagServersAsXML') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTagServersAsXML
END
GO

create proc [dbo].[p_GetTagServersAsXML]
	@TagID int = null,
	@ServerID int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select @TagID= coalesce(@TagID, 0)
	select @ServerID= coalesce(@ServerID, 0)
	
	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]

	if @ServerID = 0 -- if no server has been selected
	begin
		--We are here if a valid server id has not been entered
		if @TagID = 0
			begin -- no tag selected so do not join to server tags
					select [SQLServerID] as [@ID]
					from #SecureMonitoredSQLServers ms (nolock)
					for xml path('Srvr'),root('Srvrs')
			end
		else --tagid has been selected so we get servers for that tag
			begin
				select Srvrs.[SQLServerId] as [@ID]
				from [ServerTags] Srvrs (nolock) 
				inner join #SecureMonitoredSQLServers ms (nolock)
				on Srvrs.SQLServerId = ms.SQLServerID
				where [TagId] = isnull(@TagID,Srvrs.[TagId])
				for xml path('Srvr'),root('Srvrs')
		end
	end
	else
	--We can only be here if a server if has been entered then regardless of tag we populate the server
	begin
			select @ServerID as [@ID] 
			for xml path('Srvr'),root('Srvrs')
	end
END
