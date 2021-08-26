if (object_id('p_InsertSQLStatement') is not null)
begin
drop procedure p_InsertSQLStatement
end
go
create procedure p_InsertSQLStatement
	@SQLStatementHash varchar(30),
	@SQLStatement nvarchar(max),
	@SQLStatementID int output
as
begin

	if @SQLStatement is null or len(@SQLStatement) = 0 
		return

	select @SQLStatementID = SQLStatementID
	from SQLStatements
	where 
		SQLStatementHash = @SQLStatementHash

	if (@@rowcount > 1)
	begin
		select @SQLStatementID = SQLStatementID
		from AllSQLStatements
		where SQLStatementHash = @SQLStatementHash
	end

	if (nullif(@SQLStatementID,0) is null)
	begin
		if (select case when ( len(@SQLStatement) < 4000 and @SQLStatement = cast(@SQLStatement as varchar(4000)) ) then 1 else 0 end) = 1
		begin
			insert into SQLStatements([SQLStatementHash],[SQLStatement],[Overflow]) values (@SQLStatementHash,@SQLStatement,0)
			select @SQLStatementID = scope_identity()
		end
		else
		begin
			insert into SQLStatements([SQLStatementHash],[Overflow]) values (@SQLStatementHash,1)
			select @SQLStatementID = scope_identity()
			insert into SQLStatementsOverflow([SQLStatementID],[SQLStatementOverflow]) values (@SQLStatementID,@SQLStatement)
		end
	end

	
end
go
