if (object_id('p_GetAzureSubscriptions') is not null)
begin
drop procedure [p_GetAzureSubscriptions]
end
go

CREATE PROCEDURE [dbo].[p_GetAzureSubscriptions]
AS
BEGIN
	DECLARE @error int

	SELECT
		azSubs.[ID] AS [AzureSubscriptionId],
		azSubs.[SubscriptionId],
		azSubs.[Description] AS [SubscriptionDescription]
	FROM AzureSubscription azSubs (NOLOCK)

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while retrieving the Azure Subscriptions.', 10, 1)
        RETURN(@error)
END
