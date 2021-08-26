-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
if (object_id('Grooming.p_GroomDatabaseFileStatisticsAggregation') is not null)
begin
drop procedure Grooming.p_GroomDatabaseFileStatisticsAggregation
end
go
create procedure [Grooming].[p_GroomDatabaseFileStatisticsAggregation]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000,
@CutoffDateTime datetime,
@SQLServerID int = null,
@InstanceName sysname = null,
@Deleted bit = 0
)
as
begin

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)
declare @cmd nvarchar(1000)
declare @parms nvarchar(1000)

Declare @KeysToDelete Table (AggregatedFileStatisticsID bigint not null primary key)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DatabaseFileStatisticsAggregation'
	
if (@SQLServerID is not null)
begin

	Set @cmd = N'Declare @KeysToDelete Table (AggregatedFileStatisticsID bigint not null primary key)
			
			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) dfa.AggregatedFileStatisticsID
			From dbo.DatabaseFileStatisticsAggregation dfa with(nolock)
			Inner Join DatabaseFiles df with(nolock) on dfa.FileID = df.FileID
			Inner Join SQLServerDatabaseNames dn with(nolock) on df.DatabaseID = dn.DatabaseID
			Where dn.SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And dfa.MaxUTCCollectionDateTime <= @CutoffDateTimeIn'
				Else N''
				End + N';
		
			Delete DFA
			From dbo.DatabaseFileStatisticsAggregation DFA
			Inner Join @KeysToDelete K
				On K.AggregatedFileStatisticsID = DFA.AggregatedFileStatisticsID;';
	set @parms = N'@SQLServerIDIn int, @CutoffDateTimeIn datetime, @RecordsToDelete int'	
				
					
	if @Deleted = 1
	begin
		set @BlockName = @BlockName + ' (deleting)'
	end
end	
			
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	           
		if (@SQLServerID is null)
		begin
			Delete From @KeysToDelete;

			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) AggregatedFileStatisticsID
			From dbo.DatabaseFileStatisticsAggregation with(nolock)
			Where MaxUTCCollectionDateTime <= @CutoffDateTime;
		
			Delete DFA
			From dbo.DatabaseFileStatisticsAggregation DFA
			Inner Join @KeysToDelete K
				On K.AggregatedFileStatisticsID = DFA.AggregatedFileStatisticsID;
		end
		else
		begin
			exec sp_executesql @cmd, @parms, @SQLServerIDIn = @SQLServerID, @CutoffDateTimeIn = @CutoffDateTime, @RecordsToDelete = @RecordsToDelete;
		end
		
		
		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete)
			break;
		
	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, @InstanceName
		break;
	
	end catch
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, @InstanceName

return @RC

end
