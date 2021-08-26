if (object_id('p_GetCustomDashboards') is not null)
begin
drop procedure [p_GetCustomDashboards]
end
go

create procedure [p_GetCustomDashboards] 

@SID nvarchar(200)
AS
BEGIN
	declare @e int
	
	BEGIN TRANSACTION
	
	BEGIN
		-- check if records for UserSID
		SELECT [CustomDashboardId]
		, [CustomDashboardName]
      ,[IsDefaultOnUI]
	  ,Tags
      ,[RecordCreatedTimestamp]
      ,[RecordUpdateDateTimestamp]
		FROM CustomDashboard 
		WHERE LOWER(UserSID) = LOWER(@SID)
	
	END
	
	SET @e = ERROR_NUMBER();
	
	IF (@e = 0)
		COMMIT TRANSACTION
	ELSE 
		ROLLBACK TRANSACTION	 	

	

END
 
GO 
