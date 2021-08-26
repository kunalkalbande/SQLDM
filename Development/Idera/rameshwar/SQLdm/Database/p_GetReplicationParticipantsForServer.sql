if (object_id('p_GetReplicationParticipantsForServer') is not null)
begin
drop procedure p_GetReplicationParticipantsForServer
end
go

CREATE procedure [dbo].[p_GetReplicationParticipantsForServer]
	@serverID int = null
as

begin
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	select 
	rt.ArticleCount,
	isnull(rt.PublisherInstance, '') as PublisherInstance,
    isnull(rt.PublisherDB, '') as PublisherDB, 
    isnull(rt.DistributorInstance,'') as DistributorInstance, 
    isnull(rt.DistributorDB,'') as DistributorDB, 
	isnull(rt.SubscriberInstance,'') as SubscriberInstance,
	isnull(rt.SubscriberDB,'') as subscriberDB,
	rt.LastDistributorSnapshotDateTime, rt.SubscribedTransactions, rt.NonSubscribedTransactions, rt.NonDistributedTransactions, 
	rt.ReplicationLatency, rt.MaxSubscriptionLatency, rt.ReplicationType, rt.SubscriptionType, rt.LastSubscriberUpdate,
	rt.LastSyncStatus, rt.LastSyncSummary, rt.LastSyncTime, rt.SubscriptionStatus, 
	isnull(rt.Publication,'') as Publication, rt.PublicationDescription,
	pubdn.SQLServerID, distdn.SQLServerID,subdn.SQLServerID
	from ReplicationTopology rt 
	left join SQLServerDatabaseNames pubdn (nolock) on rt.PublisherDBID = pubdn.DatabaseID
	--left join MonitoredSQLServers ms1 on pubdn.SQLServerID = ms1.SQLServerID
	left join SQLServerDatabaseNames distdn (nolock) on rt.DistributorDBID = distdn.DatabaseID
	--left join MonitoredSQLServers ms2 on distdn.SQLServerID = ms2.SQLServerID
	left join SQLServerDatabaseNames subdn (nolock) on rt.SubscriberDBID = subdn.DatabaseID
	--left join MonitoredSQLServers ms3 on subdn.SQLServerID = ms3.SQLServerID
	where @serverID in (pubdn.SQLServerID, distdn.SQLServerID,subdn.SQLServerID) or @serverID is null
end
 
