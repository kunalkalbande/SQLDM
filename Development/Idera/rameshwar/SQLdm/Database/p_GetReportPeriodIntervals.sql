if (object_id('p_GetReportPeriodIntervals') is not null)
begin
drop procedure [p_GetReportPeriodIntervals]
end
go

CREATE PROCEDURE p_GetReportPeriodIntervals
	-- If you want a report to have an abscure period\interval then put
	-- the configuration in the reportperiodIntervals table
    -- and call this proc with that reports enum report number as the second parameter
	@period int,
	@reportNumber int = -1 -- -1 is for all defaults
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select ri.Value, ri.Label from ReportPeriodIntervals pi (nolock)
	inner join ReportIntervals ri (nolock) on ri.Value = pi.IntervalValue
	inner join ReportPeriods rp (nolock) on rp.Value = pi.PeriodValue
	where pi.PeriodValue = @period 
	and pi.ReportNumber = coalesce(@reportNumber, pi.ReportNumber)
END
GO
