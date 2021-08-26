if (object_id('p_UpdateReplicationTopologySubscriber') is not null)
begin
drop procedure p_UpdateReplicationTopologySubscriber
end
go

create PROCEDURE [dbo].[p_UpdateReplicationTopologySubscriber]
	-- Add the parameters for the stored procedure here
	@PublisherInstance nvarchar(128),
	@PublisherDB nvarchar(128),
	@SubscriberInstance nvarchar(128) = null,
	@SubscriberDB nvarchar(128) = null,
	@LastSnapshotDateTime datetime = null,
	@ReplicationType int = null,
	@SubscriptionType int = null,
	@LastSubscriberUpdate datetime = null,
	@LastSyncStatus tinyint = null,
	@LastSyncSummary nvarchar(128) = null,
	@LastSyncTime datetime = null,
	@Publication nvarchar(128) = null
	as
BEGIN
	exec p_UpdateReplicationTopology 
	1,--subscriber
	@PublisherInstance, 
	@PublisherDB,
	null,
	null,
	@SubscriberInstance, 
	@SubscriberDB,
	@LastSnapshotDateTime,
	null,
	null,
	null,
	null,
	null,
	@ReplicationType,
	@SubscriptionType,
	@LastSubscriberUpdate,
	@LastSyncStatus,
	@LastSyncSummary,
	@LastSyncTime,
	null,
	@Publication,
	null,
	null
END
go

if (object_id('p_UpdateReplicationTopologyDistributor') is not null)
begin
drop procedure p_UpdateReplicationTopologyDistributor
end
go

create PROCEDURE [dbo].[p_UpdateReplicationTopologyDistributor]
	-- Add the parameters for the stored procedure here
	@PublisherInstance nvarchar(128),
	@PublisherDB nvarchar(128),
	@DistributorInstance nvarchar(128) = null,
	@DistributorDB nvarchar(128) = null,
	@SubscriberInstance nvarchar(128) = null,
	@SubscriberDB nvarchar(128) = null,
	@LastSnapshotDateTime datetime = null,
	@SubscribedTransactions int = null,
	@NonSubscribedTransactions int = null,
	@MaxSubscriptionLatency int = null,
	@Publication nvarchar(128) = null,
	@PublicationDescription nvarchar(155) = null,
	@Articles int = 0,
	@ReplicationType tinyint = 0
	as
BEGIN
	exec p_UpdateReplicationTopology 
	2,--distributor
	@PublisherInstance, 
	@PublisherDB,
	@DistributorInstance,
	@DistributorDB,
	@SubscriberInstance, 
	@SubscriberDB,
	@LastSnapshotDateTime,
	@SubscribedTransactions,
	@NonSubscribedTransactions,
	null,
	null,
	@MaxSubscriptionLatency,
	@ReplicationType,
	null,
	null,
	null,
	null,
	null,
	null,
	@Publication,
	@PublicationDescription,
	@Articles	
END
go

if (object_id('p_UpdateReplicationTopology') is not null)
begin
drop procedure p_UpdateReplicationTopology
end
go

