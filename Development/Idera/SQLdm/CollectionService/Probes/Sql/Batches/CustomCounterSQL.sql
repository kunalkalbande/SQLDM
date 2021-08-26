-- [0] - Object Name
-- [1] - Counter Name
-- [2] - Instance Name

declare 
	@servername varchar(255), 
	@sysperfinfoname varchar(255),
	@slashpos int

select @servername = cast(serverproperty('servername') as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 
		select @sysperfinfoname = 'SQLSERVER'
	end  

select top 1
	isnull(convert(dec(38,0),cntr_value),0)
from 
	master.sys.dm_os_performance_counters
where 
	(lower(object_name) = lower(@sysperfinfoname + ':{0}')
	and lower(counter_name) = lower('{1}')
	and lower(instance_name) = lower('{2}'))
	
if (@@ROWCOUNT = 0)
begin
	if (not exists(select object_name from master.sys.dm_os_performance_counters 
			where lower(object_name) = lower(@sysperfinfoname + ':{0}')))
	begin
		select N'The object name does not exist in sys.dm_os_performance_counters.'
	end
	else
	begin
		if (not exists(select counter_name  from master.sys.dm_os_performance_counters
			where lower(object_name) = lower(@sysperfinfoname + ':{0}') and
				lower(counter_name) = lower('{1}')))		
		begin
			 select N'The counter name does not exist in sys.dm_os_performance_counters for the object.'
		end
		else
		begin
			if (not exists(select instance_name from master.sys.dm_os_performance_counters
				where lower(object_name) = lower(@sysperfinfoname + ':{0}') and
					lower(instance_name) = lower('{2}')))
			begin
				select N'The instance name does not exist in sys.dm_os_performance_counters for the object.'
			end
		end
	end
end
