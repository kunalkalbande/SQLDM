if (object_id('p_InsertSQLSignature') is not null)
begin
drop procedure p_InsertSQLSignature
end
go
create procedure p_InsertSQLSignature
	@SQLSignatureHash varchar(30),
	@SQLSignature nvarchar(max),
	@SQLStatementExampleID int,
	@SQLSignatureID int output
as
begin

	if len(@SQLSignature) = 0
		return

	select @SQLSignatureID = SQLSignatureID
	from SQLSignatures
	where 
		SQLSignatureHash = @SQLSignatureHash
	
	if (@@rowcount > 1)
	begin
		select @SQLSignatureID = SQLSignatureID
		from AllSQLSignatures
		where SQLSignatureHash = @SQLSignatureHash
	end

	if (nullif(@SQLSignatureID,0) is null)
	begin
		if (select case when ( len(@SQLSignature) < 4000 and @SQLSignature = cast(@SQLSignature as varchar(4000)) ) then 1 else 0 end) = 1
		begin
			insert into SQLSignatures([SQLSignatureHash],[SQLSignature],[Overflow],[SQLStatementExampleID]) values (@SQLSignatureHash,@SQLSignature,0,@SQLStatementExampleID)
			select @SQLSignatureID = scope_identity()
		end
		else
		begin
			insert into SQLSignatures([SQLSignatureHash],[Overflow],[SQLStatementExampleID]) values (@SQLSignatureHash,1,@SQLStatementExampleID)
			select @SQLSignatureID = scope_identity()
			insert into SQLSignaturesOverflow([SQLSignatureID],[SQLSignatureOverflow]) values (@SQLSignatureID,@SQLSignature)
			
		end
	end

	
end
go

