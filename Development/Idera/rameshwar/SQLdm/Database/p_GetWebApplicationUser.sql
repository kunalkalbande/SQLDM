IF (OBJECT_ID('p_GetWebApplicationUser') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetWebApplicationUser
END
GO
--p_GetWebApplicationUser 'FREEDOM\admin'
CREATE PROCEDURE p_GetWebApplicationUser
@LoginName NVARCHAR(500)
AS
BEGIN
	DECLARE @LoginFullName NVARCHAR(500);
	DECLARE @AppSec TABLE(AppEnabled INT);
	DECLARE @IsAppSecEnabled INT;
	SET @LoginFullName = '';
	
	INSERT INTO @AppSec EXEC p_IsAppSecEnabled;
	SELECT @IsAppSecEnabled = ISNULL(AppEnabled,0) FROM @AppSec;
	
	SELECT @LoginFullName = CASE WHEN CHARINDEX('/',@LoginName) > 0 THEN @LoginName
								 ELSE CASE WHEN CHARINDEX('\',@LoginName) > 0 THEN 
										@LoginName
									  ELSE
										HOST_NAME() + '\' + @LoginName
									  END
								END
	IF @LoginName IS NOT NULL
  	BEGIN
  		DECLARE @SID VARBINARY(85),@IsWebAppAccessGranted INT;
  		IF @IsAppSecEnabled = 0	
  		BEGIN
  			SELECT TOP 1 Name, [Type],Type_Desc, SID, Principal_ID,IS_SRVROLEMEMBER('sysadmin',Name) IsSysAdmin 
  			FROM sys.server_principals SP (NOLOCK) WHERE is_disabled = 0 AND LOWER(NAME) COLLATE DATABASE_DEFAULT = LOWER(@LoginFullName) COLLATE DATABASE_DEFAULT AND [Type] IN ('U','G');
  		END
  		ELSE IF @IsAppSecEnabled = 1
  		BEGIN
  			--getting the sid for the user
  			SELECT @SID = [SID]
  			FROM sys.server_principals SP (NOLOCK) WHERE is_disabled = 0 AND LOWER(NAME) COLLATE DATABASE_DEFAULT = LOWER(@LoginFullName) COLLATE DATABASE_DEFAULT AND [Type] IN ('U','G');
  			
  			IF((SELECT COUNT(0) FROM Permission WHERE LoginSID = @SID) > 0)
  			BEGIN
  				SELECT @IsWebAppAccessGranted = ISNULL(WebAppPermission,0) FROM Permission (NOLOCK) WHERE LoginSID = @SID AND [Enabled] = 1 ;
  				IF @IsWebAppAccessGranted = 1
				BEGIN
					SELECT TOP 1 Name, [Type],Type_Desc, [SID], Principal_ID,IS_SRVROLEMEMBER('sysadmin',Name) IsSysAdmin 
  					FROM sys.server_principals SP (NOLOCK) WHERE is_disabled = 0 AND LOWER(NAME) COLLATE DATABASE_DEFAULT = LOWER(@LoginFullName) COLLATE DATABASE_DEFAULT AND [Type] IN ('U','G');	  			
  				END									  			
  			END
  			ELSE
  			BEGIN
  				SELECT TOP 1 Name, [Type],Type_Desc, SID, Principal_ID,IS_SRVROLEMEMBER('sysadmin',Name) IsSysAdmin 
  				FROM sys.server_principals SP (NOLOCK) WHERE is_disabled = 0 AND LOWER(NAME) COLLATE DATABASE_DEFAULT = LOWER(@LoginFullName) COLLATE DATABASE_DEFAULT AND [Type] IN ('U','G');	
  			END		
  			
  			
  			
  		END
  		
  		
  		--checking if a principal exists in user or group role
  		--SELECT TOP 1 Name, [Type],Type_Desc, SID, Principal_ID,IS_SRVROLEMEMBER('sysadmin',Name) IsSysAdmin 
  		--	FROM sys.server_principals SP 
  		--	LEFT OUTER JOIN Permission P ON SP.[SID] collate database_default = P.LoginSID collate database_default WHERE P.[Enabled ]= 1
  		--	AND is_disabled = 0 AND LOWER(NAME) COLLATE DATABASE_DEFAULT = LOWER(@LoginFullName) COLLATE DATABASE_DEFAULT AND [Type] IN ('U','G')
  		
  	END  	
END