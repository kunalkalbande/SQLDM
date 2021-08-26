---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

-- [0] - Object Name
-- [1] - Counter Name
-- [2] - Instance Name


--START (RRG): Get internal for Instance Name when in Azure Platform
declare @sysperfinfoname sysname
Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
--END (RRG): Get internal for Instance Name when in Azure Platform

select top 1
	isnull(convert(dec(38,0),cntr_value),0)
from 
	sys.dm_os_performance_counters
where 
	(lower(object_name) = lower(@sysperfinfoname + ':{0}')
	and lower(counter_name) = lower('{1}')
	and lower(instance_name) = lower('{2}'))
	
if (@@ROWCOUNT = 0)
begin
	if (not exists(select object_name from sys.dm_os_performance_counters 
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