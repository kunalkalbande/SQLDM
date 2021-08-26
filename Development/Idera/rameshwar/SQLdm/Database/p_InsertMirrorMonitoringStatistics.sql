if (object_id('p_InsertMirroringStatistics') is not null)
begin
drop procedure [p_InsertMirroringStatistics]
end
go

CREATE PROCEDURE [dbo].[p_InsertMirroringStatistics]
	-- Add the parameters for the stored procedure here
    @SQLServerID int,
	@dbname nvarchar(128),
	@instance_name nvarchar(128),
	@partner_name nvarchar(128), 
	@mirroring_guid uniqueidentifier,
	@role tinyint,
	@mirroring_state tinyint,
	@witness_status tinyint,
	@log_generation_rate int,
	@unsent_log int,
	@send_rate int,
	@unrestored_log int,
	@recovery_rate int,
	@transaction_delay int,
	@transactions_per_sec int,
	@average_delay int,
	@time_recorded datetime,
	@time_behind datetime,
	@local_time datetime,
	@partner_address nvarchar(128),
	@witness_address nvarchar(128),
	@safety_level int,
	@utc_collection_datetime datetime,
	@ReturnDatabaseID int output,  
	@ReturnMessage nvarchar(128) output
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @DatabaseID int,
		@InnerReturnMessage nvarchar(128)

execute [p_InsertDatabaseName] 
   @SQLServerID
  ,@dbname
  ,0
  ,null
  ,@DatabaseID output
  ,@InnerReturnMessage output
	

-- Insert statements for procedure here
INSERT INTO [MirroringStatistics]
           ([DatabaseID]
           ,[mirroring_guid]
           ,[UTCCollectionDateTime]
           ,[role]
           ,[mirroring_state]
           ,[witness_status]
           ,[log_generation_rate]
           ,[unsent_log]
           ,[send_rate]
           ,[unrestored_log]
           ,[recovery_rate]
           ,[transaction_delay]
           ,[transactions_per_sec]
           ,[average_delay]
           ,[time_recorded]
           ,[time_behind]
           ,[local_time])
     VALUES
           (@DatabaseID
           ,@mirroring_guid
           ,@utc_collection_datetime
           ,@role
           ,@mirroring_state
           ,@witness_status
           ,@log_generation_rate
           ,@unsent_log
           ,@send_rate
           ,@unrestored_log
           ,@recovery_rate
           ,@transaction_delay
           ,@transactions_per_sec
           ,@average_delay
           ,@time_recorded 
           ,@time_behind
           ,@local_time)

if not exists (select * from [MirroringParticipants]
where [DatabaseID] = @DatabaseID)
INSERT INTO [MirroringParticipants]
           ([DatabaseID]
           ,[mirroring_guid]
           ,[role]
           ,[principal_address]
           ,[Mirror_address]
           ,[witness_address]
           ,[safety_level]
           ,[is_suspended]
           ,[mirroring_state]
           ,[witness_status]
           ,[mirror_instanceID]
           ,[principal_instanceID]
		   ,[partner_instance]
           ,[last_updated])
     VALUES
           (@DatabaseID
           ,@mirroring_guid
           ,@role
           ,case @role when 2 then @partner_address else null end
           ,case @role when 1 then @partner_address else null end 
           ,@witness_address
           ,@safety_level
           ,0
           ,@mirroring_state
           ,@witness_status
           ,case @role when 2 then @SQLServerID else null end
           ,case @role when 1 then @SQLServerID else null end
		   ,@partner_name
           ,GETDATE())
else
update [MirroringParticipants]
           set [mirroring_guid]  = @mirroring_guid
           ,[role] = @role
           ,[principal_address] = case @role when 2 then @partner_address else null end
           ,[Mirror_address] = case @role when 1 then @partner_address else null end
           ,[witness_address] = @witness_address
           ,[safety_level] = @safety_level
           ,[mirroring_state] = @mirroring_state
           ,[witness_status] = @witness_status
           ,[mirror_instanceID] = case @role when 2 then @SQLServerID else null end
           ,[principal_instanceID] = case @role when 1 then @SQLServerID else null end
		   ,[partner_instance] = @partner_name
           ,[last_updated] = GETDATE()
where [DatabaseID] = @DatabaseID
           
          set @ReturnDatabaseID = @DatabaseID
           
END