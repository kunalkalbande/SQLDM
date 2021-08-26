if (object_id('p_InsertWaitStatistics') is not null)
begin
drop procedure p_InsertWaitStatistics
end
go
create procedure p_InsertWaitStatistics
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@TimeDeltaInSeconds float,
	@ReturnID bigint output,
	@ReturnMessage nvarchar(128) output
as
begin

	insert into WaitStatistics
			([SQLServerID]
			,[UTCCollectionDateTime]
			,[TimeDeltaInSeconds]
			)
	values
			(
			@SQLServerID
			,@UTCCollectionDateTime
			,@TimeDeltaInSeconds 
			)

	select @ReturnID = scope_identity()

select @ReturnMessage = @@error

end

