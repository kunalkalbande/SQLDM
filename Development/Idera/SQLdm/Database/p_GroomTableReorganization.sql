if (object_id('Grooming.p_GroomTableReorganization') is not null)
begin
drop procedure Grooming.p_GroomTableReorganization
end
go
create procedure [Grooming].[p_GroomTableReorganization]
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

Declare @KeysToDelete Table (TableID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(TableID, UTCCollectionDateTime))

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'TableReorganization'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (TableID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(TableID, UTCCollectionDateTime))
				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) tr.TableID, tr.UTCCollectionDateTime
				From dbo.TableReorganization tr with(nolock)
				Inner Join SQLServerTableNames tn (nolock) on tr.TableID = tn.TableID
				Inner Join SQLServerDatabaseNames dn (nolock) on tn.DatabaseID = dn.DatabaseID
				Where dn.SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And tr.UTCCollectionDateTime <= @CutoffDateTimeIn'
				Else N''
				End + N';
			
				Delete TR
				From dbo.TableReorganization TR
				Inner Join @KeysToDelete K
					On K.TableID = TR.TableID
						And K.UTCCollectionDateTime = TR.UTCCollectionDateTime;';
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
			Select Top(@RecordsToDelete) TableID, UTCCollectionDateTime
			From dbo.TableReorganization with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
			
			Delete TR
			From dbo.TableReorganization TR
			Inner Join @KeysToDelete K
				On K.TableID = TR.TableID
					And K.UTCCollectionDateTime = TR.UTCCollectionDateTime;
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
