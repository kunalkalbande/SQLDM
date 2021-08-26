use $(BracketedDatabase);
GO
if not exists (select * from sys.stats where object_id = object_id(N$(SafeSchemaTable)) and name = N$(SafeName))
begin
	print 'Statistics object ' + $(SafeName) + ' no longer exists on table ' + $(SafeSchemaTable);
end
else
begin
	update statistics $(BracketedTableName)($(BracketedName));
end