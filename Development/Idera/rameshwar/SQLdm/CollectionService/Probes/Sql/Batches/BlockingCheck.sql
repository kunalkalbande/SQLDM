select 'start blocking check' 

declare extract_too_much_block 
insensitive cursor 
for 
select 
	p.spid, 
	p.program_name, 
	p.hostname, 
	p.loginame, 
	replace(db_name(ISNULL(convert(int,l.rsc_dbid),-1)),char(39),char(39)+char(39)), 
	ISNULL(l.rsc_objid,-1), 
	ISNULL(convert(int,l.req_mode),-1)
from 
	master..sysprocesses p (nolock) 
	FULL JOIN master..sysprocesses b (nolock) 
		on p.spid = b.blocked 
	LEFT OUTER JOIN master..syslockinfo l (nolock) 
		on b.spid = l.req_spid 
		and l.req_status in (2,3)
where 
	p.blocked = 0 
	and p.program_name not like 'SQLAgent%' 
	and p.program_name not in ('DiagnosticMan', 'SQL PerfMon') 
	and b.blocked <> 0 
	and b.waittime > 15000 
	
for read only 

Open extract_too_much_block 

fetch next from extract_too_much_block 
	into @spid, @program_name, @hostname, @username, @dbname, @tabid, @lock_type 
	
while @@fetch_status = 0
begin 
	select @command = 
		'select ''blocking info'', ''' 
		+ convert(varchar(5), @spid) 
		+ ''',''' 
		+ rtrim(@program_name) 
		+ ''',''' 
		+ rtrim(@hostname) 
		+ ''',''' 
		+ rtrim(@username) 
		+ ''',''' 
		+ rtrim(@dbname) 
		+ ''',''' 
		+ convert(varchar(20), @tabid) 
		+ ''',''' 
		+ convert(varchar(5), @lock_type) 
		+ ''' dbcc inputbuffer(' 
		+ convert(varchar(5),@spid) 
		+ ')'  
		
exec (@command) 

fetch next from extract_too_much_block 
	into @spid, @program_name, @hostname, @username, @dbname, @tabid, @lock_type
end 
Close extract_too_much_block 
deallocate extract_too_much_block