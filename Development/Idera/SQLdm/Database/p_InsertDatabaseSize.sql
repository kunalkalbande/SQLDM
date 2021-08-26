if (object_id('p_InsertDatabaseSize') is not null)
begin
drop procedure p_InsertDatabaseSize
end
go
create procedure p_InsertDatabaseSize
	@SQLServerID int,
	@DatabaseName nvarchar(255),
	@SystemDatabase bit,
	@UTCCollectionDateTime datetime,
	@DatabaseStatus int,
	@DataFileSizeInKilobytes decimal,        
	@LogFileSizeInKilobytes decimal, 
	@LogSizeInKilobytes decimal,         
	@DataSizeInKilobytes decimal,            
	@TextSizeInKilobytes decimal,            
	@IndexSizeInKilobytes decimal,           
	@LogExpansionInKilobytes decimal,        
	@DataExpansionInKilobytes decimal,       
	@PercentLogSpace float,                  
	@PercentDataSize float,        
	@TimeDeltaInSeconds float,
	@ReturnDatabaseID int output,
	@DatabaseCreateDate datetime,
	@ReturnMessage nvarchar(128) output
as
begin

declare @DatabaseID int,
		@InnerReturnMessage nvarchar(128),
		@DatabaseStatisticsTime datetime

execute [p_InsertDatabaseName] 
   @SQLServerID
  ,@DatabaseName
  ,@SystemDatabase
  ,@DatabaseCreateDate
  ,@DatabaseID output
  ,@InnerReturnMessage output

-- Fix for rally defect DE20479. Aditya Shukla SQLdm 8.6
-- Checking if the databaseid parameter returned from p_InsertDatabaseName has valid value or not
if(@DatabaseID is null)
begin
	--Returning prematurely if databaseid parameter is invalid
	set @ReturnDatabaseID = -1
	return
end

select @DatabaseStatisticsTime = max(UTCCollectionDateTime)
from DatabaseStatistics (nolock)
where DatabaseID = @DatabaseID

insert into [DatabaseSize]
	([DatabaseID]
	,[UTCCollectionDateTime]
	,[DatabaseStatus]
	,[DataFileSizeInKilobytes]
	,[LogFileSizeInKilobytes]
	,[LogSizeInKilobytes]
	,[DataSizeInKilobytes]
	,[TextSizeInKilobytes]
	,[IndexSizeInKilobytes] 
	,[LogExpansionInKilobytes] 
	,[DataExpansionInKilobytes] 
	,[PercentLogSpace] 
	,[PercentDataSize]
	,[TimeDeltaInSeconds]
	,[DatabaseStatisticsTime])
 values
	(@DatabaseID
	,@UTCCollectionDateTime
	,@DatabaseStatus
	,@DataFileSizeInKilobytes                
	,@LogFileSizeInKilobytes  
	,@LogSizeInKilobytes               
	,@DataSizeInKilobytes                    
	,@TextSizeInKilobytes                    
	,@IndexSizeInKilobytes                   
	,@LogExpansionInKilobytes                
	,@DataExpansionInKilobytes               
	,@PercentLogSpace                        
	,@PercentDataSize
	,@TimeDeltaInSeconds
	,@DatabaseStatisticsTime
	)

	if exists (select 1 from DatabaseSizeDateTime where DatabaseID = @DatabaseID)
	begin
		-- If the Database entry is present in the DatabaseSizeDateTime table then update other wise insert the row
		update DatabaseSizeDateTime set DatabaseID = @DatabaseID, UTCCollectionDateTime = @UTCCollectionDateTime where DatabaseID = @DatabaseID
	end
	else
	begin
		insert into DatabaseSizeDateTime (DatabaseID, UTCCollectionDateTime) values (@DatabaseID, @UTCCollectionDateTime)
	end
	
set @ReturnDatabaseID = @DatabaseID

end

