IF (OBJECT_ID('p_AuthenticateWebConsoleUserAppSecOn') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AuthenticateWebConsoleUserAppSecOn
END
GO
CREATE PROC p_AuthenticateWebConsoleUserAppSecOn
AS
DECLARE @IsSysAdmin INT,@SID VARBINARY(MAX),@WebAppSecurityEnabled INT
SELECT @IsSysAdmin = IS_SRVROLEMEMBER ('sysadmin'),@SID = SUSER_SID();
--SELECT @IsSysAdmin = 0,@SID = SUSER_SID();

IF(@IsSysAdmin = 0)
BEGIN
	IF((SELECT COUNT(0) FROM Permission WHERE LoginSID = @SID ) > 0)
	BEGIN
		SELECT @WebAppSecurityEnabled = ISNULL(WebAppPermission,0) FROM Permission (NOLOCK) WHERE 
		LoginSID = @SID AND [Enabled] = 1
	END
	ELSE
	BEGIN
		SELECT @WebAppSecurityEnabled = 0
	END
END

IF @IsSysAdmin = 1
BEGIN
	SELECT 1 IsAuthentic;
END
ELSE IF (@WebAppSecurityEnabled = 1)
BEGIN
	SELECT 1 IsAuthentic;
END
ELSE
BEGIN 
	SELECT 0 IsAuthentic;
END


Go

grant EXECUTE on [p_AuthenticateWebConsoleUserAppSecOn] to [SQLdmConsoleUser]