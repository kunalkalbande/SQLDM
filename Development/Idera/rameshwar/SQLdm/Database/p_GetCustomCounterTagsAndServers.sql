if (object_id('p_GetCustomCounterTagsAndServers') is not null)
begin
drop procedure p_GetCustomCounterTagsAndServers
end
go

create procedure [dbo].[p_GetCustomCounterTagsAndServers]
(
	@Metric int
)
AS
begin
	declare @e int

	select [TagId] from [CustomCounterTags] where [Metric] = @Metric
	select [SQLServerID] from [CustomCounterMap] where [Metric] = @Metric

	set @e = @@ERROR
	return @e
end	