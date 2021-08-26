USE $(BracketDatabase);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N$(SafeSchemaTable)) AND name = N$(SafeIndexName))
	CREATE INDEX $(BracketIndexName) 
	ON $(BracketSchemaTable) 
	( $(KeyColumns) )
	$(IncludeColumns?INCLUDE( {0} ):)