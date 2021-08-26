if (object_id('Grooming.p_GroomMetricMetaData') is not null)
begin
drop procedure Grooming.p_GroomMetricMetaData
end
go
create procedure [Grooming].[p_GroomMetricMetaData]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000
)
as
begin

if (not exists(select Metric from MetricMetaData where [Deleted] = 1))
	return;

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)

Declare @KeysToDelete Table (SQLServerID int not null,
							UTCCollectionDateTime datetime not null,
							MetricID int not null,
							primary key(SQLServerID, UTCCollectionDateTime, MetricID))

select @RowsAffected = 0, 
	@RC = 0, 
	@BlockName = N'MetricMetaData';
	
Delete MI From MetricInfo MI
Where Exists (Select 1 from MetricMetaData
			Where [Deleted] = 1
			And Metric = MI.Metric);

set @RowsAffected = @@ROWCOUNT;
set @RC = @RC + @RowsAffected;
		
Delete CCD From CustomCounterDefinition CCD
Where Exists (Select 1 from MetricMetaData
			Where [Deleted] = 1
			And Metric = CCD.Metric);

set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
		Delete From @KeysToDelete;

		Insert Into @KeysToDelete
		Select Top(@RecordsToDelete) SQLServerID, UTCCollectionDateTime, MetricID
		From dbo.CustomCounterStatistics CCS with(nolock)
		Where Exists (Select 1 from MetricMetaData
					Where [Deleted] = 1
					And Metric = CCS.MetricID);
			
		Delete CCS
		From dbo.CustomCounterStatistics CCS
		Inner Join @KeysToDelete K
			On K.SQLServerID = CCS.SQLServerID
				And K.UTCCollectionDateTime = CCS.UTCCollectionDateTime
				And K.MetricID = CCS.MetricID;

		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete)
			break;
		
	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, null
		break;
	
	end catch
end	

delete from MetricMetaData
	where [Deleted] = 1
	
set @RowsAffected = @@ROWCOUNT
set @RC = @RC + @RowsAffected	
				
exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, null

return @RC

end

