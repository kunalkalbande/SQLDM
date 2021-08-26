set nocount on;


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

select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
from master.sys.dm_exec_cached_plans
union
select
 case
   when counter_name = 'Database pages'
     then 'Committed Pages'
   else
     counter_name
 end,
 cntr_value
from
 master..sysperfinfo
where
 counter_name in
   (
     'Procedure cache pages',
     'Total Server Memory (KB)',
     'Connection Memory (KB)',
     'Lock Memory (KB)',
     'Optimizer Memory (KB)',
     'Granted Workspace Memory (KB)'
   )
 or
   (
     counter_name = 'Database pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
 or
   (
     counter_name = 'Free pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
or
   (
     counter_name = 'Total pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
order by 1