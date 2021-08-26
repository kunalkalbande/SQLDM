IF (OBJECT_ID('p_GetBaselineTemplatesById') is not null)
BEGIN
	DROP PROCEDURE [p_GetBaselineTemplatesById]
END

GO

CREATE PROCEDURE [dbo].[p_GetBaselineTemplatesById] (@InstanceID INT)
AS

BEGIN

SELECT 
	[Template],
	[TemplateID],
	[BaselineName],
	[Active]	
FROM 
	BaselineTemplates	 
WHERE SQLServerID = @InstanceID and Active = '1' and BaselineName != 'Default'
ORDER BY TemplateID DESC;

END
