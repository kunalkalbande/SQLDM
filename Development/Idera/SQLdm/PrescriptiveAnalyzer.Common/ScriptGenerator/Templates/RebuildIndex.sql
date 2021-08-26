USE $(BracketDatabase);
GO
ALTER INDEX $(BracketIndexName)
	ON $(BracketSchemaTable)
	REBUILD $(Online=ON?WITH (ONLINE=ON):)