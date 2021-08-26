select 
	name, 
	description,
	minimum, 
	maximum,  
	config_value = value,
	run_value = value_in_use,
	restart_required = case when is_dynamic = 1 then 0 else 1 end,
	configuration_id
from 
	sys.configurations

declare @outputTable table (Name varchar(64), Value varchar(32));

declare @tempversion table (i int, Name nvarchar(100), Internal_Value bigint, Character_Value nvarchar(100));

-- Minimum support for Azure
begin try
insert into @tempversion execute xp_msver 'WindowsVersion';
end try
begin catch
    print ERROR_MESSAGE()
end catch

insert into @outputTable(Name, Value) select 'WindowsVersion', Character_Value from @tempversion;

	
insert into @outputTable(Name, Value) select 'MinutesRunning', datediff(mi,create_date,getdate()) from sys.databases where name = 'tempdb';
insert into @outputTable(Name, Value) select 'EncryptedConnections', count(*) from sys.dm_exec_connections where encrypt_option = 'TRUE';
insert into @outputTable(Name, Value) select 'ServerProcessID', convert(varchar, serverproperty('ProcessID'));
insert into @outputTable(Name, Value) select 'TempDbRecoveryModel', convert(varchar, DATABASEPROPERTYEX('tempdb','RECOVERY'));
insert into @outputTable(Name, Value) select 'Edition', convert(varchar, SERVERPROPERTY('Edition'));

-- Minimum support for Azure
begin try
	insert into @outputTable(Name, Value) select 'PhysicalMemory', (physical_memory_kb * 1024) from sys.dm_os_sys_info;
end try
begin catch
    print ERROR_MESSAGE()
end catch
insert into @outputTable(Name, Value) SELECT 'ProductVersion', CAST(SERVERPROPERTY('productversion') AS varchar(32));

-- Check SeManageVolumePrivilege
if 1 = (select cast(value_in_use as int) from sys.configurations where configuration_id = 16390) -- xp_cmdshell must be enabled
	and 1 = IS_SRVROLEMEMBER('sysadmin') -- must be in sysadmin role or a proxy account is used to run commands
begin
	declare @xp__cmdshell_output table(Output VARCHAR(8000))
	BEGIN TRY
		--insert into @xp__cmdshell_output exec ('xp__cmdshell ''whoami'''); 
		insert into @outputTable(Name, Value) 
		select top 1 'SQLServerServiceAccount', rtrim(left([Output],40)) from @xp__cmdshell_output where [Output] is not null
	END TRY
	BEGIN CATCH
	END CATCH	

	BEGIN TRY
		delete from @xp__cmdshell_output
		--insert into @xp__cmdshell_output exec ('xp__cmdshell ''whoami /priv'''); 
		insert into @outputTable(Name, Value) 
		select 'SeManageVolumePrivilege', ltrim(right([Output],8)) from @xp__cmdshell_output where [Output] like 'SeManageVolumePrivilege %'
		if @@rowcount = 0
		begin
			if (not exists (select * from @xp__cmdshell_output where [Output] like ('% is not recognized as %')))
				insert into @outputTable(Name, Value) values ('SeManageVolumePrivilege', 'Disabled') 
		end
	END TRY
	BEGIN CATCH
	END CATCH	
end



select Name, Value from @outputTable; 