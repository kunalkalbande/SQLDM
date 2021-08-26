----------------------------------------------------------------------------------------------
--
--	exec sp_configure 'Ole Automation Procedures';
--	^^^^^Can be used to determine if ole automation is enabled
--
--
--  exec sp_configure 'show advanced options', 1;
--  GO
--  RECONFIGURE;
--  GO
--	^^^^^Can be used show advanced options from sp_configure
--
--
--  exec sp_configure 'Ole Automation Procedures', 1;
--  GO
--  RECONFIGURE;
--  GO
--	^^^^^Can be used to turn on ole automation
--
----------------------------------------------------------------------------------------------
declare @curconfig int 

select @curconfig = cast(value_in_use as int) from  
	sys.configurations 
	where configuration_id = 16388; -- Enable or disable Ole Automation Procedures

if (isnull(@curconfig,1) > 0)
	select @curconfig = case when object_id('master..sp_OACreate') is not null then 1 else 0 end;
	
----------------------------------------------------------------------------------------------
--  If lightweight pooling is enabled, ole cannot be used.
--
if 0 <> (select cast(value_in_use as int) from sys.configurations where configuration_id = 1546) -- ligthtweight pooling	
begin
	set @curconfig = 0;
end;

select @curconfig
