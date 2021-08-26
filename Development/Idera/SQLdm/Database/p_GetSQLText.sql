if (object_id('p_GetSQLText') is not null)
begin
drop procedure p_GetSQLText 
end
go
create procedure p_GetSQLText
	@SQLStatementHash nvarchar(30) = null,
	@SQLStatementID bigint = null,
	@SQLSignatureHash nvarchar(30) = null,
	@SQLSignatureID bigint = null
as
begin

	declare @RC int
	set @RC = 0
	
	if (@SQLStatementHash is not null)
	begin
		select 
			ID = SQLStatementID,
			SQLText = SQLStatement 
		from 
			AllSQLStatements 
		where
			SQLStatementHash = @SQLStatementHash

		set @RC = @@rowcount
	end

	if (@RC = 0 and @SQLStatementID is not null)
	begin
		select 
			ID = SQLStatementID,
			SQLText = SQLStatement 
		from 
			AllSQLStatements 
		where
			SQLStatementID = @SQLStatementID

		set @RC = @@rowcount
	end

	if (@RC = 0 and @SQLSignatureHash is not null)
	begin
		select 
			ID = SQLSignatureID,
			SQLText = SQLSignature
		from 
			AllSQLSignatures 
		where
			SQLSignatureHash = @SQLSignatureHash

		set @RC = @@rowcount
	end

	if (@RC = 0 and @SQLSignatureID is not null)
	begin
		select 
			ID = SQLSignatureID,
			SQLText = SQLSignature
		from 
			AllSQLSignatures 
		where
			SQLSignatureID = @SQLSignatureID

		set @RC = @@rowcount
	end

	if (@RC = 0)
		select 'Not found'
end