if (object_id('[p_DeleteCountersFromCustomReport]') is not null)
begin
drop procedure [p_DeleteCountersFromCustomReport]
end
go

create proc p_DeleteCountersFromCustomReport( @reportName nvarchar(255))
as
begin
delete from crc
from CustomReportsCounters crc inner join CustomReports cr on  cr.ID = crc.ID
where lower(cr.reportName) = lower(@reportName)
end
