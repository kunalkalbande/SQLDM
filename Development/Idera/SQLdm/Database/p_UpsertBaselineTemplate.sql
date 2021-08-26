IF (OBJECT_ID('p_UpsertBaselineTemplate') is not null)
BEGIN
	DROP PROCEDURE [p_UpsertBaselineTemplate]
END

GO

CREATE PROCEDURE [dbo].[p_UpsertBaselineTemplate]
(	
	@SqlServerId int,
	@baselineConfigList xml
)
AS
BEGIN
	if(@baselineConfigList is NOT NULL)
		BEGIN
		INSERT INTO [dbo].[BaselineTemplates]
			   ([SQLServerID]
			,[Template]
			,[BaselineName]
			,[Active])		 
		 SELECT	
			@SqlServerId,	 			
			A.B.value('(TemplateXML)[1]','nvarchar(1024)') AS [Template],
			A.B.value('(Name/text())[1]','nvarchar(500)') AS [BaselineName] ,
			'1'
		 FROM
			@baselineConfigList.nodes('/ArrayOfBaselineConfiguration/BaselineConfiguration') A(B)
		WHERE
			A.B.value('(TemplateID)[1]','int') < 0 
						
		UPDATE [dbo].[BaselineTemplates]
   SET Active = '0'
 WHERE TemplateID IN (SELECT							
			A.B.value('(TemplateID)[1]','int') AS [TemplateID] --TAG			
		 FROM
			@baselineConfigList.nodes('/ArrayOfBaselineConfiguration/BaselineConfiguration') A(B)
		WHERE
			A.B.value('(TemplateID)[1]','int') > 0 AND A.B.value('(IsChanged)[1]','nvarchar(20)') = 'true'
		)
		INSERT INTO [dbo].[BaselineTemplates]
			   ([SQLServerID]
			,[Template]
			,[BaselineName]
			,[Active])		 
		 SELECT	
			@SqlServerId,	 			
			A.B.value('(TemplateXML)[1]','nvarchar(1024)') AS [Template],
			A.B.value('(Name/text())[1]','nvarchar(500)') AS [BaselineName] ,
			A.B.value('(Active)[1]','bit') AS [IsActive]
		 FROM
			@baselineConfigList.nodes('/ArrayOfBaselineConfiguration/BaselineConfiguration') A(B)
		WHERE
			A.B.value('(TemplateID)[1]','int') > 0 AND A.B.value('(IsChanged)[1]','nvarchar(20)') = 'true'
		END

END