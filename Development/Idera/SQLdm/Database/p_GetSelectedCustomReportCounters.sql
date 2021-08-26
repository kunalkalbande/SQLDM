if (object_id('[p_GetSelectedCustomReportCounters]') is not null)
begin
drop procedure [p_GetSelectedCustomReportCounters]
end
go

create proc p_GetSelectedCustomReportCounters(@reportName nvarchar(255) = null)
as
begin
--declare @ID int
--select @ID = ID from CustomReports where reportName = @reportName
--
--select @ID as ID, GraphNumber, CounterShortDescription, CounterName, Aggregation
-- from CustomReportsCounters
--where ID = @ID

select crc.ID, crc.GraphNumber, crc.CounterShortDescription, crc.CounterName, crc.Aggregation, cr.reportName, cml.CounterType, cr.reportShortDescription 
from CustomReports cr inner join CustomReportsCounters crc on cr.ID = crc.ID
inner join CounterMasterList cml on crc.CounterName = cml.CounterName
where cr.reportName = coalesce(@reportName,cr.reportName)
union
select crc.ID, crc.GraphNumber, crc.CounterShortDescription, replace(mi.Name, ' ', '_') as CounterName, crc.Aggregation, cr.reportName, 2, cr.reportShortDescription 
from CustomReports cr inner join CustomReportsCounters crc on cr.ID = crc.ID
inner join MetricInfo mi on replace(mi.Name, ' ', '_') = crc.CounterName
where cr.reportName = coalesce(@reportName,cr.reportName)
and mi.Metric >= 1000
order by crc.GraphNumber asc

end
