---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

--------------------------------------------------------------------------------
--  QUERY: Lock Counter Statistics 2000
--  Tables: sys.dm_os_performance_counters
--  Returns: 
--    Counter name (counter_name)
--    Instance name (instance_name)
--    Value of counter (calculated)
--  Join on: none
--  Criteria: 
--  (1) For counter names sqlserver:locks and sqlserver:latches
--  Calculations:
--  (1) If the counter value is less than 0, attempt to convert to positive int
--------------------------------------------------------------------------------

--RRG START: this TSQL declaration section is a prereq in parent calling script - ServerOverview2012
		-- EngineEditions 5 and greater are reserved for Azure only
		--declare @engineedition sql_variant
		--set @engineedition = SERVERPROPERTY('EngineEdition')

		--declare @sysperfinfoname sysname
--RRG END: this TSQL declaration section is a prereq in parent calling script - ServerOverview2012

Declare @objectnamelocks nchar(128), @objectnamelatches nchar(128)

--START (RRG): Locks and Latches are available in Azure SQL Database Managed Instance

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))

--extract one row that contains the actual instance name and exclude others
--there should be only one instance name
select @sysperfinfoname = (select top 1 object_name from sys.sysperfinfo (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)

Set @objectnamelocks = @sysperfinfoname + ':Locks'
Set @objectnamelatches = @sysperfinfoname + ':Latches'

Select
	counter_name,
	instance_name,
	case
		when cntr_value >= 0 then convert(dec(20,0),cntr_value)
		else convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1
	end
From sys.dm_os_performance_counters with(nolock)
where object_name in (@objectnamelocks, @objectnamelatches);

--END (RRG): Locks and Latches are available in both Azure SQL Database and Managed Instance
