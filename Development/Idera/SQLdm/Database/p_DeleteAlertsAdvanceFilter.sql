if(object_id('p_DeleteAlertsAdvanceFilter') is not null)
begin
drop procedure p_DeleteAlertsAdvanceFilter
end
GO
CREATE PROCEDURE [dbo].[p_DeleteAlertsAdvanceFilter](@FilterName nvarchar(64))
AS
BEGIN
	DELETE from [dbo].[AlertsAdvanceFilter] where [FilterName] = @FilterName
END
GO

--EXEC p_DeleteAlertsAdvanceFilter 'A';
