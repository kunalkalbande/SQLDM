if (object_id('p_StartAggregationJob') is not null)
begin
drop procedure [p_StartAggregationJob]
end
go

create procedure [p_StartAggregationJob]
as
begin
	DECLARE @jobname nvarchar(256)
	SELECT @jobname = 'Aggregate Data ' + DB_NAME()

	EXEC msdb.dbo.sp_start_job @job_name=@jobname
end