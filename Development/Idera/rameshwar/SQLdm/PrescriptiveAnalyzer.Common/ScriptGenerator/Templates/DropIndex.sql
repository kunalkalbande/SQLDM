USE $(BracketDatabase);
GO
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N$(SafeSchemaTable)) AND name = N$(SafeIndexName))
	DROP INDEX $(BracketIndexName) 
	ON $(BracketSchemaTable)