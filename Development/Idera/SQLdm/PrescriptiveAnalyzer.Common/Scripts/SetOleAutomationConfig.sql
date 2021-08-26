if (select count(*) from sys.configurations where cast(value_in_use as int) <> 1 and configuration_id = 518) > 0 
	begin 
			execute master..sp_configure 'show advanced options', 1
			reconfigure with override
	end 

execute master..sp_configure 'ole automation procedures', {0}
reconfigure with override
