if (object_id('[p_UpdateCustomReport]') is not null)
begin
drop procedure [p_UpdateCustomReport]
end
go

CREATE proc [dbo].[p_UpdateCustomReport](
@reportID int = null OUT, 
@operation int,
@reportName nvarchar(255) = null,
@reportShortDescription nvarchar(255) = null,
@reportText nvarchar(4000) = null,
@showTopServers bit
) as
begin
declare @ptr varbinary(16), @len int

if @reportID is null and @operation <> 5
	select @reportID = ID 
	from CustomReports 
	where reportName = @reportName

if @operation = 1
	delete from CustomReports where ID = @reportID
if @operation = 2 
	begin
		select @ptr = TEXTPTR(reportText), @len = datalength(reportText)/2 
		from CustomReports 
		where ID = @reportID

		WriteText CustomReports.reportText @ptr null
		update CustomReports set reportText = @reportText, ShowTopServers = @showTopServers where ID = @reportID
	end
if @operation = 3
	begin
		select @ptr = TEXTPTR(reportText), @len = datalength(reportText)/2 
		from CustomReports 
		where ID = @reportID

		UpdateText CustomReports.reportText @ptr @len 0 @reportText
	end
if @operation = 4
	begin
		update CustomReports 
		set reportName = @reportName, reportShortDescription = @reportShortDescription, ShowTopServers = @showTopServers
		where ID = @reportID
	end
if @operation = 5
	begin
		insert into CustomReports (reportName, reportShortDescription, reportText, ShowTopServers)values (@reportName, @reportShortDescription, @reportText, @showTopServers)
		SELECT @reportID = SCOPE_IDENTITY()
	end
end