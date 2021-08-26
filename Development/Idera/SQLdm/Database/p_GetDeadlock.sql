if (object_id('p_GetDeadlock') is not null)
begin
drop procedure [p_GetDeadlock]
end
go

create procedure [p_GetDeadlock]
	@AlertID bigint
as
begin
	DECLARE @err int
	DECLARE @linkid uniqueidentifier

	-- if no create date is specified then set one
	SELECT @linkid = [LinkedData]
		from Alerts where AlertID = @AlertID

	if (@linkid is not null)
	begin
		select SQLServerID, UTCCollectionDateTime, XDLData 
			from Deadlocks 
			where DeadlockID = @linkid
	end

	SELECT @err = @@error
	
	RETURN @err
end
