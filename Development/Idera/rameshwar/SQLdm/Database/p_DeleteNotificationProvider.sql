if (object_id('p_DeleteNotificationProvider') is not null)
begin
drop procedure [p_DeleteNotificationProvider]
end
go

create procedure [p_DeleteNotificationProvider] 
	@ProviderID uniqueidentifier
as
begin
	DECLARE @err int
	DECLARE @id uniqueidentifier
	
	DELETE FROM [NotificationProviders] 
		WHERE [ProviderId] = @ProviderID 

	SELECT @err = @@error

	RETURN @err
end
