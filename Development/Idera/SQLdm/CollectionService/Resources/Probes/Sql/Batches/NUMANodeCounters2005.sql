----------------------------------------------------------------------------------------------
--
--
--
----------------------------------------------------------------------------------------------

use tempdb;

declare 
	@servername varchar(255),
	@sysperfinfoname varchar(255),
	@slashpos int;

declare @outputTable table (NodeName varchar(128), PLE bigint, TargetPages bigint);

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

-- Join the table with itself so that we can transform name, value pairs found accross rows
-- to different columns in the same row
insert into @outputTable(NodeName, PLE, TargetPages) 
	select 
		[NodeName] = rtrim(pc1.instance_name), 
		[PLE] = isnull(convert(bigint,pc1.cntr_value), 0),
		[TargetPages] = isnull(convert(bigint,pc2.cntr_value), 0)
	from sys.dm_os_performance_counters as pc1
	join sys.dm_os_performance_counters as pc2
		on pc1.object_name = pc2.object_name 
		and pc1.instance_name = pc2.instance_name 
		and lower(pc1.object_name) = lower(@sysperfinfoname + ':buffer node')
		and lower(pc1.counter_name) in ('page life expectancy')
		and lower(pc2.counter_name) in ('target pages')

select * from @outputTable