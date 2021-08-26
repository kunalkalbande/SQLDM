if (object_id('Grooming.p_GroomTableGrowth') is not null)
begin
drop procedure Grooming.p_GroomTableGrowth
end
go
create procedure [Grooming].[p_GroomTableGrowth]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000,
@CutoffDateTime datetime,
@SQLServerID int = null,
@InstanceName sysname = null,
@Deleted bit = 0,
@AggregationGrooming bit = 0
)
as
begin

set nocount on    
set ansi_warnings off 

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'TableGrowth'
	
if (@SQLServerID is not null)
begin					
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
		  ;with SQLServerTableDatabaseNames as (
            select  tn.DatabaseID, tn.TableID, dn.SQLServerID
            from SQLServerTableNames tn inner join 
			SQLServerDatabaseNames dn on dn.DatabaseID = tn.DatabaseID
          )
          DELETE TOP (@RecordsToDelete) tg
          FROM TableGrowth tg with (nolock) INNER JOIN
	      SQLServerTableDatabaseNames sstdn ON tg.TableID = sstdn.TableID
          Where tg.UTCCollectionDateTime <= @CutoffDateTime
		end
		else
		begin
		  if (@Deleted = 0)
		  begin
		    ;with SQLServerTableDatabaseNames as (
             select  tn.DatabaseID, tn.TableID, dn.SQLServerID
             from SQLServerTableNames tn inner join 
			  SQLServerDatabaseNames dn on dn.DatabaseID = tn.DatabaseID
            )
            DELETE TOP (@RecordsToDelete) tg
            FROM TableGrowth tg with (nolock)
            INNER JOIN SQLServerTableDatabaseNames sstdn
            ON tg.TableID = sstdn.TableID
            Where sstdn.SQLServerID = @SQLServerID
            And tg.UTCCollectionDateTime <= @CutoffDateTime
		  end
		  else
		  begin
		    ;with SQLServerTableDatabaseNames as (
             select  tn.DatabaseID, tn.TableID, dn.SQLServerID
             from SQLServerTableNames tn inner join 
			  SQLServerDatabaseNames dn on dn.DatabaseID = tn.DatabaseID
            )
            DELETE TOP (@RecordsToDelete) tg
            FROM TableGrowth tg with (nolock)
            INNER JOIN SQLServerTableDatabaseNames sstdn
            ON tg.TableID = sstdn.TableID
            Where sstdn.SQLServerID = @SQLServerID
		  end
		end
		
		
		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete OR @AggregationGrooming = 1)
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
