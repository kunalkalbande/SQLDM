if (object_id('p_DeleteCustomDashboardWidgetRecord') is not null)
begin
drop procedure p_DeleteCustomDashboardWidgetRecord
end
go

-- To Delete record from CustomDashboard for specific CustomDashboardId
CREATE PROCEDURE p_DeleteCustomDashboardWidgetRecord(
	@widgetID int,
	@isRecordDeleted bit OUT
)
AS
begin
BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION

DECLARE @countRecord int 

SELECT @countRecord = 0;

	SELECT @isRecordDeleted = 0;
	BEGIN
		
			SELECT @countRecord = COUNT(WidgetID) from [CustomDashboardWidgets] WHERE WidgetID = @widgetID
			-- TO DO REMOVE WIDGET MAPS FOR THE SAME WIDGET
			--Removing Records
			--From Widget for respective WidgetID
			if(@countRecord > 0)
			BEGIN
				DELETE from [WidgetSourceMapping] WHERE WidgetID = @widgetID
				DELETE from [WidgetTagMapping] WHERE WidgetID = @widgetID
			
				DELETE from [CustomDashboardWidgets] WHERE WidgetID = @widgetID
				SELECT @isRecordDeleted = 1;
			END
			ELSE
			BEGIN
				SELECT @isRecordDeleted = 0
			END
			
	END

	COMMIT
END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
END CATCH

RETURN @isRecordDeleted
END	
 
GO 

