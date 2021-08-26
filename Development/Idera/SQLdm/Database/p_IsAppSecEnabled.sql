--p_IsAppSecEnabled
IF (OBJECT_ID('p_IsAppSecEnabled') IS NOT NULL)
BEGIN
DROP PROCEDURE p_IsAppSecEnabled
END
GO    
CREATE PROC p_IsAppSecEnabled
AS
BEGIN
	SELECT ISNULL(Internal_Value,0) AppSecurityEnabled FROM RepositoryInfo (nolock) WHERE Name COLLATE database_default = 'ApplicationSecurityEnabled';
END