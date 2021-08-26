--SQLdm 9.0 (Ankit Srivastava) -- Added new procedure for inserting data into the LatestGroomingStatus table
--declare @currDate datetime,@id uniqueidentifier
--Select @currDate=getutcdate()
--select @id=newid()
--EXEC exec [dbo].[p_AddLatestGroomingStatus] 37,@id,@currDate,0,'Timeout',1
IF (object_id('p_AddLatestGroomingStatus') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddLatestGroomingStatus
END
GO

CREATE PROCEDURE [dbo].[p_AddLatestGroomingStatus]
(
	@SQLServerID INT, 
	@GroomingRunID uniqueidentifier, 
	@GroomingDateTimeUTC DateTime, 
	@Status smallint,-- failed =0 , succeeded =1, Hung=2
	@LastStatusMessage nvarchar(250),
	@IsPrimary bit
)
AS
BEGIN
	declare @updatecmd nvarchar(1000)
	declare @parms nvarchar(1000)

	Set @updatecmd = 'UPDATE [LatestGroomingStatus]	set 
				GroomingRunID = @GroomingRunID, 
				GroomingDateTimeUTC = @GroomingDateTimeUTC,
				[Status] = @Status,
				LastStatusMessage=@LastStatusMessage,
				[IsPrimary]=@IsPrimary ' +
				Case When @SQLServerID IS NULL Then 'where SQLServerID IS NULL;'
				Else 'where SQLServerID = @SQLServerID;' END ;
				
	Set @parms = '@SQLServerID INT, @GroomingRunID uniqueidentifier, @GroomingDateTimeUTC DateTime, @Status smallint, @LastStatusMessage nvarchar(250), @IsPrimary bit';

	IF (NOT EXISTS(Select * from [LatestGroomingStatus] where SQLServerID = @SQLServerID) AND @SQLServerID IS NOT NULL) OR
	   (NOT EXISTS(Select * from [LatestGroomingStatus] where SQLServerID IS NULL) AND @SQLServerID IS NULL)
	BEGIN
		INSERT INTO [LatestGroomingStatus] (SQLServerID, GroomingRunID, GroomingDateTimeUTC, [Status], LastStatusMessage,[IsPrimary])
			VALUES(@SQLServerID,  @GroomingRunID, @GroomingDateTimeUTC, @Status, @LastStatusMessage,@IsPrimary)
	END
	ELSE
	BEGIN
		exec sp_executesql 
						@updatecmd, 
						@parms, 
						@SQLServerID = @SQLServerID, 
						@GroomingRunID = @GroomingRunID, 
						@GroomingDateTimeUTC = @GroomingDateTimeUTC, 
						@Status = @Status, 
						@LastStatusMessage = @LastStatusMessage, 
						@IsPrimary = @IsPrimary;
	END
END