if (object_id('p_updateDefaultTemplateId') is not null)
begin
	DROP PROCEDURE [p_updateDefaultTemplateId]
end
go
CREATE PROCEDURE [p_updateDefaultTemplateId]
AS
BEGIN

CREATE TABLE #MonitoredSQLServers (      
		ID INT IDENTITY(1,1), 
		SQLServerID int
		);

INSERT INTO #MonitoredSQLServers  
SELECT SQLServerID FROM dbo.MonitoredSQLServers

DECLARE @Index INT = 1
DECLARE @TotalRows INT
SELECT @TotalRows = COUNT(1) FROM #MonitoredSQLServers

DECLARE @TemplateID int
DECLARE @SQLServerID int

WHILE @Index <= @TotalRows
	BEGIN
	
	 SELECT 
	 @TemplateID = TemplateID
	 FROM dbo.AlertTemplateLookup AL WHERE AL.Name = 'Default Template' --AND AL.Default = 1

	  SELECT 
	 @SQLServerID = SQLServerID
	 FROM #MonitoredSQLServers WHERE ID = @Index

	 EXEC [dbo].[p_AddAlertInstanceTemplate]
			@TemplateID ,
			@SQLServerID

	 SET @Index = @Index + 1
	END

DROP TABLE #MonitoredSQLServers

END