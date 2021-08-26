if (object_id('p_GetSqlAgentRunning') is not null)
begin
drop procedure p_GetSqlAgentRunning
end
go
create procedure p_GetSqlAgentRunning
	@AgentIsRunning bit output
as
begin

	declare 
		@service_status int,
		@slashpos int, 
		@agentservicename varchar(50),
		@servername varchar(255)

	select @servername = upper(cast(serverproperty('servername')  as nvarchar(255))) 

	select @slashpos = charindex('\', @servername)  

	if @slashpos <> 0 
		begin 
			select @agentservicename = 'SQLAGENT$' + substring(@servername, @slashpos + 1, 30) 
		end  
	else 
		begin 

			select @agentservicename = 'SQLSERVERAGENT' 
		end  

	create Table #runStatus(service_status nvarchar(100))

	insert #runStatus
		exec master..xp_servicecontrol N'querystate', @agentservicename 

	if exists(select * from #runStatus where rtrim(lower(service_status)) like 'running.')
		set @AgentIsRunning = 1
	else
		set @AgentIsRunning = 0

	drop table #runStatus

end



