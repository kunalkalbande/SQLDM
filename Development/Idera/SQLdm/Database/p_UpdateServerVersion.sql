if (object_id('p_UpdateServerVersion') is not null)
begin
drop procedure p_UpdateServerVersion
end
go
create procedure p_UpdateServerVersion
@SQLServerID int,
@ServerVersion nvarchar(30),
@ServerEdition nvarchar(30)
as 
begin

	update [MonitoredSQLServers] 
	set [ServerVersion] = @ServerVersion,
		[ServerEdition] = @ServerEdition
	where [SQLServerID] = @SQLServerID

end
