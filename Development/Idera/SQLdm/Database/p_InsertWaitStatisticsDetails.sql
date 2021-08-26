if (object_id('p_InsertWaitStatisticsDetails') is not null)
begin
drop procedure p_InsertWaitStatisticsDetails
end
go
create procedure p_InsertWaitStatisticsDetails
	@WaitStatisticsId bigint,
	@WaitTypeId int,
	@WaitingTasks bigint,	
	@WaitTimeInMilliseconds bigint,
	@MaxWaitTimeInMilliseconds bigint,
	@ResourceWaitTimeInMilliseconds bigint,
	@ReturnMessage nvarchar(128) output
as
begin
	insert into [WaitStatisticsDetails]
			([WaitStatisticsID]
			,[WaitTypeID]
			,[WaitingTasks]
			,[WaitTimeInMilliseconds]
			,[MaxWaitTimeInMilliseconds]
			,[ResourceWaitTimeInMilliseconds])
	values
			(
			@WaitStatisticsId
			,@WaitTypeId
			,@WaitingTasks	
			,@WaitTimeInMilliseconds 
			,@MaxWaitTimeInMilliseconds
			,@ResourceWaitTimeInMilliseconds 
			)

select @ReturnMessage = @@error

end

	