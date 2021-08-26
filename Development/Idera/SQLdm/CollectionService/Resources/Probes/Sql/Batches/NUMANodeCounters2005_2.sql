--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--use tempdb; [USE NOT PERMITTED IN AZURE]

--START (RRG): Get internal for Instance Name when in Azure Platform
Declare @sysperfinfoname sysname

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))

--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
	select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
--END (RRG): Get internal for Instance Name when in Azure Platform

declare @outputTable table (NodeName varchar(128), PLE bigint, TargetPages bigint);

if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1
BEGIN
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
END
select * from @outputTable