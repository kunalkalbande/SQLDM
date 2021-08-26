if (object_id('p_GetPreviousAnalysisActivitySnapshotDateTime') is not null)
begin
drop procedure [p_GetPreviousAnalysisActivitySnapshotDateTime]
end
go
create procedure [dbo].[p_GetPreviousAnalysisActivitySnapshotDateTime]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime
	--@RefreshType int = 0
as
begin
set transaction isolation level read uncommitted
declare @err int

select 
	max([UTCAnalysisCompleteTime])
from 
	PrescriptiveAnalysis
where 
	[UTCAnalysisCompleteTime] < @UTCSnapshotCollectionDateTime
	and [SQLServerID] = @SQLServerID
    AND RecommendationCount > 0
	--and isnull(RefreshType, 0) = @RefreshType

select @err = @@error
return @err
end