if (object_id('p_InsertWaitTypes') is not null)
begin
drop procedure p_InsertWaitTypes
end
go
create procedure p_InsertWaitTypes
	@WaitType varchar(120),
	@ReturnID int output,
	@ReturnMessage nvarchar(128) output
as
begin

	declare @WaitTypeReturn int

	select @WaitTypeReturn = [WaitTypeID] 
	from [WaitTypes]
	where [WaitType] = @WaitType
	
	if (@WaitTypeReturn is null)
	begin

		declare @WaitCategoryID int

		exec p_GetWaitCategory @WaitType, @WaitCategoryID output
		
		insert into [WaitTypes]
			(
			[WaitType]
			,[CategoryID]
			)
		values
			(
			@WaitType
			,@WaitCategoryID
			)

		select @WaitTypeReturn = [WaitTypeID] 
		from [WaitTypes]
		where [WaitType] = @WaitType
	end


	select @ReturnID = @WaitTypeReturn
	select @ReturnMessage = @@error

end

	

