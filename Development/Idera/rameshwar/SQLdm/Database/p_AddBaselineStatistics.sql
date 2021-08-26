
IF (object_id('p_AddBaselineStatistics') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddBaselineStatistics
END
GO

CREATE PROCEDURE [dbo].[p_AddBaselineStatistics]
(
	@UTCCalculation datetime,
	@SQLServerID  int,
	@TemplateID	  int,
	@MetricID     int,
	@Mean         decimal(38,5),
	@StdDeviation decimal(38,5),
	@Min          decimal(38,5),
	@Max          decimal(38,5),
	@Count        bigint
)
AS
BEGIN

	INSERT INTO BaselineStatistics VALUES (@UTCCalculation, @SQLServerID, @TemplateID, @MetricID, @Mean, @StdDeviation, @Min, @Max, @Count)

END
