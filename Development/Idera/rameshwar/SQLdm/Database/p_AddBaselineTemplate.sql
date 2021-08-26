
IF (object_id('p_AddBaselineTemplate') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddBaselineTemplate
END
GO

CREATE PROCEDURE [dbo].[p_AddBaselineTemplate]
(
	@SQLServerID  int,
	@Template     nvarchar(1024)
)
AS
BEGIN

DECLARE @oldtemmplateid int
SELECT @oldtemmplateid = TemplateID FROM BaselineTemplates WHERE SQLServerID = @SQLServerID and BaselineName='Default' and Active='1'

	IF @Template IS NOT NULL
	BEGIN
	
		IF @oldtemmplateid IS NULL OR NOT EXISTS (SELECT TemplateID FROM BaselineTemplates WHERE TemplateID = @oldtemmplateid AND Template = @Template AND BaselineName='Default' AND Active='1')
		BEGIN
		UPDATE [dbo].[BaselineTemplates]
			SET [Active] = '0'
			WHERE TemplateID =(select TemplateID from BaselineTemplates (NOLOCK) where SQLServerID = @SQLServerID and BaselineName = 'Default' and Active='1')
	
			INSERT INTO [dbo].[BaselineTemplates] ([SQLServerID]
           ,[Template]
           ,[BaselineName]
           ,[Active])
		   VALUES (@SQLServerID, @Template,'Default','1')					
		END
	
	END

END
 
