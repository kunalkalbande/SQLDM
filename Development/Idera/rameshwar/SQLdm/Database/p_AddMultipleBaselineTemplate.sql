IF (object_id('[p_AddMultipleBaselineTemplate]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_AddMultipleBaselineTemplate]
END
GO

CREATE PROCEDURE [dbo].[p_AddMultipleBaselineTemplate]
	@SQLServerId int,
	@AllConfigs XML
AS
BEGIN
	UPDATE BaselineTemplates SET Active = 0 WHERE SQLServerID = @SQLServerId 

	INSERT INTO BaselineTemplates ([SQLServerID], [Template] ,[BaselineName], [Active])
	SELECT @SQLServerId AS SQLServerID,'<?xml version="1.0" encoding="utf-16"?>'+CAST(mb.Template AS VARCHAR(1024)) AS Template,mb.BaselineName,1 AS Active FROM (SELECT B.value('(Name)[1]','nvarchar(500)') AS BaselineName,B.query('./TemplateXML/BaselineTemplate') AS Template FROM @AllConfigs.nodes('/*/*') AS A(B)) AS mb
END

GO