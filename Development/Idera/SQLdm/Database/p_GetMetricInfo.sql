if (object_id('p_GetMetricInfo') is not null)
begin
drop procedure [p_GetMetricInfo]
end
go
CREATE PROCEDURE [dbo].[p_GetMetricInfo](
	@Metric int = NULL
)
AS
begin

declare @e int

SELECT	[Metric],[Rank],[Category],[Name],[Description],[Comments]
FROM [MetricInfo] (NOLOCK)
WHERE (@Metric is null or [Metric] = @Metric)

SELECT @e = @@error

return @e

end
