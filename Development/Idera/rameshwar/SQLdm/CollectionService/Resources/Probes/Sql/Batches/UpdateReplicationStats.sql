--------------------------------------------------------------------------------
--  Batch: Replication Check
--  Tables: sysdatabases, sysobjects, sysprocesses, MSrepl_commands, 
--	MSrepl_transactions, MSsubscriptions, MSdistribution_history,
--	MSdistribution_status
--  SP: sp_replcounters, sp_helpdistributor
--------------------------------------------------------------------------------
	
	USE master
	
	UPDATE STATISTICS dbo.MSrepl_commands
		
	UPDATE STATISTICS dbo.MSrepl_transactions

		begin

		  IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_MSReplCommands_DatabaseId' 
			AND object_id = OBJECT_ID('dbo.MSrepl_commands'))
		  begin
			DROP INDEX IX_MSReplCommands_DatabaseId  ON [SchmaName].[TableName];
		  end

		end
		
		CREATE NONCLUSTERED INDEX IX_MSReplCommands_DatabaseId 
		ON @quotedDBName + '..MSrepl_commands' (publisher_database_id, article_id)  
		INCLUDE (xact_seqno);  
		GO  