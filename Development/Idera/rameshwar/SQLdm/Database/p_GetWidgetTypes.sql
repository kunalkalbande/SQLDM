if (object_id('p_GetWidgetTypes') is not null)
begin
drop procedure [p_GetWidgetTypes]
end
go

create procedure [p_GetWidgetTypes] 

AS
BEGIN
	declare @e int
	
	BEGIN TRANSACTION
	
	SET @e = @@error
	IF (@e = 0)
	BEGIN
		-- check if records for UserSID
		SELECT [WidgetTypeID]
		,[WidgetType]
		FROM [dbo].[CustomDashboardWidgetTypes]
	
	END
	
	IF (@e = 0)
		COMMIT
	ELSE
		ROLLBACK		

	return @e

END
 
GO 