create PROCEDURE [dbo].[p_UpdateReplicationTopology]
	-- Add the parameters for the stored procedure here
	@Role int,
	@PublisherInstance nvarchar(128),
	@PublisherDB nvarchar(128),
	@DistributorInstance nvarchar(128) = null,
	@DistributorDB nvarchar(128) = null,
	@SubscriberInstance nvarchar(128) = null,
	@SubscriberDB nvarchar(128) = null,
	@LastSnapshotDateTime datetime = null,
	@SubscribedTransactions int = null,
	@NonSubscribedTransactions int = null,
	@NonDistributedTransactions int = null,
	@ReplicationLatency float = null,
	@MaxSubscriptionLatency int = null,
	@ReplicationType tinyint = null,
	@SubscriptionType tinyint = null,
	@LastSubscriberUpdate datetime = null,
	@LastSyncStatus tinyint = null,
	@LastSyncSummary nvarchar(128) = null,
	@LastSyncTime datetime = null,
	@SubscriptionStatus tinyint = null,
	@Publication nvarchar(128) = null,
	@PublicationDescription nvarchar(255) = null,
	@ArticleCount int = null
	as
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @PublisherDatabaseID int,
			@SubscriberDatabaseID int,
			@DistributorDatabaseID int,
			@InnerReturnMessage nvarchar(128),
			@LookupServerID int

	if @PublisherInstance is not null
	begin
		select @LookupServerID = null
		select @LookupServerID = SQLServerID
		 from MonitoredSQLServers (nolock) where lower(InstanceName) =  lower(dbo.fn_GetServerName(@PublisherInstance))
		
		if @LookupServerID is not null and @PublisherDB is not null and @PublisherDB != ''
  		execute [p_InsertDatabaseName] 
		   @LookupServerID
		  ,@PublisherDB
		  ,0
		  ,null
		  ,@PublisherDatabaseID output
		  ,@InnerReturnMessage output
	end

	if @SubscriberInstance is not null
	begin
		select @LookupServerID = null		
		select @LookupServerID = SQLServerID
		 from MonitoredSQLServers (nolock) where lower(InstanceName) =  lower(dbo.fn_GetServerName(@SubscriberInstance))

		if @LookupServerID is not null and @SubscriberDB is not null and @SubscriberDB != ''
		execute [p_InsertDatabaseName] 
		   @LookupServerID
		  ,@SubscriberDB
		  ,0
		  ,null
		  ,@SubscriberDatabaseID output
		  ,@InnerReturnMessage output
	end

	if @DistributorInstance is not null
	begin
		select @LookupServerID = null
		select @LookupServerID = SQLServerID
		 from MonitoredSQLServers (nolock) 
		where lower(InstanceName) = lower(dbo.fn_GetServerName(@DistributorInstance)) 


		if @LookupServerID is not null and @DistributorDB is not null and @DistributorDB != ''
		execute [p_InsertDatabaseName] 
		   @LookupServerID
		  ,@DistributorDB
		  ,0
		  ,null
		  ,@DistributorDatabaseID output
		  ,@InnerReturnMessage output
	end

if not exists (select PublisherInstance from ReplicationTopology with (tablockx)
where PublisherInstance = @PublisherInstance 
	and PublisherDB = @PublisherDB
	and Publication = @Publication
	and SubscriberInstance = @SubscriberInstance
	and SubscriberDB = @SubscriberDB)
begin
INSERT INTO [ReplicationTopology]
           (PublisherInstance,
			PublisherDB,
			PublisherDBID,
			DistributorInstance,
			DistributorDB,
			DistributorDBID,
			SubscriberInstance,
			SubscriberDB,
			SubscriberDBID,
			LastPublisherSnapshotDateTime,
			SubscribedTransactions,
			NonSubscribedTransactions,
			NonDistributedTransactions,
			ReplicationLatency,
			MaxSubscriptionLatency,
			ReplicationType,
			SubscriptionType,
			LastSubscriberUpdate,
			LastSyncStatus,
			LastSyncSummary,
			LastSyncTime,
			SubscriptionStatus,
			Publication,
			PublicationDescription,
			ArticleCount)
     VALUES
           (@PublisherInstance,
			@PublisherDB,
			@PublisherDatabaseID,
			@DistributorInstance,
			@DistributorDB,
			@DistributorDatabaseID,
			@SubscriberInstance,
			@SubscriberDB,
			@SubscriberDatabaseID,
			@LastSnapshotDateTime,
			@SubscribedTransactions,
			@NonSubscribedTransactions,
			@NonDistributedTransactions,
			@ReplicationLatency,
			@MaxSubscriptionLatency,
			@ReplicationType,
			@SubscriptionType,
			@LastSubscriberUpdate,
			@LastSyncStatus,
			@LastSyncSummary,
			@LastSyncTime,
			@SubscriptionStatus,
			@Publication,
			@PublicationDescription,
			@ArticleCount)

	--if we have just added a record with a valid subscriber
	--check if there is an old orphaned publication
	if (@SubscriberInstance is not null and @SubscriberInstance <> '')
		and (@SubscriberDB is not null and @SubscriberDB <> '')
		and exists (select PublisherInstance from ReplicationTopology with (tablockx)
			where PublisherInstance = @PublisherInstance 
				and PublisherDB = @PublisherDB
				and Publication = @Publication
				and (SubscriberInstance is null or SubscriberInstance = '')
				and (SubscriberDB is null or SubscriberDB = ''))
	begin
		Delete from ReplicationTopology with (tablockx)
		where PublisherInstance = @PublisherInstance 
					and PublisherDB = @PublisherDB
					and Publication = @Publication
					and (SubscriberInstance is null or SubscriberInstance = '')
					and (SubscriberDB is null or SubscriberDB = '')
	end	

