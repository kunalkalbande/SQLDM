if (object_id('[p_GetListOfCounters]') is not null)
begin
drop procedure [p_GetListOfCounters]
end
go
create proc [dbo].[p_GetListOfCounters] as 
begin

select CounterName, CounterFriendlyName, CounterType as Source 
from CounterMasterList where AvailableInCustomReport = 1
union
select replace(mi.Name,' ','_') as CounterName,
mi.Name as CounterFriendlyName, 
2 as Source 
from CustomCounterDefinition  ccd
left outer join MetricInfo mi on mi.Metric = ccd.Metric
where ccd.Enabled = 1
order by Source,CounterFriendlyName asc

end