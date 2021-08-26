/****** Object:  Function [dbo].[p_CheckDashboardExists] 
Custom Dashboard functionality to check if custom dashboard already exists
    Script Date: 26-May-15 11:35:58 AM ******/

if (object_id('p_CheckDashboardExists') is not null)
begin
drop procedure p_CheckDashboardExists
end
go

-- To Delete record from CustomDashboard for specific CustomDashboardId
CREATE PROCEDURE p_CheckDashboardExists(
	@CustomDashboardId BIGINT,
	@isRecordFound BIT OUTPUT
)
AS
BEGIN
	DECLARE @ret INT;
	SELECT @ret = COUNT(CustomDashboardName) FROM dbo.[CustomDashboard] WHERE CustomDashboardId = @CustomDashboardId;
	SELECT @ret;
		IF(@ret > 0) SELECT @isRecordFound = 1;
		ELSE SELECT @isRecordFound = 0;
END
 

GO