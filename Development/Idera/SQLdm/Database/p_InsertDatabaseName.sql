if (object_id('p_InsertDatabaseName') is not null)
begin
drop procedure p_InsertDatabaseName
end
go
create procedure p_InsertDatabaseName
	@SQLServerID int,
	@DatabaseName nvarchar(255),
	@SystemDatabase bit,
	@DatabaseCreateDate datetime = null,
	@DatabaseID int output,
	@ReturnMessage nvarchar(128) output,
	@IsDeletedFlag bit = 0				--SQLdm 10.1.3 (Varun Chopra) - Added flag to update 'IsDeleted' field
as
begin

declare @LookupDatabaseID int,
		@LookupDatabaseCreateDate datetime

select 
	@LookupDatabaseID = DatabaseID,
	@LookupDatabaseCreateDate = CreationDateTime 
from 
	SQLServerDatabaseNames (nolock)
where 
	SQLServerID = @SQLServerID
	and DatabaseName = @DatabaseName

if (@LookupDatabaseID is null)
begin
	--Checking the existence of parameter SQLServerId. Fix for rally defect DE20479. Aditya Shukla SQLdm 8.6
	if exists(select 1 from MonitoredSQLServers (nolock) where SQLServerID = @SQLServerID)
	begin
		insert into [SQLServerDatabaseNames]
		   ([SQLServerID]
		   ,[DatabaseName]
		   ,[SystemDatabase]
		   ,[CreationDateTime])
		Values
		   (@SQLServerID
		   ,@DatabaseName
		   ,@SystemDatabase
		   ,@DatabaseCreateDate)

		select @LookupDatabaseID = SCOPE_IDENTITY()
	end
	else
	begin
		set @LookupDatabaseID = NULL
	end
end

if (@DatabaseCreateDate is not null) and (@LookupDatabaseID is not null) and (isnull(@LookupDatabaseCreateDate,'1900-01-01 00:00:00 AM') <> @DatabaseCreateDate)
begin
	update [SQLServerDatabaseNames]
		set [CreationDateTime] = @DatabaseCreateDate,[IsDeleted]=0
	where 
		SQLServerID = @SQLServerID
		and DatabaseName = @DatabaseName
end

--SQLdm 10.1.3 (Varun Chopra) - Update'IsDeleted' field
if (@IsDeletedFlag = 1)
begin
	update [SQLServerDatabaseNames]
		set [IsDeleted]=1
	where 
		SQLServerID = @SQLServerID
		and DatabaseName = @DatabaseName
end

set @DatabaseID = @LookupDatabaseID

end
