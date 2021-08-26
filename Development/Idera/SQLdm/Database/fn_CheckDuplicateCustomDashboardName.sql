
/****** Object:  Function [dbo].[fn_CheckDuplicateCustomDashboardName] 
Custom Dashboard functionality to check new custom dashboard name does not already exists
    Script Date: 26-May-15 11:35:58 AM ******/
if (object_id('fn_CheckDuplicateCustomDashboardName') is not null)
begin
drop function fn_CheckDuplicateCustomDashboardName
end
go


create function fn_CheckDuplicateCustomDashboardName(
	
	@CustomDashboardName nvarchar(500),
	@SID nvarchar(200)
	)
	RETURNS INT
begin
	declare @ret int
	select @ret = COUNT(CustomDashboardName) from dbo.[CustomDashboard] where CustomDashboardName = @CustomDashboardName AND UserSID = CONVERT(varbinary(85), @SID,1)
		
	return @ret
end
 

GO


