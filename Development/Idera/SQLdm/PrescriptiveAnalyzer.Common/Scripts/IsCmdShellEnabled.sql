----------------------------------------------------------------------------------------------
--
--	exec sp_configure 'xp_cmdshell';
--	^^^^^Can be used to determine if cmd shell is enabled
--
--
--  exec sp_configure 'show advanced options', 1;
--  GO
--  RECONFIGURE;
--  GO
--	^^^^^Can be used show advanced options from sp_configure
--
--
--  exec sp_configure 'xp_cmdshell', 1;
--  GO
--  RECONFIGURE;
--  GO
--	^^^^^Can be used to turn on cmd shell
--
----------------------------------------------------------------------------------------------

select cast(value_in_use as int) from  
	sys.configurations 
	where configuration_id = 16390 -- Enable or disable command shell