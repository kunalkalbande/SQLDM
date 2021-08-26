if (object_id('p_DeleteCustomDashboardRecord') is not null)
begin
drop procedure p_DeleteCustomDashboardRecord
end
go

-- To Delete record from CustomDashboard for specific CustomDashboardId
CREATE PROCEDURE p_DeleteCustomDashboardRecord(
	@customDashboardId int,
	@customDashboardName nvarchar(500) out,
	@isDefaultOnUI bit out,
	@userSID nvarchar(200) out,
	@errorCode INT OUT
)
AS
begin

BEGIN TRY
   -- Start A Transaction

	BEGIN TRANSACTION

	
		
			SELECT @customDashboardName=[CustomDashboardName],
			@isDefaultOnUI =[IsDefaultOnUI],
			@userSID =[UserSID]
			FROM [CustomDashboard]
			WHERE CustomDashboardId = @customDashboardId
			
			-- TO DO REMOVE WIDGET MAPS FOR THE SAME CUSTOMDASHBOARDID
			--Removing Records
			--From Widget for respective Dashboard
			DELETE FROM [WidgetSourceMapping] WHERE WidgetID IN (SELECT WidgetID FROM [CustomDashboardWidgets] WHERE DashboardID = @customDashboardId)
			DELETE FROM [WidgetTagMapping] WHERE WidgetID IN (SELECT WidgetID FROM [CustomDashboardWidgets] WHERE DashboardID = @customDashboardId)
			
			DELETE FROM [CustomDashboardWidgets] WHERE DashboardID = @customDashboardId
			DELETE FROM [CustomDashboard]
			WHERE CustomDashboardId = @customDashboardId
	 
	COMMIT TRANSACTION
	

END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
 SET @errorCode = @@error
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
END CATCH

END	
 
GO 

