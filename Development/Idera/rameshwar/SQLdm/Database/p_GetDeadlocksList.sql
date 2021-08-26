if (object_id('p_GetDeadlocksList') is not null)
begin
drop procedure [p_GetDeadlocksList]
end
go

create procedure [p_GetDeadlocksList]
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null
as
begin
	DECLARE @err int

	if ((@StartDateTime is null) or (@EndDateTime is null))
	begin
		select SQLServerID, UTCCollectionDateTime, XDLData 
			from Deadlocks 
			where SQLServerID = @SQLServerID
			order by UTCCollectionDateTime desc
	end
	else
	begin
		select SQLServerID, UTCCollectionDateTime, XDLData 
			from Deadlocks 
			where SQLServerID = @SQLServerID and UTCCollectionDateTime between @StartDateTime and @EndDateTime
			order by UTCCollectionDateTime desc
	end

	SELECT @err = @@error
	
	RETURN @err
end
