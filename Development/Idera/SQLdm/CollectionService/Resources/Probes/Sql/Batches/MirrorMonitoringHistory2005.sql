--------------------------------------------------------------------------------
--  Variables: 
--		[0] - Selected database
--		[1] - mode
--			0 = Last row
--			1 = Rows last two hours
--			2 = Rows last four hours
--			3 = Rows last eight hours
--			4 = Rows last day
--			5 = Rows last two days
--			6 = Last 100 rows
--			7 = Last 500 rows
--			8 = Last 1,000 rows
--			9 = Last 1,000,000 rows
--------------------------------------------------------------------------------
use msdb
IF IS_SRVROLEMEMBER('sysadmin') = 1
begin
	exec sys.sp_dbmmonitorresults @database_name=[{0}], @mode = {1}, @update_table = 0
end
else
begin
	IF DATABASE_PRINCIPAL_ID('dbm_monitor') IS NOT NULL
	begin
		exec sys.sp_dbmmonitorresults @database_name=[{0}], @mode = {1}, @update_table = 0
	end	
end