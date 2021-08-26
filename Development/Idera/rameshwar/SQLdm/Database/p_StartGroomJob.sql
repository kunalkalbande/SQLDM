if (object_id('p_StartGroomJob') is not null)
begin
drop procedure [p_StartGroomJob]
end
go

create procedure [p_StartGroomJob]
as
begin
	DECLARE @jobname nvarchar(256)
	SELECT @jobname = 'Groom ' + DB_NAME()

	EXEC msdb.dbo.sp_start_job @job_name=@jobname
end