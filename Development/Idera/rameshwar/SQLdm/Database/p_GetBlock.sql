if (object_id('p_GetBlock') is not null)
begin
drop procedure [p_GetBlock]
end
go

create procedure [p_GetBlock]
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
			from Blocks
			where BlockID = @linkid
	end

	SELECT @err = @@error
	
	RETURN @err
end