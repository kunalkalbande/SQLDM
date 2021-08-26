if (object_id('p_GetServerTimeline') is not null)
begin
drop procedure [p_GetServerTimeline]
end
go

create procedure [p_GetServerTimeline]
	@ServerName nvarchar(256),
	@UTCStart DateTime,
	@UTCEnd DateTime
as
begin
	
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	select 
		top 1000
		AlertID,
		UTCOccurrenceDateTime,
		DatabaseName,
		TableName,
		Active,
		Category,
		A.Metric,
		Severity,
		Rank,
		StateEvent,
		Value,
		Heading
		
	from Alerts A
		inner join MetricInfo I
			on I.Metric = A.Metric
		
	where 
		--Active = 1
		ServerName = @ServerName				
		and UTCOccurrenceDateTime between @UTCStart and @UTCEnd

	order by UTCOccurrenceDateTime desc, Severity desc, Rank desc
	
end