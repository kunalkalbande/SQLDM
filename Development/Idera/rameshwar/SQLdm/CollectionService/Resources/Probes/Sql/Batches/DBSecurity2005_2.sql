
if (db_name() not in ('master','msdb','model','tempdb'))
begin
	select
		'GuestHasDatabaseAccess', value = count(1)
	from
		sysusers
	where
		name = 'guest'
		and hasdbaccess = 1

	declare @dbowner sysname,@isTrustworthy bit,@isadmin bit
	select  @dbowner = suser_sname(), @isTrustworthy = is_trustworthy_on,@isadmin = 0
	from sys.databases with (readcommitted)
	where name = db_name()
	if (isnull(@isTrustworthy,0) > 0)
	begin
		select @isadmin = is_srvrolemember('sysadmin',@dbowner)
		if (isnull(@isadmin,-1) = -1)
		begin
			declare @privilege sysname
			exec xp_logininfo @dbowner,'all', @privilege output
			select @isadmin = case when @privilege = 'admin' then 1 else 0 end
		end
	end
	select 'IsTrustworthyVulnerable', value = @isTrustworthy & @isadmin

	select 
		'NonSystemDatabaseWeakKey', value = count(1) 
	from 
		sys.symmetric_keys 
	where 
		key_length < 128 
		or algorithm_desc in ('RC2','RC4','DESX')
end
else
begin
	select 
		'SystemDatabaseSymmetricKey', value = count(1) 
	from 
		sys.symmetric_keys 
	where name <> '##MS_ServiceMasterKey##'
end

