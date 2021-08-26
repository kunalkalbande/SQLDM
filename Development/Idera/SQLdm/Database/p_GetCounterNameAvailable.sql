if (object_id('p_GetCounterNameAvailable') is not null)
begin
drop procedure p_GetCounterNameAvailable
end
go

CREATE PROCEDURE [dbo].p_GetCounterNameAvailable(
	@CounterName nvarchar(255),
	@Available bit output
)
AS
begin
	declare @err int
	declare @Usages int
	
	select @Usages = count([Name]) 
		from [MetricInfo] 
			left join [MetricMetaData] 
			on [MetricMetaData].[Metric] = [MetricInfo].[Metric]
		where 
			lower([MetricInfo].[Name]) = lower(@CounterName) 
			and [MetricMetaData].[Deleted] <> 1
						
	if (@Usages > 0)
		select @Available = 0
	else
		select @Available = 1

	select @err = @@error
	return @err
END	

