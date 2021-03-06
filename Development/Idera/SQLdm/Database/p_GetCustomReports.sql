if (object_id('[p_GetCustomReports]') is not null)
begin
	drop procedure [p_GetCustomReports]
end
go

Create proc [dbo].[p_GetCustomReports](@reportName nvarchar(255) = null)
as
begin
	select reportName, reportShortDescription, reportText, ShowTopServers
	from CustomReports
	where lower(reportName) = isnull(lower(@reportName), lower(reportName))
end

