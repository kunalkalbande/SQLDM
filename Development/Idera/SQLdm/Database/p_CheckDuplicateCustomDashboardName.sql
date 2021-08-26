/****** Object:  Function [dbo].[fn_CheckDuplicateCustomDashboardName] 
Custom Dashboard functionality to check new custom dashboard name does not already exists
    Script Date: 26-May-15 11:35:58 AM ******/

if (object_id('p_CheckDuplicateCustomDashboardName') is not null)
begin
drop procedure p_CheckDuplicateCustomDashboardName
end
go

-- To Delete record from CustomDashboard for specific CustomDashboardId
CREATE PROCEDURE p_CheckDuplicateCustomDashboardName(
	@CustomDashboardName nvarchar(500) ,
	@userSID nvarchar(400),
	@isRecordFound bit out
)
AS
begin
	declare @ret int
	select @ret = COUNT(CustomDashboardName) from dbo.[CustomDashboard] where LOWER(CustomDashboardName) = LOWER(@CustomDashboardName) AND LOWER(UserSID) = LOWER(@userSID)
		if(@ret != 0)
		
			SET @isRecordFound = 1
		
		else
			SET @isRecordFound = 0
	return @ret
end
 

GO