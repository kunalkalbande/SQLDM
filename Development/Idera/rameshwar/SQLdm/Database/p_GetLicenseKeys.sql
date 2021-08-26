if (object_id('p_GetLicenseKeys') is not null)
begin
drop procedure p_GetLicenseKeys
end
go
CREATE PROCEDURE [dbo].[p_GetLicenseKeys](
	@LicenseID UNIQUEIDENTIFIER,
	@ReturnServerCount int output,
	@ReturnInstanceName nvarchar(128) output
)
AS
BEGIN
	DECLARE @e INT
	DECLARE @rmsc INT
	DECLARE @instanceName nvarchar(128)
	declare @listener nvarchar(1024)
	
	-- set return values to current number of registered servers and SQL instance
	SELECT @rmsc = COUNT([SQLServerID]) FROM MonitoredSQLServers (NOLOCK) WHERE Active = 1  
	select @instanceName = CONVERT(nvarchar(128), serverproperty('servername'))
	
	select @listener = rInfo.Character_Value 
	  from RepositoryInfo rInfo
	  where rInfo.Name = 'AGListener';
	
	IF (@LicenseID IS NULL) 
	BEGIN
		SELECT [LicenseID],[LicenseKey],[DateAddedUtc] FROM [LicenseKeys] (NOLOCK)
		ORDER BY [DateAddedUtc]
	END
	ELSE
	BEGIN
		SELECT [LicenseID],[LicenseKey],[DateAddedUtc] FROM [LicenseKeys] (NOLOCK)
				WHERE ([LicenseID] = @LicenseID)
		ORDER BY [DateAddedUtc]
	END

	SELECT @e = @@error
	IF (@e = 0)
	BEGIN
		SELECT @ReturnServerCount = @rmsc
		select @ReturnInstanceName = @listener
		-- irst use the listener
		if @ReturnInstanceName is null
			SELECT @ReturnInstanceName = @instanceName
	END

	RETURN @e
END