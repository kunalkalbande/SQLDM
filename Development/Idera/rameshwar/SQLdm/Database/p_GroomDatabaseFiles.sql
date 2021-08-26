if (object_id('Grooming.p_GroomDatabaseFiles') is not null)
begin
drop procedure Grooming.p_GroomDatabaseFiles
end
go
create procedure [Grooming].[p_GroomDatabaseFiles]
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

Declare @KeysToDelete Table (FileID int not null primary key)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DatabaseFiles'

if @Deleted = 1
begin
	set @BlockName = @BlockName + ' (deleting)'
end
			
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	           
		if (@SQLServerID is not null)
		begin
			Delete From @KeysToDelete;
							
			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) FileID
			From dbo.DatabaseFiles df with(nolock)
			    Inner Join SQLServerDatabaseNames dn (nolock) on df.DatabaseID = dn.DatabaseID
			Where dn.SQLServerID = @SQLServerID;
		
			Delete DF
			From dbo.DatabaseFiles DF
			Inner Join @KeysToDelete K
				On K.FileID = DF.FileID;
		end
		else
		begin
			Delete From @KeysToDelete;

			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) FileID
			From dbo.DatabaseFiles df with(nolock)
			Inner Join dbo.SQLServerDatabaseNames dn with(nolock)
				On df.DatabaseID = dn.DatabaseID
			Where dn.SQLServerID = @SQLServerID;
		
			Delete DF
			From dbo.DatabaseFiles DF
			Inner Join @KeysToDelete K
				On K.FileID = DF.FileID;
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