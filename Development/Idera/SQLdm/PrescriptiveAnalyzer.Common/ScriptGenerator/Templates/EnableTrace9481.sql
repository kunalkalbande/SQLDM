--If the trace flag needs to be re-enabled as a startup option, it should be set manually and then the SQL Server service restarted

DBCC TRACEON(9481, -1);