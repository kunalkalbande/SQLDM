if(object_id('p_AddAlertsAdvanceFilter') is not null)
begin
drop procedure p_AddAlertsAdvanceFilter
end
GO
CREATE PROCEDURE [dbo].[p_AddAlertsAdvanceFilter](@FilterName nvarchar(64), @Config nvarchar(max))
AS
BEGIN
	if(exists(select [FilterName] from [dbo].[AlertsAdvanceFilter] where [FilterName] = @FilterName))
	BEGIN
		UPDATE [dbo].[AlertsAdvanceFilter] SET [Config] = @Config WHERE [FilterName]=@FilterName
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[AlertsAdvanceFilter]([FilterName],[Config]) VALUES (@FilterName,@Config)
	END
END
GO
