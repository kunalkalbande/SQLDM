-- Get Consolidated Instance Overview
-- example
-- EXEC [dbo].[p_GetPreviousAnalysisInfo]  @InstanceId=1

if (object_id('p_GetPreviousAnalysisInfo') is not null)
begin
drop procedure p_GetPreviousAnalysisInfo
end
go
create procedure [dbo].[p_GetPreviousAnalysisInfo]
		(@InstanceId nvarchar (max))
as
begin
	select Top 1
		[dbo].[PrescriptiveAnalysis].[AnalysisID],
		[dbo].[PrescriptiveAnalysis].[UTCAnalysisStartTime],
		DateDiff(SECOND,[dbo].[PrescriptiveAnalysis].[UTCAnalysisStartTime],[dbo].[PrescriptiveAnalysis].[UTCAnalysisCompleteTime]) as seconds,
		[dbo].[PrescriptiveAnalysisType].[AnalysisType]
		from [dbo].[PrescriptiveAnalysis],[dbo].[PrescriptiveAnalysisType] where [dbo].[PrescriptiveAnalysis].[SQLServerID] = @InstanceId and [dbo].[PrescriptiveAnalysis].[AnalysisTypeID]=[dbo].[PrescriptiveAnalysisType].[AnalysisTypeID]
		order by [dbo].[PrescriptiveAnalysis].[AnalysisID] desc
end

GO