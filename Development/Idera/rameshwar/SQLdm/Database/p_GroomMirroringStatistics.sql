if (object_id('Grooming.p_GroomMirroringStatistics') is not null)
begin
drop procedure Grooming.p_GroomMirroringStatistics
end
go
create procedure [Grooming].[p_GroomMirroringStatistics]
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

Declare @KeysToDelete Table (DatabaseID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(DatabaseID, UTCCollectionDateTime))
select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'MirroringStatistics'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (DatabaseID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(DatabaseID, UTCCollectionDateTime))

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) ms.DatabaseID, ms.UTCCollectionDateTime
				From dbo.MirroringStatistics ms with(nolock)
				Inner Join SQLServerDatabaseNames dn with(nolock) on ms.DatabaseID = dn.DatabaseID
				Where dn.SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And ms.UTCCollectionDateTime <= @CutoffDateTimeIn'
				Else N''
				End + N';
			
				Delete MS
				From dbo.MirroringStatistics MS
				Inner Join @KeysToDelete K
					On K.DatabaseID = MS.DatabaseID
						And K.UTCCollectionDateTime = MS.UTCCollectionDateTime;';
	set @parms = N'@SQLServerIDIn int, @CutoffDateTimeIn datetime, @RecordsToDelete int';
				
					
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
			Select Top(@RecordsToDelete) DatabaseID, UTCCollectionDateTime
			From dbo.MirroringStatistics with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
			
			Delete MS
			From dbo.MirroringStatistics MS
			Inner Join @KeysToDelete K
				On K.DatabaseID = MS.DatabaseID
					And K.UTCCollectionDateTime = MS.UTCCollectionDateTime;
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
