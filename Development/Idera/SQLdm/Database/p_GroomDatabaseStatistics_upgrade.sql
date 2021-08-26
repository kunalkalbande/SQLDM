if (object_id('Grooming.p_GroomDatabaseStatistics_upgrade') is not null)
begin
drop procedure Grooming.p_GroomDatabaseStatistics_upgrade
end
go
create procedure [Grooming].[p_GroomDatabaseStatistics_upgrade]
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

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DatabaseStatistics_upgrade'
	
exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, @InstanceName

declare @RowsRemaining bigint

select 
	@RowsRemaining = sum(rows)
from
	sys.partitions
where
	object_id = object_id('DatabaseStatistics_upgrade')
	and index_id < 2 

-- Drop former upgrade script	
if (isnull(@RowsRemaining,0) <= 0)
begin

	if (object_id('p_UpgradeDatabaseStatistics') is not null)
	begin
	exec('drop procedure [p_UpgradeDatabaseStatistics]')
	end


	IF (OBJECT_ID('p_UpgradeDatabaseStatistics') is null)
	begin
	exec('
	create procedure [p_UpgradeDatabaseStatistics]
		@RowsRemaining bigint output
	as
	begin
		set @RowsRemaining = 0
		return @RowsRemaining
	end')
	
	if (object_id('DatabaseStatistics_upgrade') is not null)
	begin
		exec('drop table DatabaseStatistics_upgrade')
	end
end


end		

return @RC

end
