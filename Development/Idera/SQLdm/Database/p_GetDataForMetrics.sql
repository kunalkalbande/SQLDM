IF (object_id('[p_GetDataForMetrics]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetDataForMetrics]
END
GO

CREATE PROCEDURE [dbo].[p_GetDataForMetrics]
	@SQLServerId int,
	@ItemId int, 
	@WeekCount int
AS
BEGIN
	DECLARE @tableName nvarchar(256)
	DECLARE @columnName nvarchar(256)

	SELECT @tableName=StatisticTable,@columnName=MetricValue FROM BaselineMetaData WHERE ItemID = @ItemId

	DECLARE @sqlQuery nvarchar(512)
	SET @sqlQuery = 'SELECT '+@columnName+' AS Value,UTCCollectionDateTime FROM '+@tableName+' WHERE SQLServerID = '+CONVERT(nvarchar(10),@SQLServerId)+' and UTCCollectionDateTime >= (GETUTCDATE()-'+Convert(nvarchar(10),6*@WeekCount)+') and '+@columnName+' IS NOT NULL'
	print @sqlQuery
	EXECUTE sp_executesql @sqlQuery
END

GO