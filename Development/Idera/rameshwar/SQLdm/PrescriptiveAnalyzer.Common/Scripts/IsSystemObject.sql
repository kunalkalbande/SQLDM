use {0};
declare @objid int;
set @objid = {1};

if (objectproperty(@objid, 'ismsshipped') = 1) 
begin 
	select 1; 
	return; 
end;

if (objectproperty(@objid, 'isreplproc') = 1) 
begin 
	select 1; 
	return; 
end;

select 0


