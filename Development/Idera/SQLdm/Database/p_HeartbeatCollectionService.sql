if (object_id('p_HeartbeatCollectionService') is not null)
begin
drop procedure p_HeartbeatCollectionService
end
go
create procedure [dbo].[p_HeartbeatCollectionService](
	@ServiceId uniqueidentifier,
	@LastHeartbeatUTC DateTime
)
as
begin
	declare @id uniqueidentifier
	declare @e int
	declare @rc int

	update [CollectionServices] set [LastHeartbeatUTC] = @LastHeartbeatUTC 
		where ([CollectionServiceID] = @ServiceId)

	select @e = @@error, @rc = @@rowcount

	if ((@e = 0) and (@rc = 0))
	begin
		select @e = -1
	end

	return @e
end
