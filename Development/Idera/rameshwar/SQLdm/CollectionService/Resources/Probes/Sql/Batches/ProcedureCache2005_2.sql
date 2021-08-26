--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  Batch: Procedure Cache 2005
--  Tables: sys.dm_os_performance_counters, syscacheobjects
--  XSP:
--	Variables:
--  [0] - select top value
--
--  RRG: This script has been modified to work only on Azure SQL DB User Databases
--     retricted permissions prevent it from running on master database
--------------------------------------------------------------------------------
		
--START (RRG): Get internal for Instance Name when in Azure Platform
declare @sysperfinfoname sysname
		
set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))

--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
	select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)

--END (RRG): Get internal for Instance Name when in Azure Platform

if (select database_id from sys.databases where name = DB_NAME()) > 4 
if HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE') = 1 
BEGIN
select
		top({0})
		'summary',
		case
			when (status & 1) = 1 or cacheobjtype = 'Cursor Parse Tree'
				then 'Cursors'
			when cacheobjtype = 'Extended Proc'
				then 'Extended Procedure'
		else
			objtype
		end,
		sum(pagesused)
	from sys.syscacheobjects --remove master.. reference so it will work on User Databases
	group by
		case
			when (status & 1) = 1 or cacheobjtype = 'Cursor Parse Tree'
				then 'Cursors'
			when cacheobjtype = 'Extended Proc'
				then 'Extended Procedure'
		else
			objtype
		end
END
ELSE
BEGIN
	SELECT NULL, NULL, NULL
END
else
begin
	DECLARE @tempVar_ProcedureCache2005_2 TABLE
	(
		DummyCol1 nvarchar(20),
		DummyCol2 nvarchar(20),
		DummyCol3 int
	)
	SELECT * FROM @tempVar_ProcedureCache2005_2
end

if isnull(object_id('tempdb..#perfmon_counters'), 0) <> 0
	truncate table #perfmon_counters
else
	CREATE TABLE #perfmon_counters
	(
		counter_name nchar(128) NOT NULL,
		instance_name nchar(128),
		cntr_value bigint NOT NULL
	)

if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
insert into #perfmon_counters
	select
		lower(counter_name),
		lower(instance_name),
		cntr_value
	from sys.dm_os_performance_counters
	where
		lower(object_name) = lower(@sysperfinfoname + ':plan cache')

declare
	@hitratio bigint,
	@hitratiobase bigint,
	@percentbias dec(15,2)

select
	@hitratio = cntr_value
from #perfmon_counters
where
	counter_name = 'cache hit ratio' and 
	instance_name = 'execution contexts'

select
	@hitratiobase = cntr_value
from #perfmon_counters
where
	counter_name = 'cache hit ratio base' and 
	instance_name = 'execution contexts'

select @hitratio = ISNULL(@hitratio, -1)
select @hitratiobase = ISNULL(@hitratiobase, -1)

if (@hitratio = -1) or (@hitratiobase = -1)
	select @percentbias = -1
else
	select @percentbias = ((convert(dec(15,2),@hitratio) / nullif(convert(dec(15,2),@hitratiobase),0))*100)

if @percentbias = -1
	select
		top({0})
		'hitrat',
		s1.instance_name,
		'Hit Ratio %' = (CONVERT(dec(15,2),s1.cntr_value) * 100) /
			nullif((
				select ISNULL(s2.cntr_value, s1.cntr_value)
				from #perfmon_counters s2
				where
					s2.counter_name = 'cache hit ratio base'
					and s2.instance_name = s1.instance_name
			),0)
	from #perfmon_counters s1
	where
		s1.counter_name = 'cache hit ratio'	and 
		s1.cntr_value > 0
else
	select
		top({0})
		'hitrat',
		s1.instance_name,
		'Hit Ratio %' =
		(
			@percentbias + (CONVERT(dec(15,2),s1.cntr_value) * 100) /
			nullif((
				select ISNULL(s2.cntr_value, s1.cntr_value)
				from #perfmon_counters s2
				where
					s2.counter_name = 'cache hit ratio base' and 
					s2.instance_name = s1.instance_name
			),0)
		) / 2
   from #perfmon_counters s1
   where
		s1.counter_name = 'cache hit ratio' and s1.cntr_value > 0

select
	top({0})
	'heading',
	counter_name,
	isnull(sum(convert(dec(15,0),cntr_value)),0)
from #perfmon_counters
where
	counter_name like 'cache hit ratio%' and 
	instance_name <> '_total'
group by 
	counter_name