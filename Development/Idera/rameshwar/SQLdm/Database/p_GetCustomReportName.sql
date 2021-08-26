if (object_id('[p_GetCustomReportName]') is not null)
begin
drop procedure [p_GetCustomReportName]
end
go

create proc [dbo].[p_GetCustomReportName](
@reportName nvarchar(255) out
)
as
begin
declare @report nvarchar(255)

select @report = reportName from CustomReports where lower(reportName) = lower(@reportName)
if @report is not null 
	select @reportName = @report

end

--declare @a nvarchar(255)
--select @a = 'AAAAA'
--execute [p_GetCustomReportName] @a out
--
--select @a