end
else
begin
	if @Role = 0 --publisher
	begin
		update [ReplicationTopology] with (tablockx)
           set PublisherDBID = @PublisherDatabaseID,
			DistributorInstance = @DistributorInstance,
			DistributorDB = @DistributorDB,
			DistributorDBID = @DistributorDatabaseID,
			SubscriberInstance = @SubscriberInstance,
			SubscriberDB = @SubscriberDB,
			SubscriberDBID = @SubscriberDatabaseID,
			LastPublisherSnapshotDateTime = @LastSnapshotDateTime,
			SubscribedTransactions = @SubscribedTransactions,
			NonSubscribedTransactions = @NonSubscribedTransactions,
			NonDistributedTransactions = @NonDistributedTransactions,
			ReplicationLatency = @ReplicationLatency,
			MaxSubscriptionLatency = @MaxSubscriptionLatency,
			ArticleCount = @ArticleCount,
			SubscriptionStatus = @SubscriptionStatus
			where PublisherInstance = @PublisherInstance 
			and PublisherDB = @PublisherDB 
			and SubscriberInstance = @SubscriberInstance
			and SubscriberDB = @SubscriberDB
			and Publication = @Publication
	end
	else if @Role = 1 -- subscriber
	begin
	update [ReplicationTopology] with (tablockx)
           set LastSubscriberSnapshotDateTime = @LastSnapshotDateTime,
			ReplicationType = @ReplicationType,
			SubscriptionType = @SubscriptionType,
			LastSubscriberUpdate = @LastSubscriberUpdate,
			LastSyncStatus = @LastSyncStatus,
			LastSyncSummary = @LastSyncSummary,
			LastSyncTime = @LastSyncTime,
			Publication = @Publication
			where PublisherInstance = @PublisherInstance 
			and PublisherDB = @PublisherDB
			and SubscriberInstance = @SubscriberInstance
			and SubscriberDB = @SubscriberDB
			and Publication = @Publication
	end
	else if @Role = 2 --distributor
	begin
		update [ReplicationTopology] with (tablockx)
           set 
            PublisherDBID = @PublisherDatabaseID,
			DistributorInstance = @DistributorInstance,
			DistributorDB = @DistributorDB,
			DistributorDBID = @DistributorDatabaseID,
			SubscriberInstance = @SubscriberInstance,
			SubscriberDB = @SubscriberDB,
			SubscriberDBID = @SubscriberDatabaseID,
			LastDistributorSnapshotDateTime = @LastSnapshotDateTime,
			SubscribedTransactions = @SubscribedTransactions,
			NonSubscribedTransactions = @NonSubscribedTransactions,
			MaxSubscriptionLatency = @MaxSubscriptionLatency,
		    Publication = @Publication,
			PublicationDescription = @PublicationDescription,
			ArticleCount = case @ReplicationType when 2 then ArticleCount else isnull(@ArticleCount, ArticleCount) end,
			ReplicationType = @ReplicationType
			where PublisherInstance = @PublisherInstance 
			and PublisherDB = @PublisherDB
			and SubscriberInstance = @SubscriberInstance
			and SubscriberDB = @SubscriberDB
			and Publication = @Publication
	end
end			
END
