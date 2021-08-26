--SQL DM 6.0 (Vineet Kumar)
--For defect fix DE41829. This function is created to avoid the overhead of creating a table with start and end time labels for charts.
--fn_GetChartTimeLabels exists but it gives only starttime. It enhanced performance

if (object_id('fn_GetChartStartAndEndTimeLabels') is not null)
begin
drop function [fn_GetChartStartAndEndTimeLabels]
end
go
CREATE function [fn_GetChartStartAndEndTimeLabels](@sampleTime datetime, @secondsInBucket int, @secondsInGraph int) 
returns @xAxisLabels table (labelStartTime datetime, labelEndTime datetime)
as
begin
declare @endOfGraphTime datetime
declare @startOfGraphTime datetime
declare @bucketTime datetime

set @endOfGraphTime = @sampleTime

select @startOfGraphTime= dateadd(s, -@secondsInGraph,@endOfGraphTime)
while @secondsInGraph > @secondsInBucket
begin
	select @bucketTime = dateadd(s, @secondsInGraph, @startOfGraphTime)
	if DATEPART(s,@bucketTime) % @secondsInBucket = 0
	begin
		insert @xAxisLabels
		select  dateadd(ms,-datepart(ms, @bucketTime)-@secondsInBucket*1000,@bucketTime), dateadd(ms,-datepart(ms, @bucketTime),@bucketTime)
		SET @secondsInGraph = @secondsInGraph - @secondsInBucket
	end
	ELSE
	BEGIN
	set @secondsInGraph = @secondsInGraph - 1
	END
end
RETURN

end
