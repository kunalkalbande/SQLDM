/****** Object:  Function [dbo].[fn_CheckDuplicateCustomDashboardName] 
Custom Dashboard functionality to Get Match Type
    Script Date: 26-May-15 11:35:58 AM ******/
if (object_id('p_GetMatchTypes') is not null)
begin
drop procedure [p_GetMatchTypes]
end
go

create procedure [p_GetMatchTypes] 

AS
BEGIN
	declare @e int
	
	BEGIN TRANSACTION
	
	SET @e = @@error
	IF (@e = 0)
	BEGIN
		-- check if records for UserSID
		SELECT [MatchID]
      ,[MatchType]
  FROM [dbo].[CustomDashboardMatchTypes]
	
	END
	
	IF (@e = 0)
		COMMIT
	ELSE
		ROLLBACK		

	return @e

END
 
GO 
