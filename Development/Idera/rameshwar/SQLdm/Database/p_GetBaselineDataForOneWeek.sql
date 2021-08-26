IF (object_id('p_GetBaselineDataForOneWeek') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetBaselineDataForOneWeek]
END
GO

CREATE PROCEDURE [dbo].[p_GetBaselineDataForOneWeek]
	@SQLServerId int,
	@ItemId int, 
	@StartDate datetime,
	@EndDate datetime
AS
BEGIN
	DECLARE @tableName nvarchar(256)
	DECLARE @columnName nvarchar(256)

	SELECT @tableName=StatisticTable,@columnName=MetricValue FROM BaselineMetaData WHERE ItemID = @ItemId

	DECLARE @sqlQuery nvarchar(512)
	SET @sqlQuery = 'SELECT '+@columnName+' AS Value,UTCCollectionDateTime FROM '+@tableName+' WHERE SQLServerID = '+CONVERT(nvarchar(10),@SQLServerId)+' and UTCCollectionDateTime > '''+Convert(nvarchar(20),@StartDate,120)+''' and UTCCollectionDateTime <= '''+Convert(nvarchar(20),@EndDate,120)+''' and '+@columnName+' IS NOT NULL ORDER BY UTCCollectionDateTime'
	
	EXECUTE sp_executesql @sqlQuery
END

GO