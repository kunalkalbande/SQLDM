if (object_id('fn_GetChartTimeLabels') is not null)
begin
drop function fn_GetChartTimeLabels
end
go
CREATE function [fn_GetChartTimeLabels](@sampleTime datetime, @secondsInBucket int, @secondsInGraph int) 
returns @xAxisLabels table (labelTime datetime)
as
begin
declare @endOfGraphTime datetime
declare @startOfGraphTime datetime
declare @bucketTime datetime

set @endOfGraphTime = @sampleTime

select @startOfGraphTime= dateadd(s, -@secondsInGraph,@endOfGraphTime)

while @secondsInGraph > 0
begin
	select @bucketTime = dateadd(s, @secondsInGraph, @startOfGraphTime)
	if DATEPART(s,@bucketTime) % @secondsInBucket = 0
	begin
		insert @xAxisLabels
		select  dateadd(ms,-datepart(ms, @bucketTime),@bucketTime)
	end
	set @secondsInGraph = @secondsInGraph - 1
end
RETURN

end



GO


