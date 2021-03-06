IF (object_id('p_AddAlertInstanceTemplate') is not null)
BEGIN
DROP PROCEDURE p_AddAlertInstanceTemplate
END
GO
create PROCEDURE [dbo].[p_AddAlertInstanceTemplate]
@TemplateID int,
@SQLServerID VARCHAR(2000)
AS
	BEGIN
		SELECT [Index],[Value] AS 'SQLServerID'
		INTO #TempTable
		FROM fn_Split(@SQLServerID,',')

		WHILE (SELECT COUNT(SQLServerID) FROM #TempTable) > 0
		BEGIN
			DECLARE @ServerID INT
			SET @ServerID = (SELECT TOP(1)SQLServerID FROM #TempTable)

			IF NOT EXISTS(SELECT SQLServerID FROM AlertInstanceTemplate WHERE SQLServerID = @ServerID)
			BEGIN
				INSERT INTO AlertInstanceTemplate(SQLServerID,TemplateID)VALUES(@ServerID,@TemplateID)
			END
			ELSE
				BEGIN
					UPDATE AlertInstanceTemplate 
					SET TemplateID=@TemplateID 
					WHERE SQLServerID=@ServerID 
				END
			DELETE FROM #TempTable WHERE SQLServerID = @ServerID	
		END

		DROP TABLE #TempTable
END