if (object_id('p_GetBlocksList') is not null)
begin
drop procedure [p_GetBlocksList]
end
go

create procedure [p_GetBlocksList]
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null
as
begin
	DECLARE @err int

	if ((@StartDateTime is null) or (@EndDateTime is null))
	begin
		select SQLServerID, UTCCollectionDateTime, XDLData 
			from Blocks
			where SQLServerID = @SQLServerID
			order by UTCCollectionDateTime desc
	end
	else
	begin
		select SQLServerID, UTCCollectionDateTime, XDLData
			from Blocks
			where SQLServerID = @SQLServerID and UTCCollectionDateTime between @StartDateTime and @EndDateTime
			order by UTCCollectionDateTime desc
	end

	SELECT @err = @@error
	
	RETURN @err
end