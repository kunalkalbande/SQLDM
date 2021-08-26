if (object_id('p_InsertCustomCounterStatistics') is not null)
begin
drop procedure p_InsertCustomCounterStatistics
end
go
create procedure p_InsertCustomCounterStatistics
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@MetricID int,
	@TimeDeltaInSeconds float,
	@RawValue decimal(38,9),
	@DeltaValue decimal(38,9),
	@ErrorMessage nvarchar(255),
	@RunTimeInMilliseconds float,
	@ReturnMessage nvarchar(128) output
as
begin

INSERT INTO [CustomCounterStatistics]
			([SQLServerID]
			,[UTCCollectionDateTime]
			,[MetricID]
			,[TimeDeltaInSeconds]
			,[RawValue]
			,[DeltaValue]
			,[ErrorMessage]
			,[RunTimeInMilliseconds])
     VALUES
			(@SQLServerID
			,@UTCCollectionDateTime
			,@MetricID
			,@TimeDeltaInSeconds
			,@RawValue
			,@DeltaValue
			,@ErrorMessage
			,@RunTimeInMilliseconds)
end