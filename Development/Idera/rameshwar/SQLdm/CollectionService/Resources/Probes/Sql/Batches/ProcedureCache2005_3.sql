
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
from
 master..syscacheobjects
group by
 case
   when (status & 1) = 1 or cacheobjtype = 'Cursor Parse Tree'
     then 'Cursors'
   when cacheobjtype = 'Extended Proc'
     then 'Extended Procedure'
   else
     objtype
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

insert into #perfmon_counters
 select
   LOWER(counter_name),
   LOWER(instance_name),
   cntr_value
 from
   master..sysperfinfo
 where
   Lower(object_name) = lower(@sysperfinfoname + ':plan cache')

declare
 @hitratio bigint,
 @hitratiobase bigint,
 @percentbias dec(15,2)

select
 @hitratio = cntr_value
from
 #perfmon_counters
where
 counter_name = 'cache hit ratio'
 and instance_name = 'execution contexts'

select
 @hitratiobase = cntr_value
from
 #perfmon_counters
where
 counter_name = 'cache hit ratio base'
 and instance_name = 'execution contexts'

select
 @hitratio = ISNULL(@hitratio, -1)

select
 @hitratiobase = ISNULL(@hitratiobase, -1)

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
       select
         ISNULL(s2.cntr_value, s1.cntr_value)
       from
         #perfmon_counters s2
       where
         s2.counter_name = 'cache hit ratio base'
         and s2.instance_name = s1.instance_name
     ),0)
 from
   #perfmon_counters s1
 where
   s1.counter_name = 'cache hit ratio'
   and s1.cntr_value > 0
else
   select
     top({0})
     'hitrat',
     s1.instance_name,
     'Hit Ratio %' =
       (
         @percentbias + (CONVERT(dec(15,2),s1.cntr_value) * 100) /
         nullif((
           select
             ISNULL(s2.cntr_value, s1.cntr_value)
           from
             #perfmon_counters s2
           where
             s2.counter_name = 'cache hit ratio base'
             and s2.instance_name = s1.instance_name
         ),0)
       ) / 2
   from
     #perfmon_counters s1
   where
     s1.counter_name = 'cache hit ratio'
     and s1.cntr_value > 0

select
top({0})
'heading',
 counter_name,
 isnull(sum(convert(dec(15,0),cntr_value)),0)
from
 #perfmon_counters
where
 counter_name like 'cache hit ratio%'
 and instance_name <> '_total'
group by
 counter_name
