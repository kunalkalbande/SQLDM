
if (object_id('p_UpdateMetricInfo') is not null)
begin
drop procedure [p_UpdateMetricInfo]
end
go
CREATE PROCEDURE [dbo].[p_UpdateMetricInfo](
	@Metric int,
	@Comments nvarchar(4000),
	@Rank int
)
AS
begin

declare @e int

UPDATE	[MetricInfo] 
	SET [Comments] = @Comments,[Rank]=@Rank
WHERE ([Metric] = @Metric)

SELECT @e = @@error

return @e

end
