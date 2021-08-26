if (object_id('p_GetMaxAlertID') is not null)
begin
drop procedure [p_GetMaxAlertID]
end
go

create procedure [p_GetMaxAlertID] 
	@ServerName nvarchar(256),
	@ReturnAlertID bigint output
as
begin
	
declare @e int
declare @maxid bigint

	if (@ServerName is null) 
	begin
		select @maxid = max(AlertID) from Alerts
	end
	else
	begin
		select @maxid = max(AlertID) from Alerts
			where [ServerName] = @ServerName
	end
	select @e = @@error
	select @ReturnAlertID = @maxid

	return @e
end