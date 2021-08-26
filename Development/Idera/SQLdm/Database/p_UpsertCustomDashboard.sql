if (object_id('p_UpsertCustomDashboard') is not null)
begin
drop procedure [p_UpsertCustomDashboard]
end
go

create procedure [p_UpsertCustomDashboard] 
@dashboardName nvarchar(500),
@isDefaultOnUI bit,
@SID nvarchar(200),
@tags varchar(max) = null,
@RecordTimestamp datetime,
@customDashboardID INT OUT

as
begin
DECLARE @countCustomDashboard int


BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION


	DECLARE @TotalCountInCustom BIGINT;
-- check if for record, it is update or insert call
	select @countCustomDashboard = count(CustomDashboardName) 
	from [CustomDashboard] WITH (NOLOCK)
	where CustomDashboardId = @customDashboardID
	
	SELECT @TotalCountInCustom = COUNT(0) FROM [CustomDashboard] WITH (NOLOCK);
	
	
	IF(@isDefaultOnUI = 1)
	BEGIN
		UPDATE [CustomDashboard] SET [IsDefaultOnUI] = 0 WHERE [IsDefaultOnUI] = 1
	END
	--[START] SQLdm 10.0 (Gaurav Karwal) - we need to have atleast 1 dashboard which is a default on UI so overriding the passed value in case there is only one record
	IF(@isDefaultOnUI = 0 AND @TotalCountInCustom IN (0,1))
	BEGIN
		SELECT @isDefaultOnUI = 1;
	END
	--[END] SQLdm 10.0 (Gaurav Karwal) - we need to have atleast 1 dashboard which is a default on UI so overriding the passed value in case there is only one record
	
	--set @@RecordTimestamp= CURRENT_TIMESTAMP
	-- update if permission exists.
	if (@countCustomDashboard != 0 )
		begin
			-- update existing record in table
			update [dbo].[CustomDashboard]
			set [CustomDashboardName] = @dashboardName, [IsDefaultOnUI] = @isDefaultOnUI,
			[UserSID] = LOWER(@SID),
			Tags = @tags, [RecordUpdateDateTimestamp]=@RecordTimestamp
			where CustomDashboardId = @customDashboardID
		end
	else
		begin
			-- insert new record in table
			INSERT INTO [dbo].[CustomDashboard]
				   ([CustomDashboardName]
				   ,[IsDefaultOnUI]
				   ,[UserSID]
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 VALUES
				   (@dashboardName, @isDefaultOnUI, LOWER(@SID),@RecordTimestamp,@RecordTimestamp)
			SET @customDashboardID = SCOPE_IDENTITY()
		  end
	 
	
COMMIT

END TRY
BEGIN CATCH
	ROLLBACK  -- Roll back
	SELECT ERROR_NUMBER();

END CATCH
end
 
GO 
