if(object_id('p_GetAlertsAdvanceFilters') is not null)
begin
drop procedure p_GetAlertsAdvanceFilters
end
GO
CREATE PROCEDURE [dbo].[p_GetAlertsAdvanceFilters]
AS
BEGIN
	SELECT * from [dbo].[AlertsAdvanceFilter] order by [FilterName]
END
GO
