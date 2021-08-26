if (object_id('p_DeleteReplicationSessionFromServer') is not null)
begin
drop procedure [p_DeleteReplicationSessionFromServer]
end
go

Create procedure p_DeleteReplicationSessionFromServer
@publisher nvarchar(255),
@publisherdb nvarchar(255),
@publication nvarchar(255),
@subscriberInstance nvarchar(255) = null,
@subscriberDB nvarchar(255) = null
as

begin
delete from ReplicationTopology with (tablockx)
where lower([dbo].[fn_GetServerName](PublisherInstance)) = lower([dbo].[fn_GetServerName](@publisher))
and PublisherDB = @publisherdb 
and Publication = @publication
and (lower([dbo].[fn_GetServerName](SubscriberInstance)) = isnull(lower([dbo].[fn_GetServerName](@subscriberInstance)), lower([dbo].[fn_GetServerName](SubscriberInstance))) or SubscriberDBID is null)
and (SubscriberDB = isnull(@subscriberDB,SubscriberDB))
end
