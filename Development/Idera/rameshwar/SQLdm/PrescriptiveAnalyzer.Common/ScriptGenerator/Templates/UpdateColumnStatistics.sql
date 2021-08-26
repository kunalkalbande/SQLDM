$(Database?USE {0}
GO
:)
declare @tbl int;
declare @statsName sysname;
declare @colName sysname;
declare @schema_table_name nvarchar(640);
declare @exec_stmt nvarchar(4000);

set @schema_table_name = $(SafeSchemaTable);
set @colName = $(SafeColumn);
set @tbl = object_id(@schema_table_name);

select @statsName = s.name from sys.stats s
    join sys.stats_columns sc on  (s.object_id = sc.object_id and s.stats_id = sc.stats_id)
    join sys.columns c on  (sc.object_id = c.object_id and sc.column_id = c.column_id)
	where s.object_id = @tbl 
	and sc.stats_column_id = 1
	and c.name = @colName;
if @statsName is null
begin
	set @statsName = '_stats_' + @colName; 
	set @exec_stmt = 'create statistics ' + quotename(@statsName) + ' on ' + @schema_table_name + ' (' + quotename(@colName) + ')';
end
else
begin
	set @exec_stmt = 'update statistics ' + @schema_table_name + ' ' + quotename(@statsName);
end
exec (@exec_stmt)
