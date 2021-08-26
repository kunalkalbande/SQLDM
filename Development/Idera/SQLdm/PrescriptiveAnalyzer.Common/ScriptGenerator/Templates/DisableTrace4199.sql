--If trace flag is enabled as a startup option, it should be removed manually and then the SQL Server service restarted. 

DBCC TRACEOFF(4199, -1);