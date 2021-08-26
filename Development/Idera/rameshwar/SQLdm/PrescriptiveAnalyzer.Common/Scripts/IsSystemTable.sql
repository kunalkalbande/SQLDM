use {0};
declare @objid int;
set @objid = object_id({1});

if (objectproperty(@objid, 'IsMSShipped') = 1) 
begin 
	select 1; 
	return; 
end;

if (objectproperty(@objid, 'IsSystemTable') = 1) 
begin 
	select 1; 
	return; 
end;

select 0


